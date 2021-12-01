using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.View
{
    public class OTPModelview
    {
        public String otp { get; set; }
    }
    public class AdminLoginOTPModelview
    {
        public String otp { get; set; }
        public String RefCode { get; set; }
    }
}
