
using Evote_Service.Model.Entity;
using Evote_Service.Model.Interface;
using Evote_Service.Model.Util;
using Evote_Service.Model.View;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        private IAdminRepository _IAdminRepository;
        public LoginController(ILogger<ITSCController> logger, IHttpClientFactory clientFactory, IWebHostEnvironment env, ICheckUserRepository CheckUserRepository, IAdminRepository IAdminRepository, IEmailRepository emailRepository)
        {

            this.loadConfig(logger, clientFactory, env);
            _clientFactory = clientFactory;
            _ICheckUserRepository = CheckUserRepository;
            _IAdminRepository = IAdminRepository;
            _emailRepository = emailRepository;
        }


        [HttpGet("v1/callback")]
        [ProducesResponseType(typeof(UserCMUAccountModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> callback([FromQuery] string code)
        {
            String lineId = "";
            try
            {
                lineId = await getLineUser();
                if (lineId == "unauthorized") { return Unauthorized(); }
                String redirect_uri = Environment.GetEnvironmentVariable("CMU_REDIRECT_URL");
                String client_id = Environment.GetEnvironmentVariable("CMU_CLIENT_ID");
                String client_secret = Environment.GetEnvironmentVariable("CMU_CLIENTSECRET");
                String oauth_scope = Environment.GetEnvironmentVariable("CMU_OAUTH_SCOPE");
                String oauth_authorize_url = Environment.GetEnvironmentVariable("CMU_OAUTH_URL");
                String grant_type = "authorization_code";
                String urlgettoken = Environment.GetEnvironmentVariable("CMU_GETTOKEN_URL");

                String urlpersonalID = Environment.GetEnvironmentVariable("CMU_PERSONAL_URL");
                String urlbasicinfo = Environment.GetEnvironmentVariable("CMU_BASICINFO_URL");
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
                    aPIModel.title = "ผู้ลงทะเบียนต้อง บุคคลกร";
                    return StatusCodeITSC("line", lineId, "", "LoginController.callback", 403, aPIModel);
                }
                UserEntity userEntity = await _ICheckUserRepository.GetLineUserEntity(lineId);
                if (userEntity != null)
                {
                    aPIModel.title = "LineID นี้ได้ลงทะเบียนในระบบแล้ว";
                    return StatusCodeITSC("line", lineId, "", "LoginController.callback", 403, aPIModel);
                }
                response = await httpClient.GetAsync(urlpersonalID);
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();


                dynamic dataTemp = JsonConvert.DeserializeObject<dynamic>(responseString);
                String personalid = dataTemp.personal_id;
                if (await _ICheckUserRepository.CheckPersonalID(personalid) == false)
                {
                    aPIModel.title = "เลขบัตรนี้มีผู้ใช้งานแล้ว";
                    return StatusCodeITSC("line", lineId, "", "LoginController.callback", 406, aPIModel);
                }
                userEntity = new UserEntity();
                userEntity.eventVoteEntities = new List<EventVoteEntity>();
                userEntity.Email = responseprofile.cmuitaccount;
                userEntity.IsConfirmEmail = true;
                userEntity.Organization_Code = responseprofile.organization_code;
                userEntity.Organization_Name_TH = responseprofile.organization_name_TH;
                userEntity.LineId = lineId;
                userEntity.PersonalID = personalid;
                userEntity.FullName = responseprofile.firstname_TH + " " + responseprofile.lastname_TH;
                userEntity.Access_token = _access_token;
                userEntity.Refresh_token = _refresh_token;
                userEntity.Expires_in = _expires_in;
                if (await _ICheckUserRepository.RegisCMUUser(userEntity) == false)
                {
                    aPIModel.title = "ระบบขัดข้องบันทึกข้อมูลไม่สำเร็จ";
                    return StatusCodeITSC("line", lineId, "", "LoginController.callback", 503, aPIModel);
                }
                aPIModel.data = responseprofile;
                aPIModel.title = "ลงทะเบียนสำเร็จ";
                return StatusCodeITSC("CMU", lineId, responseprofile.cmuitaccount, "LoginController.callback", 200, aPIModel);
            }
            catch (Exception ex)
            {
                return StatusErrorITSC("line", lineId, "", "LoginController.callback", ex);
            }



        }


        [HttpGet("v1/admin/callback")]
        [ProducesResponseType(typeof(String), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> admincallback([FromQuery] string code)
        {
            String Cmuaccount = "";
            try
            {
                String redirect_uri = Environment.GetEnvironmentVariable("CMU_REDIRECT_ADMINURL");
                String client_id = Environment.GetEnvironmentVariable("CMU_CLIENT_ID");
                String client_secret = Environment.GetEnvironmentVariable("CMU_CLIENTSECRET");
                String oauth_scope = Environment.GetEnvironmentVariable("CMU_OAUTH_SCOPE");
                String oauth_authorize_url = Environment.GetEnvironmentVariable("CMU_OAUTH_URL");
                String grant_type = "authorization_code";
                String urlgettoken = Environment.GetEnvironmentVariable("CMU_GETTOKEN_URL");

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
                Cmuaccount = responseprofile.cmuitaccount;
                UserAdminEntity userAdminEntity = await _IAdminRepository.getAdminByEmail(responseprofile.cmuitaccount);
                if (userAdminEntity == null) { return Unauthorized(); }
                String RefCode = await _IAdminRepository.sendLoginOTP(responseprofile.cmuitaccount, _access_token, _refresh_token);
                aPIModel.data = RefCode;
                aPIModel.title = "Success";
                return StatusCodeITSC("CMU", "", responseprofile.cmuitaccount, "LoginController.admincallback", 200, aPIModel);
            }
            catch (Exception ex)
            { return StatusErrorITSC("CMU", "", Cmuaccount, "LoginController.admincallback", ex); }



        }


        [HttpPost("v1/admin/loginotp")]
        [ProducesResponseType(typeof(UserAdminModelView), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AdminloginOTP([FromBody] AdminLoginOTPModelview data)
        {

            APIModel aPIModel = new APIModel();
            UserAdminEntity userAdminEntity = await _IAdminRepository.getAdminByOTP(data, getClientIP());
            if (userAdminEntity == null)
            {
                aPIModel.title = "รหัส OTP ไม่ถูกต้อง หรือ หมดอายุ";
                return StatusCodeITSC("CMU", "", "", "LoginController.AdminloginOTP", 503, aPIModel);
            }
             
            userAdminEntity.Tel = "";
            userAdminEntity.Refresh_token = "";
            String json = JsonConvert.SerializeObject(userAdminEntity);
            UserAdminModelView userAdminModelView = JsonConvert.DeserializeObject<UserAdminModelView>(json);
         
            aPIModel.data = userAdminEntity;
            aPIModel.title = "Success";
            return StatusCodeITSC("CMU", "", userAdminEntity.Cmuaccount, "LoginController.AdminloginOTP", 200, aPIModel);

        }

    }
}
