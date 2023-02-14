using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels
{
	public class ChartModel_FiatSeriesCountByPartNumber
	{
		public ChartModel_FiatSeriesCountByPartNumber()
		{
			data = new Dictionary<string, int>();
		}
		public string name { get; set; }
		public string id { get; set; }
		public Dictionary<string, int> data { get; set; }
	}
}