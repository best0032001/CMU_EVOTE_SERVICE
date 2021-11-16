using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.Entity
{
    public class UserAdminEntity
    {
        public int UserAdminEntityId { get; set; }

        public String Cmuaccount { get; set; }
       
        public string OrganizationID { get; set; }
        public string OrganizationFullNameTha { get; set; }
    }
}
