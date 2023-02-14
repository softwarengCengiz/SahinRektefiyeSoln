using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels
{
	public class ChartModel_FiatStock
	{
	}

	public class ChartFiatPartTypes
	{
		public string name { get; set; }
		public double y { get; set; }
		public string drilldown { get; set; }
	}


	public class Series
	{
		public Series()
		{
			data = new List<object[]>();
		}
		public string name { get; set; }
		public string id { get; set; }
		public List<object[]> data { get; set; }
	}

	public class ChartFiatPartDrillDown
	{
		public IList<Series> series { get; set; }
	}

	public class ChartModelBasic
	{
		public string name { get; set; }
		public int y { get; set; }
		public bool sliced { get; set; }
		public bool selected { get; set; }
	}
}