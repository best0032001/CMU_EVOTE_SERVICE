using Evote_Service.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.Repository
{
    public class EmailRepository : IEmailRepository
    {
        public async Task<string> SendEmailOTP(string Email)
        {
            throw new NotImplementedException();
        }
    }
}
