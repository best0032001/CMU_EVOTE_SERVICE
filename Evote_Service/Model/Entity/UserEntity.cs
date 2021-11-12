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
        public String FullName { get; set; }
        public String Tel { get; set; }
        public int UserStage { get; set; }   //0  no regis //1 regis 2 confirm 3 approve
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
        public Boolean IsConfirmKYC { get; set; }

        public String UserApproved { get; set; }
    }
}
