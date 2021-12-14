
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
        private ISMSRepository _sMSRepository;
        public VoteController(ISMSRepository sMSRepository, EvoteContext evoteContext, ILogger<ITSCController> logger, IEventRepository eventRepository, IHttpClientFactory clientFactory, IWebHostEnvironment env, IEmailRepository emailRepository, IVoteRepository voteRepository)
        {

            this.loadConfig(logger, clientFactory, env);
            _emailRepository = emailRepository;
            _voteRepository = voteRepository;
            _evoteContext = evoteContext;
            _sMSRepository = sMSRepository;

        }

        [HttpPost("v1/Vote/OTP")]
        [ProducesResponseType(typeof(UserModel), (int)HttpStatusCode.OK)]
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

                ApplicationEntity applicationEntity = _evoteContext.ApplicationEntitys.Where(w => w.ApplicationEntityId == data.ApplicationEntityId).Include(i => i.EventVoteEntitys.Where(e => e.EventVoteEntityId == data.EventVoteEntityId)).FirstOrDefault();
                if (applicationEntity == null) { return Unauthorized(); }
                if (applicationEntity.EventVoteEntitys.Count == 0) { return Unauthorized(); }
                if (this.checkAppIP(applicationEntity.ServerProductionIP) == false) { return Unauthorized(); }
                if (data.lineMode)
                {
                    lineId = await getLineUser();
                    if (lineId == "unauthorized") { return Unauthorized(); }
                    userEntity = _evoteContext.UserEntitys.Where(w => w.LineId == lineId && w.UserStage == 3 && w.UserType == 2).FirstOrDefault();
                    if (userEntity == null) { return Unauthorized(); }
                }
                else
                {
                    Cmuaccount = await this.checkAppID(applicationEntity.ClientId);
                    if (Cmuaccount == "") { return Unauthorized(); }
                    userEntity = _evoteContext.UserEntitys.Where(w => w.Email == Cmuaccount && w.UserStage == 3 && w.UserType == 1).FirstOrDefault();
                    if (userEntity == null) { return Unauthorized(); }

                }
                Email = userEntity.Email;
                VoterEntity voterEntity = _evoteContext.VoterEntitys.Where(w => w.Email == Email && w.EventVoteEntityId == applicationEntity.EventVoteEntitys[0].EventVoteEntityId).FirstOrDefault();
                if (voterEntity == null) { return Unauthorized(); }
                Random _random = new Random();
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                String code = new string(Enumerable.Repeat(chars, 4)
                  .Select(s => s[_random.Next(s.Length)]).ToArray());
                voterEntity.SMSCreate = DateTime.Now;
                voterEntity.SMSOTPRef = code;
                voterEntity.SMSOTP = await _sMSRepository.getOTP(code, userEntity.Tel);
                _evoteContext.SaveChanges();
                aPIModel.title = "Success";
                aPIModel.data = code;

                return StatusCodeITSC("CMU", "", Cmuaccount, action, 200, aPIModel);

            }
            catch (Exception ex) { return StatusErrorITSC("CMU", "", Cmuaccount, action, ex); }
        }

        [HttpPost("v1/Vote")]
        [ProducesResponseType(typeof(UserEntity), (int)HttpStatusCode.OK)]
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

                ApplicationEntity applicationEntity = _evoteContext.ApplicationEntitys.Where(w => w.ApplicationEntityId == data.ApplicationEntityId).Include(i => i.EventVoteEntitys.Where(e => e.EventVoteEntityId == data.EventVoteEntityId)).FirstOrDefault();
                if (applicationEntity == null) { return Unauthorized(); }
                if (applicationEntity.EventVoteEntitys.Count == 0) { return Unauthorized(); }
                if (this.checkAppIP(applicationEntity.ServerProductionIP) == false) { return Unauthorized(); }
                if (data.lineMode)
                {
                    lineId = await getLineUser();
                    if (lineId == "unauthorized") { return Unauthorized(); }
                    userEntity = _evoteContext.UserEntitys.Where(w => w.LineId == lineId && w.UserStage == 3 && w.UserType == 2).FirstOrDefault();
                    if (userEntity == null) { return Unauthorized(); }
                }
                else
                {
                    Cmuaccount = await this.checkAppID(applicationEntity.ClientId);
                    if (Cmuaccount == "") { return Unauthorized(); }
                    userEntity = _evoteContext.UserEntitys.Where(w => w.Email == Cmuaccount && w.UserStage == 3 && w.UserType == 1).FirstOrDefault();
                    if (userEntity == null) { return Unauthorized(); }

                }
                Email = userEntity.Email;
                VoterEntity voterEntity = _evoteContext.VoterEntitys.Where(w => w.Email == Email && w.EventVoteEntityId == applicationEntity.EventVoteEntitys[0].EventVoteEntityId).FirstOrDefault();
                if (voterEntity == null) { return Unauthorized(); }
                if (!(voterEntity.SMSOTP == data.OTP && voterEntity.SMSOTPRef == data.RefOTP)) { return Unauthorized(); }
                int res = DateTime.Compare(DateTime.Now, applicationEntity.EventVoteEntitys[0].EventVotingEnd);
                if (res >= 0) { return Unauthorized(); }
                String TokenData = data.TokenData;
                String SecretKey = applicationEntity.EventVoteEntitys[0].SecretKey;
                if (!IsTokenValid(TokenData, SecretKey)) { return Unauthorized(); }

                List<Claim> claims = GetTokenClaims(TokenData, SecretKey).ToList();
                String dataVote = claims.FirstOrDefault(e => e.Type.Equals(ClaimTypes.UserData)).Value;
                aPIModel.title = "Success";
                return StatusCodeITSC("CMU", "", Cmuaccount, action, 200, aPIModel);

            }
            catch (Exception ex) { return StatusErrorITSC("CMU", "", Cmuaccount, action, ex); }
        }

        private IEnumerable<Claim> GetTokenClaims(string token, String SecretKey)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Given token is null or empty.");

            TokenValidationParameters tokenValidationParameters = GetTokenValidationParameters(SecretKey);

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            try
            {
                ClaimsPrincipal tokenValid = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
                return tokenValid.Claims;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private bool IsTokenValid(string token, String SecretKey)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Given token is null or empty.");

            TokenValidationParameters tokenValidationParameters = GetTokenValidationParameters(SecretKey);

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            try
            {
                ClaimsPrincipal tokenValid = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private TokenValidationParameters GetTokenValidationParameters(String SecretKey)
        {
            return new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime=true,
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
