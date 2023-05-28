using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SahinRektefiyeSoln.Models.ViewModels.Ticket
{
    public class ShowTicketDetailViewModel
    {
        #region Talep Bilgisi
        public int TalepNo { get; set; }
        public int? TalepSekliId { get; set; }
        public string TalepSekli { get; set; }
        public DateTime? TalepTarihi { get; set; }
        public DateTime? MusteriAramaTarihi { get; set; }
        public DateTime? MusteriAtolyeGelisTarihi { get; set; }
        public string Sofor { get; set; }
        public string Hizmet { get; set; }
        public string Notlar { get; set; }
        #endregion

        #region Müşteri Bilgisi
        public string Musteri { get; set; }
        public string MusteriTipi { get; set; }
        public List<string> Telefon { get; set; }
        public List<string> Email { get; set; }
        public string Adres { get; set; }
        #endregion

        #region Araç Bilgisi
        public string AracGrubu { get; set; }
        public string MarkaModel { get; set; }
        public string  Plaka{ get; set; }
        public string  Vin{ get; set; }
        #endregion

        #region Kargo Bilgileri
        public DateTime? KargoyaVerilisTarihi { get; set; }
        public DateTime? AramaTarihi { get; set; }
        public string KargoFirmasi { get; set; }
        public string GonderiKodu { get; set; }
        #endregion

        #region Ekler

        #endregion

        #region Parçalar
        public IList<SelectListItemWithAttribute> Parcalar { get; set; }
        public List<string> Parca { get; set; }
        public List<string> Adet { get; set; }
        public List<string> BirimFiyatParca { get; set; }
        public List<string> ParcaIskontoParca { get; set; }
        public string GenelIskontoParca { get; set; }
        public List<string> BirimToplamFiyatParca { get; set; }
        public string ToplamFiyatParca { get; set; }
        public string KDVParca { get; set; }
        public string GenelToplamParca { get; set; }
        #endregion

        #region İşçilikler
        public IList<SelectListItemWithAttribute> Iscilikler { get; set; }
        public List<string> Iscilik { get; set; }
        public List<string> BirimFiyat { get; set; }
        public List<string> ParcaIskonto { get; set; }
        public string GenelIskonto { get; set; }
        public List<string> BirimToplamFiyat { get; set; }
        public string ToplamFiyat { get; set; }
        public string KDV { get; set; }
        public string GenelToplam { get; set; }
        #endregion

        #region Arızalar
        public IList<SelectListItem> ArizaBildirim { get; set; }
        #endregion
    }
}