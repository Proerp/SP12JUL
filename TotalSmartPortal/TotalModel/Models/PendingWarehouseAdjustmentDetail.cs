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
    
    public partial class PendingWarehouseAdjustmentDetail
    {
        public int WarehouseAdjustmentID { get; set; }
        public int WarehouseAdjustmentDetailID { get; set; }
        public string PrimaryReference { get; set; }
        public System.DateTime PrimaryEntryDate { get; set; }
        public Nullable<int> LocationIssueID { get; set; }
        public Nullable<int> WarehouseIssueID { get; set; }
        public Nullable<int> WarehouseAdjustmentTypeID { get; set; }
        public int CommodityID { get; set; }
        public string CommodityCode { get; set; }
        public string CommodityName { get; set; }
        public int BatchID { get; set; }
        public System.DateTime BatchEntryDate { get; set; }
        public Nullable<decimal> QuantityRemains { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public string Remarks { get; set; }
        public Nullable<bool> IsSelected { get; set; }
        public string BatchCode { get; set; }
    }
}
