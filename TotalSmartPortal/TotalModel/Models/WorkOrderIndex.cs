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
    
    public partial class WorkOrderIndex
    {
        public int WorkOrderID { get; set; }
        public Nullable<System.DateTime> EntryDate { get; set; }
        public string Reference { get; set; }
        public string LocationCode { get; set; }
        public string CustomerName { get; set; }
        public string Description { get; set; }
        public decimal TotalQuantity { get; set; }
        public bool Approved { get; set; }
        public string FirmOrdersReference { get; set; }
        public string FirmOrdersCode { get; set; }
        public System.DateTime FirmOrderEntryDate { get; set; }
        public string FirmOrderSpecs { get; set; }
        public string FirmOrderSpecification { get; set; }
        public string BomCode { get; set; }
        public decimal QuantityMaterialEstimated { get; set; }
    }
}
