
using Evote_Service.Model.Entity;
using Evote_Service.Model.Interface;
using Evote_Service.Model.Util;
using Evote_Service.Model.View;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Evote_Service.Controllers
{
    [Produces("application/json")]
    [Route("api/")]
    [ApiController]
    public class RegisterUserController : ITSCController
    {
        private ICheckUserRepository _ICheckUserRepository;
        public RegisterUserController(ILogger<ITSCController> logger, IHttpClientFactory clientFactory, ICheckUserRepository CheckUserRepository, IWebHostEnvironment env, IEmailRepository emailRepository)
        {
            this.loadConfig(logger, clientFactory, env); _ICheckUserRepository = CheckUserRepository; _emailRepository = emailRepository;
        }

        [HttpGet("v1/ip")]
        public async Task<IActionResult> ip()
        {

            return Ok(this.getClientIP());
        }

        /// <summary>
        /// Test Name API
        /// </summary>
        /// <param name="id"></param>
        /// <returns>returns</returns>
        [HttpGet("v1/User/liff")]
        [ProducesResponseType(typeof(UserModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> getUserLiff()
        {
            // เมื่อUser เปิด Line UI  ทำการ GetLink Token แล้วส่งมาcheck Service ว่า  LINE ID นี้ Registerระบบหรือยัง
            String lineId = "";
            try
            {
                lineId = await getLineUser();
                if (lineId == "unauthorized") { return Unauthorized(); }
                APIModel aPIModel = new APIModel();
                UserModel userModel = await _ICheckUserRepository.GetLineUserModel(lineId);
                if (userModel == null) { return StatusCode(204); }
                aPIModel.data = userModel;
                aPIModel.message = "Success";
                return Ok(aPIModel);
            }
            catch (Exception ex) { return StatusErrorITSC("line", lineId, "", "CheckUserController.getUserLiff", ex); }
        }

        [HttpPost("v1/User/RegisLiff")]
        [ProducesResponseType(typeof(UserModel), (int)HttpStatusCode.Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIModel), (int)HttpStatusCode.ServiceUnavailable)]
        public async Task<IActionResult> UserRegisLiff([FromBody] UserRegisLiffModelView data)
        {
            String lineId = "";
            try
            {
                lineId = await getLineUser();
                if (lineId == "unauthorized") { return Unauthorized(); }
                if (data.firstName == "" || data.lastName == "") { return BadRequest(); }

                UserEntity userEntity = new UserEntity();
                userEntity.FullName = data.firstName + " " + data.lastName;
                userEntity.Email = "";
                userEntity.LineId = lineId;
                APIModel aPIModel = new APIModel();

                if (await _ICheckUserRepository.RegisLineUser(userEntity) == false)
                {
                    aPIModel.message = "ระบบขัดข้องบันทึกข้อมูลไม่สำเร็จ";
                    return StatusCodeITSC("line", lineId, "", "RegisterUserController.UserRegisLiff", 503, aPIModel);

                }
                UserModel userModel = await _ICheckUserRepository.GetLineUserModel(lineId);
                aPIModel.data = userModel;
                aPIModel.message = "Success";
                return StatusCodeITSC("line", lineId, userModel.Email, "RegisterUserController.UserRegisLiff", 201, aPIModel);
            }
            catch (Exception ex)
            {
                return StatusErrorITSC("line", lineId, "", "RegisterUserController.UserRegisLiff", ex);
            }
        }


        [HttpPost("v1/User/email")]
        [ProducesResponseType(typeof(UserModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIModel), (int)HttpStatusCode.NotAcceptable)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIModel), (int)HttpStatusCode.ServiceUnavailable)]
        public async Task<IActionResult> UserSendemail([FromBody] UserSendModelView data)
        {
            String lineId = "";
            try
            {
                lineId = await getLineUser();
                if (lineId == "unauthorized") { return Unauthorized(); }
                if (data.email == "") { return BadRequest(); }

                APIModel aPIModel = new APIModel();
                if (await _ICheckUserRepository.checkEmail(data.email.Trim()) == false)
                {
                    aPIModel.message = "Email นี้มีผู้ลงทะเบียนแล้ว";
                    return StatusCodeITSC("line", lineId, "", "RegisterUserController.UserSendemail", 406, aPIModel);
                }
                if (await _ICheckUserRepository.UserSendEmail(lineId, data.email.Trim()) == false)
                {
                    aPIModel.message = "ระบบขัดข้องบันทึกข้อมูลไม่สำเร็จ";
                    return StatusCodeITSC("line", lineId, "", "RegisterUserController.UserSendemail", 503, aPIModel);

                }
                UserModel userModel = await _ICheckUserRepository.GetLineUserModel(lineId);
                aPIModel.data = userModel;
                aPIModel.message = "Success";
                return StatusCodeITSC("line", lineId, userModel.Email, "RegisterUserController.UserSendemail", 200, aPIModel);
            }
            catch (Exception ex)
            {
                return StatusErrorITSC("line", lineId, "", "RegisterUserController.UserSendemail", ex);
            }
        }

        [HttpPost("v1/User/Tel")]
        [ProducesResponseType(typeof(UserModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIModel), (int)HttpStatusCode.NotAcceptable)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIModel), (int)HttpStatusCode.ServiceUnavailable)]
        public async Task<IActionResult> UserSendTel([FromBody] UserSendModelView data)
        {
            String lineId = "";
            try
            {
                lineId = await getLineUser();
                if (lineId == "unauthorized") { return Unauthorized(); }
                if (data.tel == "") { return BadRequest(); }

                APIModel aPIModel = new APIModel();
                if (await _ICheckUserRepository.CheckTel(data.tel.Trim()) == false)
                {
                    aPIModel.message = "เบอร์โทรนี้ นี้มีผู้ลงทะเบียนแล้ว";
                    return StatusCodeITSC("line", lineId, "", "RegisterUserController.UserSendTel", 406, aPIModel);
                }
                if (await _ICheckUserRepository.UserSendTel(lineId, data.tel.Trim()) == false)
                {
                    aPIModel.message = "ระบบขัดข้องบันทึกข้อมูลไม่สำเร็จ";
                    return StatusCodeITSC("line", lineId, "", "RegisterUserController.UserSendTel", 503, aPIModel);
                }
                UserModel userModel = await _ICheckUserRepository.GetLineUserModel(lineId);
                aPIModel.data = userModel;
                aPIModel.message = "Success";
                return StatusCodeITSC("line", lineId, userModel.Email, "RegisterUserController.UserSendTel", 200, aPIModel);
            }
            catch (Exception ex)
            {
                return StatusErrorITSC("line", lineId, "", "RegisterUserController.UserSendTel", ex);
            }
        }

        [HttpPost("v1/User/SMSOTP")]
        [ProducesResponseType(typeof(UserModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIModel), (int)HttpStatusCode.ServiceUnavailable)]
        public async Task<IActionResult> UserSendSMSOTP([FromBody] UserSendModelView data)
        {
            String lineId = "";
            try
            {
                lineId = await getLineUser();
                if (lineId == "unauthorized") { return Unauthorized(); }
                if (data.otp == "") { return BadRequest(); }
                APIModel aPIModel = new APIModel();
                if (await _ICheckUserRepository.UserConfirmSMSOTP(lineId, data.otp) == false)
                {
                    aPIModel.message = "รหัส OTP ไม่ถูกต้อง";
                    return StatusCodeITSC("line", lineId, "", "RegisterUserController.UserSendSMSOTP", 503, aPIModel);

                }
                UserModel userModel = await _ICheckUserRepository.GetLineUserModel(lineId);
                aPIModel.data = userModel;
                aPIModel.message = "Success";
                return StatusCodeITSC("line", lineId, userModel.Email, "RegisterUserController.UserSendSMSOTP", 200, aPIModel);
            }
            catch (Exception ex)
            {
                return StatusErrorITSC("line", lineId, "", "RegisterUserController.UserSendSMSOTP", ex);
            }
        }

        //[HttpGet("v1/User/getEmailOTP")]
        //public async Task<IActionResult> getEMAILOTP()
        //{
        //    String lineId = "";
        //    try
        //    {
        //        lineId = await getLineUser();
        //        if (lineId == "unauthorized") { return Unauthorized(); }
        //        APIModel aPIModel = new APIModel();
        //        if (await _ICheckUserRepository.getEMAILOTP(lineId) == false)
        //        {
        //            aPIModel.message = "ระบบขัดข้องไม่สามารถส่ง Email ได้";
        //            return StatusCodeITSC("line", lineId, "", "RegisterUserController.getEMAILOTP", 503, aPIModel);

        //        }
        //        UserModel userModel = await _ICheckUserRepository.GetLineUser(lineId);
        //        aPIModel.data = userModel;
        //        aPIModel.message = "Success";
        //        return StatusCodeITSC("line", lineId, userModel.Email, "RegisterUserController.getEMAILOTP", 200, aPIModel);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusErrorITSC("line", lineId, "", "RegisterUserController.getEMAILOTP", ex);
        //    }
        //}

        [HttpPost("v1/User/EmailOTP")]
        [ProducesResponseType(typeof(UserModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIModel), (int)HttpStatusCode.ServiceUnavailable)]
        public async Task<IActionResult> UserSendEmailOTP([FromBody] UserSendModelView data)
        {
            String lineId = "";
            try
            {
                lineId = await getLineUser();
                if (lineId == "unauthorized") { return Unauthorized(); }
                if (data.otp == "") { return BadRequest(); }
                String otp = data.otp;

                APIModel aPIModel = new APIModel();
                if (await _ICheckUserRepository.UserConfirmEmailOTP(lineId, data.otp) == false)
                {
                    aPIModel.message = "รหัส OTP ไม่ถูกต้อง";
                    return StatusCodeITSC("line", lineId, "", "RegisterUserController.UserSendEmailOTP", 503, aPIModel);

                }
                UserModel userModel = await _ICheckUserRepository.GetLineUserModel(lineId);
                aPIModel.data = userModel;
                aPIModel.message = "Success";
                return StatusCodeITSC("line", lineId, userModel.Email, "RegisterUserController.UserSendEmailOTP", 200, aPIModel);
            }
            catch (Exception ex)
            {
                return StatusErrorITSC("line", lineId, "", "RegisterUserController.UserSendEmailOTP", ex);
            }
        }

        [HttpPost("v1/User/photoId")]
        [ProducesResponseType(typeof(UserModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIModel), (int)HttpStatusCode.ServiceUnavailable)]
        public async Task<IActionResult> UserPostphotoId([FromBody] string body)
        {
            String lineId = "";
            try
            {
                lineId = await getLineUser();
                if (lineId == "unauthorized") { return Unauthorized(); }

                int countFiles = Request.Form.Files.Count;
                if (countFiles != 1) { return BadRequest(); }
                IFormFile file01 = Request.Form.Files["filename"];
                var path = Path.Combine(Directory.GetCurrentDirectory(), "uploadphotoid");
                FileModel fileModel = this.SaveFile(path, file01, 20);
                APIModel aPIModel = new APIModel();
                if (await _ICheckUserRepository.UserPostphotoId(lineId, fileModel) == false)
                {
                    aPIModel.message = "ระบบขัดข้องบันทึกข้อมูลไม่สำเร็จ";
                    return StatusCodeITSC("line", lineId, "", "RegisterUserController.UserPostphotoId", 503, aPIModel);

                }


                UserModel userModel = await _ICheckUserRepository.GetLineUserModel(lineId);
                aPIModel.data = userModel;
                aPIModel.message = "Success";
                return StatusCodeITSC("line", lineId, userModel.Email, "RegisterUserController.UserPostphotoId", 200, aPIModel);
            }
            catch (Exception ex)
            {
                return StatusErrorITSC("line", lineId, "", "RegisterUserController.UserPostphotoId", ex);
            }
        }


        [HttpGet("v1/User/photoId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> UserGetphotoId()
        {
            String lineId = "";
            try
            {
                lineId = await getLineUser();
                if (lineId == "unauthorized") { return Unauthorized(); }
                UserEntity userEntity = await _ICheckUserRepository.GetLineUserEntity(lineId);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "uploadphotoid", userEntity.fileNamePersonalID);
                var memory = this.loadFile(path);
                memory.Position = 0;
                return File(memory, GetContentType(path), Path.GetFileName(path));
            }
            catch (Exception ex)
            {
                return StatusErrorITSC("line", lineId, "", "RegisterUserController.UserGetphotoId", ex);
            }
        }


        [HttpPost("v1/User/kyc")]
        [ProducesResponseType(typeof(UserModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIModel), (int)HttpStatusCode.ServiceUnavailable)]
        public async Task<IActionResult> UserPostKyc([FromBody] string body)
        {
            String lineId = "";
            try
            {
                lineId = await getLineUser();
                if (lineId == "unauthorized") { return Unauthorized(); }

                int countFiles = Request.Form.Files.Count;
                if (countFiles != 1) { return BadRequest(); }
                IFormFile file01 = Request.Form.Files["filename"];
                var path = Path.Combine(Directory.GetCurrentDirectory(), "uploadkyc");
                FileModel fileModel = this.SaveFile(path, file01, 20);
                APIModel aPIModel = new APIModel();
                if (await _ICheckUserRepository.UserPostphotoKyc(lineId, fileModel) == false)
                {
                    aPIModel.message = "ระบบขัดข้องบันทึกข้อมูลไม่สำเร็จ";
                    return StatusCodeITSC("line", lineId, "", "RegisterUserController.UserPostKyc", 503, aPIModel);

                }
                UserModel userModel = await _ICheckUserRepository.GetLineUserModel(lineId);
                aPIModel.data = userModel;
                aPIModel.message = "Success";
                return StatusCodeITSC("line", lineId, userModel.Email, "RegisterUserController.UserPostKyc", 200, aPIModel);
            }
            catch (Exception ex)
            {
                return StatusErrorITSC("line", lineId, "", "RegisterUserController.UserPostKyc", ex);
            }
        }
        [HttpGet("v1/User/kyc")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> UserGetKyc()
        {
            String lineId = "";
            try
            {
                lineId = await getLineUser();
                if (lineId == "unauthorized") { return Unauthorized(); }
                UserEntity userEntity = await _ICheckUserRepository.GetLineUserEntity(lineId);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "uploadkyc", userEntity.fileNameKYC);
                var memory = this.loadFile(path);
                memory.Position = 0;
                return File(memory, GetContentType(path), Path.GetFileName(path));
            }
            catch (Exception ex)
            {
                return StatusErrorITSC("line", lineId, "", "RegisterUserController.UserGetphotoId", ex);
            }
        }

    }
}
