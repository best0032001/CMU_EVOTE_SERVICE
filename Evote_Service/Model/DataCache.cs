﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model
{
    public static class DataCache
    {
        public static List<UserMock> UserMocks;
    }

    public class UserMock
    {
        public string token { get; set; }
        public string lineId { get; set; }
    }
}
