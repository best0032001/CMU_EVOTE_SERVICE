using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.View
{
    public class UserModelView
    {
        public int UserEntityId { get; set; }
        public String LineId { get; set; }
        public Boolean isDelete { get; set; }
        
        public String Email { get; set; }
        public String Organization_Code { get; set; }
        public String Organization_Name_TH { get; set; }
        public String FullName { get; set; }
        public String PersonalID { get; set; }
        public String Tel { get; set; }
        public int UserStage { get; set; }   //0  no regis //1 regis 2 confirm 3 approve

        public String UserStageText { get; set; }   //0  no regis //1 regis 2 confirm 3 approve
        public String UserStatus { get; set; }
        public int UserType { get; set; } // 1 CMU 2 non CMU

        public DateTime? ConfirmEmailTime { get; set; }

        public DateTime? ConfirmTelTime { get; set; }

        public DateTime? ConfirmPersonalIDTime { get; set; }

        public DateTime? ConfirmKYCTime { get; set; }

        public DateTime? ApprovedTime { get; set; }
        public DateTime? NotApprovedTime { get; set; }

        public String fileNamePersonalID { get; set; }

        public String fileNameKYC { get; set; }


        public String AdminApproved { get; set; }

        public String AdminNotApproved { get; set; }

        public String CommetNotApproved { get; set; }

    }
}
