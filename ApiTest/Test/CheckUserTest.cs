using Evote_Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class CheckUserTest
    {
        private HttpClient _client;
        private CustomWebApplicationFactory<Startup> app;

        public CheckUserTest()
        {
            app = new CustomWebApplicationFactory<Startup>();
            _client = app.CreateClient();
        }
        [TestMethod]
        public async Task TestCheckLineUser()
        {
            _client = app.CreateClient();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer x01");
            var response = await _client.GetAsync("api/v1/User/CheckStage");
            Assert.IsTrue((int)response.StatusCode == 204);
        }
    }
}
