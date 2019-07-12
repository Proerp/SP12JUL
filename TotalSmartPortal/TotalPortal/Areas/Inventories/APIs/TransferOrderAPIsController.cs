﻿using System;
using System.Web.Mvc;
using System.Data.Entity;
using System.Collections.Generic;

using Microsoft.AspNet.Identity;

using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

using TotalModel.Models;
using TotalCore.Repositories.Inventories;

using TotalPortal.APIs.Sessions;

namespace TotalPortal.Areas.Inventories.APIs
{  
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class TransferOrderAPIsController : Controller
    {
        private readonly ITransferOrderAPIRepository transferOrderAPIRepository;

        public TransferOrderAPIsController(ITransferOrderAPIRepository transferOrderAPIRepository)
        {
            this.transferOrderAPIRepository = transferOrderAPIRepository;
        }


        public JsonResult GetTransferOrderIndexes([DataSourceRequest] DataSourceRequest request, bool withExtendedSearch, string nmvnTaskID, DateTime extendedFromDate, DateTime extendedToDate, int filterOptionID, int labOptionID)
        {
            this.transferOrderAPIRepository.RepositoryBag["NMVNTaskID"] = nmvnTaskID;
            this.transferOrderAPIRepository.RepositoryBag["LabOptionID"] = labOptionID;
            this.transferOrderAPIRepository.RepositoryBag["FilterOptionID"] = filterOptionID;
            ICollection<TransferOrderIndex> transferOrderIndexes = this.transferOrderAPIRepository.GetEntityIndexes<TransferOrderIndex>(User.Identity.GetUserId(), (withExtendedSearch ? extendedFromDate : HomeSession.GetGlobalFromDate(this.HttpContext)), (withExtendedSearch ? extendedToDate : HomeSession.GetGlobalToDate(this.HttpContext)));

            DataSourceResult response = transferOrderIndexes.ToDataSourceResult(request);

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAvailableWarehouses([DataSourceRequest] DataSourceRequest dataSourceRequest, int? locationID, int? nmvnTaskID)
        {
            var result = this.transferOrderAPIRepository.GetAvailableWarehouses(locationID, nmvnTaskID);
            return Json(result.ToDataSourceResult(dataSourceRequest), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTransferOrderPendingWorkOrders([DataSourceRequest] DataSourceRequest dataSourceRequest, int? locationID, int? transferOrderID, int? warehouseID, int? warehouseReceiptID, string commodityIDs)
        {
            var result = this.transferOrderAPIRepository.GetTransferOrderPendingWorkOrders(locationID, transferOrderID, warehouseID, warehouseReceiptID, commodityIDs);
            return Json(result.ToDataSourceResult(dataSourceRequest), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTransferOrderPendingBlendingInstructions([DataSourceRequest] DataSourceRequest dataSourceRequest, int? locationID, int? transferOrderID, int? warehouseID, int? warehouseReceiptID, string commodityIDs)
        {
            var result = this.transferOrderAPIRepository.GetTransferOrderPendingBlendingInstructions(locationID, transferOrderID, warehouseID, warehouseReceiptID, commodityIDs);
            return Json(result.ToDataSourceResult(dataSourceRequest), JsonRequestBehavior.AllowGet);
        }

    }
}