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
    
    public partial class BlendingInstructionIndex
    {
        public int BlendingInstructionID { get; set; }
        public System.DateTime EntryDate { get; set; }
        public string Reference { get; set; }
        public string Code { get; set; }
        public Nullable<System.DateTime> VoucherDate { get; set; }
        public string Description { get; set; }
        public Nullable<int> BlendingInstructionDetailID { get; set; }
        public string CommodityCode { get; set; }
        public string CommodityName { get; set; }
        public bool Approved { get; set; }
        public bool InActive { get; set; }
        public bool InActivePartial { get; set; }
        public string VoidTypeName { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public Nullable<decimal> QuantityIssued { get; set; }
        public Nullable<decimal> QuantityRemains { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public Nullable<decimal> QuantityAvailableArrivals { get; set; }
        public Nullable<decimal> QuantityAvailableLocation1 { get; set; }
        public Nullable<decimal> QuantityAvailableLocation2 { get; set; }
        public string Jobs { get; set; }
        public int ParentID { get; set; }
        public string ParentReference { get; set; }
    }
}