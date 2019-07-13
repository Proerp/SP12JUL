﻿using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using TotalModel;
using TotalBase.Enums;
using TotalDTO.Helpers;
using TotalDTO.Commons;

namespace TotalDTO.Productions
{
    public class BlendingInstructionPrimitiveDTO : QuantityDTO<BlendingInstructionDetailDTO>, IPrimitiveEntity, IPrimitiveDTO
    {
        public override string VoidWarning { get { return "Thanh lý"; } }

        public override GlobalEnums.NmvnTaskID NMVNTaskID { get { return GlobalEnums.NmvnTaskID.BlendingInstruction; } }

        public int GetID() { return this.BlendingInstructionID; }
        public void SetID(int id) { this.BlendingInstructionID = id; }

        public int BlendingInstructionID { get; set; }

        [Display(Name = "Số lô, batch number")]
        [Required(ErrorMessage = "Vui lòng nhập số lô")]
        public string Code { get; set; }

        [Display(Name = "Ngày chứng từ")]
        public Nullable<System.DateTime> VoucherDate { get; set; }

        [Display(Name = "Mô tả lệnh pha chế")]
        public string Jobs { get; set; }

        public virtual int CommodityID { get; set; }

        public override void PerformPresaveRule()
        {
            base.PerformPresaveRule();

            string caption = "";
            this.DtoDetails().ToList().ForEach(e => { e.ParentID = this.ParentID; e.Code = this.Code; if (caption.IndexOf(e.CommodityName) < 0) caption = caption + (caption != "" ? ", " : "") + e.CommodityName; });
            this.Caption = caption != "" ? (caption.Length > 98 ? caption.Substring(0, 95) + "..." : caption) : null;
        }
    }

    public class BlendingInstructionDTO : BlendingInstructionPrimitiveDTO, IBaseDetailEntity<BlendingInstructionDetailDTO>
    {
        public BlendingInstructionDTO()
        {
            this.BlendingInstructionViewDetails = new List<BlendingInstructionDetailDTO>();
        }

        public override int CommodityID { get { return (this.Commodity != null ? this.Commodity.CommodityID : 0); } }
        [Display(Name = "Thành phẩm")]
        [UIHint("Commons/Commodity")]
        public CommodityBaseDTO Commodity { get; set; }

        public override Nullable<int> VoidTypeID { get { return (this.VoidType != null ? this.VoidType.VoidTypeID : null); } }
        [UIHint("AutoCompletes/VoidType")]
        public VoidTypeBaseDTO VoidType { get; set; }

        public List<BlendingInstructionDetailDTO> BlendingInstructionViewDetails { get; set; }
        public List<BlendingInstructionDetailDTO> ViewDetails { get { return this.BlendingInstructionViewDetails; } set { this.BlendingInstructionViewDetails = value; } }

        public ICollection<BlendingInstructionDetailDTO> GetDetails() { return this.BlendingInstructionViewDetails; }

        protected override IEnumerable<BlendingInstructionDetailDTO> DtoDetails() { return this.BlendingInstructionViewDetails; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            foreach (var result in base.Validate(validationContext)) { yield return result; }
            if (this.ViewDetails.Count == 0) yield return new ValidationResult("Vui lòng nhập chi tiết NVL.", new[] { "BOM" });
        }

        public override void PrepareVoidDetail(int? detailID)
        {
            this.ViewDetails.RemoveAll(w => w.BlendingInstructionDetailID != detailID);
            if (this.ViewDetails.Count() > 0)
                this.VoidType = new VoidTypeBaseDTO() { VoidTypeID = this.ViewDetails[0].VoidTypeID, Code = this.ViewDetails[0].VoidTypeCode, Name = this.ViewDetails[0].VoidTypeName, VoidClassID = this.ViewDetails[0].VoidClassID };
            base.PrepareVoidDetail(detailID);
        }

        public override void PrepareRemarkDetail(int? detailID)
        {
            this.ViewDetails.RemoveAll(w => w.BlendingInstructionDetailID != detailID);
            base.PrepareRemarkDetail(detailID);
        }
    }

}
