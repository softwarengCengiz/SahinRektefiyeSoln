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
    
    public partial class IsEmriDosyalari
    {
        public int IsEmriDosyaId { get; set; }
        public int IsEmriId { get; set; }
        public string DosyaUrl { get; set; }
        public string Creator { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> IsEmriTipId { get; set; }
    
        public virtual IsEmirleri IsEmirleri { get; set; }
        public virtual IsEmriTipleri IsEmriTipleri { get; set; }
    }
}
