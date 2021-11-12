using Evote_Service.Model.Interface;
using Evote_Service.Model.sms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Evote_Service.Model.Repository
{
    public class SMSRepository : ISMSRepository
    {
        private readonly IHttpClientFactory _clientFactory;
        public SMSRepository(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        public async Task<string> getOTP(String RefCode, String tel)
        {
            Random _random = new Random();
            String Code = _random.Next(0, 9999).ToString("D4");

            String SMS_API = Environment.GetEnvironmentVariable("SMS_API");
            String SMS_SENDER = Environment.GetEnvironmentVariable("SMS_SENDER");
            String SMS_USER = Environment.GetEnvironmentVariable("SMS_USER");
            String SMS_Pass = Environment.GetEnvironmentVariable("SMS_Pass");
            var byteArray = Encoding.ASCII.GetBytes($"{SMS_USER}:{SMS_Pass}");
            String token = Convert.ToBase64String(byteArray);


            SMSModel sMSModel = new SMSModel();
            sMSModel.messages = new List<SMSMessages>();


            SMSMessages sMSMessages = new SMSMessages();
            sMSMessages.from = SMS_SENDER;
            sMSMessages.destinations = new List<SMSdestinations>();
            sMSMessages.text = "รหัส OTP ของคุณคือ " + Code + "Ref code " + RefCode;

            SMSdestinations sMSdestinations = new SMSdestinations();
            sMSdestinations.to = tel;
            sMSdestinations.messageId = RefCode;

            sMSMessages.destinations.Add(sMSdestinations);

            sMSModel.messages.Add(sMSMessages);
            String json = JsonConvert.SerializeObject(sMSModel);
            var content = new StringContent(json);
           
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            HttpClient httpClient = _clientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Basic "+ token);
            var response = await httpClient.PostAsync(SMS_API, content);
            if ((int)response.StatusCode != 200) { throw new SystemException(); }

            return Code;
        }
    }
}
