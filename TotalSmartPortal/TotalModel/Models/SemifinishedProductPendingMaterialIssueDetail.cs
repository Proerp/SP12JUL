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
    
    public partial class SemifinishedProductPendingMaterialIssueDetail
    {
        public int MaterialIssueDetailID { get; set; }
        public int MaterialIssueID { get; set; }
        public int FirmOrderID { get; set; }
        public System.DateTime FirmOrderEntryDate { get; set; }
        public string FirmOrderReference { get; set; }
        public string FirmOrderCode { get; set; }
        public int CustomerID { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public int ProductionLineID { get; set; }
        public string ProductionLineCode { get; set; }
        public string FirmOrderSpecification { get; set; }
        public int GoodsReceiptID { get; set; }
        public string GoodsReceiptReference { get; set; }
        public string GoodsReceiptCode { get; set; }
        public System.DateTime GoodsReceiptEntryDate { get; set; }
        public int GoodsReceiptDetailID { get; set; }
        public string MaterialCode { get; set; }
        public string MaterialName { get; set; }
        public decimal MaterialQuantity { get; set; }
        public Nullable<decimal> MaterialQuantityRemains { get; set; }
        public int CrucialWorkerID { get; set; }
        public string CrucialWorkerCode { get; set; }
        public string CrucialWorkerName { get; set; }
        public int MaterialIssueDetailWorkshiftID { get; set; }
        public System.DateTime MaterialIssueDetailWorkshiftEntryDate { get; set; }
        public string MaterialIssueDetailWorkshiftCode { get; set; }
        public int PlannedOrderID { get; set; }
    }
}