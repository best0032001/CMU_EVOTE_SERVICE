
using Evote_Service.Model.Entity;
using Evote_Service.Model.Interface;
using Evote_Service.Model.View;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        public RegisterUserController(ILogger<ITSCController> logger, IHttpClientFactory clientFactory, ICheckUserRepository CheckUserRepository, IWebHostEnvironment env)
        {
            this.loadConfig(logger, clientFactory, env); _ICheckUserRepository = CheckUserRepository;
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
                UserModel userModel = await _ICheckUserRepository.GetLineUser(lineId);
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
                if (data.firstName == null || data.lastname == null || data.email == null) { return BadRequest(); }
                if (data.firstName == "" || data.lastname == "" || data.email == "") { return BadRequest(); }
                String firstName = data.firstName; String lastname = data.lastname; String email = data.email;

                UserEntity userEntity = new UserEntity();
                userEntity.FullName = firstName + " " + lastname;
                userEntity.Email = email;
                userEntity.LineId = lineId;
                APIModel aPIModel = new APIModel();
              
                if (await _ICheckUserRepository.RegisLineUser(userEntity) == false)
                {
                    StatusCodeITSC("line", lineId, "", "RegisterUserController.UserRegisLiff", 503, aPIModel);
                    aPIModel.message = "ระบบขัดข้องบันทึกข้อมูลไม่สำเร็จ";
                }
                UserModel userModel = await _ICheckUserRepository.GetLineUser(lineId);
                aPIModel.data = userModel;
                aPIModel.message = "Success";
                return StatusCodeITSC("line", lineId, userModel.Email, "RegisterUserController.UserRegisLiff", 201, aPIModel);
            }
            catch (Exception ex) 
            { 
                return StatusErrorITSC("line", lineId, "", "RegisterUserController.UserRegisLiff", ex); 
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
                    StatusCodeITSC("line", lineId, "", "RegisterUserController.UserSendTel", 503, aPIModel);
                    aPIModel.message = "ระบบขัดข้องบันทึกข้อมูลไม่สำเร็จ";
                }
                UserModel userModel   = await _ICheckUserRepository.GetLineUser(lineId);
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
                    StatusCodeITSC("line", lineId, "", "RegisterUserController.UserSendSMSOTP", 503, aPIModel);
                    aPIModel.message = "รหัส OTP ไม่ถูกต้อง";
                }
                UserModel userModel = await _ICheckUserRepository.GetLineUser(lineId);
                aPIModel.data = userModel;
                aPIModel.message = "Success";
                return StatusCodeITSC("line", lineId, userModel.Email, "RegisterUserController.UserSendSMSOTP", 200, aPIModel);
            }
            catch (Exception ex)
            {
                return StatusErrorITSC("line", lineId, "", "RegisterUserController.UserSendSMSOTP", ex);
            }
        }

        [HttpGet("v1/User/getEmailOTP")]
        public async Task<IActionResult> getEMAILOTP()
        {
            String lineId = "";
            try
            {
                lineId = await getLineUser();
                if (lineId == "unauthorized") { return Unauthorized(); }
                APIModel aPIModel = new APIModel();
                if (await _ICheckUserRepository.getEMAILOTP(lineId) == false)
                {
                    StatusCodeITSC("line", lineId, "", "RegisterUserController.getEMAILOTP", 503, aPIModel);
                    aPIModel.message = "รหัส OTP ไม่ถูกต้อง";
                }
                UserModel userModel = await _ICheckUserRepository.GetLineUser(lineId);
                aPIModel.data = userModel;
                aPIModel.message = "Success";
                return StatusCodeITSC("line", lineId, userModel.Email, "RegisterUserController.getEMAILOTP", 200, aPIModel);
            }
            catch (Exception ex)
            {
                return StatusErrorITSC("line", lineId, "", "RegisterUserController.getEMAILOTP", ex);
            }
        }

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
                    StatusCodeITSC("line", lineId, "", "RegisterUserController.UserSendEmailOTP", 503, aPIModel);
                    aPIModel.message = "รหัส OTP ไม่ถูกต้อง";
                }
                UserModel userModel = await _ICheckUserRepository.GetLineUser(lineId);
                aPIModel.data = userModel;
                aPIModel.message = "Success";
                return StatusCodeITSC("line", lineId, userModel.Email, "RegisterUserController.UserSendEmailOTP", 200, aPIModel);
            }
            catch (Exception ex)
            {
                return StatusErrorITSC("line", lineId, "", "RegisterUserController.UserSendEmailOTP", ex);
            }
        }
    }
}
