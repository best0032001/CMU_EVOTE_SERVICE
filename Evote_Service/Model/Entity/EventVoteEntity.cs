using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.Entity
{
    public class EventVoteEntity
    {
        public int EventVoteEntityId { get; set; }
        public int EventStatusId { get; set; } // 1 now 2 incoming 3 passed
        public int ApplicationEntityId { get; set; }
        public int EventTypeId { get; set; }

        public string EventInformation { get; set; }
        public string SecretKey { get; set; }
        public string SecurityAlgorithm { get; set; }
        public string EventTitle { get; set; }
        public string EventDetail { get; set; }
       
        public string CreateUser { get; set; }
        public string UpdateUser { get; set; }
        public string Organization_Code { get; set; }
        public string OrganizationFullNameTha { get; set; }
        public DateTime EventCreate { get; set; }
        public DateTime EventUpdate { get; set; }
        public DateTime EventRegisterStart { get; set; }
        public DateTime EventRegisterEnd { get; set; }
        public DateTime EventVotingStart { get; set; }
        public DateTime EventVotingEnd { get; set; }

        public Boolean IsEnd { get; set; }

        public Boolean IsDev { get; set; }

        public List<VoterEntity>  voterEntities { get; set; }
    }
}
