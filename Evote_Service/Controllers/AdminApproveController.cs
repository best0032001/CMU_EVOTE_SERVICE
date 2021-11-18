
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
    public class AdminApproveController : ITSCController
    {
        private IAdminRepository _IAdminRepository;
        public AdminApproveController(ILogger<ITSCController> logger, IAdminRepository IAdminRepository, IHttpClientFactory clientFactory, IWebHostEnvironment env, IEmailRepository emailRepository)
        {
            _IAdminRepository = IAdminRepository;
            this.loadConfig(logger, clientFactory, env); _emailRepository = emailRepository;
        }

        [HttpGet("v1/User/Approve")]
        public async Task<IActionResult> getUserApprove()
        {
            String Cmuaccount = "";
            try
            {
               
                Cmuaccount = await getCmuaccount();
                if (Cmuaccount == "unauthorized") { return Unauthorized(); }

                List<UserEntity> userEntities = await _IAdminRepository.getUserWaitForApprove(Cmuaccount);
                if (userEntities == null) { return Unauthorized(); }
                APIModel aPIModel = new APIModel();
                aPIModel.data = userEntities;
                aPIModel.message = "Success";
                return StatusCodeITSC("CMU", "", Cmuaccount, "AdminApproveController.getUserApprove", 200, aPIModel);

            }
            catch (Exception ex) { return StatusErrorITSC("CMU", "", Cmuaccount, "AdminApproveController.getUserApprove", ex); }
        }

        [HttpPut("v1/User/Approve")]
        public async Task<IActionResult> adminApprove([FromBody] string body)
        {
            String Cmuaccount = "";
            try
            {
               
                Cmuaccount = await getCmuaccount();
                if (Cmuaccount == "unauthorized") { return Unauthorized(); }
                dynamic data = JsonConvert.DeserializeObject<dynamic>(body);
                if (data.userEntityId == null) { return BadRequest(); }
                if (data.userEntityId == "") { return BadRequest(); }

                String userEntityId = data.userEntityId;

                List<UserEntity> userEntities = await _IAdminRepository.adminApprove(Cmuaccount, Convert.ToInt32(userEntityId), getClientIP());
                if (userEntities == null) { return Unauthorized(); }
                APIModel aPIModel = new APIModel();
                aPIModel.data = userEntities;
                aPIModel.message = "Success";

                return StatusCodeITSC("CMU", "", Cmuaccount, "AdminApproveController.adminApprove", 200, aPIModel);
            }
            catch (Exception ex) {
                return StatusErrorITSC("CMU", "", Cmuaccount, "AdminApproveController.adminApprove", ex);
            }
        }
        [HttpPut("v1/User/NotApprove")]
        public async Task<IActionResult> adminNotApprove([FromBody] string body)
        {
            String Cmuaccount = "";
            try
            {
                Cmuaccount = await getCmuaccount();
                if (Cmuaccount == "unauthorized") { return Unauthorized(); }
                dynamic data = JsonConvert.DeserializeObject<dynamic>(body);
                if (data.userEntityId == null || data.comment == null) { return BadRequest(); }
                if (data.userEntityId == "" || data.comment == "") { return BadRequest(); }

                String userEntityId = data.userEntityId;
                String Commnet = data.comment;
                List<UserEntity> userEntities = await _IAdminRepository.adminNotApprove(Cmuaccount, Convert.ToInt32(userEntityId), Commnet, getClientIP());
                if (userEntities == null) { return Unauthorized(); }
                APIModel aPIModel = new APIModel();
                aPIModel.data = userEntities;
                aPIModel.message = "Success";
                return StatusCodeITSC("CMU", "", Cmuaccount, "AdminApproveController.adminNotApprove", 200, aPIModel);
            }
            catch (Exception ex) { return StatusErrorITSC("CMU", "", Cmuaccount, "AdminApproveController.adminNotApprove", ex); }
        }
    }
}
