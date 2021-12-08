using Evote_Service.Model.Entity;
using Evote_Service.Model.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.Interface
{
    public interface IEventRepository
    {
        Task<ApplicationEntity> getApplicationEntity(int ApplicationEntityId);

        Task<EventConfirmModelview> addEvent(int ApplicationEntityId,EventModelview eventModelview,String cmuaccount);
    }
}
