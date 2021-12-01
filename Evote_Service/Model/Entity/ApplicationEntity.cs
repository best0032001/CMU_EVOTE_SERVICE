using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.Entity
{
    public class ApplicationEntity
    {
        public int ApplicationEntityId { get; set; }

        public String ApplicationName { get; set; }

        public String ServerProductionIP { get; set; }
        public String ServerDevIP { get; set; }
        public List<EventVoteEntity> EventVoteEntitys { get; set; }
    }
}
