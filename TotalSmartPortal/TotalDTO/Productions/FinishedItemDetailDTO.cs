﻿using System;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;

using TotalModel;
using TotalDTO.Helpers;

namespace TotalDTO.Productions
{
    public class FinishedItemDetailDTO : QuantityDetailDTO, IPrimitiveEntity
    {
        public int GetID() { return this.FinishedItemDetailID; }

        public int FinishedItemDetailID { get; set; }
        public int FinishedItemID { get; set; }

        public int FirmOrderID { get; set; }
        public int FirmOrderDetailID { get; set; }

        public int PlannedOrderID { get; set; }
        public int PlannedOrderDetailID { get; set; }

        public int SemifinishedItemID { get; set; }
        public int SemifinishedItemDetailID { get; set; }

        public Nullable<int> CustomerID { get; set; }
        public int ShiftID { get; set; }
        public int WorkshiftID { get; set; }

        public int ProductionLineID { get; set; }
        public int CrucialWorkerID { get; set; }

        public int SemifinishedHandoverID { get; set; }

        [Display(Name = "Ca trộn")]
        [UIHint("StringReadonly")]
        public string WorkshiftCode { get; set; }
        [Display(Name = "Ngày trộn")]
        [UIHint("DateTimeReadonly")]
        public DateTime WorkshiftEntryDate { get; set; }
        [Display(Name = "Phiếu trộn")]
        [UIHint("StringReadonly")]
        public string SemifinishedItemReference { get; set; }


        [Display(Name = "Tồn hổn hợp")]
        [UIHint("QuantityReadonly")]
        public decimal QuantityRemains { get; set; }

        [Display(Name = "Trừ tồn hổn hợp")]
        [UIHint("QuantityReadonly")]
        public override decimal Quantity { get; set; }

        [Display(Name = "Phế phẩm")]
        [UIHint("Quantity")]
        public decimal QuantityFailure { get; set; }

        [Display(Name = "HH thừa")]
        [UIHint("QuantityReadonly")]
        public decimal QuantityExcess { get; set; }

        [Display(Name = "HH thiếu")]
        [UIHint("Quantity")]
        public decimal QuantityShortage { get; set; }

        [Display(Name = "KL màng thành phẩm")]
        [UIHint("Quantity")]
        public decimal QuantityAndExcess { get { return this.Quantity + this.QuantityExcess; } set { } }

        [Display(Name = "Biên kg")]
        [UIHint("Quantity")]
        public decimal Swarfs { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            foreach (var result in base.Validate(validationContext)) { yield return result; }

            if (this.Quantity + this.QuantityFailure + this.QuantityShortage > this.QuantityRemains) yield return new ValidationResult("Số lượng đóng gói không được lớn hơn số lượng tồn phôi [" + this.CommodityName + " Pallet: " + this.SemifinishedItemReference + "]", new[] { "Quantity" });
        }
    }


    public class FinishedItemLotDTO : BaseModel
    {
        public int FinishedItemLotID { get; set; }
        public int FinishedItemID { get; set; }

        public int FirmOrderID { get; set; }
        public int PlannedOrderID { get; set; }

        public Nullable<int> CustomerID { get; set; }
        public int ShiftID { get; set; }
        public int WorkshiftID { get; set; }

        public int CommodityID { get; set; }
        [Display(Name = "Mã hàng")]
        [UIHint("StringReadonly")]
        public string CommodityCode { get; set; }
        [Display(Name = "Tên hàng")]
        [UIHint("StringReadonly")]
        public string CommodityName { get; set; }
        public int CommodityTypeID { get; set; }

        [Display(Name = "Kg/ cuộn")]
        [UIHint("Integer")]
        public int PiecePerPack { get; set; }


        [Display(Name = "KL màng thành phẩm")]
        [UIHint("Quantity")]
        public decimal Quantity { get; set; }
        [Display(Name = "Số cuộn")]
        [UIHint("QuantityReadonly")]
        public decimal Packages { get { return this.PiecePerPack > 0 ? this.Quantity / this.PiecePerPack : 0; } set { } }
        [Display(Name = "Số cái lẻ")]
        [UIHint("QuantityReadonly")]
        public decimal OddPackages { get { return this.PiecePerPack > 0 ? this.Quantity % this.PiecePerPack : 0; } set { } }


        [Display(Name = "Phế phẩm")]
        [UIHint("QuantityReadonly")]
        public decimal QuantityFailure { get; set; }
        [Display(Name = "Thừa")]
        [UIHint("QuantityReadonly")]
        public decimal QuantityExcess { get; set; }
        [Display(Name = "Thiếu")]
        [UIHint("QuantityReadonly")]
        public decimal QuantityShortage { get; set; }
        [Display(Name = "Biên (kg)")]
        [UIHint("QuantityReadonly")]
        public decimal Swarfs { get; set; }

        public int BatchID { get; set; }
        [Display(Name = "Ngày lô hàng")]
        [UIHint("DateTimeReadonly")]
        public DateTime BatchEntryDate { get; set; }

        public string SemifinishedItemReferences { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            foreach (var result in base.Validate(validationContext)) { yield return result; }

            if (this.OddPackages != 0) yield return new ValidationResult("Số cuộn phải lớn hơn 0 và là số nguyên [" + this.CommodityName + "(" + this.Packages.ToString() + ")" + "]", new[] { "OddPackages" });
            if (this.Quantity != 0 && this.PiecePerPack == 0) yield return new ValidationResult("Vui lòng nhập số kg/ cuộn [" + this.CommodityName + "(" + this.Quantity.ToString() + ")" + "]", new[] { "PiecePerPack" });
        }
    }

}
