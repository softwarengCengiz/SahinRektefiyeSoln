using SahinRektefiyeSoln.Models.PagingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.Services
{
	public class IsEmriTipleriPageModel
	{
		public IsEmriTipleri filter { get; set; }
		public IPagedList<IsEmriTipleri> isEmriTipleri{ get; set; }
	}
}