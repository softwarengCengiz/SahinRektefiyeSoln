using SahinRektefiyeSoln.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SahinRektefiyeSoln.Components
{
	public class SessionAuthorizationAttribute : System.Web.Mvc.AuthorizeAttribute
	{
		SahinRektefiyeDbEntities db = new SahinRektefiyeDbEntities();
		public string PermissionName { get; set; }

		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			bool skipAuthorization = filterContext.
												   ActionDescriptor.
												   IsDefined(typeof(AllowAnonymousAttribute), true)
												   || filterContext.ActionDescriptor.
													ControllerDescriptor.
												   IsDefined(typeof(AllowAnonymousAttribute), true);

			if (skipAuthorization)
			{
				return;
			}


			//if (ConfigurationManager.AppSettings["DevelopmentMode"].ToString() == "1")
			//{
			//	filterContext.HttpContext.Session["UserName"] = "admin";
			//}

			if (filterContext.HttpContext.Session["UserName"] != null)
			{
				if (!string.IsNullOrEmpty(PermissionName)) // Sayfaya Yetki Kodu ile giriliyor. Kullanıcı Role Yetkisi aratılacak.
				{
					//login olmuş arkadaş.
					string UserName = filterContext.HttpContext.Session["UserName"].ToString();

					List<UserRoles> roles = db.UserRoles.Where(x => x.Users.UserName == UserName).ToList();
					List<string> permissions = new List<string>();

					foreach (UserRoles role in roles)
					{
						List<RolePermissions> RolePermissions = db.RolePermissions.Where(x => x.RoleId == role.RoleId).ToList();

						foreach (RolePermissions perm in RolePermissions)
						{
							permissions.Add(perm.Permissions.PermissionName);
						}
					}
					bool isPermitted = false;
					string yetkisizPermission = string.Empty;
					foreach (var permName in PermissionName.Split(';'))
					{
						if (permissions.Contains(permName))
						{
							isPermitted = true;
						}
						else
						{
							yetkisizPermission += permName + " ";
						}
					}

					if (!isPermitted) // Aranan Permission Yok ise
					{
						filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Admin", action = "AccessDenied", PermName = yetkisizPermission }));
					}


				}
				else
				{ // Kullanıcının login olması yeterlidir.

				}
			}
			else
			{
				////Login olmamış arkadaş direkt login sayfasına yönlenir.

				HttpApplication app = System.Web.HttpContext.Current.ApplicationInstance;
				HttpContext _ctx = app.Context;
				//app.Context.Session.Clear();

				//string loginURL = ;

				//string esname = app.Context.Request.ServerVariables.Get("SERVER_NAME") == "localhost" ? System.Environment.MachineName + "." + "domainName" : app.Context.Request.ServerVariables.Get("SERVER_NAME");

				//string yol = _ctx.Request.Url.Scheme.ToString() + "://" + esname + app.Context.Request.ServerVariables.Get("PATH_INFO");
				string yol = string.Empty;
				if (ConfigurationManager.AppSettings["LocalDomainName"] != null)
				{
					yol = _ctx.Request.Url.Scheme + "://" + ConfigurationManager.AppSettings["LocalDomainName"] + app.Context.Request.ServerVariables.Get("PATH_INFO");
				}
				if (!string.IsNullOrEmpty(Convert.ToString(app.Context.Request.ServerVariables.Get("QUERY_STRING"))))
				{
					yol += "?" + app.Context.Request.ServerVariables.Get("QUERY_STRING");
				}


				////yol = yol + sas;
				////app.Context.Response.Redirect(loginURL + "/login.aspx?back=" + yol);
				//app.Context.Response.Redirect(loginURL + "?back=" + yol);


				if (filterContext.HttpContext.Response.RedirectLocation != "/Admin/Login")
				{
					filterContext.HttpContext.Response.Clear();

					filterContext.Result = new RedirectResult("/Admin/Login" + "?back=" + yol);
					return;
				}

			}
		}


	}
}