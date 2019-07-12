﻿using System;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;

using TotalModel;
using TotalDTO.Helpers;
using TotalBase.Enums;

namespace TotalDTO.Productions
{
    public class SemifinishedItemDetailDTO : QuantityDetailDTO, IPrimitiveEntity
    {
        public int GetID() { return this.SemifinishedItemDetailID; }

        public int SemifinishedItemDetailID { get; set; }
        public int SemifinishedItemID { get; set; }

        public int MaterialIssueID { get; set; }

        public int FirmOrderID { get; set; }
        public int FirmOrderDetailID { get; set; }

        public int PlannedOrderID { get; set; }
        public int PlannedOrderDetailID { get; set; }
        [Display(Name = "Công thức")]
        [UIHint("StringReadonly")]
        public string BomCode { get; set; }

        public Nullable<int> CustomerID { get; set; }

        public int ShiftID { get; set; }
        public int WorkshiftID { get; set; }

        public int ProductionLineID { get; set; }
        public int CrucialWorkerID { get; set; }

        [Display(Name = "Cái/ tấm")]
        [UIHint("QuantityReadonly")]
        public decimal MoldQuantity { get; set; }

        [Display(Name = "Tồn lệnh")]
        [UIHint("QuantityReadonly")]
        public decimal QuantityRemains { get; set; }
        [Display(Name = "KL hỗn hợp")]
        [UIHint("Quantity")]
        public override decimal Quantity { get; set; }
        [Display(Name = "Phế phẩm")]
        [UIHint("Quantity")]
        public decimal QuantityFailure { get; set; }

        public string GetCaption(int count) { return this.CommodityCode + (count > 1 ? " [" + this.Quantity.ToString("N" + GlobalEnums.rndQuantity.ToString()) + "] " : ""); }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            foreach (var result in base.Validate(validationContext)) { yield return result; }

            if (this.Quantity > this.QuantityRemains + 1000000) yield return new ValidationResult("Số lượng xuất không được lớn hơn số lượng còn lại [" + this.CommodityName + "]", new[] { "Quantity" });
        }
    }
}
