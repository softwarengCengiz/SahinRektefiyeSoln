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
    
    public partial class IsEmriBelirtiKodlari
    {
        public int IsEmriBelirtiKodlariId { get; set; }
        public int IsEmriId { get; set; }
        public int IsEmriTipId { get; set; }
        public string IsEmriTipAciklamasi { get; set; }
        public string SeriNo { get; set; }
        public string PartNo { get; set; }
        public Nullable<System.DateTime> BelirtiKodTarihi { get; set; }
        public string Notlar { get; set; }
    
        public virtual IsEmirleri IsEmirleri { get; set; }
        public virtual IsEmriTipleri IsEmriTipleri { get; set; }
    }
}
