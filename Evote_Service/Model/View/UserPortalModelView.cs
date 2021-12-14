using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.View
{
    public class UserPortalModelView
    {
        public List<EventModelview> eventModelviewsNow { get; set; }
        public List<EventModelview> eventModelviewsIncomming { get; set; }

        public List<EventModelview> eventModelviewsPassed { get; set; }
    }
}
