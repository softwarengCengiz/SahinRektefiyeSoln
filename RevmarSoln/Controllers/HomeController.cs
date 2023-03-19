using SahinRektefiyeSoln.Components;
using SahinRektefiyeSoln.Helpers;
using SahinRektefiyeSoln.Helpers.LogHelper;
using SahinRektefiyeSoln.Infrastructure;
using SahinRektefiyeSoln.Models;
using SahinRektefiyeSoln.Models.HelperModels;
using SahinRektefiyeSoln.Models.ViewModels;
using SahinRektefiyeSoln.Models.ViewModels.Dashboard;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SahinRektefiyeSoln.Controllers
{
	[LogActionFilter]
	public class HomeController : BaseController
	{
		SahinRektefiyeDbEntities db = new SahinRektefiyeDbEntities();
		EncryptionHelper encryption = new EncryptionHelper();

		public bool CheckMyRole(string RoleName)
		{
			if (Session["UserName"] != null) // Giriş yapılmış mı?
			{
				bool isInRole = false;
				string UserName = Session["UserName"].ToString();
				Roles role;


				try
				{
					role = db.Roles.Where(x => x.RoleName == RoleName).FirstOrDefault(); // Kontrol edilmek istenen Role'ü bul Id'si ile kontrol etmek için
				}
				catch (Exception ex)
				{
					throw ex;
				}

				if (role == null)
				{
					throw new Exception("Rol Bulunamadı ! \n Kullanıcı Adı : " + UserName + "\n Parameter :  " + RoleName);
				}


				List<UserRoles> roleList = db.UserRoles.Where(x => x.UserName == UserName).ToList(); //Kullanıcının rollerini bul

				if (roleList == null)
				{
					throw new Exception("Kullanıcı Rolleri Bulunamadı ! \n Kullanıcı Adı : " + UserName + "\n Parameter :  " + RoleName);
				}

				foreach (var rowRole in roleList)
				{
					if (rowRole.RoleId == role.RoleId) // Eğer kullanıcını herhangi bir rolü bizim kontrol ediceğimiz roleId ye uyuşur ise true dön
						isInRole = true;

				}

				return isInRole;
			}
			else
			{
				return false;
			}
		}

		[HttpPost]
		public JsonResult PostMail(Messages model)
		{
			#region Controls

			if (model.FromName ==null)
				return Json(new { response = "false" , ErrorMessage="İsim boş geçilemez." });

			if (model.FromEmail == null)
				return Json(new { response = "false", ErrorMessage = "Email boş geçilemez." });

			if (model.FromSubject == null)
				return Json(new { response = "false", ErrorMessage = "Konu boş geçilemez." });

			if (model.Message == null)
				return Json(new { response = "false", ErrorMessage = "Mesaj boş geçilemez." });

			#endregion

			try
			{
				model.CreatedDate = System.DateTime.Now;
				model.SwRead = false;
				db.Messages.Add(model);
				db.SaveChanges();
				return Json(new { response = "true" , ErrorMessage = "" });

			}
			catch (Exception ex)
			{
				new WebErrorLogsHelper().Log2Db(ex);
				return Json(new { response = "false" , ErrorMessage = ex.InnerException.StackTrace.Substring(0,250) });
			}
			
		}

		#region Login

		public ActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Login(Users user)
		{
			string encryptedpass = encryption.Encrypt(user.Password);
			var obj = db.Users.Where(a => a.UserName.Equals(user.UserName) && a.Password.Equals(encryptedpass)).FirstOrDefault();
			if (obj != null)
			{
				Session["UserName"] = obj.UserName.ToString();
				return RedirectToAction("Index", "Home");
			}

			return View();
		}

		public bool CheckMyPermission(string permission)
		{
			if (Session["UserName"] != null)
			{
				bool isPermitted = false;
				string userName = Session["UserName"].ToString();
				List<UserRoles> userRoles = db.UserRoles.Where(x => x.UserName == userName).ToList();

				foreach (UserRoles role in userRoles)
				{
					var perm = db.RolePermissions.Where(x => x.RoleId == role.RoleId && x.Permissions.PermissionName == permission).FirstOrDefault();//
					if (perm != null)
						isPermitted = true;
				}
				return isPermitted;
			}
			else
			{
				return false;
			}

		}

		public bool isLogin()
		{
			if (Session["UserName"] != null)
				return true;
			else return false;
		}

		public ActionResult NotPermitted()
		{
			return View();
		}

		public ActionResult SetPageHeader(string pageTitle, string pagePermission)
		{
			ViewBag.PageTitle = pageTitle;
			ViewBag.PagePermission = pagePermission;
			return View();
		}
		#endregion

		public bool isMonthlyFirstLogin()
		{
			bool result = false;
			try
			{
				// Eğerki Agentde Bu Ay Bu yıl kayıdı yok ise o ay ilk defa giriyordur. 

				int count = db.UserAgentInfos.Where(x => x.UserName == this.userName &&x.Message.Contains("login")).Count();

				if (count > 0)
					return false;
				else
					return true;

			}
			catch (Exception)
			{
				return result;
			}

		}

		public bool isFordServiceUser()
		{
			try
			{
				int CompanyId = Convert.ToInt32(ConfigurationManager.AppSettings["FordServiceId"].ToString());
				Users kullanici = db.Users.FirstOrDefault(x => x.UserName == this.userName);

					return true;
		

			}
			catch (Exception ex)
			{
				return false;
			}
		}

		// GET: Home
		public ActionResult Index()
        {
			if (isLogin())
			{
				//LogHelper.Log(LogTarget.Database, new Logs() { UserName = Session["UserName"].ToString(), LogMessage = "ViewBag.isRevmerUser - " + ViewBag.isRevmerUser, CreatedDate = System.DateTime.Now });

				ViewBag.ActiveUsers = MvcApplication.Sessions.Count();
				ViewBag.isRevmerUser =  SFHelper.isRevmerUser(Session["UserName"].ToString());

				ViewBag.IsReadOnly = false;

				if (!(CheckMyRole("ADMIN") || CheckMyRole("YONETICI") ))
				{
					if (SFHelper.isFordMerkezUser(this.userName))
					{
						return RedirectToAction("StockDashboard", "FordSiparis");
					}
				}

				List<WorkOrders> allWorkOrders = db.WorkOrders.ToList(); 
				ViewBag.CompaniesCount = db.Companies.Count();
				ViewBag.ServicesCount = db.Services.Count();
				ViewBag.WorkOrdersCount = allWorkOrders.Where(x => x.Statu == 10).Count(); // Açık olanlar
				ViewBag.UsersCount = db.Users.Count();
				ViewBag.WayBillsCount = db.WayBills.Where(x => x.Statu == 10).Count();// Açık olanlar
				ViewBag.PartsCount = db.Parts.Count();
				ViewBag.SuppliersCount = db.Suppliers.Count();

				bool yetkili = false;
				if (CheckMyRole("ADMIN") || CheckMyRole("YONETICI") )
				{
					yetkili = true;
				}

				ViewBag.yetkili = yetkili;

				int servisIadeOnay = Parameters.IadeOnayBekleniyor;
				int servisIptalOnay = Parameters.IptalOnayBekleniyor;

				ViewBag.OnayBekleyenIade = allWorkOrders.Where(x => x.Statu == 33 ).Count() + db.IsEmirleri.Where(x=>x.IsEmriStatuId == servisIadeOnay).Count();
				ViewBag.OnayBekleyenIptal = allWorkOrders.Where(x => x.Statu == 34 ).Count() + db.IsEmirleri.Where(x => x.IsEmriStatuId == servisIptalOnay).Count();

				ViewBag.WorkOrders = allWorkOrders
									.GroupBy(o => new
									{
										Month = o.CreatedDate.Month,
										Year = o.CreatedDate.Year
									})
									.Select(g => new ArchiveEntry
									{
										Month = g.Key.Month,
										Year = g.Key.Year,
										Total = g.Count()
									})
									.OrderByDescending(a => a.Year)
									.ThenByDescending(a => a.Month)
									.ToList();

				ViewBag.WayBills = db.WayBills
									.GroupBy(o => new
									{
										Month = o.WayBillIncomeDate.Month,
										Year = o.WayBillIncomeDate.Year
									})
									.Select(g => new ArchiveEntry
									{
										Month = g.Key.Month,
										Year = g.Key.Year,
										Total = g.Count()
									})
									.OrderByDescending(a => a.Year)
									.ThenByDescending(a => a.Month)
									.ToList();

				ViewBag.WorkOrdersMonthly = allWorkOrders
											.Where(x => x.CreatedDate > System.DateTime.Now)
											.GroupBy(x => new { x.TaskId });

				return View();
			}
			else
			{
				return RedirectToAction("Login", "Admin");
			}
			
        }

        public ActionResult GetMenuForUser()
        {
            //Get User authorized Menus
            string UserName = Session["UserName"] != null ? Session["UserName"].ToString() : "";
			List<UserMenus> usermenus = db.UserMenus.Where(x => x.UserName == UserName).OrderByDescending(x => x.Menus.Rank).ToList();
			ViewBag.Cat10Cnt = usermenus.Where(x => x.Menus.MenuRank == 10).Count();
			ViewBag.Cat20Cnt = usermenus.Where(x => x.Menus.MenuRank == 20).Count();
			ViewBag.Cat30Cnt = usermenus.Where(x => x.Menus.MenuRank == 30).Count();
			ViewBag.Cat40Cnt = usermenus.Where(x => x.Menus.MenuRank == 40).Count();
			ViewBag.Cat50Cnt = usermenus.Where(x => x.Menus.MenuRank == 50).Count();
			ViewBag.Cat60Cnt = usermenus.Where(x => x.Menus.MenuRank == 60).Count();
			ViewBag.Cat70Cnt = usermenus.Where(x => x.Menus.MenuRank == 70).Count();
			ViewBag.Cat80Cnt = usermenus.Where(x => x.Menus.MenuRank == 80).Count();
			ViewBag.Cat90Cnt = usermenus.Where(x => x.Menus.MenuRank == 90).Count();
			ViewBag.Cat100Cnt = usermenus.Where(x => x.Menus.MenuRank == 100).Count();

			//if (CheckMyRole("Fiat Bayi Yetkilisi") || CheckMyRole("Fiat Merkez Rolü"))
			//	ViewBag.FiatBayiMenuShow = true;

			ViewBag.isMuhasebeUser = false;

			//if (CheckMyRole("Ford Bayi Yetkilisi") || CheckMyRole("Ford Merkez Rolü"))
			//	ViewBag.FordBayiMenuShow = true;




			return View(usermenus);
			//return View(db.UserMenus.Where(x => x.UserName == UserName).OrderByDescending(x=>x.Menus.Rank).ToList());
		}

		public ActionResult GetMenuForUserMobile()
		{
			//Get User authorized Menus
			string UserName = Session["UserName"] != null ? Session["UserName"].ToString() : "";
			List<UserMenus> usermenus = db.UserMenus.Where(x => x.UserName == UserName).OrderByDescending(x => x.Menus.Rank).ToList();
			ViewBag.Cat10Cnt = usermenus.Where(x => x.Menus.MenuRank == 10).Count();
			ViewBag.Cat20Cnt = usermenus.Where(x => x.Menus.MenuRank == 20).Count();
			ViewBag.Cat30Cnt = usermenus.Where(x => x.Menus.MenuRank == 30).Count();
			ViewBag.Cat40Cnt = usermenus.Where(x => x.Menus.MenuRank == 40).Count();
			ViewBag.Cat50Cnt = usermenus.Where(x => x.Menus.MenuRank == 50).Count();
			ViewBag.Cat60Cnt = usermenus.Where(x => x.Menus.MenuRank == 60).Count();
			ViewBag.Cat70Cnt = usermenus.Where(x => x.Menus.MenuRank == 70).Count();
			ViewBag.Cat80Cnt = usermenus.Where(x => x.Menus.MenuRank == 80).Count();
			ViewBag.Cat90Cnt = usermenus.Where(x => x.Menus.MenuRank == 90).Count();

			//if (CheckMyRole("Fiat Bayi Yetkilisi") || CheckMyRole("Fiat Merkez Rolü"))
			//	ViewBag.FiatBayiMenuShow = true;


			return View(usermenus);
		}

		public class ArchiveEntry
		{
			public int Year { get; set; }
			public int Month { get; set; }
			public string MonthName
			{
				get
				{
					return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Month);
				}
			}
			public int Total { get; set; }

			
		}

		public class MonthlyWorkOrders
		{
			public string name { get; set; }
			public int y { get; set; }
		}

		[HttpPost]
		public ActionResult ReadMail(int MessageId)
		{
			db.Messages.Find(MessageId).SwRead = true;
			db.SaveChanges();
			return Json(new { response = "true" });
		}

		public ActionResult GetMailButton()
		{
			ViewBag.UnReadMessages = db.Messages.Where(x => x.SwRead == false).Count();
			ViewBag.IsManager = CheckMyRole("ADMIN") || CheckMyRole("YONETICI") ;
			return View();
		}

		public ActionResult ChangeMenuPreference()
		{
			bool isSideBarOpen = Convert.ToBoolean(Session["isSideBarOpen"]);

			Session["isSideBarOpen"] = !isSideBarOpen;

			return Redirect(Request.UrlReferrer.AbsoluteUri);
		}

		public ActionResult GetHizliArama()
        {
            string userName= Session["UserName"].ToString();

			ViewBag.isRevmerUser = SFHelper.isRevmerUser(userName);

			return View();
		}

		[SessionAuthorization]
		[HttpPost]
		public ActionResult HizliArama(string txtplakaOrSase)
		{
			HizliAramaViewModel model = new HizliAramaViewModel();
			string plakaOrSase = txtplakaOrSase.Trim().ToLower();
			
			// 1- Servis İş Emri Kaydı
			List<IsEmirleri> isEmirleri = db.IsEmirleri.Where(x => x.Arac.Plaka.ToLower().Trim() == plakaOrSase || x.Arac.SaseNo == plakaOrSase).ToList();

			// 1- Revizyon İş Emri Kaydı
			List<WorkOrders> revizyonKayitlari = db.WorkOrders.Where(x => x.Plate.ToLower().Trim() == plakaOrSase || x.VIN.Trim().ToLower() == plakaOrSase).ToList();
			// 3- Ford Parça Kaydı

			// 4- Fiat Sipariş Kayı 

			// 5- Araç Kaydı
			List<Arac> aracs = db.Arac.Where(x => x.Plaka.ToLower().Trim() == plakaOrSase).ToList();


			model.plakaVeyaSase = plakaOrSase;
			model.isEmirleri = isEmirleri;
			model.revizyonKayitlari = revizyonKayitlari;
			model.servisAraclar = aracs;

			return View(model);
		}

		[SessionAuthorization]
		public ActionResult GetBayiDashBoard()
		{
			UserServiceDashBoard model = new UserServiceDashBoard();
			Users kullanici = db.Users.Where(x => x.UserName == this.userName).FirstOrDefault();

			int FiatCompanyId = Convert.ToInt32(ConfigurationManager.AppSettings["FiatServiceId"]); 
			int FordCompanyId = Convert.ToInt32(ConfigurationManager.AppSettings["FordServiceId"]); 

			return View(model);
		}

		public ActionResult GetApprovalWorkOrders()
		{
			GetApprovalWorkOrderPageModelcs model = new GetApprovalWorkOrderPageModelcs();
			model.approvalWorkOrders = db.WorkOrders.Where(x => x.Statu ==33 || x.Statu ==34 ).ToList();

			int IadeOnayBekleyen = Parameters.IadeOnayBekleniyor;
			int IptalOnayBekleyen = Parameters.IptalOnayBekleniyor;
			int BedelsizOnayBekleyen = Parameters.IsEmriBedelsizOnayBekliyor;

			model.approvalWorkOrdersServis = db.IsEmirleri.Where(x => x.IsEmriStatuId == IadeOnayBekleyen ||
																	x.IsEmriStatuId == IptalOnayBekleyen || 
																	(x.SwBedelsizOnay == true && x.IsEmriStatuId == BedelsizOnayBekleyen)
																	).ToList();

			return View(model);
		}

	}
}