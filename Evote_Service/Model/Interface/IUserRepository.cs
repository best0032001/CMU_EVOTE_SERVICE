using Evote_Service.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.Interface
{
    public interface IUserRepository
    {
        Task<UserEntity> getEvent(String lineId, int EventStatusId);


    }
}
