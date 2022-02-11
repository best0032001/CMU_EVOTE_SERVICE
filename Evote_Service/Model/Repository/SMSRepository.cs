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
        protected IEmailRepository _emailRepository;
        public SMSRepository(IHttpClientFactory clientFactory,IEmailRepository emailRepository)
        {
            _clientFactory = clientFactory;
            _emailRepository = emailRepository;
        }
        public async Task<string> getOTP(String RefCode, String tel)
        {
            Random _random = new Random();
            String Code = _random.Next(0, 999999).ToString("D6");

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
            sMSMessages.text = "CMU E-vote: The SMS-OTP is " + Code + " ( Ref Code " + RefCode + ")";

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

        public async Task<string> getOTPWithEmail(string RefCode, string Email)
        {
            Random _random = new Random();
            String Code = _random.Next(0, 999999).ToString("D6");

            await _emailRepository.SendEmailAsync("CMU Evote service", Email, "ยืนยันadmin เข้าใช้ระบบ", "รหัสยืนยัน " + Code + " RefCode:" + RefCode, null);

            return Code;
        }
    }
}
