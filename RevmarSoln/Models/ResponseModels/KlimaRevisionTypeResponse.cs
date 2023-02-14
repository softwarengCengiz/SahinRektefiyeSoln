using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ResponseModels
{
	public class KlimaRevisionTypeResponse
	{
		public int TurboRevisionTypeId { get; set; }
		public string RevisionDesc { get; set; }
		public Nullable<decimal> Price { get; set; }
		public bool SwOring { get; set; }
		public bool SwKavramaRulmani { get; set; }
		public bool SwBasincValfi { get; set; }
		public bool SwKasnak { get; set; }
		public bool SwKompleBobinSet { get; set; }

	}
}