using Evote_Service.Model.Entity;
using Evote_Service.Model.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.Interface
{
    public interface IUserManageRepository
    {
        Task<List<UserEntity>> searchUser(AdminSearchModelView adminSearchModelView, String cmuaccount);
        Task<List<UserEntity>> deActiveUser(String cmuaccount, Int32 userEntityId, String clientIP);
    }
}
