using Evote_Service.Model.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.Interface
{
    public interface IUserAdminRepository
    {
        Task<List<AdminModelView>> searchAdmin(AdminModelView adminModelView, String cmuaccount);
        Task<Boolean> addAdmin(AdminModelView adminModelView, String cmuaccount, String ip);
        Task<Boolean> deleteAdmin(int UserAdminEntityId);
    }
}
