using ClosedXML.Excel;
using SahinRektefiyeSoln.Components;
using SahinRektefiyeSoln.Helpers;
using SahinRektefiyeSoln.Helpers.LogHelper;
using SahinRektefiyeSoln.Infrastructure;
using SahinRektefiyeSoln.Models;
using SahinRektefiyeSoln.Models.FilterModels;
using SahinRektefiyeSoln.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SahinRektefiyeSoln.Controllers
{
	[LogActionFilter]
	public class SupplierCompanyController : BaseController
	{
		#region PermControl
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

		public bool isLogin()
		{
			if (Session["UserName"] != null)
				return true;
			else return false;
		}

		public ActionResult NotPermitted()
		{
			return RedirectToAction("NotPermitted", "Admin");
		}
		#endregion

		SahinRektefiyeDbEntities db = new SahinRektefiyeDbEntities();
		// GET: SupplierCompany
		public ActionResult Index()
        {
            return View();
        }

		
		#region ParçaCinsi|Ekle|Düzenle|Sil
		
		public ActionResult PartReviewTypes()
		{
			if (isLogin())
			{
				if (CheckMyPermission("SUPMN_01"))
				{
					string Mesaj = TempData["ErrorMessage"] as string;
					TempData["ErrorMessage"] = null;
					ViewBag.Message = Mesaj;
					return View(db.PartReviewType.ToList());
				}
				else
					return RedirectToAction("NotPermitted");
			}
			else
			{
				return RedirectToAction("Login", "Admin");
			}

		}
		
		public ActionResult CreatePartReviewType()
		{
			if (isLogin())
			{
				if (CheckMyPermission("SUPMN_01"))
				{

					return View();
				}
				else
					return RedirectToAction("NotPermitted");
			}
			else
			{
				return RedirectToAction("Login", "Admin");
			}
		}

		[HttpPost]
		public ActionResult CreatePartReviewType(PartReviewType model)
		{
			if (isLogin())
			{
				if (CheckMyPermission("SUPMN_01"))
				{

					try
					{
						model.CreatedDate = System.DateTime.Now;
						model.Creator = Session["UserName"].ToString();
						db.PartReviewType.Add(model);
						db.SaveChanges();
						ViewBag.Result = 1;
						return View();
					}
					catch (Exception ex)
					{
						ViewBag.Result = 0;
						return View();
					}
				}
				else
					return RedirectToAction("NotPermitted");
			}
			else
			{
				return RedirectToAction("Login", "Admin");
			}
		}

		[HttpGet]
		public ActionResult EditPartReviewType(int PartReviewTypeId)
		{
			if (isLogin())
			{
				if (CheckMyPermission("SUPMN_01"))
				{
					return View(db.PartReviewType.Where(x => x.PartReviewTypeId == PartReviewTypeId).FirstOrDefault());
				}
				else
					return RedirectToAction("NotPermitted");
			}
			else
			{
				return RedirectToAction("Login", "Admin");
			}
		}

		[HttpPost]
		public ActionResult EditPartReviewType(PartReviewType model)
		{
			if (isLogin())
			{
				if (CheckMyPermission("SUPMN_01"))
				{
					PartReviewType edited = db.PartReviewType.Where(x => x.PartReviewTypeId == model.PartReviewTypeId).FirstOrDefault();
					edited.PartTypeDesc = model.PartTypeDesc;
					edited.CreatedDate = System.DateTime.Now;
					edited.Creator = Session["UserName"].ToString();
					db.SaveChanges();
					return RedirectToAction("PartReviewTypes");
				}
				else
					return RedirectToAction("NotPermitted");
			}
			else
			{
				return RedirectToAction("Login", "Admin");
			}
		}

		[HttpGet]
		public ActionResult DeletePartReviewType(int PartReviewTypeId)
		{
			if (isLogin())
			{
				if (CheckMyPermission("SUPMN_01"))
				{
					string error = "";
					bool result = true;
					List<SupplierParts> FirmaParcalari = db.SupplierParts.Where(x => x.PartReviewTypeId == PartReviewTypeId).ToList();

					if (FirmaParcalari.Count() > 0)
					{
						result = false;
						error += "Sonuç'a bağlı firma parçası bulunmaktadır.<br/>";
					}
					else {
						result = true;
					}
					
					ViewBag.Result = result == true ? 1 : 0;

					if (result) // Servis'e bağlı bir şey bulunamadı Sil ! 
					{
						PartReviewType deleted = db.PartReviewType.Find(PartReviewTypeId);
						db.PartReviewType.Remove(deleted);
						db.SaveChanges();

					}
					TempData["ErrorMessage"] = error;

					return RedirectToAction("PartReviewTypes");
				}
				else
					return RedirectToAction("NotPermitted");
			}
			else
			{
				return RedirectToAction("Login", "Admin");
			}
		}

		#endregion

		#region RevizyonTipi|Ekle|Düzenle|Sil

		public ActionResult RevisionTypes()
		{
			if (isLogin())
			{
				if (CheckMyPermission("SUPMN_01"))
				{
					string Mesaj = TempData["ErrorMessage"] as string;
					TempData["ErrorMessage"] = null;
					ViewBag.Message = Mesaj;
					return View(db.RevisionType.ToList());
				}
				else
					return RedirectToAction("NotPermitted");
			}
			else
			{
				return RedirectToAction("Login", "Admin");
			}
		}

		public ActionResult CreateRevisionType()
		{
			if (isLogin())
			{
				if (CheckMyPermission("SUPMN_01"))
				{

					return View();
				}
				else
					return RedirectToAction("NotPermitted");
			}
			else
			{
				return RedirectToAction("Login", "Admin");
			}
		}

		[HttpPost]
		public ActionResult CreateRevisionType(RevisionType model)
		{
			if (isLogin())
			{
				if (CheckMyPermission("SUPMN_01"))
				{

					try
					{
						model.CreatedDate = System.DateTime.Now;
						model.Creator = Session["UserName"].ToString();
						db.RevisionType.Add(model);
						db.SaveChanges();
						ViewBag.Result = 1;
						return View();
					}
					catch (Exception ex)
					{
						ViewBag.Result = 0;
						return View();
					}
				}
				else
					return RedirectToAction("NotPermitted");
			}
			else
			{
				return RedirectToAction("Login", "Admin");
			}
		}

		[HttpGet]
		public ActionResult EditRevisionType(int RevisionTypeId)
		{
			if (isLogin())
			{
				if (CheckMyPermission("SUPMN_01"))
				{
					return View(db.RevisionType.Where(x => x.RevisionTypeId == RevisionTypeId).FirstOrDefault());
				}
				else
					return RedirectToAction("NotPermitted");
			}
			else
			{
				return RedirectToAction("Login", "Admin");
			}
		}

		[HttpPost]
		public ActionResult EditRevisionType(RevisionType model)
		{
			if (isLogin())
			{
				if (CheckMyPermission("SUPMN_01"))
				{
					RevisionType edited = db.RevisionType.Where(x => x.RevisionTypeId == model.RevisionTypeId).FirstOrDefault();
					edited.RevisionDesc = model.RevisionDesc;
					edited.CreatedDate = System.DateTime.Now;
					edited.Creator = Session["UserName"].ToString();
					db.SaveChanges();
					return RedirectToAction("RevisionTypes");
				}
				else
					return RedirectToAction("NotPermitted");
			}
			else
			{
				return RedirectToAction("Login", "Admin");
			}
		}

		[HttpGet]
		public ActionResult DeleteRevisionType(int RevisionTypeId)
		{
			if (isLogin())
			{
				if (CheckMyPermission("SUPMN_01"))
				{
					string error = "";
					bool result = true;
					List<SupplierParts> FirmaParcalari = db.SupplierParts.Where(x => x.RevisionTypeId == RevisionTypeId).ToList();

					if (FirmaParcalari.Count() > 0)
					{
						result = false;
						error += "Revizyon tipine bağlı firma parçası bulunmaktadır.<br/>";
					}
					else
					{
						result = true;
					}

					ViewBag.Result = result == true ? 1 : 0;

					if (result) // Servis'e bağlı bir şey bulunamadı Sil ! 
					{
						RevisionType deleted = db.RevisionType.Find(RevisionTypeId);
						db.RevisionType.Remove(deleted);
						db.SaveChanges();

					}
					TempData["ErrorMessage"] = error;

					return RedirectToAction("RevisionTypes");
				}
				else
					return RedirectToAction("NotPermitted");
			}
			else
			{
				return RedirectToAction("Login", "Admin");
			}
		}


		#endregion

		#region ParçaGözdenGeçirmeSonuç|Ekle|Düzenle|Sil

		public ActionResult PartReviewResults()
		{
			if (isLogin())
			{
				if (CheckMyPermission("SUPMN_01"))
				{
					string Mesaj = TempData["ErrorMessage"] as string;
					TempData["ErrorMessage"] = null;
					ViewBag.Message = Mesaj;
					return View(db.PartReviewResult.ToList());
				}
				else
					return RedirectToAction("NotPermitted");
			}
			else
			{
				return RedirectToAction("Login", "Admin");
			}
		}

		public ActionResult CreatePartReviewResult()
		{
			if (isLogin())
			{
				if (CheckMyPermission("SUPMN_01"))
				{

					return View();
				}
				else
					return RedirectToAction("NotPermitted");
			}
			else
			{
				return RedirectToAction("Login", "Admin");
			}
		}

		[HttpPost]
		public ActionResult CreatePartReviewResult(PartReviewResult model)
		{
			if (isLogin())
			{
				if (CheckMyPermission("SUPMN_01"))
				{

					try
					{
						model.CreatedDate = System.DateTime.Now;
						model.Creator = Session["UserName"].ToString();
						db.PartReviewResult.Add(model);
						db.SaveChanges();
						ViewBag.Result = 1;
						return View();
					}
					catch (Exception ex)
					{
						ViewBag.Result = 0;
						return View();
					}
				}
				else
					return RedirectToAction("NotPermitted");
			}
			else
			{
				return RedirectToAction("Login", "Admin");
			}
		}

		[HttpGet]
		public ActionResult EditPartReviewResult(int PartReviewResultId)
		{
			if (isLogin())
			{
				if (CheckMyPermission("SUPMN_01"))
				{
					return View(db.PartReviewResult.Where(x => x.PartReviewResultId == PartReviewResultId).FirstOrDefault());
				}
				else
					return RedirectToAction("NotPermitted");
			}
			else
			{
				return RedirectToAction("Login", "Admin");
			}
		}

		[HttpPost]
		public ActionResult EditPartReviewResult(PartReviewResult model)
		{
			if (isLogin())
			{
				if (CheckMyPermission("SUPMN_01"))
				{
					PartReviewResult edited = db.PartReviewResult.Where(x => x.PartReviewResultId == model.PartReviewResultId).FirstOrDefault();
					edited.PartReviewResultDesc = model.PartReviewResultDesc;
					edited.CreatedDate = System.DateTime.Now;
					edited.Creator = Session["UserName"].ToString();
					db.SaveChanges();
					return RedirectToAction("PartReviewResults");
				}
				else
					return RedirectToAction("NotPermitted");
			}
			else
			{
				return RedirectToAction("Login", "Admin");
			}
		}

		[HttpGet]
		public ActionResult DeletePartReviewResult(int PartReviewResultId)
		{
			if (isLogin())
			{
				if (CheckMyPermission("SUPMN_01"))
				{
					string error = "";
					bool result = true;
					List<SupplierParts> FirmaParcalari = db.SupplierParts.Where(x => x.PartReviewResultId == PartReviewResultId).ToList();

					if (FirmaParcalari.Count() > 0)
					{
						result = false;
						error += "Parça sonucuna bağlı firma parçası bulunmaktadır.<br/>";
					}
					else
					{
						result = true;
					}

					ViewBag.Result = result == true ? 1 : 0;

					if (result) // Servis'e bağlı bir şey bulunamadı Sil ! 
					{
						PartReviewResult deleted = db.PartReviewResult.Find(PartReviewResultId);
						db.PartReviewResult.Remove(deleted);
						db.SaveChanges();

					}
					TempData["ErrorMessage"] = error;

					return RedirectToAction("PartReviewResults");
				}
				else
					return RedirectToAction("NotPermitted");
			}
			else
			{
				return RedirectToAction("Login", "Admin");
			}
		}

		#endregion

		#region TedarikçiFirmaNoktasıEkle

		public ActionResult SupplierCompanies()
		{
			if (isLogin())
			{
				if (CheckMyPermission("SUPMN_01"))
				{
					string Mesaj = TempData["ErrorMessage"] as string;
					TempData["ErrorMessage"] = null;
					ViewBag.Message = Mesaj;
					return View(db.SupplierCompany.ToList());
				}
				else
					return RedirectToAction("NotPermitted");
			}
			else
			{
				return RedirectToAction("Login", "Admin");
			}

		}

		public ActionResult CreateSupplierCompany()
		{
			if (isLogin())
			{
				if (CheckMyPermission("SUPMN_01"))
				{
					ViewBag.Companies = new SelectList(db.Companies.ToList(), "CompanyId", "Name");
					return View();
				}
				else
					return RedirectToAction("NotPermitted");
			}
			else
			{
				return RedirectToAction("Login", "Admin");
			}
		}

		[HttpPost]
		public ActionResult CreateSupplierCompany(SupplierCompany model)
		{
			if (isLogin())
			{
				if (CheckMyPermission("SUPMN_01"))
				{

					try
					{
						model.CreatedDate = System.DateTime.Now;
						model.Creator = Session["UserName"].ToString();
						db.SupplierCompany.Add(model);
						db.SaveChanges();
						ViewBag.Result = 1;
						ViewBag.Companies = new SelectList(db.Companies.ToList(), "CompanyId", "Name");
						return View();
					}
					catch (Exception ex)
					{
						ViewBag.Result = 0;
						return View();
					}
				}
				else
					return RedirectToAction("NotPermitted");
			}
			else
			{
				return RedirectToAction("Login", "Admin");
			}
		}

		[HttpGet]
		public ActionResult EditSupplierCompany(int SupplierCompanyId)
		{
			if (isLogin())
			{
				if (CheckMyPermission("SUPMN_01"))
				{
					ViewBag.Companies = new SelectList(db.Companies.ToList(), "CompanyId", "Name");
					return View(db.SupplierCompany.Where(x => x.SupplierCompanyId == SupplierCompanyId).FirstOrDefault());
				}
				else
					return RedirectToAction("NotPermitted");
			}
			else
			{
				return RedirectToAction("Login", "Admin");
			}
		}

		[HttpPost]
		public ActionResult EditSupplierCompany(SupplierCompany model)
		{
			if (isLogin())
			{
				if (CheckMyPermission("SUPMN_01"))
				{
					SupplierCompany edited = db.SupplierCompany.Where(x => x.SupplierCompanyId == model.SupplierCompanyId).FirstOrDefault();
					edited.CompanyId = model.CompanyId;
					edited.CompanyDesc = model.CompanyDesc;
					edited.CreatedDate = System.DateTime.Now;
					edited.Creator = Session["UserName"].ToString();
					db.SaveChanges();
					return RedirectToAction("SupplierCompanies");
				}
				else
					return RedirectToAction("NotPermitted");
			}
			else
			{
				return RedirectToAction("Login", "Admin");
			}
		}

		[HttpGet]
		public ActionResult DeleteSupplierCompany(int SupplierCompanyId)
		{
			if (isLogin())
			{
				if (CheckMyPermission("SUPMN_01"))
				{
					string error = "";
					bool result = true;
					List<SupplierParts> FirmaParcalari = db.SupplierParts.Where(x => x.SupplierCompanyId == SupplierCompanyId).ToList();

					if (FirmaParcalari.Count() > 0)
					{
						result = false;
						error += "Bu firma noktasına bağlı parçalar bulunmaktadır.<br/>";
					}
					else
					{
						result = true;
					}

					ViewBag.Result = result == true ? 1 : 0;

					if (result) // Servis'e bağlı bir şey bulunamadı Sil ! 
					{
						SupplierCompany deleted = db.SupplierCompany.Find(SupplierCompanyId);
						db.SupplierCompany.Remove(deleted);
						db.SaveChanges();

					}
					TempData["ErrorMessage"] = error;

					return RedirectToAction("SupplierCompanies");
				}
				else
					return RedirectToAction("NotPermitted");
			}
			else
			{
				return RedirectToAction("Login", "Admin");
			}
		}


		#endregion

		#region TedarikçiParçaEklemeDüzenlemeSilme

		public ActionResult CreateSupplierPart()
		{
			if (isLogin())
			{
				if (CheckMyPermission("SUPMN_01"))
				{
					ViewBag.PartReviewResults = new SelectList(db.PartReviewResult.ToList(), "PartReviewResultId", "PartReviewResultDesc");
					ViewBag.PartReviewTypes = new SelectList(db.PartReviewType.ToList(), "PartReviewTypeId", "PartTypeDesc");
					ViewBag.Companies = new SelectList(db.Companies.ToList(), "CompanyId", "Name");
					ViewBag.Vehicles = new SelectList(db.Vehicles.ToList(), "VehicleId", "Name");
					ViewBag.RevisionTypes = new SelectList(db.RevisionType.ToList(), "RevisionTypeId", "RevisionDesc");
					ViewBag.Services = new SelectList(db.Services.ToList(), "ServiceId", "Name");
					ViewBag.SupplierCompanies = new SelectList(db.SupplierCompany.ToList() , "SupplierCompanyId", "CompanyDesc");
					return View();
				}
				else
					return RedirectToAction("NotPermitted");
			}
			else
			{
				return RedirectToAction("Login", "Admin");
			}
		}

		[HttpPost]
		public ActionResult CreateSupplierPart(SupplierParts model)
		{
			if (isLogin())
			{
				if (CheckMyPermission("SUPMN_01"))
				{
					if (model.PartReviewResultId == 4)
					{
						model.RevisionTypeId = null;
					}
					model.CreatedDate = System.DateTime.Now;
					model.Creator = Session["UserName"].ToString();

					db.SupplierParts.Add(model);
					db.SaveChanges();
					
					return RedirectToAction("SupplierParts");
				}
				else
					return RedirectToAction("NotPermitted");
			}
			else
			{
				return RedirectToAction("Login", "Admin");
			}
		}

		public ActionResult SupplierParts()
		{
			if (isLogin())
			{
				if (CheckMyPermission("SUPMN_02"))
				{
					string Mesaj = TempData["ErrorMessage"] as string;
					string HeaderMesaj = TempData["HeaderMassage"] as string;
					string IsSuccess = TempData["IsSuccess"] as string;
					TempData["ErrorMessage"] = null;
					TempData["HeaderMassage"] = null;
					TempData["IsSuccess"] = null;


					ViewBag.Message = Mesaj;
					ViewBag.HeaderMesaj = HeaderMesaj;
					ViewBag.IsSuccess = IsSuccess;

					ViewBag.Companies = new SelectList(db.Companies.ToList(), "CompanyId", "Name");
					ViewBag.SupplierParts = db.SupplierParts.ToList();
					ViewBag.PartReviewType = new SelectList(db.PartReviewType.ToList(), "PartReviewTypeId", "PartTypeDesc");
					ViewBag.PartReviewResults = new SelectList(db.PartReviewResult.ToList(), "PartReviewResultId", "PartReviewResultDesc");
					return View();
				}
				else
					return RedirectToAction("NotPermitted");
			}
			else
			{
				return RedirectToAction("Login", "Admin");
			}
		}

		[HttpPost]
		public ActionResult SupplierParts(SupplierPartsFilter filter)
		{
			if (isLogin())
			{
				if (CheckMyPermission("SUPMN_02"))
				{
					string Mesaj = TempData["ErrorMessage"] as string;
					TempData["ErrorMessage"] = null;
					ViewBag.Message = Mesaj;

					ViewBag.Companies = new SelectList(db.Companies.ToList(), "CompanyId", "Name");
					ViewBag.PartReviewType = new SelectList(db.PartReviewType.ToList(), "PartReviewTypeId", "PartTypeDesc");
					ViewBag.PartReviewResults = new SelectList(db.PartReviewResult.ToList(), "PartReviewResultId", "PartReviewResultDesc");
					bool addPartType   = !IsNullOrZero(filter.PartReviewTypeId);
					bool addPartNumber = !string.IsNullOrEmpty(filter.PartNumber);
					bool addVin		   = !string.IsNullOrEmpty(filter.VIN);
					bool addCompany	   = !IsNullOrZero(filter.CompanyId); 
					bool addVehicle	   = !IsNullOrZero(filter.VehicleId);
					bool addService    = !IsNullOrZero(filter.ServiceId);
					bool addRevisionNumber = !string.IsNullOrEmpty(filter.RevisionTrackNumber);
					bool addPartReviewResult = !IsNullOrZero(filter.PartReviewResultId);
					
					List<SupplierParts> parts = new List<SupplierParts>();

					parts = db.SupplierParts.Where(x=> (addPartType == true ? x.PartReviewTypeId == filter.PartReviewTypeId : true) &&
													   (addPartNumber == true ? x.PartNumber == filter.PartNumber: true) &&
													   (addVin == true ? x.VIN == filter.VIN : true) &&
													   (addCompany == true ? x.CompanyId == filter.CompanyId : true) &&
													   (addVehicle == true ? x.VehicleId == filter.VehicleId : true) &&
													   (addService == true ? x.ServiceId == filter.ServiceId : true) &&
													   (addPartReviewResult == true ? x.PartReviewResultId == filter.PartReviewResultId : true) &&
													   (filter.IsSend == true ? x.IsSend == true : x.IsSend ==false ) &&
													   (addRevisionNumber == true ? x.RevisionTrackNumber == filter.RevisionTrackNumber : true)).ToList();


					ViewBag.SupplierParts = parts;

					

					return View();
				}
				else
					return RedirectToAction("NotPermitted");
			}
			else
			{
				return RedirectToAction("Login", "Admin");
			}
		}

		public ActionResult EditCompanyPart(int SupplierPartId)
		{
			if (isLogin())
			{
				if (CheckMyPermission("SUPMN_02"))
				{
					SupplierParts part = db.SupplierParts.Find(SupplierPartId);
					ViewBag.PartReviewResults = new SelectList(db.PartReviewResult.ToList(), "PartReviewResultId", "PartReviewResultDesc");
					ViewBag.PartReviewTypes = new SelectList(db.PartReviewType.ToList(), "PartReviewTypeId", "PartTypeDesc");
					ViewBag.Companies = new SelectList(db.Companies.ToList(), "CompanyId", "Name");
					ViewBag.Vehicles = new SelectList(db.Vehicles.ToList(), "VehicleId", "Name");
					ViewBag.RevisionTypes = new SelectList(db.RevisionType.ToList(), "RevisionTypeId", "RevisionDesc");
					ViewBag.Services = new SelectList(db.Services.ToList(), "ServiceId", "Name");
					ViewBag.SupplierCompanies = new SelectList(db.SupplierCompany.ToList(), "SupplierCompanyId", "CompanyDesc");

					string UserName = Session["UserName"].ToString();
					ViewBag.IsManager = SFHelper.CheckMyRole(UserName, "ADMIN") || SFHelper.CheckMyRole(UserName, "YONETICI");



					return View(part);
				}
				else
					return RedirectToAction("NotPermitted");
			}
			else
			{
				return RedirectToAction("Login", "Admin");
			}
		}

		[HttpPost]
		public ActionResult EditCompanyPart(SupplierParts model)
		{
			if (isLogin())
			{
				if (CheckMyPermission("SUPMN_02"))
				{
					SupplierParts editedpart = db.SupplierParts.Find(model.SupplierPartsId);
					editedpart.ArrivalDate = model.ArrivalDate;
					editedpart.ShipmentNumber = model.ShipmentNumber;
					editedpart.KleymNumber = model.KleymNumber;
					editedpart.PartReviewResultId = model.PartReviewResultId;
					editedpart.PartReviewTypeId = model.PartReviewTypeId;
					editedpart.PartNumber = model.PartNumber;
					editedpart.VehicleId = model.VehicleId;
					editedpart.ServiceId = model.ServiceId;
					editedpart.RevisionTrackNumber = model.RevisionTrackNumber;
					if (model.PartReviewResultId == 4)
						editedpart.RevisionTypeId = null;
					else
						editedpart.RevisionTypeId = model.RevisionTypeId;
					editedpart.VehicleKm = model.VehicleKm;
					editedpart.VIN = model.VIN;
					editedpart.IsSend = model.IsSend;
					editedpart.SupplierCompanyId = model.SupplierCompanyId;
					editedpart.Creator = Session["UserName"].ToString();
					editedpart.CreatedDate = System.DateTime.Now;
					editedpart.RevmerPartDesc = model.RevmerPartDesc;
					db.SaveChanges();
					
					ViewBag.PartReviewResults = new SelectList(db.PartReviewResult.ToList(), "PartReviewResultId", "PartReviewResultDesc");
					ViewBag.PartReviewTypes = new SelectList(db.PartReviewType.ToList(), "PartReviewTypeId", "PartTypeDesc");
					ViewBag.Companies = new SelectList(db.Companies.ToList(), "CompanyId", "Name");
					ViewBag.Vehicles = new SelectList(db.Vehicles.ToList(), "VehicleId", "Name");
					ViewBag.RevisionTypes = new SelectList(db.RevisionType.ToList(), "RevisionTypeId", "RevisionDesc");
					ViewBag.Services = new SelectList(db.Services.ToList(), "ServiceId", "Name");
					ViewBag.SupplierCompanies = new SelectList(db.SupplierCompany.ToList(), "SupplierCompanyId", "CompanyDesc");
					return View(editedpart);
				}
				else
					return RedirectToAction("NotPermitted");
			}
			else
			{
				return RedirectToAction("Login", "Admin");
			}
		}

		public ActionResult CenterFiatCompanyParts()
		{
			if (isLogin())
			{
				if (CheckMyPermission("SUPMN_20"))
				{
					string Mesaj = TempData["ErrorMessage"] as string;
					TempData["ErrorMessage"] = null;
					ViewBag.Message = Mesaj;
					//int? CompanyId = 1008; // Test
					int? CompanyId = 1007; // Prod
					ViewBag.SupplierParts = db.SupplierParts.Where(x=>x.CompanyId == CompanyId && x.IsSend ==false).ToList();
					ViewBag.PartReviewType = new SelectList(db.PartReviewType.ToList(), "PartReviewTypeId", "PartTypeDesc");

					return View();
				}
				else
					return RedirectToAction("NotPermitted");
			}
			else
			{
				return RedirectToAction("Login", "Admin");
			}
		}

		[HttpPost]
		public ActionResult CenterFiatCompanyParts(FiatCenterSupplierPartsFilter filter,string command)
		{
			if (isLogin())
			{
				if (CheckMyPermission("SUPMN_20"))
				{
					string Mesaj = TempData["ErrorMessage"] as string;
					TempData["ErrorMessage"] = null;
					ViewBag.Message = Mesaj;

					ViewBag.Companies = new SelectList(db.Companies.ToList(), "CompanyId", "Name");
					ViewBag.PartReviewType = new SelectList(db.PartReviewType.ToList(), "PartReviewTypeId", "PartTypeDesc");

					//filter.CompanyId = 1008; // Test
					filter.CompanyId = 1007; // Prod
					bool addPartType = !IsNullOrZero(filter.PartReviewTypeId);
					bool addPartNumber = !string.IsNullOrEmpty(filter.PartNumber);
					bool addCompany = !IsNullOrZero(filter.CompanyId);
					bool addVehicle = !IsNullOrZero(filter.VehicleId);
					bool addService = !IsNullOrZero(filter.ServiceId);
					bool addRevisionNumber = !string.IsNullOrEmpty(filter.RevisionTrackNumber);

					List<SupplierParts> parts = new List<SupplierParts>();

					parts = db.SupplierParts.Where(x => (addPartType == true ? x.PartReviewTypeId == filter.PartReviewTypeId : true) &&
													   (addPartNumber == true ? x.PartNumber == filter.PartNumber : true) &&
													   (addCompany == true ? x.CompanyId == filter.CompanyId : true) &&
													   (addVehicle == true ? x.VehicleId == filter.VehicleId : true) &&
													   (addService == true ? x.ServiceId == filter.ServiceId : true) &&
													   (addRevisionNumber == true ? x.RevisionTrackNumber == filter.RevisionTrackNumber : true) &&
													   (filter.SwShowInBayi == true ? x.IsSend== true : true)).ToList();


					ViewBag.SupplierParts = parts;

					if (command == "Ara")
					{
						return View(filter);
					}
					else
					{
						DataTable dt = new DataTable("Grid");
						dt.Columns.AddRange(new DataColumn[] {
											new DataColumn("Giriş Tarihi"),
											new DataColumn("Parti Numarası"),
											new DataColumn("Kleym No"),
											new DataColumn("Parça İnceleme Sonucu"),
											new DataColumn("Parça Cinsi"),
											new DataColumn("Parça Numarası"),
											new DataColumn("Araç Cinsi"),
											new DataColumn("Revizyon Takip Numarası"),
											new DataColumn("Revizyon Tipi"),
											new DataColumn("Gönderilecek Bayi"),
											new DataColumn("Araç Km"),
											new DataColumn("Şase"),
											new DataColumn("Stokta Mı")
						});


						foreach (var part in parts)
						{
							dt.Rows.Add(part.ArrivalDate,
										part.ShipmentNumber,
										part.KleymNumber,
										(part.PartReviewResult != null ? part.PartReviewResult.PartReviewResultDesc : ""),
										(part.PartReviewType != null ? part.PartReviewType.PartTypeDesc: ""),
										part.PartNumber,
										(part.Vehicles != null ? part.Vehicles.Name : ""),
										part.RevisionTrackNumber,
										(part.RevisionType != null ? part.RevisionType.RevisionDesc:""),
										(part.Services != null ? part.Services.Name:""),
										part.VehicleKm,
										part.VIN,
										part.IsSend == true ? "Bayide":"Stokta"
										);
						}

						using (XLWorkbook wb = new XLWorkbook())
						{
							wb.Worksheets.Add(dt);
							using (MemoryStream stream = new MemoryStream())
							{
								wb.SaveAs(stream);
								return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Parçalar-" + System.DateTime.Now.ToShortDateString() + ".xlsx");
							}
						}
					}
					//return View();
				}
				else
					return RedirectToAction("NotPermitted");
			}
			else
			{
				return RedirectToAction("Login", "Admin");
			}
		}

		public ActionResult FiatSupplierParts()
		{
			if (isLogin())
			{
				if (CheckMyPermission("SUPMN_10"))
				{
					string Mesaj = TempData["ErrorMessage"] as string;
					TempData["ErrorMessage"] = null;
					ViewBag.Message = Mesaj;
					//int? CompanyId = 1008; // Test
					int? CompanyId = 1007; // Prod
					List<SupplierParts> parts = db.SupplierParts.Where(x=>x.PartReviewResultId == 2 && x.IsSend == false && x.CompanyId == CompanyId).ToList();
					ViewBag.PartReviewType = new SelectList(db.PartReviewType.ToList(), "PartReviewTypeId", "PartTypeDesc");


					ViewBag.SupplierParts = from p in parts
								group p by p.PartNumber into g
								select new FiatBayiViewModel { PartNumber = g.Key, SupplierParts = g.ToList() };

					return View();
				}
				else
					return RedirectToAction("NotPermitted");
			}
			else
			{
				return RedirectToAction("Login", "Admin");
			}
		}
		
		[HttpPost]
		public ActionResult FiatSupplierParts(FiatSupplierPartsFilter filter)
		{
			if (isLogin())
			{
				if (CheckMyPermission("SUPMN_10"))
				{
					string Mesaj = TempData["ErrorMessage"] as string;
					TempData["ErrorMessage"] = null;
					ViewBag.Message = Mesaj;
					ViewBag.PartReviewType = new SelectList(db.PartReviewType.ToList(), "PartReviewTypeId", "PartTypeDesc");


					//filter.CompanyId = 1008; // Test
					filter.CompanyId = 1007; // Prod

					bool addPartType   = !IsNullOrZero(filter.PartReviewTypeId);
					bool addPartNumber = !string.IsNullOrEmpty(filter.PartNumber);
					bool addCompany	   = !IsNullOrZero(filter.CompanyId); 
					bool addVehicle	   = !IsNullOrZero(filter.VehicleId);
					bool addRevisionNumber = !string.IsNullOrEmpty(filter.RevisionTrackNumber);
					
					List<SupplierParts> parts = new List<SupplierParts>();

					parts = db.SupplierParts.Where(x=> (addPartType == true ? x.PartReviewTypeId == filter.PartReviewTypeId : true) &&
													   (addPartNumber == true ? x.PartNumber == filter.PartNumber: true) &&
													   (addCompany == true ? x.CompanyId == filter.CompanyId : true) &&
													   (addVehicle == true ? x.VehicleId == filter.VehicleId : true) &&
													   (addRevisionNumber == true ? x.RevisionTrackNumber == filter.RevisionTrackNumber : true) && x.PartReviewResultId == 2 && x.IsSend == false).ToList();





					ViewBag.SupplierParts = from p in parts
											group p by p.PartNumber into g
											select new FiatBayiViewModel { PartNumber = g.Key, SupplierParts = g.ToList() };
					return View();
				}
				else
					return RedirectToAction("NotPermitted");
			}
			else
			{
				return RedirectToAction("Login", "Admin");
			}
		}

		public ActionResult DeleteSupplierPart(int id)
		{
			if (isLogin())
			{
				if (CheckMyPermission("SUPMN_02"))
				{
					SupplierParts part = db.SupplierParts.Find(id);
					
					string UserName = Session["UserName"].ToString();
					bool IsManager = SFHelper.CheckMyRole(UserName, "ADMIN") || SFHelper.CheckMyRole(UserName, "YONETICI");

					if (IsManager) // Yöneticidir silme işlemi yapabilir.
					{
						try
						{
							db.SupplierParts.Remove(part);
							db.SaveChanges();
							ViewBag.Result = 1;
							TempData["ErrorMessage"] = "Parça Silindi.";
							TempData["HeaderMassage"] = "Başarılı";
							TempData["IsSuccess"] = "w3-green";
						}
						catch (Exception ex)
						{
							new WebErrorLogsHelper().Log2Db(ex);
							TempData["ErrorMessage"] = "Parça silinirken bir hata oluştu.";
							TempData["HeaderMassage"] = "Başarılı";
							TempData["IsSuccess"] = "w3-red";
							ViewBag.Result = 0;
						}
					}
					else {
						
					}
					return RedirectToAction("SupplierParts");
				}
				else
					return RedirectToAction("NotPermitted");
			}
			else
			{
				return RedirectToAction("Login", "Admin");
			}
		}

		#endregion

		public bool IsNullOrZero(int? value)
		{
			bool result = false;
			if (value == null || value == 0)
				result = true;

			return result;
		}
	}
}