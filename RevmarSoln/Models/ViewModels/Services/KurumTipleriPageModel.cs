using SahinRektefiyeSoln.Models.PagingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.Services
{
	public class KurumTipleriPageModel
	{
		public KurumTipleri filter { get; set; }
		public IPagedList<KurumTipleri> kurumTipleri{ get; set; }
	}
}