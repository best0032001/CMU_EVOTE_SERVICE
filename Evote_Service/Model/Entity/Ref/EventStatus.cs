using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.Entity.Ref
{
    public class EventStatus
    {
        [Key]
        public int EventStatusId { get; set; } // 1 now 2 incoming 3 passed
        public String EventStatusName { get; set; } // 1 now 2 incoming 3 passed
    }
}
