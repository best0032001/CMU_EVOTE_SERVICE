﻿using Evote_Service;
using Evote_Service.Model.Entity;
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

            String json = "{ \"firstName\":\"firstNameTest1\" ,\"lastname\":\"lastnameTest1\",\"email\":\"test@test.com\"}";

            response = await _client.PostAsync("api/v1/User/RegisLiff", new StringContent(json));
            Assert.IsTrue((int)response.StatusCode == 201);
            String responseString = await response.Content.ReadAsStringAsync();
            APIModel dataTemp = JsonConvert.DeserializeObject<APIModel>(responseString);
            Assert.IsTrue(dataTemp.message == "Success"); ;
            String data = JsonConvert.SerializeObject(dataTemp.data);
            UserModel userModel = JsonConvert.DeserializeObject<UserModel>(data);
            Assert.IsTrue(userModel.UserStage==1); ;
            Assert.IsTrue(userModel.Email == "test@test.com");

            json = "{ \"tel\":\"000000\"}";
            response = await _client.PostAsync("api/v1/User/Tel", new StringContent(json));
            Assert.IsTrue((int)response.StatusCode == 200);

            json = "{ \"otp\":\"1234\"}";
            response = await _client.PostAsync("api/v1/User/SMSOTP", new StringContent(json));
            Assert.IsTrue((int)response.StatusCode == 200);
             responseString = await response.Content.ReadAsStringAsync();
             dataTemp = JsonConvert.DeserializeObject<APIModel>(responseString);
            Assert.IsTrue(dataTemp.message == "Success"); ;
             data = JsonConvert.SerializeObject(dataTemp.data);
            userModel = JsonConvert.DeserializeObject<UserModel>(data);
            Assert.IsTrue(userModel.IsConfirmTel == true); ;
            Assert.IsTrue(userModel.ConfirmTelTime != null);


            response = await _client.GetAsync("api/v1/User/getEmailOTP");
            Assert.IsTrue((int)response.StatusCode == 200);

            json = "{ \"otp\":\"1234\"}";
            response = await _client.PostAsync("api/v1/User/EmailOTP", new StringContent(json));
            Assert.IsTrue((int)response.StatusCode == 200);
            responseString = await response.Content.ReadAsStringAsync();
            dataTemp = JsonConvert.DeserializeObject<APIModel>(responseString);
            Assert.IsTrue(dataTemp.message == "Success"); ;
            data = JsonConvert.SerializeObject(dataTemp.data);
            userModel = JsonConvert.DeserializeObject<UserModel>(data);
            Assert.IsTrue(userModel.IsConfirmEmail == true); ;
            Assert.IsTrue(userModel.ConfirmEmailTime != null);

        }
    }
}