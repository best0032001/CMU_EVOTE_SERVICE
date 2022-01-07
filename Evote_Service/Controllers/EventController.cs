
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
                if (Cmuaccount == "unauthorized") { return Unauthorized(); }
                APIModel aPIModel = new APIModel();
                aPIModel.data = await _eventRepository.getEventEntityByApplicationEntityId(ApplicationEntityId);
                aPIModel.title = "Success";
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
                if (Cmuaccount == "unauthorized") { return Unauthorized(); }


                EventConfirmModelview eventConfirmModelview = await _eventRepository.addEvent(ApplicationEntityId, data, Cmuaccount);

                APIModel aPIModel = new APIModel();
                aPIModel.data = eventConfirmModelview;
                aPIModel.title = "Success";
                return StatusCodeITSC("CMU", "", Cmuaccount, action, 200, aPIModel);
            }
            catch (Exception ex) { return StatusErrorITSC("CMU", "", Cmuaccount, action, ex); }
        }


        [HttpPost("v1/Event/Confirm")]
        [ProducesResponseType(typeof(EventConfirmModelview), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ConfirmEvent([FromQuery] int EventVoteEntityId, [FromQuery] int ApplicationEntityId)
        {
            String Cmuaccount = "";
            String action = "EventController.ConfirmEvent";
            try
            {
                ApplicationEntity applicationEntity = await _eventRepository.getApplicationEntity(ApplicationEntityId);
                if (applicationEntity == null) { return BadRequest(); }
                if (this.checkAppIP(applicationEntity.ServerProductionIP) == false) { return Unauthorized(); }
                Cmuaccount = await this.checkAppID(applicationEntity.ClientId);
                if (Cmuaccount == "unauthorized") { return Unauthorized(); }
                EventVoteEntity eventVoteEntity = await _eventRepository.getEventEntityByEventVoteEntityId(ApplicationEntityId, EventVoteEntityId);
                if (eventVoteEntity == null) { return BadRequest(); }
                if (eventVoteEntity.PresidentEmail != Cmuaccount) { return Unauthorized(); }

               Boolean  check = await _eventRepository.ConfirmEvent(ApplicationEntityId, EventVoteEntityId, Cmuaccount);

                APIModel aPIModel = new APIModel();
                aPIModel.data = null;
                aPIModel.title = "Success";
                return StatusCodeITSC("CMU", "", Cmuaccount, action, 200, aPIModel);
            }
            catch (Exception ex) { return StatusErrorITSC("CMU", "", Cmuaccount, action, ex); }
        }

        [HttpDelete("v1/Event")]
        [ProducesResponseType(typeof(EventConfirmModelview), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> deleteEvent([FromQuery] int EventVoteEntityId, [FromQuery] int ApplicationEntityId)
        {
            String Cmuaccount = "";
            String action = "EventController.deleteEvent";
            try
            {
                ApplicationEntity applicationEntity = await _eventRepository.getApplicationEntity(ApplicationEntityId);
                if (applicationEntity == null) { return BadRequest(); }
                if (this.checkAppIP(applicationEntity.ServerProductionIP) == false) { return Unauthorized(); }
                Cmuaccount = await this.checkAppID(applicationEntity.ClientId);
                if (Cmuaccount == "unauthorized") { return Unauthorized(); }
                EventVoteEntity eventVoteEntity = await _eventRepository.getEventEntityByEventVoteEntityId(ApplicationEntityId, EventVoteEntityId);
                if (eventVoteEntity.EventStatusId != 1) { return Unauthorized(); }
                int res = DateTime.Compare(DateTime.Now, eventVoteEntity.EventVotingStart);
                if(eventVoteEntity.CreateUser!= Cmuaccount) { return Unauthorized(); }
                if (res >= 0) { return Unauthorized(); }
                APIModel aPIModel = new APIModel();
                Boolean check = await _eventRepository.deleteEvent(ApplicationEntityId, EventVoteEntityId, Cmuaccount);
                if (check == false) { return StatusCodeITSC("CMU", "", Cmuaccount, action, 503, aPIModel); }


                aPIModel.title = "Success";
                return StatusCodeITSC("CMU", "", Cmuaccount, action, 200, aPIModel);
            }
            catch (Exception ex) { return StatusErrorITSC("CMU", "", Cmuaccount, action, ex); }
        }

        [HttpPost("v1/Voter")]
        [ProducesResponseType(typeof(APIModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> addVoter([FromBody] VoterModelview data, [FromQuery] int ApplicationEntityId)
        {
            String Cmuaccount = "";
            String action = "EventController.addVoter";
            try
            {
                ApplicationEntity applicationEntity = await _eventRepository.getApplicationEntity(ApplicationEntityId);
                if (applicationEntity == null) { return BadRequest(); }
                if (this.checkAppIP(applicationEntity.ServerProductionIP) == false) { return Unauthorized(); }
                Cmuaccount = await this.checkAppID(applicationEntity.ClientId);
                if (Cmuaccount == "unauthorized") { return Unauthorized(); }
                EventVoteEntity eventVoteEntity = await _eventRepository.getEventEntityByEventVoteEntityId(ApplicationEntityId, data.EventVoteEntityId);
                if (eventVoteEntity == null) { return BadRequest(); }
                if (eventVoteEntity.CreateUser != Cmuaccount) { return Unauthorized(); }
                int res = DateTime.Compare(DateTime.Now, eventVoteEntity.EventVotingStart);
                if (res >= 0) { return Unauthorized(); }
                Boolean check = await _eventRepository.addVoter(data, Cmuaccount);

                APIModel aPIModel = new APIModel();
                aPIModel.data = null;
                aPIModel.title = "Success";
                return StatusCodeITSC("CMU", "", Cmuaccount, action, 200, aPIModel);
            }
            catch (Exception ex) { return StatusErrorITSC("CMU", "", Cmuaccount, action, ex); }
        }

        [HttpGet("v1/Voter")]
        [ProducesResponseType(typeof(APIModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> getVoter([FromQuery] int EventVoteEntityId, [FromQuery] int ApplicationEntityId)
        {
            String Cmuaccount = "";
            String action = "EventController.getVoter";
            try
            {
                ApplicationEntity applicationEntity = await _eventRepository.getApplicationEntity(ApplicationEntityId);
                if (applicationEntity == null) { return BadRequest(); }
                if (this.checkAppIP(applicationEntity.ServerProductionIP) == false) { return Unauthorized(); }
                Cmuaccount = await this.checkAppID(applicationEntity.ClientId);
                if (Cmuaccount == "unauthorized") { return Unauthorized(); }
                EventVoteEntity eventVoteEntity = await _eventRepository.getEventEntityByEventVoteEntityId(ApplicationEntityId, EventVoteEntityId);
                if (eventVoteEntity == null) { return BadRequest(); }
                List<VoterModelDataView> voterModelViews = await _eventRepository.getVoter(ApplicationEntityId, EventVoteEntityId);



                APIModel aPIModel = new APIModel();
                aPIModel.data = voterModelViews;
                aPIModel.title = "Success";
                return StatusCodeITSC("CMU", "", Cmuaccount, action, 200, aPIModel);
            }
            catch (Exception ex) { return StatusErrorITSC("CMU", "", Cmuaccount, action, ex); }
        }


        [HttpDelete("v1/Voter")]
        [ProducesResponseType(typeof(APIModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> deleteVoter([FromBody] VoterModelview data, [FromQuery] int ApplicationEntityId)
        {
            String Cmuaccount = "";
            String action = "EventController.deleteVoter";
            try
            {
                ApplicationEntity applicationEntity = await _eventRepository.getApplicationEntity(ApplicationEntityId);
                if (applicationEntity == null) { return BadRequest(); }
                if (this.checkAppIP(applicationEntity.ServerProductionIP) == false) { return Unauthorized(); }
                Cmuaccount = await this.checkAppID(applicationEntity.ClientId);
                if (Cmuaccount == "unauthorized") { return Unauthorized(); }
                EventVoteEntity eventVoteEntity = await _eventRepository.getEventEntityByEventVoteEntityId(ApplicationEntityId, data.EventVoteEntityId);
                if (eventVoteEntity == null) { return BadRequest(); }
                if (eventVoteEntity.CreateUser != Cmuaccount) { return Unauthorized(); }
                int res = DateTime.Compare(DateTime.Now, eventVoteEntity.EventVotingStart);
                if (res >= 0) return Unauthorized();
                Boolean check = await _eventRepository.deleteVoter(data, Cmuaccount);

                APIModel aPIModel = new APIModel();
                aPIModel.data = null;
                aPIModel.title = "Success";
                return StatusCodeITSC("CMU", "", Cmuaccount, action, 200, aPIModel);
            }
            catch (Exception ex) { return StatusErrorITSC("CMU", "", Cmuaccount, action, ex); }
        }


    }
}
