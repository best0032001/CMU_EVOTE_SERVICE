using Evote_Service;
using Evote_Service.Model.Entity;
using Evote_Service.Model.Util;
using Evote_Service.Model.View;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ApiTest.Test
{
    [TestClass]
    public class RegisterUserTest
    {
        private HttpClient _client;
        private CustomWebApplicationFactory<Startup> app;

        public RegisterUserTest()
        {
            app = new CustomWebApplicationFactory<Startup>();
            _client = app.CreateClient();
        }
        [TestMethod]
        public async Task TestRegis_Verity_UserLiff()
        {
            _client = app.CreateClient();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer x01");
            var response = await _client.GetAsync("api/v1/User/liff");
            Assert.IsTrue((int)response.StatusCode == 204);

            //String json = "{ \"firstName\":\"firstNameTest1\" ,\"lastName\":\"lastnameTest1\",\"email\":\"test@test.com\"}";
            String json = "{ \"firstName\":\"firstNameTest1\" ,\"lastName\":\"lastnameTest1\"}";
            var content = new StringContent(json);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            response = await _client.PostAsync("api/v1/User/RegisLiff", content);
            Assert.IsTrue((int)response.StatusCode == 201);
            String responseString = await response.Content.ReadAsStringAsync();
            APIModel dataTemp = JsonConvert.DeserializeObject<APIModel>(responseString);
            Assert.IsTrue(dataTemp.title == "Success"); ;
            String data = JsonConvert.SerializeObject(dataTemp.data);
            UserModel userModel = JsonConvert.DeserializeObject<UserModel>(data);
            Assert.IsTrue(userModel.UserStage == 1); ;


            //  กรอกเบอร์โทร

            json = "{ \"tel\":\"000000\"}";
            content = new StringContent(json);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            response = await _client.PostAsync("api/v1/User/Tel", content);
            Assert.IsTrue((int)response.StatusCode == 200);

            json = "{ \"otp\":\"1234\"}";
            content = new StringContent(json);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            response = await _client.PostAsync("api/v1/User/SMSOTP", content);
            Assert.IsTrue((int)response.StatusCode == 200);
            responseString = await response.Content.ReadAsStringAsync();
            dataTemp = JsonConvert.DeserializeObject<APIModel>(responseString);
            Assert.IsTrue(dataTemp.title == "Success"); ;
            data = JsonConvert.SerializeObject(dataTemp.data);
            userModel = JsonConvert.DeserializeObject<UserModel>(data);
            Assert.IsTrue(userModel.IsConfirmTel == true); ;
            Assert.IsTrue(userModel.ConfirmTelTime != null);

            json = "{ \"email\":\"test@test.com\"}";
            content = new StringContent(json);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            response = await _client.PostAsync("api/v1/User/email", content);
            Assert.IsTrue((int)response.StatusCode == 200);

            json = "{ \"otp\":\"1234\"}";
            content = new StringContent(json);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            response = await _client.PostAsync("api/v1/User/EmailOTP", content);
            Assert.IsTrue((int)response.StatusCode == 200);
            responseString = await response.Content.ReadAsStringAsync();
            dataTemp = JsonConvert.DeserializeObject<APIModel>(responseString);
            Assert.IsTrue(dataTemp.title == "Success"); ;
            data = JsonConvert.SerializeObject(dataTemp.data);
            userModel = JsonConvert.DeserializeObject<UserModel>(data);
            Assert.IsTrue(userModel.IsConfirmEmail == true); ;
            Assert.IsTrue(userModel.ConfirmEmailTime != null);

            response = await _client.GetAsync("api/v1/Portal");
            responseString = await response.Content.ReadAsStringAsync();
            dataTemp = JsonConvert.DeserializeObject<APIModel>(responseString);
            data = JsonConvert.SerializeObject(dataTemp.data);
            UserPortalModelView userPortalModelView = JsonConvert.DeserializeObject<UserPortalModelView>(data);

        }
    }
}
