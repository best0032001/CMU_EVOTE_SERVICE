using Evote_Service.Model.Entity;
using Evote_Service.Model.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.Interface
{
    public interface IAdminRepository
    {
        Task<List<UserEntity>> getUserWaitForApprove(String cmuaccount);

        Task<List<UserEntity>> adminApprove(String cmuaccount,Int32 userEntityId,String clientIP);

        Task<List<UserEntity>> adminNotApprove(string cmuaccount, AdminApproveModelView adminApproveModelView, String clientIP);

        Task<UserAdminEntity> getAdminByEmail(String cmuaccount);

        Task<UserAdminEntity> getAdminByOTP(AdminLoginOTPModelview adminLoginOTPModelview,String clientIP);

        Task<String> sendLoginOTP(String cmuaccount,String _access_token,String _refresh_token);

    }
}
