using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.Reports
{
	public class DenemeTableViewModel
	{
		public IPagedList<SahinRektefiyeSoln.Models.Users> kullanicilar { get; set; }
		public Users filter { get; set; }
		public int? Page { get; set; }
	}
}