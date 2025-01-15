using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RemoteSensingProject.Models.Admin;
using static RemoteSensingProject.Models.Admin.main;

namespace RemoteSensingProject.Controllers
{
    [Authorize(Roles ="admin")]
    public class AdminController : Controller
    {
        private readonly AdminServices _adminServices;
        public AdminController()
        {
            _adminServices = new AdminServices();   
        }
        // GET: Admin
        public ActionResult Dashboard()
        {
            var TotalCount = _adminServices.DashboardCount();
            ViewBag.totalEmployee = TotalCount.TotalEmployee;
            ViewBag.totalProject = TotalCount.TotalProject;
            ViewBag.totalDelayProject = TotalCount.TotalDelayproject;
            ViewBag.totalCompleteProject = TotalCount.TotalCompleteProject;
            ViewBag.totalOngoingProject = TotalCount.TotalOngoingProject;
            ViewBag.totalMeetings = TotalCount.TotalMeetings;

            return View();
        }

        #region Employee Category
        public ActionResult Emp_Category()
        {
            return View();
        }

        public ActionResult getCategoryPartial(string partial)
        {
            List<CommonResponse> list = new List<CommonResponse>();
            if (partial.Equals("_desgination"))
            {
                list = _adminServices.ListDesgination();
            }
            else if (partial.Equals("_divison"))
            {
                list = _adminServices.ListDivison();
            }
            return PartialView(partial, list);
        }

        [HttpPost]
        public ActionResult InsertDesgination(CommonResponse cr)
        {
            bool res = _adminServices.InsertDesgination(cr);
            return Json(new
            {
                status = res,
                message = res ? "Desgination inserted successfully!"  : "Some issue found while processing your request !"
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult InsertDivision(CommonResponse cr)
        {
            bool res = _adminServices.InsertDivison(cr);
            return Json(new
            {
                status = res,
                message = res ? "Divison inserted successfully!"  : "Some issue found while processing your request !"
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpDelete]
        public ActionResult removeDivison(int id)
        {
            bool res = _adminServices.removeDivison(id);
            return Json(new
            {
                status = res,
                message = res ? "Divison removed successfully !" : "Some issue occred "
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpDelete]
        public ActionResult removeDesgination(int id)
        {
            bool res = _adminServices.removeDesgination(id);
            return Json(new
            {
                status = res,
                message = res ? "Divison removed successfully !" : "Some issue occred "
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        public ActionResult Employee_Registration(int? division)
        {

            ViewBag.division = _adminServices.ListDivison();
            ViewBag.designation = _adminServices.ListDesgination();
            List<Employee_model> empList= new List<Employee_model>();
            if (division!=null && division!=0)
            {
                empList = _adminServices.SelectEmployeeRecord().Where(e=>e.Division==division).ToList<Employee_model>();

            }
            else
            {
                empList = _adminServices.SelectEmployeeRecord();
            }
            ViewData["EmployeeList"] = empList;
            return View();
        }

        [HttpPost]
        public ActionResult Employee_Registration(Employee_model emp)
        {
            string path = null;
            if (emp.EmployeeImages != null)
            {
                var fileName = Guid.NewGuid() + DateTime.Now.ToString("ddMMyyyyhhmm") + emp.EmployeeImages.FileName;
                  path = Path.Combine("/ProjectContent/Admin/Employee_Images", fileName);
                emp.Image_url = path;
            }


            bool res = _adminServices.AddEmployees(emp);

            
            if (res && emp.EmployeeImages!=null)
            {
                
                emp.EmployeeImages.SaveAs(Server.MapPath(path));

            }
            return Json(res);
            
        }
        [HttpGet]
        public ActionResult DeleteEmployees(int id)
        {
            var res = _adminServices.RemoveEmployees(id);
           
                return Json(res,JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult ChangeActieStatus(int id)
        {
            var res = _adminServices.ChangeActieStatus(id);
           
                return Json(res,JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult SelectEmployeeRecordById(int id)
        {
            var res = _adminServices.SelectEmployeeRecordById(id);
           
                return Json(res,JsonRequestBehavior.AllowGet);
        }

        #region add project
        public ActionResult Add_Project()
        {
            var data = _adminServices.SelectEmployeeRecord();
            ViewBag.projectManager = data.Where(d => d.EmployeeRole.Equals("projectManager")).ToList();
            ViewBag.subOrdinateList = data.Where(d => d.EmployeeRole.Equals("subOrdinate")).ToList();
            return View();
        }

        public ActionResult GetProjectById(int Id)
        {
            var data = _adminServices.GetProjectById(Id);
            return Json(new
            {
                status = true,
                data = data
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult InsertProject(createProjectModel pm)
        {
            if(pm.pm.projectDocument != null && pm.pm.projectDocument.FileName != "")
            {
                pm.pm.projectDocumentUrl = DateTime.Now.ToString("ddMMyyyy") + Guid.NewGuid().ToString() + Path.GetExtension(pm.pm.projectDocument.FileName);
                pm.pm.projectDocumentUrl = "/ProjectContent/Admin/ProjectDocs/" + pm.pm.projectDocumentUrl;
            }

            if(pm.stages != null && pm.stages.Count > 0 && pm.pm.ProjectStage.Equals("Yes"))
            {
                foreach(var item in pm.stages)
                {
                    if(item.Stage_Document != null && item.Stage_Document.FileName != "")
                    {
                        item.Document_Url = DateTime.Now.ToString("ddMMyyyy") + Guid.NewGuid().ToString() + Path.GetExtension(item.Stage_Document.FileName);
                        item.Document_Url = "/ProjectContent/Admin/ProjectDocs/" + item.Document_Url;
                    }
                }
            }

            bool res = _adminServices.addProject(pm);
            if (res)
            {
                if (pm.pm.projectDocument != null && pm.pm.projectDocument.FileName != "")
                {
                    pm.pm.projectDocument.SaveAs(Server.MapPath(pm.pm.projectDocumentUrl));
                }

                if (pm.stages != null && pm.stages.Count > 0 && pm.pm.ProjectStage.Equals("Yes"))
                {
                    foreach (var item in pm.stages)
                    {
                        if (item.Stage_Document != null && item.Stage_Document.FileName != "")
                        {
                            item.Stage_Document.SaveAs(Server.MapPath(item.Document_Url));
                        }
                    }
                }
            }
            return Json(new
            {
                status = res
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Project_List()
        {
            ViewBag.ProjectList = _adminServices.Project_List();
            return View();
        }
        #endregion

        public ActionResult All_Projects()
        {
            return View();
        }
        public ActionResult Project_Request()
        {
            return View();
        }
        #region /* Meeting */
        public ActionResult Min_Of_Meeting(string Id="")
        {
            List<Employee_model> empList = new List<Employee_model>();
            empList = _adminServices.BindEmployee();
            ViewBag.Employee = empList;
            Meeting_Model obj = new Meeting_Model();
            if(!string.IsNullOrEmpty(Id))
            {
                obj = _adminServices.getMeetingById(Id);
            }
            return View(obj);
        }
        [HttpPost]
        public ActionResult AddMeeting(Meeting_Model formData)
        {
            string path = null;
            if (formData.Attachment != null && formData.Attachment.ContentLength > 0)
            {
                var guid = Guid.NewGuid();
                var FileExtension = Path.GetExtension(formData.Attachment.FileName);
                var fileName = $"{guid}{FileExtension}";
                 path = Path.Combine("/ProjectContent/Admin/Meeting_Attachment", fileName);

                formData.Attachment_Url = path;
            }
            bool status = _adminServices.insertMeeting(formData);
            if (status)
            {
                formData.Attachment.SaveAs(Server.MapPath(path));
            }
            return Json(new { success = status }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetAllMeeting()
        {
            List<Meeting_Model> empList = new List<Meeting_Model>();
            empList = _adminServices.getAllmeeting();
            return Json(new { empList = empList },JsonRequestBehavior.AllowGet);
        }

        #endregion End Meeting

        public ActionResult MeetingConclusion()
        {
            return View();
        }
        public ActionResult Project_Detail()
        {
            return View();
        }
        public ActionResult View_Weekly_Update()
        {
            return View();
        }

        public ActionResult View_Project_Stage()
        {
            return View();
        }

        public ActionResult View_Expenses()
        {
            return View();
        }

        public ActionResult Project_Report()
        {
            return View();
        }

        public ActionResult Meeting_Report()
        {
            return View();
        }

        public ActionResult Member_Report()
        {
            return View();
        }

        public ActionResult Pending_Project()
        {
            return View();
        }

        public ActionResult Completed_Project()
        {
            return View();
        }
        public ActionResult Generate_Notice()
        {
            ViewBag.ProjectList = _adminServices.Project_List();

            return View();
        }
        public ActionResult GetProjectManagerByProjectId(int? id)
        {
            dynamic projectManager = null;
            if (id.HasValue)
            {

             projectManager = _adminServices.Project_List().Where(e => e.Id == id).FirstOrDefault();
            }
            
            return Json(projectManager, JsonRequestBehavior.AllowGet);

        }

        public ActionResult AddNotice(Generate_Notice gn)
        {
            string path = null;
            if (gn.Attachment != null)
            {
                var fileName = Guid.NewGuid() + DateTime.Now.ToString("ddMMyyyyhhmm") + gn.Attachment.FileName;
                path = Path.Combine("/ProjectContent/Admin/NoticeDocs", fileName);
                gn.Attachment_Url = path;
            }

            var res = _adminServices.InsertNotice(gn);
            if (res)
            {
                gn.Attachment.SaveAs(Server.MapPath(path));
            }
            return Json(res);
        }

        public ActionResult Notice_List()
        {
            ViewData["NoticeList"] = _adminServices.getNoticeList();

            return View();
        }

        public ActionResult View_Notice()
        {
            return View();
        }

        public ActionResult Expense_Report()
        {
            return View();
        }

    }
}