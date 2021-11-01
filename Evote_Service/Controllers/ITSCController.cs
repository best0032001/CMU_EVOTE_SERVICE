﻿using Evote_Service.Model;
using Evote_Service.Model.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Evote_Service.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ITSCController : ControllerBase
    {
        protected DateTime _timestart;
        protected ILogger<ITSCController> _logger;
        protected IHttpClientFactory _clientFactory;
        protected IWebHostEnvironment _env;
        protected String _accesstoken = "";
        protected IEmailRepository _emailRepository;
        private String urlLine = "https://api.line.me/v2/profile";
        public ITSCController()
        {
        }
        protected void loadConfig(ILogger<ITSCController> logger, IHttpClientFactory clientFactory, IWebHostEnvironment env)
        {
            _timestart = DateTime.Now;
            _clientFactory = clientFactory;
            _env = env;
            _logger = logger;

        }
        protected void loadConfig(ILogger<ITSCController> logger, IHttpClientFactory clientFactory)
        {
            _timestart = DateTime.Now;
            _clientFactory = clientFactory;
            _logger = logger;

        }
        public String getTokenFormHeader()
        {
            _accesstoken = Request.Headers["Authorization"];
            _accesstoken = _accesstoken.Split(' ')[1];
            return _accesstoken;

        }

        public async Task<String> getLineUser()
        {
            if (_accesstoken == "") { getTokenFormHeader(); }
            String _lineId = "";
            if (_env.IsEnvironment("test")) { return DataCache.UserMocks.Where(w => w.token == _accesstoken).First().lineId; }
            HttpClient httpClient = _clientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accesstoken);
            var response = await httpClient.GetAsync(urlLine);
            if (response.IsSuccessStatusCode) { dynamic data = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync()); _lineId = data.userId; }
            else { _lineId = "unauthorized"; }
            return _lineId;

        }
        protected StatusCodeResult StatusErrorITSC(String UserType, String LineID, String cmuaccount, String action, Exception ex)
        {
            LogModel log = new LogModel();
            log.UserType = UserType;
            log.LineID = LineID;
            log.cmuaccount = cmuaccount;
            log.HttpCode = "500";
            log.action = action;
            log.level = "Error";
            log.Timestamp = DateTime.Now;
            log.logdate = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
            log.logdata = ex.Message + " " + ex.StackTrace.Replace("\\", "").Replace(":", "");
            if (ex.InnerException != null)
            {
                log.logdata = log.logdata + " " + ex.InnerException.Message + " " + ex.InnerException.StackTrace.Replace("\\", "").Replace(":", "");
            }
            log.responseTime = (log.Timestamp - _timestart).TotalSeconds;
            String errorText = log.logdate + " " + Newtonsoft.Json.JsonConvert.SerializeObject(log);
            _logger.LogInformation(errorText);
            String NOTI_ADMIN = Environment.GetEnvironmentVariable("NOTI_ADMIN");
            _emailRepository.SendEmailAsync("CMU Evote service", NOTI_ADMIN, "Error Alert", errorText, null);
            return this.StatusCode(500);
        }
        protected ObjectResult StatusCodeITSC(String UserType, String LineID, String cmuaccount, String action, Int32 code, APIModel aPIModel)
        {
            LogModel log = new LogModel();
            log.UserType = UserType;
            log.LineID = LineID;
            log.cmuaccount = cmuaccount;
            log.HttpCode = "" + code;
            log.action = action;
            log.level = "Info";
            log.Timestamp = DateTime.Now;
            log.logdate = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
            log.logdata = "";
            log.responseTime = (log.Timestamp - _timestart).TotalSeconds;
            _logger.LogInformation(log.logdate + " " + Newtonsoft.Json.JsonConvert.SerializeObject(log));
            return this.StatusCode(code, aPIModel);
        }

    }
}