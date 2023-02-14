using SahinRektefiyeSoln.Models.PagingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.Services
{
	public class DanismanlarPageModel
	{
		public Danismanlar filter { get; set; }
		public IPagedList<Danismanlar> danismanlar { get; set; }
	}
}