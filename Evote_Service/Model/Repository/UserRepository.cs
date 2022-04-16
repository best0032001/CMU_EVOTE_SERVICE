using Evote_Service.Model.Entity;
using Evote_Service.Model.Interface;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Evote_Service.Model.View;

namespace Evote_Service.Model.Repository
{
    public class UserRepository : IUserRepository
    {
        private EvoteContext _evoteContext;
        private readonly IHttpClientFactory _clientFactory;
        private IEmailRepository _emailRepository;
        public UserRepository(EvoteContext evoteContext, IHttpClientFactory clientFactory, ISMSRepository sMSRepository, IEmailRepository emailRepository)
        {
            if (evoteContext == null)
            {
                throw new System.ArgumentNullException(nameof(evoteContext));
            }
            _evoteContext = evoteContext;
            _clientFactory = clientFactory;
            _emailRepository = emailRepository;
        }
        public async Task<UserEntity> getUserEntity(string lineId)
        {

            UserEntity userEntity = _evoteContext.UserEntitys.Where(w => w.LineId == lineId && w.IsDeactivate == false).First();

            return userEntity;
        }

        public async Task<List<EventModelview>> getEventModelviewList(string lineId)
        {
            List<EventModelview> eventModelviews = new List<EventModelview>();
            UserEntity userEntity = _evoteContext.UserEntitys.Where(w => w.LineId == lineId && w.IsDeactivate == false).First();
            eventModelviews = (from voter in _evoteContext.VoterEntitys
                               join eventvote in _evoteContext.EventVoteEntitys on voter.EventVoteEntityId equals eventvote.EventVoteEntityId
                               where voter.Email == userEntity.Email && eventvote.EventStatusId == 2
                               select new EventModelview
                               {
                                   EventVoteEntityId = eventvote.EventVoteEntityId,
                                   EventTypeId = eventvote.EventTypeId,
                                   EventTitle = eventvote.EventTitle,
                                   EventDetail = eventvote.EventDetail,
                                   Organization_Code = eventvote.Organization_Code,
                                   OrganizationFullNameTha = eventvote.OrganizationFullNameTha,
                                   //EventRegisterStart = eventvote.EventRegisterStart,
                                   //EventRegisterEnd = eventvote.EventRegisterEnd,
                                   RoundNumber = eventvote.RoundNumber,
                                   EventVotingStart = eventvote.EventVotingStart,
                                   EventVotingEnd = eventvote.EventVotingEnd,
                                   EventInformation = eventvote.EventInformation,
                                   PresidentEmail = eventvote.PresidentEmail,
                                   IsUseTime = eventvote.IsUseTime,
                                   AppLink = eventvote.AppLink,
                                   IsVote=false
                               }).ToList();

            List<ConfirmVoter> confirmVoters = _evoteContext.confirmVoters.Where(w => w.email == userEntity.Email).ToList();

            foreach (EventModelview eventModelview in eventModelviews)
            {
                ConfirmVoter confirmVoter = confirmVoters.Where(w => w.EventVoteEntityId == eventModelview.EventVoteEntityId && w.RoundNumber == eventModelview.RoundNumber).FirstOrDefault();
                if (confirmVoter != null)
                {
                    eventModelview.IsVote = true;
                }
            }

            return eventModelviews.OrderByDescending(o => o.EventVoteEntityId).ToList();
        }
    }
}
