using SahinRektefiyeSoln.Models.PagingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.Services
{
	public class AktarmaTipleriPageModel
	{
		public AktarmaTipi filter { get; set; }
		public IPagedList<AktarmaTipi> aktarmaTipleri{ get; set; }
	}
}