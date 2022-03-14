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
using Newtonsoft.Json;

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
            return _evoteContext.ApplicationEntitys.Where(w => w.ApplicationEntityId == ApplicationEntityId).FirstOrDefault();
        }

        public async Task<EventConfirmModelview> addEvent(int ApplicationEntityId, EventModelview eventModelview, String cmuaccount)
        {
            EventConfirmModelview eventConfirmModelview = new EventConfirmModelview();

            Random _random = new Random();
            const string chars = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789";
            String code = new string(Enumerable.Repeat(chars, 28)
              .Select(s => s[_random.Next(s.Length)]).ToArray());

            EventVoteEntity eventVoteEntity = new EventVoteEntity();
            eventVoteEntity.AppLink = eventModelview.AppLink;
            eventVoteEntity.confirmVoters = new List<ConfirmVoter>();
            eventVoteEntity.voteEntities = new List<VoteEntity>();

            eventVoteEntity.voterEntities = new List<VoterEntity>();
            eventVoteEntity.EventStatusId = 1;
            eventVoteEntity.RoundNumber = 1;
            eventVoteEntity.ApplicationEntityId = ApplicationEntityId;
            eventVoteEntity.EventTypeId = eventModelview.EventTypeId;
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
            //eventVoteEntity.EventRegisterStart = eventModelview.EventRegisterStart;
            //eventVoteEntity.EventRegisterEnd = eventModelview.EventRegisterEnd;
            eventVoteEntity.IsUseTime = eventModelview.IsUseTime;
            if (eventVoteEntity.IsUseTime)
            {
                eventVoteEntity.EventVotingStart = eventModelview.EventVotingStart;
                eventVoteEntity.EventVotingEnd = eventModelview.EventVotingEnd;
            }
            else
            {
                eventVoteEntity.EventVotingStart = DateTime.Now;
                eventVoteEntity.EventVotingEnd = DateTime.Now;
            }

            eventVoteEntity.IsEnd = false;
            _evoteContext.EventVoteEntitys.Add(eventVoteEntity);
            _evoteContext.SaveChanges();

            eventConfirmModelview.IsUseTime = eventVoteEntity.IsUseTime;
            eventConfirmModelview.EventVoteEntityId = eventVoteEntity.EventVoteEntityId;
            eventConfirmModelview.SecretKey = eventVoteEntity.SecretKey;
            eventConfirmModelview.SecurityAlgorithm = eventVoteEntity.SecurityAlgorithm;
            eventConfirmModelview.CreateUser = cmuaccount;
            eventConfirmModelview.EventTitle = eventVoteEntity.EventTitle;
            eventConfirmModelview.EventDetail = eventVoteEntity.EventDetail;
            eventConfirmModelview.Organization_Code = eventVoteEntity.Organization_Code;
            eventConfirmModelview.OrganizationFullNameTha = eventVoteEntity.OrganizationFullNameTha;
            //eventConfirmModelview.EventRegisterStart = eventVoteEntity.EventRegisterStart;
            //eventConfirmModelview.EventRegisterEnd = eventVoteEntity.EventRegisterEnd;
            eventConfirmModelview.IsUseTime = eventVoteEntity.IsUseTime;
            eventConfirmModelview.EventVotingStart = eventVoteEntity.EventVotingStart;
            eventConfirmModelview.EventVotingEnd = eventVoteEntity.EventVotingEnd;

            return eventConfirmModelview;
        }

        public async Task<bool> addVoter(VoterModelview voterModelview, string cmuaccount)
        {
            Boolean check = false;

            EventVoteEntity eventVoteEntity = _evoteContext.EventVoteEntitys.Where(w => w.EventVoteEntityId == voterModelview.EventVoteEntityId).Include(i => i.voterEntities).First();
            foreach (PeopleModelview peopleModelview in voterModelview.peopleModelviews)
            {
                if (eventVoteEntity.voterEntities.Where(w => w.Email == peopleModelview.Email).FirstOrDefault() == null)
                {
                    VoterEntity voterEntity = new VoterEntity();
                    voterEntity.Email = peopleModelview.Email;
                    voterEntity.FullName = peopleModelview.FullName;
                    voterEntity.UserType = peopleModelview.UserType;
                    voterEntity.CreateUser = cmuaccount;
                    voterEntity.VoterCreate = DateTime.Now;
                    voterEntity.EventVoteEntityId = voterModelview.EventVoteEntityId;
                    voterEntity.Organization_Code = peopleModelview.Organization_Code;
                    voterEntity.OrganizationFullNameTha = peopleModelview.OrganizationFullNameTha;
                    _evoteContext.VoterEntitys.Add(voterEntity);

                }

            }
            _evoteContext.SaveChanges();
            check = true;
            return check;

        }

        public async Task<List<EventVoteEntity>> getEventEntityByApplicationEntityId(int ApplicationEntityId)
        {
            return _evoteContext.EventVoteEntitys.Where(w => w.ApplicationEntityId == ApplicationEntityId).OrderBy(o => o.EventVoteEntityId).ToList();
        }

        public async Task<EventVoteEntity> getEventEntityByEventVoteEntityId(int ApplicationEntityId, int eventVoteEntityId)
        {
            return _evoteContext.EventVoteEntitys.Where(w => w.ApplicationEntityId == ApplicationEntityId && w.EventVoteEntityId == eventVoteEntityId).FirstOrDefault();
        }

        public async Task<List<VoterModelDataView>> getVoter(int ApplicationEntityId, int eventVoteEntityId)
        {
            List<VoterModelDataView> voterModelViews = new List<VoterModelDataView>();

            EventVoteEntity eventVoteEntitys = _evoteContext.EventVoteEntitys.Where(w => w.ApplicationEntityId == ApplicationEntityId && w.EventVoteEntityId == eventVoteEntityId).Include(i => i.voterEntities).FirstOrDefault();

            foreach (VoterEntity voterEntity in eventVoteEntitys.voterEntities.ToList())
            {
                String json = JsonConvert.SerializeObject(voterEntity);
                VoterModelDataView voterModelView = JsonConvert.DeserializeObject<VoterModelDataView>(json);
                if (voterEntity.UserType == 1)
                {
                    voterModelView.VoterType = "คนใน";
                }
                if (voterEntity.UserType == 2)
                {
                    voterModelView.VoterType = "คนนอก";
                }
                voterModelViews.Add(voterModelView);
            }
            return voterModelViews;
        }

        public async Task<bool> deleteEvent(int ApplicationEntityId, int eventVoteEntityId, string cmuaccount)
        {
            bool check = false;

            EventVoteEntity eventVoteEntitys = _evoteContext.EventVoteEntitys.Where(w => w.ApplicationEntityId == ApplicationEntityId && w.EventVoteEntityId == eventVoteEntityId && w.CreateUser == cmuaccount&&w.EventStatusId==1).Include(i => i.voterEntities).FirstOrDefault();


            foreach (VoterEntity voterEntity in eventVoteEntitys.voterEntities.ToList())
            {
                _evoteContext.VoterEntitys.Remove(voterEntity);
            }
            _evoteContext.EventVoteEntitys.Remove(eventVoteEntitys);
            _evoteContext.SaveChanges();

            return true;

        }

        public async Task<bool> deleteVoter(VoterModelview voterModelview, string cmuaccount)
        {
            bool check = false;

            EventVoteEntity eventVoteEntitys = _evoteContext.EventVoteEntitys.Where(w => w.EventVoteEntityId == voterModelview.EventVoteEntityId && w.CreateUser == cmuaccount).Include(i => i.voterEntities).FirstOrDefault();
            List<VoterEntity> voterEntities = eventVoteEntitys.voterEntities.ToList();
            foreach (PeopleModelview peopleModelview in voterModelview.peopleModelviews)
            {
                VoterEntity voterEntitie = voterEntities.Where(w => w.Email == peopleModelview.Email).FirstOrDefault();
                if (voterEntitie != null)
                {
                    _evoteContext.VoterEntitys.Remove(voterEntitie);
                }
            }
            _evoteContext.SaveChanges();
            return check;
        }

        public async Task<bool> ConfirmEvent(int ApplicationEntityId, int eventVoteEntityId, string cmuaccount, int voteround)
        {
            bool check = false;

            EventVoteEntity eventVoteEntitys = _evoteContext.EventVoteEntitys.Where(w => w.EventVoteEntityId == eventVoteEntityId && w.PresidentEmail == cmuaccount).FirstOrDefault();
            eventVoteEntitys.EventStatusId = 2;
            eventVoteEntitys.PresidentUpdate = DateTime.Now;

            if (voteround != 0)
            {
                String AppLink = eventVoteEntitys.AppLink;
                int index = AppLink.IndexOf("voteround");
                AppLink = AppLink.Substring(0, 86);
                AppLink = AppLink + voteround;
                eventVoteEntitys.AppLink = AppLink;
                eventVoteEntitys.RoundNumber = voteround;
            }
            _evoteContext.SaveChanges();


            return true;
        }

        public async Task<List<UserModelDataView>> getUser(UserModelSearch userModelSearch)
        {
            List<UserModelDataView> userModelDataViews = new List<UserModelDataView>();
            List<UserEntity> userEntities = new List<UserEntity>();
            if (userModelSearch.FullName == null) { userModelSearch.FullName = ""; }
            if (userModelSearch.FullName != "")
            {
                userEntities = _evoteContext.UserEntitys.Where(w => w.UserType == 2 &&w.UserStage==3&& w.FullName.Contains(userModelSearch.FullName))
               .ToList();
            }
           
            foreach (UserEntity userEntity in userEntities)
            {
                UserModelDataView userModelDataView = new UserModelDataView();
                userModelDataView.Email = userEntity.Email;
                userModelDataView.FullName = userEntity.FullName;
                userModelDataView.Organization_Code = userEntity.Organization_Code;
                userModelDataViews.Add(userModelDataView);
            }

            return userModelDataViews;
        }
    }
}
