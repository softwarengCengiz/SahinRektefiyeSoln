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
    
    public partial class MusteriMail
    {
        public int MusteriMailId { get; set; }
        public string MailAdresi { get; set; }
        public bool SwOnay { get; set; }
        public Nullable<System.DateTime> DtOnay { get; set; }
        public int MusteriId { get; set; }
        public string Creator { get; set; }
        public Nullable<System.DateTime> DtCreated { get; set; }
        public string MailOnayKodu { get; set; }
        public string Modifier { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    
        public virtual Musteri Musteri { get; set; }
    }
}
