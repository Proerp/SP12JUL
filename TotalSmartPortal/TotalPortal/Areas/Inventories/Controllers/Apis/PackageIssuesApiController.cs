﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.Collections.Generic;

using Microsoft.AspNet.Identity;


using TotalDTO;
using TotalModel;
using TotalModel.Models;

using TotalBase.Enums;
using TotalDTO.Inventories;

using TotalCore.Services.Inventories;
using TotalCore.Repositories.Commons;
using TotalCore.Repositories.Inventories;

using TotalPortal.APIs.Sessions;
using TotalPortal.Controllers.Apis;
using TotalPortal.ViewModels.Helpers;
using TotalPortal.Areas.Inventories.ViewModels;
using TotalPortal.Areas.Inventories.Builders;


namespace TotalPortal.Areas.Inventories.Controllers.Apis
{
    [RoutePrefix("Api/Inventories/PackageIssues")]
    public class PackageIssuesApiController : GenericViewDetailApiController<PackageIssue, PackageIssueDetail, PackageIssueViewDetail, PackageIssueDTO, PackageIssuePrimitiveDTO, PackageIssueDetailDTO, PackageIssueViewModel>
    {
        protected readonly IPackageIssueAPIRepository packageIssueAPIRepository;
        public PackageIssuesApiController(IPackageIssueService packageIssueService, IPackageIssueViewModelSelectListBuilder packageIssueViewModelSelectListBuilder, IPackageIssueAPIRepository packageIssueAPIRepository)
            : base(packageIssueService, packageIssueViewModelSelectListBuilder, true)
        {
            this.packageIssueAPIRepository = packageIssueAPIRepository;
        }

        protected override void ReloadAfterSave(PackageIssueViewModel simpleViewModel)
        {
            if (simpleViewModel.Reference == null)
            {
                simpleViewModel.Reference = this.packageIssueAPIRepository.GetReference(simpleViewModel.PackageIssueID);
            }

            simpleViewModel.ViewDetails.ForEach(e =>
                {
                    e.Base64Image1 = null;
                    e.Base64Image2 = null;
                });

            base.ReloadAfterSave(simpleViewModel);
        }

        [HttpGet]
        [Route("GetPackageIssueImage/{packageIssueImageID}")]
        public string GetPackageIssueImage(int packageIssueImageID)
        {
            return this.packageIssueAPIRepository.GetPackageIssueImage(packageIssueImageID);
        }

        [HttpGet]
        [Route("GetPackageIssueIndexes/{fromDay}/{fromMonth}/{fromYear}/{toDay}/{toMonth}/{toYear}")]
        public ICollection<PackageIssueIndex> GetPackageIssueIndexes(int fromDay, int fromMonth, int fromYear, int toDay, int toMonth, int toYear)
        {
            return this.packageIssueAPIRepository.GetEntityIndexes<PackageIssueIndex>(User.Identity.GetUserId(), Helpers.InitDateTime(fromYear, fromMonth, fromDay), Helpers.InitDateTime(toYear, toMonth, toDay, 23, 59, 59));
        }

        [HttpGet]
        [Route("GetBlendingInstructions/{locationID}/{blendingInstructionID}")]
        public IEnumerable<PackageIssuePendingBlendingInstruction> GetBlendingInstructions(int? locationID, int? blendingInstructionID)
        {
            return this.packageIssueAPIRepository.GetBlendingInstructions(locationID, blendingInstructionID);
        }

        [HttpGet]
        [Route("GetPendingBlendingInstructionDetails/{locationID}/{packageIssueID}/{blendingInstructionID}/{warehouseID}/{barcode}/{goodsReceiptDetailIDs}")]
        public HttpResponseMessage GetPendingBlendingInstructionDetails(int? locationID, int? packageIssueID, int? blendingInstructionID, int? warehouseID, string barcode, string goodsReceiptDetailIDs)
        {
            IEnumerable<PackageIssuePendingBlendingInstructionDetail> pendingBlendingInstructionDetails = this.packageIssueAPIRepository.GetPendingBlendingInstructionDetails(true, locationID, packageIssueID, blendingInstructionID, warehouseID, barcode, goodsReceiptDetailIDs);
            if (pendingBlendingInstructionDetails.Count() > 0 && barcode != null && barcode != "" && barcode != "0") pendingBlendingInstructionDetails = pendingBlendingInstructionDetails.Where(w => w.Barcode == barcode);

            if (pendingBlendingInstructionDetails.Count() > 0)
                return Request.CreateResponse(HttpStatusCode.OK, pendingBlendingInstructionDetails);
            else
            {
                int? foundCommodityID = null; string message = "";
                if (!this.packageIssueAPIRepository.BarcodeNotFoundMessage(out foundCommodityID, out message, false, locationID, warehouseID, 6, null, null, null, null, barcode, goodsReceiptDetailIDs, true, true))
                    if (foundCommodityID != null)
                    {
                        IEnumerable<PackageIssuePendingBlendingInstructionDetail> packageIssuePendingBlendingInstructionDetails = this.packageIssueAPIRepository.GetPendingBlendingInstructionDetails(true, locationID, packageIssueID, blendingInstructionID, warehouseID, null, goodsReceiptDetailIDs);

                        if (packageIssuePendingBlendingInstructionDetails.Where(w => w.CommodityID == foundCommodityID).Count() == 0) message = "Không có trong đơn hàng hoặc đã xuất đủ";
                    }
                return Request.CreateResponse(HttpStatusCode.NotFound, message != "" ? message : "Mã vạch không đúng hoặc không phù hợp.");
            }
        }


        #region HELPER API
        [HttpGet]
        [Route("GetPendingBlendingInstructionSummaries/{locationID}/{packageIssueID}/{blendingInstructionID}/{warehouseID}/{barcode}/{goodsReceiptDetailIDs}")]
        public IEnumerable<PendingBlendingInstructionSummary> GetPendingBlendingInstructionSummaries(int? locationID, int? packageIssueID, int? blendingInstructionID, int? warehouseID, string barcode, string goodsReceiptDetailIDs)
        {
            IEnumerable<PackageIssuePendingBlendingInstructionDetail> pendingBlendingInstructionSummaries = this.packageIssueAPIRepository.GetPendingBlendingInstructionDetails(true, locationID, packageIssueID, blendingInstructionID, warehouseID, barcode, goodsReceiptDetailIDs);
            return pendingBlendingInstructionSummaries.GroupBy(g => g.CommodityCode).Select(s => new PendingBlendingInstructionSummary() { CommodityCode = s.Key, Weight = s.Min(f => f.Weight), QuantityRemains = s.Max(f => f.QuantityRemains), QuantityRemainPackages = s.Max(f => f.QuantityRemainPackages), QuantityAvailables = s.Sum(f => f.QuantityAvailables) });
        }

        public class PendingBlendingInstructionSummary
        {
            public string CommodityCode { get; set; }
            public Nullable<decimal> Weight { get; set; }
            public Nullable<decimal> QuantityRemains { get; set; }
            public Nullable<decimal> QuantityRemainPackages { get; set; }
            public Nullable<decimal> QuantityAvailables { get; set; }
        }
        #endregion HELPER API

    }
}
