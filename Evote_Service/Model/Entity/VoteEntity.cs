using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.Entity
{
    public class VoteEntity
    {
        public int VoteEntityId { get; set; }
        public int ApplicationEntityId { get; set; }
        public int EventVoteEntityId { get; set; }
        public int RoundNumber { get; set; }

        [Column(TypeName = "varchar(3000)")]
        public String VoteData { get; set; }
    }
}
