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
using System.Security.Cryptography;
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

        private String _PublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAoAPV2hhYKqdW97A+G7bQXzeIXuw5jDPFdobdYqWRWPu97tGnILFEBx/ztNGKNiY/l9wZbpL66OKOOBvX+1uFPdur209PK7QbViOGY8khmTQuSkbGNpYrgg5+1RFJYRsY7mI2FcqjefuBbJRPUpprLoEze4Hl22fPmGaA1xz7gNues1ynBtQSp6ZQdSlwTfdRQ2sMZlPYwgo53lnZPuLmNzU2CGMDmpO8Nbr1QThSndHzSrXNw1+WDPRmwacjpsYjPnDd2zvI1I6zpLuD45Mh08ZZbb5kwzB8sg9nfzz5yD9xa3Yaz6uhr/43XCkTFD7wZMKcMebEPWXVGMzBrGReNQIDAQAB";
        private String _PrivateKey = "MIIEowIBAAKCAQEAoAPV2hhYKqdW97A+G7bQXzeIXuw5jDPFdobdYqWRWPu97tGnILFEBx/ztNGKNiY/l9wZbpL66OKOOBvX+1uFPdur209PK7QbViOGY8khmTQuSkbGNpYrgg5+1RFJYRsY7mI2FcqjefuBbJRPUpprLoEze4Hl22fPmGaA1xz7gNues1ynBtQSp6ZQdSlwTfdRQ2sMZlPYwgo53lnZPuLmNzU2CGMDmpO8Nbr1QThSndHzSrXNw1+WDPRmwacjpsYjPnDd2zvI1I6zpLuD45Mh08ZZbb5kwzB8sg9nfzz5yD9xa3Yaz6uhr/43XCkTFD7wZMKcMebEPWXVGMzBrGReNQIDAQABAoIBAG5tFyiyQi31W5wFAWeIytXa8f5n0PMDS1MXkTIhhmO9Hv7vqgFys7qi/0EaleH2lU5MczSOtB8BMhpghLWPHC1rKndnjQBhA7h3PaghRlF+5C8YFnPXQGE5dae+jUA5PgRMvHxfYl+tBE0VMISV0j++o/Oo2iKGyommu2U4OAxc68HIE+sxjkIb4FPFulZigFus0yVYosTjF/RE2JeM73DBXF4jg1qNPEyQaW61YLz9vPRjf2j7hL49fx4DIAqvn54kkKhjD5fjSpKhYL1xfDvObhyUYph0jfl7q5ezHKmIma09ff5A95/wHTi9dGnrhhYbTpTPoWyg1ohpe+ENgaECgYEA1JLfgoXhEQd65tsVIhx8MvIVfGtvF5VwVTtAbhqlNOs0Bt/4adpB8ds6llFGpvZXfr0KZsUg27JfBCyNWzwITeA8ma2k+GK2r11ZhFw8AzPVs7My9R2gGDZMZNRxmfQKLjC5cwnG/2AMQVNhxxtl6tJrLtFGw50il7fls8r8JakCgYEAwLRCLMcPppemZ0ntvDw4MSvZtx1Zadf2gigSseBkqf28EK4HgJc3agQHtxRQEL7nXbl7z7TmGCQszvDmDvUGgq1uTBX5zWNu6boFmMQrsJBEufnEjyoVPPfq5fvxUyhOzngMeeFlmv/qR3KeBsFfA2ClruO2JmbR8Qq09Wb0c60CgYEAzSLfZonj5Bcf12BcSIrMoC1V5reWgV/JA7cmOhqkiyjfEDNa+muRb+Br7VuJnt3jGX88ZmidiOXdI54K25xXNy/Jy1Py+2/nc9vV4xFPKJgBBmVMK5bnQ/ZCSpto9XS3zlNe41DwJMl/ihr5JLef5rggjxGOBH/DPj5NAPBF2+ECgYBkke7zZZRCcmTTBR9AnQEKkIMYcQXIGoC5Xuaa1KxUl2q+HcUmlETEXIQWRVCf3LHtFS+LsDJhqQeFnO3EIpaaPp8QsGtliJ5K9t2S49aVWEW19adivCjHX+/ExV8l8iRm1vpT5ZFcenEvhp74kZTfs2Hky0y17/VjYh4c8PVlJQKBgHUGHtIGepbSjFKHVaBlCtnIpjA+0wV1KgCaUgJE+NRcWqY9+byKZKMk5IFU/lSeOJh2uRhX823WOOGpOXDqGRWAEt8ktJCf+YWXDGMM7b0hDTicvds4NZC3C0V9v1+Tc592Ez05IVQ41TfpOrsDRYdA/AfUW0DAXcCKslxTQe/n";
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

            var publicKey = Convert.FromBase64String(_PublicKey);
            RSA rsaPublic = RSA.Create();
            rsaPublic.ImportSubjectPublicKeyInfo(publicKey, out int keyLengthPub);
            VoteEnCrypt voteEnCrypt = new VoteEnCrypt();
            String cypherText = await voteEnCrypt.EnCrypt(json, rsaPublic);

            Claim[] Claims = new Claim[]
                {
                    new Claim(ClaimTypes.UserData, cypherText)
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

            Assert.IsTrue(voteEntities.Count > 0);


            String VoteData = voteEntities[0].VoteData;

            var privateKey = Convert.FromBase64String(_PrivateKey);
            RSA rsaPrivate = RSA.Create();
            rsaPrivate.ImportRSAPrivateKey(privateKey, out int keyLengthPri);

            String deCryptText = await voteEnCrypt.DeCrypt(VoteData, rsaPrivate);


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
