using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.Entity
{
    public class UserEntity
    {
        public int UserEntityId { get; set; }


        [Column(TypeName = "varchar(50)")]
        public String UserCode { get; set; }


        [Column(TypeName = "varchar(50)")]
        public String Email { get; set; }

        [Column(TypeName = "varchar(50)")]
        public String Organization_Code { get; set; }

        [Column(TypeName = "varchar(250)")]
        public String Organization_Name_TH { get; set; }


        [Column(TypeName = "varchar(250)")]
        public String FullName { get; set; }

     
        public String PersonalID { get; set; }

        public String Tel { get; set; }
        public int UserStage { get; set; }   //0  no regis //1 regis 2 confirm 3 approve  4 rejected
        public int UserType { get; set; } // 1 CMU 2 non CMU


        [Column(TypeName = "varchar(50)")]
        public String LineId { get; set; }
        public DateTime CreateTime { get; set; }

        [Column(TypeName = "varchar(20)")]
        public String SMSOTP { get; set; }
        [Column(TypeName = "varchar(20)")]
        public String SMSOTPRef { get; set; }

        public DateTime? SMSExpire { get; set; }

        [Column(TypeName = "varchar(20)")]
        public String EmailOTP { get; set; }
        [Column(TypeName = "varchar(20)")]
        public String EmailOTPRef { get; set; }


        public Boolean IsConfirmEmail { get; set; }
        public DateTime? ConfirmEmailTime { get; set; }
        public Boolean IsConfirmTel { get; set; }
        public DateTime? ConfirmTelTime { get; set; }

    
        public Boolean IsConfirmPersonalID { get; set; }
        public DateTime? ConfirmPersonalIDTime { get; set; }


        [Column(TypeName = "varchar(100)")]
        public String fileNamePersonalID { get; set; }
        [Column(TypeName = "varchar(100)")]
        public String fullPathPersonalID { get; set; }
        [Column(TypeName = "varchar(100)")]
        public String dbPathPersonalID { get; set; }
     

        public Boolean IsConfirmKYC { get; set; }
        public DateTime? ConfirmKYCTime { get; set; }

        [Column(TypeName = "varchar(100)")]
        public String fileNameKYC { get; set; }
        [Column(TypeName = "varchar(100)")]
        public String fullPathKYC { get; set; }
        [Column(TypeName = "varchar(100)")]
        public String dbPathKYC { get; set; }

        [Column(TypeName = "varchar(100)")]
        public String fileNameFace { get; set; }
        [Column(TypeName = "varchar(100)")]
        public String fullPathFace { get; set; }
        [Column(TypeName = "varchar(100)")]
        public String dbPathFace { get; set; }


     
        public String faceData { get; set; }


        [Column(TypeName = "varchar(50)")]
        public String AdminApproved { get; set; }
        [Column(TypeName = "varchar(50)")]
        public String AdminApprovedIP { get; set; }
        [Column(TypeName = "varchar(50)")]
        public String AdminNotApproved { get; set; }
        [Column(TypeName = "varchar(50)")]
        public String AdminNotApprovedIP { get; set; }
        [Column(TypeName = "varchar(50)")]
        public String CommetNotApproved { get; set; }
        public DateTime? ApprovedTime { get; set; }
        public DateTime? NotApprovedTime { get; set; }

        [Column(TypeName = "varchar(50)")]
        public String Access_token { get; set; }
        [Column(TypeName = "varchar(50)")]
        public String Refresh_token { get; set; }
        [Column(TypeName = "varchar(50)")]
        public String Expires_in { get; set; }

       // public List<EventVoteEntity> eventVoteEntities { get; set; }
    }
}
