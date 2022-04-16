using System;

namespace Evote_Service.Model.View
{
    public class AdminModelView
    {
        public int UserAdminEntityId { get; set; }
        public String FullName { get; set; }
        public String Cmuaccount { get; set; }
        public string OrganizationFullNameTha { get; set; }
        public Boolean SuperAdmin { get; set; }
        public Boolean OrganAdmin { get; set; }
        public string Organization_Code { get; set; }
    }
}
