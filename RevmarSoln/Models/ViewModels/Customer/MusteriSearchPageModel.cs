using SahinRektefiyeSoln.Models.PagingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.Customer
{
	public class MusteriSearchPageModel
	{
		public Musteri filter { get; set; }
		public IPagedList<Musteri> musteriler{ get; set; }
	}
}