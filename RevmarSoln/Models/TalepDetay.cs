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
    
    public partial class TalepDetay
    {
        public int TalepDetayId { get; set; }
        public int TalepId { get; set; }
        public string MotorDolapNo { get; set; }
        public string KapakDolapNo { get; set; }
        public Nullable<System.DateTime> BildirimTarihi { get; set; }
        public string ServisAdı { get; set; }
        public string Marka { get; set; }
        public string Model { get; set; }
        public string MotorTipi { get; set; }
        public Nullable<int> YakıtTipi { get; set; }
        public Nullable<int> SilindirSayisi { get; set; }
        public Nullable<int> Garanti { get; set; }
        public Nullable<int> Revizyon { get; set; }
        public string RevizyonAciklama { get; set; }
        public string ServisNo { get; set; }
        public Nullable<int> AlınanIs { get; set; }
        public string Plaka { get; set; }
        public Nullable<int> KM { get; set; }
        public string VinNo { get; set; }
        public string MotorNo { get; set; }
        public string SupapSayisi { get; set; }
        public string MusteriNot { get; set; }
        public string ArizaDiger { get; set; }
        public string ParcaDiger { get; set; }
        public string ParcaAdet { get; set; }
        public string ArizaList { get; set; }
        public string ParcaList { get; set; }
        public string ParcaListAdet { get; set; }
        public string ParcaListId { get; set; }
        public Nullable<int> IsLogoEnable { get; set; }
    
        public virtual Talepler Talepler { get; set; }
    }
}
