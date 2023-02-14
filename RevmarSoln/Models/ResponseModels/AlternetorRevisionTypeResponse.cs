using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ResponseModels
{
	public class AlternetorRevisionTypeResponse
	{
		public int AlternatorRevisionTypeId { get; set; }
		public string AlternatorDesc { get; set; }
		public Nullable<decimal> Price { get; set; }
		public bool SwRotorBakiri { get; set; }
		public bool SwRulman { get; set; }
		public bool SwRulmanYatagi { get; set; }
		public bool SwKasnak { get; set; }
		public bool SwKonjektor { get; set; }
		public string VehicleType { get; set; }
	}
}