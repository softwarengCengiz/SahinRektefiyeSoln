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
    
    public partial class StokKartiDetay
    {
        public int StokKartiDetayId { get; set; }
        public Nullable<decimal> GunlukSatisHizi { get; set; }
        public Nullable<decimal> OrtalamaMaliyet { get; set; }
        public Nullable<decimal> SonMaliyet { get; set; }
        public int StokKartiId { get; set; }
    
        public virtual StokKarti StokKarti { get; set; }
    }
}
