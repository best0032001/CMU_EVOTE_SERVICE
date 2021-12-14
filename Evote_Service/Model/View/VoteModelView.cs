using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.View
{
    public class VoteModelView
    {
        public Boolean lineMode { get; set; }
        public int EventVoteEntityId { get; set; }
        public int ApplicationEntityId { get; set; }
        public int EventTypeId { get; set; }

        public String TokenData { get; set; }
        public String OTP { get; set; }
        public String RefOTP { get; set; }
    }

    public class VoteSMSModelView
    {
        public Boolean lineMode { get; set; }
        public int EventVoteEntityId { get; set; }
        public int ApplicationEntityId { get; set; }
        public int EventTypeId { get; set; }
    }
    public class VoteDataModel
    {
        public String data { get; set; }
    }
}
