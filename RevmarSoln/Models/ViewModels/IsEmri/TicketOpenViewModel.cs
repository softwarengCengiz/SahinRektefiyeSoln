using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahinRektefiyeSoln.Models.ViewModels.IsEmri
{
	public class TicketOpenViewModel
    {
        public int Id { get; set; }
        public int HizliIsEmriVehicleMusteriId { get; set; }
        public string MusteriTipi { get; set; }
		public string MusteriAdi { get; set; }
		public string MusteriSoyadi { get; set; }
		public string Telefon { get; set; }
		public string Adres { get; set; }
		public string ArayanKisiAdSoyad { get; set; }
		public string SaseNo { get; set; }
        public string Plate { get; set; }
        public int KM { get; set; }
		public int AracGrubuId { get; set; }
        public int PartId { get; set; }
        public int VehicleId { get; set; }
        public Nullable<System.DateTime> KayitTarihi { get; set; }
        public Nullable<System.DateTime> MusteriAramaTarihi { get; set; }
        public string Not { get; set; }

		public string SoforUserName { get; set; }
	}
}