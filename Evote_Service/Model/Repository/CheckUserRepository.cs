﻿using Evote_Service.Model.Entity;
using Evote_Service.Model.Interface;
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
        public CheckUserRepository(EvoteContext evoteContext, IHttpClientFactory clientFactory)
        {
            if (evoteContext == null)
            {
                throw new System.ArgumentNullException(nameof(evoteContext));
            }
            _evoteContext = evoteContext;
            _clientFactory = clientFactory;
        }
        public async Task<int> CheckLineUser(string lineId)
        {
            UserEntity userEntitys= _evoteContext.UserEntitys.Where(w => w.LineId == lineId).FirstOrDefault();
            if (userEntitys == null){ return 0; }
            return userEntitys.UserStage;
        }
    }
}
