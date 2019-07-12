﻿using System.Net;
using System.Web.Mvc;
using System.Text;

using RequireJsNet;

using TotalBase.Enums;
using TotalDTO.Commons;
using TotalModel.Models;

using TotalCore.Services.Commons;

using TotalPortal.Controllers;
using TotalPortal.Areas.Commons.ViewModels;
using TotalPortal.Areas.Commons.Builders;

namespace TotalPortal.Areas.Commons.Controllers
{
    public class BomsController : GenericViewDetailController<Bom, BomDetail, BomViewDetail, BomDTO, BomPrimitiveDTO, BomDetailDTO, BomViewModel>
    {
        public BomsController(IBomService bomService, IBomViewModelSelectListBuilder bomViewModelSelectListBuilder)
            : base(bomService, bomViewModelSelectListBuilder)
        {
        }

        public override void AddRequireJsOptions()
        {
            base.AddRequireJsOptions();

            StringBuilder commodityTypeIDList = new StringBuilder();
            commodityTypeIDList.Append((int)GlobalEnums.CommodityTypeID.Materials);

            RequireJsOptions.Add("commodityTypeIDList", commodityTypeIDList.ToString(), RequireJsOptionsScope.Page);

            RequireJsOptions.Add("masterCommodityTypeIDs", ((int)GlobalEnums.CommodityTypeID.Items).ToString(), RequireJsOptionsScope.Page);
        }

        public virtual ActionResult Commodities(int id)
        {
            BomViewModel bomViewModel = this.GetViewModel(id, GlobalEnums.AccessLevel.Readable);
            if (bomViewModel == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            return View(bomViewModel);
        }
    }

}