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
    
    public partial class MaterialIssuePendingFirmOrderMaterial
    {
        public int FirmOrderMaterialID { get; set; }
        public int CommodityID { get; set; }
        public string CommodityCode { get; set; }
        public string CommodityName { get; set; }
        public int CommodityTypeID { get; set; }
        public Nullable<int> GoodsReceiptID { get; set; }
        public Nullable<int> GoodsReceiptDetailID { get; set; }
        public Nullable<System.DateTime> GoodsReceiptEntryDate { get; set; }
        public string GoodsReceiptReference { get; set; }
        public string GoodsReceiptCode { get; set; }
        public Nullable<decimal> QuantityAvailables { get; set; }
        public int Quantity { get; set; }
        public Nullable<bool> IsSelected { get; set; }
        public Nullable<int> BatchID { get; set; }
        public Nullable<System.DateTime> BatchEntryDate { get; set; }
        public string OfficialCode { get; set; }
        public string CodePartA { get; set; }
        public string CodePartB { get; set; }
        public string CodePartC { get; set; }
        public string CodePartD { get; set; }
        public string CodePartE { get; set; }
        public string CodePartF { get; set; }
        public Nullable<int> LabID { get; set; }
        public string Barcode { get; set; }
        public string BatchCode { get; set; }
        public string SealCode { get; set; }
        public string LabCode { get; set; }
        public Nullable<System.DateTime> ProductionDate { get; set; }
        public Nullable<System.DateTime> ExpiryDate { get; set; }
        public string LayerCode { get; set; }
        public int BomID { get; set; }
        public int BomDetailID { get; set; }
        public int WorkOrderDetailID { get; set; }
        public Nullable<decimal> WorkOrderRemains { get; set; }
    }
}