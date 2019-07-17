﻿using System;
using System.Collections.Generic;

using TotalModel.Models;

namespace TotalCore.Repositories.Inventories
{
    public interface IGoodsReceiptRepository : IGenericWithDetailRepository<GoodsReceipt, GoodsReceiptDetail>
    {
    }

    public interface IGoodsReceiptAPIRepository : IGenericAPIRepository
    {
        string GetReference(int goodsReceiptID);

        IEnumerable<GoodsReceiptPendingCustomer> GetCustomers(int? locationID);
        IEnumerable<GoodsReceiptPendingPurchaseRequisition> GetPurchaseRequisitions(int? locationID);
        IEnumerable<GoodsReceiptPendingPurchaseRequisitionDetail> GetPendingPurchaseRequisitionDetails(int? locationID, int? goodsReceiptID, int? purchaseRequisitionID, int? customerID, string purchaseRequisitionDetailIDs, bool isReadonly);

        IEnumerable<GoodsReceiptPendingPurchasing> GetPurchasings(int? locationID, int? nmvnTaskID);
        IEnumerable<GoodsReceiptPendingGoodsArrival> GetGoodsArrivals(int? locationID, int? nmvnTaskID);
        IEnumerable<GoodsReceiptPendingGoodsArrivalPackage> GetPendingGoodsArrivalPackages(bool webAPI, int? locationID, int? nmvnTaskID, int? goodsReceiptID, int? goodsArrivalID, string barcode, string goodsArrivalPackageIDs);

        IEnumerable<GoodsReceiptPendingWarehouse> GetWarehouses(int? locationID, int? nmvnTaskID);
        IEnumerable<GoodsReceiptPendingWarehouseTransfer> GetWarehouseTransfers(int? locationID, int? nmvnTaskID);
        List<GoodsReceiptPendingWarehouseTransferDetail> GetPendingWarehouseTransferDetails(int? nmvnTaskID, int? goodsReceiptID, int? warehouseTransferID, int? warehouseID, int? warehouseIssueID, string warehouseTransferDetailIDs, bool oneStep);



        IEnumerable<GoodsReceiptPendingPlannedOrderCustomer> GetPlannedOrderCustomers(int? locationID);
        IEnumerable<GoodsReceiptPendingPlannedOrder> GetPlannedOrders(int? locationID);
        IEnumerable<GoodsReceiptPendingPlannedOrderDetail> GetPendingPlannedOrderDetails(int? locationID, int? goodsReceiptID, int? plannedOrderID, int? customerID, string finishedProductPackageIDs, bool isReadonly);

        IEnumerable<GoodsReceiptPendingPlannedItemCustomer> GetPlannedItemCustomers(int? locationID);
        IEnumerable<GoodsReceiptPendingPlannedItem> GetPlannedItems(int? locationID);
        IEnumerable<GoodsReceiptPendingPlannedItemDetail> GetPendingPlannedItemDetails(int? locationID, int? goodsReceiptID, int? plannedOrderID, int? customerID, string finishedItemPackageIDs);

        
        IEnumerable<GoodsReceiptPendingRecyclate> GetRecyclates(int? locationID);
        IEnumerable<GoodsReceiptPendingRecyclateDetail> GetPendingRecyclateDetails(int? locationID, int? goodsReceiptID, int? recyclateID, string recyclatePackageIDs);


        IEnumerable<GoodsReceiptPendingMaterialIssueDetail> GetPendingMaterialIssueDetails(int? locationID, int? goodsReceiptID, string materialIssueDetailIDs, bool isReadonly);



        List<PendingWarehouseAdjustmentDetail> GetPendingWarehouseAdjustmentDetails(int? locationID, int? goodsReceiptID, int? warehouseAdjustmentID, int? warehouseID, string warehouseAdjustmentDetailIDs, bool isReadonly);
        
        
        int? GetGoodsReceiptID(int? goodsArrivalID, int? plannedOrderID, int? warehouseTransferID, int? warehouseAdjustmentID);

        IEnumerable<GoodsReceiptDetailAvailable> GetGoodsReceiptDetailAvailables(int? locationID, int? warehouseID, int? warehouseReceiptID, int? commodityID, string commodityIDs, int? batchID, int? blendingInstructionID, string barcode, string goodsReceiptDetailIDs, bool onlyApproved, bool onlyIssuable);        
    }

}
