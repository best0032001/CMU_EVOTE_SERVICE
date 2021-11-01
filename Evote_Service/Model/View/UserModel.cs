﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.View
{
    public class UserModel
    {
        public String Email { get; set; }
        public String FullName { get; set; }
        public int UserStage { get; set; }
        public int UserType { get; set; }

        public String Tel { get; set; }
        public Boolean IsConfirmEmail { get; set; }
        public DateTime? ConfirmEmailTime { get; set; }
        public Boolean IsConfirmTel { get; set; }
        public DateTime? ConfirmTelTime { get; set; }
        public Boolean IsConfirmPersonalID { get; set; }
        public Boolean IsConfirmKYC { get; set; }
    }
}