using Evote_Service.Model.Entity;
using Evote_Service.Model.Interface;
using Evote_Service.Model.Util;
using Evote_Service.Model.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Evote_Service.Model.Repository
{
    public class UserManageRepository : IUserManageRepository
    {
        private EvoteContext _evoteContext;
        private readonly IHttpClientFactory _clientFactory;
        private IEmailRepository _emailRepository;
        public UserManageRepository(EvoteContext evoteContext, IHttpClientFactory clientFactory, IEmailRepository emailRepository, ISMSRepository sMSRepository)
        {
            if (evoteContext == null)
            {
                throw new System.ArgumentNullException(nameof(evoteContext));
            }
            _evoteContext = evoteContext;
            _emailRepository = emailRepository;


        }
        public async Task<List<UserEntity>> searchUser(AdminSearchModelView adminSearchModelView, string cmuaccount)
        {
            String RAW_KEY = Environment.GetEnvironmentVariable("RAW_KEY");
            String PASS_KEY = Environment.GetEnvironmentVariable("PASS_KEY");
            Crypto crypto = new Crypto(PASS_KEY, RAW_KEY);
            if (adminSearchModelView.PersonalID != "")
            {
                adminSearchModelView.PersonalID = crypto.Encrypt(adminSearchModelView.PersonalID);
            }
            if (adminSearchModelView.Tel != "")
            {
                adminSearchModelView.Tel = crypto.Encrypt(adminSearchModelView.Tel);
            }
            List<UserEntity> userEntities = new List<UserEntity>();
            UserAdminEntity userAdminEntity = _evoteContext.UserAdminEntitys.Where(w => w.Cmuaccount == cmuaccount).FirstOrDefault();
            if (userAdminEntity == null) { return null; }
            Boolean checkActive = false;

            if (adminSearchModelView.UserStatus == 2)
            {
                checkActive = true;
            }
            if (userAdminEntity.SuperAdmin)
            {
                if (adminSearchModelView.Organization_Code == "")
                {
                    adminSearchModelView.Organization_Code = "0000000000";
                }
                userEntities = _evoteContext.UserEntitys.Where(w => w.UserStage == 3 && w.UserType == 2)
                                .WhereIf(adminSearchModelView.Organization_Code != "0000000000", w => w.Organization_Code == adminSearchModelView.Organization_Code)
                                .WhereIf(adminSearchModelView.FullName != "", w => w.FullName.Contains(adminSearchModelView.FullName))
                                .WhereIf(adminSearchModelView.Email != "", w => w.Email.Contains(adminSearchModelView.Email))
                                .WhereIf(adminSearchModelView.PersonalID != "", w => w.PersonalID.Contains(adminSearchModelView.PersonalID))
                                .WhereIf(adminSearchModelView.Tel != "", w => w.Tel.Contains(adminSearchModelView.Tel))
                                 .WhereIf(adminSearchModelView.UserStatus != 0, w => w.IsDeactivate == checkActive)
                                .ToList();
            }
            else
            {
                userEntities = _evoteContext.UserEntitys.Where(w => w.UserStage == 3 && w.UserType == 2 && w.Organization_Code == userAdminEntity.Organization_Code)
                                .WhereIf(adminSearchModelView.FullName != "", w => w.FullName.Contains(adminSearchModelView.FullName))
                                .WhereIf(adminSearchModelView.Email != "", w => w.Email.Contains(adminSearchModelView.Email))
                                .WhereIf(adminSearchModelView.PersonalID != "", w => w.PersonalID.Contains(adminSearchModelView.PersonalID))
                                .WhereIf(adminSearchModelView.Tel != "", w => w.Tel.Contains(adminSearchModelView.Tel))
                                  .WhereIf(adminSearchModelView.UserStatus != 0, w => w.IsDeactivate == checkActive)
                                .ToList();
            }

            return userEntities;
        }

        public async Task<List<UserEntity>> deActiveUser(string cmuaccount, int userEntityId, string clientIP)
        {
            List<UserEntity> userEntities = new List<UserEntity>();
            UserAdminEntity userAdminEntity = _evoteContext.UserAdminEntitys.Where(w => w.Cmuaccount == cmuaccount).FirstOrDefault();
            if (userAdminEntity == null) { return null; }
            UserEntity userEntity = null;
            if (userAdminEntity.SuperAdmin)
            {
                userEntity = _evoteContext.UserEntitys.Where(w => w.UserStage == 3 && w.UserEntityId == userEntityId && w.IsDeactivate == false).FirstOrDefault();
            }
            else
            {
                userEntity = _evoteContext.UserEntitys.Where(w => w.UserStage == 3 && w.UserEntityId == userEntityId && w.IsDeactivate == false && w.Organization_Code == userAdminEntity.Organization_Code).FirstOrDefault();
            }
            if (userEntity == null) { return null; }
            userEntity.AdminDeactivateIP = clientIP;
            userEntity.AdminDeactivate = cmuaccount;
            userEntity.DeactivateTime = DateTime.Now;
            userEntity.IsDeactivate = true;
            _evoteContext.SaveChanges();
            await _emailRepository.SendEmailAsync("CMU Evote service", userEntity.Email, "User ID ของท่านถูกเจ้าหน้าที่ระงับการใช้งานแล้ว", "  ", null);
            // userEntities = _evoteContext.UserEntitys.Where(w => w.UserStage == 2).OrderByDescending(o => o.UserEntityId).ToList();
            AdminSearchModelView adminSearchModelView = new AdminSearchModelView();
            userEntities = await this.searchUser(adminSearchModelView, cmuaccount);

            return userEntities;
        }
    }
}
