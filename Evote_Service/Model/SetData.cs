﻿using Evote_Service.Model.Entity;
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
            userEntity.Email = "test@test.cmu.ac.th";
            userEntity.FullName = "test";
            userEntity.Tel = "1234";
            userEntity.UserStage = 2;


            _evoteContext.UserEntitys.Add(userEntity);
            _evoteContext.SaveChanges();


        }
        private void innit()
        {
            setDefaultAdmin();
        }
        private void setDefaultAdmin()
        {
            UserAdminEntity userAdminEntity = new UserAdminEntity();
            userAdminEntity.Cmuaccount = "cheewin.b@cmu.ac.th";
            _evoteContext.UserAdminEntitys.Add(userAdminEntity);
            _evoteContext.SaveChanges();
        }
    }
}
