
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


        [HttpPost("v1/Admin/Approve")]
        [ProducesResponseType(typeof(List<UserModelView>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIModel), (int)HttpStatusCode.ServiceUnavailable)]
        public async Task<IActionResult> searchApprove([FromBody] AdminSearchModelView data)
        {
            String Cmuaccount = "";
            String action = "AdminApproveController.searchApprove";
            try
            {
                String json = "";
             
                Cmuaccount = await getCmuaccount();
                if (Cmuaccount == "unauthorized") { return Unauthorized(); }

                List<UserEntity> userEntities = await _IAdminRepository.searchUser(data,Cmuaccount);
                if (userEntities == null) { return Unauthorized(); }

                List<UserModelView> userModelViews = new List<UserModelView>();
                foreach (UserEntity userEntity in userEntities)
                {
                    json = JsonConvert.SerializeObject(userEntity);
                    UserModelView userModelView = JsonConvert.DeserializeObject<UserModelView>(json);
                    userModelViews.Add(userModelView);
                }

                APIModel aPIModel = new APIModel();
                aPIModel.data = userModelViews;
                aPIModel.title = "Success";
                return StatusCodeITSC("CMU", "", Cmuaccount, action, 200, aPIModel);

            }
            catch (Exception ex) { return StatusErrorITSC("CMU", "", Cmuaccount, action, ex); }
        }

        [HttpGet("v1/Admin/Approve")]
        [ProducesResponseType(typeof(List<UserModelView>), (int)HttpStatusCode.OK)]
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

                List<UserModelView> userModelViews = new List<UserModelView>();
                foreach (UserEntity userEntity in userEntities)
                {
                    String json = JsonConvert.SerializeObject(userEntity);
                    UserModelView userModelView = JsonConvert.DeserializeObject<UserModelView>(json);
                    userModelViews.Add(userModelView);
                }

                APIModel aPIModel = new APIModel();
                aPIModel.data = userModelViews;
                aPIModel.title = "Success";
                return StatusCodeITSC("CMU", "", Cmuaccount, action, 200, aPIModel);

            }
            catch (Exception ex) { return StatusErrorITSC("CMU", "", Cmuaccount, action, ex); }
        }

        [HttpPut("v1/Admin/Approve")]
        [ProducesResponseType(typeof(List<UserModelView>), (int)HttpStatusCode.OK)]
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

                List<UserModelView> userModelViews = new List<UserModelView>();
                foreach (UserEntity userEntity in userEntities)
                {
                    String json = JsonConvert.SerializeObject(userEntity);
                    UserModelView userModelView = JsonConvert.DeserializeObject<UserModelView>(json);
                    userModelViews.Add(userModelView);
                }

                APIModel aPIModel = new APIModel();
                aPIModel.data = userModelViews;
                aPIModel.title = "Success";

                return StatusCodeITSC("CMU", "", Cmuaccount, action, 200, aPIModel);
            }
            catch (Exception ex) {
                return StatusErrorITSC("CMU", "", Cmuaccount, action, ex);
            }
        }

        [HttpPut("v1/Admin/NotApprove")]
        [ProducesResponseType(typeof(List<UserModelView>), (int)HttpStatusCode.OK)]
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

                List<UserModelView> userModelViews = new List<UserModelView>();
                foreach (UserEntity userEntity in userEntities)
                {
                    String json = JsonConvert.SerializeObject(userEntity);
                    UserModelView userModelView = JsonConvert.DeserializeObject<UserModelView>(json);
                    userModelViews.Add(userModelView);
                }

                APIModel aPIModel = new APIModel();
                aPIModel.data = userModelViews;
                aPIModel.title = "Success";
                return StatusCodeITSC("CMU", "", Cmuaccount, "AdminApproveController.adminNotApprove", 200, aPIModel);
            }
            catch (Exception ex) { return StatusErrorITSC("CMU", "", Cmuaccount, "AdminApproveController.adminNotApprove", ex); }
        }

        [HttpGet("v1/admin/photoId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> UserGetphotoId([FromQuery] int userEntityId)
        {
            String Cmuaccount = "";
            String lineId = "";
            String action = "AdminApproveController.UserGetphotoId";
            try
            {
                Cmuaccount = await getCmuaccount();
                if (Cmuaccount == "unauthorized") { return Unauthorized(); }
                UserEntity userEntity = await _IAdminRepository.getUserEntity(Cmuaccount, userEntityId, getClientIP());
                if (userEntity == null) { return Unauthorized(); }
                var path = Path.Combine(Directory.GetCurrentDirectory(), "uploadphotoid", userEntity.fileNamePersonalID);
                var memory = this.loadFile(path);
                memory.Position = 0;
                return File(memory, GetContentType(path), Path.GetFileName(path));
            }
            catch (Exception ex)
            {
                return StatusErrorITSC("line", lineId, "", action, ex);
            }
        }


        [HttpGet("v1/admin/kyc")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Getkyc([FromQuery] int userEntityId)
        {
            String Cmuaccount = "";
            String lineId = "";
            String action = "AdminApproveController.Getkyc";
            try
            {
                Cmuaccount = await getCmuaccount();
                if (Cmuaccount == "unauthorized") { return Unauthorized(); }
                UserEntity userEntity = await _IAdminRepository.getUserEntity(Cmuaccount, userEntityId, getClientIP());
                if (userEntity == null) { return Unauthorized(); }
                var path = Path.Combine(Directory.GetCurrentDirectory(), "uploadkyc", userEntity.fileNameKYC);
                var memory = this.loadFile(path);
                memory.Position = 0;
                return File(memory, GetContentType(path), Path.GetFileName(path));
            }
            catch (Exception ex)
            {
                return StatusErrorITSC("line", lineId, "", action, ex);
            }
        }
    }
}
