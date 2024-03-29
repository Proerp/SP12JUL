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
    
    public partial class WorkOrderPendingFirmOrder
    {
        public int ProductionOrderDetailID { get; set; }
        public int ProductionOrderID { get; set; }
        public int PlannedOrderID { get; set; }
        public int FirmOrderID { get; set; }
        public string FirmOrderCode { get; set; }
        public string FirmOrderReference { get; set; }
        public System.DateTime FirmOrderEntryDate { get; set; }
        public string FirmOrderSpecs { get; set; }
        public string FirmOrderSpecification { get; set; }
        public int BomID { get; set; }
        public decimal TotalQuantity { get; set; }
        public Nullable<decimal> TotalQuantityRemains { get; set; }
        public Nullable<decimal> QuantityMaterialEstimatedRemains { get; set; }
        public int CustomerID { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public int WarehouseID { get; set; }
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }
        public decimal FirmOrderQuantityMaterialEstimated { get; set; }
        public decimal FirmOrderQuantityMaterialEstimatedIssued { get; set; }
    }
}
