using Evote_Service.Model;
using Evote_Service.Model.Interface;
using Evote_Service.Model.View;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
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
        protected String getClientIP()
        {
            String _forwardIPTemp = "";
            if (!_env.IsEnvironment("test"))
            {
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Development")
                {
                    var req = Request;
                    String remoteIpAddress = req.HttpContext.Connection.RemoteIpAddress.ToString();
                    remoteIpAddress = remoteIpAddress.Split(":")[3];

                    String gateWayIP = Environment.GetEnvironmentVariable("GATEWAY_IP");
                    List<String> gateWayIPList = gateWayIP.Split(" ").ToList();
                    if (!gateWayIPList.Contains(remoteIpAddress.Trim()))
                    {
                        throw new UnauthorizedAccessException();
                    }
                    _forwardIPTemp = Request.Headers["x-forwarded-for"];
                    _forwardIPTemp = _forwardIPTemp.Split(',')[0];

                }
            }
            return _forwardIPTemp;
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

        protected FileModel SaveFile(String folderName, IFormFile formFile, Int32 maxMb)
        {

            FileModel fileModel = new FileModel();
            String fileName = "";
            try
            {
                var file = formFile;
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (file.Length > 0)
                {

                    int MaxContentLength = 1024 * 1024 * 20 * maxMb; //Size = 5 MB  
                    IList<string> AllowedFileExtensions = new List<string> { ".heic", ".jfif", ".jpeg", ".jpg", ".gif", ".png", ".docx", ".doc", ".pdf", ".xlsx", ".xls", ".csv" };
                    var ext = formFile.FileName.Substring(formFile.FileName.LastIndexOf('.'));
                    var extension = ext.ToLower();
                    if (!AllowedFileExtensions.Contains(extension))
                    {
                        fileModel.isSave = false;
                        fileModel.fileName = "no save - Please Upload  type .heic,.jfif,.jpg,.gif,.png,.docx,.doc,.pdf,.xlsx,.xls,.csv";
                        return fileModel;
                    }
                    else if (formFile.Length > MaxContentLength)
                    {

                        fileModel.isSave = false;
                        fileModel.fileName = "no save - MaxContentLength" + maxMb + "Mb";
                        return fileModel;
                    }
                    int num = new Random().Next(100000);
                    using (SHA1Managed sha1 = new SHA1Managed())
                    {
                        var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(formFile.FileName + num));
                        var sb = new StringBuilder(hash.Length * 2);
                        foreach (byte b in hash)
                        {
                            // can be "x2" if you want lowercase
                            sb.Append(b.ToString("X2"));
                        }
                        fileName = sb.ToString();
                    }
                    fileName = fileName + extension;
                    fileModel.fullPath = Path.Combine(pathToSave, fileName);
                    fileModel.dbPath = Path.Combine(folderName, fileName);
                    fileModel.fileName = fileName;
                    fileModel.isSave = false;
                    using (var stream = new FileStream(fileModel.fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                        fileModel.isSave = true;
                    }
                    if (fileModel.isSave == false)
                    {
                        fileModel.fileName = "no file";
                    }


                }
                else
                {
                    fileModel.isSave = false;
                    fileModel.fileName = "no file";
                }
            }
            catch (Exception ex)
            {
                fileModel.isSave = false;
                if (ex.InnerException != null)
                {
                    fileModel.error = ex.Message + " " + ex.StackTrace + " " + ex.InnerException.StackTrace;
                }
                else
                {
                    fileModel.error = ex.Message + " " + ex.StackTrace;
                }
                fileModel.fileName = "error";
            }

            return fileModel;
        }

        protected MemoryStream loadFile(String fullPath)
        {
            try
            {
                var memory = new MemoryStream();
                using (var stream = new FileStream(fullPath, FileMode.Open))
                {
                    stream.CopyTo(memory);
                }
                return memory;
            }
            catch { return null; }
        }
        protected string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }
        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
    }
}
