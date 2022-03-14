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
        Task<List<EventVoteEntity>> getEventEntityByApplicationEntityId(int ApplicationEntityId);
        Task<EventVoteEntity> getEventEntityByEventVoteEntityId(int ApplicationEntityId,int eventVoteEntityId);
        Task<EventConfirmModelview> addEvent(int ApplicationEntityId,EventModelview eventModelview,String cmuaccount);


        Task<Boolean> deleteEvent(int ApplicationEntityId, int eventVoteEntityId, String cmuaccount);

        Task<Boolean> ConfirmEvent(int ApplicationEntityId, int eventVoteEntityId, String cmuaccount, int voteround);
        Task<Boolean> addVoter( VoterModelview voterModelview, String cmuaccount);

        Task<Boolean> deleteVoter(VoterModelview voterModelview, String cmuaccount);

        Task<List<VoterModelDataView>> getVoter(int ApplicationEntityId, int eventVoteEntityId);

        Task<List<UserModelDataView>> getUser(UserModelSearch userModelSearch);
        
    }
}
