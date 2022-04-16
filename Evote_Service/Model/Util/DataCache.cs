using Evote_Service.Model.Entity.Ref;
using Evote_Service.Model.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.Util
{
    public static class DataCache
    {
        public static String cmuitaccount_basicinfo = "cmuitaccount.basicinfo";
        public static String authorization_code = "authorization_code";
        public static List<UserMock> UserMocks;
        public static List<AdminMock> AdminMocks;
        public static List<RefUserStage> RefUserStages;
        public static List<OrganizationModelView> organList;
    }

    public class UserMock
    {
        public string token { get; set; }
        public string lineId { get; set; }
        public string email { get; set; }
    }
    public class AdminMock
    {
        public string token { get; set; }
        public string Cmuaccount { get; set; }
    }
}
