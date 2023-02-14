using SahinRektefiyeSoln.Models.PagingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.Services
{
	public class PaketlerPageModel
	{
		public Paketler filter { get; set; }
		public IPagedList<Paketler> paketler { get; set; }

	}
}