using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels
{
	public class HizliAramaViewModel
	{
		public HizliAramaViewModel()
		{
			isEmirleri = new List<IsEmirleri>();
			servisAraclar = new List<Arac>();
			revizyonKayitlari = new List<WorkOrders>();
		}
		public string plakaVeyaSase { get; set; }
		public List<IsEmirleri> isEmirleri { get; set; }
		public List<Arac> servisAraclar { get; set; }
		public List<WorkOrders> revizyonKayitlari { get; set; }
										 
	}

	public class FordRecord
	{
		public string Barkod { get; set; }

		public string PartiNo { get; set; }

		public string RevizyonTakipNumarasi { get; set; }

		public bool swRevizyon { get; set; }

		public string Aciklama { get; set; }

		public string Nedeni { get; set; }

		public int RevizyonTipi { get; set; }

		public string RevmerFaturaNo { get; set; }

	}
}