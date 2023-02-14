using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.Stok
{
	public class StokKartıViewModel
	{
		public StokKartıViewModel()
		{
			stok = new StokKarti();
		}
		public StokKarti stok { get; set; }
		public string DepoIds { get; set; }
		public string VehicleIds { get; set; }
		public string ParcaTipId { get; set; }
		public string Kdv { get; set; }
	}
}