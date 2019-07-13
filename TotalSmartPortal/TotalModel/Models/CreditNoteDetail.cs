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
    
    public partial class CreditNoteDetail
    {
        public int CreditNoteDetailID { get; set; }
        public System.DateTime EntryDate { get; set; }
        public int LocationID { get; set; }
        public int CreditNoteID { get; set; }
        public int CustomerID { get; set; }
        public int CommodityID { get; set; }
        public int CommodityTypeID { get; set; }
        public Nullable<int> PromotionID { get; set; }
        public int SalespersonID { get; set; }
        public int CalculatingTypeID { get; set; }
        public bool VATbyRow { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TradeDiscountRate { get; set; }
        public decimal VATPercent { get; set; }
        public decimal GrossPrice { get; set; }
        public decimal Amount { get; set; }
        public decimal VATAmount { get; set; }
        public decimal GrossAmount { get; set; }
        public string Remarks { get; set; }
        public bool Approved { get; set; }
    
        public virtual CreditNote CreditNote { get; set; }
        public virtual Commodity Commodity { get; set; }
    }
}