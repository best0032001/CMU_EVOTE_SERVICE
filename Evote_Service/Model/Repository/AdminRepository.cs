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
    public class AdminRepository : IAdminRepository
    {
        private EvoteContext _evoteContext;
        private readonly IHttpClientFactory _clientFactory;
        private ISMSRepository _sMSRepository;
        private IEmailRepository _emailRepository;
        public AdminRepository(EvoteContext evoteContext, IHttpClientFactory clientFactory)
        {
            if (evoteContext == null)
            {
                throw new System.ArgumentNullException(nameof(evoteContext));
            }
            _evoteContext = evoteContext;
           
        }
        public async Task<List<UserEntity>> getUserWaitForApprove(String cmuaccount)
        {
            List<UserEntity> userEntities = new List<UserEntity>();
            UserAdminEntity userAdminEntity= _evoteContext.UserAdminEntitys.Where(w => w.Cmuaccount == cmuaccount).FirstOrDefault();
            if (userAdminEntity == null) { return null; }
            userEntities= _evoteContext.UserEntitys.Where(w => w.UserStage == 2).ToList();
            return userEntities;

        }
    }
}
