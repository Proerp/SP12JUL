﻿using System.Net;
using System.Web.Mvc;
using System.Text;

using RequireJsNet;

using TotalBase.Enums;
using TotalModel;
using TotalDTO;
using TotalDTO.Inventories;
using TotalModel.Models;
using TotalPortal.ViewModels.Helpers;

using TotalCore.Services.Inventories;

using TotalPortal.Controllers;
using TotalPortal.Areas.Inventories.ViewModels;
using TotalPortal.Areas.Inventories.Builders;
using TotalPortal.Areas.Commons.Controllers.Sessions;
using TotalPortal.Areas.Inventories.Controllers.Sessions;
using TotalPortal.APIs.Sessions;

namespace TotalPortal.Areas.Inventories.Controllers
{
    public class WarehouseTransfersController<TDto, TPrimitiveDto, TDtoDetail, TViewDetailViewModel> : GenericViewDetailController<WarehouseTransfer, WarehouseTransferDetail, WarehouseTransferViewDetail, TDto, TPrimitiveDto, TDtoDetail, TViewDetailViewModel>
        where TDto : TPrimitiveDto, IBaseDetailEntity<TDtoDetail>
        where TPrimitiveDto : BaseDTO, IPrimitiveEntity, IPrimitiveDTO, new()
        where TDtoDetail : class, IPrimitiveEntity
        where TViewDetailViewModel : TDto, IViewDetailViewModel<TDtoDetail>, IWarehouseTransferViewModel, new()
    {
        public WarehouseTransfersController(IWarehouseTransferService<TDto, TPrimitiveDto, TDtoDetail> transferOrderService, IWarehouseTransferViewModelSelectListBuilder<TViewDetailViewModel> transferOrderViewModelSelectListBuilder)
            : base(transferOrderService, transferOrderViewModelSelectListBuilder, true)
        {
        }

        public override void AddRequireJsOptions()
        {
            base.AddRequireJsOptions();

            TViewDetailViewModel viewDetailViewModel = new TViewDetailViewModel();

            StringBuilder commodityTypeIDList = new StringBuilder();
            commodityTypeIDList.Append((int)(viewDetailViewModel.IsMaterial ? GlobalEnums.CommodityTypeID.Materials : (viewDetailViewModel.IsItem ? GlobalEnums.CommodityTypeID.Items : (viewDetailViewModel.IsProduct ? GlobalEnums.CommodityTypeID.Products : GlobalEnums.CommodityTypeID.Unknown))));

            RequireJsOptions.Add("commodityTypeIDList", commodityTypeIDList.ToString(), RequireJsOptionsScope.Page);


            StringBuilder warehouseTaskIDList = new StringBuilder();
            warehouseTaskIDList.Append((int)(viewDetailViewModel.IsMaterial ? GlobalEnums.WarehouseTaskID.MaterialAdjustment : (viewDetailViewModel.IsItem ? GlobalEnums.WarehouseTaskID.ItemAdjustment : (viewDetailViewModel.IsProduct ? GlobalEnums.WarehouseTaskID.ProductAdjustment : GlobalEnums.WarehouseTaskID.Unknown))));

            ViewBag.WarehouseTaskID = (int)(viewDetailViewModel.IsMaterial ? GlobalEnums.WarehouseTaskID.MaterialAdjustment : (viewDetailViewModel.IsItem ? GlobalEnums.WarehouseTaskID.ItemAdjustment : (viewDetailViewModel.IsProduct ? GlobalEnums.WarehouseTaskID.ProductAdjustment : GlobalEnums.WarehouseTaskID.Unknown)));
            ViewBag.WarehouseTaskIDList = warehouseTaskIDList.ToString();
        }

        public virtual ActionResult GetGoodsReceiptDetailAvailables()
        {
            this.AddRequireJsOptions();
            TViewDetailViewModel viewDetailViewModel = new TViewDetailViewModel();
            return View("../WarehouseTransfers/GetGoodsReceiptDetailAvailables", this.InitViewModel(viewDetailViewModel));
        }

        public virtual ActionResult GetTransferOrderDetails()
        {
            this.AddRequireJsOptions();
            TViewDetailViewModel viewDetailViewModel = new TViewDetailViewModel();
            return View(this.InitViewModel(viewDetailViewModel));
        }


        protected override TViewDetailViewModel InitViewModelByDefault(TViewDetailViewModel simpleViewModel)
        {
            simpleViewModel = base.InitViewModelByDefault(simpleViewModel);

            if (((IWarehouseTransferPrimitiveDTO)simpleViewModel).ShiftID == 0)
            {
                string shiftSession = ShiftSession.GetShift(this.HttpContext);
                if (HomeSession.TryParseID(shiftSession) > 0) ((IWarehouseTransferPrimitiveDTO)simpleViewModel).ShiftID = (int)HomeSession.TryParseID(shiftSession);
            }

            if (simpleViewModel.Storekeeper == null)
            {
                string storekeeperSession = WarehouseTransferSession.GetStorekeeper(this.HttpContext);

                if (HomeSession.TryParseID(storekeeperSession) > 0)
                {
                    simpleViewModel.Storekeeper = new TotalDTO.Commons.EmployeeBaseDTO();
                    simpleViewModel.Storekeeper.EmployeeID = (int)HomeSession.TryParseID(storekeeperSession);
                    simpleViewModel.Storekeeper.Name = HomeSession.TryParseName(storekeeperSession);
                }
            }

            return simpleViewModel;
        }

        protected override void BackupViewModelToSession(TViewDetailViewModel simpleViewModel)
        {
            base.BackupViewModelToSession(simpleViewModel);
            ShiftSession.SetShift(this.HttpContext, ((IWarehouseTransferPrimitiveDTO)simpleViewModel).ShiftID);
            WarehouseTransferSession.SetStorekeeper(this.HttpContext, simpleViewModel.Storekeeper.EmployeeID, simpleViewModel.Storekeeper.Name);
        }

        protected override PrintViewModel InitPrintViewModel(int? id, int? detailID)
        {
            PrintViewModel printViewModel = base.InitPrintViewModel(id, detailID);

            TViewDetailViewModel viewDetailViewModel = this.GetViewModel(id, GlobalEnums.AccessLevel.Readable, true); if (viewDetailViewModel == null) printViewModel.Id = 0;

            printViewModel.PrintOptionID = viewDetailViewModel.Approved ? 1 : 0;
            printViewModel.ReportPath = viewDetailViewModel.IsMaterial ? "MaterialTransferSheet" : (viewDetailViewModel.IsItem ? "ItemTransferSheet" : (viewDetailViewModel.IsProduct ? "ProductTransferSheet" : ""));

            return printViewModel;
        }
    }





    public class MaterialTransfersController : WarehouseTransfersController<WarehouseTransferDTO<WTOptionMaterial>, WarehouseTransferPrimitiveDTO<WTOptionMaterial>, WarehouseTransferDetailDTO, MaterialTransferViewModel>
    {
        public MaterialTransfersController(IMaterialTransferService materialTransferService, IMaterialTransferViewModelSelectListBuilder materialTransferViewModelSelectListBuilder)
            : base(materialTransferService, materialTransferViewModelSelectListBuilder)
        {
        }
    }

    public class ItemTransfersController : WarehouseTransfersController<WarehouseTransferDTO<WTOptionItem>, WarehouseTransferPrimitiveDTO<WTOptionItem>, WarehouseTransferDetailDTO, ItemTransferViewModel>
    {
        public ItemTransfersController(IItemTransferService itemTransferService, IItemTransferViewModelSelectListBuilder itemTransferViewModelSelectListBuilder)
            : base(itemTransferService, itemTransferViewModelSelectListBuilder)
        {
        }
    }

    public class HeapTransfersController : ItemTransfersController
    {
        public override GlobalEnums.NmvnTaskID ModuleDetailID { get { return GlobalEnums.NmvnTaskID.HeapTransfer; } }

        public HeapTransfersController(IItemTransferService itemTransferService, IItemTransferViewModelSelectListBuilder itemTransferViewModelSelectListBuilder)
            : base(itemTransferService, itemTransferViewModelSelectListBuilder)
        {
        }

        protected override ItemTransferViewModel NewViewModel()
        {
            ItemTransferViewModel itemTransferViewModel = base.NewViewModel();
            itemTransferViewModel.ModuleDetailID = this.ModuleDetailID;
            return itemTransferViewModel;
        }
    }

    public class ProductTransfersController : WarehouseTransfersController<WarehouseTransferDTO<WTOptionProduct>, WarehouseTransferPrimitiveDTO<WTOptionProduct>, WarehouseTransferDetailDTO, ProductTransferViewModel>
    {
        public ProductTransfersController(IProductTransferService productTransferService, IProductTransferViewModelSelectListBuilder productTransferViewModelSelectListBuilder)
            : base(productTransferService, productTransferViewModelSelectListBuilder)
        {
        }
    }
}