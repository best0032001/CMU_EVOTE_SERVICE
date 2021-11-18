using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.Entity.Ref
{
    public class RefUserStage
    {
        [Key]
        public int RefUserStageID { get; set; }
        public string UserStageName { get; set; }

    }
}
