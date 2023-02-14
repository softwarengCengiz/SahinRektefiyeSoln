using SahinRektefiyeSoln.Models.PagingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.Services
{
	public class YakitTipleriPageModel
	{
		public YakitTipi filter { get; set; }
		public IPagedList<YakitTipi> yakitTipleri{ get; set; }
	}
}