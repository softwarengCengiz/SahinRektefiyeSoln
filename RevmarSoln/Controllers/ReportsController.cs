using PagedList;
using SahinRektefiyeSoln.Components;
using SahinRektefiyeSoln.Helpers;
using SahinRektefiyeSoln.Helpers.LogHelper;
using SahinRektefiyeSoln.Infrastructure;
using SahinRektefiyeSoln.Models;
using SahinRektefiyeSoln.Models.HelperModels;
using SahinRektefiyeSoln.Models.ViewModels.Reports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SahinRektefiyeSoln.Controllers
{
	[LogActionFilter]
	public class ReportsController : BaseController
	{
		SahinRektefiyeDbEntities db = new SahinRektefiyeDbEntities();

		[SessionAuthorization(PermissionName = "asdf")]
		public ActionResult Index()
		{

			return View();
		}

		
		public ActionResult DenemeTable(DenemeTableViewModel filterModel)
		{
			int pageIndex = filterModel.Page ?? 1;
			
			List<Users> customers = db.Users.ToList();
			if (filterModel.filter != null)
			{
				customers = db.Users.Where(x => x.UserName.Contains(filterModel.filter.UserName) && x.PhoneNumber.Contains(filterModel.filter.PhoneNumber)).ToList();
			}
			//List<Users> customers = db.Users.Where(x => x.UserName.Contains(filterModel.filter.UserName) && x.PhoneNumber.Contains(filterModel.filter.PhoneNumber)).ToList();

			DenemeTableViewModel pageModel = new DenemeTableViewModel();
			pageModel.kullanicilar = customers.ToPagedList(pageIndex, 10);

			if (Request.IsAjaxRequest())
			{
				return PartialView("_ReportUsers", pageModel);
			}
			else {
				return View(pageModel);
			}
			
		}

		public ActionResult DenemePartTable(DenemePartTableViewModel filterModel)
		{
			int pageIndex = filterModel.Page ?? 1;

			List<StokKarti> parcalar = db.StokKarti.ToList();

			if (filterModel.ParcaNo != null)
			{
				parcalar = parcalar.Where(x => x.ParcaNo.ToLower().Contains(filterModel.ParcaNo.ToLower())).ToList();
			}

			if (filterModel.ParcaAdi != null)
			{
				parcalar = parcalar.Where(x => x.ParcaAdi.ToLower().Contains(filterModel.ParcaAdi.ToLower())).ToList();
			}

			DenemePartTableViewModel pageModel = new DenemePartTableViewModel();
			pageModel.parcalar = parcalar.ToPagedList(pageIndex, 10);

			if (Request.IsAjaxRequest())
			{
				return PartialView("_ReportPartTable", pageModel);
			}
			else
			{
				return View(pageModel);
			}
		}

		//[SessionAuthorization(PermissionName = "asdf")]
		public ActionResult DashBoard()
		{
			List<WorkOrders> workorders = db.WorkOrders.ToList();
			return View(workorders);

		}

		public ActionResult UserIp()
		{
			List<UserAgentInfos> list = db.UserAgentInfos.ToList();
			return View(list);
		}


		public ActionResult ComboUI()
		{
			ViewBag.Iller = new SelectList(db.iller.ToList().OrderBy(x => x.sehir), "id", "sehir");
			return View();
		}

		public ActionResult Fuel()
		{
			return View();
		}

		public JsonResult GetIsEmirleri()
		{
			JsonResult result = new JsonResult();
			try
			{
				// Initialization.   
				string search = Request.Form.GetValues("search[value]")[0];
				string draw = Request.Form.GetValues("draw")[0];
				string order = Request.Form.GetValues("order[0][column]")[0];
				string orderDir = Request.Form.GetValues("order[0][dir]")[0];
				int startRec = Convert.ToInt32(Request.Form.GetValues("start")[0]);
				int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);
				// Loading.   
				List<IsEmirleri> data = db.IsEmirleri.ToList();
				// Total record count.   
				int totalRecords = data.Count;
				// Verification.   
				if (!string.IsNullOrEmpty(search) &&
					!string.IsNullOrWhiteSpace(search))
				{
					// Apply search   
					data = data.Where(p => p.IsEmriId.ToString().ToLower().Contains(search.ToLower()) //||
																									  //p.LastName.ToLower().Contains(search.ToLower()) ||
																									  //p.EmailID.ToString().ToLower().Contains(search.ToLower()) ||
																									  //p.UserRole.UserRoleName.ToLower().Contains(search.ToLower()) ||
																									  //p.UserStatus.Name.ToLower().Contains(search.ToLower())
					 ).ToList();
				}
				// Sorting.   
				if (!(string.IsNullOrEmpty(order) && string.IsNullOrEmpty(orderDir)))
				{
					data = data.OrderBy(x => order + " " + orderDir).ToList();
				}
				int recFilter = data.Count;
				data = data.Skip(startRec).Take(pageSize).ToList();

				var modifiedData = data.Select(d =>
					new
					{
						IsEmriId = d.IsEmriId
						//Name = d.FirstName + " " + d.LastName,
						//Email = d.EmailID,
						//Status = d.UserStatus.Name,
						//UserRole = d.UserRole.UserRoleName
					}
					);
				// Loading drop down lists.   
				result = this.Json(new
				{
					draw = Convert.ToInt32(draw),
					recordsTotal = totalRecords,
					recordsFiltered = recFilter,
					data = modifiedData
				}, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				new WebErrorLogsHelper().Log2Db(ex);
				// Info   
				Console.Write(ex);
			}
			// Return info.   
			return result;
		}

		
		public class SiparisParcaGrubu
		{
			public int ServiceId { get; set; }
			public int PartTypeId { get; set; }
			public string ParcaAdi { get; set; }
			public string OnTaki { get; set; }
			public string OrtaTaki { get; set; }
			public string SonTaki { get; set; }
			public int Adet { get; set; }
		}

		public class StokGrup
		{
			public int PartType { get; set; }
			public string OnTaki { get; set; }
			public string OrtaTaki { get; set; }
			public string SonTaki { get; set; }
			public int Adet { get; set; }
		}

		public static string getKonsinyeJobName()
		{
			return AppDomain.CurrentDomain.BaseDirectory + "\\JobLogs\\OtomatikKonsinye_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
		}

	
	}
}