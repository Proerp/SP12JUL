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
    
    public partial class ProductionOrderPendingPlannedOrder
    {
        public int PlannedOrderID { get; set; }
        public string PlannedOrderReference { get; set; }
        public string PlannedOrderCode { get; set; }
        public System.DateTime PlannedOrderEntryDate { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public int CustomerID { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerOfficialName { get; set; }
        public string CustomerVATCode { get; set; }
        public string CustomerAttentionName { get; set; }
        public int CustomerTerritoryID { get; set; }
        public string CustomerEntireTerritoryEntireName { get; set; }
        public System.DateTime PlannedOrderDeliveryDate { get; set; }
    }
}