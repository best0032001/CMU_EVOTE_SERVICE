using Evote_Service.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.Repository.Mock
{
    public class EmailRepositoryMock : IEmailRepository
    {
        public async Task<string> SendEmailOTP(string Email)
        {
            return "1234";
        }
    }
}
