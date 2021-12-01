using Evote_Service.Model.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.Repository.Mock
{
    public class EmailRepositoryMock : IEmailRepository
    {
        public async Task SendEmailAsync(string nameSender, string email_To, string subject, string message, List<IFormFile> Attachment)
        {
            
        }

        public Task SendEmailAsync(string nameSender, string reply, string email_To, string subject, string message, List<IFormFile> Attachment)
        {
            throw new NotImplementedException();
        }

        public async Task<string> SendEmailOTP(string Email, String codeRef)
        {
            return "1234";
        }
    }
}
