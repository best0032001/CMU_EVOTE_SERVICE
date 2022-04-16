using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.View
{
    public class UserAdminModelView
    {

        public String FullName { get; set; }
        public String Cmuaccount { get; set; }
        public string OrganizationFullNameTha { get; set; }
        public String Access_token { get; set; }
        //  public Boolean SuperAdmin { get; set; }
        public List<UserMenuView> MenuList { get; set; }
    }
    public class UserMenuView
    {
        public String MenuName { get; set; }
        public String MenuLink { get; set; }
        public String icon { get; set; }
    }
}
