using RemoteSensingProject.Models.Admin;
using static RemoteSensingProject.Models.SubOrdinate.main;
using RemoteSensingProject.Models.SubOrdinate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace RemoteSensingProject.Controllers
{
    [Authorize(Roles = "subOrdinate")]
    public class SubOrdinateController : Controller
    {
        private readonly SubOrinateService _subOrdinate;
        private readonly AdminServices _adminServices;
        public SubOrdinateController()
        {
            _subOrdinate =new  SubOrinateService();
            _adminServices = new AdminServices();
            

        }
        // GET: SubOrdinate
        public ActionResult Dashboard()
        {
            return View();
        }
        #region Assigned Project
        public ActionResult Assigned_Project()
        {
            var managerName = User.Identity.Name;
            UserCredential userObj = new UserCredential();
            userObj = _subOrdinate.getManagerDetails(managerName);

            List<ProjectList> _list = new List<ProjectList>();
            ViewData["AssignedProjectList"] = _subOrdinate.getProjectBySubOrdinate(userObj.userId);

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
        public ActionResult Meeting_List()
        {
            return View();
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