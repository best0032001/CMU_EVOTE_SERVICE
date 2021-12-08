
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
    public class EventController : ITSCController
    {
        private IEventRepository _eventRepository;
        public EventController(ILogger<ITSCController> logger, IEventRepository eventRepository, IHttpClientFactory clientFactory, IWebHostEnvironment env, IEmailRepository emailRepository)
        {

            this.loadConfig(logger, clientFactory, env);
            _emailRepository = emailRepository;
            _eventRepository = eventRepository;
        }

        [HttpGet("v1/Event")]
        [ProducesResponseType(typeof(List<EventVoteEntity>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> getEvent([FromQuery] int ApplicationEntityId)
        {
            String Cmuaccount = "";
            String action = "EventController.getEvent";
            try
            {
                ApplicationEntity applicationEntity = await _eventRepository.getApplicationEntity(ApplicationEntityId);
                if (applicationEntity == null) { return BadRequest(); }
                if (this.checkAppIP(applicationEntity.ServerProductionIP) == false) { return Unauthorized(); }
                Cmuaccount = await this.checkAppID(applicationEntity.ClientId);
                if (Cmuaccount == "") { return Unauthorized(); }
                APIModel aPIModel = new APIModel();
                aPIModel.data = applicationEntity.EventVoteEntitys.OrderBy(o => o.ApplicationEntityId).ToList();
                aPIModel.message = "Success";
                return StatusCodeITSC("CMU", "", Cmuaccount, action, 200, aPIModel);
            }
            catch (Exception ex) { return StatusErrorITSC("CMU", "", Cmuaccount, action, ex); }
        }

        [HttpPost("v1/Event")]
        [ProducesResponseType(typeof(EventConfirmModelview), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> addEvent([FromBody] EventModelview data, [FromQuery] int ApplicationEntityId)
        {
            String Cmuaccount = "";
            String action = "EventController.addEvent";
            try
            {
                ApplicationEntity applicationEntity = await _eventRepository.getApplicationEntity(ApplicationEntityId);
                if (applicationEntity == null) { return BadRequest(); }
                if (this.checkAppIP(applicationEntity.ServerProductionIP) == false) { return Unauthorized(); }
                Cmuaccount = await this.checkAppID(applicationEntity.ClientId);
                if (Cmuaccount == "") { return Unauthorized(); }


                EventConfirmModelview eventConfirmModelview  = await _eventRepository.addEvent(ApplicationEntityId, data, Cmuaccount);

                APIModel aPIModel = new APIModel();
                aPIModel.data = eventConfirmModelview;
                aPIModel.message = "Success";
                return StatusCodeITSC("CMU", "", Cmuaccount, action, 200, aPIModel);
            }
            catch (Exception ex) { return StatusErrorITSC("CMU", "", Cmuaccount, action, ex); }
        }
    }
}
