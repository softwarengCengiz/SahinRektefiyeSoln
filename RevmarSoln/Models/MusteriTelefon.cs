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
    
    public partial class MusteriTelefon
    {
        public int MusteriTelefonId { get; set; }
        public string TelefonNumarasi { get; set; }
        public bool SwOnaySms { get; set; }
        public Nullable<System.DateTime> DtOnaySms { get; set; }
        public bool SwOnayArama { get; set; }
        public Nullable<System.DateTime> DtOnayArama { get; set; }
        public int MusteriId { get; set; }
        public string Creator { get; set; }
        public Nullable<System.DateTime> DtCreated { get; set; }
        public string Modifier { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string OnaySmsKodu { get; set; }
        public string OnayAramaKodu { get; set; }
        public string GenelOnayKodu { get; set; }
        public Nullable<System.DateTime> DtGenelOnayKodu { get; set; }
        public bool SwPrefered { get; set; }
    
        public virtual Musteri Musteri { get; set; }
    }
}
