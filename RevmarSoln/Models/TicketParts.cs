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
    
    public partial class TicketParts
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TicketParts()
        {
            this.Talepler = new HashSet<Talepler>();
        }
    
        public int TalepParcaId { get; set; }
        public Nullable<int> TalepId { get; set; }
        public string Parca { get; set; }
        public string Adet { get; set; }
        public string BirimFiyat { get; set; }
        public string ParcaIskonto { get; set; }
        public string BirimToplamFiyat { get; set; }
        public string ToplamFiyat { get; set; }
        public string KDV { get; set; }
        public string GenelToplam { get; set; }
        public string GenelIskonto { get; set; }
        public string Olusturan { get; set; }
        public Nullable<System.DateTime> OlusturmaTarihi { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Talepler> Talepler { get; set; }
        public virtual Talepler Talepler1 { get; set; }
    }
}
