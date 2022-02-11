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
    public class UserPortalTest
    {
        private HttpClient _client;
        private CustomWebApplicationFactory<Startup> app;

        public UserPortalTest()
        {
            app = new CustomWebApplicationFactory<Startup>();
            _client = app.CreateClient();
        }
        [TestMethod]
        public async Task TestUserPortal()
        {
            _client = app.CreateClient();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer x02");

            var response = await _client.GetAsync("api/v1/Portal");
            Assert.IsTrue((int)response.StatusCode == 200);
            String responseString = await response.Content.ReadAsStringAsync();
            APIModel dataTemp = JsonConvert.DeserializeObject<APIModel>(responseString);
            Assert.IsTrue(dataTemp.title == "Success");
            String data = JsonConvert.SerializeObject(dataTemp.data);
            UserPortalModelView userPortalModelView = JsonConvert.DeserializeObject<UserPortalModelView>(data);
         //   Assert.IsTrue(userPortalModelView.eventModelviewsPassed.Count == 1);
        }
    }
}
