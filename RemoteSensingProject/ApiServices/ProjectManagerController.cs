using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Spreadsheet;
using Grpc.Core;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Routing;
using RemoteSensingProject.Models;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.LoginManager;
using RemoteSensingProject.Models.ProjectManager;
using static RemoteSensingProject.Models.Admin.main;

namespace RemoteSensingProject.ApiServices
{
    [JwtAuthorize]
    public class ProjectManagerController : ApiController
    {
        private readonly AdminServices _adminServices;
        private readonly LoginServices _loginService;
        private readonly ManagerService _managerService;
        public ProjectManagerController()
        {
            _adminServices = new AdminServices();
            _loginService = new LoginServices();
            _managerService = new ManagerService();
        }


        #region Project substances
        [HttpGet]
        [Route("api/getProjectExpencesList")]
        public IHttpActionResult GetExpencesList(int projectId, int headId)
        {
            try
            {
                var data = _managerService.ExpencesList(headId, projectId);
                if (!data.Any())
                {
                    return Ok(new
                    {
                        status = false,
                        StatusCode = 500,
                        message = "Data not found !"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = true,
                        StatusCode = 200,
                        data = data,
                        message = "Data found!"
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        [Route("api/addProjectExpenses")]
        public IHttpActionResult AddExpenses()
        {
            try
            {
                var request = HttpContext.Current.Request;
                List<string> validationErrors = new List<string>();
                if (string.IsNullOrWhiteSpace(request.Form.Get("projectId")))
                    validationErrors.Add("Project Id is required.");


                if (string.IsNullOrWhiteSpace(request.Form.Get("projectHeadId")))
                    validationErrors.Add("Project heads Id is required.");

                if (string.IsNullOrWhiteSpace(request.Form.Get("title")))
                    validationErrors.Add("Title is required.");

                if (string.IsNullOrWhiteSpace(request.Form.Get("date")))
                    validationErrors.Add("date is required.");

                if (string.IsNullOrWhiteSpace(request.Form.Get("amount")))
                    validationErrors.Add("Amount is required.");

                if (string.IsNullOrWhiteSpace(request.Form.Get("description")))
                    validationErrors.Add("Description is required.");


                var formData = new ProjectExpenses
                {
                    projectId = Convert.ToInt32(request.Form.Get("projectId")),
                    projectHeadId = Convert.ToInt32(request.Form.Get("projectHeadId")),
                    title = request.Form.Get("title"),
                    date = Convert.ToDateTime(request.Form.Get("date")),
                    amount = Convert.ToDecimal(request.Form.Get("amount")),
                    attatchment_url = request.Form.Get("attatchment_url"),
                    description = request.Form.Get("description")
                };

                var file = request.Files["Attatchment_file"];
                if (file != null && file.FileName != "")
                {
                    formData.attatchment_url = DateTime.Now.ToString("ddMMMyyyy") + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    formData.attatchment_url = Path.Combine("/ProjectContent/ProjectManager/HeadsSlip/", formData.attatchment_url);
                }
                if (validationErrors.Any())
                {
                    return BadRequest(new
                    {
                        status = false,
                        StatusCode = 500,
                        message = string.Join("\n", validationErrors)
                    });
                }
                bool res = _managerService.insertExpences(formData);
                if (res)
                {
                    if (file != null && file.FileName != "")
                    {
                        file.SaveAs(HttpContext.Current.Server.MapPath(formData.attatchment_url));
                    }
                }
                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 200 : 500,
                    message = res ? "Expenses added successfully !" : "Some issue occured !"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("api/getFinancialReport")]
        public IHttpActionResult getFinancialReport(int projectId)
        {
            try
            {
                var data = _managerService.GetExtrnlFinancialReport(projectId);
                return Ok(new
                {
                    status = data.Any(),
                    StatusCode = data.Any() ? 200 : 500,
                    message = data.Any() ? "Data Found !" : "Data not found !",
                    data = data
                });
            }catch(Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    message = ex.Message
                });
            }
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("api/getWeeklyUpdate")]
        public IHttpActionResult getWeeklyUpdate(int projectId)
        {
            try
            {
                var data = _managerService.MonthlyProjectUpdate(projectId);
                if (!data.Any())
                {
                    return BadRequest(new
                    {
                        status = false,
                        StatusCode = 500,
                        message = "Data not found !"
                    });
                }
                return Ok(new
                {
                    status = true,
                    StatusCode = 200,
                    message = "data found !",
                    data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }


        [HttpPost]
        [Route("api/insertWeeklyUpdate")]
        public IHttpActionResult insertWeeklyUpdate()
        {
            try
            {
                var request = HttpContext.Current.Request;
                List<string> validationErrors = new List<string>();
                if (string.IsNullOrWhiteSpace(request.Form.Get("ProjectId")))
                    validationErrors.Add("Project Id is required.");


                if (string.IsNullOrWhiteSpace(request.Form.Get("completionPerc")))
                    validationErrors.Add("Project completion in percentage is required.");

                if (string.IsNullOrWhiteSpace(request.Form.Get("comments")))
                    validationErrors.Add("description is required.");

                if (string.IsNullOrWhiteSpace(request.Form.Get("date")))
                    validationErrors.Add("date is required.");
                var formData = new Project_MonthlyUpdate
                {
                    ProjectId = Convert.ToInt32(request.Form.Get("ProjectId")),
                    completionPerc = Convert.ToInt32(request.Form.Get("completionPerc")),
                    comments = request.Form.Get("comments"),
                    date = Convert.ToDateTime(request.Form.Get("date"))
                };
                if (validationErrors.Any())
                {
                    return BadRequest(new
                    {
                        status = false,
                        StatusCode = 500,
                        message = string.Join("\n", validationErrors)
                    });
                }
                bool res = _managerService.UpdateMonthlyStatus(formData);
                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 200 : 500,
                    message = res ? "Weekly Status updated successfuly !" : "Some issue occured !"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }


        [HttpPost]
        [Route("api/UpdateProjectStages")]
        public IHttpActionResult updateProjectStages()
        {
            try
            {
                var request = HttpContext.Current.Request;
                var formData = new Project_Statge
                {
                    Stage_Id = Convert.ToInt32(request.Form.Get("Stage_Id")),
                    Comment = request.Form.Get("Comment"),
                    CompletionPrecentage = request.Form.Get("CompletionPrecentage"),
                    StageDocument_Url = request.Form.Get("StageDocument_Url"),
                    Status = request.Form.Get("Status")

                };

                var file = request.Files["StageDocument"];
                if (file != null && file.FileName != "")
                {
                    formData.StageDocument_Url = DateTime.Now.ToString("ddMMyyyy") + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    formData.StageDocument_Url = Path.Combine("/ProjectContent/ProjectManager/ProjectDocs/", formData.StageDocument_Url);
                }

                bool res = _managerService.InsertStageStatus(formData);
                if (res)
                {
                    if (file != null && file.FileName != "")
                    {
                        file.SaveAs(HttpContext.Current.Server.MapPath(formData.StageDocument_Url));
                    }
                }
                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 200 : 500,
                    message = res ? "updateProjectStages updated successfully !" : "Some issue occred !"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }


        [HttpGet]
        [Route("api/ViewStagesComments")]
        public IHttpActionResult stagesList(int stageId)
        {
            try
            {

                var data = _managerService.ViewStagesComments(stageId.ToString());
                return Ok(new
                {
                    status = data.Any(),
                    StatusCode = data.Any() ? 200 : 500,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }
        #endregion

        #region dashboard
        [HttpGet]
        [Route("api/ManagerDashboard")]
        public IHttpActionResult Dashboard(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return Ok(new
                    {
                        status = false,
                        StatusCode = 500,
                        message = "Invalid userid !"
                    });
                }

                var data = _managerService.DashboardCount(Convert.ToInt32(userId));
                return Ok(new
                {
                    status = true,
                    message = "Data found !",
                    data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }
        #endregion

        #region Assigned PRoject
        [HttpGet]
        [Route("api/managerAssignedProject")]
        public IHttpActionResult AssignedPRoject(int userId, int page, int limit)
        {
            try
            {
                var data = _managerService.All_Project_List(userId, page, limit, "AssignedProject");
                if (data.Any())
                {
                    return Ok(new
                    {
                        status = true,
                        StatusCode = 200,
                        message = "Data found !",
                        data = data
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        status = false,
                        StatusCode = 500,
                        message = "Data not found !"
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }
        #endregion

        #region Project
        [HttpGet]
        [Route("api/getManagerProject")]
        public IHttpActionResult GetProjectList(int userId, int page, int limit)
        {
            try
            {
                var data = _managerService.All_Project_List(userId, limit, page, "ManagerProject");
                if (data.Any())
                {
                    return Ok(new
                    {
                        status = true,
                        StatusCode = 200,
                        message = "Data found !",
                        data = data
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = false,
                        StatusCode = 500,
                        message = "Data not found !"
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }


        [HttpGet]
        [Route("api/getManagerCompleteProject")]
        public IHttpActionResult GetCompleteProject(int userId, int page, int limit)
        {
            try
            {
                var data = _managerService.All_Project_List(userId, limit, page, "Complete");
                return Ok(new
                {
                    status = data.Any(),
                    StatusCode = data.Any() ? 200 : 500,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("api/getmanagerdelayProject")]
        public IHttpActionResult getmanagerDelay(int userId, int? limit, int? page)
        {
            try
            {
                var data = _managerService.All_Project_List(userId, limit, page, "Delay");
                return Ok(new
                {
                    status = data.Any(),
                    StatusCode = data.Any() ? 200 : 500,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }
        [HttpGet]
        [Route("api/getmanagerOngoingProject")]
        public IHttpActionResult onGoingProject(int userId, int? limit, int? page)
        {
            try
            {
                var data = _managerService.All_Project_List(userId, limit, page, "Ongoing");
                return Ok(new
                {
                    status = data.Any(),
                    StatusCode = data.Any() ? 200 : 500,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("api/getAllProjectList")]
        public IHttpActionResult getAllProjectList(int userId, int? limit, int? page)
        {
            try
            {
                var data = _managerService.All_Project_List(userId, limit, page, null);
                return Ok(new
                {
                    status = data.Any(),
                    StatusCode = data.Any() ? 200 : 500,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }
        #endregion

        #region Notice
        [HttpGet]
        [Route("api/getManagerNotice")]
        public IHttpActionResult NoticeList(int managerId)
        {
            try
            {
                var data = _adminServices.getNoticeList().Where(d => d.ProjectManagerId == managerId).ToList();
                if (!data.Any())
                {
                    return BadRequest(new
                    {
                        status = false,
                        StatusCode = 500,
                        message = "Data not found !"
                    });
                }
                return Ok(new
                {
                    status = true,
                    StatusCode = 200,
                    message = "data found !",
                    data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }
        #endregion

        #region Manager Meeting
        [HttpGet]
        [Route("api/managerMeetingList")]
        public IHttpActionResult managerMeeting(int managerId)
        {
            try
            {
                var data = _adminServices.getAllmeeting().Where(d => d.CreaterId == managerId).ToList();
                if (data.Any())
                {
                    return Ok(new
                    {
                        status = data.Any(),
                        StatusCode = 200,
                        message = "Data found",
                        data = data
                    });
                }
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = "Data Not found!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("api/getConclusionForManagerMeeting")]
        public IHttpActionResult getConclusionForMeeting(int meetingId, int userId)
        {
            var res = _managerService.getConclusionForMeeting(meetingId, userId);
            return Ok(new { status = true, message = "data retrieved", data = res });
        }
        [HttpGet]
        [Route("api/getAllmeeting")]
        public IHttpActionResult getAllmeeting(int managerId)
        {
            var res = _managerService.getAllmeeting(managerId);
            return Ok(new { status = true, message = "data retrieved", data = res });
        }
        [HttpPost]
        [Route("api/GetResponseFromMember")]
        public IHttpActionResult GetResponseFromMember()
        {
            var httpRequest = HttpContext.Current.Request;
            getMemberResponse mr = new getMemberResponse
            {
                ApprovedStatus = Convert.ToInt32(httpRequest.Form.Get("approveStatus")),
                reason = httpRequest.Form.Get("reason"),
                MeetingId = Convert.ToInt32(httpRequest.Form.Get("meetingId")),
                MemberId = Convert.ToInt32(httpRequest.Form.Get("memberId"))
            };
            var res = _managerService.GetResponseFromMember(mr);
            if (res)
            {
                return Ok(new { status = true, message = "Response Send Successfully", statusCode = 200 });

            }
            else
            {

                return Ok(new { status = true, message = "something went wrong", statusCode = 500 });
            }
        }
        [HttpGet]
        [Route("api/getProjectStatusForDashboard")]
        public IHttpActionResult getProjectstatus(string userId)
        {
            var res = _adminServices.getAllProjectCompletion().Where(e => e.ProjectManager == userId).ToList();

            return Ok(new { status = true, message = "data retrieved", data = res });
        }
        [HttpGet]
        [Route("api/getProblemListByManager")]
        public IHttpActionResult getProblemListByManager(int userId, int page, int limit)
        {
            var res = _adminServices.getProblemList(page, limit, null, userId);


            return Ok(new { status = true, message = "data retrieved", data = res });
        }


        #endregion


        #region outsource
        [HttpPost]
        [Route("api/CreateOutSource")]
        public IHttpActionResult CreateSource()
        {
            try
            {
                var request = HttpContext.Current.Request;
                var formData = new OuterSource
                {
                    EmpId = Convert.ToInt32(request.Form.Get("EmpId")),
                    EmpName = request.Form.Get("EmpName"),
                    mobileNo = Convert.ToInt64(request.Form.Get("mobileNo")),
                    gender = request.Form.Get("gender"),
                    email = request.Form.Get("email")
                };
                bool res = _managerService.insertOutSource(formData);
                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 200 : 500,
                    message = res ? "Outsource created successfully !" : "Some issue occured"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("api/GetOuterSourceList")]
        public IHttpActionResult GetOuterSourceListById(int userId)
        {
            try
            {
                var data = _managerService.selectAllOutSOurceList(userId);
                return Ok(new
                {
                    status = data.Any(),
                    StatusCode = data.Any() ? 200 : 500,
                    message = data.Any() ? "Data found !" : "Some issue occured !",
                    data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        [Route("api/createTask")]
        public IHttpActionResult createTask()
        {
            try
            {
                var request = HttpContext.Current.Request;
                var formData = new OutSourceTask
                {
                    title = request.Form.Get("title"),
                    description = request.Form.Get("description"),
                    empId = Convert.ToInt32(request.Form.Get("empId"))
                };
                var outSourceList = request.Form["outSourceId"];
                if (outSourceList != null)
                {
                    formData.outSourceId = outSourceList.Split(',').Select(value => int.Parse(value.ToString())).ToArray();
                }
                bool res = _managerService.createTask(formData);
                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 200 : 500,
                    message = res ? "Task created successfully !" : "Some issue occured !"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("api/getTaskList")]
        public IHttpActionResult getTaskList(int empId)
        {
            try
            {
                var data = _managerService.taskList(empId);
                return Ok(new
                {
                    status = data.Any(),
                    StatusCode = data.Any() ? 200 : 500,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }
        [System.Web.Mvc.AllowAnonymous]
        [HttpGet]
        [Route("api/ViewTaskId")]
        public IHttpActionResult ViewTaskList(int taskId)
        {
            try
            {
                var data = _managerService.ViewOutSourceList(taskId);
                return Ok(new
                {
                    status = data.Any(),
                    StatusCode = data.Any() ? 200 : 500,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }

        [HttpPut]
        [Route("api/UpdateTaskStatus")]
        public IHttpActionResult UpdateTAskStatus(int taskId)
        {
            try
            {
                bool res = _managerService.updateTaskStatus(taskId);
                return Ok(new
                {
                    status = res,
                    message = res ? "Task updated successfully !" : "Some issue occured !"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }
        #endregion


        #region Reimbursement
        [HttpPost]
        [Route("api/submitReinbursement")]
        public IHttpActionResult reimbursementSubmit()
        {
            try
            {
                var request = HttpContext.Current.Request;
                var formData = new Reimbursement
                {
                    userId = Convert.ToInt32(request.Form.Get("userId")),
                    type = request.Form.Get("type"),
                    vrNo = request.Form.Get("vrNo"),
                    date = Convert.ToDateTime(request.Form.Get("date")),
                    particulars = request.Form.Get("particulars"),
                    items = request.Form.Get("items"),
                    purpose = request.Form.Get("purpose"),
                    amount = Convert.ToDecimal(request.Form.Get("amount"))
                };
                bool res = _managerService.insertReimbursement(formData);
                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 200 : 500,
                    message = res ? "Added Successfully!" : "Some error Occured"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }


        [HttpPost]
        [Route("api/finalSubmitReinbursement")]
        public IHttpActionResult finalSubmitReinbursement()
        {
            try
            {
                var request = HttpContext.Current.Request;
                string type = request.Form.Get("type");
                int id = Convert.ToInt32(request.Form.Get("id"));
                int userId = Convert.ToInt32(request.Form.Get("UserId"));
                bool res = _managerService.submitReinbursementForm(type, userId, id);
                return Ok(new
                {
                    status = res,
                    StatusCode = res? 200 : 500,
                    message = res ? "Reinbursement submitted successfully !" : "Some issue found while processing your request !"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("api/getReimbursementByUserId")]
        public IHttpActionResult getReimbursement(int userId, int page, int limit)
        {
            try
            {
                var data = _managerService.GetReimbursements(page, limit, null, userId);
                return Ok(new
                {
                    status = data.Any(),
                    data = data,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }


        [HttpGet]
        [Route("api/ViewReinbursementBytype")]
        public IHttpActionResult viewReinbursement(int userId, string type, int id, int page, int limit)
        {
            try
            {
                var data = _managerService.GetReimbursements(page, limit, id, userId, type);
                return Ok(new
                {
                    status = data.Any(),
                    StatuCode = data.Any() ? 200 : 500,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }
        #endregion

        #region tourPurposal
        [HttpPost]
        [Route("api/submitTourProposal")]
        public IHttpActionResult toursubmit()
        {
            try
            {
                var request = HttpContext.Current.Request;
                var formdata = new tourProposal
                {
                    userId = Convert.ToInt32(request.Form.Get("userId")),
                    projectId = Convert.ToInt32(request.Form.Get("projectId")),
                    dateOfDept = Convert.ToDateTime(request.Form.Get("dateOfDept")),
                    place = request.Form.Get("place"),
                    periodFrom = Convert.ToDateTime(request.Form.Get("periodFrom")),
                    periodTo = Convert.ToDateTime(request.Form.Get("periodTo")),
                    returnDate = Convert.ToDateTime(request.Form.Get("returnDate")),
                    purpose = request.Form.Get("purpose")
                };
                bool res = _managerService.insertTour(formdata);
                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 200 : 500,
                    message = res ? "Added successfully!" : "Error Occured"
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }
        [HttpGet]
        [Route("api/GetTourForUserId")]
        public IHttpActionResult gettour(int userId)
        {
            try
            {
                var data = _managerService.getTourList(userId);
                return Ok(new
                {
                    status = data.Any(),
                    data = data
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }
        #endregion

        #region /*Get Hiring Vehicle*/
        [Route("api/addHiringRequest")]
        [HttpPost]
        public IHttpActionResult addHiringRequest()
        {
            try
            {
                var request = HttpContext.Current.Request;
                var formdata = new HiringVehicle
                {
                    headId = Convert.ToInt32(request.Form.Get("headId")),
                    amount = Convert.ToDecimal(request.Form.Get("amount")),
                    userId = Convert.ToInt32(request.Form.Get("userId")),
                    projectId = Convert.ToInt32(request.Form.Get("projectId")),
                    dateFrom = Convert.ToDateTime(request.Form.Get("dateFrom")),
                    dateTo = Convert.ToDateTime(request.Form.Get("dateTo")),
                    proposedPlace = (string)request.Form.Get("proposedPlace"),
                    purposeOfVisit = (string)request.Form.Get("purposeOfVisit"),
                    totalDaysNight = Convert.ToString(request.Form.Get("totalDaysNight")),
                    totalPlainHills = Convert.ToString(request.Form.Get("totalPlanHills")),
                    taxi = (string)request.Form.Get("taxi"),
                    BookAgainstCentre = (string)request.Form.Get("BookAgainstCentre"),
                    availbilityOfFund = (string)request.Form.Get("availabilityOfFund"),
                    note = (string)request.Form.Get("note")
                };
                bool res = _managerService.insertHiring(formdata);
                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 200 : 500,
                    message = res ? "Added successfully!" : "Error Occured"
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }
        [HttpGet]
        [Route("api/getHiringList")]
        public IHttpActionResult getHiringList(int id)


        {
            try
            {
                var data = _managerService.GetHiringList(id);
                return Ok(new
                {
                    status = data.Any(),
                    data = data
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }
        [HttpGet]
        [Route("api/getAllHiringList")]
        public IHttpActionResult getAllHiringList(int userId)
        {
            try
            {
                var data = _managerService.GetHiringVehicles(userId);
                return Ok(new
                {
                    status = data.Any(),
                    data = data
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }
        #endregion
        private IHttpActionResult BadRequest(object value)
        {
            return Content(HttpStatusCode.BadRequest, value);
        }

        #region Report
        [HttpGet]
        [Route("api/ProjectManagerReimbursementReport")]
        public IHttpActionResult ProjectManagerReimbursementReport(int userId)
        {
            try
            {
                var data = _managerService.reinbursementReport(userId);
                return Ok(new
                {
                    status = data.Any(),
                    StatuCode = data.Any() ? 200 : 500,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("api/ProjectManagerHiringReportProjects")]
        public IHttpActionResult ProjectManagerHiringReportProjects(int userId)
        {
            try
            {
                var data = _managerService.ProjectManagerHiringReportProjects(userId);
                return Ok(new
                {
                    status = data.Any(),
                    StatuCode = data.Any() ? 200 : 500,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("api/ProjectManagerHiringReportbyProjects")]
        public IHttpActionResult ProjectManagerHiringReportbyProjects(int userId, int projectId)
        {
            try
            {
                var data = _managerService.ProjectManagerHiringReportbyProjects(userId, projectId);
                return Ok(new
                {
                    status = data.Any(),
                    StatuCode = data.Any() ? 200 : 500,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("api/ProjectManagerTourProposalReport")]
        public IHttpActionResult ProjectManagerTourProposalReport(int userId)
            {
                try
                {
                    var data = _managerService.ProjectManagertourreportProjects(userId);
                    return Ok(new
                    {
                        status = data.Any(),
                        StatuCode = data.Any() ? 200 : 500,
                        data = data
                    });
                }
                catch (Exception ex)
                {
                    return BadRequest(new
                    {
                        status = false,
                        StatusCode = 500,
                        message = ex.Message
                    });
                } 
        }
        [HttpGet]
        [Route("api/ProjectManagertourreportByProjects")]
        public IHttpActionResult ProjectManagertourreportByProjects(int userId,int projectId)
        {
            try
            {
                var data = _managerService.ProjectManagertourreportByProjects(userId,projectId);
                return Ok(new
                {
                    status = data.Any(),
                    StatuCode = data.Any() ? 200 : 500,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }
        #endregion

        #region Raise Problem
        [HttpPost]
        [Route("api/raiseproblem")]
        public IHttpActionResult RaiseProblem()
        {
            try
            {
                var request = HttpContext.Current.Request;
                var formdata = new RaiseProblem
                {
                    title = request.Form.Get("title").ToString(),
                    projectId = Convert.ToInt32(request.Form.Get("projectId")),
                    description = request.Form.Get("description").ToString(),
                    id = Convert.ToInt32(request.Form.Get("userId"))
                };
                var file = request.Files["document"];
                if (file.ContentLength>0)
                {
                    formdata.documentname = $"/ProjectContent/ProjectManager/raisedproblem{Guid.NewGuid()}{file.FileName}";
                }
                bool res = _managerService.insertRaisedProblem(formdata);
                if (res)
                {
                    if (file.ContentLength > 0)
                        file.SaveAs(HttpContext.Current.Server.MapPath(formdata.documentname));
                }
                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 200 : 500,
                    message = res ? "Added successfully!" : "Error Occured"
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }
        [HttpGet]
        [Route("api/getraisedproblem")]
        public IHttpActionResult getRaisedProblem(int userId)
        {
            try
            {
                var data = _managerService.getProblems(userId);
                return Ok(new
                {
                    status = data.Any(),
                    data = data
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }
        [HttpGet]
        [Route("api/deleteraisedproblem")]
        public IHttpActionResult deleteraisedproblem(int id,int userId)
        {
            try
            {
                bool res = _managerService.deleteRaisedProblem(id, userId);
                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 200 : 500,
                    message = res?"Deleted Successfully":"Some issue occured"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }
        #endregion
        #region attendance
        [HttpPost]
        [Route("api/addAttendancefrompm")]
        public IHttpActionResult AddAttendace(AttendanceManage am)
        {
            try
            {
                var result = _managerService.InsertAttendance(am);
                if (result.success)
                {
                    if (result.skippedDates.Count > 0)
                    {
                        return Ok(new
                        {
                            success = true,
                            StatusCode = 200,
                            message = "Attendance submitted. Some dates were skipped as they already exist.",
                            skipped = result.skippedDates
                        });
                    }

                    return Ok(new { success = true, StatusCode = 200, message = "Attendance submitted successfully." });
                }
                else
                {
                    return Ok(new { success = false, message = "Error occurred: " + result.error });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 404,
                    message = ex.Message
                });
            }
        }
        [HttpGet]
        [Route("api/getattendancebyIdofEmp")]
        public IHttpActionResult GetAttendanceByIdOfEmp(int projectManager,int EmpId)
        {
            try
            {
                var data = _managerService.GetAllAttendanceForProjectManager(projectManager, EmpId);
                if (data != null)
                {
                    return Ok(new
                    {
                        status = data.Any(),
                        data = data,
                        message = "Data found!"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = data.Any(),
                        data = data,
                        message = "Data not found!"
                    });
                }
            }
            catch(Exception ex)
            {
                return Ok(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }
        [HttpGet]
        [Route("api/ApproveAttendance")]
        public IHttpActionResult ApproveAttendance(int id, bool status, string remark)
        {
            try
            {
                bool res = _managerService.AttendanceApproval(id, status, remark);
                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 200 : 500,
                    message = (res == true && status == true) ? "Approved Successfully" : res ? "Rejected  Successfully" : "Something went wrong"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("api/getrepoattendance")]
        public IHttpActionResult getrepoattendance(int month, int year,int projectManager,int EmpId)
        {
            try
            {
                var data = _managerService.getReportAttendance(month, year,projectManager,EmpId);
                if (data != null)
                {
                    return Ok(new
                    {
                        status = data.Any(),
                        data = data,
                        message = "Data found!"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = data.Any(),
                        data = data,
                        message = "Data not found!"
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }
        #endregion
        #region Get All Report for App
        [HttpGet]
        [Route("api/getallreimbursementfilter")]
        public IHttpActionResult getallreimbursementforapp(int userId)
        {
            try
            {
                var data = _managerService.reinbursementReport(userId);
                return Ok(new
                {
                    status = data.Any(),
                    data = data
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }
        [HttpGet]
        [Route("api/getalltourproposalfilter")]
        public IHttpActionResult getalltourproposalforapp(int userId)
        {
            try
            {
                var data = _managerService.getTourList(userId);
                return Ok(new
                {
                    status = data.Any(),
                    data = data
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }
        [HttpGet]
        [Route("api/getallhiringfilter")]
        public IHttpActionResult getallhiringforapp(int userId)
        {
            try
            {
                var data = _managerService.GetHiringVehicles(userId);
                return Ok(new
                {
                    status = data.Any(),
                    data = data
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }
        #endregion

        #region Project Update 
        [HttpGet]
        [Route("api/getMonthlyIntUpdate")]
        public IHttpActionResult getMonthlyIntUpdate(int id)
        {
            try
            {
                var data = _managerService.MonthlyProjectUpdate(id);
                if (data != null)
                {
                    return Ok(new
                    {
                        status = data.Any(),
                        data = data,
                        message = "Data Found!"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = data.Any(),
                        StatusCode = 400,
                        message = "Data not Found!"
                    });
                }
            }
            catch(Exception ex)
            {
                return Ok(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }
        [HttpGet]
        [Route("api/getMonthlyExtUpdate")]
        public IHttpActionResult getMonthlyExtUpdate(int id)
        {
            try
            {
                var data = _managerService.GetExtrnlFinancialReport(id);
                if (data != null)
                {
                    return Ok(new
                    {
                        status = data.Any(),
                        data = data,
                        message = "Data Found!"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = data.Any(),
                        StatusCode = 400,
                        message = "Data not Found!"
                    });
                }
            }
            catch(Exception ex)
            {
                return Ok(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }
        [HttpPost]
        [Route("api/updateIntMonthly")]
        public IHttpActionResult MonthlyUpdateInt()
        {
            try
            {
                var request = HttpContext.Current.Request;
                var formdata = new Project_MonthlyUpdate
                {
                    ProjectId = Convert.ToInt32(request.Form.Get("ProjectId")),
                    date = Convert.ToDateTime(request.Form.Get("date")),
                    comments = request.Form.Get("comments").ToString(),
                    unit = request.Form.Get("unit").ToString(),
                    annual = request.Form.Get("annual").ToString(),
                    reviewMonth = request.Form.Get("reviewMonth").ToString(),
                    MonthEndSequentially = request.Form.Get("MonthEndSequentially").ToString(),
                    StateBeneficiaries = request.Form.Get("StateBeneficiaries").ToString()
                };
                bool res = _managerService.UpdateMonthlyStatus(formdata);
                if (res)
                {
                    return Ok(new
                    {
                        status = res,
                        StatusCode = 200,
                        message = "Update Successfully"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = res,
                        StatusCode = 500,
                        message = "Some Issue Occured"
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }
        [HttpPost]
        [Route("api/updateExtMonthly")]
        public IHttpActionResult MonthlyUpdateExt()
        {
            try
            {
                var request = HttpContext.Current.Request;
                var formdata = new FinancialMonthlyReport
                {
                    projectId = Convert.ToInt32(request.Form.Get("projectId")),
                    aim = request.Form.Get("aim").ToString(),
                    date = request.Form.Get("date").ToString(),
                    month_aim =  request.Form.Get("month_aim").ToString(),
                    completeInMonth = request.Form.Get("completeInMonth").ToString(),
                    departBeneficiaries = request.Form.Get("departBeneficiaries").ToString()
                };
                bool res = _managerService.updateFinancialReportMonthly(formdata);
                if (res)
                {
                    return Ok(new
                    {
                        status = res,
                        StatusCode = 200,
                        message = "Update Successfully"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = res,
                        StatusCode = 500,
                        message = "Some Issue Occured"
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }
        #endregion

        #region Progress Report
        [HttpPost]
        [Route("api/addEmpMonthlyReport")]
        public IHttpActionResult AddEmpMonthlyReport()
        {
            try
            {
                var req = HttpContext.Current.Request;
                EmpReportModel emp = new EmpReportModel
                {
                    PmId = Convert.ToInt32(req.Form.Get("PmId")),
                    ProjectId = Convert.ToInt32(req.Form.Get("ProjectId")),
                    Unit = req.Form.Get("Unit").ToString(),
                    AnnualTarget = Convert.ToInt32(req.Form.Get("AnnualTarget")),
                    TargetUptoReviewMonth = Convert.ToInt32(req.Form.Get("TargetUptoReviewMonth")),
                    AchievementDuringReviewMonth = Convert.ToInt32(req.Form.Get("AchievementDuringReviewMonth")),
                    CumulativeAchievement = Convert.ToInt32(req.Form.Get("CumulativeAchievement")),
                    BenefitingDepartments = req.Form.Get("BenefitingDepartments").ToString(),
                    Remarks = req.Form.Get("Remarks").ToString()
                };
                bool res = _managerService.InsertEmpReport(emp);
                return Ok(new
                {
                    status = res,
                    StatusCode = res ? 200 : 500,
                    message = res ? "Added Successfully!" : "Some error Occured"
                });
            }
            catch
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 500,
                    message = "Some error Occured"
                });
            }
        }

        [HttpGet]
        [Route("api/getEmpMonthlyReport")]   
        public IHttpActionResult getEmpMonthlyReport(int userId)
        {
            try
            {
                var data = _managerService.GetEmpReport(userId);
                if (data != null)
                {
                    return Ok(new
                    {
                        status = data.Any(),
                        data = data,
                        message = "Data Found!"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = data.Any(),
                        StatusCode = 400,
                        message = "Data not Found!"
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = false,
                    StatusCode = 500,
                    message = ex.Message
                });
            }
        }
        #endregion
    }
}
