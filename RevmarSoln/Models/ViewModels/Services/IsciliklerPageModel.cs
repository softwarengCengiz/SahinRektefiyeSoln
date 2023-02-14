using SahinRektefiyeSoln.Models.PagingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.Services
{
	public class IsciliklerPageModel
	{
		public Iscilikler filter { get; set; }
		public IPagedList<Iscilikler> iscilikler{ get; set; }
		public int IscilikTipId { get; set; }
		public int CompanyId { get; set; }
	}
}