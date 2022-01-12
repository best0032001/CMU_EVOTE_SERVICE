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

            UserEntity userEntity = _evoteContext.UserEntitys.Where(w => w.LineId == lineId).First();

            return userEntity;
        }

        public async Task<List<EventModelview>> getEventModelviewList(string lineId)
        {
            List<EventModelview> eventModelviews = new List<EventModelview>();
            UserEntity userEntity = _evoteContext.UserEntitys.Where(w => w.LineId == lineId).First();
            eventModelviews = (from vote in _evoteContext.VoterEntitys
                               join eventvote in _evoteContext.EventVoteEntitys on vote.EventVoteEntityId equals eventvote.EventVoteEntityId
                               where vote.Email == userEntity.Email
                               select new EventModelview
                               {
                                   EventTypeId = eventvote.EventTypeId,
                                   EventTitle = eventvote.EventTitle,
                                   EventDetail = eventvote.EventDetail,
                                   Organization_Code = eventvote.Organization_Code,
                                   OrganizationFullNameTha = eventvote.OrganizationFullNameTha,
                                   EventRegisterStart = eventvote.EventRegisterStart,
                                   EventRegisterEnd = eventvote.EventRegisterEnd,
                                   EventVotingStart = eventvote.EventRegisterStart,
                                   EventVotingEnd = eventvote.EventVotingEnd,
                                   EventInformation = eventvote.EventInformation,
                                   PresidentEmail = eventvote.PresidentEmail,
                                   AppLink = ""
                               }).ToList();

            return eventModelviews;
        }
    }
}
