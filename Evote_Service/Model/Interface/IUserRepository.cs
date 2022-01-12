using Evote_Service.Model.Entity;
using Evote_Service.Model.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.Interface
{
    public interface IUserRepository
    {
        Task<UserEntity> getUserEntity(String lineId);

        Task<List<EventModelview>> getEventModelviewList(String lineId);
    }
}
