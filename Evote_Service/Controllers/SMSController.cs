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

namespace Evote_Service.Controllers
{
    [Produces("application/json")]
    [Route("api/")]
    public class SMSController : ITSCController
    {


        private IEventRepository _eventRepository;
        private ISMSRepository _sMSRepository;
        private EvoteContext _evoteContext;
        public SMSController(ILogger<ITSCController> logger, IEventRepository eventRepository, EvoteContext evoteContext, ISMSRepository sMSRepository, IHttpClientFactory clientFactory, IWebHostEnvironment env, IEmailRepository emailRepository)
        {

            this.loadConfig(logger, clientFactory, env);
            _emailRepository = emailRepository;
            _evoteContext = evoteContext;
            _eventRepository = eventRepository;
            _sMSRepository = sMSRepository;
        }


        [HttpGet("v1/SMS")]
        [ProducesResponseType(typeof(APIModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIModel), (int)HttpStatusCode.ServiceUnavailable)]
        public async Task<IActionResult> getSMS( [FromQuery] int ApplicationEntityId)
        {
            String Cmuaccount = "";
            String Email = "";
            String lineId = "";
            UserEntity userEntity = null;
            String action = "SMSController.getSMS";
            try
            {
                APIModel aPIModel = new APIModel();

                ApplicationEntity applicationEntity = _evoteContext.ApplicationEntitys.Where(w => w.ApplicationEntityId == ApplicationEntityId).FirstOrDefault();
                if (applicationEntity == null) { return BadRequest(); }
                if (this.checkAppIP(applicationEntity.ServerProductionIP) == false) { return Unauthorized(); }
                Cmuaccount = await this.checkAppID(applicationEntity.ClientId);
                if (Cmuaccount == "unauthorized") { return Unauthorized(); }
                userEntity = _evoteContext.UserEntitys.Where(w => w.Email == Cmuaccount && w.UserStage == 3 && w.UserType == 1).FirstOrDefault();
                if (userEntity == null) { return Unauthorized(); }
                Random _random = new Random();
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                String code = new string(Enumerable.Repeat(chars, 4)
                  .Select(s => s[_random.Next(s.Length)]).ToArray());
                //  sent OTP

                String RAW_KEY = Environment.GetEnvironmentVariable("RAW_KEY");
                String PASS_KEY = Environment.GetEnvironmentVariable("PASS_KEY");
                Crypto crypto = new Crypto(PASS_KEY, RAW_KEY);
                userEntity.SMSOTP = await _sMSRepository.getOTP(code, crypto.DecryptFromBase64(userEntity.Tel));
                userEntity.SMSExpire = DateTime.Now.AddMinutes(5);
                userEntity.SMSOTPRef = code;
                _evoteContext.SaveChanges();
                aPIModel.title = "Success";
                aPIModel.data = code;
                return StatusCodeITSC("CMU", "", Cmuaccount, action, 200, aPIModel);

            }
            catch (Exception ex) { return StatusErrorITSC("CMU", "", Cmuaccount, action, ex); }
        }
        [HttpPost("v1/SMS")]
        [ProducesResponseType(typeof(APIModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIModel), (int)HttpStatusCode.ServiceUnavailable)]
        public async Task<IActionResult> checkSMS([FromBody] AdminLoginOTPModelview data,[FromQuery] int ApplicationEntityId)
        {
            String Cmuaccount = "";
            String Email = "";
            String lineId = "";
            UserEntity userEntity = null;
            String action = "SMSController.checkSMS";
            try
            {
                APIModel aPIModel = new APIModel();

                ApplicationEntity applicationEntity = _evoteContext.ApplicationEntitys.Where(w => w.ApplicationEntityId == ApplicationEntityId).FirstOrDefault();
                if (applicationEntity == null) { return BadRequest(); }
                if (this.checkAppIP(applicationEntity.ServerProductionIP) == false) { return Unauthorized(); }
                Cmuaccount = await this.checkAppID(applicationEntity.ClientId);
                if (Cmuaccount == "unauthorized") { return Unauthorized(); }
                userEntity = _evoteContext.UserEntitys.Where(w => w.Email == Cmuaccount && w.UserStage == 3 && w.UserType == 1).FirstOrDefault();
                if (userEntity == null) { return Unauthorized(); }
                if (!(userEntity.SMSOTP == data.otp && userEntity.SMSOTPRef == data.RefCode)) { return Unauthorized(); }
                int res   = DateTime.Compare(DateTime.Now, (DateTime)userEntity.SMSExpire);
                if (res >= 0) { return Unauthorized(); }
                aPIModel.title = "Success";
              
                return StatusCodeITSC("CMU", "", Cmuaccount, action, 200, aPIModel);

            }
            catch (Exception ex) { return StatusErrorITSC("CMU", "", Cmuaccount, action, ex); }
        }
    }
}
