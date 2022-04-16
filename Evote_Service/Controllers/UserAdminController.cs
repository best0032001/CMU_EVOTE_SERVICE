

using Evote_Service.Model;
using Evote_Service.Model.Entity;
using Evote_Service.Model.Interface;
using Evote_Service.Model.Util;
using Evote_Service.Model.View;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Evote_Service.Controllers
{
    [Produces("application/json")]
    [Route("api/")]
    [ApiController]
    public class UserAdminController : ITSCController
    {
        private EvoteContext _evoteContext;
        private IUserAdminRepository _userAdminRepository;
        public UserAdminController(ILogger<ITSCController> logger, IAdminRepository IAdminRepository, IHttpClientFactory clientFactory, IUserAdminRepository userAdminRepository, IWebHostEnvironment env, IEmailRepository emailRepository, IEventRepository eventRepository, EvoteContext evoteContext)
        {

            this.loadConfig(logger, clientFactory, env);
            _emailRepository = emailRepository;
            _userAdminRepository = userAdminRepository;
            _evoteContext = evoteContext;

        }

        [HttpGet("v1/admin/menu")]
        [ProducesResponseType(typeof(List<UserMenuView>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> getMenu()
        {

            String Cmuaccount = "";
            String action = "UserAdminController.getMenu";
            List<UserMenuView> userMenuViews = new List<UserMenuView>();
            try
            {
                APIModel aPIModel = new APIModel();
                String json = "";
                Cmuaccount = await this.getCmuaccount();
                if (Cmuaccount == "unauthorized") { return Unauthorized(); }
                UserAdminEntity userAdminEntity = _evoteContext.UserAdminEntitys.Where(w => w.Cmuaccount == Cmuaccount).FirstOrDefault();
                if (userAdminEntity == null)
                {
                    return Unauthorized();
                }
                if (userAdminEntity.SuperAdmin)
                {
                    UserMenuView userMenuView1 = new UserMenuView();
                    userMenuView1.MenuName = "คนรออนุมัติ";
                    userMenuView1.MenuLink = "/admin/dashboardsuperadmin";
                    userMenuView1.icon = "mdi-home";
                    userMenuViews.Add(userMenuView1);

                    UserMenuView userMenuView2 = new UserMenuView();
                    userMenuView2.MenuName = "จัดการผู้ใช้งาน";
                    userMenuView2.MenuLink = "/admin/usemanagesuperadmin";
                    userMenuView2.icon = "mdi-home";
                    userMenuViews.Add(userMenuView2);
                }
                else
                {
                    UserMenuView userMenuView1 = new UserMenuView();
                    userMenuView1.MenuName = "คนรออนุมัติ";
                    userMenuView1.MenuLink = "/admin/dashboard";
                    userMenuView1.icon = "mdi-home";
                    userMenuViews.Add(userMenuView1);

                    UserMenuView userMenuView2 = new UserMenuView();
                    userMenuView2.MenuName = "จัดการผู้ใช้งาน";
                    userMenuView2.MenuLink = "/admin/usemanage";
                    userMenuView2.icon = "mdi-home";
                    userMenuViews.Add(userMenuView2);
                }


               

                if (userAdminEntity.SuperAdmin)
                {
                    UserMenuView userMenuView4 = new UserMenuView();
                    userMenuView4.MenuName = "จัดการผู้ดูแลระดับสูงสุด";
                    userMenuView4.MenuLink = "/admin/superadmin";
                    userMenuView4.icon = "mdi-home";
                    userMenuViews.Add(userMenuView4);
                }
                if (userAdminEntity.OrganAdmin)
                {
                    UserMenuView userMenuView3 = new UserMenuView();
                    userMenuView3.MenuName = "จัดการผู้ดูแลระดับส่วนงาน";
                    userMenuView3.MenuLink = "/admin/organadmin";
                    userMenuView3.icon = "mdi-home";
                    userMenuViews.Add(userMenuView3);
                }

                aPIModel.data = userMenuViews;
                aPIModel.title = "Success";
                return StatusCodeITSC("CMU", "", Cmuaccount, action, 200, aPIModel);

            }
            catch (Exception ex) { return StatusErrorITSC("CMU", "", Cmuaccount, action, ex); }
        }

        [HttpGet("v1/Organizations/{isAll}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> getOrganizations(Boolean isAll)
        {
            APIModel aPIModel = new APIModel();
            String lineId = "";
            String action = "UserAdminController.getOrganizations";
            try
            {
                List<OrganizationModelView> listAll = new List<OrganizationModelView>();
                if (DataCache.organList == null)
                {
                    HttpClient httpClient = _clientFactory.CreateClient();
                    String API  = Environment.GetEnvironmentVariable("ORIGIN");
                    var response = await httpClient.GetAsync("https://mis-api.cmu.ac.th/hr/v2.1/reference/organizations");
                    String responseString = await response.Content.ReadAsStringAsync();
                    APIModel dataTemp = JsonConvert.DeserializeObject<APIModel>(responseString);
                    String datajSON = JsonConvert.SerializeObject(dataTemp.data);
                    List<OrganizationModelView> list = JsonConvert.DeserializeObject<List<OrganizationModelView>>(datajSON);
                    DataCache.organList = list;
                }

                if (isAll)
                {

                    OrganizationModelView organizationModelView = new OrganizationModelView();
                    organizationModelView.organizationID = "0000000000";
                    organizationModelView.organizationNameTha = "ทั้งหมด";
                    listAll.Add(organizationModelView);

                    listAll = listAll.Union(DataCache.organList).ToList();
                }
                else
                {
                    listAll = DataCache.organList;
                }

                APIModel model = new APIModel();
                model.data = listAll;
                model.title = "Success";
                return Ok(model);
            }
            catch (Exception ex) { return StatusErrorITSC("line", lineId, "", action, ex); }
        }


        [HttpPost("v1/AdminSearch")]
        [ProducesResponseType(typeof(List<AdminModelView>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIModel), (int)HttpStatusCode.ServiceUnavailable)]
        public async Task<IActionResult> AdminSearch([FromBody] AdminModelView data)
        {
            String Cmuaccount = "";
            String action = "UserAdminController.SuperAdminSearch";
            try
            {
                String json = "";
                Cmuaccount = await this.getCmuaccount();
                if (Cmuaccount == "unauthorized") { return Unauthorized(); }
                UserAdminEntity userAdminEntity = _evoteContext.UserAdminEntitys.Where(w => w.Cmuaccount == Cmuaccount && (w.SuperAdmin == true || w.OrganAdmin == true)).FirstOrDefault();
                if (userAdminEntity == null)
                {
                    return Unauthorized();
                }
                if (userAdminEntity.OrganAdmin)
                {
                    data.Organization_Code = userAdminEntity.Organization_Code;
                    // data.OrganAdmin = userAdminEntity.OrganAdmin;
                }
                List<AdminModelView> adminModelViews = await _userAdminRepository.searchAdmin(data, Cmuaccount);
                APIModel aPIModel = new APIModel();
                aPIModel.data = adminModelViews;
                aPIModel.title = "Success";
                return StatusCodeITSC("CMU", "", Cmuaccount, action, 200, aPIModel);

            }
            catch (Exception ex) { return StatusErrorITSC("CMU", "", Cmuaccount, action, ex); }
        }

        [HttpPost("v1/Admin")]
        [ProducesResponseType(typeof(List<AdminModelView>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIModel), (int)HttpStatusCode.ServiceUnavailable)]
        public async Task<IActionResult> addAdmin([FromBody] AdminModelView data)
        {
            String Cmuaccount = "";
            String action = "UserAdminController.addAdmin";
            try
            {
                APIModel aPIModel = new APIModel();
                String json = "";
                Cmuaccount = await this.getCmuaccount();
                if (Cmuaccount == "unauthorized") { return Unauthorized(); }
                UserAdminEntity userAdminEntity = _evoteContext.UserAdminEntitys.Where(w => w.Cmuaccount == Cmuaccount && (w.SuperAdmin == true || w.OrganAdmin == true)).FirstOrDefault();
                if (userAdminEntity == null)
                {
                    return Unauthorized();
                }
                if (userAdminEntity.OrganAdmin)
                {
                    data.Organization_Code = userAdminEntity.Organization_Code;
                    data.OrganizationFullNameTha = userAdminEntity.OrganizationFullNameTha;
                }
                UserAdminEntity userAdminEntityTemp = _evoteContext.UserAdminEntitys.Where(w => w.Cmuaccount == data.Cmuaccount).FirstOrDefault();
                if (userAdminEntityTemp != null)
                {
                    aPIModel.data = null;
                    aPIModel.title = "ไม่สามารถบันทึกได้   " + data.FullName + "   ได้ถูกกำหนดเป็น Admin แล้ว";
                    return StatusCodeITSC("CMU", "", Cmuaccount, action, 400, aPIModel);
                }

                List<AdminModelView> adminModelViews = null;
                if (await _userAdminRepository.addAdmin(data, Cmuaccount, getClientIP()))
                {
                    if (userAdminEntity.OrganAdmin)
                    {
                        data.Organization_Code = userAdminEntity.Organization_Code;
                    }
                    else
                    {
                        data.Organization_Code = "0000000000";
                    }
                    adminModelViews = await _userAdminRepository.searchAdmin(data, Cmuaccount);
                }
                aPIModel.data = adminModelViews;
                aPIModel.title = "Success";
                return StatusCodeITSC("CMU", "", Cmuaccount, action, 200, aPIModel);

            }
            catch (Exception ex) { return StatusErrorITSC("CMU", "", Cmuaccount, action, ex); }
        }

        [HttpDelete("v1/Admin")]
        [ProducesResponseType(typeof(List<AdminModelView>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIModel), (int)HttpStatusCode.ServiceUnavailable)]
        public async Task<IActionResult> DeleteAdmin([FromQuery] int UserAdminEntityId)
        {
            String Cmuaccount = "";
            String action = "UserAdminController.DeleteAdmin";
            try
            {
                APIModel aPIModel = new APIModel();
                String json = "";
                Cmuaccount = await this.getCmuaccount();
                if (Cmuaccount == "unauthorized") { return Unauthorized(); }
                UserAdminEntity userAdminEntity = _evoteContext.UserAdminEntitys.Where(w => w.Cmuaccount == Cmuaccount && (w.SuperAdmin == true || w.OrganAdmin == true)).FirstOrDefault();
                if (userAdminEntity == null)
                {
                    return Unauthorized();
                }
                UserAdminEntity userAdminEntityTemp = _evoteContext.UserAdminEntitys.Where(w => w.UserAdminEntityId == UserAdminEntityId).FirstOrDefault();
                if (userAdminEntityTemp == null)
                {
                    return BadRequest();
                }
                if (userAdminEntity.OrganAdmin)
                {
                    if (userAdminEntity.Organization_Code != userAdminEntityTemp.Organization_Code)
                    {
                        return Unauthorized();
                    }
                }
                List<AdminModelView> adminModelViews = null;
                AdminModelView data = new AdminModelView();
                if (await _userAdminRepository.deleteAdmin(UserAdminEntityId))
                {
                    if (userAdminEntity.OrganAdmin)
                    {
                        data.Organization_Code = userAdminEntity.Organization_Code;
                    }
                    else
                    {
                        data.Organization_Code = "0000000000";
                    }
                    adminModelViews = await _userAdminRepository.searchAdmin(data, Cmuaccount);
                }
                aPIModel.data = adminModelViews;
                aPIModel.title = "Success";
                return StatusCodeITSC("CMU", "", Cmuaccount, action, 200, aPIModel);

            }
            catch (Exception ex) { return StatusErrorITSC("CMU", "", Cmuaccount, action, ex); }
        }

        [HttpPost("v1/UserAdmin")]
        [ProducesResponseType(typeof(List<UserModelDataView>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> getUserAdmin([FromBody] UserModelSearch data)
        {
            String Cmuaccount = "";
            String action = "UserAdminController.getUserAdmin";
            try
            {
                APIModel aPIModel = new APIModel();
                String json = "";
                Cmuaccount = await this.getCmuaccount();
                if (Cmuaccount == "unauthorized") { return Unauthorized(); }
                UserAdminEntity userAdminEntity = _evoteContext.UserAdminEntitys.Where(w => w.Cmuaccount == Cmuaccount && (w.SuperAdmin == true || w.OrganAdmin == true)).FirstOrDefault();
                if (userAdminEntity == null)
                {
                    return Unauthorized();
                }
                if (userAdminEntity.OrganAdmin)
                {
                    data.OrganizationID = userAdminEntity.Organization_Code;
                }
                List<UserModelDataView> userModelDataViews = new List<UserModelDataView>();
                try
                {
                    APIModel aPIModeltemp = await this.HrSearchUser(getTokenFormHeader(), data);
                    String datajSON = JsonConvert.SerializeObject(aPIModeltemp.data);
                    List<EmployeeResultSearchModel> list = JsonConvert.DeserializeObject<List<EmployeeResultSearchModel>>(datajSON);
                    foreach (EmployeeResultSearchModel employeeResultSearchModel in list)
                    {
                        UserModelDataView userModelDataView = new UserModelDataView();

                        userModelDataView.Email = employeeResultSearchModel.EmailCMU;
                        userModelDataView.OrganizationFullNameTha = employeeResultSearchModel.Faculty;
                        userModelDataView.Organization_Code = data.OrganizationID;
                        userModelDataView.FullName = employeeResultSearchModel.FirstNameTha + " " + employeeResultSearchModel.LastNameTha;
                        userModelDataViews.Add(userModelDataView);
                    }
                }
                catch
                { }
                aPIModel.data = userModelDataViews;
                aPIModel.title = "Success";
                return StatusCodeITSC("CMU", "", Cmuaccount, action, 200, aPIModel);
            }
            catch (Exception ex) { return StatusErrorITSC("CMU", "", Cmuaccount, action, ex); }
        }
    }
}
