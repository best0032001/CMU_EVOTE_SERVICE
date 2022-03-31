using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.View
{
    public class EventModelview
    {
        public int EventVoteEntityId { get; set; }
        public int EventTypeId { get; set; }
        public string EventTitle { get; set; }
        public string EventDetail { get; set; }
        public int EventStatusId { get; set; }
        public int RoundNumber { get; set; }
        public string Organization_Code { get; set; }
        public string OrganizationFullNameTha { get; set; }

        //public DateTime EventRegisterStart { get; set; }
        //public DateTime EventRegisterEnd { get; set; }
        public Boolean IsUseTime { get; set; }
        public DateTime EventVotingStart { get; set; }
        public DateTime EventVotingEnd { get; set; }
        public string EventInformation { get; set; }
        public string PresidentEmail { get; set; }
        public string AppLink { get; set; }
        public Boolean IsToVote { get; set; }
        public Boolean IsVote { get; set; }


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

        //public DateTime EventRegisterStart { get; set; }
        //public DateTime EventRegisterEnd { get; set; }
        public Boolean IsUseTime { get; set; }
        public DateTime EventVotingStart { get; set; }
        public DateTime EventVotingEnd { get; set; }
        public string CreateUser { get; set; }
    }
    public class VoterModelview
    {
        public int EventVoteEntityId { get; set; }
        public List<PeopleModelview> peopleModelviews { get; set; }
      
    }
    public class PeopleModelview
    {
        public String Email { get; set; }
        public String Organization_Code { get; set; }
        public String OrganizationFullNameTha { get; set; }
        public String FullName { get; set; }
        public int UserType { get; set; } // 1 CMU 2 non CMU
    }
}
