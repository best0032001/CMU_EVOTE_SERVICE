using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.Entity
{
    public class UserAdminEntity
    {
        public int UserAdminEntityId { get; set; }


        [Column(TypeName = "varchar(250)")]
        public String FullName { get; set; }


        [Column(TypeName = "varchar(50)")]
        public String Cmuaccount { get; set; }


        [Column(TypeName = "varchar(20)")]
        public String Tel { get; set; }

        public Boolean SuperAdmin { get; set; }
        public Boolean OrganAdmin { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string Organization_Code { get; set; }

        [Column(TypeName = "varchar(205)")]
        public string OrganizationFullNameTha { get; set; }



        [Column(TypeName = "varchar(10)")]
        public String SMSOTP { get; set; }
        [Column(TypeName = "varchar(10)")]
        public String SMSOTPRef { get; set; }

        public DateTime? SMSExpire { get; set; }



        [Column(TypeName = "varchar(50)")]
        public String Access_token { get; set; }
        [Column(TypeName = "varchar(50)")]
        public String Refresh_token { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string CreateUser { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string CreateIP { get; set; }
    }
}
