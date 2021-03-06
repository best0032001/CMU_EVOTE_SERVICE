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
        private IEmailRepository _emailRepository;
        private ApplicationDBContext _applicationDBContext;
        public CheckUserRepository(EvoteContext evoteContext, IHttpClientFactory clientFactory, ISMSRepository sMSRepository, IEmailRepository emailRepository, ApplicationDBContext applicationDBContext)
        {
            if (evoteContext == null)
            {
                throw new System.ArgumentNullException(nameof(evoteContext));
            }
            _evoteContext = evoteContext;
            _clientFactory = clientFactory;
            _sMSRepository = sMSRepository;
            _emailRepository = emailRepository;
            _applicationDBContext = applicationDBContext;
        }
        public async Task<UserModel> GetLineUserModel(string lineId)
        {
            UserEntity userEntitys = _evoteContext.UserEntitys.Where(w => w.LineId == lineId&&w.IsDeactivate==false).FirstOrDefault();
            if (userEntitys == null) { return null; }
            if (userEntitys.IsConfirmEmail == false)
            { userEntitys.Email = ""; }
            if (userEntitys.IsConfirmTel == false)
            { userEntitys.Tel = ""; }


            UserModel userModel = JsonConvert.DeserializeObject<UserModel>(JsonConvert.SerializeObject(userEntitys));

            String RAW_KEY = Environment.GetEnvironmentVariable("RAW_KEY");
            String PASS_KEY = Environment.GetEnvironmentVariable("PASS_KEY");
            Crypto crypto = new Crypto(PASS_KEY, RAW_KEY);
            try
            {
                userModel.Tel = crypto.DecryptFromBase64(userModel.Tel);
                userModel.PersonalID = crypto.DecryptFromBase64(userModel.PersonalID);
            }
            catch { }

            return userModel;
        }

        public async Task<bool> RegisLineUser(UserEntity userEntity)
        {
            if (_evoteContext.UserEntitys.Where(w => w.LineId == userEntity.LineId).FirstOrDefault() != null) { return false; }
            userEntity.UserType = 2;
            userEntity.CreateTime = DateTime.Now;
            userEntity.UserStage = 1;
            Random _random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            String code = new string(Enumerable.Repeat(chars, 16)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
            userEntity.UserCode = code;
            _evoteContext.UserEntitys.Add(userEntity);
            _evoteContext.SaveChanges();
            return true;
        }

        public async Task<bool> UserSendTel(string lineId, string tel)
        {
            UserEntity userEntitys = _evoteContext.UserEntitys.Where(w => w.LineId == lineId).FirstOrDefault();
            if (userEntitys == null) { return false; }
            if (userEntitys.UserStage != 1) { return false; }
            if (userEntitys.IsConfirmTel == true) { return false; }

            String RAW_KEY = Environment.GetEnvironmentVariable("RAW_KEY");
            String PASS_KEY = Environment.GetEnvironmentVariable("PASS_KEY");
            Crypto crypto = new Crypto(PASS_KEY, RAW_KEY);
            userEntitys.Tel = crypto.Encrypt(tel);

            Random _random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            String code = new string(Enumerable.Repeat(chars, 4)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
            //  sent OTP
            userEntitys.SMSOTP = await _sMSRepository.getOTP(code, tel);
            userEntitys.SMSExpire = DateTime.Now.AddMinutes(5);
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
            int res = DateTime.Compare(DateTime.Now, (DateTime)userEntitys.SMSExpire);
            if (res >= 0) { return false; }
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
            UserEntity userEntitys = _evoteContext.UserEntitys.Where(w => w.Email == email & w.IsConfirmEmail == true && w.IsDeactivate == false).FirstOrDefault();
            if (userEntitys == null) { return true; }
            return false;
        }
        public async Task<bool> CheckTel(string tel)
        {

            String RAW_KEY = Environment.GetEnvironmentVariable("RAW_KEY");
            String PASS_KEY = Environment.GetEnvironmentVariable("PASS_KEY");
            Crypto crypto = new Crypto(PASS_KEY, RAW_KEY);
            tel = crypto.Encrypt(tel);
            UserEntity userEntitys = _evoteContext.UserEntitys.Where(w => w.Tel == tel & w.IsConfirmTel == true && w.IsDeactivate == false).FirstOrDefault();
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

        public async Task<bool> UserPostphotoId(string lineId, FileModel fileModel, String PersonalID)
        {
            UserEntity userEntitys = _evoteContext.UserEntitys.Where(w => w.LineId == lineId).FirstOrDefault();
            if (userEntitys == null) { return false; }
            if (userEntitys.UserStage != 1) { return false; }
            String RAW_KEY = Environment.GetEnvironmentVariable("RAW_KEY");
            String PASS_KEY = Environment.GetEnvironmentVariable("PASS_KEY");
            Crypto crypto = new Crypto(PASS_KEY, RAW_KEY);
            userEntitys.fileNamePersonalID = fileModel.fileName;
            userEntitys.fullPathPersonalID = fileModel.fullPath;
            userEntitys.dbPathPersonalID = fileModel.dbPath;
            userEntitys.IsConfirmPersonalID = true;
            userEntitys.PersonalID = crypto.Encrypt(PersonalID);
            userEntitys.ConfirmPersonalIDTime = DateTime.Now;
            CheckUserStage(userEntitys);
            _evoteContext.SaveChanges();
            return true;
        }
        public async Task<Boolean> CheckPersonalID(String PersonalID)
        {
            String RAW_KEY = Environment.GetEnvironmentVariable("RAW_KEY");
            String PASS_KEY = Environment.GetEnvironmentVariable("PASS_KEY");
            Crypto crypto = new Crypto(PASS_KEY, RAW_KEY);
            PersonalID = crypto.Encrypt(PersonalID);
            UserEntity userEntitys = _evoteContext.UserEntitys.Where(w => w.PersonalID == PersonalID && w.IsDeactivate == false).FirstOrDefault();
            if (userEntitys == null) { return true; }
            return false;
        }
        public async Task<UserEntity> GetLineUserEntity(string lineId)
        {
            UserEntity userEntitys = _evoteContext.UserEntitys.Where(w => w.LineId == lineId && w.IsDeactivate == false).FirstOrDefault();
            if (userEntitys == null) { return null; }
            return userEntitys;
        }

        public async Task<bool> UserPostphotoKyc(string lineId, FileModel fileModelFace, String faceData)
        {
            UserEntity userEntitys = _evoteContext.UserEntitys.Where(w => w.LineId == lineId).FirstOrDefault();
            if (userEntitys == null) { return false; }
            if (userEntitys.UserStage != 1) { return false; }
            //userEntitys.fileNameKYC = fileModelKYC.fileName;
            //userEntitys.fullPathKYC = fileModelKYC.fullPath;
            //userEntitys.dbPathKYC = fileModelKYC.dbPath;

            userEntitys.fileNameFace = fileModelFace.fileName;
            userEntitys.fullPathFace = fileModelFace.fullPath;
            userEntitys.dbPathFace = fileModelFace.dbPath;

            userEntitys.faceData = faceData;

            userEntitys.IsConfirmKYC = true;
            userEntitys.ConfirmKYCTime = DateTime.Now;
            CheckUserStage(userEntitys);
            _evoteContext.SaveChanges();
            return true;
        }

        private void CheckUserStage(UserEntity userEntitys)
        {
            if (userEntitys.IsConfirmEmail == true && userEntitys.IsConfirmKYC == true && userEntitys.IsConfirmPersonalID == true && userEntitys.IsConfirmTel == true)
            {
                userEntitys.UserStage = 2;
                String NOTI_ADMIN = Environment.GetEnvironmentVariable("NOTI_ADMIN");
                String body = userEntitys.FullName + " รอการตรวจสอบยืนยันตัวตน";
                _emailRepository.SendEmailAsync("CMU Evote service", NOTI_ADMIN, "มีผู้ลงทะเบียนใหม่", body, null);
            }
            if (userEntitys.UserType == 1 && userEntitys.IsConfirmTel == true)
            {
                userEntitys.UserStage = 3;
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

        public async Task<bool> reset(string lineId)
        {
            UserEntity userEntitys = _evoteContext.UserEntitys.Where(w => w.LineId == lineId).FirstOrDefault();
            if (userEntitys == null) { return false; }
            if (userEntitys.UserStage != 4) { return false; }
            userEntitys.UserStage = 1;
            userEntitys.PersonalID = "";
            userEntitys.Tel = "";
            userEntitys.IsConfirmEmail = false;
            userEntitys.ConfirmEmailTime = null;
            userEntitys.IsConfirmKYC = false;
            userEntitys.ConfirmKYCTime = null;

            userEntitys.IsConfirmPersonalID = false;
            userEntitys.ConfirmPersonalIDTime = null;
            userEntitys.IsConfirmTel = false;
            userEntitys.ConfirmTelTime = null;
            _evoteContext.SaveChanges();

            return true;
        }

        public async Task<UserModel> getVotePermission(string lineId, int EvoteServiceEventVoteEntityId,int VoteRound)
        {
            UserEntity userEntitys = _evoteContext.UserEntitys.Where(w => w.LineId == lineId && w.IsDeactivate == false).FirstOrDefault();
            if (userEntitys == null) { return null; }
            VoterEntity voterEntity = _evoteContext.VoterEntitys.Where(w => w.EventVoteEntityId == EvoteServiceEventVoteEntityId && w.Email == userEntitys.Email).FirstOrDefault();
            if (voterEntity == null) { return null; }

            ConfirmVoter confirmVoter = _evoteContext.confirmVoters.Where(w => w.email == userEntitys.Email && w.EventVoteEntityId == EvoteServiceEventVoteEntityId && w.RoundNumber == VoteRound).FirstOrDefault();
            if (confirmVoter != null) { return null; }
            if (userEntitys.IsConfirmEmail == false)
            { userEntitys.Email = ""; }
            if (userEntitys.IsConfirmTel == false)
            { userEntitys.Tel = ""; }

            UserModel userModel = JsonConvert.DeserializeObject<UserModel>(JsonConvert.SerializeObject(userEntitys));
            return userModel;
        }

        public async Task<UserModel> GetLineUserModelByRegisProcess(string lineId)
        {
            UserEntity userEntitys = _evoteContext.UserEntitys.Where(w => w.LineId == lineId && w.IsDeactivate == false).FirstOrDefault();
            if (userEntitys == null) { return null; }
            if (userEntitys.IsConfirmEmail == false)
            { userEntitys.Email = ""; }
            if (userEntitys.IsConfirmTel == false)
            { userEntitys.Tel = ""; }

            if (userEntitys.UserType == 1 && userEntitys.IsConfirmTel == false)
            {
                _evoteContext.UserEntitys.Remove(userEntitys);
                _evoteContext.SaveChanges();

                return null;
            }

            UserModel userModel = JsonConvert.DeserializeObject<UserModel>(JsonConvert.SerializeObject(userEntitys));

            String RAW_KEY = Environment.GetEnvironmentVariable("RAW_KEY");
            String PASS_KEY = Environment.GetEnvironmentVariable("PASS_KEY");
            Crypto crypto = new Crypto(PASS_KEY, RAW_KEY);
            try
            {
                userModel.Tel = crypto.DecryptFromBase64(userModel.Tel);
                userModel.PersonalID = crypto.DecryptFromBase64(userModel.PersonalID);
            }
            catch { }

            return userModel;
        }
    }
}
