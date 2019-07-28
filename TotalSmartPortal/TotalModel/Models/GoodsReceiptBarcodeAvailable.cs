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
    
    public partial class GoodsReceiptBarcodeAvailable
    {
        public Nullable<int> GoodsReceiptDetailID { get; set; }
        public Nullable<System.DateTime> BatchEntryDate { get; set; }
        public string BatchCode { get; set; }
        public string LabCode { get; set; }
        public string Barcode { get; set; }
        public int CommodityID { get; set; }
        public string CommodityCode { get; set; }
        public string CommodityName { get; set; }
        public Nullable<int> BinLocationID { get; set; }
        public string BinLocationCode { get; set; }
        public decimal UnitWeight { get; set; }
        public decimal TareWeight { get; set; }
        public Nullable<decimal> QuantityAvailables { get; set; }
        public bool Approved { get; set; }
        public Nullable<bool> LabApproved { get; set; }
        public Nullable<bool> LabHold { get; set; }
        public Nullable<bool> LabInActive { get; set; }
        public string LabInActiveCode { get; set; }
        public Nullable<int> GoodsArrivalPackageID { get; set; }
        public System.DateTime EntryDate { get; set; }
        public string Reference { get; set; }
        public Nullable<System.DateTime> ExpiryDate { get; set; }
        public Nullable<int> WarehouseID { get; set; }
        public string WarehouseCode { get; set; }
        public Nullable<int> BatchID { get; set; }
        public Nullable<bool> BatchExpiried { get; set; }
    }
}
