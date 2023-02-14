using SahinRektefiyeSoln.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Helpers.LogHelper
{
	public enum LogTarget
	{
		File, Database, EventLog
	}

	public static class LogHelper
	{
		private static LogBase logger = null;
		public static void Log(LogTarget target, Logs message)
		{
			switch (target)
			{
				case LogTarget.File:
					logger = new FileLogger();
					logger.Log(message);
					break;
				case LogTarget.Database:
					logger = new DBLogger();
					logger.Log(message);
					break;
				case LogTarget.EventLog:
					logger = new EventLogger();
					logger.Log(message);
					break;
				default:
					return;
			}
		}
	}
}