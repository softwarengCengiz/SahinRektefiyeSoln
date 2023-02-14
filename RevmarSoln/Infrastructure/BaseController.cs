using SahinRektefiyeSoln.Helpers;
using SahinRektefiyeSoln.Models;
using SahinRektefiyeSoln.Models.HelperModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SahinRektefiyeSoln.Infrastructure
{
	public class BaseController : Controller
	{
		public string BaseUrl
		{
			get
			{
				return ConfigurationManager.AppSettings["LocalDomainName"].ToString();
			}
			set { }
		}
		public string userName { get { return Session["UserName"].ToString(); } }

		public bool isLogin()
		{
			if (Session["UserName"] != null)
				return true;
			else return false;
		}

		protected override void OnException(ExceptionContext filterContext)
		{
			var outputLines = new StringBuilder();
			filterContext.ExceptionHandled = true;

			SahinRektefiyeDbEntities db = new SahinRektefiyeDbEntities();

			string page = System.Web.HttpContext.Current.ApplicationInstance.Request.Url.AbsoluteUri;

			string error = filterContext.Exception.Message.Length > 3000 ? filterContext.Exception.Message.Substring(0, 3000) : filterContext.Exception.Message;

			string stackTrace = FlattenException(filterContext.Exception);//.StackTrace.Length > 3000 ? filterContext.Exception.StackTrace.Substring(0, 3000) : filterContext.Exception.StackTrace;
																		  //string stackTrace = GetExceptionMessages(filterContext.Exception);//.StackTrace.Length > 3000 ? filterContext.Exception.StackTrace.Substring(0, 3000) : filterContext.Exception.StackTrace;

			string urlWithParameters = string.Empty;
			try
			{
				urlWithParameters = filterContext.HttpContext.Request.RawUrl;
			}
			catch { }

			if (filterContext.Exception.Message.Contains("Validation failed"))
			{
				DbEntityValidationException e = filterContext.Exception as DbEntityValidationException;


				foreach (var eve in e.EntityValidationErrors)
				{
					outputLines.AppendFormat("{0}: Entity of type {1} in state {2} has the following validation errors:", DateTime.Now, eve.Entry.Entity.GetType().Name, eve.Entry);

					foreach (var ve in eve.ValidationErrors)
					{
						outputLines.AppendFormat("- Property: {0}, Error: {1}", ve.PropertyName, ve.ErrorMessage);
					}
				}

			}
			string Parameters = string.Empty;
			string FullUrl = string.Empty;
			try
			{
				Parameters = System.Web.HttpContext.Current.Request.Form.ToString();
			}
			catch { }
			try
			{
				FullUrl = System.Web.HttpContext.Current.Request.RawUrl;
			}
			catch { }

			Guid guidId = Guid.NewGuid();

			WebErrorLogs log = new WebErrorLogs()
			{
				IdLog = guidId,
				UserName = this.userName,
				DtLog = System.DateTime.Now,
				Page = page,
				Error = error,
				StackTrace = outputLines.ToString() + "---------" + stackTrace,
				Parameters = Parameters,
				FullUrl = FullUrl
			};

			db.WebErrorLogs.Add(log);
			db.SaveChanges();

			try
			{
				ExceptionLog ex = new ExceptionLog();

				ex.StackTrace = outputLines.ToString() + stackTrace;
				ex.InnerMessage = error;
				ex.Parameters = "";
				ex.Message = page + " sayfasından hata!";

				SFHelper.MailGonder(ex, "Web Error Log");
			}
			catch
			{

			}


			var result = new PartialViewResult();
			result.ViewName = "~/Views/Admin/ErrorPage.cshtml";
			result.ViewBag.ErrorId = log.IdLog;
			result.ViewBag.StackTrace = stackTrace;
			result.ViewBag.Error = error;
			filterContext.Result = result;


		}

		//private string GetExceptionMessages(Exception e, string msgs = "")
		//{
		//	if (e == null) return string.Empty;
		//	if (msgs == "") msgs = e.Message;
		//	if (e.InnerException != null)
		//		msgs += "\r\nInnerException: " + GetExceptionMessages(e.InnerException);
		//	return msgs;
		//}

		public string FlattenException(Exception exception)
		{
			var stringBuilder = new StringBuilder();

			while (exception != null)
			{
				stringBuilder.AppendLine(exception.Message);
				stringBuilder.AppendLine(exception.StackTrace);

				exception = exception.InnerException;
			}

			return stringBuilder.ToString();
		}


	}
}