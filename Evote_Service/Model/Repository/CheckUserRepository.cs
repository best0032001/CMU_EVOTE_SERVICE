using Evote_Service.Model.Entity;
using Evote_Service.Model.Interface;
using Evote_Service.Model.View;
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
        public CheckUserRepository(EvoteContext evoteContext, IHttpClientFactory clientFactory)
        {
            if (evoteContext == null)
            {
                throw new System.ArgumentNullException(nameof(evoteContext));
            }
            _evoteContext = evoteContext;
            _clientFactory = clientFactory;
        }
        public async Task<UserModel> GetLineUser(string lineId)
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
    }
}
