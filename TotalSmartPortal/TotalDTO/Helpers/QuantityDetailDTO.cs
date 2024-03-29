﻿using System.ComponentModel.DataAnnotations;

using TotalModel;

namespace TotalDTO.Helpers
{
    public interface IQuantityDetailDTO : IBaseModel
    {
        int CommodityID { get; set; }
        string CommodityCode { get; set; }
        string CommodityName { get; set; }
        string SalesUnit { get; set; }
        int CommodityTypeID { get; set; }

        decimal Quantity { get; set; }
    }

    public abstract class QuantityDetailDTO : BaseModel, IQuantityDetailDTO, IBaseModel
    {
        public virtual int CommodityID { get; set; }
        
        //[UIHint("StringReadonly")]  Note: Must set later for any derived class, event for readonly attribute. Don't know why can not override this attribute when needed only
        [Display(Name = "Mã hàng")]
        [Required(ErrorMessage = "Vui lòng chọn mặt hàng")]        
        public virtual string CommodityCode { get; set; }

        [Display(Name = "Tên hàng")]
        [UIHint("StringReadonly")]
        public virtual string CommodityName { get; set; }

        [Display(Name = "ĐVT")]
        [UIHint("StringReadonly")]
        public virtual string SalesUnit { get; set; }

        [Display(Name = "TL Net")]
        [UIHint("DecimalN0Readonly")]
        public virtual decimal Weight { get; set; }

        [Range(1, 99999999999, ErrorMessage = "Lỗi bắt buộc phải có id loại hàng hóa")]
        [Required(ErrorMessage = "Lỗi bắt buộc phải có loại hàng hóa")]
        public virtual int CommodityTypeID { get; set; }

        [Display(Name = "P/P")]
        public virtual int PiecePerPack { get; set; }

        [Display(Name = "SL")]                
        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        public virtual decimal Quantity { get; set; }
    }
}
