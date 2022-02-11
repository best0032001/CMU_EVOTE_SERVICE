using Evote_Service.Model;
using Evote_Service.Model.Interface;
using Evote_Service.Model.Util;
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

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Controllers
{
    [Route("api/")]
    [ApiController]
    public class MailTestController : ControllerBase
    {
        protected IEmailRepository _emailRepository;
        public MailTestController(IEmailRepository emailRepository)
        {
            _emailRepository = emailRepository;
        }

        [HttpGet("v1/MailTest")]
        public async Task<ActionResult> mail()
        {
            //String NOTI_ADMIN = Environment.GetEnvironmentVariable("NOTI_ADMIN");
            //String Reply = Environment.GetEnvironmentVariable("MAIL_REPLY");
            //await _emailRepository.SendEmailAsync("CMU Evote service", NOTI_ADMIN, "no reply", "test", null);
            //await _emailRepository.SendEmailAsync("CMU Evote service", Reply,NOTI_ADMIN ," with reply", "test", null);

            return Ok();
        }
    }
}
