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
    
    public partial class FinishedHandover
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FinishedHandover()
        {
            this.FinishedHandoverDetails = new HashSet<FinishedHandoverDetail>();
        }
    
        public int FinishedHandoverID { get; set; }
        public System.DateTime EntryDate { get; set; }
        public string Reference { get; set; }
        public Nullable<int> CustomerID { get; set; }
        public Nullable<int> PlannedOrderID { get; set; }
        public int FinishedLeaderID { get; set; }
        public int StorekeeperID { get; set; }
        public int UserID { get; set; }
        public int PreparedPersonID { get; set; }
        public int OrganizationalUnitID { get; set; }
        public int LocationID { get; set; }
        public int ApproverID { get; set; }
        public decimal TotalQuantity { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public System.DateTime EditedDate { get; set; }
        public bool Approved { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }
        public string Caption { get; set; }
        public int WorkshiftID { get; set; }
        public int NMVNTaskID { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Employee Employee1 { get; set; }
        public virtual Location Location { get; set; }
        public virtual Workshift Workshift { get; set; }
        public virtual PlannedOrder PlannedOrder { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FinishedHandoverDetail> FinishedHandoverDetails { get; set; }
    }
}
