using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SahinRektefiyeSoln.Models.ViewModels.Ticket
{
    public class CapInputQualityViewModel
    {
        public int KapakGirisKaliteId { get; set; }
        public int TalepId { get; set; }
        public string KapakDolapNo { get; set; }
        public string MotorDolapNo { get; set; }
        public string IzınVerilenMinKapakKalinligi { get; set; }
        public string IzınVerilenMaxKapakEgriligi { get; set; }
        public string OlculenKapakKalinligi { get; set; }
        public string OlculenKapakEgriligi { get; set; }
        public IList<string> IncelemeSonuclari { get; set; }
        public IList<string> YapilacakIslemler { get; set; }
        public IList<string> OzelIslemler { get; set; }
        public IList<string> OzelUretimler { get; set; }
        public IList<string> GerekliParcalar { get; set; }
        public IList<string> GerekliParcaAdet { get; set; }
        public string GerekliParcaYorumlar { get; set; }
        public List<List<string>> YedekParcaGirisKontrol { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> ParcaGirisTarihi { get; set; }
        public string TeslimAlan { get; set; }
        public List<List<string>> SupapDusukluguYuksekligi { get; set; }
        public string GerceklestirilenTasmaMiktari { get; set; }
        public string RevizyonSonrasiKapakKalinligi { get; set; }
        public string SupapAyariEmme { get; set; }
        public string SupapAyariEgzoz { get; set; }
        public string YatakSikmaTorku1 { get; set; }
        public string YatakSikmaTorku2 { get; set; }
        public string YatakSikmaTorku3 { get; set; }
        public string YatakSikmaTorku4 { get; set; }
        public string KapakSikmaTorku1 { get; set; }
        public string KapakSikmaTorku2 { get; set; }
        public string KapakSikmaTorku3 { get; set; }
        public string KapakSikmaTorku4 { get; set; }
        public IList<string> YapilanIsler { get; set; }
        public string Aciklama { get; set; }
        public IList<SelectListItemWithAttribute> EngineInfoDet { get; set; }
    }
}