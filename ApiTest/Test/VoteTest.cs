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
    public class VoteTest
    {
        private HttpClient _client;
        private CustomWebApplicationFactory<Startup> app;

        public VoteTest()
        {
            app = new CustomWebApplicationFactory<Startup>();
            _client = app.CreateClient();
        }
        [TestMethod]
        public async Task TestAdminApprove()
        {
            _client = app.CreateClient();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer x02");


            VoteSMSModelView voteModelView = new VoteSMSModelView();
            voteModelView.lineMode = true;
            voteModelView.ApplicationEntityId = 1;
            voteModelView.EventVoteEntityId = 1;
            voteModelView.EventTypeId = 1;

            String json = JsonConvert.SerializeObject(voteModelView);
            var content = new StringContent(json);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await _client.PostAsync("api/v1/Vote/OTP", content);

            Assert.IsTrue((int)response.StatusCode == 200);

        }
    }
}
