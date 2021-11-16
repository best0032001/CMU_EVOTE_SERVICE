using Evote_Service.Model.Entity;
using Evote_Service.Model.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.Interface
{
    public interface ICheckUserRepository
    {

        Task<UserModel> GetLineUserModel(String lineId);

        Task<UserEntity> GetLineUserEntity(String lineId);
        Task<Boolean> RegisLineUser(UserEntity userEntity);

        Task<Boolean> UserSendTel(String lineId, String tel);
        Task<Boolean> UserSendEmail(String lineId, String email);
        Task<Boolean> UserConfirmSMSOTP(String lineId, String otp);
        Task<Boolean> getEMAILOTP(String lineId);

        Task<Boolean> UserConfirmEmailOTP(String lineId, String otp);

        Task<Boolean> UserPostphotoId(String lineId, FileModel fileModel);

        Task<Boolean> UserPostphotoKyc(String lineId, FileModel fileModel);
    }
}
