using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.Entity
{
    public class VoteEntity
    {
        public int VoteEntityId { get; set; }
        public int ApplicationEntityId { get; set; }
        public int EventVoteEntityId { get; set; }
        public int VoteRoundEntityId { get; set; }
        public int RoundNumber { get; set; }
        public String VoteData { get; set; }
    }
}
