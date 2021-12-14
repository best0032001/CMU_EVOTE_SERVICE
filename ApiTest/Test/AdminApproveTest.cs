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
    public class AdminApproveTest
    {
        private HttpClient _client;
        private CustomWebApplicationFactory<Startup> app;

        public AdminApproveTest()
        {
            app = new CustomWebApplicationFactory<Startup>();
            _client = app.CreateClient();
        }

        [TestMethod]
        public async Task TestAdminApprove()
        {
            _client = app.CreateClient();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer a01");

            var response = await _client.GetAsync("api/v1/Admin/Approve");
            Assert.IsTrue((int)response.StatusCode == 200);
            String responseString = await response.Content.ReadAsStringAsync();
            APIModel dataTemp = JsonConvert.DeserializeObject<APIModel>(responseString);
            Assert.IsTrue(dataTemp.title == "Success");
            String data = JsonConvert.SerializeObject(dataTemp.data);
            List<UserEntity> userEntities = JsonConvert.DeserializeObject<List<UserEntity>>(data);
            Assert.IsTrue(userEntities.Count > 0);

            AdminApproveModelView adminApproveModelView = new AdminApproveModelView();
            adminApproveModelView.userEntityId = userEntities[0].UserEntityId;
            adminApproveModelView.comment = "test";
            String json = JsonConvert.SerializeObject(adminApproveModelView);
            var content = new StringContent(json);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            response = await _client.PutAsync("api/v1/Admin/NotApprove", content);
       
            Assert.IsTrue((int)response.StatusCode == 200);

            responseString = await response.Content.ReadAsStringAsync();
            dataTemp = JsonConvert.DeserializeObject<APIModel>(responseString);
            Assert.IsTrue(dataTemp.title == "Success");
            data = JsonConvert.SerializeObject(dataTemp.data);
            userEntities = JsonConvert.DeserializeObject<List<UserEntity>>(data);
            Assert.IsTrue(userEntities.Count == 0);



            //json = "{ \"userEntityId\":\"" + userEntities[0].UserEntityId + "\"}";
            //response = await _client.PutAsync("api/v1/User/Approve", new StringContent(json));
            //Assert.IsTrue((int)response.StatusCode == 200);

            //responseString = await response.Content.ReadAsStringAsync();
            //dataTemp = JsonConvert.DeserializeObject<APIModel>(responseString);
            //Assert.IsTrue(dataTemp.message == "Success");
            //data = JsonConvert.SerializeObject(dataTemp.data);
            //userEntities = JsonConvert.DeserializeObject<List<UserEntity>>(data);
            //Assert.IsTrue(userEntities.Count == 0);

        }
    }
}
