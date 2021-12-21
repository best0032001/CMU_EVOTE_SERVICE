using Evote_Service.Model.Entity;
using Evote_Service.Model.Interface;
using Evote_Service.Model.Util;
using Evote_Service.Model.View;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Evote_Service.Model.Repository
{
    public class AdminRepository : IAdminRepository
    {
        private EvoteContext _evoteContext;
        private readonly IHttpClientFactory _clientFactory;
        private ISMSRepository _sMSRepository;
        private IEmailRepository _emailRepository;
        public AdminRepository(EvoteContext evoteContext, IHttpClientFactory clientFactory, IEmailRepository emailRepository, ISMSRepository sMSRepository)
        {
            if (evoteContext == null)
            {
                throw new System.ArgumentNullException(nameof(evoteContext));
            }
            _evoteContext = evoteContext;
            _emailRepository = emailRepository;
            _sMSRepository = sMSRepository;

        }
        public async Task<List<UserEntity>> getUserWaitForApprove(String cmuaccount)
        {
            List<UserEntity> userEntities = new List<UserEntity>();
            UserAdminEntity userAdminEntity = _evoteContext.UserAdminEntitys.Where(w => w.Cmuaccount == cmuaccount).FirstOrDefault();
            if (userAdminEntity == null) { return null; }
            userEntities = _evoteContext.UserEntitys.Where(w => w.UserStage == 2).ToList();
            return userEntities;

        }

        public async Task<List<UserEntity>> adminApprove(string cmuaccount, Int32 userEntityId, String clientIP)
        {
            List<UserEntity> userEntities = new List<UserEntity>();
            UserAdminEntity userAdminEntity = _evoteContext.UserAdminEntitys.Where(w => w.Cmuaccount == cmuaccount).FirstOrDefault();
            if (userAdminEntity == null) { return null; }
            UserEntity userEntity = _evoteContext.UserEntitys.Where(w => w.UserStage == 2 && w.UserEntityId == userEntityId).FirstOrDefault();
            if (userEntity == null) { return null; }
            userEntity.UserStage = 3;
            userEntity.AdminApprovedIP = clientIP;
            userEntity.AdminApproved = cmuaccount;
            userEntity.ApprovedTime = DateTime.Now;

            List<VoterEntity> voterEntitys = _evoteContext.VoterEntitys.Where(w => w.Email == userEntity.Email).ToList();
            foreach (VoterEntity voterEntity in voterEntitys)
            {
                EventVoteEntity eventVoteEntity = _evoteContext.EventVoteEntitys.Where(w => w.EventVoteEntityId == voterEntity.EventVoteEntityId).FirstOrDefault();
                if (eventVoteEntity != null)
                {
                    userEntity.eventVoteEntities.Add(eventVoteEntity);
                }
            }
            _evoteContext.SaveChanges();

            await _emailRepository.SendEmailAsync("CMU Evote service", userEntity.Email, "การยืนยันตัวของท่านได้รับการตรวจสอบแล้ว", "  ", null);
            userEntities = _evoteContext.UserEntitys.Where(w => w.UserStage == 2).ToList();
            return userEntities;
        }

        public async Task<List<UserEntity>> adminNotApprove(string cmuaccount, AdminApproveModelView adminApproveModelView, String clientIP)
        {
            List<UserEntity> userEntities = new List<UserEntity>();
            UserAdminEntity userAdminEntity = _evoteContext.UserAdminEntitys.Where(w => w.Cmuaccount == cmuaccount).FirstOrDefault();
            if (userAdminEntity == null) { return null; }
            UserEntity userEntity = _evoteContext.UserEntitys.Where(w => w.UserStage == 2 && w.UserEntityId == adminApproveModelView.userEntityId).FirstOrDefault();
            if (userEntity == null) { return null; }
            userEntity.UserStage = 4;
            userEntity.AdminNotApprovedIP = clientIP;
            userEntity.CommetNotApproved = adminApproveModelView.comment;
            userEntity.AdminNotApproved = cmuaccount;
            userEntity.NotApprovedTime = DateTime.Now;

            userEntity.IsConfirmKYC = adminApproveModelView.IsConfirmKYC;
            userEntity.IsConfirmPersonalID = adminApproveModelView.IsConfirmPersonalID;
            _evoteContext.SaveChanges();
            await _emailRepository.SendEmailAsync("CMU Evote service", userEntity.Email, "การยืนยันตัวของท่านไม่ได้รับยืนยัน", adminApproveModelView.comment, null);

            userEntities = _evoteContext.UserEntitys.Where(w => w.UserStage == 2).ToList();
            return userEntities;
        }

        public async Task<UserAdminEntity> getAdminByEmail(string cmuaccount)
        {
            return _evoteContext.UserAdminEntitys.Where(w => w.Cmuaccount == cmuaccount).FirstOrDefault();
        }

        public async Task<string> sendLoginOTP(String cmuaccount, String _access_token, String _refresh_token)
        {
            UserAdminEntity userAdminEntity = _evoteContext.UserAdminEntitys.Where(w => w.Cmuaccount == cmuaccount).First();
            Random _random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            String code = new string(Enumerable.Repeat(chars, 4)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
            userAdminEntity.Access_token = _access_token;
            userAdminEntity.Refresh_token = _refresh_token;
            userAdminEntity.SMSOTP = await _sMSRepository.getOTP(code, userAdminEntity.Tel);
            userAdminEntity.SMSOTPRef = code;
            _evoteContext.SaveChanges();


            return code;
        }

        public async Task<UserAdminEntity> getAdminByOTP(AdminLoginOTPModelview adminLoginOTPModelview, String clientIP)
        {

            UserAdminEntity userAdminEntity = _evoteContext.UserAdminEntitys.Where(w => w.SMSOTP == adminLoginOTPModelview.otp && w.SMSOTPRef == adminLoginOTPModelview.RefCode).FirstOrDefault();
            if (userAdminEntity == null) { return null; }
            userAdminEntity.SMSOTP = "";
            userAdminEntity.SMSOTPRef = "";
            AdminLoginLog adminLoginLog = new AdminLoginLog();
            adminLoginLog.Cmuaccount = userAdminEntity.Cmuaccount;
            adminLoginLog.LoginTime = DateTime.Now;
            adminLoginLog.ClientIP = clientIP;
            _evoteContext.SaveChanges();


            return userAdminEntity;
        }

        public async Task<UserEntity> getUserEntity(string cmuaccount, int userEntityId, string clientIP)
        {
            UserAdminEntity userAdminEntity = _evoteContext.UserAdminEntitys.Where(w => w.Cmuaccount == cmuaccount).FirstOrDefault();
            if (userAdminEntity == null) { return null; }
            UserEntity userEntity = new UserEntity();
            userEntity = _evoteContext.UserEntitys.Where(w =>  w.UserEntityId == userEntityId).FirstOrDefault();
            return userEntity;
        }

        public async Task<List<UserEntity>> searchUser(AdminSearchModelView adminSearchModelView, string cmuaccount)
        {
            List<UserEntity> userEntities = new List<UserEntity>();
            UserAdminEntity userAdminEntity = _evoteContext.UserAdminEntitys.Where(w => w.Cmuaccount == cmuaccount).FirstOrDefault();
            if (userAdminEntity == null) { return null; }
            userEntities = _evoteContext.UserEntitys.Where(w => w.UserStage >= 2)
                .WhereIf(adminSearchModelView.FullName != "", w => w.FullName.Contains(adminSearchModelView.FullName))
                .WhereIf(adminSearchModelView.Email != "", w => w.Email.Contains(adminSearchModelView.Email))
                .WhereIf(adminSearchModelView.Organization_Name_TH != "", w => w.Organization_Name_TH.Contains(adminSearchModelView.Organization_Name_TH))
                .WhereIf(adminSearchModelView.PersonalID != "", w => w.PersonalID.Contains(adminSearchModelView.PersonalID))
                .WhereIf(adminSearchModelView.Tel != "", w => w.Tel.Contains(adminSearchModelView.Tel))
                .WhereIf(adminSearchModelView.UserStage != 0, w => w.UserStage == adminSearchModelView.UserStage)
                .ToList();
            return userEntities;
        }
    }
}
