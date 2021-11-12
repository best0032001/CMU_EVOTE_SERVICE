using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evote_Service.Model.Interface
{
    public interface ISMSRepository
    {
        Task<String> getOTP(String RefCode, String tel);
    }
}
