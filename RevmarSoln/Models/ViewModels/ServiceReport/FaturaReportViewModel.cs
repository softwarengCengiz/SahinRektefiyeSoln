using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.ServiceReport
{
	public class FaturaReportViewModel
	{
		public FaturaReportViewModel()
		{
			faturalanmisIsEmirleri = new List<IsEmirleri>();
		}
		public DateTime bsgRaporTarihi { get; set; }

		public DateTime btsRaporTarihi { get; set; }

		public List<IsEmirleri> faturalanmisIsEmirleri { get; set; }
	}
}