using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.Stok
{
	public class TopluIndirimEkraniPageModel
	{
		public int ParcaTipId { get; set; }
		public List<StokKarti> stokKartlari { get; set; }
		public double IndirimOrani { get; set; }
	}
}