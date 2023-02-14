using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ResponseModels
{
	public class DireksiyonRevisionTypeResponse
	{
		public int DireksiyonRevisionTypeId { get; set; }
		public string RevisionDesc { get; set; }
		public Nullable<decimal> Price { get; set; }
		public bool SwKeceler { get; set; }
		public bool SwBurclar { get; set; }
		public bool SwKoruk { get; set; }
		public bool SwRotKolu { get; set; }
		public bool SwRotBasi { get; set; }
		public bool SwPiston { get; set; }
		public bool SwMil { get; set; }
		public bool SwUstBayin { get; set; }
		public bool SwKovan { get; set; }
		public bool SwBilya { get; set; }
		public bool SwKayis { get; set; }
		public bool SwYazilim { get; set; }

	}
}