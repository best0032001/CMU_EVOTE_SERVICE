using Evote_Service.Model.Entity;
using Evote_Service.Model.Interface;
using Evote_Service.Model.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Evote_Service.Model.Repository.Mockw
{
    public class AdminRepositoryMock : IAdminRepository
    {
        public async Task<List<UserEntity>> adminApprove(string cmuaccount, int userEntityId)
        {
            throw new NotImplementedException();
        }

        public Task<List<UserEntity>> adminApprove(string cmuaccount, int userEntityId, string clientIP)
        {
            throw new NotImplementedException();
        }

        public Task<List<UserEntity>> adminNotApprove(string cmuaccount, int userEntityId, string comment)
        {
            throw new NotImplementedException();
        }

        public Task<List<UserEntity>> adminNotApprove(string cmuaccount, int userEntityId, string comment, string clientIP)
        {
            throw new NotImplementedException();
        }

        public async Task<List<UserEntity>> getUserWaitForApprove(String cmuaccount)
        {
            throw new NotImplementedException();
        }
    }
}
