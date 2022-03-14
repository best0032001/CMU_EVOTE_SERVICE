
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
    public class VoteController : ITSCController
    {
        private IVoteRepository _voteRepository;
        private EvoteContext _evoteContext;
        private ApplicationDBContext _applicationDBContext;
        private ISMSRepository _sMSRepository;
        private IEventRepository _eventRepository;
        public VoteController(ApplicationDBContext applicationDBContext, ISMSRepository sMSRepository, EvoteContext evoteContext, ILogger<ITSCController> logger, IEventRepository eventRepository, IHttpClientFactory clientFactory, IWebHostEnvironment env, IEmailRepository emailRepository, IVoteRepository voteRepository)
        {

            this.loadConfig(logger, clientFactory, env);
            _emailRepository = emailRepository;
            _voteRepository = voteRepository;
            _evoteContext = evoteContext;
            _sMSRepository = sMSRepository;
            _eventRepository = eventRepository;
            _applicationDBContext = applicationDBContext;

        }

        [HttpPost("v1/Vote/OTP")]
        [ProducesResponseType(typeof(APIModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> VoteOTP([FromBody] VoteSMSModelView data)
        {
            String Cmuaccount = "";
            String Email = "";
            String lineId = "";
            UserEntity userEntity = null;
            String action = "VoteController.VoteOTP";
            try
            {
                APIModel aPIModel = new APIModel();

                ApplicationEntity applicationEntity = _evoteContext.ApplicationEntitys.Where(w => w.ApplicationEntityId == data.ApplicationEntityId).FirstOrDefault();
                EventVoteEntity eventVoteEntity = _evoteContext.EventVoteEntitys.Where(w => w.ApplicationEntityId == applicationEntity.ApplicationEntityId && w.EventVoteEntityId == data.EventVoteEntityId).FirstOrDefault();
                if (applicationEntity == null) { return Unauthorized(); }
                if (eventVoteEntity == null) { return Unauthorized(); }
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Development")
                {
                    if (eventVoteEntity.IsUseTime)
                    {
                        int res = DateTime.Compare(DateTime.Now, eventVoteEntity.EventVotingStart);
                        if (res < 0) { return Unauthorized(); }
                        res = DateTime.Compare(DateTime.Now, eventVoteEntity.EventVotingEnd);
                        if (res >= 0) { return Unauthorized(); }
                    }
                    else
                    {
                        if (eventVoteEntity.EventStatusId != 2) { return Unauthorized(); }
                    }

                }


                if (this.checkAppIP(applicationEntity.ServerProductionIP) == false) { return Unauthorized(); }
                if (data.lineMode)
                {
                    lineId = await getLineUser();
                    if (lineId == "unauthorized") { return Unauthorized(); }
                    userEntity = _evoteContext.UserEntitys.Where(w => w.LineId == lineId && w.UserStage == 3).FirstOrDefault();
                    if (userEntity == null) { return Unauthorized(); }
                }
                else
                {
                    Cmuaccount = await this.checkAppID(applicationEntity.ClientId);
                    if (Cmuaccount == "unauthorized") { return Unauthorized(); }
                    userEntity = _evoteContext.UserEntitys.Where(w => w.Email == Cmuaccount && w.UserStage == 3 && w.UserType == 1).FirstOrDefault();
                    if (userEntity == null) { return Unauthorized(); }

                }
                Email = userEntity.Email;
                VoterEntity voterEntity = _evoteContext.VoterEntitys.Where(w => w.Email == Email && w.EventVoteEntityId == eventVoteEntity.EventVoteEntityId).FirstOrDefault();
                if (voterEntity == null) { return Unauthorized(); }
                Random _random = new Random();
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                String code = new string(Enumerable.Repeat(chars, 4)
                  .Select(s => s[_random.Next(s.Length)]).ToArray());
                voterEntity.SMSExpire = DateTime.Now.AddMinutes(5);
                voterEntity.SMSOTPRef = code;
                String RAW_KEY = Environment.GetEnvironmentVariable("RAW_KEY");
                String PASS_KEY = Environment.GetEnvironmentVariable("PASS_KEY");
                Crypto crypto = new Crypto(PASS_KEY, RAW_KEY);

                voterEntity.SMSOTP = await _sMSRepository.getOTP(code, crypto.DecryptFromBase64(userEntity.Tel));
                _evoteContext.SaveChanges();
                aPIModel.title = "Success";
                aPIModel.data = code;

                return StatusCodeITSC("CMU", "", Cmuaccount, action, 200, aPIModel);

            }
            catch (Exception ex) { return StatusErrorITSC("CMU", "", Cmuaccount, action, ex); }
        }

        [HttpPost("v1/Vote")]
        [ProducesResponseType(typeof(APIModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIModel), (int)HttpStatusCode.ServiceUnavailable)]
        public async Task<IActionResult> Vote(VoteModelView data)
        {
            String Cmuaccount = "";
            String Email = "";
            String lineId = "";
            UserEntity userEntity = null;
            String action = "VoteController.Vote";
            try
            {
                APIModel aPIModel = new APIModel();

                ApplicationEntity applicationEntity = _evoteContext.ApplicationEntitys.Where(w => w.ApplicationEntityId == data.ApplicationEntityId).FirstOrDefault();
                EventVoteEntity eventVoteEntity = _evoteContext.EventVoteEntitys.Where(w => w.ApplicationEntityId == applicationEntity.ApplicationEntityId && w.EventVoteEntityId == data.EventVoteEntityId).FirstOrDefault();
                if (applicationEntity == null) { return Unauthorized(); }
                if (eventVoteEntity == null) { return Unauthorized(); }

                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Development")
                {
                    if (eventVoteEntity.IsUseTime)
                    {
                        int res = DateTime.Compare(DateTime.Now, eventVoteEntity.EventVotingStart);
                        if (res < 0) { return Unauthorized(); }
                        res = DateTime.Compare(DateTime.Now, eventVoteEntity.EventVotingEnd);
                        if (res >= 0) { return Unauthorized(); }
                    }
                    else
                    {
                        if (eventVoteEntity.EventStatusId != 2) { return Unauthorized(); }
                    }

                }

                if (this.checkAppIP(applicationEntity.ServerProductionIP) == false) { return Unauthorized(); }
                if (data.lineMode)
                {
                    lineId = await getLineUser();
                    if (lineId == "unauthorized") { return Unauthorized(); }
                    userEntity = _evoteContext.UserEntitys.Where(w => w.LineId == lineId && w.UserStage == 3).FirstOrDefault();
                    if (userEntity == null) { return Unauthorized(); }
                }
                else
                {
                    Cmuaccount = await this.checkAppID(applicationEntity.ClientId);
                    if (Cmuaccount == "unauthorized") { return Unauthorized(); }
                    userEntity = _evoteContext.UserEntitys.Where(w => w.Email == Cmuaccount && w.UserStage == 3 && w.UserType == 1).FirstOrDefault();
                    if (userEntity == null) { return Unauthorized(); }

                }
                Email = userEntity.Email;

                ConfirmVoter confirmVoter = _evoteContext.confirmVoters.Where(w => w.email == Email && w.EventVoteEntityId == eventVoteEntity.EventVoteEntityId && w.RoundNumber == data.VoteRound).FirstOrDefault();
                if (confirmVoter != null) { return Unauthorized(); }
                VoterEntity voterEntity = _evoteContext.VoterEntitys.Where(w => w.Email == Email && w.EventVoteEntityId == eventVoteEntity.EventVoteEntityId).FirstOrDefault();
                if (voterEntity == null) { return Unauthorized(); }

                String TokenData = data.TokenData;
                String SecretKey = "";
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Development")
                {
                    int res = 0;
                    if (eventVoteEntity.IsUseTime)
                    {
                        res = DateTime.Compare(DateTime.Now, eventVoteEntity.EventVotingStart);
                        if (res < 0) { return Unauthorized(); }
                        res = DateTime.Compare(DateTime.Now, eventVoteEntity.EventVotingEnd);
                        if (res >= 0) { return Unauthorized(); }
                    }
                    else
                    {
                        if (eventVoteEntity.EventStatusId != 2) { return Unauthorized(); }
                    }


                    SecretKey = eventVoteEntity.SecretKey;
                    if (!(voterEntity.SMSOTP == data.OTP && voterEntity.SMSOTPRef == data.RefOTP)) { return Unauthorized(); }
                    res = DateTime.Compare(DateTime.Now, (DateTime)voterEntity.SMSExpire);
                    if (res > 0) { return Unauthorized(); }
                }
                else
                {
                    //for test
                    SecretKey = "TW9zaGVFcmV6UHJpdmF0ZUtleQ==";
                }

                List<Claim> claims = GetTokenClaims(TokenData, SecretKey);
                if (claims == null) { return Unauthorized(); }
                String dataVote = claims.FirstOrDefault(e => e.Type.Equals(ClaimTypes.UserData)).Value;

       
                VoteEntity voteEntitie = new VoteEntity();
                voteEntitie.VoteData = dataVote;
                voteEntitie.RoundNumber = data.VoteRound;
                voteEntitie.ApplicationEntityId = applicationEntity.ApplicationEntityId;
                voteEntitie.EventVoteEntityId = applicationEntity.EventVoteEntitys[0].EventVoteEntityId;
                _evoteContext.voteEntities.Add(voteEntitie);


                confirmVoter = new ConfirmVoter();
                confirmVoter.email = Email;
                confirmVoter.EventVoteEntityId = voteEntitie.EventVoteEntityId;
                confirmVoter.RoundNumber = data.VoteRound;
                _evoteContext.confirmVoters.Add(confirmVoter);

                _evoteContext.SaveChanges();



                aPIModel.title = "Success";
                return StatusCodeITSC("CMU", "", Cmuaccount, action, 200, aPIModel);

            }
            catch (Exception ex) { return StatusErrorITSC("CMU", "", Cmuaccount, action, ex); }
        }

        [HttpPost("v1/Votedata")]
        [ProducesResponseType(typeof(List<VoteEntity>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> getVotedata(VoteSMSModelView data)
        {
            String Cmuaccount = "";
            UserEntity userEntity = null;
            String action = "VoteController.getVotedata";
            try
            {
                APIModel aPIModel = new APIModel();
                ApplicationEntity applicationEntity = _evoteContext.ApplicationEntitys.Where(w => w.ApplicationEntityId == data.ApplicationEntityId).FirstOrDefault();
                EventVoteEntity eventVoteEntity = _evoteContext.EventVoteEntitys.Where(w => w.ApplicationEntityId == applicationEntity.ApplicationEntityId && w.EventVoteEntityId == data.EventVoteEntityId).FirstOrDefault();
                if (applicationEntity == null) { return Unauthorized(); }
                if (eventVoteEntity == null) { return Unauthorized(); }
                Cmuaccount = await this.checkAppID(applicationEntity.ClientId);
                if (Cmuaccount == "unauthorized") { return Unauthorized(); }

                if (eventVoteEntity.PresidentEmail != Cmuaccount) { return Unauthorized(); }
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Development")
                {
                    if (eventVoteEntity.IsUseTime)
                    {
                        int res = DateTime.Compare(DateTime.Now, eventVoteEntity.EventVotingEnd);
                        if (res <= 0) { return Unauthorized(); }
                    }
                    else
                    {
                        //if (eventVoteEntity.EventStatusId != 2) { return Unauthorized(); }
                    }

                }
                eventVoteEntity.EventStatusId = 3;
                _evoteContext.SaveChanges();
                List<VoteEntity> voteEntities = _evoteContext.voteEntities.Where(w => w.ApplicationEntityId == data.ApplicationEntityId && w.EventVoteEntityId == eventVoteEntity.EventVoteEntityId && w.RoundNumber == data.VoteRound).OrderBy(o => o.VoteData).ToList();
                aPIModel.data = voteEntities;
                aPIModel.title = "Success";
                return StatusCodeITSC("CMU", "", Cmuaccount, action, 200, aPIModel);

            }
            catch (Exception ex) { return StatusErrorITSC("CMU", "", Cmuaccount, action, ex); }
        }



        [HttpGet("v1/count")]
        [ProducesResponseType(typeof(APIModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> countEvent([FromQuery] int EventVoteEntityId, [FromQuery] int RoundNumber, [FromQuery] int ApplicationEntityId)
        {
            String Cmuaccount = "";
            String action = "VoteController.countEvent";
            try
            {
                ApplicationEntity applicationEntity = await _eventRepository.getApplicationEntity(ApplicationEntityId);
                if (applicationEntity == null) { return BadRequest(); }
                if (this.checkAppIP(applicationEntity.ServerProductionIP) == false) { return Unauthorized(); }
                Cmuaccount = await this.checkAppID(applicationEntity.ClientId);
                if (Cmuaccount == "unauthorized") { return Unauthorized(); }
                EventVoteEntity eventVoteEntity = await _eventRepository.getEventEntityByEventVoteEntityId(ApplicationEntityId, EventVoteEntityId);
                if (eventVoteEntity.CreateUser != Cmuaccount) { return Unauthorized(); }

                int n = _evoteContext.confirmVoters.Where(w => w.EventVoteEntityId == EventVoteEntityId && w.RoundNumber == RoundNumber).Count();

                APIModel aPIModel = new APIModel();
                aPIModel.data = n;
                aPIModel.title = "Success";
                return StatusCodeITSC("CMU", "", Cmuaccount, action, 200, aPIModel);
            }
            catch (Exception ex) { return StatusErrorITSC("CMU", "", Cmuaccount, action, ex); }
        }


        [HttpGet("v1/VoteBatch")]
        [ProducesResponseType(typeof(List<VoteEntity>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> VoteBatch()
        {
            String Cmuaccount = "";
            String Email = "";
            UserEntity userEntity = null;
            String action = "VoteController.VoteBatch";
            try
            {
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Development")
                {
                    String TokenKey = Environment.GetEnvironmentVariable("TOEN_BATCH");
                    if (TokenKey != this.getTokenFormHeader()) { return Unauthorized(); }
                }
                APIModel aPIModel = new APIModel();
                List<VoteEntity> voteEntities = _applicationDBContext.voteEntities.OrderBy(o => o.VoteData).ToList();
                foreach (VoteEntity voteEntity in voteEntities)
                {
                    String json = JsonConvert.SerializeObject(voteEntity);
                    VoteEntity model = JsonConvert.DeserializeObject<VoteEntity>(json);
                    _evoteContext.voteEntities.Add(model);
                    _applicationDBContext.voteEntities.Remove(voteEntity);
                }
                _evoteContext.SaveChanges();
                _applicationDBContext.SaveChanges();
                aPIModel.title = "Success";
                return StatusCodeITSC("CMU", "", Cmuaccount, action, 200, aPIModel);

            }
            catch (Exception ex) { return StatusErrorITSC("CMU", "", Cmuaccount, action, ex); }
        }
        [HttpGet("v1/ConfirmBatch")]
        [ProducesResponseType(typeof(List<VoteEntity>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]      
        public async Task<IActionResult> ConfirmBatch()
        {
            String Cmuaccount = "";
            String Email = "";
            UserEntity userEntity = null;
            String action = "VoteController.ConfirmBatch";
            try
            {
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Development")
                {
                    String TokenKey = Environment.GetEnvironmentVariable("TOEN_BATCH");
                    if (TokenKey != this.getTokenFormHeader()) { return Unauthorized(); }
                }
                APIModel aPIModel = new APIModel();
                List<ConfirmVoter> confirmVoters = _applicationDBContext.confirmVoters.OrderBy(o => o.email).ToList();
                foreach (ConfirmVoter confirmVoter in confirmVoters)
                {
                    String json = JsonConvert.SerializeObject(confirmVoter);
                    ConfirmVoter model = JsonConvert.DeserializeObject<ConfirmVoter>(json);
                    _evoteContext.confirmVoters.Add(model);
                    _applicationDBContext.confirmVoters.Remove(confirmVoter);
                }
                _evoteContext.SaveChanges();
                _applicationDBContext.SaveChanges();

                aPIModel.title = "Success";
                return StatusCodeITSC("CMU", "", Cmuaccount, action, 200, aPIModel);

            }
            catch (Exception ex) { return StatusErrorITSC("CMU", "", Cmuaccount, action, ex); }
        }

        private List<Claim> GetTokenClaims(string token, String SecretKey)
        {
            if (string.IsNullOrEmpty(token))
            { return null; }

            TokenValidationParameters tokenValidationParameters = GetTokenValidationParameters(SecretKey);

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            try
            {
                ClaimsPrincipal tokenValid = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
                return tokenValid.Claims.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private TokenValidationParameters GetTokenValidationParameters(String SecretKey)
        {
            return new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = GetSymmetricSecurityKey(SecretKey)
            };
        }
        private SecurityKey GetSymmetricSecurityKey(String SecretKey)
        {
            byte[] symmetricKey = Convert.FromBase64String(SecretKey);
            return new SymmetricSecurityKey(symmetricKey);
        }
    }



}
