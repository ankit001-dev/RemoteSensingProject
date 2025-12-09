using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNet.SignalR;
using RemoteSensingProject.Models;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.LoginManager;
using RemoteSensingProject.Models.ProjectManager;
using RemoteSensingProject.Models.SubOrdinate;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Web;
using System.Web.Http;
using static RemoteSensingProject.Models.SubOrdinate.main;

namespace RemoteSensingProject.ApiServices
{
    [JwtAuthorize]
    public class OutSourceController : ApiController
    {
        private readonly AdminServices _adminServices;
        private readonly ManagerService _managerservice;
        private readonly LoginServices _loginService;
        private readonly SubOrinateService _subordinate;
        public OutSourceController()
        {
            _adminServices = new AdminServices();
            _loginService = new LoginServices();
            _managerservice = new ManagerService();
            _subordinate = new SubOrinateService();
        }
        [HttpGet]
        [Route("api/getOutSourceTask")]
        public IHttpActionResult getOutSourceAssignTask(int id,int? limit = null,int? page = null,string searchTerm = null)
        {
            try
            {
                var data = _subordinate.getOutSourceTask(id,limit:limit,page:page,searchTerm:searchTerm);
                var selectprop = new[] { "id", "Title", "Description", "CompleteStatus", "Status" };
                var newdata = CommonHelper.SelectProperties(data, selectprop);
                if (data.Count > 0)
                {
                    return CommonHelper.Success(this, newdata, "Data fetched successfully", 200, data[0].Pagination);
                }
                else
                {
                    return CommonHelper.NoData(this);
                }
            }
            catch (Exception ex)
            {
                return CommonHelper.Error(this, ex.Message);
            }

        }
        [HttpPost]
        [Route("api/submitOutSourceTask")]
        public IHttpActionResult submitOutSourceTask()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                OutSource_Task task = new OutSource_Task
                {
                    Reason = httpRequest.Form.Get("reason"),
                    EmpId = Convert.ToInt32(httpRequest.Form.Get("EmpId")),
                    id = Convert.ToInt32(httpRequest.Form.Get("id"))
                };
                bool status = _subordinate.AddOutSourceTask(task);
                if (status)
                {
                    return Ok(new
                    {
                        status = true,
                        message = "Task added successfully !"
                    }); ;
                }
                else
                {
                    return Ok(new
                    {
                        status = false,
                        message = "Some issue found while processing request. Please try after sometime."
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

        private IHttpActionResult BadRequest(object value)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("api/addAttendance")]
        public IHttpActionResult addAttendance()
        {
            try
            {
                var request = HttpContext.Current.Request;
                var formdata = new AttendanceManage
                {
                    EmpId = Convert.ToInt32(request.Form.Get("EmpId")),
                    address = request.Form.Get("address").ToString(),
                    longitude = request.Form.Get("longitude").ToString(),
                    latitude = request.Form.Get("latitude").ToString(),
                    attendanceStatus = request.Form.Get("attendanceStatus").ToString(),
                    attendanceDate = Convert.ToDateTime(request.Form.Get("attendanceDate")),
                    projectManager = Convert.ToInt32(request.Form.Get("projectManager"))
                };
                DateTime today = DateTime.Now.Date;
                if (formdata.attendanceDate.Date != today)
                {
                    return Ok(new
                    {
                        status = false,
                        StatusCode = 500,
                        message = "Attendance can only be recorded for today."
                    });
                }
                var res = _managerservice.insertAttendance(formdata);
                if (res.success)
                {
                    if (res.error == null)
                    {
                        return Ok(new
                        {
                            status = res.success,
                            StatusCode = 400,
                            message = "Attendance already marked for today."
                        });
                    }
                    else if (res.error == "Added Successfully")
                    {
                        return Ok(new
                        {
                            status = res.success,
                            StatusCode = 200,
                            message = "Attendance added successfully."
                        });
                    }
                    else if (res.error == "Server Error")
                    {
                        return Ok(new
                        {
                            status = res.success,
                            StatusCode = 404,
                            message = "Something went wrong. Try Again!"
                        });
                    }
                }
                else
                {
                    return Ok(new
                    {
                        status = res.success,
                        StatusCode = 500,
                        message = "Server issue. Please Try after sometime!"
                    });
                }
                return Ok(new
                {
                    status = false,
                    StatusCode = 500
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
        [Route("api/getAttendance")]
        public IHttpActionResult GetAllAttendanceForOutsource(int EmpId)
        {
            try
            {
                var data = _managerservice.GetAllAttendanceForOutsource(EmpId);
                if (data != null)
                {
                    return Ok(new
                    {
                        status = true,
                        data = data,
                        message = "Data found !"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = false,
                        data = data,
                        message = "Data not found !"
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
        [HttpGet]
        [Route("api/knowToday")]
        public IHttpActionResult knowToday(int EmpId,DateTime attendancedate)
        {
            try
            {
                var data = _managerservice.toKnowToday(EmpId, attendancedate);
                if (data)
                {
                    return Ok(new
                    {
                        status = data,
                        data = data,
                        message = "Data found !"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = data,
                        data = data,
                        message = "Data not found !"
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
        [HttpGet]
        [Route("api/getChartData")]
        public IHttpActionResult getChartData(int EmpId)
        {
            try
            {
                var data = _managerservice.chardata(EmpId);
                if (data != null)
                {
                    return Ok(new
                    {
                        status = true,
                        data = data,
                        message = "Data found !"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = false,
                        data = data,
                        message = "Data not found !"
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
        [HttpGet]
        [Route("api/getDateTime")]
        public IHttpActionResult GetDateTime()
        {
            try
            {
               
                string currentDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // Get SignalR context and push the DateTime to all connected clients
                var context = GlobalHost.ConnectionManager.GetHubContext<RealtimeDateTime>();
                context.Clients.All.receiveTime(currentDateTime);  // Broadcasting time to all clients

                // Return the DateTime in the response
                return Ok(new { time = currentDateTime });

                //DateTime date = DateTime.Now.Date;
                //DateTime time = DateTime.Now.ToLocalTime();
                //return Ok(new
                //{
                //    date = date,
                //    //time = 
                //});
            }
            catch(Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    StatusCode = 404,
                    message = ex.Message
                });
            }
        }
    }
}
