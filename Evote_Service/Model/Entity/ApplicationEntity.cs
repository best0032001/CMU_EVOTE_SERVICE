using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.Entity
{
    public class ApplicationEntity
    {
        public int ApplicationEntityId { get; set; }


        [Column(TypeName = "varchar(250)")]
        public String ApplicationName { get; set; }
        [Column(TypeName = "varchar(50)")]
        public String ClientId { get; set; }

        public Boolean LineAuth { get; set; }
        public Boolean CMUAuth { get; set; }

        [Column(TypeName = "varchar(50)")]
        public String ServerProductionIP { get; set; }

        public List<EventVoteEntity> EventVoteEntitys { get; set; }
    }
}
