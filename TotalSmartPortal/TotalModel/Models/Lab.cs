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
    
    public partial class Lab
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Lab()
        {
            this.GoodsArrivalDetails = new HashSet<GoodsArrivalDetail>();
            this.GoodsArrivalPackages = new HashSet<GoodsArrivalPackage>();
            this.MaterialIssueDetails = new HashSet<MaterialIssueDetail>();
            this.GoodsReceiptDetails = new HashSet<GoodsReceiptDetail>();
        }
    
        public int LabID { get; set; }
        public System.DateTime EntryDate { get; set; }
        public string Reference { get; set; }
        public string Code { get; set; }
        public Nullable<int> GoodsArrivalID { get; set; }
        public int UserID { get; set; }
        public int PreparedPersonID { get; set; }
        public int OrganizationalUnitID { get; set; }
        public int LocationID { get; set; }
        public int ApproverID { get; set; }
        public decimal TotalQuantity { get; set; }
        public string CommodityCodes { get; set; }
        public string CommodityNames { get; set; }
        public string SealCodes { get; set; }
        public string BatchCodes { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public System.DateTime EditedDate { get; set; }
        public bool Approved { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }
        public Nullable<int> VoidTypeID { get; set; }
        public bool InActive { get; set; }
        public Nullable<System.DateTime> InActiveDate { get; set; }
        public bool Hold { get; set; }
        public Nullable<System.DateTime> HoldDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GoodsArrivalDetail> GoodsArrivalDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GoodsArrivalPackage> GoodsArrivalPackages { get; set; }
        public virtual GoodsArrival GoodsArrival { get; set; }
        public virtual Location Location { get; set; }
        public virtual OrganizationalUnit OrganizationalUnit { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialIssueDetail> MaterialIssueDetails { get; set; }
        public virtual VoidType VoidType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GoodsReceiptDetail> GoodsReceiptDetails { get; set; }
    }
}