namespace SahinRektefiyeSoln.Models.ViewModels.Teklif
{
    public class TeklifOlusturPageModel
    {
		public Musteri musteri { get; set; }
		public Arac arac { get; set; }
		public IsEmirleri isEmri { get; set; }

		public int IsEmriTipId { get; set; }
		public int VehicleMusteriId { get; set; }

	}
}