using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Xml.Linq;
using Microsoft.AspNetCore.Hosting.Server;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.LoginManager;
using RemoteSensingProject.Models.ProjectManager;
using static RemoteSensingProject.Models.Admin.main;

namespace RemoteSensingProject.ApiServices
{
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
            catch(Exception ex)
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
            }catch(Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode= 500,
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("api/getWeeklyUpdate")]
        public IHttpActionResult getWeeklyUpdate(int projectId)
        {
            try
            {
                var data = _managerService.WeeklyUpdateList(projectId);
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
                var formData = new Project_WeeklyUpdate
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
                bool res = _managerService.updateWeeklyStatus(formData);
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
                if(file != null && file.FileName != "")
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
            }catch(Exception ex)
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
                var data = _managerService.DashboardCount(userId);
                return Ok(new
                {
                    status =true,
                    message = "Data found !",
                    data = data
                });
            }catch(Exception ex)
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
        public IHttpActionResult AssignedPRoject(int userId)
        {
            try
            {
                var data = _managerService.getAllProjectByManager(userId.ToString());
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
            }catch(Exception ex)
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
        public IHttpActionResult GetProjectList(int userId)
        {
            try
            {
                var data = _managerService.Project_List(userId.ToString());
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
        public IHttpActionResult GetCompleteProject(int userId)
        {
            try
            {
                var data = _managerService.All_Project_List(userId.ToString()).Where(d => d.ProjectStatus).ToList();
                return Ok(new
                {
                    status = data.Any(),
                    StatusCode = data.Any() ? 200 : 500,
                    data = data
                });
            }catch(Exception ex)
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
        public IHttpActionResult getmanagerDelay(int userId)
        {
            try
            {
                var data = _managerService.All_Project_List(userId.ToString()).Where(d => d.CompletionDate < DateTime.Now && !d.ProjectStatus).ToList();
                return Ok(new
                {
                    status = data.Any(),
                    StatusCode = data.Any() ? 200 : 500,
                    data = data
                });
            }catch(Exception ex)
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
        public IHttpActionResult onGoingProject(int userId)
        {
            try
            {
                var data = _managerService.All_Project_List(userId.ToString()).Where(d => d.StartDate > DateTime.Now && d.CompletionDate < DateTime.Now).ToList();
                return Ok(new
                {
                    status = data.Any(),
                    StatusCode = data.Any() ? 200 : 500,
                    data = data
                });
            }catch(Exception ex)
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
            }catch(Exception ex)
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
        public IHttpActionResult getAllmeeting( int managerId)
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
            catch(Exception ex)
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
            }catch(Exception ex)
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
                if(outSourceList != null)
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
            }catch(Exception ex)
            {
                return BadRequest(new { 
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
                    message = res ?"Task updated successfully !" : "Some issue occured !"
                });
            }catch(Exception ex)
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


        private IHttpActionResult BadRequest(object value)
        {
            return Content(HttpStatusCode.BadRequest, value);
        }
    }
}
