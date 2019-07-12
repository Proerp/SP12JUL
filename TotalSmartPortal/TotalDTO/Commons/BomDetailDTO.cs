﻿using System;
using System.ComponentModel.DataAnnotations;

using TotalModel;
using TotalDTO.Helpers;

namespace TotalDTO.Commons
{
    public class BomDetailDTO : QuantityDetailDTO, IPrimitiveEntity
    {
        public int GetID() { return this.BomDetailID; }

        public int BomDetailID { get; set; }
        public int BomID { get; set; }

        public int MaterialID { get { return this.CommodityID; } }

        [Required(ErrorMessage = "Nhập TV")]
        [Display(Name = "Trục")]
        public string LayerCode { get { return this.layerCode; } set { this.layerCode = (value != null ? value.ToUpper() : value); } }
        private string layerCode;

        [Display(Name = "Mã NVL")]
        [UIHint("AutoCompletes/CommodityBase")]
        public override string CommodityCode { get; set; }
        [Display(Name = "Tên NVL")]
        public override string CommodityName { get; set; }

        public decimal UnitRate { get { return this.SalesUnit.ToUpper() == "GR" ? 1000 : 1; } }

        [Display(Name = "KL")]
        [UIHint("Quantity")]
        [Range(0, 99999999999, ErrorMessage = "Số lượng không hợp lệ")]
        public override decimal Quantity { get; set; }

        public decimal BlockQuantity { get { return this.Quantity; } set { } }
        public decimal LayerQuantity { get; set; }

        [Display(Name = "%")]
        [UIHint("DecimalN0")]
        [Range(1, 100, ErrorMessage = "Vui lòng nhập %")]
        public decimal BlockUnit { get; set; }

        public bool MajorStaple { get; set; }
    }
}
