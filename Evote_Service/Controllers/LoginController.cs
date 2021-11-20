
using Evote_Service.Model.Entity;
using Evote_Service.Model.Interface;
using Evote_Service.Model.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Evote_Service.Controllers
{
    [Route("api/")]
    [ApiController]
    public class LoginController : ITSCController
    {
        private readonly IHttpClientFactory _clientFactory;
        private ICheckUserRepository _ICheckUserRepository;
        public LoginController(ILogger<ITSCController> logger,IHttpClientFactory clientFactory, IWebHostEnvironment env,ICheckUserRepository CheckUserRepository)
        {

            this.loadConfig(logger, clientFactory, env);
            _clientFactory = clientFactory;
            _ICheckUserRepository = CheckUserRepository;
        }


        [HttpGet("callback")]
        public async Task<ActionResult<String>> callback(String code)
        {
            String lineId = await getLineUser();
            if (lineId == "unauthorized") { return Unauthorized(); }
            String redirect_uri = Environment.GetEnvironmentVariable("CMU_REDIRECT_URL");
            String client_id = Environment.GetEnvironmentVariable("CMU_CLIENT_ID");
            String client_secret = Environment.GetEnvironmentVariable("CMU_CLIENTSECRET");
            String oauth_scope = Environment.GetEnvironmentVariable("CMU_OAUTH_SCOPE");
            String oauth_authorize_url = Environment.GetEnvironmentVariable("CMU_OAUTH_URL");
            String grant_type = "authorization_code";
            String urlgettoken  = Environment.GetEnvironmentVariable("CMU_GETTOKEN_URL");

            var postData = new Dictionary<string, string>
            {
                { "code", code },
                { "redirect_uri", redirect_uri },
                { "client_id", client_id },
                { "client_secret", client_secret },
                { "grant_type", grant_type }
            };

            var content = new FormUrlEncodedContent(postData);
            HttpClient httpClient = _clientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            HttpResponseMessage response = await httpClient.PostAsync(urlgettoken, content);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();


            dynamic responseGetToken = JsonConvert.DeserializeObject<dynamic>(responseString);
            String _access_token = responseGetToken.access_token;
            String _refresh_token = responseGetToken.refresh_token;
            String _expires_in = responseGetToken.expires_in;

            String urlbasicinfo = Environment.GetEnvironmentVariable("CMU_BASICINFO_URL");
            httpClient = _clientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _access_token);
            response = await httpClient.GetAsync(urlbasicinfo);
            response.EnsureSuccessStatusCode();
            responseString = await response.Content.ReadAsStringAsync();

            APIModel aPIModel = new APIModel();
            UserCMUAccountModel responseprofile = JsonConvert.DeserializeObject<UserCMUAccountModel>(responseString);
            if (responseprofile.itaccounttype_EN != "MIS Employee") 
            {
                aPIModel.message = "ผู้ลงทะเบียนต้อง บุคคลกร";
                return StatusCodeITSC("line", lineId, "", "LoginController.callback", 403, aPIModel);
            }
            UserEntity userEntity=  await _ICheckUserRepository.GetLineUserEntity(lineId);
            if (userEntity != null)
            {
                aPIModel.message = "LineID นี้ได้ลงทะเบียนในระบบแล้ว";
                return StatusCodeITSC("line", lineId, "", "LoginController.callback", 403, aPIModel);
            }
            userEntity = new UserEntity();
            userEntity.Email = responseprofile.cmuitaccount;
            userEntity.Organization_Code = responseprofile.organization_code;
            userEntity.Organization_Name_TH = responseprofile.organization_name_TH;
            userEntity.LineId = lineId;
            userEntity.FullName = responseprofile.firstname_TH+" "+ responseprofile.lastname_TH;
            userEntity.Access_token = _access_token;
            userEntity.Refresh_token = _refresh_token;
            userEntity.Expires_in = _expires_in;
            if (await _ICheckUserRepository.RegisCMUUser(userEntity) == false)
            {
                aPIModel.message = "ระบบขัดข้องบันทึกข้อมูลไม่สำเร็จ";
                return StatusCodeITSC("line", lineId, "", "LoginController.callback", 503, aPIModel);
            }
            aPIModel.data = responseprofile;
            aPIModel.message = "ลงทะเบียนสำเร็จ";
            return StatusCodeITSC("CMU", lineId, responseprofile.cmuitaccount, "LoginController.callback", 200, aPIModel);

          
        }

    }
}
