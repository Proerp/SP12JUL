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
    
    public partial class RecyclateViewDetail
    {
        public int RecyclateDetailID { get; set; }
        public int RecyclateID { get; set; }
        public Nullable<int> SemifinishedProductID { get; set; }
        public Nullable<int> FinishedProductPackageID { get; set; }
        public Nullable<int> FinishedItemDetailID { get; set; }
        public System.DateTime RootEntryDate { get; set; }
        public string RootReference { get; set; }
        public string ProductionLineCode { get; set; }
        public string FirmOrderCode { get; set; }
        public string Specification { get; set; }
        public int CommodityID { get; set; }
        public string CommodityCode { get; set; }
        public int CommodityTypeID { get; set; }
        public Nullable<int> RecycleCommodityID { get; set; }
        public string RecycleCommodityCode { get; set; }
        public string RecycleCommodityName { get; set; }
        public Nullable<int> RecycleCommodityTypeID { get; set; }
        public decimal QuantityFailures { get; set; }
        public decimal QuantitySwarfs { get; set; }
        public decimal QuantityRemains { get; set; }
        public decimal Quantity { get; set; }
    }
}
