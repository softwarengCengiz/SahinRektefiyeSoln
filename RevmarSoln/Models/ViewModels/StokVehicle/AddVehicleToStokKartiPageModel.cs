using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.StokVehicle
{
	public class AddVehicleToStokKartiPageModel
	{
		public StokKarti StokKarti { get; set; }
		public int StokKartiId { get; set; }
		public int VehicleIds { get; set; }
	}
}