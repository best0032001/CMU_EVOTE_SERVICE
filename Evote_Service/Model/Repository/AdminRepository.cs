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
    public class AdminRepository : IAdminRepository
    {
        private EvoteContext _evoteContext;
        private readonly IHttpClientFactory _clientFactory;
        private ISMSRepository _sMSRepository;
        private IEmailRepository _emailRepository;
        public AdminRepository(EvoteContext evoteContext, IHttpClientFactory clientFactory, IEmailRepository emailRepository)
        {
            if (evoteContext == null)
            {
                throw new System.ArgumentNullException(nameof(evoteContext));
            }
            _evoteContext = evoteContext;
            _emailRepository = emailRepository;

        }
        public async Task<List<UserEntity>> getUserWaitForApprove(String cmuaccount)
        {
            List<UserEntity> userEntities = new List<UserEntity>();
            UserAdminEntity userAdminEntity= _evoteContext.UserAdminEntitys.Where(w => w.Cmuaccount == cmuaccount).FirstOrDefault();
            if (userAdminEntity == null) { return null; }
            userEntities= _evoteContext.UserEntitys.Where(w => w.UserStage == 2).ToList();
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
            _evoteContext.SaveChanges();

            await _emailRepository.SendEmailAsync("CMU Evote service", userEntity.Email, "การยืนยันตัวของท่านได้รับการตรวจสอบแล้ว", "  ", null);
            userEntities = _evoteContext.UserEntitys.Where(w => w.UserStage == 2).ToList();
            return userEntities;
        }

        public async Task<List<UserEntity>> adminNotApprove(string cmuaccount, Int32 userEntityId,String comment, String clientIP)
        {
            List<UserEntity> userEntities = new List<UserEntity>();
            UserAdminEntity userAdminEntity = _evoteContext.UserAdminEntitys.Where(w => w.Cmuaccount == cmuaccount).FirstOrDefault();
            if (userAdminEntity == null) { return null; }
            UserEntity userEntity = _evoteContext.UserEntitys.Where(w => w.UserStage == 2 && w.UserEntityId == userEntityId).FirstOrDefault();
            if (userEntity == null) { return null; }
            userEntity.UserStage = 4;
            userEntity.AdminNotApprovedIP = clientIP;
            userEntity.CommetNotApproved = comment;
            userEntity.AdminNotApproved = cmuaccount;
            userEntity.NotApprovedTime = DateTime.Now;
            _evoteContext.SaveChanges();
            await _emailRepository.SendEmailAsync("CMU Evote service", userEntity.Email, "การยืนยันตัวของท่านไม่ได้รับยืนยัน", comment, null);

            userEntities = _evoteContext.UserEntitys.Where(w => w.UserStage == 2).ToList();
            return userEntities;
        }
    }
}
