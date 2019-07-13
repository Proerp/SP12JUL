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
    
    public partial class WarehouseEntry
    {
        public int GoodsIssueID { get; set; }
        public string TaskAction { get; set; }
        public System.DateTime EntryDate { get; set; }
        public string Reference { get; set; }
        public string WarehouseCode { get; set; }
        public int CustomerID { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Description { get; set; }
        public string DeliveryAdviceReferences { get; set; }
        public int GoodsIssueDetailID { get; set; }
        public int CommodityID { get; set; }
        public string CommodityCode { get; set; }
        public string CodePartA { get; set; }
        public string CodePartB { get; set; }
        public string CodePartC { get; set; }
        public string CodePartD { get; set; }
        public string CommodityName { get; set; }
        public decimal Quantity { get; set; }
        public decimal FreeQuantity { get; set; }
        public decimal ListedPrice { get; set; }
        public decimal ListedGrossPrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal ListedAmount { get; set; }
        public decimal Amount { get; set; }
        public Nullable<decimal> DiscountAmount { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TradeDiscountAmount { get; set; }
        public decimal TotalTaxableAmount { get; set; }
        public decimal TotalVATAmount { get; set; }
        public decimal TotalGrossAmount { get; set; }
    }
}