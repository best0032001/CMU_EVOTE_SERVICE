using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.Entity
{
    public class UserEntity
    {
        public int UserEntityId { get; set; }
        public String Email { get; set; }
        public String Organization_Code { get; set; }
        public String Organization_Name_TH { get; set; }
        public String FullName { get; set; }
        public String PersonalID { get; set; }
        public String Tel { get; set; }
        public int UserStage { get; set; }   //0  no regis //1 regis 2 confirm 3 approve  4 rejected
        public int UserType { get; set; } // 1 CMU 2 non CMU
        public String LineId { get; set; }
        public DateTime CreateTime { get; set; }

        public String SMSOTP { get; set; }
        public String SMSOTPRef { get; set; }
        public String EmailOTP { get; set; }
        public String EmailOTPRef { get; set; }


        public Boolean IsConfirmEmail { get; set; }
        public DateTime? ConfirmEmailTime { get; set; }
        public Boolean IsConfirmTel { get; set; }
        public DateTime? ConfirmTelTime { get; set; }

    
        public Boolean IsConfirmPersonalID { get; set; }
        public DateTime? ConfirmPersonalIDTime { get; set; }
        public String fileNamePersonalID { get; set; }
        public String fullPathPersonalID { get; set; }
        public String dbPathPersonalID { get; set; }
     

        public Boolean IsConfirmKYC { get; set; }
        public DateTime? ConfirmKYCTime { get; set; }
        public String fileNameKYC { get; set; }
        public String fullPathKYC { get; set; }
        public String dbPathKYC { get; set; }

        public String fileNameFace { get; set; }
        public String fullPathFace { get; set; }
        public String dbPathFace { get; set; }

        public String faceData { get; set; }

        public String AdminApproved { get; set; }
        public String AdminApprovedIP { get; set; }
        public String AdminNotApproved { get; set; }
        public String AdminNotApprovedIP { get; set; }
        public String CommetNotApproved { get; set; }
        public DateTime? ApprovedTime { get; set; }
        public DateTime? NotApprovedTime { get; set; }


        public String Access_token { get; set; }
        public String Refresh_token { get; set; }
        public String Expires_in { get; set; }

        public List<EventVoteEntity> eventVoteEntities { get; set; }
    }
}
