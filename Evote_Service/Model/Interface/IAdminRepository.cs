using Evote_Service.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.Interface
{
    public interface IAdminRepository
    {
        Task<List<UserEntity>> getUserWaitForApprove(String cmuaccount);
    }
}
