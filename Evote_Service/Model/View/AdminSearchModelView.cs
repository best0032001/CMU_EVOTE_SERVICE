﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.View
{
    public class AdminSearchModelView
    {
        public String Email { get; set; }
        public String Organization_Code { get; set; }
        public String Organization_Name_TH { get; set; }
        public String FullName { get; set; }
        public String PersonalID { get; set; }
        public String Tel { get; set; }
        public int UserStage { get; set; }
    }
}
