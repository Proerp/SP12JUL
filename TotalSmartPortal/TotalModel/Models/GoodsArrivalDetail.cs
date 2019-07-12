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
    
    public partial class GoodsArrivalDetail
    {
        public int GoodsArrivalDetailID { get; set; }
        public System.DateTime EntryDate { get; set; }
        public int GoodsArrivalID { get; set; }
        public int LocationID { get; set; }
        public int CustomerID { get; set; }
        public int TransporterID { get; set; }
        public Nullable<int> PurchaseOrderID { get; set; }
        public Nullable<int> PurchaseOrderDetailID { get; set; }
        public int CommodityID { get; set; }
        public int CommodityTypeID { get; set; }
        public int WarehouseID { get; set; }
        public string Code { get; set; }
        public string SealCode { get; set; }
        public string BatchCode { get; set; }
        public string LabCode { get; set; }
        public string Barcode { get; set; }
        public Nullable<System.DateTime> ProductionDate { get; set; }
        public Nullable<System.DateTime> ExpiryDate { get; set; }
        public decimal Quantity { get; set; }
        public decimal QuantityReceipted { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> VoidTypeID { get; set; }
        public bool Approved { get; set; }
        public bool InActive { get; set; }
        public bool InActivePartial { get; set; }
        public Nullable<System.DateTime> InActivePartialDate { get; set; }
        public int SerialID { get; set; }
        public decimal UnitWeight { get; set; }
        public decimal Packages { get; set; }
        public Nullable<int> LabID { get; set; }
        public int BatchID { get; set; }
        public System.DateTime BatchEntryDate { get; set; }
        public decimal TareWeight { get; set; }
        public int NMVNTaskID { get; set; }
    
        public virtual GoodsArrival GoodsArrival { get; set; }
        public virtual VoidType VoidType { get; set; }
        public virtual PurchaseOrderDetail PurchaseOrderDetail { get; set; }
        public virtual Lab Lab { get; set; }
        public virtual Commodity Commodity { get; set; }
    }
}
