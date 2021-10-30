﻿using System;
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
        public int UserStage { get; set; }   //0  no regis //1 regis 2 confirm
        public int UserType { get; set; } // 1 CMU 2 non CMU
        public String LineId { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
