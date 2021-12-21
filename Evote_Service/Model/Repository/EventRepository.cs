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
using Microsoft.IdentityModel.Tokens;

namespace Evote_Service.Model.Repository
{
    public class EventRepository : IEventRepository
    {
        private EvoteContext _evoteContext;

        private readonly IHttpClientFactory _clientFactory;
        public EventRepository(EvoteContext evoteContext, IHttpClientFactory clientFactory, ISMSRepository sMSRepository, IEmailRepository emailRepository)
        {
            if (evoteContext == null)
            {
                throw new System.ArgumentNullException(nameof(evoteContext));
            }
            _evoteContext = evoteContext;
            _clientFactory = clientFactory;

        }

        public async Task<ApplicationEntity> getApplicationEntity(int ApplicationEntityId)
        {
            return _evoteContext.ApplicationEntitys.Where(w => w.ApplicationEntityId == ApplicationEntityId).Include(i => i.EventVoteEntitys).FirstOrDefault();
        }

        public async Task<EventConfirmModelview> addEvent(int ApplicationEntityId, EventModelview eventModelview, String cmuaccount)
        {
            EventConfirmModelview eventConfirmModelview = new EventConfirmModelview();

            Random _random = new Random();
            const string chars = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789";
            String code = new string(Enumerable.Repeat(chars, 28)
              .Select(s => s[_random.Next(s.Length)]).ToArray());

            EventVoteEntity eventVoteEntity = new EventVoteEntity();
            eventVoteEntity.voteRoundEntities = new List<VoteRoundEntity>();
         
            eventVoteEntity.EventStatusId = 2;
            eventVoteEntity.ApplicationEntityId = ApplicationEntityId;
            eventVoteEntity.EventTypeId = 0;
            eventVoteEntity.SecretKey = code;
            eventVoteEntity.SecurityAlgorithm = SecurityAlgorithms.HmacSha256Signature;
            eventVoteEntity.EventTitle = eventModelview.EventTitle;
            eventVoteEntity.EventInformation = eventModelview.EventInformation;
            eventVoteEntity.EventDetail = eventModelview.EventDetail;
            eventVoteEntity.CreateUser = cmuaccount;
            eventVoteEntity.UpdateUser = cmuaccount;
            eventVoteEntity.PresidentEmail = eventModelview.PresidentEmail;
            eventVoteEntity.Organization_Code = eventModelview.Organization_Code;
            eventVoteEntity.OrganizationFullNameTha = eventModelview.OrganizationFullNameTha;
            eventVoteEntity.EventCreate = DateTime.Now;
            eventVoteEntity.EventUpdate = DateTime.Now;
            eventVoteEntity.EventRegisterStart = eventModelview.EventRegisterStart;
            eventVoteEntity.EventRegisterEnd = eventModelview.EventRegisterEnd;
            eventVoteEntity.EventVotingStart = eventModelview.EventVotingStart;
            eventVoteEntity.EventVotingEnd = eventModelview.EventVotingEnd;
            eventVoteEntity.IsEnd = false;
            _evoteContext.EventVoteEntitys.Add(eventVoteEntity);
            _evoteContext.SaveChanges();

            VoteRoundEntity voteRoundEntity = new VoteRoundEntity();
            voteRoundEntity.EventVoteEntityId = eventVoteEntity.EventVoteEntityId;
            voteRoundEntity.RoundNumber = 1;
            voteRoundEntity.confirmVoters = new List<ConfirmVoter>();
            voteRoundEntity.voteEntities = new List<VoteEntity>();
            _evoteContext.voteRoundEntities.Add(voteRoundEntity);
            _evoteContext.SaveChanges();


            eventConfirmModelview.EventVoteEntityId = eventVoteEntity.EventVoteEntityId;
            eventConfirmModelview.SecretKey = eventVoteEntity.SecretKey;
            eventConfirmModelview.SecurityAlgorithm = eventVoteEntity.SecurityAlgorithm;
            eventConfirmModelview.CreateUser = cmuaccount;
            eventConfirmModelview.EventTitle = eventVoteEntity.EventTitle;
            eventConfirmModelview.EventDetail = eventVoteEntity.EventDetail;
            eventConfirmModelview.Organization_Code = eventVoteEntity.Organization_Code;
            eventConfirmModelview.OrganizationFullNameTha = eventVoteEntity.OrganizationFullNameTha;
            eventConfirmModelview.EventRegisterStart = eventVoteEntity.EventRegisterStart;
            eventConfirmModelview.EventRegisterEnd = eventVoteEntity.EventRegisterEnd;
            eventConfirmModelview.EventVotingStart = eventVoteEntity.EventVotingStart;
            eventConfirmModelview.EventVotingEnd = eventVoteEntity.EventVotingEnd;

            return eventConfirmModelview;
        }

        public async Task<bool> addVote(VoterModelview voterModelview, string cmuaccount)
        {
            Boolean check = false;
            List<UserEntity> userEntitys= _evoteContext.UserEntitys.Where(w=>w.UserStage==3).Include(i=>i.eventVoteEntities).ToList();
            EventVoteEntity eventVoteEntity= _evoteContext.EventVoteEntitys.Where(w => w.EventVoteEntityId == voterModelview.EventVoteEntityId).Include(i=>i.voterEntities).First();
            foreach (PeopleModelview peopleModelview in voterModelview.peopleModelviews)
            {
                if (eventVoteEntity.voterEntities.Where(w => w.Email == peopleModelview.Email).FirstOrDefault() == null)
                {
                    VoterEntity voterEntity = new VoterEntity();
                    voterEntity.Email = peopleModelview.Email;
                    voterEntity.CreateUser = cmuaccount;
                    voterEntity.VoterCreate = DateTime.Now;
                    voterEntity.EventVoteEntityId = voterModelview.EventVoteEntityId;
                    voterEntity.Organization_Code = peopleModelview.Organization_Code;
                    _evoteContext.VoterEntitys.Add(voterEntity);
                    UserEntity userEntity= userEntitys.Where(w => w.Email == voterEntity.Email).FirstOrDefault();
                    if (userEntity != null)
                    {
                         EventVoteEntity eventVoteEntityCheck= userEntity.eventVoteEntities.Where(w => w.EventVoteEntityId == voterModelview.EventVoteEntityId).FirstOrDefault();
                        if (eventVoteEntityCheck == null)
                        {
                            userEntity.eventVoteEntities.Add(eventVoteEntity);
                        }
                    }

                }
              
            }
            _evoteContext.SaveChanges();
            check = true;
            return check;

        }
    }
}
