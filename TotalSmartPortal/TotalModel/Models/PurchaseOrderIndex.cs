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
    
    public partial class PurchaseOrderIndex
    {
        public int PurchaseOrderID { get; set; }
        public Nullable<System.DateTime> EntryDate { get; set; }
        public string Reference { get; set; }
        public string Code { get; set; }
        public string LocationCode { get; set; }
        public string CustomerName { get; set; }
        public string VoidTypeName { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
        public string Purposes { get; set; }
        public string Description { get; set; }
        public decimal TotalQuantity { get; set; }
        public bool Approved { get; set; }
        public bool InActive { get; set; }
        public bool InActivePartial { get; set; }
        public decimal TotalQuantityArrived { get; set; }
        public string CommodityCode { get; set; }
        public string CommodityName { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public Nullable<decimal> QuantityArrived { get; set; }
        public Nullable<decimal> QuantityRemains { get; set; }
        public Nullable<System.DateTime> VoucherDate { get; set; }
    }
}
