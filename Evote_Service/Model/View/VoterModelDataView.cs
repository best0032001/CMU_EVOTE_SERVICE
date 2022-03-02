using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.View
{
    public class VoterModelDataView
    {
        public int VoterEntityId { get; set; }
        public int EventVoteEntityId { get; set; }
        public String Email { get; set; }
        public String Organization_Code { get; set; }
        public String OrganizationFullNameTha { get; set; }
        public String FullName { get; set; }

        public String VoterType { get; set; }

    }
}
