using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.Entity
{
    public class AdminLoginLog
    {
        public int AdminLoginLogId { get; set; }

        public String Cmuaccount { get; set; }
        public DateTime LoginTime { get; set; }

        public String ClientIP { get; set; }
    }
}
