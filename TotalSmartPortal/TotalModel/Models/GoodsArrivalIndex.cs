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
    
    public partial class GoodsArrivalIndex
    {
        public int GoodsArrivalID { get; set; }
        public Nullable<System.DateTime> EntryDate { get; set; }
        public string Reference { get; set; }
        public string Code { get; set; }
        public string LocationCode { get; set; }
        public string CustomerName { get; set; }
        public string TransporterName { get; set; }
        public string Description { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalQuantityReceipted { get; set; }
        public bool Approved { get; set; }
        public decimal TotalPackages { get; set; }
        public Nullable<System.DateTime> PurchaseOrderEntryDate { get; set; }
        public Nullable<int> PurchaseOrderID { get; set; }
        public string PurchaseOrderCodes { get; set; }
        public string PurchaseOrderReferences { get; set; }
        public string PackingList { get; set; }
        public string CustomsDeclaration { get; set; }
        public string CustomerCode { get; set; }
        public string Caption { get; set; }
        public string CommodityCode { get; set; }
        public string CommodityName { get; set; }
        public string BatchCode { get; set; }
        public string LabCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal Packages { get; set; }
    }
}
