
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
    public class AdminApproveController: ITSCController
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

                List<UserEntity> userEntities= await _IAdminRepository.getUserWaitForApprove(Cmuaccount);
                if (userEntities == null) { return Unauthorized(); }
                APIModel aPIModel = new APIModel();
                aPIModel.data = userEntities;
                aPIModel.message = "Success";
                return Ok(aPIModel);
            }
            catch (Exception ex) { return StatusErrorITSC("CMU","" , Cmuaccount, "AdminApproveController.getUserApprove", ex); }
        }

    }
}
