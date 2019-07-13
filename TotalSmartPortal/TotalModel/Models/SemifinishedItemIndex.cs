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
    
    public partial class SemifinishedItemIndex
    {
        public int SemifinishedItemID { get; set; }
        public Nullable<System.DateTime> EntryDate { get; set; }
        public string Reference { get; set; }
        public string LocationCode { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string ProductionLineCode { get; set; }
        public int WorkshiftID { get; set; }
        public System.DateTime WorkshiftEntryDate { get; set; }
        public string WorkshiftCode { get; set; }
        public string FirmOrdersReference { get; set; }
        public string FirmOrdersCode { get; set; }
        public string Specification { get; set; }
        public string Description { get; set; }
        public decimal TotalQuantity { get; set; }
        public bool Approved { get; set; }
        public string BomCode { get; set; }
        public decimal TotalQuantityFailure { get; set; }
    }
}