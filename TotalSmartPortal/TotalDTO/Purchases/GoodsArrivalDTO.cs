﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using TotalModel;
using TotalBase.Enums;
using TotalDTO.Helpers;
using TotalDTO.Commons;

namespace TotalDTO.Purchases
{
    public interface IGoodsArrivalOption { GlobalEnums.NmvnTaskID NMVNTaskID { get; } }

    public class GoodsArrivalOptionMaterial : IGoodsArrivalOption { public GlobalEnums.NmvnTaskID NMVNTaskID { get { return GlobalEnums.NmvnTaskID.MaterialArrival; } } }
    public class GoodsArrivalOptionItem : IGoodsArrivalOption { public GlobalEnums.NmvnTaskID NMVNTaskID { get { return GlobalEnums.NmvnTaskID.ItemArrival; } } }
    public class GoodsArrivalOptionProduct : IGoodsArrivalOption { public GlobalEnums.NmvnTaskID NMVNTaskID { get { return GlobalEnums.NmvnTaskID.ProductArrival; } } }

    public interface IGoodsArrivalPrimitiveDTO : IQuantityDTO, IPrimitiveEntity, IPrimitiveDTO, IBaseDTO
    {
        int GoodsArrivalID { get; set; }

        int CustomerID { get; set; }
        int TransporterID { get; set; }

        Nullable<int> WarehouseID { get; set; }

        bool HasPurchaseOrder { get; set; }
        Nullable<int> PurchaseOrderID { get; set; }
        string PurchaseOrderReference { get; set; }
        string PurchaseOrderReferences { get; set; }
        string PurchaseOrderCode { get; set; }
        string PurchaseOrderCodes { get; set; }
        [Display(Name = "Phiếu đặt hàng")]
        string PurchaseOrderReferenceNote { get; }
        [Display(Name = "Số đơn hàng")]
        string PurchaseOrderCodeNote { get; }
        [Display(Name = "Ngày đặt hàng")]
        Nullable<System.DateTime> PurchaseOrderVoucherDate { get; set; }

        [Display(Name = "Số hóa đơn")]
        [UIHint("Commons/SOCode")]
        string Code { get; set; }
        [Display(Name = "Số packing list")]
        string PackingList { get; set; }
        [Display(Name = "Số tờ khai hải quan")]
        string CustomsDeclaration { get; set; }
        [Display(Name = "Ngày chứng từ")]
        Nullable<System.DateTime> CustomsDeclarationDate { get; set; }
        [Display(Name = "Ngày dự kiến giao hàng")]
        Nullable<System.DateTime> DeliveryDate { get; set; }

        int SalespersonID { get; set; }

        [Display(Name = "Tổng trọng lượng")]
        decimal TotalPackages { get; set; }
    }

    public class GoodsArrivalPrimitiveDTO<TGoodsArrivalOption> : QuantityDTO<GoodsArrivalDetailDTO>, IPrimitiveEntity, IPrimitiveDTO
        where TGoodsArrivalOption : IGoodsArrivalOption, new()
    {
        public GlobalEnums.NmvnTaskID NMVNTaskID { get { return new TGoodsArrivalOption().NMVNTaskID; } }

        public int GetID() { return this.GoodsArrivalID; }
        public void SetID(int id) { this.GoodsArrivalID = id; }

        public int GoodsArrivalID { get; set; }

        public virtual int CustomerID { get; set; }
        public virtual int TransporterID { get; set; }

        public virtual Nullable<int> WarehouseID { get; set; }

        public bool HasPurchaseOrder { get; set; }
        public Nullable<int> PurchaseOrderID { get; set; }
        public string PurchaseOrderReference { get; set; }
        public string PurchaseOrderReferences { get; set; }
        public string PurchaseOrderCode { get; set; }
        public string PurchaseOrderCodes { get; set; }
        [Display(Name = "Phiếu đặt hàng")]
        public string PurchaseOrderReferenceNote { get { return this.PurchaseOrderID != null ? this.PurchaseOrderReference : (this.PurchaseOrderReferences != "" ? this.PurchaseOrderReferences : "Giao hàng tổng hợp của nhiều ĐH"); } }
        [Display(Name = "Số đơn hàng")]
        public string PurchaseOrderCodeNote { get { return this.PurchaseOrderID != null ? this.PurchaseOrderCode : (this.PurchaseOrderCodes != "" ? this.PurchaseOrderCodes : ""); } }
        [Display(Name = "Ngày đặt hàng")]
        public Nullable<System.DateTime> PurchaseOrderVoucherDate { get; set; }

        [Display(Name = "Số hóa đơn")]
        [UIHint("Commons/SOCode")]
        public string Code { get; set; }
        [Display(Name = "Số packing list")]
        public string PackingList { get; set; }
        [Display(Name = "Số tờ khai hải quan")]
        public string CustomsDeclaration { get; set; }
        [Display(Name = "Ngày chứng từ")]
        public Nullable<System.DateTime> CustomsDeclarationDate { get; set; }
        [Display(Name = "Ngày dự kiến giao hàng")]
        public Nullable<System.DateTime> DeliveryDate { get; set; }

        public virtual int SalespersonID { get; set; }

        [Display(Name = "Tổng trọng lượng")]
        public decimal TotalPackages { get; set; }
        protected virtual decimal GetTotalWeight() { return this.DtoDetails().Select(o => o.Packages).Sum(); }


        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            foreach (var result in base.Validate(validationContext)) { yield return result; }
            
            if (GlobalEnums.DMC) this.TotalPackages = this.GetTotalWeight();
            if (this.TotalPackages != this.GetTotalWeight()) yield return new ValidationResult("Lỗi tổng số kiện", new[] { "TotalPackages" });
        }

        public override void PerformPresaveRule()
        {
            base.PerformPresaveRule();

            string purchaseOrderReferences = ""; string purchaseOrderCodes = ""; string caption = ""; int serialID = 0;
            this.DtoDetails().ToList().ForEach(e => { e.NMVNTaskID = this.NMVNTaskID; e.CustomerID = this.CustomerID; e.TransporterID = this.TransporterID; e.WarehouseID = this.WarehouseID; e.SalespersonID = this.SalespersonID; e.SerialID = ++serialID; if (this.HasPurchaseOrder && purchaseOrderReferences.IndexOf(e.PurchaseOrderReference) < 0) purchaseOrderReferences = purchaseOrderReferences + (purchaseOrderReferences != "" ? ", " : "") + e.PurchaseOrderReference; if (this.HasPurchaseOrder && e.PurchaseOrderCode != null && purchaseOrderCodes.IndexOf(e.PurchaseOrderCode) < 0) purchaseOrderCodes = purchaseOrderCodes + (purchaseOrderCodes != "" ? ", " : "") + e.PurchaseOrderCode; if (caption.IndexOf(e.CommodityCode) < 0) caption = caption + (caption != "" ? ", " : "") + e.CommodityCode; });
            this.PurchaseOrderReferences = purchaseOrderReferences; this.PurchaseOrderCodes = purchaseOrderCodes != "" ? purchaseOrderCodes : null; this.Caption = caption != "" ? (caption.Length > 98 ? caption.Substring(0, 95) + "..." : caption) : null; //if (this.HasPurchaseOrder) this.Code = this.PurchaseOrderCodes;
        }
    }

    public interface IGoodsArrivalDTO : IGoodsArrivalPrimitiveDTO
    {
        [Display(Name = "Khách hàng")]
        [UIHint("Commons/CustomerBase")]
        CustomerBaseDTO Customer { get; set; }

        [Display(Name = "Đơn vị, người nhận hàng")]
        [UIHint("Commons/CustomerBase")]
        CustomerBaseDTO Transporter { get; set; }

        [Display(Name = "Kho hàng")]
        [UIHint("AutoCompletes/WarehouseBase")]
        WarehouseBaseDTO Warehouse { get; set; }

        [Display(Name = "Nhân viên nhận hàng")]
        [UIHint("AutoCompletes/EmployeeBase")]
        EmployeeBaseDTO Salesperson { get; set; }

        List<GoodsArrivalDetailDTO> GoodsArrivalViewDetails { get; set; }
        List<GoodsArrivalDetailDTO> ViewDetails { get; set; }

        string ControllerName { get; }
        string PurchaseController { get; }

        bool IsItem { get; }
        bool IsProduct { get; }
    }

    public class GoodsArrivalDTO<TGoodsArrivalOption> : GoodsArrivalPrimitiveDTO<TGoodsArrivalOption>, IBaseDetailEntity<GoodsArrivalDetailDTO>, IGoodsArrivalDTO
        where TGoodsArrivalOption : IGoodsArrivalOption, new()
    {
        public GoodsArrivalDTO()
        {
            this.GoodsArrivalViewDetails = new List<GoodsArrivalDetailDTO>();
            if (GlobalEnums.CBPP) { this.Salesperson = new EmployeeBaseDTO() { EmployeeID = 1, PreparedPersonID = 1, Name = "NONAME" }; }
        }

        public override int CustomerID { get { return (this.Customer != null ? this.Customer.CustomerID : 0); } }
        [Display(Name = "Khách hàng")]
        [UIHint("Commons/CustomerBase")]
        public CustomerBaseDTO Customer { get; set; }

        public override int TransporterID { get { return (this.Transporter != null ? this.Transporter.CustomerID : 0); } }
        [Display(Name = "Đơn vị, người nhận hàng")]
        [UIHint("Commons/CustomerBase")]
        public CustomerBaseDTO Transporter { get; set; }

        public override Nullable<int> WarehouseID { get { return (this.Warehouse != null ? this.Warehouse.WarehouseID : null); } }
        [Display(Name = "Kho hàng")]
        [UIHint("AutoCompletes/WarehouseBase")]
        public WarehouseBaseDTO Warehouse { get; set; }


        public override int SalespersonID { get { return (this.Salesperson != null ? this.Salesperson.EmployeeID : 0); } }
        [Display(Name = "Nhân viên nhận hàng")]
        [UIHint("AutoCompletes/EmployeeBase")]
        public EmployeeBaseDTO Salesperson { get; set; }


        public List<GoodsArrivalDetailDTO> GoodsArrivalViewDetails { get; set; }
        public List<GoodsArrivalDetailDTO> ViewDetails { get { return this.GoodsArrivalViewDetails; } set { this.GoodsArrivalViewDetails = value; } }

        public ICollection<GoodsArrivalDetailDTO> GetDetails() { return this.GoodsArrivalViewDetails; }

        protected override IEnumerable<GoodsArrivalDetailDTO> DtoDetails() { return this.GoodsArrivalViewDetails; }



        public string ControllerName { get { return this.NMVNTaskID.ToString() + "s"; } }
        public string PurchaseController { get { return this.IsMaterial ? "PurchaseMaterials" : (this.IsItem ? "PurchaseItems" : (this.IsProduct ? "PurchaseProducts" : "")); } }

        public bool IsMaterial { get { return this.NMVNTaskID == GlobalEnums.NmvnTaskID.MaterialArrival; } }
        public bool IsItem { get { return this.NMVNTaskID == GlobalEnums.NmvnTaskID.ItemArrival; } }
        public bool IsProduct { get { return this.NMVNTaskID == GlobalEnums.NmvnTaskID.ProductArrival; } }
    }
}
