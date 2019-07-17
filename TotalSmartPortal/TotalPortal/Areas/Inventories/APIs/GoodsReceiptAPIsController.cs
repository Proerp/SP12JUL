using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Collections.Generic;
using System.Web.UI;

using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;


using TotalBase.Enums;
using TotalModel.Models;

using TotalDTO.Inventories;

using TotalCore.Repositories.Inventories;
using TotalPortal.Areas.Inventories.ViewModels;
using TotalPortal.APIs.Sessions;

using Microsoft.AspNet.Identity;

namespace TotalPortal.Areas.Inventories.APIs
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class GoodsReceiptAPIsController : Controller
    {
        private readonly IGoodsReceiptAPIRepository goodsReceiptAPIRepository;

        public GoodsReceiptAPIsController(IGoodsReceiptAPIRepository goodsReceiptAPIRepository)
        {
            this.goodsReceiptAPIRepository = goodsReceiptAPIRepository;
        }


        public JsonResult GetGoodsReceiptIndexes([DataSourceRequest] DataSourceRequest request, string nmvnTaskID, int moduleDetailID)
        {
            this.goodsReceiptAPIRepository.RepositoryBag["NMVNTaskID"] = nmvnTaskID;
            this.goodsReceiptAPIRepository.RepositoryBag["ModuleDetailID"] = moduleDetailID;
            ICollection<GoodsReceiptIndex> goodsReceiptIndexes = this.goodsReceiptAPIRepository.GetEntityIndexes<GoodsReceiptIndex>(User.Identity.GetUserId(), HomeSession.GetGlobalFromDate(this.HttpContext), HomeSession.GetGlobalToDate(this.HttpContext));

            DataSourceResult response = goodsReceiptIndexes.ToDataSourceResult(request);

            return Json(response, JsonRequestBehavior.AllowGet);
        }





        public JsonResult GetCustomers([DataSourceRequest] DataSourceRequest dataSourceRequest, int? locationID)
        {
            var result = this.goodsReceiptAPIRepository.GetCustomers(locationID);
            return Json(result.ToDataSourceResult(dataSourceRequest), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPurchaseRequisitions([DataSourceRequest] DataSourceRequest dataSourceRequest, int? locationID)
        {
            var result = this.goodsReceiptAPIRepository.GetPurchaseRequisitions(locationID);
            return Json(result.ToDataSourceResult(dataSourceRequest), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPendingPurchaseRequisitionDetails([DataSourceRequest] DataSourceRequest dataSourceRequest, int? locationID, int? goodsReceiptID, int? purchaseRequisitionID, int? customerID, string purchaseRequisitionDetailIDs, bool isReadonly)
        {
            var result = this.goodsReceiptAPIRepository.GetPendingPurchaseRequisitionDetails(locationID, goodsReceiptID, purchaseRequisitionID, customerID, purchaseRequisitionDetailIDs, false);
            return Json(result.ToDataSourceResult(dataSourceRequest), JsonRequestBehavior.AllowGet);
        }






        public JsonResult GetPurchasings([DataSourceRequest] DataSourceRequest dataSourceRequest, int? locationID, int? nmvnTaskID)
        {
            var result = this.goodsReceiptAPIRepository.GetPurchasings(locationID, nmvnTaskID);
            return Json(result.ToDataSourceResult(dataSourceRequest), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGoodsArrivals([DataSourceRequest] DataSourceRequest dataSourceRequest, int? locationID, int? nmvnTaskID)
        {
            var result = this.goodsReceiptAPIRepository.GetGoodsArrivals(locationID, nmvnTaskID);
            return Json(result.ToDataSourceResult(dataSourceRequest), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPendingGoodsArrivalPackages([DataSourceRequest] DataSourceRequest dataSourceRequest, int? locationID, int? nmvnTaskID, int? goodsReceiptID, int? goodsArrivalID, string barcode, string goodsArrivalPackageIDs)
        {
            var result = this.goodsReceiptAPIRepository.GetPendingGoodsArrivalPackages(false, locationID, nmvnTaskID, goodsReceiptID, goodsArrivalID, barcode, goodsArrivalPackageIDs);
            return Json(result.ToDataSourceResult(dataSourceRequest), JsonRequestBehavior.AllowGet);
        }






        public JsonResult GetWarehouses([DataSourceRequest] DataSourceRequest dataSourceRequest, int? locationID, int? nmvnTaskID)
        {
            var result = this.goodsReceiptAPIRepository.GetWarehouses(locationID, nmvnTaskID);
            return Json(result.ToDataSourceResult(dataSourceRequest), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWarehouseTransfers([DataSourceRequest] DataSourceRequest dataSourceRequest, int? locationID, int? nmvnTaskID)
        {
            var result = this.goodsReceiptAPIRepository.GetWarehouseTransfers(locationID, nmvnTaskID);
            return Json(result.ToDataSourceResult(dataSourceRequest), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPendingWarehouseTransferDetails([DataSourceRequest] DataSourceRequest dataSourceRequest, int? nmvnTaskID, int? goodsReceiptID, int? warehouseTransferID, int? warehouseID, int? warehouseIssueID, string warehouseTransferDetailIDs, bool oneStep)
        {
            var result = this.goodsReceiptAPIRepository.GetPendingWarehouseTransferDetails(nmvnTaskID, goodsReceiptID, warehouseTransferID, warehouseID, warehouseIssueID, warehouseTransferDetailIDs, oneStep);
            return Json(result.ToDataSourceResult(dataSourceRequest), JsonRequestBehavior.AllowGet);
        }





        public JsonResult GetPlannedOrderCustomers([DataSourceRequest] DataSourceRequest dataSourceRequest, int? locationID)
        {
            var result = this.goodsReceiptAPIRepository.GetPlannedOrderCustomers(locationID);
            return Json(result.ToDataSourceResult(dataSourceRequest), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPlannedOrders([DataSourceRequest] DataSourceRequest dataSourceRequest, int? locationID)
        {
            var result = this.goodsReceiptAPIRepository.GetPlannedOrders(locationID);
            return Json(result.ToDataSourceResult(dataSourceRequest), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPendingPlannedOrderDetails([DataSourceRequest] DataSourceRequest dataSourceRequest, int? locationID, int? goodsReceiptID, int? plannedOrderID, int? customerID, string finishedProductPackageIDs, bool isReadonly)
        {
            var result = this.goodsReceiptAPIRepository.GetPendingPlannedOrderDetails(locationID, goodsReceiptID, plannedOrderID, customerID, finishedProductPackageIDs, false);
            return Json(result.ToDataSourceResult(dataSourceRequest), JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetPlannedItemCustomers([DataSourceRequest] DataSourceRequest dataSourceRequest, int? locationID)
        {
            var result = this.goodsReceiptAPIRepository.GetPlannedItemCustomers(locationID);
            return Json(result.ToDataSourceResult(dataSourceRequest), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPlannedItems([DataSourceRequest] DataSourceRequest dataSourceRequest, int? locationID)
        {
            var result = this.goodsReceiptAPIRepository.GetPlannedItems(locationID);
            return Json(result.ToDataSourceResult(dataSourceRequest), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPendingPlannedItemDetails([DataSourceRequest] DataSourceRequest dataSourceRequest, int? locationID, int? goodsReceiptID, int? plannedOrderID, int? customerID, string finishedItemPackageIDs)
        {
            var result = this.goodsReceiptAPIRepository.GetPendingPlannedItemDetails(locationID, goodsReceiptID, plannedOrderID, customerID, finishedItemPackageIDs);
            return Json(result.ToDataSourceResult(dataSourceRequest), JsonRequestBehavior.AllowGet);
        }


        
        public JsonResult GetRecyclates([DataSourceRequest] DataSourceRequest dataSourceRequest, int? locationID)
        {
            var result = this.goodsReceiptAPIRepository.GetRecyclates(locationID);
            return Json(result.ToDataSourceResult(dataSourceRequest), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPendingRecyclateDetails([DataSourceRequest] DataSourceRequest dataSourceRequest, int? locationID, int? goodsReceiptID, int? recyclateID, string recyclatePackageIDs)
        {
            var result = this.goodsReceiptAPIRepository.GetPendingRecyclateDetails(locationID, goodsReceiptID, recyclateID, recyclatePackageIDs);
            return Json(result.ToDataSourceResult(dataSourceRequest), JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetPendingMaterialIssueDetails([DataSourceRequest] DataSourceRequest dataSourceRequest, int? locationID, int? goodsReceiptID, string materialIssueDetailIDs, bool isReadonly)
        {
            var result = this.goodsReceiptAPIRepository.GetPendingMaterialIssueDetails(locationID, goodsReceiptID, materialIssueDetailIDs, false);
            return Json(result.ToDataSourceResult(dataSourceRequest), JsonRequestBehavior.AllowGet);
        }






        public JsonResult GetGoodsReceiptDetailAvailables([DataSourceRequest] DataSourceRequest dataSourceRequest, int? locationID, int? warehouseID, int? warehouseReceiptID, int? commodityID, string commodityIDs, int? batchID, int? blendingInstructionID, string barcode, string goodsReceiptDetailIDs, bool onlyApproved, bool onlyIssuable)
        {
            var result = this.goodsReceiptAPIRepository.GetGoodsReceiptDetailAvailables(locationID, warehouseID, warehouseReceiptID, commodityID, commodityIDs, batchID, blendingInstructionID, barcode, goodsReceiptDetailIDs, onlyApproved, onlyIssuable);
            return Json(result.ToDataSourceResult(dataSourceRequest), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This function is the same as GetGoodsReceiptDetailAvailables, BUT: it is designed for importinng
        /// </summary>
        /// <param name="locationID"></param>
        /// <param name="warehouseID"></param>
        /// <param name="warehouseReceiptID"></param>
        /// <param name="commodityID"></param>
        /// <param name="commodityIDs"></param>
        /// <param name="batchID"></param>
        /// <param name="blendingInstructionID"></param>
        /// <param name="barcode"></param>
        /// <param name="goodsReceiptDetailIDs"></param>
        /// <param name="onlyApproved"></param>
        /// <param name="onlyIssuable"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ImportGoodsReceiptDetailAvailables(int? locationID, int? warehouseID, int? warehouseReceiptID, int? commodityID, string commodityIDs, int? batchID, int? blendingInstructionID, string barcode, string goodsReceiptDetailIDs, bool onlyApproved, bool onlyIssuable)
        {
            try
            {
                var result = this.goodsReceiptAPIRepository.GetGoodsReceiptDetailAvailables(locationID, warehouseID, warehouseReceiptID, commodityID, commodityIDs, batchID, blendingInstructionID, barcode, goodsReceiptDetailIDs, onlyApproved, onlyIssuable);
                if (result.Count() > 0) 
                    return Json(result.First(), JsonRequestBehavior.AllowGet); 
                else
                {
                    int? foundCommodityID = null; string message = "";
                    this.goodsReceiptAPIRepository.BarcodeNotFoundMessage(out foundCommodityID, out message, false, locationID, warehouseID, warehouseReceiptID, commodityID, commodityIDs, batchID, blendingInstructionID, barcode, goodsReceiptDetailIDs, onlyApproved, onlyIssuable);

                    return Json(new GoodsReceiptDetailAvailable() { CommodityName = message != "" ? message : "Mã vạch không đúng hoặc không phù hợp." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new GoodsReceiptDetailAvailable() { CommodityName = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
