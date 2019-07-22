using System;
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
        [Display(Name = "Mã NVL")]
        [UIHint("StringReadonly")]
        public override string CommodityCode { get; set; }

        [Display(Name = "Tên NVL")]
        [UIHint("StringReadonly")]
        public override string CommodityName { get; set; }

        public int ShiftID { get; set; }
        public int WorkshiftID { get; set; }

        public int ProductionLineID { get; set; }
        public int CrucialWorkerID { get; set; }

        [UIHint("DateTime")]
        [Display(Name = "Bắt đầu")]
        public Nullable<System.DateTime> StartDate { get; set; }
        [UIHint("DateTime")]
        [Display(Name = "Kết thúc")]
        public Nullable<System.DateTime> StopDate { get; set; }
        [UIHint("Integer")]
        [Display(Name = "Nhiệt độ")]
        public int Temperature { get; set; }

        [Display(Name = "Cái/ tấm")]
        [UIHint("QuantityReadonly")]
        public decimal MoldQuantity { get; set; }

        [Display(Name = "Tồn lệnh")]
        [UIHint("QuantityReadonly")]
        public decimal QuantityRemains { get; set; }
        [Display(Name = "Tồn NVL")]
        [UIHint("QuantityReadonly")]
        public decimal MaterialIssueRemains { get; set; }

        [Display(Name = "KL hỗn hợp")]
        [UIHint("Quantity")]
        public override decimal Quantity { get; set; }
        [Display(Name = "Hao hụt")]
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
