using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.StokKartiDuzenle
{
	public class StokKartiDuzenlePageModel
	{
		public StokKarti model { get; set; }
		public StokKartiDetay detay { get; set; }
		public string Kdv { get; set; }
		public string ParcaTipId { get; set; }

		public List<IsEmriKalemleri> KimeSattim { get; set; }
	}
}