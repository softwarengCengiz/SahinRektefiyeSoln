using SahinRektefiyeSoln.Models.PagingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.Services
{
	public class MesleklerPageModel
	{
		public Meslekler filter { get; set; }
		public IPagedList<Meslekler> meslekler{ get; set; }
	}
}