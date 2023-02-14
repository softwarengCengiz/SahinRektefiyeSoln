using SahinRektefiyeSoln.Models.PagingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.Services
{
	public class RenklerPageModel
	{
		public Renkler filter { get; set; }
		public IPagedList<Renkler> renkler{ get; set; }

	}
}