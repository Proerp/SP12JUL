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
    
    public partial class WorkOrderDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WorkOrderDetail()
        {
            this.MaterialIssueDetails = new HashSet<MaterialIssueDetail>();
        }
    
        public int WorkOrderDetailID { get; set; }
        public int WorkOrderID { get; set; }
        public System.DateTime EntryDate { get; set; }
        public int NMVNTaskID { get; set; }
        public int LocationID { get; set; }
        public int CustomerID { get; set; }
        public int ProductionOrderID { get; set; }
        public int ProductionOrderDetailID { get; set; }
        public int PlannedOrderID { get; set; }
        public int FirmOrderID { get; set; }
        public int FirmOrderMaterialID { get; set; }
        public int BomID { get; set; }
        public int BomDetailID { get; set; }
        public int CommodityID { get; set; }
        public decimal Quantity { get; set; }
        public decimal QuantityIssued { get; set; }
        public string Remarks { get; set; }
        public bool Approved { get; set; }
        public Nullable<int> VoidTypeID { get; set; }
        public bool InActive { get; set; }
        public bool InActivePartial { get; set; }
        public Nullable<System.DateTime> InActivePartialDate { get; set; }
    
        public virtual Commodity Commodity { get; set; }
        public virtual FirmOrderMaterial FirmOrderMaterial { get; set; }
        public virtual WorkOrder WorkOrder { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialIssueDetail> MaterialIssueDetails { get; set; }
    }
}
