using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ResponseModels
{
	public class MarsMotorRevisionTypeResponse
	{
		public int TurboRevisionTypeId { get; set; }
		public string RevisionDesc { get; set; }
		public Nullable<decimal> Price { get; set; }
		public bool SwKomur { get; set; }
		public bool SwBurcRulman { get; set; }
		public bool SwMarsDislisi { get; set; }
		public bool SwMarsOtomatigi { get; set; }
		public bool SwYardimciOtomatik { get; set; }
		public string VehicleType { get; set; }

	}
}