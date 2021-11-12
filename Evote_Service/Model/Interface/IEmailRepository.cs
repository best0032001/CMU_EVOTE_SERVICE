using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.Interface
{
    public interface IEmailRepository
    {
        Task<String> SendEmailOTP(String Email,String code);
        Task SendEmailAsync(String nameSender, string email_To, string subject, string message, List<IFormFile> Attachment);
    }
}
