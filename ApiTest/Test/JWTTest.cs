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
    public class JWTTest
    {
        private String SecretKey = "TW9zaGVFcmV6UHJpdmF0ZUtleQ==";
        private String SecretKeyTest = "1111111111111111";
        [TestMethod]
        public async Task TestJWT()
        {
            Crypto crypto = new Crypto(SecretKey, SecretKeyTest);

            String dataTest = crypto.Encrypt("Test");

            String Test = crypto.DecryptFromBase64(dataTest);
            Assert.IsTrue(Test == "Test");
            VoteModel voteModel = new VoteModel();
            voteModel.data1 = "data1";
            voteModel.data2 = "data2";
            voteModel.data3 = "data3";
            String json = JsonConvert.SerializeObject(voteModel);
            Claim[] Claims = new Claim[]
                {
                    new Claim(ClaimTypes.UserData, json)
                };
            string token = GenerateToken(Claims);
            Assert.IsTrue(token.Length > 0);

            //  String tokenFake = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3VzZXJkYXRhIjoie1wiZGF0YTFcIjpcImRhdGEzXCIsXCJkYXRhMlwiOlwiZGF0YTJcIixcImRhdGEzXCI6XCJkYXRhM1wifSIsIm5iZiI6MTYzOTQ2NzE4NywiZXhwIjoxNjM5NDY3MjQ3LCJpYXQiOjE2Mzk0NjcxODd9.xYdSfNa5D4GmpkkbjGsEgNL-rJ-TMAUOiPVhvDUA9VY";
            List<Claim> claims = GetTokenClaims(token).ToList();
            String data = claims.FirstOrDefault(e => e.Type.Equals(ClaimTypes.UserData)).Value;

            VoteModel model = JsonConvert.DeserializeObject<VoteModel>(data);
            Assert.IsTrue(model.data1 == "data1");
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
