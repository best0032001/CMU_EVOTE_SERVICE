using Evote_Service.Model.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.Interface
{
    public interface ICheckUserRepository
    {

        Task<UserModel> CheckLineUser(String lineId);
    }
}
