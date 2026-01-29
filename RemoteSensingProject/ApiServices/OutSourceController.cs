// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// RemoteSensingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// RemoteSensingProject.ApiServices.OutSourceController
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using RemoteSensingProject.Models;
using RemoteSensingProject.Models.Admin;
using RemoteSensingProject.Models.LoginManager;
using RemoteSensingProject.Models.ProjectManager;
using RemoteSensingProject.Models.SubOrdinate;

namespace RemoteSensingProject.ApiServices
{
	[JwtAuthorize(Roles = "outSource")]
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
		public IHttpActionResult getOutSourceAssignTask(int id, int? limit = null, int? page = null, string searchTerm = null)
		{
			try
			{
				List<RemoteSensingProject.Models.SubOrdinate.main.OutSource_Task> data = _subordinate.getOutSourceTask(id, limit, page, searchTerm);
				string[] selectprop = new string[5] { "id", "Title", "Description", "CompleteStatus", "Status" };
				List<object> newdata = CommonHelper.SelectProperties(data, selectprop);
				if (data.Count > 0)
				{
					return CommonHelper.Success((ApiController)(object)this, newdata, "Data fetched successfully", 200, data[0].Pagination);
				}
				return CommonHelper.NoData((ApiController)(object)this);
			}
			catch (Exception ex)
			{
				return CommonHelper.Error((ApiController)(object)this, ex.Message);
			}
		}

		[HttpPost]
		[Route("api/submitOutSourceTask")]
		public IHttpActionResult submitOutSourceTask()
		{
			try
			{
				HttpRequest httpRequest = HttpContext.Current.Request;
				RemoteSensingProject.Models.SubOrdinate.main.OutSource_Task task = new RemoteSensingProject.Models.SubOrdinate.main.OutSource_Task
				{
					Reason = httpRequest.Form.Get("reason"),
					EmpId = Convert.ToInt32(httpRequest.Form.Get("EmpId")),
					id = Convert.ToInt32(httpRequest.Form.Get("id"))
				};
				if (_subordinate.AddOutSourceTask(task))
				{
					return Ok(new
					{
						status = true,
						message = "Task added successfully !"
					});
				}
				return Ok(new
				{
					status = false,
					message = "Some issue found while processing request. Please try after sometime."
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
				HttpRequest request = HttpContext.Current.Request;
				AttendanceManage formdata = new AttendanceManage
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
				(bool, string) res = _managerservice.insertAttendance(formdata);
				if (res.Item1)
				{
					if (res.Item2 == null)
					{
						return Ok(new
						{
							status = res.Item1,
							StatusCode = 400,
							message = "Attendance already marked for today."
						});
					}
					if (res.Item2 == "Added Successfully")
					{
						return Ok(new
						{
							status = res.Item1,
							StatusCode = 200,
							message = "Attendance added successfully."
						});
					}
					if (res.Item2 == "Server Error")
					{
						return Ok(new
						{
							status = res.Item1,
							StatusCode = 404,
							message = "Something went wrong. Try Again!"
						});
					}
					return Ok(new
					{
						status = false,
						StatusCode = 500
					});
				}
				return Ok(new
				{
					status = res.Item1,
					StatusCode = 500,
					message = "Server issue. Please Try after sometime!"
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
				List<AttendanceManage> data = _managerservice.GetAllAttendanceForOutsource(EmpId);
				if (data != null)
				{
					return Ok(new
					{
						status = true,
						data = data,
						message = "Data found !"
					});
				}
				return Ok(new
				{
					status = false,
					data = data,
					message = "Data not found !"
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
		[Route("api/knowToday")]
		public IHttpActionResult knowToday(int EmpId, DateTime attendancedate)
		{
			try
			{
				bool data = _managerservice.toKnowToday(EmpId, attendancedate);
				if (data)
				{
					return Ok(new
					{
						status = data,
						data = data,
						message = "Data found !"
					});
				}
				return Ok(new
				{
					status = data,
					data = data,
					message = "Data not found !"
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
		[Route("api/getChartData")]
		public IHttpActionResult getChartData(int EmpId)
		{
			try
			{
				List<AttendanceManage> data = _managerservice.chardata(EmpId);
				if (data != null)
				{
					return Ok(new
					{
						status = true,
						data = data,
						message = "Data found !"
					});
				}
				return Ok(new
				{
					status = false,
					data = data,
					message = "Data not found !"
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
		[Route("api/getDateTime")]
		public IHttpActionResult GetDateTime()
		{
			try
			{
				string currentDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				IHubContext context = GlobalHost.ConnectionManager.GetHubContext<RealtimeDateTime>();
				((dynamic)context.Clients.All).receiveTime(currentDateTime);
				return Ok(new
				{
					time = currentDateTime
				});
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
	}
}