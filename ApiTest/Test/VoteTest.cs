using ApiTest.model;
using Evote_Service;
using Evote_Service.Model.Entity;
using Evote_Service.Model.Util;
using Evote_Service.Model.View;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ApiTest.Test
{
    [TestClass]
    public class VoteTest
    {
        private HttpClient _client;
        private CustomWebApplicationFactory<Startup> app;
        private String SecretKey = "TW9zaGVFcmV6UHJpdmF0ZUtleQ==";
        public VoteTest()
        {
            app = new CustomWebApplicationFactory<Startup>();
            _client = app.CreateClient();
        }
        [TestMethod]
        public async Task TestVote()
        {
            _client = app.CreateClient();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer x02");


            VoteSMSModelView voteSMSModelView = new VoteSMSModelView();
            voteSMSModelView.lineMode = true;
            voteSMSModelView.ApplicationEntityId = 1;
            voteSMSModelView.EventVoteEntityId = 1;
            voteSMSModelView.EventTypeId = 1;
            voteSMSModelView.VoteRound = 1;

            String json = JsonConvert.SerializeObject(voteSMSModelView);
            var content = new StringContent(json);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await _client.PostAsync("api/v1/Vote/OTP", content);
            Assert.IsTrue((int)response.StatusCode == 200);

            String responseString = await response.Content.ReadAsStringAsync();
            APIModel dataTemp = JsonConvert.DeserializeObject<APIModel>(responseString);
            Assert.IsTrue(dataTemp.title == "Success");
            String data = JsonConvert.SerializeObject(dataTemp.data);
            data = JsonConvert.DeserializeObject<String>(data);


            VoteModelView voteModelView = new VoteModelView();
            voteModelView.lineMode = true;
            voteModelView.ApplicationEntityId = 1;
            voteModelView.EventVoteEntityId = 1;
            voteModelView.EventTypeId = 1;
            voteModelView.VoteRound = 1;

            voteModelView.OTP = "1234";
            voteModelView.RefOTP = data;

            VoteModel voteModel = new VoteModel();
            voteModel.data1 = "data1";
            voteModel.data2 = "data2";
            voteModel.data3 = "data3";
            json = JsonConvert.SerializeObject(voteModel);
            Claim[] Claims = new Claim[]
                {
                    new Claim(ClaimTypes.UserData, json)
                };
            string token = GenerateToken(Claims);
            voteModelView.TokenData = token;


            json = JsonConvert.SerializeObject(voteModelView);
            content = new StringContent(json);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            response = await _client.PostAsync("api/v1/Vote", content);
            Assert.IsTrue((int)response.StatusCode == 200);


            json = JsonConvert.SerializeObject(voteSMSModelView);
            content = new StringContent(json);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            response = await _client.PostAsync("api/v1/Votedata", content);
            Assert.IsTrue((int)response.StatusCode == 200);
            responseString = await response.Content.ReadAsStringAsync();
            dataTemp = JsonConvert.DeserializeObject<APIModel>(responseString);
            Assert.IsTrue(dataTemp.title == "Success");
            data = JsonConvert.SerializeObject(dataTemp.data);
            List<VoteEntity> voteEntities = JsonConvert.DeserializeObject<List<VoteEntity>>(data);

            Assert.IsTrue(voteEntities.Count>0);


            response = await _client.GetAsync("api/v1/VoteBatch");
            Assert.IsTrue((int)response.StatusCode == 200);
            response = await _client.GetAsync("api/v1/ConfirmBatch");
            Assert.IsTrue((int)response.StatusCode == 200);
        }

        string GenerateToken(Claim[] Claims)
        {
            String token = "";

            SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(Claims),
                Expires = DateTime.Now.AddMinutes(Convert.ToInt32(1)),
                SigningCredentials = new SigningCredentials(GetSymmetricSecurityKey(SecretKey), SecurityAlgorithms.HmacSha256Signature)
            };
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            token = jwtSecurityTokenHandler.WriteToken(securityToken);
            return token;
        }
        SecurityKey GetSymmetricSecurityKey(String SecretKey)
        {
            byte[] symmetricKey = Convert.FromBase64String(SecretKey);
            return new SymmetricSecurityKey(symmetricKey);
        }

        IEnumerable<Claim> GetTokenClaims(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Given token is null or empty.");

            TokenValidationParameters tokenValidationParameters = GetTokenValidationParameters();

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
        private TokenValidationParameters GetTokenValidationParameters()
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
    }
}
