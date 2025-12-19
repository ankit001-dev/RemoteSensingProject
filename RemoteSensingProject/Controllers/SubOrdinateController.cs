using RemoteSensingProject.Models.Admin;
using static RemoteSensingProject.Models.SubOrdinate.main;
using RemoteSensingProject.Models.SubOrdinate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.IO;
using RemoteSensingProject.Models.ProjectManager;

namespace RemoteSensingProject.Controllers
{
    [Authorize(Roles = "subOrdinate")]
    public class SubOrdinateController : Controller
    {
        private readonly SubOrinateService _subOrdinate;
        private readonly AdminServices _adminServices;
        private readonly ManagerService _managerServices;
        public SubOrdinateController()
        {
            _subOrdinate =new  SubOrinateService();
            _adminServices = new AdminServices();
            _managerServices = new ManagerService();

        }
        // GET: SubOrdinate
        public ActionResult Dashboard()
        {
            var managerName = User.Identity.Name;
            int userId =Convert.ToInt32(_managerServices.getManagerDetails(managerName).userId);
            var dcount = _subOrdinate.GetDashboardCounts(Convert.ToInt32(userId));
            List<Models.SubOrdinate.main.ProjectList> _list = new List<Models.SubOrdinate.main.ProjectList>();
            ViewData["AssignedProjectList"] = _managerServices.All_Project_List(0, null, null, null, id: userId);
            return View(dcount);
        }
        #region Assigned Project
        public ActionResult Assigned_Project(string searchTerm = null, string statusFilter = null,string filterType= null)
        {
            var managerName = User.Identity.Name;
            int userId = Convert.ToInt32(_managerServices.getManagerDetails(managerName).userId);

            List<Models.SubOrdinate.main.ProjectList> _list = new List<Models.SubOrdinate.main.ProjectList>();
            var data = _managerServices.All_Project_List(
                0, null, null, "SubordinateProject",
                id: userId,
                searchTerm: searchTerm,
                statusFilter: statusFilter);

            if (!string.IsNullOrEmpty(filterType))
            {
                data = data
                    .Where(d => d.ProjectType.ToLower() == filterType.ToLower())
                    .ToList();
            }

            ViewData["AssignedProjectList"] = data;
            return View();
        }
        public ActionResult GetProjecDatatById(int Id)
        {
            var data = _adminServices.GetProjectById(Id);
            return Json(new
            {
                status = true,
                data = data
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion End Assigned
        #region Raise Problem
        public ActionResult AddRaiseProblem(Raise_Problem formData)
        {
            string path = null;
            if(formData.Attachment !=null && formData.Attachment.ContentLength > 0)
            {
                string filePage = Server.MapPath("~/ProjectContent/SubOrdinate/ProblemDocs");
                if (!Directory.Exists(filePage))
                    Directory.CreateDirectory(filePage);
                var guid = Guid.NewGuid();
                var FileExtension = Path.GetExtension(formData.Attachment.FileName);
                var fileName = $"{guid}{FileExtension}";
                path = Path.Combine("/ProjectContent/SubOrdinate/ProblemDocs", fileName);

                formData.Attchment_Url = path;
            }
            bool status = _subOrdinate.InsertSubOrdinateProblem(formData);
            if (status)
            {
                formData.Attachment.SaveAs(Server.MapPath(path));
            }
           
            return Json(new { status=status},JsonRequestBehavior.AllowGet);
        }

        #endregion End Problem
        public ActionResult Meeting_List(string searchTerm = null, string statusFilter = null)
        {
            var userId = _managerServices.getManagerDetails(User.Identity.Name);
            var res = _subOrdinate.getAllSubordinatemeeting(int.Parse(userId.userId),searchTerm:searchTerm,statusFilter:statusFilter);
            return View(res);
        }
        public ActionResult GetConclusions(int id)
        {
            var userId = _managerServices.getManagerDetails(User.Identity.Name);
            var res = _managerServices.getConclusionForMeeting(id, int.Parse(userId.userId));
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getPresentMember(int id)
        {
            var res = _adminServices.getPresentMember(id);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getKeypointResponse(int id)
        {
            var res = _adminServices.getKeypointResponse(id);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetMemberResponse(getMemberResponse mr)
        {
            var userId = _managerServices.getManagerDetails(User.Identity.Name);
            mr.MemberId = int.Parse(userId.userId);
            var res = _managerServices.GetResponseFromMember(mr);
            return Json(res);
        }

        public ActionResult RejectList()
        {
            return View();
        }
        public ActionResult FundReport()
        {
            return View();
        }
    }
}