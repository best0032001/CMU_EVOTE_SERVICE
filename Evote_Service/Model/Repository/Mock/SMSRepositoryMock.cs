using Evote_Service.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.Repository.Mock
{
    public class SMSRepositoryMock : ISMSRepository
    {
        public async Task<string> getOTP()
        {
            return "1234";
        }
    }
}
