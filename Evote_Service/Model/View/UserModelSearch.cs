using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.View
{
    public class UserModelSearch
    {
        public String FullName { get; set; }
        public int EventVoteEntityId { get; set; }
    }

    public class UserModelDataView
    {
        public String Email { get; set; }
        public String Organization_Code { get; set; }
        public String FullName { get; set; }

    }
}
