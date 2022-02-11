using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.Entity
{
    public class VoterEntity
    {
        public int VoterEntityId { get; set; }
        public int EventVoteEntityId { get; set; }

        [Column(TypeName = "varchar(50)")]
        public String Email { get; set; }

        [Column(TypeName = "varchar(250)")]
        public String FullName { get; set; }
        public int UserType { get; set; } // 1 CMU 2 non CMU

        [Column(TypeName = "varchar(50)")]
        public String Organization_Code { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string CreateUser { get; set; }
        public DateTime VoterCreate { get; set; }

        [Column(TypeName = "varchar(20)")]
        public String SMSOTP { get; set; }
        [Column(TypeName = "varchar(20)")]
        public String SMSOTPRef { get; set; }
        public DateTime? SMSExpire { get; set; }

    }
}
