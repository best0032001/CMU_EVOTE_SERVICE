using Evote_Service.Model.Entity;
using Evote_Service.Model.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.Interface
{
    public interface ICheckUserRepository
    {
        Task<Boolean> reset(String lineId);
        Task<UserModel> GetLineUserModel(String lineId);

        Task<UserEntity> GetLineUserEntity(String lineId);
        Task<Boolean> RegisLineUser(UserEntity userEntity);

        Task<Boolean> RegisCMUUser(UserEntity userEntity);

        Task<Boolean> CheckTel(String tel);
        Task<Boolean> UserSendTel(String lineId, String tel);
        Task<Boolean> checkEmail(String email);
        Task<Boolean> UserSendEmail(String lineId, String email);
        Task<Boolean> UserConfirmSMSOTP(String lineId, String otp);
        Task<Boolean> getEMAILOTP(String lineId);

        Task<Boolean> UserConfirmEmailOTP(String lineId, String otp);

        Task<Boolean> UserPostphotoId(String lineId, FileModel fileModel,String PersonalID);

        Task<Boolean> UserPostphotoKyc(String lineId, FileModel fileModelFace, String facedata);

        Task<Boolean> CheckPersonalID(String PersonalID);
    }
}
