using PagedList;

namespace SahinRektefiyeSoln.Models.ViewModels.Stok
{
	public class StokKartiTanimlariV2ViewModel
	{
		public IPagedList<SahinRektefiyeSoln.Models.StokKarti> stokKartlari { get; set; }
		public int? Page { get; set; }

		public string ParcaNo { get; set; }
		public string ParcaAdi { get; set; }
		public string command { get; set; }
	}
}