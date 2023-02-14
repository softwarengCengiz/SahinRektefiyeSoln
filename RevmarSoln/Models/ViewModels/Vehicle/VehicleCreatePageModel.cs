using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.Vehicle
{
	public class VehicleCreatePageModel
	{
		public Arac arac { get; set; }
		public IEnumerable<HttpPostedFileBase> files { get; set; }
		public string CompanyId { get; set; }
		public string VehicleId { get; set; }
		public int CsModelYear { get; set; }
		public int AracTipId { get; set; }
		public int VitesTipId { get; set; }
		public int AracGrupId { get; set; }
		public int YakitTipId { get; set; }
		public int AktarmaTipId { get; set; }
		public int RenkId { get; set; }
		public int VehicleMusteriId { get; set; }


	}
}