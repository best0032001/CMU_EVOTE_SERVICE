using Evote_Service.Model.Entity;
using Evote_Service.Model.Entity.Ref;
using Evote_Service.Model.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Evote_Service.Model.View;
using Evote_Service.Model.Interface;

namespace Evote_Service.Model
{
    public class SetData
    {
        private EvoteContext _evoteContext;
        private IEventRepository _eventRepository;
        public SetData(IWebHostEnvironment env, EvoteContext evoteContext, IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
            _evoteContext = evoteContext;
            if (env.IsEnvironment("test")) { innitMock(); }
            if (evoteContext == null)
            {
                throw new System.ArgumentNullException(nameof(evoteContext));
            }
            innit();
        }
        private void innitMock()
        {
            DataCache.AdminMocks = new List<AdminMock>();
            DataCache.UserMocks = new List<UserMock>();
            UserMock userMock1 = new UserMock();
            userMock1.token = "x01";
            userMock1.lineId = "l01";
            DataCache.UserMocks.Add(userMock1);

            UserMock userMock2 = new UserMock();
            userMock2.token = "x02";
            userMock2.lineId = "l02";
            userMock2.email = "cheewin.b@cmu.ac.th";
            DataCache.UserMocks.Add(userMock2);

            AdminMock adminMock = new AdminMock();
            adminMock.token = "a01";
            adminMock.Cmuaccount = "cheewin.b@cmu.ac.th";
            DataCache.AdminMocks.Add(adminMock);

            UserEntity userEntity = new UserEntity();
            userEntity.Email = "cheewin.b@cmu.ac.th";
            userEntity.FullName = "test";
            userEntity.Tel = "1234";
            userEntity.UserStage = 3;
            userEntity.UserType = 2;
            userEntity.LineId = "l02";

            UserEntity userEntityTest = new UserEntity();
            userEntityTest.Email = "test@test.com";
            userEntityTest.FullName = "test";
            userEntityTest.Tel = "1234";
            userEntityTest.UserStage = 2;


            _evoteContext.UserEntitys.Add(userEntity);
            _evoteContext.UserEntitys.Add(userEntityTest);
            _evoteContext.SaveChanges();


        }
        private void innit()
        {
            setDefaultAdmin();
            setRefUserStage();
            setApplicationTest();
            setEventTest();
        }
        private void setDefaultAdmin()
        {

            UserAdminEntity model= _evoteContext.UserAdminEntitys.Where(w => w.Cmuaccount == "cheewin.b@cmu.ac.th").FirstOrDefault();
            if (model == null)
            {
                UserAdminEntity userAdminEntityFirst = new UserAdminEntity();
                userAdminEntityFirst.Cmuaccount = "cheewin.b@cmu.ac.th";
                userAdminEntityFirst.SuperAdmin = true;
                _evoteContext.UserAdminEntitys.Add(userAdminEntityFirst);

                UserAdminEntity userAdminEntitySec = new UserAdminEntity();
                userAdminEntitySec.Cmuaccount = "jirakit.s@cmu.ac.th";
                userAdminEntitySec.SuperAdmin = true;
                _evoteContext.UserAdminEntitys.Add(userAdminEntitySec);
                _evoteContext.SaveChanges();
            }
    
        }
        private void setRefUserStage()
        {
            List<RefUserStage> list = _evoteContext.RefUserStages.ToList();
            if (list.Count == 0)
            {
                RefUserStage refUserStage1 = new RefUserStage();
                refUserStage1.RefUserStageID = 1;
                refUserStage1.UserStageName = "regis";
                _evoteContext.RefUserStages.Add(refUserStage1);
                _evoteContext.SaveChanges();

                RefUserStage refUserStage2 = new RefUserStage();
                refUserStage2.RefUserStageID = 2;
                refUserStage2.UserStageName = "confirm";
                _evoteContext.RefUserStages.Add(refUserStage2);
                _evoteContext.SaveChanges();

                RefUserStage refUserStage3 = new RefUserStage();
                refUserStage3.RefUserStageID = 3;
                refUserStage3.UserStageName = "approved";
                _evoteContext.RefUserStages.Add(refUserStage3);
                _evoteContext.SaveChanges();

                RefUserStage refUserStage4 = new RefUserStage();
                refUserStage4.RefUserStageID = 4;
                refUserStage4.UserStageName = "rejected";
                _evoteContext.RefUserStages.Add(refUserStage4);
                _evoteContext.SaveChanges();
            }
        }

        private void setApplicationTest()
        {
            List<ApplicationEntity> list = _evoteContext.ApplicationEntitys.ToList();
            if (list.Count == 0)
            {
                ApplicationEntity applicationEntity = new ApplicationEntity();
                applicationEntity.ApplicationName = "ApplicationNametest";
                applicationEntity.ClientId = "ClientIdtest";
                applicationEntity.LineAuth = true;
                applicationEntity.CMUAuth = false;
                applicationEntity.ServerProductionIP = "10.10.10.prod";
                applicationEntity.EventVoteEntitys = new List<EventVoteEntity>();
                _evoteContext.ApplicationEntitys.Add(applicationEntity);
                _evoteContext.SaveChanges();
            }
        }
        private void setEventTest()
        {
            ApplicationEntity applicationEntity= _evoteContext.ApplicationEntitys.Where(w => w.ApplicationEntityId == 1).Include(i => i.EventVoteEntitys).First();
            if (applicationEntity.EventVoteEntitys.ToList().Count == 0)
            {
                EventModelview eventModelview = new EventModelview();
                eventModelview.EventTitle = "Test";
                eventModelview.EventDetail = "Test";
                eventModelview.Organization_Code = "00";
                eventModelview.OrganizationFullNameTha = "00";
                eventModelview.EventRegisterStart = DateTime.Now;
                eventModelview.EventRegisterEnd = DateTime.Now;
                eventModelview.EventVotingStart = DateTime.Now;
                eventModelview.EventVotingEnd = DateTime.Now;

              _eventRepository.addEvent(1, eventModelview, "cheewin.b@cmu.ac.th");

                VoterModelview voterModelview = new VoterModelview();
                voterModelview.EventVoteEntityId = 1;
                voterModelview.peopleModelviews = new List<PeopleModelview>();
                PeopleModelview peopleModelviewModel1 = new PeopleModelview();
                PeopleModelview peopleModelviewModel2 = new PeopleModelview();
                PeopleModelview peopleModelviewModel3 = new PeopleModelview();


                peopleModelviewModel1.Email = "cheewin.b@cmu.ac.th";
                peopleModelviewModel1.Organization_Code = "0000000043";
                peopleModelviewModel2.Email = "jirakit.s@cmu.ac.th";
                peopleModelviewModel2.Organization_Code = "0000000043";
                peopleModelviewModel3.Email = "test@test.com";
                peopleModelviewModel3.Organization_Code = "0000000043";

                voterModelview.peopleModelviews.Add(peopleModelviewModel1);
                voterModelview.peopleModelviews.Add(peopleModelviewModel2);
                voterModelview.peopleModelviews.Add(peopleModelviewModel3);
                _eventRepository.addVote(voterModelview, "cheewin.b@cmu.ac.th");

            }
        }
    }
}
