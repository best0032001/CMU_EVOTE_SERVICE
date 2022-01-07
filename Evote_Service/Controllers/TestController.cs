
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
    public class TestController : ITSCController
    {
        private IAdminRepository _IAdminRepository;
        private IEventRepository _eventRepository;
        public TestController(ILogger<ITSCController> logger, IEventRepository eventRepository, IHttpClientFactory clientFactory, IWebHostEnvironment env, IEmailRepository emailRepository)
        {

            this.loadConfig(logger, clientFactory, env);
            _emailRepository = emailRepository;
            _eventRepository = eventRepository;
        }

        [HttpGet("v1/EventTest")]
        [ProducesResponseType(typeof(EventConfirmModelview), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> addEvent()
        {
            String Cmuaccount = "";
            String action = "EventController.addEvent";
            EventModelview eventModelview = new EventModelview();
            eventModelview.EventTitle = "Test";
            eventModelview.EventDetail = "Test";
            eventModelview.Organization_Code = "00";
            eventModelview.OrganizationFullNameTha = "00";
            eventModelview.EventRegisterStart = DateTime.Now;
            eventModelview.EventRegisterEnd = DateTime.Now;
            eventModelview.EventVotingStart = DateTime.Now;
            eventModelview.EventVotingEnd = DateTime.Now;
          
            eventModelview.EventInformation = "sdfsfsfsafs";
            int ApplicationEntityId = 1;
            try
            {
                ApplicationEntity applicationEntity = await _eventRepository.getApplicationEntity(ApplicationEntityId);
                if (applicationEntity == null) { return BadRequest(); }
                EventVoteEntity eventVoteEntity = await _eventRepository.getEventEntityByEventVoteEntityId(ApplicationEntityId, 10);
                Cmuaccount = "cheewin.b@cmu.ac.th";
                if (Cmuaccount == "") { return Unauthorized(); }


                EventConfirmModelview eventConfirmModelview = await _eventRepository.addEvent(ApplicationEntityId, eventModelview, Cmuaccount);

                APIModel aPIModel = new APIModel();
                aPIModel.data = eventConfirmModelview;
                aPIModel.title = "Success";
                return StatusCodeITSC("CMU", "", Cmuaccount, action, 200, aPIModel);
            }
            catch (Exception ex) { return StatusErrorITSC("CMU", "", Cmuaccount, action, ex); }
        }
    }
}
