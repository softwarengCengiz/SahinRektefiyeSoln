using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SahinRektefiyeSoln.Models.ViewModels.Ticket
{
    public class EngineOutputQualityViewModel
    {
        public int MotorCikisKaliteId { get; set; }
        public int TalepId { get; set; }
        public string KapakDolapNo { get; set; }
        public string MotorDolapNo { get; set; }
        public IList<string> BlokKrankKolIsleri { get; set; }
        public IList<string> OzelIslemler { get; set; }
        public IList<string> OzelUretimler { get; set; }
        public IList<string> GerekliParca { get; set; }
        public IList<string> GerekliParcaOlcu { get; set; }
        public List<List<string>> TeslimAlinanYedekParcalar { get; set; }
        public string AlinanDigerParcalar { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> ParcaGirisSaati { get; set; }
        public string TeslimAlan { get; set; }
        public string SilindirCaplariStdDeger { get; set; }
        public List<List<string>> SilindirCaplari { get; set; }
        public string GomlekYuksekligiStdDeger { get; set; }
        public List<List<string>> GomlekBlokYukseklikleri { get; set; }
        public List<List<string>> AnaMuyluDetay { get; set; }
        public List<List<string>> KolMuyluDetay { get; set; }
        public List<List<string>> PistonYukseklikleri { get; set; }
        public string BlokYuzeyiTaslamaMiktari { get; set; }
        public string AnaMuyluStdDeger { get; set; }
        public string KolMuyluStdDeger { get; set; }
        public string KrankMiliGezintiDegeri { get; set; }
        public string KrankMiliStdDeger { get; set; }
        public string PistonYuksekligiStdDeger { get; set; }
        public string ContaTipi { get; set; }
        public string Aciklama { get; set; }
        //public IList<string> GerekliParcaText { get; set; }
        public IList<SelectListItemWithAttribute> EngineInfoDet { get; set; }
    }
}