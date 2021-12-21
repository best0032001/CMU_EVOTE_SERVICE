using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.Entity
{
    public class VoteRoundEntity
    {
        public int VoteRoundEntityId { get; set; }
        public int EventVoteEntityId { get; set; }
        public int RoundNumber { get; set; }

        public List<ConfirmVoter> confirmVoters { get; set; }

        public List<VoteEntity> voteEntities { get; set; }
    }
}
