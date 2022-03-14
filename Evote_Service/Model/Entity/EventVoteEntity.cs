using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.Entity
{
    public class EventVoteEntity
    {
        public int EventVoteEntityId { get; set; }
        public int EventStatusId { get; set; } 
        public int ApplicationEntityId { get; set; }
        public int EventTypeId { get; set; }
        public int RoundNumber { get; set; }

        [Column(TypeName = "varchar(1000)")]
        public string EventInformation { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string SecretKey { get; set; }
        [Column(TypeName = "varchar(250)")]
        public string SecurityAlgorithm { get; set; }

        [Column(TypeName = "varchar(250)")]
        public string EventTitle { get; set; }

        [Column(TypeName = "varchar(1000)")]
        public string EventDetail { get; set; }


        [Column(TypeName = "varchar(50)")]
        public string CreateUser { get; set; }


        [Column(TypeName = "varchar(50)")]
        public string PresidentEmail { get; set; }

        public DateTime? PresidentUpdate { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string UpdateUser { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string Organization_Code { get; set; }

        [Column(TypeName = "varchar(250)")]
        public string OrganizationFullNameTha { get; set; }
        public DateTime EventCreate { get; set; }
        public DateTime EventUpdate { get; set; }


        //public DateTime EventRegisterStart { get; set; }
        //public DateTime EventRegisterEnd { get; set; }


        public Boolean IsUseTime { get; set; }

        public DateTime EventVotingStart { get; set; }
        public DateTime EventVotingEnd { get; set; }

        [Column(TypeName = "varchar(250)")]
        public string AppLink { get; set; }
        public Boolean IsEnd { get; set; }

        public Boolean IsDev { get; set; }
        public List<ConfirmVoter> confirmVoters { get; set; }
        public List<VoteEntity> voteEntities { get; set; }
        public List<VoterEntity>  voterEntities { get; set; }  //ผู้มี สิทธิ์ Vote
    }
}
