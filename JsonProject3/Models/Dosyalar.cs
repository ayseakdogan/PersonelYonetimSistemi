//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace JsonProject3.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Dosyalar
    {
        public int DosyaId { get; set; }
        public string DosyaYolu { get; set; }
        public Nullable<int> PersonalId { get; set; }
    
        public virtual Personel Personel { get; set; }
    }
}