using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.Entity
{
    public class UserAdminEntity
    {
        public int UserAdminEntityId { get; set; }

        public String FullName { get; set; }

        public String Cmuaccount { get; set; }

        public String Tel { get; set; }

        public Boolean SuperAdmin { get; set; }

        public string Organization_Code { get; set; }
        public string OrganizationFullNameTha { get; set; }

        
        public String SMSOTP { get; set; }
        public String SMSOTPRef { get; set; }

        public DateTime? SMSExpire { get; set; }

        public String Access_token { get; set; }
        public String Refresh_token { get; set; }
    }
}
