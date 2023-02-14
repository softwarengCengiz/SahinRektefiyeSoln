using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ResponseModels
{
	public class DpfRevisionTypeResponse
	{
		public int DpfRevisionTypeId { get; set; }
		public string RevisionDesc { get; set; }
		public Nullable<decimal> Price { get; set; }
		public bool SwSoketModul { get; set; }
		public bool SwManifolt { get; set; }
		public bool SwMekanikValf { get; set; }
		public bool SwElektronikValf { get; set; }
	}
}