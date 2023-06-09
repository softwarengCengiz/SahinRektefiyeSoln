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
    
    public partial class SayimHeader
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SayimHeader()
        {
            this.SayimDetail = new HashSet<SayimDetail>();
            this.StokKartiSayimLoglari = new HashSet<StokKartiSayimLoglari>();
        }
    
        public int SayimHeaderId { get; set; }
        public int SayimTipId { get; set; }
        public int SayimStatuId { get; set; }
        public string SayimAciklama { get; set; }
        public Nullable<System.DateTime> DtSayimBaslangic { get; set; }
        public Nullable<System.DateTime> DtSayimBitis { get; set; }
        public Nullable<System.DateTime> DtCreated { get; set; }
        public string Creator { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SayimDetail> SayimDetail { get; set; }
        public virtual SayimStatu SayimStatu { get; set; }
        public virtual SayimTipi SayimTipi { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StokKartiSayimLoglari> StokKartiSayimLoglari { get; set; }
    }
}
