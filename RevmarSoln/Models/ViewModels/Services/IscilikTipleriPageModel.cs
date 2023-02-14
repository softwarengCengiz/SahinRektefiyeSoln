using SahinRektefiyeSoln.Models.PagingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.Services
{
	public class IscilikTipleriPageModel
	{
		public IscilikTipleri filter { get; set; }
		public IPagedList<IscilikTipleri> iscilikTipleri { get; set; }
	}
}