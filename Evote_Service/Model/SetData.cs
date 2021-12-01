using Evote_Service.Model.Entity;
using Evote_Service.Model.Entity.Ref;
using Evote_Service.Model.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model
{
    public class SetData
    {
        private EvoteContext _evoteContext;
        public SetData(IWebHostEnvironment env, EvoteContext evoteContext)
        {
            _evoteContext = evoteContext;
            if (env.IsEnvironment("test")) { innitMock(); }
            if (evoteContext == null)
            {
                throw new System.ArgumentNullException(nameof(evoteContext));
            }
            innit();
        }
        private void innitMock()
        {
            DataCache.AdminMocks = new List<AdminMock>();
            DataCache.UserMocks = new List<UserMock>();
            UserMock userMock1 = new UserMock();
            userMock1.token = "x01";
            userMock1.lineId = "l01";
            DataCache.UserMocks.Add(userMock1);

            AdminMock adminMock = new AdminMock();
            adminMock.token = "a01";
            adminMock.Cmuaccount = "cheewin.b@cmu.ac.th";
            DataCache.AdminMocks.Add(adminMock);

            UserEntity userEntity = new UserEntity();
            userEntity.Email = "cheewin.b@cmu.ac.th";
            userEntity.FullName = "test";
            userEntity.Tel = "1234";
            userEntity.UserStage = 2;


            _evoteContext.UserEntitys.Add(userEntity);
            _evoteContext.SaveChanges();


        }
        private void innit()
        {
            setDefaultAdmin();
            setRefUserStage();
        }
        private void setDefaultAdmin()
        {
            UserAdminEntity userAdminEntity = new UserAdminEntity();
            userAdminEntity.Cmuaccount = "cheewin.b@cmu.ac.th";
            userAdminEntity.SuperAdmin = true;
            _evoteContext.UserAdminEntitys.Add(userAdminEntity);
            _evoteContext.SaveChanges();
        }
        private void setRefUserStage()
        {
            List<RefUserStage> list = _evoteContext.RefUserStages.ToList();
            if (list.Count == 0)
            {
                RefUserStage refUserStage1 = new RefUserStage();
                refUserStage1.RefUserStageID = 1;
                refUserStage1.UserStageName = "regis";
                _evoteContext.RefUserStages.Add(refUserStage1);
                _evoteContext.SaveChanges();

                RefUserStage refUserStage2 = new RefUserStage();
                refUserStage2.RefUserStageID = 2;
                refUserStage2.UserStageName = "confirm";
                _evoteContext.RefUserStages.Add(refUserStage2);
                _evoteContext.SaveChanges();

                RefUserStage refUserStage3 = new RefUserStage();
                refUserStage3.RefUserStageID = 3;
                refUserStage3.UserStageName = "approved";
                _evoteContext.RefUserStages.Add(refUserStage3);
                _evoteContext.SaveChanges();

                RefUserStage refUserStage4 = new RefUserStage();
                refUserStage4.RefUserStageID = 4;
                refUserStage4.UserStageName = "rejected";
                _evoteContext.RefUserStages.Add(refUserStage4);
                _evoteContext.SaveChanges();
            }
        }
    }
}
