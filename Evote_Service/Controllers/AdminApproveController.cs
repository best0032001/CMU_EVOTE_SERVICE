
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
    public class AdminApproveController : ITSCController
    {
        private IAdminRepository _IAdminRepository;
        public AdminApproveController(ILogger<ITSCController> logger, IAdminRepository IAdminRepository, IHttpClientFactory clientFactory, IWebHostEnvironment env, IEmailRepository emailRepository)
        {
        
            this.loadConfig(logger, clientFactory, env); 
            _emailRepository = emailRepository;
            _IAdminRepository = IAdminRepository;
        }

        [HttpGet("v1/Admin/Approve")]
        [ProducesResponseType(typeof(UserEntity), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIModel), (int)HttpStatusCode.ServiceUnavailable)]
        public async Task<IActionResult> getUserApprove()
        {
            String Cmuaccount = "";
            String action = "AdminApproveController.getUserApprove";
            try
            {
               
                Cmuaccount = await getCmuaccount();
                if (Cmuaccount == "unauthorized") { return Unauthorized(); }

                List<UserEntity> userEntities = await _IAdminRepository.getUserWaitForApprove(Cmuaccount);
                if (userEntities == null) { return Unauthorized(); }
                APIModel aPIModel = new APIModel();
                aPIModel.data = userEntities;
                aPIModel.message = "Success";
                return StatusCodeITSC("CMU", "", Cmuaccount, action, 200, aPIModel);

            }
            catch (Exception ex) { return StatusErrorITSC("CMU", "", Cmuaccount, action, ex); }
        }

        [HttpPut("v1/Admin/Approve")]
        [ProducesResponseType(typeof(UserEntity), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> adminApprove([FromQuery] Int32 userEntityId)
        {
            String Cmuaccount = "";
            String action = "AdminApproveController.adminApprove";
            try
            {
               
                Cmuaccount = await getCmuaccount();
                if (Cmuaccount == "unauthorized") { return Unauthorized(); }
                if (userEntityId == 0) { return BadRequest(); }

           

                List<UserEntity> userEntities = await _IAdminRepository.adminApprove(Cmuaccount, userEntityId, getClientIP());
                if (userEntities == null) { return Unauthorized(); }
                APIModel aPIModel = new APIModel();
                aPIModel.data = userEntities;
                aPIModel.message = "Success";

                return StatusCodeITSC("CMU", "", Cmuaccount, action, 200, aPIModel);
            }
            catch (Exception ex) {
                return StatusErrorITSC("CMU", "", Cmuaccount, action, ex);
            }
        }

        [HttpPut("v1/Admin/NotApprove")]
        [ProducesResponseType(typeof(UserEntity), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> adminNotApprove([FromBody] AdminApproveModelView data)
        {
            String Cmuaccount = "";
            try
            {
                Cmuaccount = await getCmuaccount();
                if (Cmuaccount == "unauthorized") { return Unauthorized(); }
                if (data.userEntityId == 0 || data.comment == "") { return BadRequest(); }

                List<UserEntity> userEntities = await _IAdminRepository.adminNotApprove(Cmuaccount, data, getClientIP());
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
