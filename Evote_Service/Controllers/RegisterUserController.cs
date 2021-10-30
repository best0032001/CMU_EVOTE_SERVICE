﻿
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
                aPIModel.message = "ระบบขัดข้องบันทึกข้อมูลไม่สำเร็จ";
                if (await _ICheckUserRepository.RegisLineUser(userEntity) == false)
                {
                    StatusCodeITSC("line", lineId, "", "RegisterUserController.UserRegisLiff", 503, aPIModel);
                }
                aPIModel.data = await _ICheckUserRepository.GetLineUser(lineId);
                aPIModel.message = "Success";
                return StatusCodeITSC("line", lineId, "", "RegisterUserController.UserRegisLiff", 201, aPIModel);
            }
            catch (Exception ex) 
            { 
                return StatusErrorITSC("line", lineId, "", "RegisterUserController.CheckStage", ex); 
            }
        }
    }
}
