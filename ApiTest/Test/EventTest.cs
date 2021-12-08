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
    public class EventTest
    {
        private HttpClient _client;
        private CustomWebApplicationFactory<Startup> app;

        public EventTest()
        {
            app = new CustomWebApplicationFactory<Startup>();
            _client = app.CreateClient();
        }

        [TestMethod]
        public async Task TestEvent()
        {
            _client = app.CreateClient();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer a01");

            var response = await _client.GetAsync("api/v1/Event?ApplicationEntityId=" + 1);
            Assert.IsTrue((int)response.StatusCode == 200);
            String responseString = await response.Content.ReadAsStringAsync();
            APIModel dataTemp = JsonConvert.DeserializeObject<APIModel>(responseString);
            Assert.IsTrue(dataTemp.message == "Success");
            String data = JsonConvert.SerializeObject(dataTemp.data);
            List<EventVoteEntity> eventVoteEntities = JsonConvert.DeserializeObject<List<EventVoteEntity>>(data);
            Assert.IsTrue(eventVoteEntities.Count == 0);

            EventModelview eventModelview = new EventModelview();
            eventModelview.EventTitle = "Test";
            eventModelview.EventDetail = "Test";
            eventModelview.Organization_Code = "00";
            eventModelview.OrganizationFullNameTha = "00";
            eventModelview.EventRegisterStart = DateTime.Now;
            eventModelview.EventRegisterEnd = DateTime.Now;
            eventModelview.EventVotingStart = DateTime.Now;
            eventModelview.EventVotingEnd = DateTime.Now;


            String json = JsonConvert.SerializeObject(eventModelview);
            var content = new StringContent(json);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            response = await _client.PostAsync("api/v1/Event?ApplicationEntityId=" + 1, content);
            Assert.IsTrue((int)response.StatusCode == 200);

            responseString = await response.Content.ReadAsStringAsync();
            dataTemp = JsonConvert.DeserializeObject<APIModel>(responseString);
            Assert.IsTrue(dataTemp.message == "Success");
            data = JsonConvert.SerializeObject(dataTemp.data);
            EventConfirmModelview eventConfirmModelview = JsonConvert.DeserializeObject<EventConfirmModelview>(data);

            Assert.IsTrue(eventConfirmModelview.SecretKey.Count() > 0);
        }


    }
}
