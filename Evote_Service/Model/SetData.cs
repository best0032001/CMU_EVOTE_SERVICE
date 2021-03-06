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

            String RAW_KEY = Environment.GetEnvironmentVariable("RAW_KEY");
            String PASS_KEY = Environment.GetEnvironmentVariable("PASS_KEY");
            Crypto crypto = new Crypto(PASS_KEY, RAW_KEY);


            userEntity.Tel = crypto.Encrypt("1234");
            userEntity.UserStage = 3;
            userEntity.UserType = 2;
            userEntity.LineId = "l02";
            userEntity.PersonalID = crypto.Encrypt("1234");

            userEntity.Organization_Name_TH = "-";

            UserEntity userEntityTest = new UserEntity();
            userEntityTest.Email = "jirakit.s@cmu.ac.th";
            userEntityTest.FullName = "test";
            userEntityTest.Tel = crypto.Encrypt("1234");
            userEntityTest.PersonalID = crypto.Encrypt("1234");
            userEntityTest.Organization_Name_TH = "-";
            userEntityTest.UserStage = 2;


            _evoteContext.UserEntitys.Add(userEntity);
            _evoteContext.UserEntitys.Add(userEntityTest);
            _evoteContext.SaveChanges();


        }
        private void innit()
        {

            setRef();
            setApplicationTest();
            setEventTest();
            setDefaultAdmin();
        }
        private void setDefaultAdmin()
        {

            UserAdminEntity model = _evoteContext.UserAdminEntitys.Where(w => w.Cmuaccount == "cheewin.b@cmu.ac.th").FirstOrDefault();
            if (model == null)
            {
                UserAdminEntity userAdminEntityFirst = new UserAdminEntity();
                userAdminEntityFirst.Cmuaccount = "cheewin.b@cmu.ac.th";
                userAdminEntityFirst.FullName = "people 1";
                userAdminEntityFirst.SuperAdmin = true;
                userAdminEntityFirst.OrganizationFullNameTha = "-";
                userAdminEntityFirst.Tel = Environment.GetEnvironmentVariable("ADMIN_TEL1");
                _evoteContext.UserAdminEntitys.Add(userAdminEntityFirst);

                UserAdminEntity userAdminEntitySec = new UserAdminEntity();
                userAdminEntitySec.Cmuaccount = "jirakit.s@cmu.ac.th";
                userAdminEntitySec.FullName = "people 2";
                userAdminEntitySec.SuperAdmin = true;
                userAdminEntitySec.OrganizationFullNameTha = "-";
                userAdminEntitySec.Tel = Environment.GetEnvironmentVariable("ADMIN_TEL2");
                _evoteContext.UserAdminEntitys.Add(userAdminEntitySec);
                _evoteContext.SaveChanges();
            }

        }
        private void setRef()
        {
            List<RefUserStage> list = _evoteContext.RefUserStages.ToList();
            if (list.Count == 0)
            {
                RefUserStage refUserStage1 = new RefUserStage();
                //refUserStage1.RefUserStageID = 1;
                refUserStage1.UserStageName = "Regis";
                _evoteContext.RefUserStages.Add(refUserStage1);
                _evoteContext.SaveChanges();

                RefUserStage refUserStage2 = new RefUserStage();
                //refUserStage2.RefUserStageID = 2;
                refUserStage2.UserStageName = "Confirm";
                _evoteContext.RefUserStages.Add(refUserStage2);
                _evoteContext.SaveChanges();

                RefUserStage refUserStage3 = new RefUserStage();
                //refUserStage3.RefUserStageID = 3;
                refUserStage3.UserStageName = "Approved";
                _evoteContext.RefUserStages.Add(refUserStage3);
                _evoteContext.SaveChanges();

                RefUserStage refUserStage4 = new RefUserStage();
                //refUserStage4.RefUserStageID = 4;
                refUserStage4.UserStageName = "Rejected";
                _evoteContext.RefUserStages.Add(refUserStage4);
                _evoteContext.SaveChanges();


            }
            DataCache.RefUserStages = _evoteContext.RefUserStages.OrderBy(o => o.RefUserStageID).ToList();

            List<EventStatus> eventStatuses = _evoteContext.EventStatus.ToList();
            if (eventStatuses.Count == 0)
            {
                EventStatus eventStatus1 = new EventStatus();
                //eventStatus1.EventStatusId = 1;
                eventStatus1.EventStatusName = "SetUP"; //   admin  สร้าง ยังลบแก้ไขได้
                _evoteContext.EventStatus.Add(eventStatus1);
                _evoteContext.SaveChanges();

                EventStatus eventStatus2 = new EventStatus();
                //eventStatus2.EventStatusId = 2;
                eventStatus2.EventStatusName = "PresidentConfirm"; //   admin  สร้างลบแก้ไขไม่ได้
                _evoteContext.EventStatus.Add(eventStatus2);
                _evoteContext.SaveChanges();

                EventStatus eventStatus3 = new EventStatus();
                //eventStatus3.EventStatusId = 3;
                eventStatus3.EventStatusName = "End Process"; //   
                _evoteContext.EventStatus.Add(eventStatus3);
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

                ApplicationEntity applicationEntity2 = new ApplicationEntity();
                applicationEntity2.ApplicationName = "ApplicationNametes2";
                applicationEntity2.ClientId = Environment.GetEnvironmentVariable("CMU_CLIENT_ID");
                applicationEntity2.LineAuth = true;
                applicationEntity2.CMUAuth = false;
                applicationEntity2.ServerProductionIP = Environment.GetEnvironmentVariable("E_COUNCIL_IP");
                applicationEntity2.EventVoteEntitys = new List<EventVoteEntity>();
                _evoteContext.ApplicationEntitys.Add(applicationEntity2);
                _evoteContext.SaveChanges();
            }
        }
        private void setEventTest()
        {
            ApplicationEntity applicationEntity = _evoteContext.ApplicationEntitys.Where(w => w.ApplicationEntityId == 1).Include(i => i.EventVoteEntitys).First();
            if (applicationEntity.EventVoteEntitys.ToList().Count == 0)
            {
                EventModelview eventModelview = new EventModelview();
                eventModelview.EventTitle = "Test";
                eventModelview.EventDetail = "Test";
                eventModelview.Organization_Code = "00";
                eventModelview.OrganizationFullNameTha = "00";
                //eventModelview.EventRegisterStart = DateTime.Now;
                //eventModelview.EventRegisterEnd = DateTime.Now;
                eventModelview.EventVotingStart = DateTime.Now;
                eventModelview.EventVotingEnd = DateTime.Now;
                eventModelview.PresidentEmail = "cheewin.b@cmu.ac.th";
                _eventRepository.addEvent(1, eventModelview, "cheewin.b@cmu.ac.th");
                _eventRepository.addEvent(2, eventModelview, "cheewin.b@cmu.ac.th");


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
                _eventRepository.addVoter(voterModelview, "cheewin.b@cmu.ac.th");

            }
        }
    }
}
