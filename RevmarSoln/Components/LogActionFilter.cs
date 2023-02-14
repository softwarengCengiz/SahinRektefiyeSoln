using SahinRektefiyeSoln.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SahinRektefiyeSoln.Components
{
	public class LogActionFilter : ActionFilterAttribute
	{
		//
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			SahinRektefiyeDbEntities db = new SahinRektefiyeDbEntities();
			
			WebUsageLogs log = new WebUsageLogs();
			log.UserName = Convert.ToString(filterContext.HttpContext.Session["UserName"]== null ? "" : filterContext.HttpContext.Session["UserName"]);
			log.Ip = SahinRektefiyeSoln.Helpers.SFHelper.GetIp();
			string url = Convert.ToString(filterContext.HttpContext.Request.Url);
			log.Url = url.Length > 510 ? url.Substring(0,510) : url;
			log.DtCreated = System.DateTime.Now;

			db.WebUsageLogs.Add(log);
			db.SaveChanges();

		}

		/// <summary>
		/// Logs the request vars.
		/// </summary>
		/// <param name="methodName">Name of the method.</param>
		/// <param name="values">The values.</param>

		private static void LogRequestVars(string methodName, ActionExecutingContext values)
		{
			var descriptor = values.ActionDescriptor;
			var actionName = descriptor.ActionName;
			var controllerName = descriptor.ControllerDescriptor.ControllerName;
			var url = values.HttpContext.Request.Url;



			// UserName  : Kim Girdi 
			// Url       : Hangi Url'e Girdi 
			// IP        : Hangi Ip ile girdi
			// DtCreated : Ne zaman Girdi.


		}
	}
}