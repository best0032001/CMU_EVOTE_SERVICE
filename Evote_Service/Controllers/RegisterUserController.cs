
using Evote_Service.Model.Entity;
using Evote_Service.Model.Interface;
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
using System.Net.Http;
using System.Threading.Tasks;

namespace Evote_Service.Controllers
{
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
        [HttpGet("v1/User/liff")]
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
        public async Task<IActionResult> UserRegisLiff([FromBody] string body)
        {
            String lineId = "";
            try
            {
                lineId = await getLineUser();
                if (lineId == "unauthorized") { return Unauthorized(); }
                dynamic data = JsonConvert.DeserializeObject<dynamic>(body);
               // if (data.firstName == null || data.lastName == null || data.email == null) { return BadRequest(); }
                if (data.firstName == null || data.lastName == null ) { return BadRequest(); }
              //  if (data.firstName == "" || data.lastName == "" || data.email == "") { return BadRequest(); }
                if (data.firstName == "" || data.lastName == "" ) { return BadRequest(); }
                String firstName = data.firstName; String lastName = data.lastName; 

                UserEntity userEntity = new UserEntity();
                userEntity.FullName = firstName + " " + lastName;
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
        public async Task<IActionResult> UserSendemail([FromBody] string body)
        {
            String lineId = "";
            try
            {
                lineId = await getLineUser();
                if (lineId == "unauthorized") { return Unauthorized(); }
                dynamic data = JsonConvert.DeserializeObject<dynamic>(body);
                if (data.email == null) { return BadRequest(); }
                if (data.email == "") { return BadRequest(); }
                String email = data.email;

                APIModel aPIModel = new APIModel();
                if (await _ICheckUserRepository.UserSendEmail(lineId, email) == false)
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
        public async Task<IActionResult> UserSendTel([FromBody] string body)
        {
            String lineId = "";
            try
            {
                lineId = await getLineUser();
                if (lineId == "unauthorized") { return Unauthorized(); }
                dynamic data = JsonConvert.DeserializeObject<dynamic>(body);
                if (data.tel == null) { return BadRequest(); }
                if (data.tel == "") { return BadRequest(); }
                String tel = data.tel;

                APIModel aPIModel = new APIModel();
                if (await _ICheckUserRepository.UserSendTel(lineId, tel) == false)
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
        public async Task<IActionResult> UserSendSMSOTP([FromBody] string body)
        {
            String lineId = "";
            try
            {
                lineId = await getLineUser();
                if (lineId == "unauthorized") { return Unauthorized(); }
                dynamic data = JsonConvert.DeserializeObject<dynamic>(body);
                if (data.otp == null) { return BadRequest(); }
                if (data.otp == "") { return BadRequest(); }
                String otp = data.otp;

                APIModel aPIModel = new APIModel();
                if (await _ICheckUserRepository.UserConfirmSMSOTP(lineId, otp) == false)
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
        public async Task<IActionResult> UserSendEmailOTP([FromBody] string body)
        {
            String lineId = "";
            try
            {
                lineId = await getLineUser();
                if (lineId == "unauthorized") { return Unauthorized(); }
                dynamic data = JsonConvert.DeserializeObject<dynamic>(body);
                if (data.otp == null) { return BadRequest(); }
                if (data.otp == "") { return BadRequest(); }
                String otp = data.otp;

                APIModel aPIModel = new APIModel();
                if (await _ICheckUserRepository.UserConfirmEmailOTP(lineId, otp) == false)
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
    }
}
