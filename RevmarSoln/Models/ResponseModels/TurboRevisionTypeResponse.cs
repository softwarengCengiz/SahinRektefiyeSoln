using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ResponseModels
{
	public class TurboRevisionTypeResponse
	{
		public int TurboRevisionTypeId { get; set; }
		public string RevisionDesc { get; set; }
		public Nullable<decimal> Price { get; set; }
		public bool SwSoketModul { get; set; }
		public bool SwGovde { get; set; }
		public bool SwMil { get; set; }
		public bool SwKompresorCarki { get; set; }
		public bool SwMajorTakim { get; set; }
		public bool SwMinorTakim { get; set; }
		public string VehicleType { get; set; }
	}
}