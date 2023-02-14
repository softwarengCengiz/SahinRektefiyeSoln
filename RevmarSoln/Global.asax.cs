using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SahinRektefiyeSoln
{
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			RouteConfig.RegisterRoutes(RouteTable.Routes);
		}

		private static readonly List<string> _sessions = new List<string>();
		private static readonly object padlock = new object();

		public static List<string> Sessions
		{
			get
			{
				return _sessions;
			}
		}

		protected void Session_Start(object sender, EventArgs e)
		{
			lock (padlock)
			{
				_sessions.Add(Session.SessionID);
			}
		}
		protected void Session_End(object sender, EventArgs e)
		{
			lock (padlock)
			{
				_sessions.Remove(Session.SessionID);
			}
		}
	}
}
