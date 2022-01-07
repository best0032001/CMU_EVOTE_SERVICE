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
        public Boolean SuperAdmin { get; set; }


    }
}
