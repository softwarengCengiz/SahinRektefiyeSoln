using SahinRektefiyeSoln.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SahinRektefiyeSoln.Helpers;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data;
using SahinRektefiyeSoln.Models.FilterModels;
using ClosedXML.Excel;
using System.IO;
using System.Data.Entity.Core.Objects;
using SahinRektefiyeSoln.Models.ViewModels;
using System.Web.Script.Serialization;
using SahinRektefiyeSoln.Helpers.LogHelper;
using SahinRektefiyeSoln.Components;
using SahinRektefiyeSoln.Infrastructure;
using SahinRektefiyeSoln.Models.ViewModels.Admin;
using System.Configuration;

namespace SahinRektefiyeSoln.Controllers
{
    //[LogActionFilter]
    public class AdminController : BaseController
    {
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
        SahinRektefiyeDbEntities db = new SahinRektefiyeDbEntities();
        EncryptionHelper encryption = new EncryptionHelper();

        #region Login

        public ActionResult Login()
        {
            string yol = Convert.ToString(Request.QueryString["back"]);
            return View();
        }

        [HttpPost]
        public ActionResult Login(Users user, string back)
        {
            string encryptedpass = encryption.Encrypt(user.Password);

            var obj = db.Users.Where(a => a.UserName.Equals(user.UserName) && a.Password.Equals(encryptedpass)).FirstOrDefault();

            if (obj != null)
            {
                Session["isSideBarOpen"] = true;

                Session["UserName"] = obj.UserName.ToString();
                if (obj.ProfilePhotoUrl != null)
                {
                    Session["UserProfilePhoto"] = obj.ProfilePhotoUrl.ToString();
                }
                else { Session["UserProfilePhoto"] = null; }

                //LogHelper.Log(LogTarget.Database, new Logs() { UserName = obj.UserName.ToString(), LogMessage = obj.UserName.ToString() + " login oldu.", CreatedDate = System.DateTime.Now });

                try
                {
                    UserAgentInfos info = new UserAgentInfos();
                    info.UserName = obj.UserName.ToString();
                    info.Ip = SFHelper.GetIp();
                    info.Browser = Convert.ToString(Request.Browser.Browser);
                    info.Version = Convert.ToString(Request.Browser.Version);
                    info.DtCreated = System.DateTime.Now;
                    info.Message = obj.UserName.ToString() + " login oldu.";
                    db.UserAgentInfos.Add(info);
                    db.SaveChanges();
                }
                catch (Exception ex) { new WebErrorLogsHelper().Log2Db(ex); }

                if (!string.IsNullOrEmpty(back))
                    return Redirect(back);
                else
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

        public ActionResult SignOut()
        {
            Session["UserName"] = null;
            return RedirectToAction("Login");
        }

        #endregion

        public ActionResult EditProfileLogo(string UserName)
        {
            Users currentUser = db.Users.Where(x => x.UserName == UserName).FirstOrDefault();

            if (currentUser != null) // kullanıcı bulundu ise 
            {
                ViewBag.UserName = currentUser.UserName;
                ViewBag.ProfilePhotoUrl = currentUser.ProfilePhotoUrl;
            }
            return View();
        }

        [HttpPost]
        public ActionResult EditProfileLogo(string UserName, HttpPostedFileBase profilPhotoFile)
        {
            Users currentUser = db.Users.Where(x => x.UserName == UserName).FirstOrDefault();
            if (profilPhotoFile != null)
            {
                string silinecekImagePath = null;
                if (currentUser.ProfilePhotoUrl != null)
                {
                    silinecekImagePath = currentUser.ProfilePhotoUrl;
                }

                string pic = System.IO.Path.GetFileName(profilPhotoFile.FileName);
                Guid g = Guid.NewGuid();

                pic = g.ToString() + "_" + currentUser.UserName + "_" + pic;// System.DateTime.Now.ToShortDateString() + "_" + encryption.Encrypt(pic).Replace('/','q').ToString().Substring(1, 10) + pic;
                string path = System.IO.Path.Combine(Server.MapPath("~/Images/ProfilePhotos"), pic);
                // file is uploaded
                profilPhotoFile.SaveAs(path);

                if (silinecekImagePath != null || !string.IsNullOrEmpty(silinecekImagePath)) // eğer önceden resim yüklenmiş ise fotoyu sil.
                {
                    try
                    {
                        System.IO.File.Delete(Server.MapPath("~/Images/ProfilePhotos") + silinecekImagePath.Replace("/Images/ProfilePhotos", "").Replace("/", "\\"));
                    }
                    catch (Exception ex)
                    {
                        new WebErrorLogsHelper().Log2Db(ex);
                        throw;
                    }

                }



                //if (subDirectoryExists)
                //{
                //	Directory.Delete("/Images/ProfilePhotos/" + silinecekImagePath, true);

                //}

                currentUser.ProfilePhotoUrl = "/Images/ProfilePhotos/" + pic;
                Session["UserProfilePhoto"] = currentUser.ProfilePhotoUrl;
            }
            db.SaveChanges();


            return RedirectToAction("EditProfile");
        }

        public DataTable ReturnChartData()
        {
            using (var context = new SahinRektefiyeDbEntities())
            {
                var dt = new DataTable();
                var conn = context.Database.Connection;
                var connectionState = conn.State;
                try
                {
                    if (connectionState != ConnectionState.Open) conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "FiatPartCountByWorkType";
                        cmd.CommandType = CommandType.StoredProcedure;
                        //cmd.Parameters.Add(new SqlParameter("return_cursor",SqlDbType.));
                        using (var reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // error handling
                    new WebErrorLogsHelper().Log2Db(ex);

                }
                finally
                {
                    if (connectionState != ConnectionState.Closed) conn.Close();
                }
                return dt;
            }
        }

        public DataTable ReturnChartPart()
        {
            using (var context = new SahinRektefiyeDbEntities())
            {
                var dt = new DataTable();
                var conn = context.Database.Connection;
                var connectionState = conn.State;
                try
                {
                    if (connectionState != ConnectionState.Open) conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "FiatSeriesCountByPartNumber";
                        cmd.CommandType = CommandType.StoredProcedure;
                        //cmd.Parameters.Add(new SqlParameter("return_cursor",SqlDbType.));
                        using (var reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // error handling
                    new WebErrorLogsHelper().Log2Db(ex);
                }
                finally
                {
                    if (connectionState != ConnectionState.Closed) conn.Close();
                }
                return dt;
            }
        }

        public ActionResult ManagerIndex()
        {
            if (isLogin())
            {
                if (CheckMyRole("ADMIN") || CheckMyRole("YONETICI"))
                {
                    double ciro = Convert.ToDouble(72850.50);

                    ObjectParameter Kdvli = new System.Data.Entity.Core.Objects.ObjectParameter("Kdvli", typeof(string));
                    ObjectParameter Kdvsiz = new System.Data.Entity.Core.Objects.ObjectParameter("Kdvsiz", typeof(string));
                    ObjectParameter SadeceKdv = new System.Data.Entity.Core.Objects.ObjectParameter("SadeceKdv", typeof(string));

                    db.GetCiro(Kdvli, Kdvsiz, SadeceKdv);

                    ViewBag.Kdvli = Kdvli.Value;
                    ViewBag.Kdvsiz = Kdvsiz.Value;
                    ViewBag.SadeceKdv = SadeceKdv.Value;


                    DataTable FiatPartCountByWorkType = ReturnChartData();

                    List<ChartModel_FiatPartCountByWorkType> set = new List<ChartModel_FiatPartCountByWorkType>();

                    foreach (DataRow item in FiatPartCountByWorkType.Rows)
                    {
                        set.Add(new ChartModel_FiatPartCountByWorkType { drilldown = item["drilldown"].ToString(), name = item["name"].ToString(), y = Convert.ToInt32(item["y"]) });
                    }
                    var json = new JavaScriptSerializer().Serialize(set);

                    ViewBag.FiatPartCountByWorkType = json;

                    DataTable FiatCountPartNumber = ReturnChartPart();
                    List<ChartModel_FiatSeriesCountByPartNumber> a = new List<ChartModel_FiatSeriesCountByPartNumber>();
                    string control = "";
                    foreach (DataRow item in FiatCountPartNumber.Rows)
                    {
                        control = item["PartTypeDesc"].ToString();

                        ChartModel_FiatSeriesCountByPartNumber yenimodel = new ChartModel_FiatSeriesCountByPartNumber();
                        yenimodel.id = item["PartTypeDesc"].ToString();
                        yenimodel.name = item["PartNumber"].ToString();

                        a.Add(yenimodel);
                    }

                    return View();
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult MailBox()
        {
            string userName = Session["UserName"].ToString();
            if (SFHelper.isRevmerUser(userName))
                return View(db.Messages.OrderByDescending(x => x.CreatedDate).ToList());
            else
                return RedirectToAction("NotPermitted");

        }

        public ActionResult DeleteMail(int MessageId)
        {
            string userName = Session["UserName"].ToString();
            if (SFHelper.isRevmerUser(userName))
            {
                try
                {
                    Messages mail = db.Messages.Find(MessageId);
                    db.Messages.Remove(mail);
                    db.SaveChanges();
                    ViewBag.Result = 1;
                }
                catch (Exception ex)
                {
                    ViewBag.Result = 0;
                    new WebErrorLogsHelper().Log2Db(ex);
                }
                return View("MailBox", db.Messages.OrderByDescending(x => x.CreatedDate).ToList());


            }
            else
                return RedirectToAction("NotPermitted");
        }

        // GET: Admin
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        public ActionResult CreateUser()
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_00"))
                {
                    ViewBag.Roles = new SelectList(db.Roles.Where(x => x.RoleName != "ADMIN").ToList(), "RoleId", "RoleName");
                    return View();
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        [HttpPost]
        public ActionResult CreateUser(CreateUserPageModel model)
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_00"))
                {
                    try
                    {

                        EncryptionHelper encryption = new EncryptionHelper();
                        model.user.Password = encryption.Encrypt(model.user.UserName + "123");
                        model.user.Creator = Session["UserName"].ToString();
                        model.user.CreatedDate = System.DateTime.Now;
                        db.Users.Add(model.user);
                        db.SaveChanges();

                        if (!SFHelper.IsNullOrZero(model.RoleId))
                        {
                            //Role Eklenmeye çalışıyor.

                            UserRoles yeniRole = new UserRoles();
                            yeniRole.RoleId = model.RoleId;
                            yeniRole.UserName = model.user.UserName;

                            db.UserRoles.Add(yeniRole);
                            db.SaveChanges();
                        }

                        db.AutoCreateUserMenu(model.user.UserName);

                        ViewBag.Result = 1;

                        ViewBag.Services = new SelectList(db.Services.ToList(), "ServiceId", "Name");
                        ViewBag.Roles = new SelectList(db.Roles.Where(x => x.RoleName != "ADMIN").ToList(), "RoleId", "RoleName");

                    }
                    catch (Exception ex)
                    {
                        ViewBag.Result = 0;
                        new WebErrorLogsHelper().Log2Db(ex);
                    }
                    return View();
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        public ActionResult CreateRole()
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_01"))
                    return View();
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        [HttpPost]
        public ActionResult CreateRole(Roles newRole)
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_01"))
                {
                    try
                    {
                        newRole.CreatedDate = System.DateTime.Now;
                        newRole.Creator = Session["UserName"].ToString();
                        db.Roles.Add(newRole);
                        db.SaveChanges();

                        ViewBag.Result = 1;
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Result = 0;
                        new WebErrorLogsHelper().Log2Db(ex);
                    }

                    return View();
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        public ActionResult CreatePermission()
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_02"))
                    return View();
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public ActionResult CreatePermission(Permissions newPermission)
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_02"))
                {
                    try
                    {
                        newPermission.CreatedDate = System.DateTime.Now;
                        newPermission.Creator = Session["UserName"].ToString();
                        db.Permissions.Add(newPermission);
                        db.SaveChanges();
                        ViewBag.Result = 1;
                    }
                    catch (Exception ex)
                    {
                        new WebErrorLogsHelper().Log2Db(ex);
                        ViewBag.Result = 0;
                    }

                    return View();
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        public ActionResult CreateRolePermission()
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_03"))
                {
                    ViewBag.Roles = new SelectList(db.Roles.Where(x => x.RoleName != "ADMIN").ToList(), "RoleId", "RoleName");

                    var members = db.Permissions.ToList();
                    List<object> newList = new List<object>();
                    foreach (var perm in members)
                        newList.Add(new
                        {
                            Id = perm.PermissionId,
                            Name = perm.PermissionName + " " + perm.PermissionDesc
                        });
                    ViewBag.Permissions = new SelectList(newList, "Id", "Name");

                    //ViewBag.Permissions = new SelectList(db.Permissions.ToList(), "PermissionId", "PermissionName");
                    return View();
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        [HttpPost]
        public ActionResult CreateRolePermission(RolePermissions newRolePermission)
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_03"))
                {
                    try
                    {
                        Roles role = db.Roles.Where(x => x.RoleId == newRolePermission.RoleId).FirstOrDefault();
                        Permissions permission = db.Permissions.Where(x => x.PermissionId == newRolePermission.PermissionId).FirstOrDefault();

                        newRolePermission.Roles = role;
                        newRolePermission.Permissions = permission;

                        db.RolePermissions.Add(newRolePermission);
                        db.SaveChanges();
                        ViewBag.Roles = new SelectList(db.Roles.Where(x => x.RoleName != "ADMIN").ToList(), "RoleId", "RoleName");
                        var members = db.Permissions.ToList();
                        List<object> newList = new List<object>();
                        foreach (var perm in members)
                            newList.Add(new
                            {
                                Id = perm.PermissionId,
                                Name = perm.PermissionName + " - " + perm.PermissionDesc
                            });
                        ViewBag.Permissions = new SelectList(newList, "Id", "Name");
                        //ViewBag.Permissions = new SelectList(db.Permissions.ToList(), "PermissionId", "PermissionName");

                        ViewBag.Result = 1;
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Result = 0;
                        new WebErrorLogsHelper().Log2Db(ex);

                    }

                    return View();
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        public ActionResult CreateUserRole()
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_04"))
                {
                    ViewBag.Users = new SelectList(db.Users.Where(x => x.UserName != "admin").ToList(), "UserName", "UserName");
                    ViewBag.Roles = new SelectList(db.Roles.Where(x => x.RoleName != "ADMIN").ToList(), "RoleId", "RoleName");
                    return View();
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        [HttpPost]
        public ActionResult CreateUserRole(UserRoles newUserRole)
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_04"))
                {
                    try
                    {
                        Roles role = db.Roles.Where(x => x.RoleId == newUserRole.RoleId).FirstOrDefault();
                        Users user = db.Users.Where(x => x.UserName == newUserRole.UserName).FirstOrDefault();

                        newUserRole.Roles = role;
                        newUserRole.Users = user;

                        db.UserRoles.Add(newUserRole);
                        db.SaveChanges();

                        // Kullanıcının şuanki tüm menulerini sil 
                        List<UserMenus> usermenu = db.UserMenus.Where(x => x.UserName == newUserRole.UserName).ToList();
                        usermenu.ForEach(x => db.UserMenus.Remove(x));
                        // Kullanıcının şuanki tüm menulerini sil 
                        db.SaveChanges();
                        AutoCreateMenuForUser(newUserRole.UserName, role.RoleId);


                        ViewBag.Users = new SelectList(db.Users.Where(x => x.UserName != "admin").ToList(), "UserName", "UserName");
                        ViewBag.Roles = new SelectList(db.Roles.Where(x => x.RoleName != "ADMIN").ToList(), "RoleId", "RoleName");
                        ViewBag.Result = 1;
                    }
                    catch (Exception ex)
                    {

                        ViewBag.Users = new SelectList(db.Users.Where(x => x.UserName != "admin").ToList(), "UserName", "UserName");
                        ViewBag.Roles = new SelectList(db.Roles.ToList(), "RoleId", "RoleName");
                        ViewBag.Result = 1;
                        new WebErrorLogsHelper().Log2Db(ex);

                    }
                    return View();
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        public ActionResult CreateMenu()
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_05"))
                {
                    return View();
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        [HttpPost]
        public ActionResult CreateMenu(Menus newMenu)
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_05"))
                {
                    try
                    {
                        newMenu.CreatedDate = System.DateTime.Now;
                        newMenu.Creator = Session["UserName"].ToString();
                        db.Menus.Add(newMenu);
                        db.SaveChanges();
                        ViewBag.Result = 1;
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Result = 0;
                        new WebErrorLogsHelper().Log2Db(ex);
                    }

                    return View();
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        public ActionResult CreateUserMenu()
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_06"))
                {
                    ViewBag.Users = new SelectList(db.Users.Where(x => x.UserName != "admin").ToList(), "UserName", "UserName");
                    ViewBag.Menus = new SelectList(db.Menus.ToList(), "MenuId", "Title");
                    return View();
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        [HttpPost]
        public ActionResult CreateUserMenu(UserMenus newUserMenu)
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_06"))
                {
                    try
                    {
                        Menus menu = db.Menus.Where(x => x.MenuId == newUserMenu.MenuId).FirstOrDefault();
                        Users user = db.Users.Where(x => x.UserName == newUserMenu.UserName).FirstOrDefault();

                        newUserMenu.Menus = menu;
                        newUserMenu.Users = user;

                        db.UserMenus.Add(newUserMenu);
                        db.SaveChanges();

                        ViewBag.Users = new SelectList(db.Users.Where(x => x.UserName != "admin").ToList(), "UserName", "UserName");
                        ViewBag.Menus = new SelectList(db.Menus.ToList(), "MenuId", "Title");
                        ViewBag.Result = 1;
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Result = 0;
                        new WebErrorLogsHelper().Log2Db(ex);

                    }


                    return View();
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        public ActionResult CreateService()
        {
            if (isLogin())
            {
                if (CheckMyPermission("MNGR_01"))
                {

                    return View();
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        [HttpPost]
        public ActionResult CreateService(Services newService)
        {
            if (isLogin())
            {
                if (CheckMyPermission("MNGR_01"))
                {

                    try
                    {
                        newService.CreatedDate = System.DateTime.Now;
                        newService.Creator = Session["UserName"].ToString();
                        db.Services.Add(newService);
                        db.SaveChanges();
                        ViewBag.Result = 1;
                        return View();
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Result = 0;
                        new WebErrorLogsHelper().Log2Db(ex);
                        return View();
                    }

                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }


        }

        public ActionResult Services()
        {
            if (isLogin())
            {
                if (CheckMyPermission("MNGR_01"))
                {
                    string Mesaj = TempData["ErrorMessage"] as string;
                    TempData["ErrorMessage"] = null;
                    ViewBag.Message = Mesaj;
                    return View(db.Services.ToList());
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        [HttpPost]
        public ActionResult Services(string ServiceName)
        {
            if (isLogin())
            {
                if (CheckMyPermission("MNGR_01"))
                {

                    return View(db.Services.Where(x => (string.IsNullOrEmpty(ServiceName) == true ? true : x.Name.ToUpper().Contains(ServiceName))).ToList());
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpGet]
        public ActionResult EditService(int ServiceId)
        {
            if (isLogin())
            {
                if (CheckMyPermission("MNGR_01"))
                {
                    return View(db.Services.Where(x => x.ServiceId == ServiceId).FirstOrDefault());
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public ActionResult EditService(Services service)
        {
            if (isLogin())
            {
                if (CheckMyPermission("MNGR_01"))
                {
                    Services edited = db.Services.Where(x => x.ServiceId == service.ServiceId).FirstOrDefault();
                    edited.Name = service.Name;
                    edited.ContactName = service.ContactName;
                    edited.PhoneNumber = service.PhoneNumber;
                    edited.Address = service.Address;
                    db.SaveChanges();
                    return RedirectToAction("Services");
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //[HttpGet]
        //public ActionResult DeleteService(int ServiceId)
        //{
        //	if (isLogin())
        //	{
        //		if (CheckMyPermission("MNGR_01"))
        //		{
        //			string error = "";
        //			bool result = true;
        //			Services edited = db.Services.Where(x => x.ServiceId == ServiceId).FirstOrDefault();

        //			if (edited.CompanyServices.Count() > 0)
        //			{
        //				result = false;
        //				error += "Servis'e bağlı firma bulunmaktadır.<br/>";
        //			}
        //			if (edited.WorkOrders.Count() > 0)
        //			{
        //				result = false;
        //				error += "Servis'e bağlı iş emri bulunmaktadır.<br/>";
        //			}
        //			ViewBag.Result = result == true ? 1 : 0;

        //			if (result) // Servis'e bağlı bir şey bulunamadı Sil ! 
        //			{
        //				db.Services.Remove(edited);
        //				db.SaveChanges();

        //			}
        //			TempData["ErrorMessage"] = error;

        //			return RedirectToAction("Services");
        //		}
        //		else
        //			return RedirectToAction("NotPermitted");
        //	}
        //	else
        //	{
        //		return RedirectToAction("Login");
        //	}
        //}

        public ActionResult CreateCompany()
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_08"))
                {

                    return View();
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        [HttpPost]
        public ActionResult CreateCompany(Companies newCompany)
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_08"))
                {
                    try
                    {
                        newCompany.CreatedDate = System.DateTime.Now;
                        newCompany.Creator = Session["UserName"].ToString();
                        db.Companies.Add(newCompany);
                        db.SaveChanges();
                        ViewBag.Result = 1;
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Result = 0;
                        new WebErrorLogsHelper().Log2Db(ex);

                    }
                    return View();
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        public ActionResult EditCompany(int CompanyId)
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_08"))
                {
                    return View(db.Companies.Where(x => x.CompanyId == CompanyId).FirstOrDefault());
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public ActionResult EditCompany(Companies company)
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_08"))
                {
                    try
                    {
                        Companies editedCompany = db.Companies.Where(x => x.CompanyId == company.CompanyId).FirstOrDefault();
                        editedCompany.ContactName = company.ContactName;
                        editedCompany.Name = company.Name;
                        editedCompany.PhoneNumber = company.PhoneNumber;
                        editedCompany.ContactName = company.ContactName;
                        editedCompany.Address = company.Address;
                        db.SaveChanges();

                        ViewBag.Result = 1;
                        return RedirectToAction("Companies");
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Result = 0;
                        new WebErrorLogsHelper().Log2Db(ex);
                        return View();
                    }

                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpGet]
        public ActionResult DeleteCompany(int CompanyId)
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_08"))
                {
                    try
                    {
                        string error = "";
                        bool result = true;
                        Companies editedCompany = db.Companies.Where(x => x.CompanyId == CompanyId).FirstOrDefault();

                        if (editedCompany.Vehicles.Count() > 0)
                        {
                            result = false;
                            error += "Firmaya bağlı model bulunmaktadır.<br/>";
                        }

                        if (editedCompany.WorkOrders.Count() > 0)
                        {
                            result = false;
                            error += "Firmaya bağlı iş emri bulunmaktadır.<br/>";
                        }

                        ViewBag.Result = result == true ? 1 : 0;

                        if (result) // Firmaya bağlı bir şey bulunamadı Sil ! 
                        {
                            db.Companies.Remove(editedCompany);
                            db.SaveChanges();

                        }
                        TempData["ErrorMessage"] = error;
                        return RedirectToAction("Companies");
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Result = 0;
                        new WebErrorLogsHelper().Log2Db(ex);
                        return View();
                    }

                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult CreateServiceForCompany()
        {
            if (isLogin())
            {
                if (CheckMyPermission("MNGR_01"))
                {
                    ViewBag.Companies = new SelectList(db.Companies.ToList(), "CompanyId", "Name");
                    ViewBag.Services = new SelectList(db.Services.ToList(), "ServiceId", "Name");
                    return View();
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        //[HttpPost]
        //public ActionResult CreateServiceForCompany(CompanyServices newCompanyService)
        //{
        //	if (isLogin())
        //	{
        //		if (CheckMyPermission("MNGR_01"))
        //		{
        //			try
        //			{
        //				Services service = db.Services.Where(x => x.ServiceId == newCompanyService.ServiceId).FirstOrDefault();
        //				Companies company = db.Companies.Where(x => x.CompanyId == newCompanyService.CompanyId).FirstOrDefault();

        //				newCompanyService.Companies = company;
        //				newCompanyService.Services = service;
        //				db.CompanyServices.Add(newCompanyService);
        //				db.SaveChanges();

        //				ViewBag.Companies = new SelectList(db.Companies.ToList(), "CompanyId", "Name");
        //				ViewBag.Services = new SelectList(db.Services.ToList(), "ServiceId", "Name");
        //				ViewBag.Result = 1;
        //			}
        //			catch (Exception ex)
        //			{

        //				ViewBag.Result = 0;
        //				new WebErrorLogsHelper().Log2Db(ex);
        //			}

        //			return View();
        //		}
        //		else
        //			return RedirectToAction("NotPermitted");
        //	}
        //	else
        //	{
        //		return RedirectToAction("Login");
        //	}

        //}

        public ActionResult CreateVehicle()
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_10"))
                {
                    ViewBag.Companies = new SelectList(db.Companies.ToList(), "CompanyId", "Name");
                    return View();
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }

        }
        [HttpPost]
        public ActionResult CreateVehicle(Vehicles newVehicle)
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_10"))
                {
                    try
                    {
                        Companies company = db.Companies.Where(x => x.CompanyId == newVehicle.CompanyId).FirstOrDefault();
                        newVehicle.Companies = company;
                        newVehicle.CreatedDate = System.DateTime.Now;
                        newVehicle.Creator = Session["UserName"].ToString();
                        db.Vehicles.Add(newVehicle);
                        db.SaveChanges();

                        ViewBag.Companies = new SelectList(db.Companies.ToList(), "CompanyId", "Name");
                        ViewBag.Result = 1;
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Result = 0;
                        new WebErrorLogsHelper().Log2Db(ex);
                    }
                    return View();
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        public ActionResult UserRoles()
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_11"))
                {
                    ViewBag.Users = new SelectList(db.Users.Where(x => x.UserName != "admin").ToList(), "UserName", "UserName");
                    return View(db.UserRoles.ToList());
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        [HttpPost]
        public ActionResult UserRoles(string UserName, string RoleName)
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_11"))
                {
                    ViewBag.Users = new SelectList(db.Users.Where(x => x.UserName != "admin").ToList(), "UserName", "UserName");

                    return View(db.UserRoles.Where(x => string.IsNullOrEmpty(UserName) != true ? x.UserName.Equals(UserName) : true && x.Roles.RoleName.Contains(RoleName)).ToList());
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        public ActionResult DeleteUserRole(string UserName, int RoleId)
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_04"))
                {
                    try
                    {
                        UserRoles userRole = db.UserRoles.Where(x => x.UserName.Equals(UserName) && x.RoleId.Equals(RoleId)).FirstOrDefault();
                        db.UserRoles.Remove(userRole);
                        db.SaveChanges();

                        ViewBag.Result = 1;
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Result = 0;
                        new WebErrorLogsHelper().Log2Db(ex);
                    }
                    return RedirectToAction("UserRoles");
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        //PERMISSION : MNGR_00
        public ActionResult Companies()
        {
            if (isLogin())
            {
                if (CheckMyPermission("MNGR_00"))
                {
                    string Mesaj = TempData["ErrorMessage"] as string;
                    TempData["ErrorMessage"] = null;
                    ViewBag.Message = Mesaj;
                    return View(db.Companies.ToList());
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }

        }
        //PERMISSION : MNGR_01
        //public ActionResult DeleteCompanyService(int CompanyId, int ServiceId)
        //{
        //	if (isLogin())
        //	{
        //		if (CheckMyPermission("MNGR_01"))
        //		{
        //			try
        //			{
        //				CompanyServices service = db.CompanyServices.Where(x => x.ServiceId == ServiceId).FirstOrDefault();
        //				db.CompanyServices.Remove(service);
        //				db.SaveChanges();
        //				ViewBag.Result = 1;
        //				return RedirectToAction("Services", new { CompanyId = CompanyId, ServiceId = ServiceId });
        //			}
        //			catch (Exception ex)
        //			{
        //				ViewBag.Result = 0;
        //				new WebErrorLogsHelper().Log2Db(ex);
        //				return RedirectToAction("Services", new { CompanyId = CompanyId, ServiceId = ServiceId });
        //			}

        //		}
        //		else
        //			return RedirectToAction("NotPermitted");
        //	}
        //	else
        //	{
        //		return RedirectToAction("Login");
        //	}

        //}

        //PERMISSION : ADM_12
        public ActionResult Menus()
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_12"))
                {
                    return View(db.Menus.ToList());
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        //PERMISSION : ADM_13
        public ActionResult CreateTask()
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_13"))
                {
                    return View();
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //PERMISSION : ADM_13
        [HttpPost]
        public ActionResult CreateTask(Tasks newTask)
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_13"))
                {
                    try
                    {
                        newTask.Creator = Session["UserName"].ToString();
                        newTask.CreatedDate = System.DateTime.Now;
                        db.Tasks.Add(newTask);
                        db.SaveChanges();
                        ViewBag.Result = 1;
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Result = 0;
                    }
                    return View();
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult EditTask(int TaskId)
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_13"))
                {
                    Tasks editedTask = db.Tasks.Where(x => x.TaskId == TaskId).FirstOrDefault();
                    return View(editedTask);
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public ActionResult EditTask(Tasks editedTask)
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_13"))
                {
                    Tasks eTask = db.Tasks.Where(x => x.TaskId == editedTask.TaskId).FirstOrDefault();
                    try
                    {

                        eTask.TaskName = editedTask.TaskName;
                        eTask.PreFix = editedTask.PreFix.ToUpper().Trim();

                        db.SaveChanges();
                        ViewBag.Result = 1;
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Result = 0;
                        new WebErrorLogsHelper().Log2Db(ex);
                    }
                    return RedirectToAction("Tasks");
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult Tasks()
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_13"))
                {
                    return View(db.Tasks.ToList());
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        //PERMISSION : MNGR_01
        public ActionResult UserMenu(string UserName)
        {
            if (isLogin())
            {
                if (CheckMyPermission("MNGR_01"))
                {
                    return View(db.UserMenus.Where(x => x.UserName == UserName).ToList());
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        //PERMISSION : EMP_01
        public ActionResult CreatePart()
        {
            if (isLogin())
            {
                if (CheckMyPermission("EMP_01"))
                {
                    ViewBag.Tasks = new SelectList(db.Tasks.ToList(), "TaskId", "TaskName");
                    return View();
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //PERMISSION : EMP_01
        [HttpPost]
        public ActionResult CreatePart(Parts newPart)
        {
            if (isLogin())
            {
                if (CheckMyPermission("EMP_01"))
                {
                    try
                    {
                        newPart.CreatedBy = Session["UserName"].ToString();
                        newPart.CreatedDate = System.DateTime.Now;
                        db.Parts.Add(newPart);
                        db.SaveChanges();

                        ViewBag.Result = 1;
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Result = 0;
                        new WebErrorLogsHelper().Log2Db(ex);
                    }
                    return View();
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult CreateTamirParca()
        {
            if (isLogin())
            {
                if (CheckMyPermission("EMP_01"))
                {
                    return View();
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //PERMISSION : EMP_01
        [HttpPost]
        public ActionResult CreateTamirParca(TamirParca newPart)
        {
            if (isLogin())
            {
                if (CheckMyPermission("EMP_01"))
                {
                    try
                    {
                        newPart.CreatedBy = Session["UserName"].ToString();
                        newPart.CreatedDate = System.DateTime.Now;
                        db.TamirParca.Add(newPart);
                        db.SaveChanges();

                        ViewBag.Result = 1;
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Result = 0;
                        new WebErrorLogsHelper().Log2Db(ex);
                    }
                    return View();
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //PERMISSION : EMP_01
        public ActionResult ArizaBildirim()
        {
            if (isLogin())
            {
                if (CheckMyPermission("EMP_01"))
                {
                    string Mesaj = TempData["ErrorMessage"] as string;
                    TempData["ErrorMessage"] = null;
                    ViewBag.Message = Mesaj;
                    return View(db.ArizaBildirim.ToList());
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult CreateArizaBildirim()
        {
            if (isLogin())
            {
                if (CheckMyPermission("EMP_01"))
                {
                    return View();
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //PERMISSION : EMP_01
        [HttpPost]
        public ActionResult CreateArizaBildirim(ArizaBildirim newPart)
        {
            if (isLogin())
            {
                if (CheckMyPermission("EMP_01"))
                {
                    try
                    {
                        newPart.CreatedBy = Session["UserName"].ToString();
                        newPart.CreatedDate = System.DateTime.Now;
                        db.ArizaBildirim.Add(newPart);
                        db.SaveChanges();

                        ViewBag.Result = 1;
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Result = 0;
                        new WebErrorLogsHelper().Log2Db(ex);
                    }
                    return View();
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //PERMISSION : EMP_01
        [HttpGet]
        public ActionResult EditArizaBildirim(int arizaBildirimId)
        {
            if (isLogin())
            {
                if (CheckMyPermission("EMP_01"))
                {
                    ArizaBildirim arizaBildirim = db.ArizaBildirim.Where(x => x.ArizaBildirimId == arizaBildirimId).FirstOrDefault();
                    return View(arizaBildirim);
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //PERMISSION : EMP_01
        [HttpPost]
        public ActionResult EditArizaBildirim(ArizaBildirim edittedArizaBildirim)
        {
            if (isLogin())
            {
                if (CheckMyPermission("EMP_01"))
                {
                    try
                    {
                        ArizaBildirim arizaBildirim = db.ArizaBildirim.Where(x => x.ArizaBildirimId == edittedArizaBildirim.ArizaBildirimId).FirstOrDefault();
                        arizaBildirim.Name = edittedArizaBildirim.Name;

                        db.SaveChanges();
                        ViewBag.Result = 1;
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Result = 0;
                        new WebErrorLogsHelper().Log2Db(ex);
                    }
                    return RedirectToAction("ArizaBildirim");
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public ActionResult DeleteArizaBildirim(int id)
        {
            if (isLogin())
            {
                if (CheckMyPermission("EMP_01"))
                {
                    ArizaBildirim arizaBildirim = db.ArizaBildirim.Where(x => x.ArizaBildirimId == id).FirstOrDefault();
                    try
                    {
                        db.ArizaBildirim.Remove(arizaBildirim);
                        db.SaveChanges();
                        ViewBag.Result = 1;
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Result = 0;
                        new WebErrorLogsHelper().Log2Db(ex);
                    }
                    return RedirectToAction("ArizaBildirim");
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //PERMISSION : EMP_01
        public ActionResult Parts()
        {
            if (isLogin())
            {
                if (CheckMyPermission("EMP_01"))
                {
                    string Mesaj = TempData["ErrorMessage"] as string;
                    TempData["ErrorMessage"] = null;
                    ViewBag.Message = Mesaj;
                    return View(db.Parts.ToList());
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        //PERMISSION : EMP_01
        public ActionResult TamirParca()
        {
            if (isLogin())
            {
                if (CheckMyPermission("EMP_01"))
                {
                    string Mesaj = TempData["ErrorMessage"] as string;
                    TempData["ErrorMessage"] = null;
                    ViewBag.Message = Mesaj;
                    return View(db.TamirParca.ToList());
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpGet]
        public ActionResult EditTamirParca(int TamirParcaId)
        {
            if (isLogin())
            {
                if (CheckMyPermission("EMP_01"))
                {
                    TamirParca tamirParca = db.TamirParca.Where(x => x.TamirParcaId == TamirParcaId).FirstOrDefault();
                    //ViewBag.Tasks = new SelectList(db.Tasks.ToList(), "TaskId", "TaskName");
                    return View(tamirParca);
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //PERMISSION : EMP_01
        [HttpPost]
        public ActionResult EditTamirParca(TamirParca edittedTamirParca)
        {
            if (isLogin())
            {
                if (CheckMyPermission("EMP_01"))
                {
                    try
                    {
                        TamirParca tamirParca = db.TamirParca.Where(x => x.TamirParcaId == edittedTamirParca.TamirParcaId).FirstOrDefault();
                        tamirParca.Name = edittedTamirParca.Name;

                        db.SaveChanges();
                        ViewBag.Result = 1;
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Result = 0;
                        new WebErrorLogsHelper().Log2Db(ex);
                    }
                    return RedirectToAction("TamirParca");
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public ActionResult DeleteTamirParca(int id)
        {
            if (isLogin())
            {
                if (CheckMyPermission("EMP_01"))
                {
                    TamirParca tamirParca = db.TamirParca.Where(x => x.TamirParcaId == id).FirstOrDefault();
                    try
                    {
                        db.TamirParca.Remove(tamirParca);
                        db.SaveChanges();
                        ViewBag.Result = 1;
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Result = 0;
                        new WebErrorLogsHelper().Log2Db(ex);
                    }
                    return RedirectToAction("TamirParca");
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }


        //PERMISSION : EMP_01
        [HttpPost]
        public ActionResult Parts(string PartCode, string PartName, string command)
        {
            if (isLogin())
            {
                if (CheckMyPermission("EMP_01"))
                {
                    List<Parts> parts = db.Parts.Where(x => (string.IsNullOrEmpty(PartCode) == true ? true : x.PartCode.Contains(PartCode)) &&
                                                  (string.IsNullOrEmpty(PartName) == true ? true : x.Name.Contains(PartName))
                                                  ).ToList();
                    if (command == "Ara")
                    {
                        return View(parts);
                    }
                    else
                    {
                        DataTable dt = new DataTable("Grid");
                        dt.Columns.AddRange(new DataColumn[] {
                                            new DataColumn("Parça Kodu"),
                                            new DataColumn("Parça Adı"),
                                            new DataColumn("Alış Fiyatı"),
                                            new DataColumn("Stok"),
                                            new DataColumn("Oluşturan Kullanıcı"),
                                            new DataColumn("Oluşturma Tarihi")});


                        foreach (var part in parts)
                        {

                            dt.Rows.Add(part.PartCode,
                                        part.Name,
                                        part.CreatedBy,
                                        Convert.ToDateTime(part.CreatedDate).ToShortDateString()
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

                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }

        }


        //PERMISSION : EMP_01
        public ActionResult EditPart(int PartId)
        {
            if (isLogin())
            {
                if (CheckMyPermission("EMP_01"))
                {
                    Parts editedPart = db.Parts.Where(x => x.PartId == PartId).FirstOrDefault();
                    ViewBag.Tasks = new SelectList(db.Tasks.ToList(), "TaskId", "TaskName");
                    return View(editedPart);
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //PERMISSION : EMP_01
        [HttpPost]
        public ActionResult EditPart(Parts editedPart)
        {
            if (isLogin())
            {
                if (CheckMyPermission("EMP_01"))
                {
                    Parts ePart = db.Parts.Where(x => x.PartId == editedPart.PartId).FirstOrDefault();
                    try
                    {

                        ePart.Name = editedPart.Name;
                        ePart.PartCode = editedPart.PartCode;
                        //ePart.Stock = editedPart.Stock;
                        db.SaveChanges();
                        ViewBag.Result = 1;
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Result = 0;
                        new WebErrorLogsHelper().Log2Db(ex);
                    }
                    return RedirectToAction("Parts");
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public ActionResult DeletePart(int id)
        {
            if (isLogin())
            {
                if (CheckMyPermission("EMP_01"))
                {
                    Parts ePart = db.Parts.Where(x => x.PartId == id).FirstOrDefault();
                    try
                    {
                        db.Parts.Remove(ePart);
                        db.SaveChanges();
                        ViewBag.Result = 1;
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Result = 0;
                        new WebErrorLogsHelper().Log2Db(ex);
                    }
                    return RedirectToAction("Parts");
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //PERMISSION : MNG_03
        public ActionResult Roles()
        {
            if (isLogin())
            {
                if (CheckMyPermission("MNG_03"))
                {
                    return View(db.Roles.ToList());
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //PERMISSION : MNG_03
        public ActionResult Role(int RoleId)
        {
            if (isLogin())
            {
                if (CheckMyPermission("MNG_03"))
                {
                    Roles editedRole = db.Roles.Where(x => x.RoleId == RoleId).FirstOrDefault();
                    return View(editedRole);
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //PERMISSION : MNG_03
        [HttpPost]
        public ActionResult Role(Roles role)
        {
            if (isLogin())
            {
                if (CheckMyPermission("MNG_03"))
                {
                    Roles editedRole = db.Roles.Where(x => x.RoleId == role.RoleId).FirstOrDefault();
                    editedRole.RoleName = role.RoleName;
                    db.SaveChanges();
                    return RedirectToAction("Roles");
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //PERMISSION : MNG_03
        public ActionResult DeletePermFromRole(int RoleId, int PermId)
        {
            if (isLogin())
            {
                if (CheckMyPermission("MNG_03"))
                {
                    try
                    {
                        RolePermissions roleperm = db.RolePermissions.Where(x => x.RoleId == RoleId && x.PermissionId == PermId).FirstOrDefault();
                        db.RolePermissions.Remove(roleperm);
                        db.SaveChanges();
                        ViewBag.Result = 1;
                        return RedirectToAction("Role", new { RoleId = RoleId });
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Result = 0;
                        new WebErrorLogsHelper().Log2Db(ex);
                        return RedirectToAction("Role", new { RoleId = RoleId });
                    }

                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //PERMISSION : MNG_03
        public ActionResult DeleteRole(int RoleId)
        {
            if (isLogin())
            {
                if (CheckMyPermission("MNG_03"))
                {
                    Roles role = db.Roles.Where(x => x.RoleId == RoleId).FirstOrDefault();

                    role.RolePermissions.ToList().ForEach(x => db.RolePermissions.Remove(db.RolePermissions.Where(a => x.RolePermissionId == a.RolePermissionId).FirstOrDefault()));

                    db.Roles.Remove(role);
                    db.SaveChanges();

                    return RedirectToAction("Roles");
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //PERMISSION : ADM_00
        public ActionResult Users()
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_00"))
                {
                    ViewBag.Result = Convert.ToInt32(TempData["Result"] == null ? -1 : TempData["Result"]);
                    return View(db.Users.ToList());
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpGet]
        public ActionResult DeleteUser(string UserName)
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_00"))
                {
                    try
                    {
                        Users editedUsr = db.Users.Where(x => x.UserName == UserName).FirstOrDefault();
                        foreach (UserRoles role in editedUsr.UserRoles) // Rolleri Silinir.
                        {
                            db.UserRoles.Remove(role);
                        }

                        foreach (UserMenus menu in editedUsr.UserMenus) // Tanımlı menu silinir.
                        {
                            db.UserMenus.Remove(menu);
                        }

                        db.Users.Remove(editedUsr);
                        db.SaveChanges();
                        TempData["Result"] = 1;

                    }
                    catch (Exception ex)
                    {
                        new WebErrorLogsHelper().Log2Db(ex);
                        TempData["Result"] = 0;
                    }

                    return RedirectToAction("Users");
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //PERMISSION : ADM_00
        public ActionResult User(string UserName)
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_00"))
                {
                    Users editedUser = db.Users.Where(x => x.UserName == UserName).FirstOrDefault();
                    var userRoleId = editedUser.UserRoles.FirstOrDefault().RoleId;
                    ViewBag.Services = new SelectList(db.Services.ToList(), "ServiceId", "Name");
                    ViewBag.Roles = new SelectList(db.Roles.Where(x => x.RoleName != "ADMIN").ToList(), "RoleId", "RoleName", userRoleId);
                    return View(editedUser);
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //PERMISSION : ADM_00
        [HttpPost]
        public ActionResult User(Users user, int RoleId)
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_00"))
                {
                    Users edituser = db.Users.Where(x => x.UserName == user.UserName).FirstOrDefault();
                    edituser.FirstName = user.FirstName;
                    edituser.SurName = user.SurName;
                    edituser.PhoneNumber = user.PhoneNumber;
                    edituser.UserRoles.FirstOrDefault().RoleId = RoleId;
                    db.SaveChanges();
                    return RedirectToAction("Users");
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        ////PERMISSION : ADM_00
        //public ActionResult UserPortfolio(string UserName)
        //{
        //    if (isLogin())
        //    {
        //        if (CheckMyPermission("ADM_00"))
        //        {
        //            Users editedUser = db.Users.Where(x => x.UserName == UserName).FirstOrDefault();
        //            return View(editedUser);
        //        }
        //        else
        //            return RedirectToAction("NotPermitted");
        //    }
        //    else
        //    {
        //        return RedirectToAction("Login");
        //    }
        //}

        public ActionResult ResetUserPass(string UserName)
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_00"))
                {
                    try
                    {
                        Users editedUser = db.Users.Where(x => x.UserName == UserName).FirstOrDefault();
                        EncryptionHelper encryption = new EncryptionHelper();
                        editedUser.Password = encryption.Encrypt(editedUser.UserName + "123");
                        db.SaveChanges();
                        ViewBag.Result = 1;

                    }
                    catch (Exception ex)
                    {
                        ViewBag.Result = 0;
                        new WebErrorLogsHelper().Log2Db(ex);
                        throw;
                    }


                    return RedirectToAction("Users");
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        //PERMISSION :  ADM_10
        public ActionResult CompanyVehicles()
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_10"))
                {
                    return View(db.Vehicles.ToList());
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult DeleteModelFromCompany(int VehicleId)
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_10"))
                {
                    Vehicles vehicle = db.Vehicles.Where(x => x.VehicleId == VehicleId).FirstOrDefault();
                    db.Vehicles.Remove(vehicle);

                    db.SaveChanges();

                    return RedirectToAction("CompanyVehicles");
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //PERMISSION :  ADM_14
        public ActionResult CreateSupplier()
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_14"))
                {
                    return View();
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //PERMISSION :  ADM_14
        [HttpPost]
        public ActionResult CreateSupplier(Suppliers supplier)
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_14"))
                {
                    supplier.Creator = Session["UserName"].ToString();
                    supplier.CreatedDate = System.DateTime.Now;
                    db.Suppliers.Add(supplier);
                    db.SaveChanges();
                    return RedirectToAction("Suppliers");
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //PERMISSION :  ADM_14
        public ActionResult Suppliers()
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_14"))
                {
                    return View(db.Suppliers.ToList());
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //PERMISSION :  ADM_14
        [HttpPost]
        public ActionResult Suppliers(string SupplierCode, string SupplierName)
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_14"))
                {
                    return View(db.Suppliers.Where(x => (string.IsNullOrEmpty(SupplierCode) == true ? true : x.SupplierCode.Contains(SupplierCode)) &&
                                                        (string.IsNullOrEmpty(SupplierName) == true ? true : x.Name.Contains(SupplierName))).ToList());
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //PERMISSION :  ADM_14
        public ActionResult EditSupplier(int SupplierId)
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_14"))
                {
                    return View(db.Suppliers.Where(x => x.SupplierId == SupplierId).FirstOrDefault());
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //PERMISSION :  ADM_14
        [HttpPost]
        public ActionResult EditSupplier(Suppliers supplier)
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_14"))
                {
                    Suppliers editedSupplier = db.Suppliers.Where(x => x.SupplierId == supplier.SupplierId).FirstOrDefault();
                    editedSupplier.Name = supplier.Name;
                    editedSupplier.SupplierCode = supplier.SupplierCode;
                    editedSupplier.CompanyVNo = supplier.CompanyVNo;
                    editedSupplier.PhoneNo = supplier.PhoneNo;
                    editedSupplier.CompanyVd = supplier.CompanyVd;
                    editedSupplier.Address = supplier.Address;
                    db.SaveChanges();

                    return RedirectToAction("Suppliers");
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //PERMISSION :  ADM_14
        public ActionResult DeleteSupplier(int SupplierId)
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_14"))
                {
                    Suppliers editedSupplier = db.Suppliers.Where(x => x.SupplierId == SupplierId).FirstOrDefault();

                    //İrsaliyeParçaları Silinir 
                    db.WayBillParts.Where(x => x.WayBills.SupplierId == SupplierId).ToList().ForEach(
                        x => db.WayBillParts.Remove(x));
                    db.SaveChanges();

                    //İrsaliyeleri Sil
                    db.WayBills.Where(x => x.SupplierId == SupplierId).ToList().ForEach(
                        x => db.WayBills.Remove(x));
                    db.SaveChanges();

                    //Tedarikçi Silinir.
                    db.Suppliers.Remove(editedSupplier);
                    db.SaveChanges();

                    return RedirectToAction("Suppliers");
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //PERMISSION : ADM_15
        public ActionResult CreateBillWay()
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_15"))
                {
                    ViewBag.Suppliers = new SelectList(db.Suppliers.ToList(), "SupplierId", "Name");
                    return View();
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        //PERMISSION : ADM_15
        [HttpPost]
        public ActionResult CreateBillWay(WayBills wayBill)
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_15"))
                {
                    wayBill.WayBillIncomeDate = System.DateTime.Now;
                    wayBill.Creator = Session["UserName"].ToString();
                    wayBill.CreatedDate = System.DateTime.Now;
                    wayBill.Statu = 10;
                    db.WayBills.Add(wayBill);
                    db.SaveChanges();

                    return RedirectToAction("CreateWayBillHeader", new { WayBillId = wayBill.WayBillId });
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //PERMISSION : ADM_15
        public ActionResult CreateWayBillHeader(int WayBillId)
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_15"))
                {
                    ViewBag.Suppliers = new SelectList(db.Suppliers.ToList(), "SupplierId", "Name");
                    ViewBag.Parts = new SelectList(db.Parts.ToList(), "PartId", "Name");
                    WayBills wabill = db.WayBills.Where(x => x.WayBillId == WayBillId).FirstOrDefault();
                    return View(wabill);
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //PERMISSION : ADM_15
        [HttpPost]
        public ActionResult AddPartToWayBill(int WayBillId, int PartId, int Quantity, decimal Price)
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_15"))
                {
                    WayBillParts newPart = new WayBillParts()
                    {
                        PartId = PartId,
                        WayBillId = WayBillId,
                        Quantity = Quantity,
                        Price = Price,
                        Creator = Session["UserName"].ToString(),
                        CreatedDate = System.DateTime.Now
                    };
                    db.WayBillParts.Add(newPart);
                    db.SaveChanges();
                    return RedirectToAction("CreateWayBillHeader", new { WayBillId = WayBillId });
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //PERMISSION : ADM_15
        public ActionResult DeletePartFromWayBill(int WayBillPartId, int WayBillId)
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_15"))
                {
                    WayBillParts waybillpart = db.WayBillParts.Where(x => x.WayBillPartId == WayBillPartId).FirstOrDefault();
                    db.WayBillParts.Remove(waybillpart);
                    db.SaveChanges();

                    return RedirectToAction("CreateWayBillHeader", new { WayBillId = WayBillId });
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //PERMISSION : ADM_15
        public ActionResult UpdateWayBillStatu(int WayBillId, int WayBillStatu)
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_15"))
                {
                    WayBills waybill = db.WayBills.Where(x => x.WayBillId == WayBillId).FirstOrDefault();


                    // İrsaliye Tamamlandıdan -> Açık statüsüne geliyor. yeniden düzenlenicek 
                    // Önceden eklenen parçaları stockdan düş 
                    if (waybill.Statu == 40 && WayBillStatu == 10)
                    {
                        List<WayBillParts> irsaliyeParcalari = db.WayBillParts.Where(x => x.WayBillId == WayBillId).ToList();
                        foreach (WayBillParts parca in irsaliyeParcalari)
                        {
                            Parts part = db.Parts.Where(x => x.PartId == parca.PartId).FirstOrDefault();
                        }
                    }
                    else
                    {
                        if (WayBillStatu == 40) // İrsaliye Tamamlandığında Parçaları stock a alır.
                        {
                            List<WayBillParts> irsaliyeParcalari = db.WayBillParts.Where(x => x.WayBillId == WayBillId).ToList();

                            foreach (WayBillParts parca in irsaliyeParcalari)
                            {
                                Parts part = db.Parts.Where(x => x.PartId == parca.PartId).FirstOrDefault();
                            }
                        }
                    }
                    waybill.Statu = WayBillStatu;
                    db.SaveChanges();

                    return RedirectToAction("CreateWayBillHeader", new { WayBillId = WayBillId });
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //PERMISSION : ADM_17
        public ActionResult EditProfile()
        {
            if (isLogin())
            {
                EncryptionHelper encryption = new EncryptionHelper();
                string UserName = Session["UserName"].ToString();
                Users user = db.Users.Where(x => x.UserName == UserName).FirstOrDefault();
                user.Password = encryption.Decrypt(user.Password);
                return View(user);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //PERMISSION : ADM_17
        [HttpPost]
        public ActionResult EditProfile(Users edituser)
        {
            if (isLogin())
            {

                try
                {
                    Users user = db.Users.Where(x => x.UserName == edituser.UserName).FirstOrDefault();
                    user.FirstName = edituser.FirstName;
                    user.SurName = edituser.SurName;
                    user.PhoneNumber = edituser.PhoneNumber;

                    EncryptionHelper encryption = new EncryptionHelper();
                    user.Password = encryption.Encrypt(edituser.Password);
                    db.SaveChanges();
                    ViewBag.Result = 1;
                    return View(edituser);
                }
                catch (Exception ex)
                {
                    ViewBag.Result = 0;
                    new WebErrorLogsHelper().Log2Db(ex);
                    return View(edituser);
                }

            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //PERMISSION : ADM_15
        public ActionResult WayBills()
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_15"))
                {
                    ViewBag.Suppliers = new SelectList(db.Suppliers.ToList(), "SupplierId", "Name");
                    return View(db.WayBills.ToList());
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        //PERMISSION : ADM_15
        [HttpPost]
        public ActionResult WayBills(string WayBillNo, int? SupplierId)
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_15"))
                {
                    ViewBag.Suppliers = new SelectList(db.Suppliers.ToList(), "SupplierId", "Name");

                    return View(db.WayBills.Where(x => ((SupplierId == 0 || SupplierId == null) ? true : x.SupplierId == SupplierId) &&
                                                       (string.IsNullOrEmpty(WayBillNo) == true ? true : x.WayBillNo == WayBillNo)).ToList());
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public void AutoCreateMenuForUser(string UserName, int RoleId)
        {
            db.AutoCreateUserMenu(UserName);

            db.SaveChanges();

        }

        public ActionResult RefreshMenuForUser(string UserName)
        {
            db.AutoCreateUserMenu(UserName);
            db.SaveChanges();
            ViewBag.Result = 1;
            return View("Users", db.Users.ToList());

        }

        #region GetJsons


        public JsonResult GetSupplierInfos(int SupplierId)
        {
            //List<SelectListItem> PList = new List<SelectListItem>();
            var PList = db.Suppliers.Where(x => x.SupplierId == SupplierId)
                     .Select(i => new
                     {
                         SupplierCode = i.SupplierCode,
                         Name = i.Name.ToString(),
                         PhoneNo = i.PhoneNo.ToString(),
                         CompanyVd = i.CompanyVd,
                         CompanyVNo = i.CompanyVNo,
                         Address = i.Address
                     }).FirstOrDefault();

            return Json(PList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckPermission(string permission)
        {
            bool isPermitted = false;
            if (Session["UserName"] != null)
            {

                string userName = Session["UserName"].ToString();
                List<UserRoles> userRoles = db.UserRoles.Where(x => x.UserName == userName).ToList();

                foreach (UserRoles role in userRoles)
                {
                    var perm = db.RolePermissions.Where(x => x.RoleId == role.RoleId && x.Permissions.PermissionName == permission).FirstOrDefault();//
                    if (perm != null)
                        isPermitted = true;
                }
                return Json(new { success = isPermitted }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = isPermitted }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        // PERMISSION : ADM_21
        // Role'e yetki tanımlama sayfası 
        [HttpGet]
        public ActionResult RolePerm(int RoleId)
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_21"))
                {
                    List<CreateRolePermission> rp = new List<CreateRolePermission>();

                    db.Permissions.ToList().ForEach(x =>
                        rp.Add(new CreateRolePermission
                        {
                            RoleId = RoleId,
                            PermissionDesc = x.PermissionDesc,
                            PermissionName = x.PermissionName,
                            PermissionId = x.PermissionId,
                            ischecked = x.RolePermissions.Where(r => r.RoleId == RoleId && r.PermissionId == x.PermissionId).Count() > 0
                        })
                    );
                    ViewBag.RoleName = db.Roles.Where(x => x.RoleId == RoleId).FirstOrDefault()?.RoleName;
                    ViewBag.RoleId = RoleId;
                    return View(rp);
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public ActionResult RolePerm(FormCollection form)
        {
            if (isLogin())
            {
                if (CheckMyPermission("ADM_21"))
                {
                    // Rol bulunur.

                    // rol içerisindeki Yetkiler bulunur. 
                    string roleId = form["RoleId"].ToString();
                    List<RolePermissions> rolePermissions = db.Roles.Find(Convert.ToInt32(roleId)).RolePermissions.ToList();


















                    string[] permissionList = form["selectedPerms"].Split(',');

                    foreach (var perm in permissionList)
                    {
                        if (perm != "false")
                        {

                        }
                    }



                    return View();
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult AccessDenied(string PermName)
        {
            ViewBag.PermName = PermName;
            return View();
        }

        public ActionResult MagicScreen()
        {
            if (isLogin())
            {
                if (CheckMyRole("ADMIN"))
                {
                    return View();
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // @Html.AntiForgeryToken() view 
        public ActionResult MagicScreen(string value)
        {
            if (isLogin())
            {
                if (CheckMyRole("ADMIN"))
                {
                    if (this.userName == "admin")
                    {
                        using (var ctx = new SahinRektefiyeDbEntities())
                        {
                            try
                            {
                                if (!value.ToUpper().Contains("SELECT"))
                                {
                                    ctx.Database.ExecuteSqlCommand(value);
                                    ctx.SaveChanges();
                                }
                                else
                                {
                                    ViewBag.DbQuery = value;
                                    string connectionUrl = string.Empty;
                                    string DevelopmentMode = ConfigurationManager.AppSettings["DevelopmentMode"].ToString();
                                    if (DevelopmentMode == "1")
                                    {
                                        connectionUrl = "Server=DESKTOP-N2FKS7Q;Database=RevmarDb;Trusted_Connection=True;";

                                    }
                                    else
                                    {
                                        connectionUrl = "Server=SERVER;Database=RevmarDb;Trusted_Connection=True;User Id=Sa;Password=Logo*123;integrated security=false;MultipleActiveResultSets=True;";
                                    }
                                    // <add name="SahinRektefiyeDbEntities" connectionString="metadata=res://*/Models.RevmarModel.csdl|res://*/Models.RevmarModel.ssdl|res://*/Models.RevmarModel.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=SERVER;initial catalog=RevmarDb;User Id=Sa;Password=Logo*123;integrated security=false;MultipleActiveResultSets=True;App=EntityFramework&quot;" providerName="System.Data.EntityClient" />

                                    string SqlString = value;

                                    SqlDataAdapter sda = new SqlDataAdapter(SqlString, connectionUrl);
                                    SqlConnection Conn = new SqlConnection(connectionUrl);
                                    DataTable dt = new DataTable();
                                    try
                                    {
                                        Conn.Open();
                                        sda.Fill(dt);
                                    }
                                    catch (SqlException se)
                                    {

                                    }
                                    finally
                                    {
                                        Conn.Close();
                                    }

                                    ViewBag.datatable = dt;
                                }

                                ViewBag.CommandResult = "Query executed successfully.";

                                return View();
                            }
                            catch (Exception ex)
                            {
                                ViewBag.CommandResult = ex.Message + "<br/>" + ex.StackTrace + "<br/>" + ex.InnerException;

                            }

                        }
                    }

                    return View();
                }
                else
                    return RedirectToAction("NotPermitted");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult CreateMultipleUserRoles()
        {
            return View();
        }

        [SessionAuthorization(PermissionName = "ADM_01")]
        public ActionResult RolePermissionsList()
        {
            return View(db.Roles.ToList());
        }

        [SessionAuthorization]
        public ActionResult ErrorPage()
        {

            return View();
        }

        [SessionAuthorization]
        public ActionResult Error(Guid LogId)
        {
            WebErrorLogs error = db.WebErrorLogs.Find(LogId);

            return View(error);
        }

        [SessionAuthorization(PermissionName = "ADM_00")]
        public ActionResult Errors()
        {
            List<WebErrorLogs> errors = db.WebErrorLogs.ToList();

            return View(errors);
        }

        [SessionAuthorization(PermissionName = "ADM_00")]
        public ActionResult CheckSendMail()
        {
            return View();
        }

        [HttpPost]
        [SessionAuthorization(PermissionName = "ADM_00")]
        public ActionResult CheckSendMail(string mail, string sifre, string host, int port, string konu)
        {
            try
            {
                SFHelper.CheckMailGonder(port, host, mail, sifre, konu);
            }
            catch (Exception ex)
            {

                ViewBag.Exception = ex.InnerException;
            }


            return View();
        }
    }
}