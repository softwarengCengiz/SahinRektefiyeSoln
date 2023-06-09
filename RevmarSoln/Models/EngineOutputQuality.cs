//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SahinRektefiyeSoln.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class EngineOutputQuality
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EngineOutputQuality()
        {
            this.Talepler1 = new HashSet<Talepler>();
        }
    
        public int MotorCikisKaliteId { get; set; }
        public Nullable<int> TalepId { get; set; }
        public string KapakDolapNo { get; set; }
        public string MotorDolapNo { get; set; }
        public string BlokKrankKolIsleri { get; set; }
        public string OzelIslemler { get; set; }
        public string OzelUretimler { get; set; }
        public string GerekliParca { get; set; }
        public string GerekliParcaOlcu { get; set; }
        public string TeslimAlinanYedekParcalar { get; set; }
        public string AlinanDigerParcalar { get; set; }
        public Nullable<System.DateTime> ParcaGirisSaati { get; set; }
        public string TeslimAlan { get; set; }
        public string SilindirCaplariStdDeger { get; set; }
        public string SilindirCaplari { get; set; }
        public string GomlekYuksekligiStdDeger { get; set; }
        public string GomlekBlokYukseklikleri { get; set; }
        public string AnaMuyluDetay { get; set; }
        public string KolMuyluDetay { get; set; }
        public string PistonYukseklikleri { get; set; }
        public string BlokYuzeyiTaslamaMiktari { get; set; }
        public string AnaMuyluStdDeger { get; set; }
        public string KolMuyluStdDeger { get; set; }
        public string KrankMiliGezintiDegeri { get; set; }
        public string KrankMiliStdDeger { get; set; }
        public string PistonYuksekligiStdDeger { get; set; }
        public string ContaTipi { get; set; }
        public string Aciklama { get; set; }
    
        public virtual Talepler Talepler { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Talepler> Talepler1 { get; set; }
    }
}
