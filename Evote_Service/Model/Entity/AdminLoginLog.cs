using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.Entity
{
    public class AdminLoginLog
    {
        public int AdminLoginLogId { get; set; }


        [Column(TypeName = "varchar(100)")]
        public String Cmuaccount { get; set; }
        public DateTime LoginTime { get; set; }

        [Column(TypeName = "varchar(50)")]
        public String ClientIP { get; set; }
    }
}
