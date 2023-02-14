using SahinRektefiyeSoln.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Helpers.LogHelper
{
	public abstract class LogBase
	{
		protected readonly object lockObj = new object();
		public abstract void Log(Logs log);
	}

	public class FileLogger : LogBase
	{
		public string filePath = ConfigurationManager.AppSettings["LogPath"].ToString();//@"D:\IDGLog.txt";

		
		public override void Log(Logs log)
		{
			string now = (log.CreatedDate.Value.ToShortDateString() + " " + log.CreatedDate.Value.ToShortTimeString() + ":"+ log.CreatedDate.Value.Second.ToString("D2")).PadRight(25);

			lock (lockObj)
			{
				//using (StreamWriter streamWriter = new StreamWriter(filePath))
				//{
				//	streamWriter.WriteLine(log.LogMessage);
				//	streamWriter.Close();
				//}

				using (StreamWriter sw = File.AppendText(filePath))
				{
					sw.WriteLine(now + log.LogMessage);
				}
			}
		}
	}

	public class EventLogger : LogBase
	{
		public override void Log(Logs log)
		{
			lock (lockObj)
			{
				EventLog m_EventLog = new EventLog("");
				m_EventLog.Source = "IDGEventLog";
				m_EventLog.WriteEntry(log.LogMessage);
			}
		}
	}

	public class DBLogger : LogBase
	{
		public override void Log(Logs log)
		{
			lock (lockObj)
			{
				using (SahinRektefiyeDbEntities db = new SahinRektefiyeDbEntities())
				{
					db.Logs.Add(log);
					db.SaveChanges();
				}
			
				//Code to log data to the database
			}
		}
	}
}