﻿
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
    public class UserPortalController : ITSCController
    {

        private IUserRepository _userRepository;
        public UserPortalController(ILogger<ITSCController> logger, IUserRepository userRepository, IHttpClientFactory clientFactory, IWebHostEnvironment env, IEmailRepository emailRepository)
        {

            this.loadConfig(logger, clientFactory, env);
            _emailRepository = emailRepository;
            _userRepository = userRepository;
        }

        [HttpGet("v1/Portal")]
        [ProducesResponseType(typeof(UserPortalModelView), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> getPortal()
        {
            String lineId = "";
            String action = "UserPortalController.getPortal";
            try
            {
                lineId = await getLineUser();
                if (lineId == "unauthorized") { return Unauthorized(); }
                APIModel aPIModel = new APIModel();

                UserEntity userEntity= await _userRepository.getUserEntity(lineId);
                List<EventModelview> eventModelviews = await _userRepository.getEventModelviewList(lineId);
                UserPortalModelView userPortalModelView = new UserPortalModelView();
                userPortalModelView.eventModelviewsNow = new List<EventModelview>();
                userPortalModelView.eventModelviewsPassed = new List<EventModelview>();
                userPortalModelView.eventModelviewsIncomming = new List<EventModelview>();



                foreach (EventModelview eventModelview in eventModelviews)
                {
                    if (eventModelview.IsUseTime)
                    {
                        int res = DateTime.Compare(DateTime.Now, eventModelview.EventVotingStart);
                        if (res < 0)
                        {

                            userPortalModelView.eventModelviewsIncomming.Add(eventModelview);
                        }
                        else
                        {
                            res = DateTime.Compare(DateTime.Now, eventModelview.EventVotingEnd);
                            if (res > 0)
                            {
                                userPortalModelView.eventModelviewsPassed.Add(eventModelview);
                            }

                            else
                            {
                                userPortalModelView.eventModelviewsNow.Add(eventModelview);
                            }
                        }
                    }
                    else
                    {
                        if (eventModelview.EventStatusId == 2)
                        {
                            userPortalModelView.eventModelviewsNow.Add(eventModelview);
                        }
                    }
 
                }
                aPIModel.data = userPortalModelView;
                aPIModel.title = "Success";
                return StatusCodeITSC("line", lineId, userEntity.Email, action, 200, aPIModel);
            }
            catch (Exception ex)
            {
                return StatusErrorITSC("line", lineId, "", action, ex);
            }
        }
    }
}
