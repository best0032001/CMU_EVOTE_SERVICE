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
    public class UserAdminRepository : IUserAdminRepository
    {
        private EvoteContext _evoteContext;
        public UserAdminRepository(EvoteContext evoteContext, IHttpClientFactory clientFactory, IEmailRepository emailRepository)
        {
            if (evoteContext == null)
            {
                throw new System.ArgumentNullException(nameof(evoteContext));
            }
            _evoteContext = evoteContext;
        }

        public async Task<Boolean> addAdmin(AdminModelView adminModelView, string cmuaccount, string ip)
        {

            UserAdminEntity userAdminEntityModel = new UserAdminEntity();
            userAdminEntityModel.FullName = adminModelView.FullName;
            userAdminEntityModel.Cmuaccount = adminModelView.Cmuaccount;
            userAdminEntityModel.CreateUser = cmuaccount;
            userAdminEntityModel.CreateIP = ip;
            userAdminEntityModel.OrganAdmin = adminModelView.OrganAdmin;
            userAdminEntityModel.SuperAdmin = false;
            userAdminEntityModel.Organization_Code = adminModelView.Organization_Code;
            userAdminEntityModel.OrganizationFullNameTha = adminModelView.OrganizationFullNameTha;
            _evoteContext.UserAdminEntitys.Add(userAdminEntityModel);
            _evoteContext.SaveChanges();


            return true;
        }

        public async Task<bool> deleteAdmin(int UserAdminEntityId)
        {
            UserAdminEntity userAdminEntity = _evoteContext.UserAdminEntitys.Where(w => w.UserAdminEntityId == UserAdminEntityId).FirstOrDefault();
            _evoteContext.UserAdminEntitys.Remove(userAdminEntity);
            _evoteContext.SaveChanges();
            return true;
        }

        public async Task<List<AdminModelView>> searchAdmin(AdminModelView adminModelView, string cmuaccount)
        {
            List<AdminModelView> adminModelViews = new List<AdminModelView>();
            List<UserAdminEntity> userAdminEntities = _evoteContext.UserAdminEntitys.Where(w => w.Cmuaccount != "")
                   .WhereIf(adminModelView.FullName != "", w => w.FullName.Contains(adminModelView.FullName))
                   .WhereIf(adminModelView.Cmuaccount != "", w => w.Cmuaccount.Contains(adminModelView.Cmuaccount))
                     .WhereIf(adminModelView.OrganAdmin == true, w => w.OrganAdmin == true)
                   .WhereIf(adminModelView.Organization_Code != "0000000000", w => w.Organization_Code == adminModelView.Organization_Code)
                .OrderBy(o => o.FullName).ToList();

            foreach (UserAdminEntity userAdminEntity in userAdminEntities)
            {
                String json = JsonConvert.SerializeObject(userAdminEntity);
                AdminModelView Model = JsonConvert.DeserializeObject<AdminModelView>(json);
                adminModelViews.Add(Model);
            }

            return adminModelViews;
        }

    }
}
