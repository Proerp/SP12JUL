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
    
    public partial class GoodsReceiptPendingPurchaseRequisition
    {
        public int PurchaseRequisitionID { get; set; }
        public string PurchaseRequisitionReference { get; set; }
        public string PurchaseRequisitionCode { get; set; }
        public System.DateTime PurchaseRequisitionEntryDate { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public int CustomerID { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerOfficialName { get; set; }
        public int GoodsReceiptTypeID { get; set; }
        public int WarehouseID { get; set; }
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }
    }
}
