using Evote_Service.Model.Entity;
using Evote_Service.Model.Interface;
using Evote_Service.Model.Util;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Evote_Service.Model.Repository
{
    public class CheckUserRepository : ICheckUserRepository
    {
        private EvoteContext _evoteContext;
        private readonly IHttpClientFactory _clientFactory;
        private ISMSRepository _sMSRepository;
        private IEmailRepository  _emailRepository;
        public CheckUserRepository(EvoteContext evoteContext, IHttpClientFactory clientFactory,ISMSRepository sMSRepository, IEmailRepository emailRepository)
        {
            if (evoteContext == null)
            {
                throw new System.ArgumentNullException(nameof(evoteContext));
            }
            _evoteContext = evoteContext;
            _clientFactory = clientFactory;
            _sMSRepository = sMSRepository;
            _emailRepository = emailRepository;
        }
        public async Task<UserModel> GetLineUserModel(string lineId)
        {
            UserEntity userEntitys= _evoteContext.UserEntitys.Where(w => w.LineId == lineId).FirstOrDefault();
            if (userEntitys == null){ return null; }
            return JsonConvert.DeserializeObject<UserModel>(JsonConvert.SerializeObject(userEntitys)); ;
        }

        public async Task<bool> RegisLineUser(UserEntity userEntity)
        {
            if (_evoteContext.UserEntitys.Where(w => w.LineId == userEntity.LineId).FirstOrDefault() != null) { return false; }
            userEntity.UserType = 2;
            userEntity.CreateTime = DateTime.Now;
            userEntity.UserStage = 1;
            _evoteContext.UserEntitys.Add(userEntity);
            _evoteContext.SaveChanges();
            return true;
        }

        public async Task<bool> UserSendTel(string lineId, string tel)
        {
            UserEntity userEntitys= _evoteContext.UserEntitys.Where(w => w.LineId == lineId).FirstOrDefault();
            if (userEntitys == null) { return false; }
            if (userEntitys.UserStage!=1) { return false; }
            if (userEntitys.IsConfirmTel ==true) { return false; }
            userEntitys.Tel = tel;
            
            Random _random = new Random();
            String code = _random.Next(0, 99999999).ToString("D8");
            //  sent OTP
            userEntitys.SMSOTP = await _sMSRepository.getOTP(code, userEntitys.Tel) ;
            userEntitys.SMSOTPRef = code;
            _evoteContext.SaveChanges();
            return true;
        }

        public async Task<bool> UserConfirmSMSOTP(string lineId, string otp)
        {
            UserEntity userEntitys = _evoteContext.UserEntitys.Where(w => w.LineId == lineId).FirstOrDefault();
            if (userEntitys == null) { return false; }
            if (userEntitys.UserStage != 1) { return false; }
            if (userEntitys.IsConfirmTel == true) { return false; }

            if (userEntitys.SMSOTP != otp) { return false; }
            userEntitys.IsConfirmTel = true;
            userEntitys.ConfirmTelTime = DateTime.Now;
            CheckUserStage(userEntitys);
            _evoteContext.SaveChanges();
            return true;
        }

        public async Task<bool> getEMAILOTP(string lineId)
        {
            UserEntity userEntitys = _evoteContext.UserEntitys.Where(w => w.LineId == lineId).FirstOrDefault();
            if (userEntitys == null) { return false; }
            if (userEntitys.UserStage != 1) { return false; }
            if (userEntitys.IsConfirmEmail == true) { return false; }
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            String code = new string(Enumerable.Repeat(chars, 4)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            userEntitys.EmailOTP = await _emailRepository.SendEmailOTP(userEntitys.Email, code);
            userEntitys.EmailOTPRef = code;

            _evoteContext.SaveChanges();
            return true;
        }

        public async Task<bool> UserConfirmEmailOTP(string lineId, string otp)
        {
            UserEntity userEntitys = _evoteContext.UserEntitys.Where(w => w.LineId == lineId).FirstOrDefault();
            if (userEntitys == null) { return false; }
            if (userEntitys.UserStage != 1) { return false; }
            if (userEntitys.IsConfirmEmail == true) { return false; }

            if (userEntitys.EmailOTP != otp) { return false; }
            userEntitys.IsConfirmEmail = true;
            userEntitys.ConfirmEmailTime = DateTime.Now;
            userEntitys.EmailOTPRef = "";
            CheckUserStage(userEntitys);
            _evoteContext.SaveChanges();
            return true;
        }
        public async Task<bool> checkEmail(string email)
        {
            UserEntity userEntitys= _evoteContext.UserEntitys.Where(w => w.Email == email&w.IsConfirmEmail==true).FirstOrDefault();
            if (userEntitys == null) { return true; }
            return false;
        }
        public async Task<bool> CheckTel(string tel)
        {
            UserEntity userEntitys = _evoteContext.UserEntitys.Where(w => w.Tel == tel & w.IsConfirmTel == true).FirstOrDefault();
            if (userEntitys == null) { return true; }
            return false;
        }
        public async Task<bool> UserSendEmail(string lineId, string email)
        {
            UserEntity userEntitys = _evoteContext.UserEntitys.Where(w => w.LineId == lineId).FirstOrDefault();
            if (userEntitys == null) { return false; }
            if (userEntitys.UserStage != 1) { return false; }
            if (userEntitys.IsConfirmEmail == true) { return false; }
            userEntitys.Email = email;

            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            String code = new string(Enumerable.Repeat(chars, 4)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            userEntitys.EmailOTP = await _emailRepository.SendEmailOTP(userEntitys.Email, code);
            userEntitys.EmailOTPRef = code;
            _evoteContext.SaveChanges();
            return true;
        }

        public async Task<bool> UserPostphotoId(string lineId, FileModel fileModel)
        {
            UserEntity userEntitys = _evoteContext.UserEntitys.Where(w => w.LineId == lineId).FirstOrDefault();
            if (userEntitys == null) { return false; }
            if (userEntitys.UserStage != 1) { return false; }
            userEntitys.fileNamePersonalID = fileModel.fileName;
            userEntitys.fullPathPersonalID = fileModel.fullPath;
            userEntitys.dbPathPersonalID = fileModel.dbPath;
            userEntitys.IsConfirmPersonalID = true;
            CheckUserStage(userEntitys);
            _evoteContext.SaveChanges();
            return true;
        }

        public async Task<UserEntity> GetLineUserEntity(string lineId)
        {
            UserEntity userEntitys = _evoteContext.UserEntitys.Where(w => w.LineId == lineId).FirstOrDefault();
            if (userEntitys == null) { return null; }
            return userEntitys;
        }

        public async Task<bool> UserPostphotoKyc(string lineId, FileModel fileModel)
        {
            UserEntity userEntitys = _evoteContext.UserEntitys.Where(w => w.LineId == lineId).FirstOrDefault();
            if (userEntitys == null) { return false; }
            if (userEntitys.UserStage != 1) { return false; }
            userEntitys.fileNameKYC = fileModel.fileName;
            userEntitys.fullPathKYC = fileModel.fullPath;
            userEntitys.dbPathKYC = fileModel.dbPath;
            userEntitys.IsConfirmKYC = true;
            CheckUserStage(userEntitys);
            _evoteContext.SaveChanges();
            return true;
        }

        private void CheckUserStage(UserEntity userEntitys)
        {
            if (userEntitys.IsConfirmEmail == true && userEntitys.IsConfirmKYC == true && userEntitys.IsConfirmPersonalID == true && userEntitys.IsConfirmTel == true)
            {
                userEntitys.UserStage = 2;
            }
        }

        public async Task<bool> RegisCMUUser(UserEntity userEntity)
        {
            if (_evoteContext.UserEntitys.Where(w => w.LineId == userEntity.LineId).FirstOrDefault() != null) { return false; }
            userEntity.UserType = 1;
            userEntity.CreateTime = DateTime.Now;
            userEntity.UserStage = 1;
            _evoteContext.UserEntitys.Add(userEntity);
            _evoteContext.SaveChanges();
            return true;
        }
    }
}
