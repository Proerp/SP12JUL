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
    
    public partial class RecyclatePendingWorkshift
    {
        public int WorkshiftID { get; set; }
        public System.DateTime WorkshiftEntryDate { get; set; }
        public string WorkshiftCode { get; set; }
        public int WarehouseID { get; set; }
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }
        public Nullable<decimal> TotalQuantityRemains { get; set; }
    }
}
