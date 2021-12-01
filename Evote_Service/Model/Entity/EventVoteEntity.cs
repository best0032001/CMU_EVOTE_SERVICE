using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.Entity
{
    public class EventVoteEntity
    {
        public int EventVoteEntityId { get; set; }
        public int ApplicationEntityId { get; set; }
        public int EventTypeId { get; set; }
        public string EventTitle { get; set; }
        public string EventDetail { get; set; }
        public string PublicKey { get; set; }
        public string CreateUser { get; set; }
        public string UpdateUser { get; set; }
        public string ClientIP { get; set; }
        public string OrganizationID { get; set; }
        public string OrganizationFullNameTha { get; set; }
        public DateTime? EventCreate { get; set; }
        public DateTime? EventUpdate { get; set; }
        public DateTime? EventRegisterStart { get; set; }
        public DateTime? EventRegisterEnd { get; set; }
        public DateTime? EventVotingStart { get; set; }
        public DateTime? EventVotingEnd { get; set; }
    }
}
