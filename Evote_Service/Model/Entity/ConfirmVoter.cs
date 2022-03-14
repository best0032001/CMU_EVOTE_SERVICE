using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.Entity
{
    public class ConfirmVoter
    {
        public int ConfirmVoterId { get; set; }
        public int EventVoteEntityId { get; set; }
        public int RoundNumber { get; set; }

        [Column(TypeName = "varchar(100)")]
        public String email { get; set; }
    }
}
