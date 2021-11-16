using Evote_Service.Model.Entity;
using Evote_Service.Model.Interface;
using Evote_Service.Model.View;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Evote_Service.Model.Repository.Mock
{
    public class AdminRepositoryMock : IAdminRepository
    {
        public async Task<List<UserEntity>> getUserWaitForApprove(String cmuaccount)
        {
            throw new NotImplementedException();
        }
    }
}
