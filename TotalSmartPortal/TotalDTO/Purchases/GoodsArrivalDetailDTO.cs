﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using TotalBase.Enums;
using TotalModel;
using TotalDTO.Helpers;

namespace TotalDTO.Purchases
{
    public class GoodsArrivalDetailDTO : QuantityDetailDTO, IPrimitiveEntity
    {
        public int GetID() { return this.GoodsArrivalDetailID; }

        public int GoodsArrivalDetailID { get; set; }
        public int GoodsArrivalID { get; set; }

        public GlobalEnums.NmvnTaskID NMVNTaskID { get; set; }

        public int CustomerID { get; set; }
        public int TransporterID { get; set; }
        public int SalespersonID { get; set; }

        public Nullable<int> WarehouseID { get; set; }

        public Nullable<int> PurchaseOrderID { get; set; }
        public Nullable<int> PurchaseOrderDetailID { get; set; }

        [Display(Name = "Phiếu ĐH")]
        [UIHint("StringReadonly")]
        public string PurchaseOrderReference { get; set; }
        [Display(Name = "Số ĐH")]
        [UIHint("StringReadonly")]
        public string PurchaseOrderCode { get; set; }
        [Display(Name = "Ngày ĐH")]
        [UIHint("DateTimeReadonly")]
        public Nullable<System.DateTime> PurchaseOrderEntryDate { get; set; }

        [Display(Name = "Shelflife")]
        [UIHint("IntegerReadonly")]
        public Nullable<int> Shelflife { get; set; }
        [Display(Name = "Ngày SX")]
        [UIHint("Date")]
        public Nullable<System.DateTime> ProductionDate { get; set; }
        [Display(Name = "HSD")]
        [UIHint("Date")]
        public Nullable<System.DateTime> ExpiryDate { get; set; }
        [Display(Name = "# HSD")]
        [UIHint("IntegerReadonly")]
        public Nullable<int> Lifespan { get { return this.ExpiryDate != null && this.ProductionDate != null ? ((DateTime)this.ExpiryDate - (DateTime)this.ProductionDate).Days : (int?)null; } set { } }


        [UIHint("StringReadonly")]
        public override string CommodityCode { get; set; }

        [Display(Name = "Số cont")]
        [Required(ErrorMessage = "Vui lòng nhập số seal")]
        public virtual string SealCode { get; set; }

        [Display(Name = "Số lô")]
        [Required(ErrorMessage = "Vui lòng nhập số batch")]
        public virtual string BatchCode { get; set; }

        [Display(Name = "Lab code")]
        [Required(ErrorMessage = "Vui lòng nhập lab code")]
        public virtual string LabCode { get; set; }

        [Display(Name = "Ngày lô hàng")]
        [UIHint("DateTimeReadonly")]
        public Nullable<System.DateTime> BatchEntryDate { get { return this.EntryDate == null ? this.EntryDate : ((DateTime)this.EntryDate).Date; } }

        [Display(Name = "Tồn đơn")]
        [UIHint("QuantityReadonly")]
        public decimal QuantityRemains { get; set; }

        [Display(Name = "KL")]
        [UIHint("Quantity")]
        public override decimal Quantity { get; set; }

        [Display(Name = "Kg/kiện")]
        [UIHint("Quantity")]
        public decimal UnitWeight { get; set; }

        [Display(Name = "BB")]
        [UIHint("Quantity")]
        public decimal TareWeight { get; set; }

        [Display(Name = "Số kiện")]
        [UIHint("QuantityReadonly")]
        public decimal Packages { get; set; }


        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            foreach (var result in base.Validate(validationContext)) { yield return result; }

            if (GlobalEnums.DMC) { this.UnitWeight = this.Quantity; this.Packages = (this.Quantity > 0 ? 1 : 0); }

            if (GlobalEnums.CBPP && this.TareWeight <= 0) yield return new ValidationResult("Vui lòng nhập trọng lượng bao bì [" + this.CommodityName + "]", new[] { "TareWeight" });
            if (this.Quantity != 0 && (this.Quantity != this.Packages * this.UnitWeight || this.UnitWeight == 0 || this.Packages - Math.Truncate(this.Packages) != 0)) yield return new ValidationResult("Số kiện phải lớn hơn 0 và là số nguyên [" + this.CommodityName + "]", new[] { "UnitWeight" });
            if (this.PurchaseOrderID > 0 && (this.Quantity > this.QuantityRemains * (decimal)1.1)) yield return new ValidationResult("Số lượng xuất không được lớn hơn số lượng còn lại [" + this.CommodityName + "]", new[] { "Quantity" });
        }
    }


    public class ExpiryDateAaViewModel : BaseDTO
    {
        public int GoodsArrivalDetailID { get; set; }
        public int GoodsArrivalID { get; set; }

        [Display(Name = "Mã hàng")]
        [UIHint("StringReadonly")]
        public string CommodityCode { get; set; }

        [Display(Name = "Tên hàng")]
        [UIHint("StringReadonly")]
        public string CommodityName { get; set; }

        [Display(Name = "Số lô")]
        [UIHint("StringReadonly")]
        public string BatchCode { get; set; }
        
        [Display(Name = "Số hồ sơ gia hạn")]
        [Required(ErrorMessage = "Vui lòng nhập số hồ sơ gia hạn")]
        public string Remarks { get; set; }
    }

    public class ExpiryDateBbViewModel : ExpiryDateAaViewModel
    {
        [Display(Name = "Ngày SX")]
        [UIHint("Date")]
        public Nullable<System.DateTime> CurrentProductionDate { get; set; }
        [Display(Name = "HSD")]
        [UIHint("Date")]
        public Nullable<System.DateTime> CurrentExpiryDate { get; set; }
    }

    public class ExpiryDateViewModel : ExpiryDateAaViewModel
    {
        [Display(Name = "Ngày SX")]
        [UIHint("Date")]
        public Nullable<System.DateTime> ProductionDate { get; set; }
        [Display(Name = "HSD")]
        [UIHint("Date")]
        public Nullable<System.DateTime> ExpiryDate { get; set; }
    }
}
