using SahinRektefiyeSoln.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace SahinRektefiyeSoln.Helpers.LogHelper
{
	public class WebErrorLogsHelper
	{

		public void Log2Db(Exception exception )
		{
			try
			{
				var outputLines = new StringBuilder();
				SahinRektefiyeDbEntities db = new SahinRektefiyeDbEntities();

				string reqBody = GetRequestBody();

				string page = System.Web.HttpContext.Current.ApplicationInstance.Request.Url.AbsoluteUri;

				string error = exception.Message.Length > 3000 ? exception.Message.Substring(0, 3000) : exception.Message;

				string stackTrace = FlattenException(exception);

				if (exception.Message.Contains("Validation failed"))
				{
					DbEntityValidationException e = exception as DbEntityValidationException;
					

					foreach (var eve in e.EntityValidationErrors)
					{
						outputLines.AppendFormat("{0}: Entity of type {1} in state {2} has the following validation errors:" , DateTime.Now, eve.Entry.Entity.GetType().Name, eve.Entry);

						foreach (var ve in eve.ValidationErrors)
						{
							outputLines.AppendFormat("- Property: {0}, Error: {1}" , ve.PropertyName, ve.ErrorMessage);
						}
					}

				}

				WebErrorLogs newError = new WebErrorLogs();
				newError.IdLog = Guid.NewGuid();

				newError.UserName = System.Web.HttpContext.Current.Session["UserName"].ToString();
				newError.DtLog = System.DateTime.Now;
				newError.Page = page;
				newError.Error = error;
				newError.StackTrace = outputLines.ToString() + "--------"+ stackTrace;
				newError.Parameters = System.Web.HttpContext.Current.Request.Form.ToString();
				newError.FullUrl = System.Web.HttpContext.Current.Request.RawUrl;

				db.WebErrorLogs.Add(newError);
				db.SaveChanges();
			}
			catch (Exception ex)
			{


			}


		}

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

		public static string GetRequestBody()
		{
			var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
			bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
			var bodyText = bodyStream.ReadToEnd();
			return bodyText;
		}



	}
}