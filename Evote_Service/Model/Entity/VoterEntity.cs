using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.Entity
{
    public class VoterEntity
    {
        public int VoterEntityId { get; set; }
        public int EventVoteEntityId { get; set; }
        public String Email { get; set; }
        public String Organization_Code { get; set; }
        public string CreateUser { get; set; }
        public DateTime VoterCreate { get; set; }

        public String SMSOTP { get; set; }
        public String SMSOTPRef { get; set; }
        public DateTime SMSCreate { get; set; }

    }
}
