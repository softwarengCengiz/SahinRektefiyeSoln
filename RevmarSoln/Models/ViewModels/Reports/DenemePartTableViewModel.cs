using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace SahinRektefiyeSoln.Models.ViewModels.Reports
{
	public class DenemePartTableViewModel
	{
		public IPagedList<SahinRektefiyeSoln.Models.StokKarti> parcalar { get; set; }
		public StokKarti filter { get; set; }
		public int? Page { get; set; }

		public int StokKartiId { get; set; }
		public string ParcaNo { get; set; }
		public string ParcaAdi { get; set; }
		public int EnAzSatisAdedi { get; set; }
		public decimal ToplamStok { get; set; }
		public decimal AtolyeStok { get; set; }
		public decimal SatisBirimFiyati { get; set; }
		public int Kdv { get; set; }
		public int ParcaTipId { get; set; }

	}
}