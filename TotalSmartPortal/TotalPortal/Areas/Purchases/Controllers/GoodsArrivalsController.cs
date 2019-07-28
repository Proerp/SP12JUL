using System;
using System.Net;
using System.Web.Mvc;
using System.Text;

using AutoMapper;
using RequireJsNet;

using TotalBase.Enums;

using TotalModel;
using TotalDTO;
using TotalModel.Models;

using TotalCore.Services.Purchases;

using TotalDTO.Purchases;

using TotalPortal.Controllers;
using TotalPortal.ViewModels.Helpers;
using TotalPortal.Areas.Purchases.ViewModels;
using TotalPortal.Areas.Purchases.Builders;

using TotalPortal.APIs.Sessions;

namespace TotalPortal.Areas.Purchases.Controllers
{
    public class GoodsArrivalsController<TDto, TPrimitiveDto, TDtoDetail, TViewDetailViewModel> : GenericViewDetailController<GoodsArrival, GoodsArrivalDetail, GoodsArrivalViewDetail, TDto, TPrimitiveDto, TDtoDetail, TViewDetailViewModel>
        where TDto : TPrimitiveDto, IBaseDetailEntity<TDtoDetail>
        where TPrimitiveDto : BaseDTO, IPrimitiveEntity, IPrimitiveDTO, new()
        where TDtoDetail : class, IPrimitiveEntity
        where TViewDetailViewModel : TDto, IViewDetailViewModel<TDtoDetail>, IGoodsArrivalViewModel, new()
    {
        private IGoodsArrivalService<TDto, TPrimitiveDto, TDtoDetail> goodsArrivalService;
        public GoodsArrivalsController(IGoodsArrivalService<TDto, TPrimitiveDto, TDtoDetail> goodsArrivalService, IGoodsArrivalViewModelSelectListBuilder<TViewDetailViewModel> goodsArrivalViewModelSelectListBuilder)
            : base(goodsArrivalService, goodsArrivalViewModelSelectListBuilder, true)
        {
            this.goodsArrivalService = goodsArrivalService;
        }

        public virtual ActionResult GetPendingPurchaseOrderDetails()
        {
            this.AddRequireJsOptions();
            return View();
        }

        [AccessLevelAuthorize(GlobalEnums.AccessLevel.Editable), ImportModelStateFromTempData]
        [OnResultExecutingFilterAttribute]
        public virtual ActionResult ChangeExpiryDate(int? id, int? detailID)
        {
            TViewDetailViewModel simpleViewModel = this.GetViewModel(id, GlobalEnums.AccessLevel.Readable, true);
            if (simpleViewModel == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            GoodsArrivalDetailDTO goodsArrivalDetailDTO = simpleViewModel.ViewDetails.Find(w => w.GetID() == detailID) as GoodsArrivalDetailDTO;
            ExpiryDateBbViewModel expiryDateBbViewModel = new ExpiryDateBbViewModel() { GoodsArrivalID = goodsArrivalDetailDTO.GoodsArrivalID, GoodsArrivalDetailID = goodsArrivalDetailDTO.GoodsArrivalDetailID, CommodityCode = goodsArrivalDetailDTO.CommodityCode, CommodityName = goodsArrivalDetailDTO.CommodityName, BatchCode = goodsArrivalDetailDTO.BatchCode, CurrentProductionDate = goodsArrivalDetailDTO.ProductionDate, CurrentExpiryDate = goodsArrivalDetailDTO.ExpiryDate, Remarks = goodsArrivalDetailDTO.Remarks };

            return View(expiryDateBbViewModel);
        }

        [HttpPost, ActionName("ChangeExpiryDate")]
        [ValidateAntiForgeryToken, ExportModelStateToTempData]
        public virtual ActionResult ChangeExpiryDateConfirmed(ExpiryDateViewModel expiryDateViewModel)
        {
            try
            {
                if (expiryDateViewModel.ExpiryDate == null || expiryDateViewModel.Remarks == null || expiryDateViewModel.Remarks.Trim() == "") throw new System.ArgumentException("Lỗi gia hạn sử dụng", "Vui lòng nhập ngày và số hồ sơ gia hạn.");

                GoodsArrival entity = this.GetEntityAndCheckAccessLevel(expiryDateViewModel.GoodsArrivalID, GlobalEnums.AccessLevel.Readable);
                if (entity == null) throw new System.ArgumentException("Lỗi gia hạn sử dụng", "BadRequest.");

                if (this.goodsArrivalService.ChangeExpiryDate(expiryDateViewModel.GoodsArrivalID, expiryDateViewModel.GoodsArrivalDetailID, expiryDateViewModel.ExpiryDate, expiryDateViewModel.Remarks))
                {
                    ModelState.Clear(); ////https://weblog.west-wind.com/posts/2012/apr/20/aspnet-mvc-postbacks-and-htmlhelper-controls-ignoring-model-changes
                    //IMPORTANT NOTES: ASP.NET MVC Postbacks and HtmlHelper Controls ignoring Model Changes
                    //HtmlHelpers controls (like .TextBoxFor() etc.) don't bind to model values on Postback, but rather get their value directly out of the POST buffer from ModelState. Effectively it looks like you can't change the display value of a control via model value updates on a Postback operation. 
                    //When MVC binds controls like @Html.TextBoxFor() or @Html.TextBox(), it always binds values on a GET operation. On a POST operation however, it'll always used the AttemptedValue to display the control. MVC binds using the ModelState on a POST operation, not the model's value
                    //So, if you want the behavior that I was expecting originally you can actually get it by clearing the ModelState in the controller code: ModelState.Clear();
                    //voidDetailViewModel.Remarks = voidDetailViewModel.Remarks;
                    return View("ChangeExpiryDateSuccess", expiryDateViewModel);
                }
                else
                    throw new System.ArgumentException("Lỗi gia hạn sử dụng", "Lỗi gia hạn sử dụng.");
            }
            catch (Exception exception)
            {
                ModelState.AddValidationErrors(exception);
                return RedirectToAction("ChangeExpiryDate", new { @id = expiryDateViewModel.GoodsArrivalID, @detailId = expiryDateViewModel.GoodsArrivalDetailID });
            }
        }
    }


    public class MaterialArrivalsController : GoodsArrivalsController<GoodsArrivalDTO<GoodsArrivalOptionMaterial>, GoodsArrivalPrimitiveDTO<GoodsArrivalOptionMaterial>, GoodsArrivalDetailDTO, MaterialArrivalViewModel>
    {
        public MaterialArrivalsController(IMaterialArrivalService materialArrivalService, IMaterialArrivalViewModelSelectListBuilder materialArrivalViewModelSelectListBuilder)
            : base(materialArrivalService, materialArrivalViewModelSelectListBuilder)
        {
        }
    }


    public class ItemArrivalsController : GoodsArrivalsController<GoodsArrivalDTO<GoodsArrivalOptionItem>, GoodsArrivalPrimitiveDTO<GoodsArrivalOptionItem>, GoodsArrivalDetailDTO, ItemArrivalViewModel>
    {
        public ItemArrivalsController(IItemArrivalService itemArrivalService, IItemArrivalViewModelSelectListBuilder itemArrivalViewModelSelectListBuilder)
            : base(itemArrivalService, itemArrivalViewModelSelectListBuilder)
        {
        }
    }


    public class ProductArrivalsController : GoodsArrivalsController<GoodsArrivalDTO<GoodsArrivalOptionProduct>, GoodsArrivalPrimitiveDTO<GoodsArrivalOptionProduct>, GoodsArrivalDetailDTO, ProductArrivalViewModel>
    {
        public ProductArrivalsController(IProductArrivalService productArrivalService, IProductArrivalViewModelSelectListBuilder productArrivalViewModelSelectListBuilder)
            : base(productArrivalService, productArrivalViewModelSelectListBuilder)
        {
        }
    }
}