using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.Stok
{
	public class ParcaAlternatifPageModel
	{
		public StokKarti stokKarti { get; set; }
		public List<StokKartiAlternatif> alternatifStokKartlari { get; set; }
		public List<StokKarti> secilebilecekStokKartlari { get; set; }
	}
}