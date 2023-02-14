using PagedList;

namespace SahinRektefiyeSoln.Models.ViewModels.IsEmri
{
	public class IsEmriParcaEkleViewModel
	{
		public IPagedList<SahinRektefiyeSoln.Models.StokKarti> parcalar { get; set; }
		public StokKarti filter { get; set; }
		public int? Page { get; set; }


		public int StokKartiId { get; set; }
		public int? ParcaTipiId { get; set; }
		public string ParcaNo { get; set; }
		public string ParcaAdi { get; set; }
		public decimal ToplamStok { get; set; }
		public decimal AtolyeStok { get; set; }
		public decimal SatisBirimFiyati { get; set; }
		public int Kdv { get; set; }
		public int ParcaTipId { get; set; }
		public string Creator { get; set; }
		public string Modifier { get; set; }
	}
}