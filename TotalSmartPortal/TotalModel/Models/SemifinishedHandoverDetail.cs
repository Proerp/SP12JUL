//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TotalModel.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SemifinishedHandoverDetail
    {
        public int SemifinishedHandoverDetailID { get; set; }
        public int SemifinishedHandoverID { get; set; }
        public System.DateTime EntryDate { get; set; }
        public int NMVNTaskID { get; set; }
        public int LocationID { get; set; }
        public Nullable<int> SemifinishedItemID { get; set; }
        public Nullable<int> SemifinishedProductID { get; set; }
        public decimal Quantity { get; set; }
        public string Remarks { get; set; }
        public bool Approved { get; set; }
    
        public virtual SemifinishedHandover SemifinishedHandover { get; set; }
        public virtual SemifinishedProduct SemifinishedProduct { get; set; }
        public virtual SemifinishedItem SemifinishedItem { get; set; }
    }
}
