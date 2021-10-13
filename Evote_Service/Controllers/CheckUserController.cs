
using Evote_Service.Model.Interface;
using Evote_Service.Model.View;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Evote_Service.Controllers
{
    [Route("api/")]
    [ApiController]
    public class CheckUserController : ITSCController
    {
        private ICheckUserRepository _ICheckUserRepository;
        public CheckUserController(ILogger<ITSCController> logger, IHttpClientFactory clientFactory, ICheckUserRepository CheckUserRepository, IWebHostEnvironment env)
        {
            this.loadConfig(logger, clientFactory, env); _ICheckUserRepository = CheckUserRepository;
        }
        [HttpGet("v1/User/CheckStage")]
        public async Task<IActionResult> CheckStage()
        {
            try
            {
                String lineId = await getLineUser();
                if (lineId == "unauthorized") { return Unauthorized(); }
                APIModel aPIModel = new APIModel();
                UserModel userModel = await _ICheckUserRepository.CheckLineUser(lineId);
                if (userModel == null) { return StatusCode(204); }
                aPIModel.data = userModel;
                aPIModel.message = "Success";
                return Ok(aPIModel);
            }
            catch (Exception ex) { return StatusErrorITSC("CheckUserController.CheckStage", ex); }
        }
    }
}
