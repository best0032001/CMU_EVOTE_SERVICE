using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.View
{
    public class EventModelview
    {
        public int EventTypeId { get; set; }
        public string EventTitle { get; set; }
        public string EventDetail { get; set; }
   

        public string Organization_Code { get; set; }
        public string OrganizationFullNameTha { get; set; }

        public DateTime EventRegisterStart { get; set; }
        public DateTime EventRegisterEnd { get; set; }
        public DateTime EventVotingStart { get; set; }
        public DateTime EventVotingEnd { get; set; }
    }
    public class EventConfirmModelview
    {
        public int EventVoteEntityId { get; set; }
        public string SecretKey { get; set; }
        public string SecurityAlgorithm { get; set; }
        public string EventTitle { get; set; }
        public string EventDetail { get; set; }


        public string Organization_Code { get; set; }
        public string OrganizationFullNameTha { get; set; }

        public DateTime EventRegisterStart { get; set; }
        public DateTime EventRegisterEnd { get; set; }
        public DateTime EventVotingStart { get; set; }
        public DateTime EventVotingEnd { get; set; }
        public string CreateUser { get; set; }
    }
}
