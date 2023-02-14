using SahinRektefiyeSoln.Models.PagingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.Vehicle
{
	public class VehicleSearchPageModel
	{
		public Arac filter { get; set; }
		public IPagedList<Arac> araclar { get; set; }
		

	}
}