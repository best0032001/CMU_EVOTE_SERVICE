using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.sms
{
    public class SMSModel
    {
        public List<SMSMessages> messages { get; set; }
    }

    public class SMSMessages 
    { 
        public String from { get; set; }
        public List<SMSdestinations> destinations { get; set; }
        public String text { get; set; }
    }
    public class SMSdestinations
    {
        public String to { get; set; }
        public String messageId { get; set; }
    }
}
