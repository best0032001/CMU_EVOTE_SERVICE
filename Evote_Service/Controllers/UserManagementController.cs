
using Evote_Service.Model;
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
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Evote_Service.Controllers
{
    [Produces("application/json")]
    [Route("api/")]
    public class UserManagementController : ITSCController
    {
        private EvoteContext _evoteContext;
        private IUserManageRepository _userManageRepository;
        public UserManagementController(ApplicationDBContext applicationDBContext, ISMSRepository sMSRepository, EvoteContext evoteContext, ILogger<ITSCController> logger, IEventRepository eventRepository, IHttpClientFactory clientFactory, IWebHostEnvironment env, IEmailRepository emailRepository, IVoteRepository voteRepository, IUserManageRepository userManageRepository)
        {

            this.loadConfig(logger, clientFactory, env);
            _emailRepository = emailRepository;
            _evoteContext = evoteContext;
            _userManageRepository = userManageRepository;
        }

        [HttpPost("v1/UserManagement")]
        [ProducesResponseType(typeof(List<UserModelView>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIModel), (int)HttpStatusCode.ServiceUnavailable)]
        public async Task<IActionResult> searchUser([FromBody] AdminSearchModelView data)
        {
            String Cmuaccount = "";
            String action = "UserManagementController.searchUser";
            try
            {

                Cmuaccount = await getCmuaccount();
                if (Cmuaccount == "unauthorized") { return Unauthorized(); }
                List<UserEntity> userEntities = await _userManageRepository.searchUser(data, Cmuaccount);
                if (userEntities == null) { return Unauthorized(); }

                List<UserModelView> userModelViews = this.MapUserModelView(userEntities);

                APIModel aPIModel = new APIModel();
                aPIModel.data = userModelViews;
                aPIModel.title = "Success";
                return StatusCodeITSC("CMU", "", Cmuaccount, action, 200, aPIModel);
            }
            catch (Exception ex) { return StatusErrorITSC("CMU", "", Cmuaccount, action, ex); }
        }

        [HttpPut("v1/Admin/Deactive")]
        [ProducesResponseType(typeof(List<UserModelView>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> adminDeactive([FromQuery] Int32 userEntityId)
        {
            String Cmuaccount = "";
            String action = "UserManagementController.adminDeactive";
            try
            {
                String RAW_KEY = Environment.GetEnvironmentVariable("RAW_KEY");
                String PASS_KEY = Environment.GetEnvironmentVariable("PASS_KEY");
                Crypto crypto = new Crypto(PASS_KEY, RAW_KEY);
                Cmuaccount = await getCmuaccount();
                if (Cmuaccount == "unauthorized") { return Unauthorized(); }
                if (userEntityId == 0) { return BadRequest(); }

                List<UserEntity> userEntities = await _userManageRepository.deActiveUser(Cmuaccount, userEntityId, getClientIP());
                if (userEntities == null) { return Unauthorized(); }

                List<UserModelView> userModelViews = new List<UserModelView>();
                foreach (UserEntity userEntity in userEntities)
                {
                    String json = JsonConvert.SerializeObject(userEntity);
                    UserModelView userModelView = JsonConvert.DeserializeObject<UserModelView>(json);
                    userModelView.Tel = crypto.DecryptFromBase64(userModelView.Tel);
                    userModelView.PersonalID = crypto.DecryptFromBase64(userModelView.PersonalID);
                    userModelView.UserStageText = DataCache.RefUserStages.Where(w => w.RefUserStageID == userModelView.UserStage).First().UserStageName;
                    userModelViews.Add(userModelView);
                }
                // ขาดการส่ง Email
                APIModel aPIModel = new APIModel();
                aPIModel.data = userModelViews;
                aPIModel.title = "Success";

                return StatusCodeITSC("CMU", "", Cmuaccount, action, 200, aPIModel);
            }
            catch (Exception ex)
            {
                return StatusErrorITSC("CMU", "", Cmuaccount, action, ex);
            }
        }
    }
}
