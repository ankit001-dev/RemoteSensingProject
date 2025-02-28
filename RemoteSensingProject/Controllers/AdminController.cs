using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RemoteSensingProject.Models.Admin;
using static RemoteSensingProject.Models.Admin.main;
using  RemoteSensingProject.Models.ProjectManager;


namespace RemoteSensingProject.Controllers
{
    [Authorize(Roles ="admin")]
    public class AdminController : Controller
    {
        private readonly AdminServices _adminServices;
        private readonly ManagerService _managerServices;

        public AdminController()
        {
            _adminServices = new AdminServices();  
            _managerServices = new ManagerService();

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
            ViewBag.totalBudget = TotalCount.TotalBudget;
            ViewBag.pendingBudget = TotalCount.PendingBudget;
            ViewBag.expenditure = TotalCount.expenditure;

            return View();
        }
        public ActionResult BindOverallCompletionPercentage()
        {
            List<main.DashboardCount> list = new List<main.DashboardCount>();
            list = _adminServices.getAllProjectCompletion();
            return Json(new { list = list},JsonRequestBehavior.AllowGet);

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
            var TotalCount = _adminServices.DashboardCount();
            ViewBag.pendingBudget = TotalCount.PendingBudget;
            decimal b = Convert.ToDecimal(ViewBag.pendingBudget);
            var data = _adminServices.SelectEmployeeRecord();
            ViewBag.projectManager = data.Where(d => d.EmployeeRole.Equals("projectManager")).ToList();
            ViewBag.subOrdinateList = data.Where(d => d.EmployeeRole.Equals("subOrdinate")).ToList();

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
        public ActionResult InsertProject(createProjectModel pm)
        {
            if(pm.pm.projectDocument != null && pm.pm.projectDocument.FileName != "")
            {
                pm.pm.projectDocumentUrl = DateTime.Now.ToString("ddMMyyyy") + Guid.NewGuid().ToString() + Path.GetExtension(pm.pm.projectDocument.FileName);
                pm.pm.projectDocumentUrl = Path.Combine("/ProjectContent/Admin/ProjectDocs/", pm.pm.projectDocumentUrl);
            }

            if(pm.stages != null && pm.stages.Count > 0 && pm.pm.ProjectStage)
            {
                foreach(var item in pm.stages)
                {
                    if(item.Stage_Document != null && item.Stage_Document.FileName != "")
                    {
                        item.Document_Url = DateTime.Now.ToString("ddMMyyyy") + Guid.NewGuid().ToString() + Path.GetExtension(item.Stage_Document.FileName);
                        item.Document_Url = Path.Combine("/ProjectContent/Admin/ProjectDocs/", item.Document_Url);
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

                if (pm.stages != null && pm.stages.Count > 0 && pm.pm.ProjectStage)
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
            ViewBag.ManagerList = _adminServices.SelectEmployeeRecord().Where(d => d.EmployeeRole.Equals("projectManager")).ToList();
            ViewBag.ProjectList = _adminServices.Project_List();
            return View();
        }
        public ActionResult Project_Request()
        {
            return View();
        }
        #region /* Meeting */
        public ActionResult Min_Of_Meeting()
        {
          
            var empList = _adminServices.BindEmployee();
            return View(empList);
        }

        public ActionResult GetConclusions(int id)
        {
            var res = _adminServices.getConclusion(id);
            return Json(res, JsonRequestBehavior.AllowGet);
        }   public ActionResult GetConclusionOnly(int id,int meetingId)
        {
            var res = _adminServices.getConclusion(meetingId).Where(e=>e.Id==id).Select(e=>e.Conclusion).FirstOrDefault();
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddMeeting(AddMeeting_Model formData)
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
            if (status && formData.Attachment != null)
            {
                formData.Attachment.SaveAs(Server.MapPath(path));
            }
            return Json(new { success = status }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UpdateMeeting(AddMeeting_Model formData)
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
            bool status = _adminServices.UpdateMeeting(formData);
            if (status && formData.Attachment!=null)
            {
                formData.Attachment.SaveAs(Server.MapPath(path));
            }
            return Json(new { success = status }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetMeetingById(int id)
        {
           var obj = _adminServices.getMeetingById(id);
            return Json(obj,JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public ActionResult GetMemberJoiningStatusByMeeting(int meetingId)
        {

            var res = _managerServices.getMemberJoiningStatus(meetingId);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetAllMeeting()
        {
            List<Meeting_Model> empList = new List<Meeting_Model>();
            empList = _adminServices.getAllmeeting().Where(e=>e.CreaterId==0).ToList();
            return Json(new { empList = empList },JsonRequestBehavior.AllowGet);
        }

        #endregion End Meeting

        public ActionResult MeetingConclusion(int meeting)
        {
            ViewBag.empList = _adminServices.BindEmployee();
            var obj = _adminServices.getMeetingById(meeting);
            ViewBag.getMember = _adminServices.GetMeetingMemberList(meeting);
            ViewBag.MeetingConclusion = _adminServices.getConclusion(meeting);
            return View(obj);
        }
        [HttpPost]
        public ActionResult AddMeetingConclusion(MeetingConclusion mc)
        {
            var res = _adminServices.AddMeetingResponse(mc);
            return Json(res);
        }
        public ActionResult getPresentMember(int id)
        {
            var res = _adminServices.getPresentMember(id);
            return Json(res,JsonRequestBehavior.AllowGet);
        }
        public ActionResult getKeypointResponse(int id)
        {
            var res = _adminServices.getKeypointResponse(id);
            return Json(res,JsonRequestBehavior.AllowGet);
        }
        public ActionResult Project_Detail()
        {
            return View();
        }
        public ActionResult View_Weekly_Update(int Id)
        {
            ManagerService ms = new ManagerService();
            ViewData["weeklyUpdate"] = ms.WeeklyUpdateList(Id);
            return View();
        }

        public ActionResult View_Project_Stage(int Id)
        {
            ViewBag.ProjectStages = _adminServices.ProjectStagesList(Id);
            return View();
        }
        public ActionResult viewStagesReason(string stageId)
        {
            List<Project_Statge> stageList = new List<Project_Statge>();
            stageList = _managerServices.ViewStagesComments(stageId);
            return Json(new { stageList = stageList }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult View_Expenses(int Id)
        {
            ViewBag.ProjectExpenses = _adminServices.ProjectBudgetList(Id);
            return View();
        }
        #region reports
        public ActionResult Project_Report()
        {
            return View();
        }

        public ActionResult Meeting_Report()
        {
            var empList = _adminServices.BindEmployee();

            return View(empList);
        }

        public ActionResult Member_Report()
        {
            ViewBag.MemberList = _adminServices.SelectEmployeeRecord();
            ViewBag.DivisonList = _adminServices.ListDivison();
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
        #endregion
        public ActionResult Generate_Notice()
        {
            ViewBag.ProjectList = _adminServices.Project_List();

            return View();
        }
        [HttpGet]
        public ActionResult GetProjectManagerByProjectId(int? id)
        {
            dynamic projectManager = null;
            if (id.HasValue)
            {

             projectManager = _adminServices.Project_List().Where(e => e.Id == id).FirstOrDefault();
            }
            
            return Json(projectManager, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public ActionResult GetProjectById(int? id)
        {
            dynamic project = null;
            if (id.HasValue)
            {

             project = _adminServices.GetProjectById((int)id);

            }

            return Json(project, JsonRequestBehavior.AllowGet);

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
            if (res && gn.Attachment!=null)
            {
                gn.Attachment.SaveAs(Server.MapPath(path));
            }
            return Json(res);
        }

        public ActionResult Notice_List(int? projectId)
        {
            dynamic noticeList = null;
            if (projectId.HasValue)
            {
                noticeList= _adminServices.getNoticeList().Where(e=>e.ProjectId== projectId).ToList();
            }
            else
            {
                noticeList = _adminServices.getNoticeList();

            }
            ViewBag.ProjectList = _adminServices.Project_List();

            ViewData["NoticeList"] = noticeList;

            return View();
        }

        public ActionResult View_Notice()
        {
            return View();
        }

        public ActionResult Expense_Report()
        {
            ViewBag.ProjectList = _adminServices.Project_List();
            return View();
        }
        public ActionResult getHeadsByProject(int id)
        {
            List<Project_model> _headList = new List<Project_model>();
            _headList = _adminServices.getHeadByProject(id);
            return Json(new { _headList = _headList },JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetExpenses(int hId,int pId )
        {
            List<ProjectExpenses> list = new List<ProjectExpenses>();
             list = _managerServices.ExpencesList(hId,pId );
            return Json(new { list = list }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SubOrdinateProblemList()
        {
            ViewBag.ProjectProblemList = _managerServices.getSubOrdinateProblemforAdmin();
            return View();
        }


        public ActionResult ReimbursementRequest()
        {
            ViewData["ReimBurseData"] = _managerServices.GetReimbursements();
            return View();
        }
        [HttpPost]
        public ActionResult ReimbursementReque(bool status,int id)
        {
            bool res = _adminServices.ReimbursementApproval(id, status);
            if (res)
            {
                return Json(new
                {
                    status = res,
                    message = "Approved"
                });
            }
            else
            {
                return Json(new
                {
                    status = res,
                    message = "Rejected"
                });
            }
        }
        public ActionResult TravelRequest()
        {
            var res = _adminServices.getAllTourList();
            ViewData["allTourList"] = res;
            return View();
        }

        [HttpPost]
        public ActionResult TravelRequest(bool status,int id)
        {
            bool res = _adminServices.Tourapproval(id, status);
            if (res)
            {
                return Json(new
                {
                    status = res,
                    message = "Approved"
                });
            }
            else
            {
                return Json(new
                {
                    status = res,
                    message = "Rejected"
                });
            }
        }

        //public ActionResult AddBudget()
        //{
        //    ViewData["budgetList"] = _adminServices.getBudgetList();
        //    return View();
        //}
        //[HttpPost]
        //public ActionResult AddBudget(BudgetForm data)
        //{
        //    var res = _adminServices.InsertBuget(data);
        //    if (res)
        //    {
        //        return Json(new
        //        {
        //            status = res,
        //            message = "Added Successfully"
        //        });
        //    }
        //    else
        //    {
        //        return Json(new
        //        {
        //            status = res,
        //            message = "Some error occured"
        //        });
        //    }
        //}
    }
}