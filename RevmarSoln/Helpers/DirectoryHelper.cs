using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Helpers
{
	public class DirectoryHelper
	{
		public void ClearIsEmriExcelFolder()
		{
			string IsEmriExcelBasePath = Convert.ToString(ConfigurationManager.AppSettings["IsEmriExcelBasePath"]);

			string[] files = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory + IsEmriExcelBasePath  + @"\IsEmirleri"));
			foreach (string file in files)
			{
				File.Delete(file);
			}

		}
	}
}