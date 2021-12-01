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
        Task SendEmailAsync(string nameSender, string email_To, string subject, string message, List<IFormFile> Attachment);
        Task SendEmailAsync(string nameSender, string reply, string email_To, string subject, string message, List<IFormFile> Attachment);
    }
}
