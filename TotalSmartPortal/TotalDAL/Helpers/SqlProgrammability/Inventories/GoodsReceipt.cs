﻿using System;
using System.Text;

using TotalBase;
using TotalBase.Enums;
using TotalModel.Models;

namespace TotalDAL.Helpers.SqlProgrammability.Inventories
{
    public class GoodsReceipt
    {
        private readonly TotalSmartPortalEntities totalSmartPortalEntities;

        public GoodsReceipt(TotalSmartPortalEntities totalSmartPortalEntities)
        {
            this.totalSmartPortalEntities = totalSmartPortalEntities;
        }

        public void RestoreProcedure()
        {
            this.GetGoodsReceiptIndexes();

            this.GetGoodsReceiptViewDetails();

            this.GetGoodsReceiptPendingPurchasings();
            this.GetGoodsReceiptPendingGoodsArrivals();
            this.GetGoodsReceiptPendingGoodsArrivalPackages();

            this.GetGoodsReceiptPendingCustomers();
            this.GetGoodsReceiptPendingPurchaseRequisitions();
            this.GetGoodsReceiptPendingPurchaseRequisitionDetails();

            this.GetGoodsReceiptPendingWarehouses();
            this.GetGoodsReceiptPendingWarehouseTransfers();
            this.GetGoodsReceiptPendingWarehouseTransferDetails();

            this.GetGoodsReceiptPendingPlannedOrders();
            this.GetGoodsReceiptPendingPlannedOrderCustomers();
            this.GetGoodsReceiptPendingPlannedOrderDetails();

            this.GetGoodsReceiptPendingPlannedItems();
            this.GetGoodsReceiptPendingPlannedItemCustomers();
            this.GetGoodsReceiptPendingPlannedItemDetails();

            this.GetGoodsReceiptPendingRecyclates();
            this.GetGoodsReceiptPendingRecyclateDetails();

            this.GetGoodsReceiptPendingMaterialIssueDetails();

            GenerateSQLPendingDetails generatePendingWarehouseAdjustmentDetails = new GenerateSQLPendingDetails(this.totalSmartPortalEntities, GlobalEnums.GoodsReceiptTypeID.WarehouseAdjustments, "WarehouseAdjustments", "WarehouseAdjustmentDetails", "WarehouseAdjustmentID", "@WarehouseAdjustmentID", "WarehouseAdjustmentDetailID", "@WarehouseAdjustmentDetailIDs", "WarehouseReceiptID", "PrimaryReference", "PrimaryEntryDate");
            generatePendingWarehouseAdjustmentDetails.GetPendingPickupDetails("GetPendingWarehouseAdjustmentDetails");

            this.GoodsReceiptSaveRelative();
            this.GoodsReceiptPostSaveValidate();

            this.GoodsReceiptApproved();
            this.GoodsReceiptEditable();

            this.GoodsReceiptToggleApproved();

            this.GoodsReceiptInitReference();

            this.GetGoodsReceiptID();
            this.GetGoodsReceiptDetailAvailables();

            this.GetGoodsReceiptBarcodeAvailables();
        }


        private void GetGoodsReceiptIndexes()
        {
            string queryString;

            queryString = " @NMVNTaskID int, @ModuleDetailID int, @AspUserID nvarchar(128), @FromDate DateTime, @ToDate DateTime " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";
            queryString = queryString + "    BEGIN " + "\r\n";

            queryString = queryString + "       SELECT      GoodsReceipts.GoodsReceiptID, CAST(GoodsReceipts.EntryDate AS DATE) AS EntryDate, GoodsReceipts.Reference, Locations.Code AS LocationCode, Workshifts.Name AS WorkshiftName, Users.FirstName AS UserFirstName, Users.LastName AS UserLastName, ISNULL(Customers.Code, GoodsReceipts.Purposes) AS CustomerCode, Customers.Name AS CustomerName, ISNULL(PlannedOrders.Reference, '') +  ISNULL(' (' + PlannedOrders.Code + ')', '') + ISNULL(WarehouseIssues.Code, '') + IIF(GoodsReceipts.GoodsReceiptTypeID = 7, N'Thu hồi màng đã cấp', '') AS GoodsReceiptTypeCaption, GoodsReceipts.Caption, GoodsReceipts.Description, " + "\r\n";
            queryString = queryString + "                   ISNULL(GoodsArrivals.Code, GoodsReceipts.Code) AS GoodsArrivalCode, GoodsArrivals.PackingList AS GoodsArrivalPackingList, GoodsArrivals.CustomsDeclaration AS GoodsArrivalCustomsDeclaration, GoodsArrivals.CustomsDeclarationDate AS GoodsArrivalCustomsDeclarationDate, ISNULL(GoodsArrivals.PurchaseOrderCodes, GoodsReceipts.PrimaryReferences) AS GoodsArrivalPurchaseOrderCodes, PurchaseOrders.VoucherDate AS GoodsArrivalPurchaseOrderVoucherDate, GoodsReceipts.TotalRows, GoodsReceipts.TotalQuantity, GoodsReceipts.Approved " + "\r\n";
            queryString = queryString + "       FROM        GoodsReceipts " + "\r\n";
            queryString = queryString + "                   INNER JOIN Locations ON (GoodsReceipts.GoodsReceiptTypeID = 6 OR GoodsReceipts.GoodsReceiptTypeID = 7 OR GoodsReceipts.GoodsReceiptTypeID = 8 OR GoodsReceipts.GoodsReceiptTypeID = 18 OR GoodsReceipts.GoodsReceiptTypeID = 19 " + (GlobalEnums.CBPP ? "" : "OR GoodsReceipts.GoodsReceiptTypeID = 9") + ") AND GoodsReceipts.NMVNTaskID = @NMVNTaskID AND GoodsReceipts.ModuleDetailID = @ModuleDetailID AND GoodsReceipts.EntryDate >= @FromDate AND GoodsReceipts.EntryDate <= @ToDate AND GoodsReceipts.OrganizationalUnitID IN (SELECT AccessControls.OrganizationalUnitID FROM AccessControls INNER JOIN AspNetUsers ON AccessControls.UserID = AspNetUsers.UserID WHERE AspNetUsers.Id = @AspUserID AND AccessControls.NMVNTaskID = @NMVNTaskID AND AccessControls.AccessLevel > 0) AND Locations.LocationID = GoodsReceipts.LocationID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Workshifts ON GoodsReceipts.WorkshiftID = Workshifts.WorkshiftID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Users ON GoodsReceipts.UserID = Users.UserID " + "\r\n";
            queryString = queryString + "                   LEFT JOIN Customers ON GoodsReceipts.CustomerID = Customers.CustomerID " + "\r\n";
            queryString = queryString + "                   LEFT JOIN PlannedOrders ON GoodsReceipts.PlannedOrderID = PlannedOrders.PlannedOrderID " + "\r\n";

            queryString = queryString + "                   LEFT JOIN Warehouses WarehouseIssues ON GoodsReceipts.WarehouseIssueID = WarehouseIssues.WarehouseID " + "\r\n";

            queryString = queryString + "                   LEFT JOIN GoodsArrivals ON GoodsReceipts.GoodsArrivalID = GoodsArrivals.GoodsArrivalID " + "\r\n";
            queryString = queryString + "                   LEFT JOIN PurchaseOrders ON GoodsArrivals.PurchaseOrderID = PurchaseOrders.PurchaseOrderID " + "\r\n";

            queryString = queryString + "    END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetGoodsReceiptIndexes", queryString);
        }


        private void GetGoodsReceiptViewDetails()
        {
            string queryString;

            queryString = " @GoodsReceiptID Int " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";
            queryString = queryString + "    BEGIN " + "\r\n";

            queryString = queryString + "       SELECT      GoodsReceiptDetails.GoodsReceiptDetailID, GoodsReceiptDetails.GoodsReceiptID, GoodsReceiptDetails.PurchaseRequisitionID, GoodsReceiptDetails.PurchaseRequisitionDetailID, PurchaseRequisitions.Reference AS PurchaseRequisitionReference, PurchaseRequisitions.Code AS PurchaseRequisitionCode, PurchaseRequisitions.EntryDate AS PurchaseRequisitionEntryDate, GoodsReceiptDetails.GoodsArrivalID, GoodsReceiptDetails.GoodsArrivalDetailID, GoodsReceiptDetails.GoodsArrivalPackageID, GoodsArrivals.Reference AS GoodsArrivalReference, GoodsArrivals.Code AS GoodsArrivalCode, GoodsArrivals.EntryDate AS GoodsArrivalEntryDate, GoodsArrivals.PurchaseOrderCodes, Customers.Code AS CustomerCode, " + "\r\n";
            queryString = queryString + "                   GoodsReceiptDetails.WarehouseTransferID, GoodsReceiptDetails.WarehouseTransferDetailID, WarehouseTransfers.Reference AS WarehouseTransferReference, WarehouseTransfers.EntryDate AS WarehouseTransferEntryDate, WarehouseTransferGoodsReceiptDetails.Reference AS GoodsReceiptReference, WarehouseTransferGoodsReceiptDetails.EntryDate AS GoodsReceiptEntryDate, GoodsReceiptDetails.BatchID, GoodsReceiptDetails.BatchEntryDate, GoodsReceiptDetails.WarehouseAdjustmentID, GoodsReceiptDetails.WarehouseAdjustmentDetailID, WarehouseAdjustmentDetails.Reference AS WarehouseAdjustmentReference, WarehouseAdjustmentDetails.EntryDate AS WarehouseAdjustmentEntryDate, WarehouseAdjustmentDetails.WarehouseAdjustmentTypeID, " + "\r\n";
            queryString = queryString + "                   Commodities.CommodityID, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, GoodsReceiptDetails.CommodityTypeID, GoodsReceiptDetails.BinLocationID, BinLocations.Code AS BinLocationCode, GoodsReceiptDetails.Barcode, GoodsReceiptDetails.BatchCode, GoodsReceiptDetails.SealCode, GoodsReceiptDetails.LabCode, GoodsReceiptDetails.LabID, GoodsReceiptDetails.ProductionDate, GoodsReceiptDetails.ExpiryDate, GoodsReceiptDetails.UnitWeight, GoodsReceiptDetails.TareWeight, " + "\r\n";
            queryString = queryString + "                   ROUND(ISNULL(IIF(PurchaseRequisitionDetails.Approved = 1 AND PurchaseRequisitionDetails.InActive = 0 AND PurchaseRequisitionDetails.InActivePartial = 0, PurchaseRequisitionDetails.Quantity - PurchaseRequisitionDetails.QuantityReceipted, 0), 0) + ISNULL(IIF(GoodsArrivalPackages.Approved = 1 AND GoodsArrivalPackages.InActive = 0 AND GoodsArrivalPackages.InActivePartial = 0, GoodsArrivalPackages.Quantity - GoodsArrivalPackages.QuantityReceipted, 0), 0) + ISNULL(WarehouseTransferDetails.Quantity - WarehouseTransferDetails.QuantityReceipted, 0) + ISNULL(MaterialIssueDetails.Quantity - MaterialIssueDetails.QuantitySemifinished - MaterialIssueDetails.QuantityFailure - MaterialIssueDetails.QuantityReceipted, 0) + ISNULL(WarehouseAdjustmentDetails.Quantity - WarehouseAdjustmentDetails.QuantityReceipted, 0) + ISNULL(FinishedProductPackages.Quantity - FinishedProductPackages.QuantityReceipted, 0) + ISNULL(FinishedItemPackages.Quantity - FinishedItemPackages.QuantityReceipted, 0) + ISNULL(RecyclatePackages.Quantity - RecyclatePackages.QuantityReceipted, 0) + GoodsReceiptDetails.Quantity, " + (int)GlobalEnums.rndQuantity + ") AS QuantityRemains, GoodsReceiptDetails.Quantity, GoodsReceiptDetails.Remarks, " + "\r\n";
            queryString = queryString + "                   GoodsReceiptDetails.MaterialIssueDetailID, MaterialIssueDetails.MaterialIssueID, MaterialIssueDetails.EntryDate AS MaterialIssueEntryDate, Workshifts.Name AS WorkshiftName, Workshifts.EntryDate AS WorkshiftEntryDate, ProductionLines.Code AS ProductionLinesCode, " + "\r\n";

            queryString = queryString + "                   GoodsReceiptDetails.FinishedProductPackageID, FinishedProductPackages.FinishedProductID, FinishedProductPackages.EntryDate AS FinishedProductEntryDate, FinishedProductPackages.SemifinishedProductReferences, " + "\r\n";
            queryString = queryString + "                   GoodsReceiptDetails.FinishedItemPackageID, FinishedItemPackages.FinishedItemID, FinishedItemPackages.EntryDate AS FinishedItemEntryDate, FinishedItemPackages.SemifinishedItemReferences, " + "\r\n";
            queryString = queryString + "                   GoodsReceiptDetails.RecyclatePackageID, RecyclatePackages.RecyclateID, RecyclatePackages.EntryDate AS RecyclateEntryDate, " + "\r\n"; //RecyclatePackages.SemiRecyclateReferences, 
            queryString = queryString + "                   ISNULL(FirmOrders.Reference, ISNULL(FinishedItemFirmOrders.Reference, MaterialIssueFirmOrders.Reference)) AS FirmOrderReference, ISNULL(FirmOrders.Code, ISNULL(FinishedItemFirmOrders.Code, MaterialIssueFirmOrders.Code)) AS FirmOrderCode, ISNULL(FirmOrders.Specs, ISNULL(FinishedItemFirmOrders.Specs, MaterialIssueFirmOrders.Specs)) AS FirmOrderSpecs  " + "\r\n";

            queryString = queryString + "       FROM        GoodsReceiptDetails " + "\r\n";
            queryString = queryString + "                   INNER JOIN Commodities ON GoodsReceiptDetails.GoodsReceiptID = @GoodsReceiptID AND GoodsReceiptDetails.CommodityID = Commodities.CommodityID " + "\r\n";
            queryString = queryString + "                   INNER JOIN BinLocations ON GoodsReceiptDetails.BinLocationID = BinLocations.BinLocationID " + "\r\n";

            queryString = queryString + "                   LEFT JOIN MaterialIssueDetails ON GoodsReceiptDetails.MaterialIssueDetailID = MaterialIssueDetails.MaterialIssueDetailID " + "\r\n";
            queryString = queryString + "                   LEFT JOIN FirmOrders MaterialIssueFirmOrders ON MaterialIssueDetails.FirmOrderID = MaterialIssueFirmOrders.FirmOrderID " + "\r\n";
            queryString = queryString + "                   LEFT JOIN Workshifts ON MaterialIssueDetails.WorkshiftID = Workshifts.WorkshiftID " + "\r\n";
            queryString = queryString + "                   LEFT JOIN ProductionLines ON MaterialIssueDetails.ProductionLineID = ProductionLines.ProductionLineID " + "\r\n";

            queryString = queryString + "                   LEFT JOIN GoodsArrivalPackages ON GoodsReceiptDetails.GoodsArrivalPackageID = GoodsArrivalPackages.GoodsArrivalPackageID " + "\r\n";
            queryString = queryString + "                   LEFT JOIN GoodsArrivals ON GoodsArrivalPackages.GoodsArrivalID = GoodsArrivals.GoodsArrivalID " + "\r\n";
            queryString = queryString + "                   LEFT JOIN Customers ON GoodsArrivals.CustomerID = Customers.CustomerID " + "\r\n";

            queryString = queryString + "                   LEFT JOIN PurchaseRequisitionDetails ON GoodsReceiptDetails.PurchaseRequisitionDetailID = PurchaseRequisitionDetails.PurchaseRequisitionDetailID " + "\r\n";
            queryString = queryString + "                   LEFT JOIN PurchaseRequisitions ON PurchaseRequisitionDetails.PurchaseRequisitionID = PurchaseRequisitions.PurchaseRequisitionID " + "\r\n";

            queryString = queryString + "                   LEFT JOIN WarehouseTransferDetails ON GoodsReceiptDetails.WarehouseTransferDetailID = WarehouseTransferDetails.WarehouseTransferDetailID " + "\r\n";
            queryString = queryString + "                   LEFT JOIN WarehouseTransfers ON WarehouseTransferDetails.WarehouseTransferID = WarehouseTransfers.WarehouseTransferID " + "\r\n";
            queryString = queryString + "                   LEFT JOIN GoodsReceiptDetails WarehouseTransferGoodsReceiptDetails ON WarehouseTransferDetails.GoodsReceiptDetailID = WarehouseTransferGoodsReceiptDetails.GoodsReceiptDetailID " + "\r\n";

            queryString = queryString + "                   LEFT JOIN WarehouseAdjustmentDetails ON GoodsReceiptDetails.WarehouseAdjustmentDetailID = WarehouseAdjustmentDetails.WarehouseAdjustmentDetailID " + "\r\n";

            queryString = queryString + "                   LEFT JOIN FinishedProductPackages ON GoodsReceiptDetails.FinishedProductPackageID = FinishedProductPackages.FinishedProductPackageID " + "\r\n";
            queryString = queryString + "                   LEFT JOIN FirmOrders ON FinishedProductPackages.FirmOrderID = FirmOrders.FirmOrderID " + "\r\n";
            queryString = queryString + "                   LEFT JOIN RecyclatePackages ON GoodsReceiptDetails.RecyclatePackageID = RecyclatePackages.RecyclatePackageID " + "\r\n";

            queryString = queryString + "                   LEFT JOIN FinishedItemPackages ON GoodsReceiptDetails.FinishedItemPackageID = FinishedItemPackages.FinishedItemPackageID " + "\r\n";
            queryString = queryString + "                   LEFT JOIN FirmOrders FinishedItemFirmOrders ON FinishedItemPackages.FirmOrderID = FinishedItemFirmOrders.FirmOrderID " + "\r\n";

            queryString = queryString + "       ORDER BY    Commodities.CommodityTypeID, GoodsReceiptDetails.GoodsReceiptDetailID " + "\r\n";

            queryString = queryString + "    END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetGoodsReceiptViewDetails", queryString);
        }




        #region GoodsArrival

        private void GetGoodsReceiptPendingPurchasings()
        {
            string queryString = " @LocationID int, @NMVNTaskID int " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "       DECLARE         @GoodsArrivalCount int              DECLARE @PurchaseOrderCodes varchar(3999)           DECLARE @CustomerNames varchar(555) " + "\r\n";
            queryString = queryString + "       DECLARE         @PendingPurchasings TABLE (PurchaseOrderCode nvarchar(60), CustomerName nvarchar(150) NOT NULL, WarehouseID int NOT NULL, WarehouseCode nvarchar(10) NOT NULL, WarehouseName nvarchar(60) NOT NULL, PackageCount int NULL, TotalQuantityRemains decimal(18, 2) NULL)" + "\r\n";

            queryString = queryString + "       INSERT INTO     @PendingPurchasings (PurchaseOrderCode, CustomerName, WarehouseID, WarehouseCode, WarehouseName, PackageCount, TotalQuantityRemains) " + "\r\n";
            queryString = queryString + "       SELECT          ISNULL(GoodsArrivals.PurchaseOrderCodes, GoodsArrivals.Code) AS PurchaseOrderCode, Customers.Name AS CustomerName, Warehouses.WarehouseID, Warehouses.Code AS WarehouseCode, Warehouses.Name AS WarehouseName, GoodsArrivalPackageSummaries.PackageCount, GoodsArrivalPackageSummaries.TotalQuantityRemains " + "\r\n";
            queryString = queryString + "       FROM            (SELECT GoodsArrivalID, COUNT(GoodsArrivalPackageID) AS PackageCount, SUM(ROUND(Quantity - QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ")) AS TotalQuantityRemains FROM GoodsArrivalPackages WHERE LocationID = @LocationID AND NMVNTaskID = @NMVNTaskID + 70129005 AND Approved = 1 AND InActive = 0 AND InActivePartial = 0 AND ROUND(Quantity - QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") > 0 GROUP BY GoodsArrivalID) GoodsArrivalPackageSummaries " + "\r\n";
            queryString = queryString + "                       INNER JOIN GoodsArrivals ON GoodsArrivalPackageSummaries.GoodsArrivalID = GoodsArrivals.GoodsArrivalID " + "\r\n";
            queryString = queryString + "                       INNER JOIN Customers ON GoodsArrivals.CustomerID = Customers.CustomerID " + "\r\n";
            queryString = queryString + "                       INNER JOIN Warehouses ON GoodsArrivals.WarehouseID = Warehouses.WarehouseID " + "\r\n";

            queryString = queryString + "       SET @GoodsArrivalCount = @@ROWCOUNT " + "\r\n";

            queryString = queryString + "       IF (@GoodsArrivalCount > 0) " + "\r\n";
            queryString = queryString + "       BEGIN " + "\r\n";
            queryString = queryString + "           SELECT      @PurchaseOrderCodes = LEFT(STUFF((SELECT ', ' + PurchaseOrderCode FROM @PendingPurchasings FOR XML PATH('')) ,1,1,''), 100) " + "\r\n";
            queryString = queryString + "           SELECT      @CustomerNames = LEFT(STUFF((SELECT ', ' + CustomerName FROM (SELECT DISTINCT CustomerName FROM @PendingPurchasings) DistinctCustomerNames FOR XML PATH('')) ,1,1,''), 100) " + "\r\n";
            queryString = queryString + "       END " + "\r\n";

            queryString = queryString + "       SELECT          " + (int)@GlobalEnums.GoodsReceiptTypeID.GoodsArrival + " AS GoodsReceiptTypeID, @PurchaseOrderCodes AS PurchaseOrderCodes, @CustomerNames AS CustomerNames, WarehouseID, WarehouseCode, WarehouseName, SUM(PackageCount) AS PackageCount, SUM(TotalQuantityRemains) AS TotalQuantityRemains, '' AS Description FROM @PendingPurchasings GROUP BY WarehouseID, WarehouseCode, WarehouseName " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetGoodsReceiptPendingPurchasings", queryString);
        }

        private void GetGoodsReceiptPendingGoodsArrivals()
        {
            string queryString = " @LocationID int, @NMVNTaskID int " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "       DECLARE         @GoodsArrivalIDs TABLE (GoodsArrivalID int NOT NULL) " + "\r\n";
            queryString = queryString + "       INSERT INTO     @GoodsArrivalIDs (GoodsArrivalID) SELECT GoodsArrivalID FROM GoodsArrivalPackages WHERE LocationID = @LocationID AND NMVNTaskID = @NMVNTaskID + 70129005 AND Approved = 1 AND InActive = 0 AND InActivePartial = 0 AND ROUND(Quantity - QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") > 0 " + "\r\n";

            queryString = queryString + "       SELECT          " + (int)@GlobalEnums.GoodsReceiptTypeID.GoodsArrival + " AS GoodsReceiptTypeID, GoodsArrivals.GoodsArrivalID, GoodsArrivals.Reference AS GoodsArrivalReference, GoodsArrivals.Code AS GoodsArrivalCode, GoodsArrivals.EntryDate AS GoodsArrivalEntryDate, GoodsArrivals.Description, GoodsArrivals.Remarks, " + "\r\n";
            queryString = queryString + "                       GoodsArrivals.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, Customers.OfficialName AS CustomerOfficialName, Warehouses.WarehouseID, Warehouses.Code AS WarehouseCode, Warehouses.Name AS WarehouseName, GoodsArrivals.CustomsDeclaration AS GoodsArrivalCustomsDeclaration, GoodsArrivals.CustomsDeclarationDate AS GoodsArrivalCustomsDeclarationDate, GoodsArrivals.PurchaseOrderCodes AS GoodsArrivalPurchaseOrderCodes, PurchaseOrders.VoucherDate AS GoodsArrivalPurchaseOrderVoucherDate " + "\r\n";

            queryString = queryString + "       FROM            GoodsArrivals " + "\r\n";
            queryString = queryString + "                       INNER JOIN Customers ON GoodsArrivals.GoodsArrivalID IN (SELECT GoodsArrivalID FROM @GoodsArrivalIDs) AND GoodsArrivals.CustomerID = Customers.CustomerID " + "\r\n";
            queryString = queryString + "                       INNER JOIN EntireTerritories CustomerEntireTerritories ON Customers.TerritoryID = CustomerEntireTerritories.TerritoryID " + "\r\n";
            queryString = queryString + "                       INNER JOIN Warehouses ON GoodsArrivals.WarehouseID = Warehouses.WarehouseID " + "\r\n";
            queryString = queryString + "                       LEFT JOIN PurchaseOrders ON GoodsArrivals.PurchaseOrderID = PurchaseOrders.PurchaseOrderID " + "\r\n";

            queryString = queryString + "       ORDER BY        GoodsArrivals.GoodsArrivalID " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetGoodsReceiptPendingGoodsArrivals", queryString);
        }

        private void GetGoodsReceiptPendingGoodsArrivalPackages()
        {
            string queryString;

            queryString = " @WebAPI bit, @LocationID Int, @NMVNTaskID int, @GoodsReceiptID Int, @GoodsArrivalID Int, @Barcode nvarchar(60), @GoodsArrivalPackageIDs varchar(3999) " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "   BEGIN " + "\r\n";
            queryString = queryString + "       IF  (NOT @Barcode IS NULL AND @Barcode <> '' AND @Barcode <> '0') " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLGoodsArrival(true) + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLGoodsArrival(false) + "\r\n";
            queryString = queryString + "   END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetGoodsReceiptPendingGoodsArrivalPackages", queryString);
        }

        private string BuildSQLGoodsArrival(bool isBarcode)
        {
            string queryString = "";
            queryString = queryString + "   BEGIN " + "\r\n";

            queryString = queryString + "       IF  (@GoodsArrivalPackageIDs <> '' AND @GoodsArrivalPackageIDs <> '0') " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLGoodsArrival(isBarcode, true) + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLGoodsArrival(isBarcode, false) + "\r\n";

            queryString = queryString + "   END " + "\r\n";

            return queryString;
        }

        private string BuildSQLGoodsArrival(bool isBarcode, bool isGoodsArrivalPackageIDs)
        {
            string queryString = "";
            queryString = queryString + "   BEGIN " + "\r\n";

            queryString = queryString + "       IF  (NOT @GoodsArrivalID IS NULL AND @GoodsArrivalID <> 0) " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLGoodsArrival(isBarcode, isGoodsArrivalPackageIDs, true) + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLGoodsArrival(isBarcode, isGoodsArrivalPackageIDs, false) + "\r\n";

            queryString = queryString + "   END " + "\r\n";

            return queryString;
        }

        private string BuildSQLGoodsArrival(bool isBarcode, bool isGoodsArrivalPackageIDs, bool isGoodsArrivalID)
        {
            string queryString = "";
            queryString = queryString + "   BEGIN " + "\r\n";

            queryString = queryString + "       IF (@GoodsReceiptID <= 0 OR @WebAPI = 1) " + "\r\n";
            queryString = queryString + "               BEGIN " + "\r\n"; //@WebAPI = 1: MEANS: EVERY TIME, WHEN MOIBILE APP REMOVE A DETAIL ROW: IT [MUST SAVE], IN ORDER TO CALL THIS STORED PROCEDURE TO GET THE PENDING. BY THIS RULE: THERE IS NO NEED FOR THE BuildSQLGoodsArrivalEdit
            queryString = queryString + "                   " + this.BuildSQLGoodsArrivalNew(isBarcode, isGoodsArrivalPackageIDs, isGoodsArrivalID) + "\r\n";
            queryString = queryString + "                   ORDER BY GoodsArrivals.EntryDate, GoodsArrivals.GoodsArrivalID, GoodsArrivalPackages.SerialID, GoodsArrivalPackages.GoodsArrivalPackageID DESC " + "\r\n";
            queryString = queryString + "               END " + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "               BEGIN " + "\r\n";
            queryString = queryString + "                   " + this.BuildSQLGoodsArrivalNew(isBarcode, isGoodsArrivalPackageIDs, isGoodsArrivalID) + " WHERE GoodsArrivalPackages.GoodsArrivalPackageID NOT IN (SELECT GoodsArrivalPackageID FROM GoodsReceiptDetails WHERE GoodsReceiptID = @GoodsReceiptID) " + "\r\n";
            queryString = queryString + "                   UNION ALL " + "\r\n";
            queryString = queryString + "                   " + this.BuildSQLGoodsArrivalEdit(isBarcode, isGoodsArrivalPackageIDs, isGoodsArrivalID) + "\r\n";
            queryString = queryString + "                   ORDER BY GoodsArrivals.EntryDate, GoodsArrivals.GoodsArrivalID, GoodsArrivalPackages.SerialID, GoodsArrivalPackages.GoodsArrivalPackageID DESC " + "\r\n";
            queryString = queryString + "               END " + "\r\n";

            queryString = queryString + "   END " + "\r\n";

            return queryString;
        }

        private string BuildSQLGoodsArrivalNew(bool isBarcode, bool isGoodsArrivalPackageIDs, bool isGoodsArrivalID)
        {
            string queryString = "";

            queryString = queryString + "       SELECT      TOP 200 GoodsArrivals.GoodsArrivalID, GoodsArrivalPackages.GoodsArrivalDetailID, GoodsArrivalPackages.GoodsArrivalPackageID, GoodsArrivals.Reference AS GoodsArrivalReference, GoodsArrivals.Code AS GoodsArrivalCode, GoodsArrivals.EntryDate AS GoodsArrivalEntryDate, GoodsArrivals.PurchaseOrderCodes, Customers.Code AS CustomerCode, GoodsArrivalPackages.BatchID, GoodsArrivalPackages.BatchEntryDate, " + "\r\n";
            queryString = queryString + "                   Commodities.CommodityID, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, Commodities.CommodityTypeID, GoodsArrivalPackages.LabID, GoodsArrivalPackages.ProductionDate, GoodsArrivalPackages.ExpiryDate, GoodsArrivalPackages.SerialID, GoodsArrivalPackages.Barcode, GoodsArrivalPackages.BatchCode, GoodsArrivalPackages.SealCode, GoodsArrivalPackages.LabCode, GoodsArrivalPackages.UnitWeight, GoodsArrivalPackages.TareWeight, " + "\r\n";
            queryString = queryString + "                   ROUND(GoodsArrivalPackages.Quantity - GoodsArrivalPackages.QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") AS QuantityRemains, " + "\r\n";
            queryString = queryString + "                   0.0 AS Quantity, GoodsArrivals.Description, GoodsArrivalPackages.Remarks, CAST(1 AS bit) AS IsSelected " + "\r\n";

            queryString = queryString + "       FROM        GoodsArrivals " + "\r\n";
            queryString = queryString + "                   INNER JOIN GoodsArrivalPackages ON " + (isGoodsArrivalID ? "GoodsArrivals.GoodsArrivalID = @GoodsArrivalID AND " : "") + " GoodsArrivals.NMVNTaskID = @NMVNTaskID + 70129005 AND GoodsArrivalPackages.Approved = 1 AND GoodsArrivalPackages.InActive = 0 AND GoodsArrivalPackages.InActivePartial = 0 AND ROUND(GoodsArrivalPackages.Quantity - GoodsArrivalPackages.QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") > 0 AND GoodsArrivals.GoodsArrivalID = GoodsArrivalPackages.GoodsArrivalID" + (isBarcode ? " AND GoodsArrivalPackages.Barcode = @Barcode" : "") + (isGoodsArrivalPackageIDs ? " AND GoodsArrivalPackages.GoodsArrivalPackageID NOT IN (SELECT Id FROM dbo.SplitToIntList (@GoodsArrivalPackageIDs))" : "") + "\r\n";
            queryString = queryString + "                   INNER JOIN Commodities ON GoodsArrivalPackages.CommodityID = Commodities.CommodityID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Customers ON GoodsArrivals.CustomerID = Customers.CustomerID " + "\r\n";

            return queryString;
        }

        private string BuildSQLGoodsArrivalEdit(bool isBarcode, bool isGoodsArrivalPackageIDs, bool isGoodsArrivalID)
        {
            string queryString = "";

            queryString = queryString + "       SELECT      GoodsArrivals.GoodsArrivalID, GoodsArrivalPackages.GoodsArrivalDetailID, GoodsArrivalPackages.GoodsArrivalPackageID, GoodsArrivals.Reference AS GoodsArrivalReference, GoodsArrivals.Code AS GoodsArrivalCode, GoodsArrivals.EntryDate AS GoodsArrivalEntryDate, GoodsArrivals.PurchaseOrderCodes, Customers.Code AS CustomerCode, GoodsArrivalPackages.BatchID, GoodsArrivalPackages.BatchEntryDate, " + "\r\n";
            queryString = queryString + "                   Commodities.CommodityID, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, Commodities.CommodityTypeID, GoodsArrivalPackages.LabID, GoodsArrivalPackages.ProductionDate, GoodsArrivalPackages.ExpiryDate, GoodsArrivalPackages.SerialID, GoodsArrivalPackages.Barcode, GoodsArrivalPackages.BatchCode, GoodsArrivalPackages.SealCode, GoodsArrivalPackages.LabCode, GoodsArrivalPackages.UnitWeight, GoodsArrivalPackages.TareWeight, " + "\r\n";
            queryString = queryString + "                   ROUND(GoodsArrivalPackages.Quantity - GoodsArrivalPackages.QuantityReceipted + GoodsReceiptDetails.Quantity, " + (int)GlobalEnums.rndQuantity + ") AS QuantityRemains, " + "\r\n";
            queryString = queryString + "                   0.0 AS Quantity, GoodsArrivals.Description, GoodsArrivalPackages.Remarks, CAST(1 AS bit) AS IsSelected " + "\r\n";

            queryString = queryString + "       FROM        GoodsArrivalPackages " + "\r\n";
            queryString = queryString + "                   INNER JOIN GoodsReceiptDetails ON GoodsReceiptDetails.GoodsReceiptID = @GoodsReceiptID AND GoodsArrivalPackages.GoodsArrivalPackageID = GoodsReceiptDetails.GoodsArrivalPackageID" + (isBarcode ? " AND GoodsArrivalPackages.Barcode = @Barcode" : "") + (isGoodsArrivalPackageIDs ? " AND GoodsArrivalPackages.GoodsArrivalPackageID NOT IN (SELECT Id FROM dbo.SplitToIntList (@GoodsArrivalPackageIDs))" : "") + "\r\n";
            queryString = queryString + "                   INNER JOIN Commodities ON GoodsArrivalPackages.CommodityID = Commodities.CommodityID " + "\r\n";
            queryString = queryString + "                   INNER JOIN GoodsArrivals ON GoodsArrivalPackages.GoodsArrivalID = GoodsArrivals.GoodsArrivalID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Customers ON GoodsArrivals.CustomerID = Customers.CustomerID " + "\r\n";

            return queryString;
        }
        #endregion GoodsArrival



        #region PurchaseRequisition

        private void GetGoodsReceiptPendingCustomers()
        {
            string queryString = " @LocationID int " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "       SELECT          " + (int)@GlobalEnums.GoodsReceiptTypeID.PurchaseRequisition + " AS GoodsReceiptTypeID, Customers.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, Customers.OfficialName AS CustomerOfficialName, Customers.VATCode AS CustomerVATCode, Customers.AttentionName AS CustomerAttentionName, Customers.TerritoryID AS CustomerTerritoryID, CustomerEntireTerritories.EntireName AS CustomerEntireTerritoryEntireName, Warehouses.WarehouseID, Warehouses.Code AS WarehouseCode, Warehouses.Name AS WarehouseName " + "\r\n";

            queryString = queryString + "       FROM           (SELECT DISTINCT CustomerID FROM PurchaseRequisitions WHERE PurchaseRequisitionID IN (SELECT PurchaseRequisitionID FROM PurchaseRequisitionDetails WHERE LocationID = @LocationID AND Approved = 1 AND InActive = 0 AND InActivePartial = 0 AND ROUND(Quantity - QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") > 0)) CustomerPENDING " + "\r\n";
            queryString = queryString + "                       INNER JOIN Customers ON CustomerPENDING.CustomerID = Customers.CustomerID " + "\r\n";
            queryString = queryString + "                       INNER JOIN EntireTerritories CustomerEntireTerritories ON Customers.TerritoryID = CustomerEntireTerritories.TerritoryID " + "\r\n";
            queryString = queryString + "                       INNER JOIN CustomerCategories ON Customers.CustomerCategoryID = CustomerCategories.CustomerCategoryID " + "\r\n";


            queryString = queryString + "                       INNER JOIN Warehouses ON Warehouses.WarehouseID = 2 " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetGoodsReceiptPendingCustomers", queryString);
        }

        private void GetGoodsReceiptPendingPurchaseRequisitions()
        {
            string queryString = " @LocationID int " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "       SELECT          " + (int)@GlobalEnums.GoodsReceiptTypeID.PurchaseRequisition + " AS GoodsReceiptTypeID, PurchaseRequisitions.PurchaseRequisitionID, PurchaseRequisitions.Reference AS PurchaseRequisitionReference, PurchaseRequisitions.Code AS PurchaseRequisitionCode, PurchaseRequisitions.EntryDate AS PurchaseRequisitionEntryDate, PurchaseRequisitions.Description, PurchaseRequisitions.Remarks, " + "\r\n";
            queryString = queryString + "                       PurchaseRequisitions.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, Customers.OfficialName AS CustomerOfficialName, Warehouses.WarehouseID, Warehouses.Code AS WarehouseCode, Warehouses.Name AS WarehouseName " + "\r\n";

            queryString = queryString + "       FROM            PurchaseRequisitions " + "\r\n";
            queryString = queryString + "                       INNER JOIN Customers ON PurchaseRequisitions.PurchaseRequisitionID IN (SELECT PurchaseRequisitionID FROM PurchaseRequisitionDetails WHERE LocationID = @LocationID AND Approved = 1 AND InActive = 0 AND InActivePartial = 0 AND ROUND(Quantity - QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") > 0) AND PurchaseRequisitions.CustomerID = Customers.CustomerID " + "\r\n";
            queryString = queryString + "                       INNER JOIN EntireTerritories CustomerEntireTerritories ON Customers.TerritoryID = CustomerEntireTerritories.TerritoryID " + "\r\n";


            queryString = queryString + "                       INNER JOIN Warehouses ON Warehouses.WarehouseID = 6 " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetGoodsReceiptPendingPurchaseRequisitions", queryString);
        }

        private void GetGoodsReceiptPendingPurchaseRequisitionDetails()
        {
            string queryString;

            queryString = " @LocationID Int, @GoodsReceiptID Int, @PurchaseRequisitionID Int, @CustomerID Int, @PurchaseRequisitionDetailIDs varchar(3999), @IsReadonly bit " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "   BEGIN " + "\r\n";

            queryString = queryString + "       IF  (@PurchaseRequisitionID <> 0) " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLPurchaseRequisition(true) + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLPurchaseRequisition(false) + "\r\n";

            queryString = queryString + "   END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetGoodsReceiptPendingPurchaseRequisitionDetails", queryString);
        }

        private string BuildSQLPurchaseRequisition(bool isPurchaseRequisitionID)
        {
            string queryString = "";
            queryString = queryString + "   BEGIN " + "\r\n";
            queryString = queryString + "       IF  (@PurchaseRequisitionDetailIDs <> '') " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLPurchaseRequisition(isPurchaseRequisitionID, true) + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLPurchaseRequisition(isPurchaseRequisitionID, false) + "\r\n";
            queryString = queryString + "   END " + "\r\n";

            return queryString;
        }

        private string BuildSQLPurchaseRequisition(bool isPurchaseRequisitionID, bool isPurchaseRequisitionDetailIDs)
        {
            string queryString = "";
            queryString = queryString + "   BEGIN " + "\r\n";

            queryString = queryString + "       IF (@GoodsReceiptID <= 0) " + "\r\n";
            queryString = queryString + "               BEGIN " + "\r\n";
            queryString = queryString + "                   " + this.BuildSQLPurchaseRequisitionNew(isPurchaseRequisitionID, isPurchaseRequisitionDetailIDs) + "\r\n";
            queryString = queryString + "                   ORDER BY PurchaseRequisitions.EntryDate, PurchaseRequisitions.PurchaseRequisitionID, PurchaseRequisitionDetails.PurchaseRequisitionDetailID " + "\r\n";
            queryString = queryString + "               END " + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";

            queryString = queryString + "               IF (@IsReadonly = 1) " + "\r\n";
            queryString = queryString + "                   BEGIN " + "\r\n";
            queryString = queryString + "                       " + this.BuildSQLPurchaseRequisitionEdit(isPurchaseRequisitionID, isPurchaseRequisitionDetailIDs) + "\r\n";
            queryString = queryString + "                       ORDER BY PurchaseRequisitions.EntryDate, PurchaseRequisitions.PurchaseRequisitionID, PurchaseRequisitionDetails.PurchaseRequisitionDetailID " + "\r\n";
            queryString = queryString + "                   END " + "\r\n";

            queryString = queryString + "               ELSE " + "\r\n"; //FULL SELECT FOR EDIT MODE

            queryString = queryString + "                   BEGIN " + "\r\n";
            queryString = queryString + "                       " + this.BuildSQLPurchaseRequisitionNew(isPurchaseRequisitionID, isPurchaseRequisitionDetailIDs) + " WHERE PurchaseRequisitionDetails.PurchaseRequisitionDetailID NOT IN (SELECT PurchaseRequisitionDetailID FROM GoodsReceiptDetails WHERE GoodsReceiptID = @GoodsReceiptID) " + "\r\n";
            queryString = queryString + "                       UNION ALL " + "\r\n";
            queryString = queryString + "                       " + this.BuildSQLPurchaseRequisitionEdit(isPurchaseRequisitionID, isPurchaseRequisitionDetailIDs) + "\r\n";
            queryString = queryString + "                       ORDER BY PurchaseRequisitions.EntryDate, PurchaseRequisitions.PurchaseRequisitionID, PurchaseRequisitionDetails.PurchaseRequisitionDetailID " + "\r\n";
            queryString = queryString + "                   END " + "\r\n";

            queryString = queryString + "   END " + "\r\n";

            return queryString;
        }

        private string BuildSQLPurchaseRequisitionNew(bool isPurchaseRequisitionID, bool isPurchaseRequisitionDetailIDs)
        {
            string queryString = "";

            queryString = queryString + "       SELECT      PurchaseRequisitions.PurchaseRequisitionID, PurchaseRequisitionDetails.PurchaseRequisitionDetailID, PurchaseRequisitions.Reference AS PurchaseRequisitionReference, PurchaseRequisitions.Code AS PurchaseRequisitionCode, PurchaseRequisitions.EntryDate AS PurchaseRequisitionEntryDate, 0 AS BatchID, GETDATE() AS BatchEntryDate, " + "\r\n";
            queryString = queryString + "                   Commodities.CommodityID, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, Commodities.CommodityTypeID, " + "\r\n";
            queryString = queryString + "                   ROUND(PurchaseRequisitionDetails.Quantity - PurchaseRequisitionDetails.QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") AS QuantityRemains, " + "\r\n";
            queryString = queryString + "                   0.0 AS Quantity, PurchaseRequisitions.Description, PurchaseRequisitionDetails.Remarks, CAST(1 AS bit) AS IsSelected " + "\r\n";

            queryString = queryString + "       FROM        PurchaseRequisitions " + "\r\n";
            queryString = queryString + "                   INNER JOIN PurchaseRequisitionDetails ON " + (isPurchaseRequisitionID ? " PurchaseRequisitions.PurchaseRequisitionID = @PurchaseRequisitionID " : "PurchaseRequisitions.LocationID = @LocationID AND PurchaseRequisitions.CustomerID = @CustomerID ") + " AND PurchaseRequisitionDetails.Approved = 1 AND PurchaseRequisitionDetails.InActive = 0 AND PurchaseRequisitionDetails.InActivePartial = 0 AND ROUND(PurchaseRequisitionDetails.Quantity - PurchaseRequisitionDetails.QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") > 0 AND PurchaseRequisitions.PurchaseRequisitionID = PurchaseRequisitionDetails.PurchaseRequisitionID" + (isPurchaseRequisitionDetailIDs ? " AND PurchaseRequisitionDetails.PurchaseRequisitionDetailID NOT IN (SELECT Id FROM dbo.SplitToIntList (@PurchaseRequisitionDetailIDs))" : "") + "\r\n";
            queryString = queryString + "                   INNER JOIN Commodities ON PurchaseRequisitionDetails.CommodityID = Commodities.CommodityID " + "\r\n";

            return queryString;
        }

        private string BuildSQLPurchaseRequisitionEdit(bool isPurchaseRequisitionID, bool isPurchaseRequisitionDetailIDs)
        {
            string queryString = "";

            queryString = queryString + "       SELECT      PurchaseRequisitions.PurchaseRequisitionID, PurchaseRequisitionDetails.PurchaseRequisitionDetailID, PurchaseRequisitions.Reference AS PurchaseRequisitionReference, PurchaseRequisitions.Code AS PurchaseRequisitionCode, PurchaseRequisitions.EntryDate AS PurchaseRequisitionEntryDate, 0 AS BatchID, GETDATE() AS BatchEntryDate, " + "\r\n";
            queryString = queryString + "                   Commodities.CommodityID, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, Commodities.CommodityTypeID, " + "\r\n";
            queryString = queryString + "                   ROUND(PurchaseRequisitionDetails.Quantity - PurchaseRequisitionDetails.QuantityReceipted + GoodsReceiptDetails.Quantity, " + (int)GlobalEnums.rndQuantity + ") AS QuantityRemains, " + "\r\n";
            queryString = queryString + "                   0.0 AS Quantity, PurchaseRequisitions.Description, PurchaseRequisitionDetails.Remarks, CAST(1 AS bit) AS IsSelected " + "\r\n";

            queryString = queryString + "       FROM        PurchaseRequisitionDetails " + "\r\n";
            queryString = queryString + "                   INNER JOIN GoodsReceiptDetails ON GoodsReceiptDetails.GoodsReceiptID = @GoodsReceiptID AND PurchaseRequisitionDetails.PurchaseRequisitionDetailID = GoodsReceiptDetails.PurchaseRequisitionDetailID" + (isPurchaseRequisitionDetailIDs ? " AND PurchaseRequisitionDetails.PurchaseRequisitionDetailID NOT IN (SELECT Id FROM dbo.SplitToIntList (@PurchaseRequisitionDetailIDs))" : "") + "\r\n";
            queryString = queryString + "                   INNER JOIN Commodities ON PurchaseRequisitionDetails.CommodityID = Commodities.CommodityID " + "\r\n";
            queryString = queryString + "                   INNER JOIN PurchaseRequisitions ON PurchaseRequisitionDetails.PurchaseRequisitionID = PurchaseRequisitions.PurchaseRequisitionID " + "\r\n";

            return queryString;
        }
        #endregion PurchaseRequisition





        #region WarehouseTransfer
        private void GetGoodsReceiptPendingWarehouseTransfers()
        {
            string queryString = " @LocationID int, @NMVNTaskID int " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "       DECLARE         @WarehouseTransferIDs TABLE (WarehouseTransferID int NOT NULL) " + "\r\n";
            queryString = queryString + "       INSERT INTO     @WarehouseTransferIDs (WarehouseTransferID) SELECT WarehouseTransferID FROM WarehouseTransferDetails WHERE LocationReceiptID = @LocationID AND NMVNTaskID = @NMVNTaskID - 1000000 AND Approved = 1 AND ROUND(Quantity - QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") > 0 " + "\r\n";

            queryString = queryString + "       SELECT          " + (int)@GlobalEnums.GoodsReceiptTypeID.WarehouseTransfer + " AS GoodsReceiptTypeID, WarehouseTransfers.WarehouseTransferID, WarehouseTransfers.Reference AS WarehouseTransferReference, WarehouseTransfers.EntryDate AS WarehouseTransferEntryDate, WarehouseTransfers.Caption, WarehouseTransfers.Description, WarehouseTransfers.Remarks, " + "\r\n";
            queryString = queryString + "                       Warehouses.WarehouseID, Warehouses.Code AS WarehouseCode, Warehouses.Name AS WarehouseName, WarehouseIssues.WarehouseID AS WarehouseIssueID, WarehouseIssues.Code AS WarehouseIssueCode, WarehouseIssues.Name AS WarehouseIssueName " + "\r\n";

            queryString = queryString + "       FROM            WarehouseTransfers " + "\r\n";
            queryString = queryString + "                       INNER JOIN Warehouses ON WarehouseTransfers.WarehouseTransferID IN (SELECT WarehouseTransferID FROM @WarehouseTransferIDs) AND WarehouseTransfers.WarehouseReceiptID = Warehouses.WarehouseID " + "\r\n";
            queryString = queryString + "                       INNER JOIN Warehouses WarehouseIssues ON WarehouseTransfers.WarehouseID = WarehouseIssues.WarehouseID " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetGoodsReceiptPendingWarehouseTransfers", queryString);
        }

        private void GetGoodsReceiptPendingWarehouses()
        {
            string queryString = " @LocationID int, @NMVNTaskID int " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "       DECLARE         @WarehouseTransferIDs TABLE (WarehouseTransferID int NOT NULL) " + "\r\n";
            queryString = queryString + "       INSERT INTO     @WarehouseTransferIDs (WarehouseTransferID) SELECT WarehouseTransferID FROM WarehouseTransferDetails WHERE LocationReceiptID = @LocationID AND NMVNTaskID = @NMVNTaskID - 1000000 AND Approved = 1 AND ROUND(Quantity - QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") > 0 " + "\r\n";

            queryString = queryString + "       SELECT          " + (int)@GlobalEnums.GoodsReceiptTypeID.WarehouseTransfer + " AS GoodsReceiptTypeID, Warehouses.WarehouseID, Warehouses.Code AS WarehouseCode, Warehouses.Name AS WarehouseName, WarehouseIssues.WarehouseID AS WarehouseIssueID, WarehouseIssues.Code AS WarehouseIssueCode, WarehouseIssues.Name AS WarehouseIssueName " + "\r\n";

            queryString = queryString + "       FROM           (SELECT WarehouseReceiptID, WarehouseID FROM WarehouseTransfers WHERE WarehouseTransferID IN (SELECT WarehouseTransferID FROM @WarehouseTransferIDs) GROUP BY WarehouseReceiptID, WarehouseID) WarehousePENDING " + "\r\n";
            queryString = queryString + "                       INNER JOIN Warehouses ON WarehousePENDING.WarehouseReceiptID = Warehouses.WarehouseID " + "\r\n";
            queryString = queryString + "                       INNER JOIN Warehouses WarehouseIssues ON WarehousePENDING.WarehouseID = WarehouseIssues.WarehouseID " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetGoodsReceiptPendingWarehouses", queryString);
        }


        private void GetGoodsReceiptPendingWarehouseTransferDetails()
        {
            string queryString;

            queryString = " @NMVNTaskID int, @GoodsReceiptID Int, @WarehouseTransferID Int, @WarehouseID Int, @WarehouseIssueID Int, @WarehouseTransferDetailIDs varchar(3999), @OneStep bit " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "   BEGIN " + "\r\n";

            queryString = queryString + "       IF  (@WarehouseTransferID <> 0) " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLWarehouseTransfer(true) + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLWarehouseTransfer(false) + "\r\n";

            queryString = queryString + "   END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetGoodsReceiptPendingWarehouseTransferDetails", queryString);
        }

        private string BuildSQLWarehouseTransfer(bool isWarehouseTransferID)
        {
            string queryString = "";
            queryString = queryString + "   BEGIN " + "\r\n";
            queryString = queryString + "       IF  (@WarehouseTransferDetailIDs <> '') " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLWarehouseTransfer(isWarehouseTransferID, true) + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLWarehouseTransfer(isWarehouseTransferID, false) + "\r\n";
            queryString = queryString + "   END " + "\r\n";

            return queryString;
        }

        private string BuildSQLWarehouseTransfer(bool isWarehouseTransferID, bool isWarehouseTransferDetailIDs)
        {
            string queryString = "";
            queryString = queryString + "   BEGIN " + "\r\n";

            queryString = queryString + "       IF (@GoodsReceiptID <= 0) " + "\r\n";
            queryString = queryString + "               BEGIN " + "\r\n";
            queryString = queryString + "                   " + this.BuildSQLWarehouseTransferNew(isWarehouseTransferID, isWarehouseTransferDetailIDs) + "\r\n";
            queryString = queryString + "                   ORDER BY WarehouseTransfers.EntryDate, WarehouseTransfers.WarehouseTransferID, WarehouseTransferDetails.WarehouseTransferDetailID " + "\r\n";
            queryString = queryString + "               END " + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";

            queryString = queryString + "               BEGIN " + "\r\n";
            queryString = queryString + "                   " + this.BuildSQLWarehouseTransferNew(isWarehouseTransferID, isWarehouseTransferDetailIDs) + " WHERE WarehouseTransferDetails.WarehouseTransferDetailID NOT IN (SELECT WarehouseTransferDetailID FROM GoodsReceiptDetails WHERE GoodsReceiptID = @GoodsReceiptID) " + "\r\n";
            queryString = queryString + "                   UNION ALL " + "\r\n";
            queryString = queryString + "                   " + this.BuildSQLWarehouseTransferEdit(isWarehouseTransferID, isWarehouseTransferDetailIDs) + "\r\n";
            queryString = queryString + "                   ORDER BY WarehouseTransfers.EntryDate, WarehouseTransfers.WarehouseTransferID, WarehouseTransferDetails.WarehouseTransferDetailID " + "\r\n";
            queryString = queryString + "               END " + "\r\n";

            queryString = queryString + "   END " + "\r\n";

            return queryString;
        }

        private string BuildSQLWarehouseTransferNew(bool isWarehouseTransferID, bool isWarehouseTransferDetailIDs)
        {
            string queryString = "";

            queryString = queryString + "       SELECT      WarehouseTransfers.WarehouseTransferID, WarehouseTransferDetails.WarehouseTransferDetailID, WarehouseTransfers.Reference AS WarehouseTransferReference, WarehouseTransfers.EntryDate AS WarehouseTransferEntryDate, WarehouseTransferGoodsReceiptDetails.Reference AS GoodsReceiptReference, WarehouseTransferGoodsReceiptDetails.EntryDate AS GoodsReceiptEntryDate, WarehouseTransferGoodsReceiptDetails.BatchID, WarehouseTransferGoodsReceiptDetails.BatchEntryDate, WarehouseTransferGoodsReceiptDetails.SealCode, WarehouseTransferGoodsReceiptDetails.BatchCode, WarehouseTransferGoodsReceiptDetails.LabCode, WarehouseTransferGoodsReceiptDetails.Barcode, WarehouseTransferGoodsReceiptDetails.UnitWeight, WarehouseTransferGoodsReceiptDetails.TareWeight, " + "\r\n";
            queryString = queryString + "                   Commodities.CommodityID, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, Commodities.CommodityTypeID, WarehouseTransferGoodsReceiptDetails.LabID, WarehouseTransferGoodsReceiptDetails.ProductionDate, WarehouseTransferGoodsReceiptDetails.ExpiryDate, WarehouseTransferDetails.BinLocationID, BinLocations.Code AS BinLocationCode, " + "\r\n";
            queryString = queryString + "                   ROUND(WarehouseTransferDetails.Quantity - WarehouseTransferDetails.QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") AS QuantityRemains, " + "\r\n";
            queryString = queryString + "                   0.0 AS Quantity, WarehouseTransfers.Description, WarehouseTransferDetails.Remarks, CAST(1 AS bit) AS IsSelected " + "\r\n";

            queryString = queryString + "       FROM        WarehouseTransfers " + "\r\n";
            queryString = queryString + "                   INNER JOIN WarehouseTransferDetails ON " + (isWarehouseTransferID ? " WarehouseTransfers.WarehouseTransferID = @WarehouseTransferID " : "WarehouseTransfers.NMVNTaskID = @NMVNTaskID - 1000000 AND WarehouseTransfers.WarehouseReceiptID = @WarehouseID AND WarehouseTransfers.WarehouseID = @WarehouseIssueID ") + " AND (@OneStep = 1 OR WarehouseTransferDetails.Approved = 1) AND ROUND(WarehouseTransferDetails.Quantity - WarehouseTransferDetails.QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") > 0 AND WarehouseTransfers.WarehouseTransferID = WarehouseTransferDetails.WarehouseTransferID" + (isWarehouseTransferDetailIDs ? " AND WarehouseTransferDetails.WarehouseTransferDetailID NOT IN (SELECT Id FROM dbo.SplitToIntList (@WarehouseTransferDetailIDs))" : "") + "\r\n";
            queryString = queryString + "                   INNER JOIN Commodities ON WarehouseTransferDetails.CommodityID = Commodities.CommodityID " + "\r\n";
            queryString = queryString + "                   INNER JOIN BinLocations ON WarehouseTransferDetails.BinLocationID = BinLocations.BinLocationID " + "\r\n";
            queryString = queryString + "                   INNER JOIN GoodsReceiptDetails WarehouseTransferGoodsReceiptDetails ON WarehouseTransferDetails.GoodsReceiptDetailID = WarehouseTransferGoodsReceiptDetails.GoodsReceiptDetailID " + "\r\n";

            return queryString;
        }

        private string BuildSQLWarehouseTransferEdit(bool isWarehouseTransferID, bool isWarehouseTransferDetailIDs)
        {
            string queryString = "";

            queryString = queryString + "       SELECT      WarehouseTransfers.WarehouseTransferID, WarehouseTransferDetails.WarehouseTransferDetailID, WarehouseTransfers.Reference AS WarehouseTransferReference, WarehouseTransfers.EntryDate AS WarehouseTransferEntryDate, WarehouseTransferGoodsReceiptDetails.Reference AS GoodsReceiptReference, WarehouseTransferGoodsReceiptDetails.EntryDate AS GoodsReceiptEntryDate, WarehouseTransferGoodsReceiptDetails.BatchID, WarehouseTransferGoodsReceiptDetails.BatchEntryDate, WarehouseTransferGoodsReceiptDetails.SealCode, WarehouseTransferGoodsReceiptDetails.BatchCode, WarehouseTransferGoodsReceiptDetails.LabCode, WarehouseTransferGoodsReceiptDetails.Barcode, WarehouseTransferGoodsReceiptDetails.UnitWeight, WarehouseTransferGoodsReceiptDetails.TareWeight, " + "\r\n";
            queryString = queryString + "                   Commodities.CommodityID, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, Commodities.CommodityTypeID, WarehouseTransferGoodsReceiptDetails.LabID, WarehouseTransferGoodsReceiptDetails.ProductionDate, WarehouseTransferGoodsReceiptDetails.ExpiryDate, WarehouseTransferDetails.BinLocationID, BinLocations.Code AS BinLocationCode, " + "\r\n";
            queryString = queryString + "                   ROUND(WarehouseTransferDetails.Quantity - WarehouseTransferDetails.QuantityReceipted + GoodsReceiptDetails.Quantity, " + (int)GlobalEnums.rndQuantity + ") AS QuantityRemains, " + "\r\n";
            queryString = queryString + "                   0.0 AS Quantity, WarehouseTransfers.Description, WarehouseTransferDetails.Remarks, CAST(1 AS bit) AS IsSelected " + "\r\n";

            queryString = queryString + "       FROM        WarehouseTransferDetails " + "\r\n";
            queryString = queryString + "                   INNER JOIN GoodsReceiptDetails ON GoodsReceiptDetails.GoodsReceiptID = @GoodsReceiptID AND WarehouseTransferDetails.WarehouseTransferDetailID = GoodsReceiptDetails.WarehouseTransferDetailID" + (isWarehouseTransferDetailIDs ? " AND WarehouseTransferDetails.WarehouseTransferDetailID NOT IN (SELECT Id FROM dbo.SplitToIntList (@WarehouseTransferDetailIDs))" : "") + "\r\n";
            queryString = queryString + "                   INNER JOIN Commodities ON WarehouseTransferDetails.CommodityID = Commodities.CommodityID " + "\r\n";
            queryString = queryString + "                   INNER JOIN BinLocations ON WarehouseTransferDetails.BinLocationID = BinLocations.BinLocationID " + "\r\n";
            queryString = queryString + "                   INNER JOIN WarehouseTransfers ON WarehouseTransferDetails.WarehouseTransferID = WarehouseTransfers.WarehouseTransferID " + "\r\n";
            queryString = queryString + "                   INNER JOIN GoodsReceiptDetails WarehouseTransferGoodsReceiptDetails ON WarehouseTransferDetails.GoodsReceiptDetailID = WarehouseTransferGoodsReceiptDetails.GoodsReceiptDetailID " + "\r\n";

            return queryString;
        }
        #endregion WarehouseTransfer


        #region FinishedProduct
        private void GetGoodsReceiptPendingPlannedOrders()
        {
            string queryString = " @LocationID int " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";
            queryString = queryString + "       SELECT          " + (int)@GlobalEnums.GoodsReceiptTypeID.FinishedProduct + " AS GoodsReceiptTypeID, PlannedOrders.PlannedOrderID, PlannedOrders.EntryDate AS PlannedOrderEntryDate, PlannedOrders.Reference AS PlannedOrderReference, PlannedOrders.Code AS PlannedOrderCode, PlannedOrders.Caption AS PlannedOrderCaption, FinishedProductPackages.MinHandoverDate, Customers.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, Warehouses.WarehouseID, Warehouses.Code AS WarehouseCode, Warehouses.Name AS WarehouseName " + "\r\n";
            queryString = queryString + "       FROM            PlannedOrders " + "\r\n";
            queryString = queryString + "                       INNER JOIN (SELECT PlannedOrderID, MIN(FinishedHandoverDate) AS MinHandoverDate FROM FinishedProductPackages WHERE LocationID = @LocationID AND Approved = 1 AND HandoverApproved = 1 AND ROUND(Quantity - QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") > 0 GROUP BY PlannedOrderID) FinishedProductPackages ON PlannedOrders.PlannedOrderID = FinishedProductPackages.PlannedOrderID " + "\r\n";
            queryString = queryString + "                       INNER JOIN Customers ON PlannedOrders.CustomerID = Customers.CustomerID " + "\r\n";


            queryString = queryString + "                       INNER JOIN Warehouses ON Warehouses.WarehouseID = 3 " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetGoodsReceiptPendingPlannedOrders", queryString);
        }

        private void GetGoodsReceiptPendingPlannedOrderCustomers()
        {
            string queryString = " @LocationID int " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";
            queryString = queryString + "       SELECT          " + (int)@GlobalEnums.GoodsReceiptTypeID.FinishedProduct + " AS GoodsReceiptTypeID, Customers.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, Warehouses.WarehouseID, Warehouses.Code AS WarehouseCode, Warehouses.Name AS WarehouseName " + "\r\n";
            queryString = queryString + "       FROM            Customers " + "\r\n";

            queryString = queryString + "                       INNER JOIN Warehouses ON Warehouses.WarehouseID = 3 " + "\r\n";

            queryString = queryString + "       WHERE           CustomerID IN (SELECT DISTINCT CustomerID FROM FinishedProductPackages WHERE LocationID = @LocationID AND Approved = 1 AND HandoverApproved = 1 AND ROUND(Quantity - QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") > 0 GROUP BY CustomerID) " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetGoodsReceiptPendingPlannedOrderCustomers", queryString);
        }


        private void GetGoodsReceiptPendingPlannedOrderDetails()
        {
            string queryString;

            queryString = " @LocationID Int, @GoodsReceiptID Int, @PlannedOrderID Int, @CustomerID Int, @FinishedProductPackageIDs varchar(3999), @IsReadonly bit " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "   BEGIN " + "\r\n";

            queryString = queryString + "       IF  (@PlannedOrderID <> 0) " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLFinishedProduct(true) + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLFinishedProduct(false) + "\r\n";

            queryString = queryString + "   END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetGoodsReceiptPendingPlannedOrderDetails", queryString);
        }

        private string BuildSQLFinishedProduct(bool isFinishedProductID)
        {
            string queryString = "";
            queryString = queryString + "   BEGIN " + "\r\n";
            queryString = queryString + "       IF  (@FinishedProductPackageIDs <> '') " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLFinishedProduct(isFinishedProductID, true) + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLFinishedProduct(isFinishedProductID, false) + "\r\n";
            queryString = queryString + "   END " + "\r\n";

            return queryString;
        }

        private string BuildSQLFinishedProduct(bool isFinishedProductID, bool isFinishedProductPackageIDs)
        {
            string queryString = "";
            queryString = queryString + "   BEGIN " + "\r\n";

            queryString = queryString + "       IF (@GoodsReceiptID <= 0) " + "\r\n";
            queryString = queryString + "               BEGIN " + "\r\n";
            queryString = queryString + "                   " + this.BuildSQLFinishedProductNew(isFinishedProductID, isFinishedProductPackageIDs) + "\r\n";
            queryString = queryString + "                   ORDER BY FinishedProductPackages.EntryDate, FinishedProductPackages.FinishedProductID, FinishedProductPackages.FinishedProductPackageID " + "\r\n";
            queryString = queryString + "               END " + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";

            queryString = queryString + "               IF (@IsReadonly = 1) " + "\r\n";
            queryString = queryString + "                   BEGIN " + "\r\n";
            queryString = queryString + "                       " + this.BuildSQLFinishedProductEdit(isFinishedProductID, isFinishedProductPackageIDs) + "\r\n";
            queryString = queryString + "                       ORDER BY FinishedProductPackages.EntryDate, FinishedProductPackages.FinishedProductID, FinishedProductPackages.FinishedProductPackageID " + "\r\n";
            queryString = queryString + "                   END " + "\r\n";

            queryString = queryString + "               ELSE " + "\r\n"; //FULL SELECT FOR EDIT MODE

            queryString = queryString + "                   BEGIN " + "\r\n";
            queryString = queryString + "                       " + this.BuildSQLFinishedProductNew(isFinishedProductID, isFinishedProductPackageIDs) + " WHERE FinishedProductPackages.FinishedProductPackageID NOT IN (SELECT FinishedProductPackageID FROM GoodsReceiptDetails WHERE GoodsReceiptID = @GoodsReceiptID) " + "\r\n";
            queryString = queryString + "                       UNION ALL " + "\r\n";
            queryString = queryString + "                       " + this.BuildSQLFinishedProductEdit(isFinishedProductID, isFinishedProductPackageIDs) + "\r\n";
            queryString = queryString + "                       ORDER BY FinishedProductPackages.EntryDate, FinishedProductPackages.FinishedProductID, FinishedProductPackages.FinishedProductPackageID " + "\r\n";
            queryString = queryString + "                   END " + "\r\n";

            queryString = queryString + "   END " + "\r\n";

            return queryString;
        }

        private string BuildSQLFinishedProductNew(bool isFinishedProductID, bool isFinishedProductPackageIDs)
        {
            string queryString = "";

            queryString = queryString + "       SELECT      FinishedProductPackages.FinishedProductID, FinishedProductPackages.FinishedProductPackageID, FinishedProductPackages.EntryDate AS FinishedProductEntryDate, FirmOrders.Reference AS FirmOrderReference, FirmOrders.Code AS FirmOrderCode, FinishedProductPackages.SemifinishedProductReferences, FinishedProductPackages.BatchID, FinishedProductPackages.BatchEntryDate, Batches.Code AS BatchCode, " + "\r\n";
            queryString = queryString + "                   Commodities.CommodityID, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, Commodities.CommodityTypeID, " + "\r\n";
            queryString = queryString + "                   ROUND(FinishedProductPackages.Quantity - FinishedProductPackages.QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") AS QuantityRemains, " + "\r\n";
            queryString = queryString + "                   0.0 AS Quantity, FinishedProductPackages.Remarks, CAST(1 AS bit) AS IsSelected " + "\r\n";

            queryString = queryString + "       FROM        FirmOrders " + "\r\n";
            queryString = queryString + "                   INNER JOIN FinishedProductPackages ON " + (isFinishedProductID ? " FinishedProductPackages.PlannedOrderID = @PlannedOrderID " : "FinishedProductPackages.LocationID = @LocationID AND FinishedProductPackages.CustomerID = @CustomerID ") + " AND FinishedProductPackages.Approved = 1 AND FinishedProductPackages.HandoverApproved = 1 AND ROUND(FinishedProductPackages.Quantity - FinishedProductPackages.QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") > 0 AND FirmOrders.FirmOrderID = FinishedProductPackages.FirmOrderID " + (isFinishedProductPackageIDs ? " AND FinishedProductPackages.FinishedProductPackageID NOT IN (SELECT Id FROM dbo.SplitToIntList (@FinishedProductPackageIDs))" : "") + "\r\n";
            queryString = queryString + "                   INNER JOIN Commodities ON FinishedProductPackages.CommodityID = Commodities.CommodityID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Batches ON FinishedProductPackages.BatchID = Batches.BatchID " + "\r\n";

            return queryString;
        }

        private string BuildSQLFinishedProductEdit(bool isFinishedProductID, bool isFinishedProductPackageIDs)
        {
            string queryString = "";

            queryString = queryString + "       SELECT      FinishedProductPackages.FinishedProductID, FinishedProductPackages.FinishedProductPackageID, FinishedProductPackages.EntryDate AS FinishedProductEntryDate, FirmOrders.Reference AS FirmOrderReference, FirmOrders.Code AS FirmOrderCode, FinishedProductPackages.SemifinishedProductReferences, FinishedProductPackages.BatchID, FinishedProductPackages.BatchEntryDate, Batches.Code AS BatchCode, " + "\r\n";
            queryString = queryString + "                   Commodities.CommodityID, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, Commodities.CommodityTypeID, " + "\r\n";
            queryString = queryString + "                   ROUND(FinishedProductPackages.Quantity - FinishedProductPackages.QuantityReceipted + GoodsReceiptDetails.Quantity, " + (int)GlobalEnums.rndQuantity + ") AS QuantityRemains, " + "\r\n";
            queryString = queryString + "                   0.0 AS Quantity, FinishedProductPackages.Remarks, CAST(1 AS bit) AS IsSelected " + "\r\n";

            queryString = queryString + "       FROM        FinishedProductPackages " + "\r\n";
            queryString = queryString + "                   INNER JOIN GoodsReceiptDetails ON GoodsReceiptDetails.GoodsReceiptID = @GoodsReceiptID AND FinishedProductPackages.FinishedProductPackageID = GoodsReceiptDetails.FinishedProductPackageID" + (isFinishedProductPackageIDs ? " AND FinishedProductPackages.FinishedProductPackageID NOT IN (SELECT Id FROM dbo.SplitToIntList (@FinishedProductPackageIDs))" : "") + "\r\n";
            queryString = queryString + "                   INNER JOIN FirmOrders ON FinishedProductPackages.FirmOrderID = FirmOrders.FirmOrderID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Commodities ON FinishedProductPackages.CommodityID = Commodities.CommodityID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Batches ON FinishedProductPackages.BatchID = Batches.BatchID " + "\r\n";

            return queryString;
        }

        #endregion FinishedProduct



        #region FinishedItem
        private void GetGoodsReceiptPendingPlannedItems()
        {
            string queryString = " @LocationID int " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";
            queryString = queryString + "       SELECT          " + (int)@GlobalEnums.GoodsReceiptTypeID.FinishedItem + " AS GoodsReceiptTypeID, PlannedOrders.PlannedOrderID, PlannedOrders.EntryDate AS PlannedOrderEntryDate, PlannedOrders.Reference AS PlannedOrderReference, PlannedOrders.Code AS PlannedOrderCode, PlannedOrders.Caption AS PlannedOrderCaption, FinishedItemPackages.MinHandoverDate, Customers.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, Warehouses.WarehouseID, Warehouses.Code AS WarehouseCode, Warehouses.Name AS WarehouseName " + "\r\n";
            queryString = queryString + "       FROM            PlannedOrders " + "\r\n";
            queryString = queryString + "                       INNER JOIN (SELECT PlannedOrderID, MIN(FinishedHandoverDate) AS MinHandoverDate FROM FinishedItemPackages WHERE LocationID = @LocationID AND Approved = 1 AND HandoverApproved = 1 AND ROUND(Quantity - QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") > 0 GROUP BY PlannedOrderID) FinishedItemPackages ON PlannedOrders.PlannedOrderID = FinishedItemPackages.PlannedOrderID " + "\r\n";
            queryString = queryString + "                       INNER JOIN Customers ON PlannedOrders.CustomerID = Customers.CustomerID " + "\r\n";

            queryString = queryString + "                       INNER JOIN Warehouses ON Warehouses.WarehouseID = 2 " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetGoodsReceiptPendingPlannedItems", queryString);
        }

        private void GetGoodsReceiptPendingPlannedItemCustomers()
        {
            string queryString = " @LocationID int " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";
            queryString = queryString + "       SELECT          " + (int)@GlobalEnums.GoodsReceiptTypeID.FinishedItem + " AS GoodsReceiptTypeID, Customers.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, Warehouses.WarehouseID, Warehouses.Code AS WarehouseCode, Warehouses.Name AS WarehouseName " + "\r\n";
            queryString = queryString + "       FROM            Customers " + "\r\n";

            queryString = queryString + "                       INNER JOIN Warehouses ON Warehouses.WarehouseID = 2 " + "\r\n";

            queryString = queryString + "       WHERE           CustomerID IN (SELECT DISTINCT CustomerID FROM FinishedItemPackages WHERE LocationID = @LocationID AND Approved = 1 AND HandoverApproved = 1 AND ROUND(Quantity - QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") > 0 GROUP BY CustomerID) " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetGoodsReceiptPendingPlannedItemCustomers", queryString);
        }


        private void GetGoodsReceiptPendingPlannedItemDetails()
        {
            string queryString;

            queryString = " @LocationID Int, @GoodsReceiptID Int, @PlannedOrderID Int, @CustomerID Int, @FinishedItemPackageIDs varchar(3999) " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "   BEGIN " + "\r\n";

            queryString = queryString + "       IF  (@PlannedOrderID <> 0) " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLFinishedItem(true) + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLFinishedItem(false) + "\r\n";

            queryString = queryString + "   END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetGoodsReceiptPendingPlannedItemDetails", queryString);
        }

        private string BuildSQLFinishedItem(bool isFinishedItemID)
        {
            string queryString = "";
            queryString = queryString + "   BEGIN " + "\r\n";
            queryString = queryString + "       IF  (@FinishedItemPackageIDs <> '') " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLFinishedItem(isFinishedItemID, true) + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLFinishedItem(isFinishedItemID, false) + "\r\n";
            queryString = queryString + "   END " + "\r\n";

            return queryString;
        }

        private string BuildSQLFinishedItem(bool isFinishedItemID, bool isFinishedItemPackageIDs)
        {
            string queryString = "";
            queryString = queryString + "   BEGIN " + "\r\n";

            queryString = queryString + "       IF (@GoodsReceiptID <= 0) " + "\r\n";
            queryString = queryString + "               BEGIN " + "\r\n";
            queryString = queryString + "                   " + this.BuildSQLFinishedItemNew(isFinishedItemID, isFinishedItemPackageIDs) + "\r\n";
            queryString = queryString + "                   ORDER BY FinishedItemPackages.EntryDate, FinishedItemPackages.FinishedItemID, FinishedItemPackages.FinishedItemPackageID " + "\r\n";
            queryString = queryString + "               END " + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "               BEGIN " + "\r\n";
            queryString = queryString + "                   " + this.BuildSQLFinishedItemNew(isFinishedItemID, isFinishedItemPackageIDs) + " WHERE FinishedItemPackages.FinishedItemPackageID NOT IN (SELECT FinishedItemPackageID FROM GoodsReceiptDetails WHERE GoodsReceiptID = @GoodsReceiptID) " + "\r\n";
            queryString = queryString + "                   UNION ALL " + "\r\n";
            queryString = queryString + "                   " + this.BuildSQLFinishedItemEdit(isFinishedItemID, isFinishedItemPackageIDs) + "\r\n";
            queryString = queryString + "                   ORDER BY FinishedItemPackages.EntryDate, FinishedItemPackages.FinishedItemID, FinishedItemPackages.FinishedItemPackageID " + "\r\n";
            queryString = queryString + "               END " + "\r\n";

            queryString = queryString + "   END " + "\r\n";

            return queryString;
        }

        private string BuildSQLFinishedItemNew(bool isFinishedItemID, bool isFinishedItemPackageIDs)
        {
            string queryString = "";

            queryString = queryString + "       SELECT      FinishedItemPackages.FinishedItemID, FinishedItemPackages.FinishedItemPackageID, FinishedItemPackages.EntryDate AS FinishedItemEntryDate, FirmOrders.Reference AS FirmOrderReference, FirmOrders.Code AS FirmOrderCode, FinishedItemPackages.SemifinishedItemReferences, FinishedItemPackages.BatchID, FinishedItemPackages.BatchEntryDate, Batches.Code AS BatchCode, " + "\r\n";
            queryString = queryString + "                   Commodities.CommodityID, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, Commodities.CommodityTypeID, " + "\r\n";
            queryString = queryString + "                   ROUND(FinishedItemPackages.Quantity - FinishedItemPackages.QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") AS QuantityRemains, " + "\r\n";
            queryString = queryString + "                   0.0 AS Quantity, FinishedItemPackages.Remarks, CAST(1 AS bit) AS IsSelected " + "\r\n";

            queryString = queryString + "       FROM        FirmOrders " + "\r\n";
            queryString = queryString + "                   INNER JOIN FinishedItemPackages ON " + (isFinishedItemID ? " FinishedItemPackages.PlannedOrderID = @PlannedOrderID " : "FinishedItemPackages.LocationID = @LocationID AND FinishedItemPackages.CustomerID = @CustomerID ") + " AND FinishedItemPackages.Approved = 1 AND FinishedItemPackages.HandoverApproved = 1 AND ROUND(FinishedItemPackages.Quantity - FinishedItemPackages.QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") > 0 AND FirmOrders.FirmOrderID = FinishedItemPackages.FirmOrderID " + (isFinishedItemPackageIDs ? " AND FinishedItemPackages.FinishedItemPackageID NOT IN (SELECT Id FROM dbo.SplitToIntList (@FinishedItemPackageIDs))" : "") + "\r\n";
            queryString = queryString + "                   INNER JOIN Commodities ON FinishedItemPackages.CommodityID = Commodities.CommodityID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Batches ON FinishedItemPackages.BatchID = Batches.BatchID " + "\r\n";

            return queryString;
        }

        private string BuildSQLFinishedItemEdit(bool isFinishedItemID, bool isFinishedItemPackageIDs)
        {
            string queryString = "";

            queryString = queryString + "       SELECT      FinishedItemPackages.FinishedItemID, FinishedItemPackages.FinishedItemPackageID, FinishedItemPackages.EntryDate AS FinishedItemEntryDate, FirmOrders.Reference AS FirmOrderReference, FirmOrders.Code AS FirmOrderCode, FinishedItemPackages.SemifinishedItemReferences, FinishedItemPackages.BatchID, FinishedItemPackages.BatchEntryDate, Batches.Code AS BatchCode, " + "\r\n";
            queryString = queryString + "                   Commodities.CommodityID, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, Commodities.CommodityTypeID, " + "\r\n";
            queryString = queryString + "                   ROUND(FinishedItemPackages.Quantity - FinishedItemPackages.QuantityReceipted + GoodsReceiptDetails.Quantity, " + (int)GlobalEnums.rndQuantity + ") AS QuantityRemains, " + "\r\n";
            queryString = queryString + "                   0.0 AS Quantity, FinishedItemPackages.Remarks, CAST(1 AS bit) AS IsSelected " + "\r\n";

            queryString = queryString + "       FROM        FinishedItemPackages " + "\r\n";
            queryString = queryString + "                   INNER JOIN GoodsReceiptDetails ON GoodsReceiptDetails.GoodsReceiptID = @GoodsReceiptID AND FinishedItemPackages.FinishedItemPackageID = GoodsReceiptDetails.FinishedItemPackageID" + (isFinishedItemPackageIDs ? " AND FinishedItemPackages.FinishedItemPackageID NOT IN (SELECT Id FROM dbo.SplitToIntList (@FinishedItemPackageIDs))" : "") + "\r\n";
            queryString = queryString + "                   INNER JOIN FirmOrders ON FinishedItemPackages.FirmOrderID = FirmOrders.FirmOrderID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Commodities ON FinishedItemPackages.CommodityID = Commodities.CommodityID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Batches ON FinishedItemPackages.BatchID = Batches.BatchID " + "\r\n";

            return queryString;
        }

        #endregion FinishedItem





        #region Recyclate
        private void GetGoodsReceiptPendingRecyclates()
        {
            string queryString = " @LocationID int " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";
            queryString = queryString + "       SELECT          " + (int)@GlobalEnums.GoodsReceiptTypeID.Recyclate + " AS GoodsReceiptTypeID, Recyclates.RecyclateID, Recyclates.EntryDate AS RecyclateEntryDate, Recyclates.Reference AS RecyclateReference, Workshifts.EntryDate AS WorkshiftEntryDate, Workshifts.Code AS WorkshiftCode, Recyclates.Caption AS RecyclateCaption, Warehouses.WarehouseID, Warehouses.Code AS WarehouseCode, Warehouses.Name AS WarehouseName, RecyclatePackages.QuantityRemains " + "\r\n";
            queryString = queryString + "       FROM            Recyclates " + "\r\n";
            queryString = queryString + "                       INNER JOIN (SELECT RecyclateID, ROUND(SUM(Quantity - QuantityReceipted), " + (int)GlobalEnums.rndQuantity + ") AS QuantityRemains FROM RecyclatePackages WHERE Approved = 1 AND ROUND(Quantity - QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") > 0 GROUP BY RecyclateID) RecyclatePackages ON Recyclates.RecyclateID = RecyclatePackages.RecyclateID " + "\r\n";
            queryString = queryString + "                       INNER JOIN Workshifts ON Recyclates.WorkshiftID = Workshifts.WorkshiftID " + "\r\n";
            queryString = queryString + "                       INNER JOIN Warehouses ON Recyclates.WarehouseID = Warehouses.WarehouseID " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetGoodsReceiptPendingRecyclates", queryString);
        }

        private void GetGoodsReceiptPendingRecyclateDetails()
        {
            string queryString;

            queryString = " @LocationID Int, @GoodsReceiptID Int, @RecyclateID Int, @RecyclatePackageIDs varchar(3999) " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "   BEGIN " + "\r\n";

            queryString = queryString + "       IF  (@RecyclatePackageIDs <> '') " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLRecyclate(true) + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLRecyclate(false) + "\r\n";

            queryString = queryString + "   END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetGoodsReceiptPendingRecyclateDetails", queryString);
        }

        private string BuildSQLRecyclate(bool isRecyclatePackageIDs)
        {
            string queryString = "";
            queryString = queryString + "   BEGIN " + "\r\n";

            queryString = queryString + "       IF (@GoodsReceiptID <= 0) " + "\r\n";
            queryString = queryString + "               BEGIN " + "\r\n";
            queryString = queryString + "                   " + this.BuildSQLRecyclateNew(isRecyclatePackageIDs) + "\r\n";
            queryString = queryString + "                   ORDER BY RecyclatePackages.RecyclatePackageID " + "\r\n";
            queryString = queryString + "               END " + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "               BEGIN " + "\r\n";
            queryString = queryString + "                   " + this.BuildSQLRecyclateNew(isRecyclatePackageIDs) + " WHERE RecyclatePackages.RecyclatePackageID NOT IN (SELECT RecyclatePackageID FROM GoodsReceiptDetails WHERE GoodsReceiptID = @GoodsReceiptID) " + "\r\n";
            queryString = queryString + "                   UNION ALL " + "\r\n";
            queryString = queryString + "                   " + this.BuildSQLRecyclateEdit(isRecyclatePackageIDs) + "\r\n";
            queryString = queryString + "                   ORDER BY RecyclatePackages.RecyclatePackageID " + "\r\n";
            queryString = queryString + "               END " + "\r\n";

            queryString = queryString + "   END " + "\r\n";

            return queryString;
        }

        private string BuildSQLRecyclateNew(bool isRecyclatePackageIDs)
        {
            string queryString = "";

            queryString = queryString + "       SELECT      RecyclatePackages.RecyclateID, RecyclatePackages.RecyclatePackageID, RecyclatePackages.EntryDate AS RecyclateEntryDate, RecyclatePackages.BatchID, RecyclatePackages.BatchEntryDate, Batches.Code AS BatchCode, " + "\r\n";
            queryString = queryString + "                   Commodities.CommodityID, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, Commodities.CommodityTypeID, " + "\r\n";
            queryString = queryString + "                   ROUND(RecyclatePackages.Quantity - RecyclatePackages.QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") AS QuantityRemains, " + "\r\n";
            queryString = queryString + "                   0.0 AS Quantity, RecyclatePackages.Remarks, CAST(1 AS bit) AS IsSelected " + "\r\n";

            queryString = queryString + "       FROM        Commodities " + "\r\n";
            queryString = queryString + "                   INNER JOIN RecyclatePackages ON RecyclatePackages.RecyclateID = @RecyclateID AND RecyclatePackages.Approved = 1 AND ROUND(RecyclatePackages.Quantity - RecyclatePackages.QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") > 0 AND RecyclatePackages.CommodityID = Commodities.CommodityID " + (isRecyclatePackageIDs ? " AND RecyclatePackages.RecyclatePackageID NOT IN (SELECT Id FROM dbo.SplitToIntList (@RecyclatePackageIDs))" : "") + "\r\n";
            queryString = queryString + "                   INNER JOIN Batches ON RecyclatePackages.BatchID = Batches.BatchID " + "\r\n";

            return queryString;
        }

        private string BuildSQLRecyclateEdit(bool isRecyclatePackageIDs)
        {
            string queryString = "";

            queryString = queryString + "       SELECT      RecyclatePackages.RecyclateID, RecyclatePackages.RecyclatePackageID, RecyclatePackages.EntryDate AS RecyclateEntryDate, RecyclatePackages.BatchID, RecyclatePackages.BatchEntryDate, Batches.Code AS BatchCode, " + "\r\n";
            queryString = queryString + "                   Commodities.CommodityID, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, Commodities.CommodityTypeID, " + "\r\n";
            queryString = queryString + "                   ROUND(RecyclatePackages.Quantity - RecyclatePackages.QuantityReceipted + GoodsReceiptDetails.Quantity, " + (int)GlobalEnums.rndQuantity + ") AS QuantityRemains, " + "\r\n";
            queryString = queryString + "                   0.0 AS Quantity, RecyclatePackages.Remarks, CAST(1 AS bit) AS IsSelected " + "\r\n";

            queryString = queryString + "       FROM        RecyclatePackages " + "\r\n";
            queryString = queryString + "                   INNER JOIN GoodsReceiptDetails ON GoodsReceiptDetails.GoodsReceiptID = @GoodsReceiptID AND RecyclatePackages.RecyclatePackageID = GoodsReceiptDetails.RecyclatePackageID" + (isRecyclatePackageIDs ? " AND RecyclatePackages.RecyclatePackageID NOT IN (SELECT Id FROM dbo.SplitToIntList (@RecyclatePackageIDs))" : "") + "\r\n";
            queryString = queryString + "                   INNER JOIN Commodities ON RecyclatePackages.CommodityID = Commodities.CommodityID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Batches ON RecyclatePackages.BatchID = Batches.BatchID " + "\r\n";

            return queryString;
        }

        #endregion Recyclate



        #region MaterialIssue
        private void GetGoodsReceiptPendingMaterialIssueDetails()
        {
            string queryString;

            queryString = " @LocationID Int, @GoodsReceiptID Int, @MaterialIssueDetailIDs varchar(3999), @IsReadonly bit " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "   BEGIN " + "\r\n";

            queryString = queryString + "       IF  (@MaterialIssueDetailIDs <> '') " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLMaterialIssue(true) + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLMaterialIssue(false) + "\r\n";

            queryString = queryString + "   END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetGoodsReceiptPendingMaterialIssueDetails", queryString);
        }

        private string BuildSQLMaterialIssue(bool isMaterialIssueDetailIDs)
        {
            string queryString = "";
            queryString = queryString + "   BEGIN " + "\r\n";

            queryString = queryString + "       IF (@GoodsReceiptID <= 0) " + "\r\n";
            queryString = queryString + "               BEGIN " + "\r\n";
            queryString = queryString + "                   " + this.BuildSQLMaterialIssueNew(isMaterialIssueDetailIDs) + "\r\n";
            queryString = queryString + "                   ORDER BY MaterialIssueDetails.EntryDate, MaterialIssueDetails.MaterialIssueID, MaterialIssueDetails.MaterialIssueDetailID " + "\r\n";
            queryString = queryString + "               END " + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";

            queryString = queryString + "               IF (@IsReadonly = 1) " + "\r\n";
            queryString = queryString + "                   BEGIN " + "\r\n";
            queryString = queryString + "                       " + this.BuildSQLMaterialIssueEdit(isMaterialIssueDetailIDs) + "\r\n";
            queryString = queryString + "                       ORDER BY MaterialIssueDetails.EntryDate, MaterialIssueDetails.MaterialIssueID, MaterialIssueDetails.MaterialIssueDetailID " + "\r\n";
            queryString = queryString + "                   END " + "\r\n";

            queryString = queryString + "               ELSE " + "\r\n"; //FULL SELECT FOR EDIT MODE

            queryString = queryString + "                   BEGIN " + "\r\n";
            queryString = queryString + "                       " + this.BuildSQLMaterialIssueNew(isMaterialIssueDetailIDs) + " WHERE MaterialIssueDetails.MaterialIssueDetailID NOT IN (SELECT MaterialIssueDetailID FROM GoodsReceiptDetails WHERE GoodsReceiptID = @GoodsReceiptID) " + "\r\n";
            queryString = queryString + "                       UNION ALL " + "\r\n";
            queryString = queryString + "                       " + this.BuildSQLMaterialIssueEdit(isMaterialIssueDetailIDs) + "\r\n";
            queryString = queryString + "                       ORDER BY MaterialIssueDetails.EntryDate, MaterialIssueDetails.MaterialIssueID, MaterialIssueDetails.MaterialIssueDetailID " + "\r\n";
            queryString = queryString + "                   END " + "\r\n";

            queryString = queryString + "   END " + "\r\n";

            return queryString;
        }

        private string BuildSQLMaterialIssueNew(bool isMaterialIssueDetailIDs)
        {
            string queryString = "";

            queryString = queryString + "       SELECT      MaterialIssueDetails.MaterialIssueID, MaterialIssueDetails.MaterialIssueDetailID, FirmOrders.Reference AS FirmOrderReference, FirmOrders.Code AS FirmOrderCode, FirmOrders.Specs AS FirmOrderSpecs, MaterialIssueDetails.EntryDate AS MaterialIssueEntryDate, MaterialIssueDetails.BatchID, MaterialIssueDetails.BatchEntryDate, MaterialIssueDetails.Barcode, MaterialIssueDetails.BatchCode, MaterialIssueDetails.SealCode, MaterialIssueDetails.LabCode, " + "\r\n";
            queryString = queryString + "                   Commodities.CommodityID, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, Commodities.CommodityTypeID, MaterialIssueDetails.LabID, MaterialIssueDetails.ProductionDate, MaterialIssueDetails.ExpiryDate, " + "\r\n";
            queryString = queryString + "                   ROUND(MaterialIssueDetails.Quantity - MaterialIssueDetails.QuantitySemifinished - MaterialIssueDetails.QuantityFailure - MaterialIssueDetails.QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") AS QuantityRemains, " + "\r\n"; //QuantityRemains NOT INCLUDED QuantityLoss, BUT: CHECK PENDING MUST MINUS QuantityLoss!!!!!!
            queryString = queryString + "                   0.0 AS Quantity, MaterialIssueDetails.Remarks, CAST(0 AS bit) AS IsSelected, " + "\r\n";
            queryString = queryString + "                   Workshifts.Name AS WorkshiftName, Workshifts.EntryDate AS WorkshiftEntryDate, ProductionLines.Code AS ProductionLinesCode " + "\r\n";

            queryString = queryString + "       FROM        FirmOrders " + "\r\n";
            queryString = queryString + "                   INNER JOIN MaterialIssueDetails ON MaterialIssueDetails.LocationID = @LocationID AND MaterialIssueDetails.Approved = 1 AND ROUND(MaterialIssueDetails.Quantity - MaterialIssueDetails.QuantitySemifinished - MaterialIssueDetails.QuantityFailure - MaterialIssueDetails.QuantityReceipted - MaterialIssueDetails.QuantityLoss, " + (int)GlobalEnums.rndQuantity + ") > 0 AND FirmOrders.FirmOrderID = MaterialIssueDetails.FirmOrderID" + (isMaterialIssueDetailIDs ? " AND MaterialIssueDetails.MaterialIssueDetailID NOT IN (SELECT Id FROM dbo.SplitToIntList (@MaterialIssueDetailIDs))" : "") + "\r\n";
            queryString = queryString + "                   INNER JOIN Commodities ON MaterialIssueDetails.CommodityID = Commodities.CommodityID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Workshifts ON MaterialIssueDetails.WorkshiftID = Workshifts.WorkshiftID " + "\r\n";
            queryString = queryString + "                   INNER JOIN ProductionLines ON MaterialIssueDetails.ProductionLineID = ProductionLines.ProductionLineID " + "\r\n";

            return queryString;
        }

        private string BuildSQLMaterialIssueEdit(bool isMaterialIssueDetailIDs)
        {
            string queryString = "";

            queryString = queryString + "       SELECT      MaterialIssueDetails.MaterialIssueID, MaterialIssueDetails.MaterialIssueDetailID, FirmOrders.Reference AS FirmOrderReference, FirmOrders.Code AS FirmOrderCode, FirmOrders.Specs AS FirmOrderSpecs, MaterialIssueDetails.EntryDate AS MaterialIssueEntryDate, MaterialIssueDetails.BatchID, MaterialIssueDetails.BatchEntryDate, MaterialIssueDetails.Barcode, MaterialIssueDetails.BatchCode, MaterialIssueDetails.SealCode, MaterialIssueDetails.LabCode, " + "\r\n";
            queryString = queryString + "                   Commodities.CommodityID, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, Commodities.CommodityTypeID, MaterialIssueDetails.LabID, MaterialIssueDetails.ProductionDate, MaterialIssueDetails.ExpiryDate, " + "\r\n";
            queryString = queryString + "                   ROUND(MaterialIssueDetails.Quantity - MaterialIssueDetails.QuantitySemifinished - MaterialIssueDetails.QuantityFailure - MaterialIssueDetails.QuantityReceipted + GoodsReceiptDetails.Quantity, " + (int)GlobalEnums.rndQuantity + ") AS QuantityRemains, " + "\r\n";
            queryString = queryString + "                   0.0 AS Quantity, MaterialIssueDetails.Remarks, CAST(0 AS bit) AS IsSelected, " + "\r\n";
            queryString = queryString + "                   Workshifts.Name AS WorkshiftName, Workshifts.EntryDate AS WorkshiftEntryDate, ProductionLines.Code AS ProductionLinesCode " + "\r\n";

            queryString = queryString + "       FROM        MaterialIssueDetails " + "\r\n";
            queryString = queryString + "                   INNER JOIN GoodsReceiptDetails ON GoodsReceiptDetails.GoodsReceiptID = @GoodsReceiptID AND MaterialIssueDetails.MaterialIssueDetailID = GoodsReceiptDetails.MaterialIssueDetailID" + (isMaterialIssueDetailIDs ? " AND MaterialIssueDetails.MaterialIssueDetailID NOT IN (SELECT Id FROM dbo.SplitToIntList (@MaterialIssueDetailIDs))" : "") + "\r\n";
            queryString = queryString + "                   INNER JOIN Commodities ON MaterialIssueDetails.CommodityID = Commodities.CommodityID " + "\r\n";
            queryString = queryString + "                   INNER JOIN FirmOrders ON MaterialIssueDetails.FirmOrderID = FirmOrders.FirmOrderID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Workshifts ON MaterialIssueDetails.WorkshiftID = Workshifts.WorkshiftID " + "\r\n";
            queryString = queryString + "                   INNER JOIN ProductionLines ON MaterialIssueDetails.ProductionLineID = ProductionLines.ProductionLineID " + "\r\n";

            return queryString;
        }
        #endregion MaterialIssue



        private void GoodsReceiptSaveRelative()
        {
            string queryString = " @EntityID int, @SaveRelativeOption int " + "\r\n"; //SaveRelativeOption: 1: Update, -1:Undo
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";
            queryString = queryString + "       BEGIN " + "\r\n";

            queryString = queryString + "           DECLARE @GoodsReceiptTypeID int, @OneStep bit, @AffectedROWCOUNT int ";
            queryString = queryString + "           SELECT  @GoodsReceiptTypeID = GoodsReceiptTypeID, @OneStep = OneStep FROM GoodsReceipts WHERE GoodsReceiptID = @EntityID ";


            queryString = queryString + "           IF (@SaveRelativeOption = 1) " + "\r\n";
            queryString = queryString + "               BEGIN " + "\r\n";

            queryString = queryString + "                   IF (@GoodsReceiptTypeID = " + (int)GlobalEnums.GoodsReceiptTypeID.WarehouseTransfer + " AND @OneStep = 1) " + "\r\n";
            queryString = queryString + "                       BEGIN  " + "\r\n";
            queryString = queryString + "                           UPDATE          GoodsReceipts " + "\r\n";
            queryString = queryString + "                           SET             GoodsReceipts.Reference = WarehouseTransfers.Reference " + "\r\n";
            queryString = queryString + "                           FROM            GoodsReceipts " + "\r\n";
            queryString = queryString + "                                           INNER JOIN WarehouseTransfers ON GoodsReceipts.GoodsReceiptID = @EntityID AND GoodsReceipts.WarehouseTransferID = WarehouseTransfers.WarehouseTransferID " + "\r\n";
            queryString = queryString + "                       END " + "\r\n";

            queryString = queryString + "                   IF (@GoodsReceiptTypeID = " + (int)GlobalEnums.GoodsReceiptTypeID.WarehouseAdjustments + ") " + "\r\n";
            queryString = queryString + "                       BEGIN  " + "\r\n";
            queryString = queryString + "                           UPDATE          GoodsReceipts " + "\r\n";
            queryString = queryString + "                           SET             GoodsReceipts.Reference = WarehouseAdjustments.Reference " + "\r\n";
            queryString = queryString + "                           FROM            GoodsReceipts " + "\r\n";
            queryString = queryString + "                                           INNER JOIN WarehouseAdjustments ON GoodsReceipts.GoodsReceiptID = @EntityID AND GoodsReceipts.WarehouseAdjustmentID = WarehouseAdjustments.WarehouseAdjustmentID " + "\r\n";
            queryString = queryString + "                       END  " + "\r\n";


            queryString = queryString + "                   UPDATE          GoodsReceiptDetails " + "\r\n";
            queryString = queryString + "                   SET             GoodsReceiptDetails.Reference = GoodsReceipts.Reference " + "\r\n";
            queryString = queryString + "                   FROM            GoodsReceipts INNER JOIN GoodsReceiptDetails ON GoodsReceipts.GoodsReceiptID = @EntityID AND GoodsReceipts.GoodsReceiptID = GoodsReceiptDetails.GoodsReceiptID " + "\r\n";

            #region UPDATE WorkshiftID
            queryString = queryString + "                   DECLARE         @EntryDate Datetime, @ShiftID int, @WorkshiftID int " + "\r\n";
            queryString = queryString + "                   SELECT          @EntryDate = CONVERT(date, EntryDate), @ShiftID = ShiftID FROM GoodsReceipts WHERE GoodsReceiptID = @EntityID " + "\r\n";
            queryString = queryString + "                   SET             @WorkshiftID = (SELECT TOP 1 WorkshiftID FROM Workshifts WHERE EntryDate = @EntryDate AND ShiftID = @ShiftID) " + "\r\n";

            queryString = queryString + "                   IF             (@WorkshiftID IS NULL) " + "\r\n";
            queryString = queryString + "                       BEGIN ";
            queryString = queryString + "                           INSERT INTO     Workshifts(EntryDate, ShiftID, Code, Name, WorkingHours, Remarks) SELECT @EntryDate, ShiftID, Code, Name, WorkingHours, Remarks FROM Shifts WHERE ShiftID = @ShiftID " + "\r\n";
            queryString = queryString + "                           SELECT          @WorkshiftID = SCOPE_IDENTITY(); " + "\r\n";
            queryString = queryString + "                       END ";

            queryString = queryString + "                   UPDATE          GoodsReceipts        SET WorkshiftID = @WorkshiftID WHERE GoodsReceiptID = @EntityID " + "\r\n";
            queryString = queryString + "                   UPDATE          GoodsReceiptDetails  SET WorkshiftID = @WorkshiftID WHERE GoodsReceiptID = @EntityID " + "\r\n";
            #endregion UPDATE WorkshiftID

            queryString = queryString + "               END " + "\r\n";



            queryString = queryString + "           IF (@GoodsReceiptTypeID > 0) " + "\r\n";
            queryString = queryString + "               BEGIN  " + "\r\n";

            queryString = queryString + "                   IF (@GoodsReceiptTypeID = " + (int)GlobalEnums.GoodsReceiptTypeID.PurchaseRequisition + ") " + "\r\n";
            queryString = queryString + "                       BEGIN  " + "\r\n";
            queryString = queryString + "                           UPDATE          PurchaseRequisitionDetails " + "\r\n";
            queryString = queryString + "                           SET             PurchaseRequisitionDetails.QuantityReceipted = ROUND(PurchaseRequisitionDetails.QuantityReceipted + GoodsReceiptDetails.Quantity * @SaveRelativeOption, " + (int)GlobalEnums.rndQuantity + ") " + "\r\n";
            queryString = queryString + "                           FROM            GoodsReceiptDetails " + "\r\n";
            queryString = queryString + "                                           INNER JOIN PurchaseRequisitionDetails ON ((PurchaseRequisitionDetails.Approved = 1 AND PurchaseRequisitionDetails.InActive = 0 AND PurchaseRequisitionDetails.InActivePartial = 0) OR @SaveRelativeOption = -1) AND GoodsReceiptDetails.GoodsReceiptID = @EntityID AND GoodsReceiptDetails.PurchaseRequisitionDetailID = PurchaseRequisitionDetails.PurchaseRequisitionDetailID " + "\r\n";
            queryString = queryString + "                           SET @AffectedROWCOUNT = @@ROWCOUNT " + "\r\n";
            queryString = queryString + "                       END " + "\r\n";

            queryString = queryString + "                   IF (@GoodsReceiptTypeID = " + (int)GlobalEnums.GoodsReceiptTypeID.GoodsArrival + ") " + "\r\n";
            queryString = queryString + "                       BEGIN  " + "\r\n";
            queryString = queryString + "                           UPDATE          GoodsArrivalDetails " + "\r\n"; //NOT CHECK @@ROWCOUNT ON THIS STATEMENT. JUST CHECK FOR THE NEXT STATEMENT ONLY (ON ONLY TABLE: GoodsArrivalPackages)
            queryString = queryString + "                           SET             GoodsArrivalDetails.QuantityReceipted = ROUND(GoodsArrivalDetails.QuantityReceipted + GoodsReceiptDetails.Quantity * @SaveRelativeOption, " + (int)GlobalEnums.rndQuantity + ") " + "\r\n";
            queryString = queryString + "                           FROM           (SELECT GoodsArrivalDetailID, SUM(Quantity) AS Quantity FROM GoodsReceiptDetails WHERE GoodsReceiptID = @EntityID GROUP BY GoodsArrivalDetailID) AS GoodsReceiptDetails " + "\r\n";
            queryString = queryString + "                                           INNER JOIN GoodsArrivalDetails ON GoodsReceiptDetails.GoodsArrivalDetailID = GoodsArrivalDetails.GoodsArrivalDetailID " + "\r\n";

            queryString = queryString + "                           UPDATE          GoodsArrivalPackages " + "\r\n";
            queryString = queryString + "                           SET             GoodsArrivalPackages.QuantityReceipted = ROUND(GoodsArrivalPackages.QuantityReceipted + GoodsReceiptDetails.Quantity * @SaveRelativeOption, " + (int)GlobalEnums.rndQuantity + ") " + "\r\n";
            queryString = queryString + "                           FROM            GoodsReceiptDetails " + "\r\n";
            queryString = queryString + "                                           INNER JOIN GoodsArrivalPackages ON ((GoodsArrivalPackages.Approved = 1 AND GoodsArrivalPackages.InActive = 0 AND GoodsArrivalPackages.InActivePartial = 0) OR @SaveRelativeOption = -1) AND GoodsReceiptDetails.GoodsReceiptID = @EntityID AND GoodsReceiptDetails.GoodsArrivalPackageID = GoodsArrivalPackages.GoodsArrivalPackageID " + "\r\n";
            queryString = queryString + "                           SET @AffectedROWCOUNT = @@ROWCOUNT " + "\r\n";
            queryString = queryString + "                       END " + "\r\n";

            queryString = queryString + "                   IF (@GoodsReceiptTypeID = " + (int)GlobalEnums.GoodsReceiptTypeID.WarehouseTransfer + ") " + "\r\n";
            queryString = queryString + "                       BEGIN  " + "\r\n";
            queryString = queryString + "                           UPDATE          WarehouseTransferDetails " + "\r\n";
            queryString = queryString + "                           SET             WarehouseTransferDetails.QuantityReceipted = ROUND(WarehouseTransferDetails.QuantityReceipted + GoodsReceiptDetails.Quantity * @SaveRelativeOption, " + (int)GlobalEnums.rndQuantity + ") " + "\r\n";
            queryString = queryString + "                           FROM            GoodsReceiptDetails " + "\r\n";
            queryString = queryString + "                                           INNER JOIN WarehouseTransferDetails ON (WarehouseTransferDetails.OneStep = 1 OR WarehouseTransferDetails.Approved = 1 OR @SaveRelativeOption = -1) AND GoodsReceiptDetails.GoodsReceiptID = @EntityID AND GoodsReceiptDetails.WarehouseTransferDetailID = WarehouseTransferDetails.WarehouseTransferDetailID " + "\r\n";
            queryString = queryString + "                           SET @AffectedROWCOUNT = @@ROWCOUNT " + "\r\n";
            queryString = queryString + "                       END " + "\r\n";

            queryString = queryString + "                   IF (@GoodsReceiptTypeID = " + (int)GlobalEnums.GoodsReceiptTypeID.FinishedProduct + ") " + "\r\n";
            queryString = queryString + "                       BEGIN  " + "\r\n";
            queryString = queryString + "                           UPDATE          FinishedProductPackages " + "\r\n";
            queryString = queryString + "                           SET             FinishedProductPackages.QuantityReceipted = ROUND(FinishedProductPackages.QuantityReceipted + GoodsReceiptDetails.Quantity * @SaveRelativeOption, " + (int)GlobalEnums.rndQuantity + ") " + "\r\n";
            queryString = queryString + "                           FROM            GoodsReceiptDetails " + "\r\n";
            queryString = queryString + "                                           INNER JOIN FinishedProductPackages ON ((FinishedProductPackages.Approved = 1 AND FinishedProductPackages.HandoverApproved = 1) OR @SaveRelativeOption = -1) AND GoodsReceiptDetails.GoodsReceiptID = @EntityID AND GoodsReceiptDetails.FinishedProductPackageID = FinishedProductPackages.FinishedProductPackageID " + "\r\n";
            queryString = queryString + "                           SET @AffectedROWCOUNT = @@ROWCOUNT " + "\r\n";
            queryString = queryString + "                       END " + "\r\n";

            queryString = queryString + "                   IF (@GoodsReceiptTypeID = " + (int)GlobalEnums.GoodsReceiptTypeID.FinishedItem + ") " + "\r\n";
            queryString = queryString + "                       BEGIN  " + "\r\n";
            queryString = queryString + "                           UPDATE          FinishedItemPackages " + "\r\n";
            queryString = queryString + "                           SET             FinishedItemPackages.QuantityReceipted = ROUND(FinishedItemPackages.QuantityReceipted + GoodsReceiptDetails.Quantity * @SaveRelativeOption, " + (int)GlobalEnums.rndQuantity + ") " + "\r\n";
            queryString = queryString + "                           FROM            GoodsReceiptDetails " + "\r\n";
            queryString = queryString + "                                           INNER JOIN FinishedItemPackages ON ((FinishedItemPackages.Approved = 1 AND FinishedItemPackages.HandoverApproved = 1) OR @SaveRelativeOption = -1) AND GoodsReceiptDetails.GoodsReceiptID = @EntityID AND GoodsReceiptDetails.FinishedItemPackageID = FinishedItemPackages.FinishedItemPackageID " + "\r\n";
            queryString = queryString + "                           SET @AffectedROWCOUNT = @@ROWCOUNT " + "\r\n";
            queryString = queryString + "                       END " + "\r\n";

            queryString = queryString + "                   IF (@GoodsReceiptTypeID = " + (int)GlobalEnums.GoodsReceiptTypeID.Recyclate + ") " + "\r\n";
            queryString = queryString + "                       BEGIN  " + "\r\n";
            queryString = queryString + "                           UPDATE          RecyclatePackages " + "\r\n";
            queryString = queryString + "                           SET             RecyclatePackages.QuantityReceipted = ROUND(RecyclatePackages.QuantityReceipted + GoodsReceiptDetails.Quantity * @SaveRelativeOption, " + (int)GlobalEnums.rndQuantity + ") " + "\r\n";
            queryString = queryString + "                           FROM            GoodsReceiptDetails " + "\r\n";
            queryString = queryString + "                                           INNER JOIN RecyclatePackages ON (RecyclatePackages.Approved = 1 OR @SaveRelativeOption = -1) AND GoodsReceiptDetails.GoodsReceiptID = @EntityID AND GoodsReceiptDetails.RecyclatePackageID = RecyclatePackages.RecyclatePackageID " + "\r\n";
            queryString = queryString + "                           SET @AffectedROWCOUNT = @@ROWCOUNT " + "\r\n";
            queryString = queryString + "                       END " + "\r\n";

            //queryString = queryString + "                   IF (@GoodsReceiptTypeID = " + (int)GlobalEnums.GoodsReceiptTypeID.GoodsIssueTransfer + ") " + "\r\n";
            //queryString = queryString + "                       BEGIN  " + "\r\n";
            //queryString = queryString + "                           UPDATE          GoodsIssueTransferDetails " + "\r\n";
            //queryString = queryString + "                           SET             GoodsIssueTransferDetails.QuantityReceipted = ROUND(GoodsIssueTransferDetails.QuantityReceipted + GoodsReceiptDetails.Quantity * @SaveRelativeOption, " + (int)GlobalEnums.rndQuantity + ") " + "\r\n";
            //queryString = queryString + "                           FROM            GoodsReceiptDetails " + "\r\n";
            //queryString = queryString + "                                           INNER JOIN GoodsIssueTransferDetails ON GoodsReceiptDetails.GoodsReceiptID = @EntityID AND GoodsIssueTransferDetails.Approved = 1 AND GoodsReceiptDetails.GoodsIssueTransferDetailID = GoodsIssueTransferDetails.GoodsIssueTransferDetailID " + "\r\n";
            //queryString = queryString + "                           SET @AffectedROWCOUNT = @@ROWCOUNT " + "\r\n";
            //queryString = queryString + "                       END " + "\r\n";


            queryString = queryString + "                   IF (@GoodsReceiptTypeID = " + (int)GlobalEnums.GoodsReceiptTypeID.MaterialIssue + ") " + "\r\n";
            queryString = queryString + "                       BEGIN  " + "\r\n";
            queryString = queryString + "                           UPDATE          MaterialIssueDetails " + "\r\n";
            queryString = queryString + "                           SET             MaterialIssueDetails.QuantityReceipted = ROUND(MaterialIssueDetails.QuantityReceipted + GoodsReceiptDetails.Quantity * @SaveRelativeOption, " + (int)GlobalEnums.rndQuantity + "), " + "\r\n";
            queryString = queryString + "                                           MaterialIssueDetails.QuantityLoss = IIF(@SaveRelativeOption = 1, ROUND(MaterialIssueDetails.Quantity - MaterialIssueDetails.QuantitySemifinished - MaterialIssueDetails.QuantityFailure - (MaterialIssueDetails.QuantityReceipted + GoodsReceiptDetails.Quantity), " + (int)GlobalEnums.rndQuantity + "), 0) " + "\r\n"; //UPDATE: SET QuantityLoss = Remains, UNDO: SET QuantityLoss = 0            
            queryString = queryString + "                           FROM            GoodsReceiptDetails " + "\r\n";
            queryString = queryString + "                                           INNER JOIN MaterialIssueDetails ON (MaterialIssueDetails.Approved = 1 OR @SaveRelativeOption = -1) AND GoodsReceiptDetails.GoodsReceiptID = @EntityID AND GoodsReceiptDetails.MaterialIssueDetailID = MaterialIssueDetails.MaterialIssueDetailID " + "\r\n";
            queryString = queryString + "                           SET @AffectedROWCOUNT = @@ROWCOUNT " + "\r\n";
            queryString = queryString + "                       END " + "\r\n";


            queryString = queryString + "                   IF (@GoodsReceiptTypeID = " + (int)GlobalEnums.GoodsReceiptTypeID.WarehouseAdjustments + ") " + "\r\n";
            queryString = queryString + "                       BEGIN  " + "\r\n";
            queryString = queryString + "                           UPDATE          WarehouseAdjustmentDetails " + "\r\n";
            queryString = queryString + "                           SET             WarehouseAdjustmentDetails.QuantityReceipted = ROUND(WarehouseAdjustmentDetails.QuantityReceipted + GoodsReceiptDetails.Quantity * @SaveRelativeOption, " + (int)GlobalEnums.rndQuantity + ") " + "\r\n";
            queryString = queryString + "                           FROM            GoodsReceiptDetails " + "\r\n";
            queryString = queryString + "                                           INNER JOIN WarehouseAdjustmentDetails ON GoodsReceiptDetails.GoodsReceiptID = @EntityID AND WarehouseAdjustmentDetails.Quantity > 0 AND GoodsReceiptDetails.WarehouseAdjustmentDetailID = WarehouseAdjustmentDetails.WarehouseAdjustmentDetailID " + "\r\n";
            queryString = queryString + "                           SET @AffectedROWCOUNT = @@ROWCOUNT " + "\r\n";
            //------------------------------------------------------SHOULD UPDATE GoodsReceiptID, GoodsReceiptDetailID BACK TO WarehouseAdjustmentDetails FOR GoodsReceipts OF WarehouseAdjustmentDetails? THE ANSWER: WE CAN DO IT HERE, BUT IT BREAK THE RELATIONSHIP (cyclic redundancy relationship: GoodsReceiptDetails => WarehouseAdjustmentDetails => THUS: WE SHOULD NOT MAKE ANOTHER RELATIONSHIP FROM WarehouseAdjustmentDetails => GoodsReceiptDetails ) => SO: SHOULD NOT!!!
            queryString = queryString + "                       END " + "\r\n";

            queryString = queryString + "                   IF @AffectedROWCOUNT <> (SELECT COUNT(*) FROM GoodsReceiptDetails WHERE GoodsReceiptID = @EntityID) " + "\r\n";
            queryString = queryString + "                       BEGIN " + "\r\n";
            queryString = queryString + "                           DECLARE     @msg NVARCHAR(300) = N'Phiếu giao hàng đã hủy, hoặc chưa duyệt; hoặc số lượng nhập kho không phù hợp' ; " + "\r\n";
            queryString = queryString + "                           THROW       61001,  @msg, 1; " + "\r\n";
            queryString = queryString + "                       END " + "\r\n";

            queryString = queryString + "               END  " + "\r\n";

            queryString = queryString + "       END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GoodsReceiptSaveRelative", queryString);
        }

        private void GoodsReceiptPostSaveValidate()
        {
            string[] queryArray = new string[15];

            queryArray[0] = " SELECT TOP 1 @FoundEntity = N'Ngày đặt hàng: ' + CAST(PurchaseRequisitions.EntryDate AS nvarchar) FROM GoodsReceiptDetails INNER JOIN PurchaseRequisitions ON GoodsReceiptDetails.GoodsReceiptID = @EntityID AND GoodsReceiptDetails.PurchaseRequisitionID = PurchaseRequisitions.PurchaseRequisitionID AND GoodsReceiptDetails.EntryDate < PurchaseRequisitions.EntryDate ";
            queryArray[1] = " SELECT TOP 1 @FoundEntity = N'Số lượng xuất vượt quá số lượng đặt hàng: ' + CAST(ROUND(Quantity - QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") AS nvarchar) FROM PurchaseRequisitionDetails WHERE (ROUND(Quantity - QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") < 0) ";

            queryArray[2] = " SELECT TOP 1 @FoundEntity = N'Ngày đặt hàng: ' + CAST(GoodsArrivals.EntryDate AS nvarchar) FROM GoodsReceiptDetails INNER JOIN GoodsArrivals ON GoodsReceiptDetails.GoodsReceiptID = @EntityID AND GoodsReceiptDetails.GoodsArrivalID = GoodsArrivals.GoodsArrivalID AND GoodsReceiptDetails.EntryDate < GoodsArrivals.EntryDate ";
            queryArray[3] = " SELECT TOP 1 @FoundEntity = N'Số lượng xuất vượt quá số lượng đặt hàng: ' + CAST(ROUND(Quantity - QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") AS nvarchar) FROM GoodsArrivalPackages WHERE (ROUND(Quantity - QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") < 0) ";
            queryArray[4] = " SELECT TOP 1 @FoundEntity = N'Số lượng xuất vượt quá số lượng đặt hàng: ' + CAST(ROUND(Quantity - QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") AS nvarchar) FROM GoodsArrivalDetails WHERE (ROUND(Quantity - QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") < 0) ";

            queryArray[5] = " SELECT TOP 1 @FoundEntity = N'Ngày đặt hàng: ' + CAST(FinishedProducts.EntryDate AS nvarchar) FROM GoodsReceiptDetails INNER JOIN FinishedProducts ON GoodsReceiptDetails.GoodsReceiptID = @EntityID AND GoodsReceiptDetails.FinishedProductID = FinishedProducts.FinishedProductID AND GoodsReceiptDetails.EntryDate < FinishedProducts.EntryDate ";
            queryArray[6] = " SELECT TOP 1 @FoundEntity = N'Số lượng xuất vượt quá số lượng đặt hàng: ' + CAST(ROUND(Quantity - QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") AS nvarchar) FROM FinishedProductPackages WHERE (ROUND(Quantity - QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") < 0) ";

            queryArray[7] = " SELECT TOP 1 @FoundEntity = N'Ngày đặt hàng: ' + CAST(FinishedItems.EntryDate AS nvarchar) FROM GoodsReceiptDetails INNER JOIN FinishedItems ON GoodsReceiptDetails.GoodsReceiptID = @EntityID AND GoodsReceiptDetails.FinishedItemID = FinishedItems.FinishedItemID AND GoodsReceiptDetails.EntryDate < FinishedItems.EntryDate ";
            queryArray[8] = " SELECT TOP 1 @FoundEntity = N'Số lượng xuất vượt quá số lượng đặt hàng: ' + CAST(ROUND(Quantity - QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") AS nvarchar) FROM FinishedItemPackages WHERE (ROUND(Quantity - QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") < 0) ";

            queryArray[9] = " SELECT TOP 1 @FoundEntity = N'Ngày đặt hàng: ' + CAST(MaterialIssues.EntryDate AS nvarchar) FROM GoodsReceiptDetails INNER JOIN MaterialIssues ON GoodsReceiptDetails.GoodsReceiptID = @EntityID AND GoodsReceiptDetails.MaterialIssueID = MaterialIssues.MaterialIssueID AND GoodsReceiptDetails.EntryDate < MaterialIssues.EntryDate ";
            queryArray[10] = " SELECT TOP 1 @FoundEntity = N'Số lượng NVL sử dụng vượt quá số lượng đã xuất kho cho sản xuất: ' + CAST(ROUND(Quantity - QuantitySemifinished - QuantityFailure - QuantityReceipted - QuantityLoss, " + (int)GlobalEnums.rndQuantity + ") AS nvarchar) FROM MaterialIssueDetails WHERE (ROUND(Quantity - QuantitySemifinished - QuantityFailure - QuantityReceipted - QuantityLoss, " + (int)GlobalEnums.rndQuantity + ") < 0) ";

            queryArray[11] = " SELECT TOP 1 @FoundEntity = N'Ngày đặt hàng: ' + CAST(WarehouseTransfers.EntryDate AS nvarchar) FROM GoodsReceiptDetails INNER JOIN WarehouseTransfers ON GoodsReceiptDetails.GoodsReceiptID = @EntityID AND GoodsReceiptDetails.WarehouseTransferID = WarehouseTransfers.WarehouseTransferID AND GoodsReceiptDetails.EntryDate < WarehouseTransfers.EntryDate ";
            queryArray[12] = " SELECT TOP 1 @FoundEntity = N'Số lượng xuất vượt quá số lượng đặt hàng: ' + CAST(ROUND(Quantity - QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") AS nvarchar) FROM WarehouseTransferDetails WHERE (ROUND(Quantity - QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") < 0) ";

            queryArray[13] = " SELECT TOP 1 @FoundEntity = N'Ngày đặt hàng: ' + CAST(Recyclates.EntryDate AS nvarchar) FROM GoodsReceiptDetails INNER JOIN Recyclates ON GoodsReceiptDetails.GoodsReceiptID = @EntityID AND GoodsReceiptDetails.RecyclateID = Recyclates.RecyclateID AND GoodsReceiptDetails.EntryDate < Recyclates.EntryDate ";
            queryArray[14] = " SELECT TOP 1 @FoundEntity = N'Số lượng xuất vượt quá số lượng đặt hàng: ' + CAST(ROUND(Quantity - QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") AS nvarchar) FROM RecyclatePackages WHERE (ROUND(Quantity - QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") < 0) ";

            this.totalSmartPortalEntities.CreateProcedureToCheckExisting("GoodsReceiptPostSaveValidate", queryArray);
        }




        private void GoodsReceiptApproved()
        {
            string[] queryArray = new string[1];

            queryArray[0] = " SELECT TOP 1 @FoundEntity = GoodsReceiptID FROM GoodsReceipts WHERE GoodsReceiptID = @EntityID AND Approved = 1";

            this.totalSmartPortalEntities.CreateProcedureToCheckExisting("GoodsReceiptApproved", queryArray);
        }


        private void GoodsReceiptEditable()
        {
            string[] queryArray = new string[4]; //IMPORTANT: THESE QUERIES SHOULD BE COPIED TO WarehouseAdjustmentEditable AND WarehouseTransferEditable (THE SAME: GoodsReceiptEditable, WarehouseAdjustmentEditable, WarehouseTransferEditable)

            queryArray[0] = " SELECT TOP 1 @FoundEntity = GoodsReceiptID FROM MaterialIssueDetails WHERE GoodsReceiptID = @EntityID ";
            queryArray[1] = " SELECT TOP 1 @FoundEntity = GoodsReceiptID FROM WarehouseTransferDetails WHERE GoodsReceiptID = @EntityID ";
            queryArray[2] = " SELECT TOP 1 @FoundEntity = GoodsReceiptID FROM WarehouseAdjustmentDetails WHERE GoodsReceiptID = @EntityID ";
            queryArray[3] = " SELECT TOP 1 @FoundEntity = GoodsReceiptID FROM PackageIssueDetails WHERE GoodsReceiptID = @EntityID ";

            this.totalSmartPortalEntities.CreateProcedureToCheckExisting("GoodsReceiptEditable", queryArray);
        }

        private void GoodsReceiptToggleApproved()
        {
            string queryString = " @EntityID int, @Approved bit " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "       UPDATE      GoodsReceipts  SET Approved = @Approved, ApprovedDate = GetDate() WHERE GoodsReceiptID = @EntityID AND Approved = ~@Approved" + "\r\n";

            queryString = queryString + "       IF @@ROWCOUNT = 1 " + "\r\n";
            queryString = queryString + "           BEGIN " + "\r\n";
            queryString = queryString + "               UPDATE          GoodsReceiptDetails  SET Approved = @Approved WHERE GoodsReceiptID = @EntityID ; " + "\r\n";
            queryString = queryString + "           END " + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           BEGIN " + "\r\n";
            queryString = queryString + "               DECLARE     @msg NVARCHAR(300) = N'Dữ liệu không tồn tại hoặc đã ' + iif(@Approved = 0, N'hủy', '')  + N' duyệt' ; " + "\r\n";
            queryString = queryString + "               THROW       61001,  @msg, 1; " + "\r\n";
            queryString = queryString + "           END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GoodsReceiptToggleApproved", queryString);
        }


        private void GoodsReceiptInitReference()
        {
            SimpleInitReference simpleInitReference = new GoodsReceiptInitReference("GoodsReceipts", "GoodsReceiptID", "Reference", ModelSettingManager.ReferenceLength, ModelSettingManager.ReferencePrefix(GlobalEnums.NmvnTaskID.GoodsReceipt));
            this.totalSmartPortalEntities.CreateTrigger("GoodsReceiptInitReference", simpleInitReference.CreateQuery());

            string queryString = " @GoodsReceiptID int " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";
            queryString = queryString + "       SELECT TOP 1 Reference FROM GoodsReceipts WHERE GoodsReceiptID = @GoodsReceiptID " + "\r\n";
            this.totalSmartPortalEntities.CreateStoredProcedure("GoodsReceiptGetReference", queryString);
        }




        private void GetGoodsReceiptID()
        {
            string queryString;

            queryString = " @GoodsArrivalID Int, @PlannedOrderID Int, @WarehouseTransferID Int, @WarehouseAdjustmentID Int " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "   IF (NOT @GoodsArrivalID IS NULL) " + "\r\n";
            queryString = queryString + "       SELECT TOP 1 GoodsReceiptID FROM GoodsReceipts WHERE GoodsArrivalID = @GoodsArrivalID " + "\r\n";
            queryString = queryString + "   ELSE " + "\r\n";
            queryString = queryString + "       IF (NOT @PlannedOrderID IS NULL) " + "\r\n";
            queryString = queryString + "           SELECT TOP 1 GoodsReceiptID FROM GoodsReceipts WHERE PlannedOrderID = @PlannedOrderID " + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           IF (NOT @WarehouseTransferID IS NULL) " + "\r\n";
            queryString = queryString + "               SELECT TOP 1 GoodsReceiptID FROM GoodsReceipts WHERE WarehouseTransferID = @WarehouseTransferID " + "\r\n";
            queryString = queryString + "           ELSE " + "\r\n";
            queryString = queryString + "               IF (NOT @WarehouseAdjustmentID IS NULL) " + "\r\n";
            queryString = queryString + "                   SELECT TOP 1 GoodsReceiptID FROM GoodsReceipts WHERE WarehouseAdjustmentID = @WarehouseAdjustmentID " + "\r\n";
            queryString = queryString + "               ELSE " + "\r\n";
            queryString = queryString + "                   SELECT NULL " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetGoodsReceiptID", queryString);
        }

        private void GetGoodsReceiptDetailAvailables()
        {
            string queryString = " @LocationID Int, @WarehouseID Int, @WarehouseReceiptID Int, @CommodityID Int, @CommodityIDs varchar(3999), @BatchID Int, @BlendingInstructionID Int, @Barcode nvarchar(60), @GoodsReceiptDetailIDs varchar(3999), @OnlyApproved bit, @OnlyIssuable bit " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "   BEGIN " + "\r\n";

            queryString = queryString + "       DECLARE @BlendingInstructionDetails TABLE (CommodityID int NOT NULL, Quantity decimal(18, 2) NOT NULL) " + "\r\n";
            queryString = queryString + "       IF  (NOT @BlendingInstructionID IS NULL) " + "\r\n";
            queryString = queryString + "           BEGIN " + "\r\n";
            queryString = queryString + "               INSERT INTO     @BlendingInstructionDetails (CommodityID, Quantity) " + "\r\n";
            queryString = queryString + "               SELECT          CommodityID,  ROUND(Quantity - QuantityIssued, " + (int)GlobalEnums.rndQuantity + ") AS Quantity FROM BlendingInstructionDetails WHERE Approved = 1 AND InActive = 0 AND InActivePartial = 0 AND (ROUND(Quantity - QuantityIssued, " + (int)GlobalEnums.rndQuantity + ") >= 1 OR (ROUND(Quantity - QuantityIssued, " + (int)GlobalEnums.rndQuantity + ") > 0 AND QuantityIssued = 0)) " + "\r\n";
            queryString = queryString + "               INSERT INTO     @BlendingInstructionDetails (CommodityID, Quantity) " + "\r\n";
            queryString = queryString + "               SELECT          CommodityID, -ROUND(Quantity - QuantityIssued, " + (int)GlobalEnums.rndQuantity + ") AS Quantity FROM GoodsReceiptDetails WHERE WarehouseID = @WarehouseReceiptID AND CommodityID IN (SELECT CommodityID FROM @BlendingInstructionDetails) AND ROUND(Quantity - QuantityIssued, " + (int)GlobalEnums.rndQuantity + ") > 0 " + "\r\n";
            queryString = queryString + "           END " + "\r\n";


            queryString = queryString + "       IF  (@WarehouseID <> 0) " + "\r\n";
            queryString = queryString + "           " + this.GetGoodsReceiptDetailAvailableSQL(true) + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           " + this.GetGoodsReceiptDetailAvailableSQL(false) + "\r\n";

            queryString = queryString + "   END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetGoodsReceiptDetailAvailables", queryString);
        }

        private string GetGoodsReceiptDetailAvailableSQL(bool isWarehouseID)
        {
            string queryString = "";
            queryString = queryString + "   BEGIN " + "\r\n";
            if (GlobalEnums.CBPP)
            {
                queryString = queryString + "       IF  (@WarehouseReceiptID = 6) " + "\r\n"; //@WarehouseReceiptID = 6: KPC
                queryString = queryString + "           " + this.GetGoodsReceiptDetailAvailableSQL(isWarehouseID, true) + "\r\n";
                queryString = queryString + "       ELSE " + "\r\n";
                queryString = queryString + "           " + this.GetGoodsReceiptDetailAvailableSQL(isWarehouseID, false) + "\r\n";
            }
            else
                queryString = queryString + "           " + this.GetGoodsReceiptDetailAvailableSQL(isWarehouseID, false) + "\r\n";
            queryString = queryString + "   END " + "\r\n";

            return queryString;
        }

        private string GetGoodsReceiptDetailAvailableSQL(bool isWarehouseID, bool isLabOK)
        {
            string queryString = "";
            queryString = queryString + "   BEGIN " + "\r\n";

            queryString = queryString + "       IF  (@CommodityID <> 0) " + "\r\n";
            queryString = queryString + "           " + this.GetGoodsReceiptDetailAvailableSQL(isWarehouseID, isLabOK, true, false) + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           BEGIN " + "\r\n";
            queryString = queryString + "               IF  (@CommodityIDs <> '' AND @CommodityIDs <> '0') " + "\r\n";
            queryString = queryString + "                   " + this.GetGoodsReceiptDetailAvailableSQL(isWarehouseID, isLabOK, false, true) + "\r\n";
            queryString = queryString + "               ELSE " + "\r\n";
            queryString = queryString + "                   " + this.GetGoodsReceiptDetailAvailableSQL(isWarehouseID, isLabOK, false, false) + "\r\n";
            queryString = queryString + "           END " + "\r\n";
            queryString = queryString + "   END " + "\r\n";

            return queryString;
        }

        private string GetGoodsReceiptDetailAvailableSQL(bool isWarehouseID, bool isLabOK, bool isCommodityID, bool isCommodityIDs)
        {
            string queryString = "";
            queryString = queryString + "   BEGIN " + "\r\n";
            queryString = queryString + "       IF  (NOT @BatchID IS NULL) " + "\r\n";
            queryString = queryString + "           " + this.GetGoodsReceiptDetailAvailableSQL(isWarehouseID, isLabOK, isCommodityID, isCommodityIDs, true) + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           " + this.GetGoodsReceiptDetailAvailableSQL(isWarehouseID, isLabOK, isCommodityID, isCommodityIDs, false) + "\r\n";
            queryString = queryString + "   END " + "\r\n";

            return queryString;
        }

        private string GetGoodsReceiptDetailAvailableSQL(bool isWarehouseID, bool isLabOK, bool isCommodityID, bool isCommodityIDs, bool isBatchID)
        {
            string queryString = "";
            queryString = queryString + "   BEGIN " + "\r\n";
            queryString = queryString + "       IF  (NOT @BlendingInstructionID IS NULL) " + "\r\n";
            queryString = queryString + "           " + this.GetGoodsReceiptDetailAvailableSQL(isWarehouseID, isLabOK, isCommodityID, isCommodityIDs, isBatchID, true) + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           " + this.GetGoodsReceiptDetailAvailableSQL(isWarehouseID, isLabOK, isCommodityID, isCommodityIDs, isBatchID, false) + "\r\n";
            queryString = queryString + "   END " + "\r\n";

            return queryString;
        }

        private string GetGoodsReceiptDetailAvailableSQL(bool isWarehouseID, bool isLabOK, bool isCommodityID, bool isCommodityIDs, bool isBatchID, bool isBlendingInstruction)
        {
            string queryString = "";
            queryString = queryString + "   BEGIN " + "\r\n";
            queryString = queryString + "       IF  (NOT @Barcode IS NULL AND @Barcode <> '' AND @Barcode <> '0') " + "\r\n";
            queryString = queryString + "           " + this.GetGoodsReceiptDetailAvailableSQL(isWarehouseID, isLabOK, isCommodityID, isCommodityIDs, isBatchID, isBlendingInstruction, true) + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           " + this.GetGoodsReceiptDetailAvailableSQL(isWarehouseID, isLabOK, isCommodityID, isCommodityIDs, isBatchID, isBlendingInstruction, false) + "\r\n";
            queryString = queryString + "   END " + "\r\n";

            return queryString;
        }

        private string GetGoodsReceiptDetailAvailableSQL(bool isWarehouseID, bool isLabOK, bool isCommodityID, bool isCommodityIDs, bool isBatchID, bool isBlendingInstruction, bool isBarcode)
        {
            string queryString = "";
            queryString = queryString + "   BEGIN " + "\r\n";
            queryString = queryString + "       IF  (@GoodsReceiptDetailIDs <> '' AND @GoodsReceiptDetailIDs <> '0') " + "\r\n";
            queryString = queryString + "           " + this.GetGoodsReceiptDetailAvailableSQL(isWarehouseID, isLabOK, isCommodityID, isCommodityIDs, isBatchID, isBlendingInstruction, isBarcode, true) + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           " + this.GetGoodsReceiptDetailAvailableSQL(isWarehouseID, isLabOK, isCommodityID, isCommodityIDs, isBatchID, isBlendingInstruction, isBarcode, false) + "\r\n";
            queryString = queryString + "   END " + "\r\n";

            return queryString;
        }

        private string GetGoodsReceiptDetailAvailableSQL(bool isWarehouseID, bool isLabOK, bool isCommodityID, bool isCommodityIDs, bool isBatchID, bool isBlendingInstruction, bool isBarcode, bool isGoodsReceiptDetailIDs)
        {
            string queryString = "";
            string queryBlendingInstruction = "";
            if (isBlendingInstruction) queryBlendingInstruction = " GoodsReceiptDetails.CommodityID IN (SELECT CommodityID FROM @BlendingInstructionDetails BlendingInstructionDetails GROUP BY CommodityID HAVING ROUND(SUM(Quantity), " + (int)GlobalEnums.rndQuantity + ") > 0) AND ";

            queryString = queryString + "   BEGIN " + "\r\n";

            queryString = queryString + "       SELECT      GoodsReceiptDetails.GoodsReceiptID, GoodsReceiptDetails.GoodsReceiptDetailID, GoodsReceiptDetails.Reference AS GoodsReceiptReference, GoodsReceiptDetails.Code AS GoodsReceiptCode, GoodsReceiptDetails.EntryDate AS GoodsReceiptEntryDate, GoodsReceiptDetails.BatchID, GoodsReceiptDetails.BatchEntryDate, GoodsReceiptDetails.WarehouseID, Warehouses.Code AS WarehouseCode, Commodities.CommodityID, Commodities.Code AS CommodityCode, Commodities.OfficialCode, Commodities.CodePartA, Commodities.CodePartB, Commodities.CodePartC, Commodities.CodePartD, Commodities.CodePartE, Commodities.CodePartF, Commodities.Name AS CommodityName, Commodities.CommodityTypeID, " + "\r\n";
            queryString = queryString + "                   ISNULL('Production: ' + ' ' + PurchaseRequisitions.Reference, ISNULL('Production: ' + ' ' + GoodsArrivals.Reference, ISNULL('From: ' + ' ' + WarehouseIssues.Name, ISNULL(WarehouseAdjustmentTypes.Name  + ' ' + WarehouseAdjustments.AdjustmentJobs, '')))) AS LineReferences, GoodsReceiptDetails.BinLocationID, BinLocations.Code AS BinLocationCode, GoodsReceiptDetails.Barcode, GoodsReceiptDetails.BatchCode, GoodsReceiptDetails.SealCode, GoodsReceiptDetails.LabCode, GoodsReceiptDetails.LabID, GoodsReceiptDetails.ProductionDate, GoodsReceiptDetails.ExpiryDate, GoodsReceiptDetails.Approved, GoodsReceiptDetails.Remarks, Warehouses.Issuable, " + "\r\n";
            queryString = queryString + "                   ROUND(GoodsReceiptDetails.Quantity - GoodsReceiptDetails.QuantityIssued, " + GlobalEnums.rndQuantity + ") AS QuantityAvailables, ISNULL(CAST(0 AS bit), CAST(0 AS bit)) AS IsSelected " + "\r\n";

            queryString = queryString + "       FROM        GoodsReceiptDetails " + "\r\n";
            if (isBarcode) queryString = queryString + "    INNER JOIN BinLocations ON " + queryBlendingInstruction + (isBarcode ? "GoodsReceiptDetails.Barcode = @Barcode AND " : "") + " GoodsReceiptDetails.BinLocationID = BinLocations.BinLocationID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Warehouses ON ROUND(GoodsReceiptDetails.Quantity - GoodsReceiptDetails.QuantityIssued, " + GlobalEnums.rndQuantity + ") > 0 " + (isWarehouseID ? " AND GoodsReceiptDetails.WarehouseID = @WarehouseID" : " AND GoodsReceiptDetails.LocationID = @LocationID") + (isLabOK ? " AND GoodsReceiptDetails.ExpiryDate >= CONVERT(DATE, GETDATE()) AND GoodsReceiptDetails.LabID IN (SELECT LabID FROM Labs WHERE Approved = 1 AND InActive = 0 AND Hold = 0)" : "") + (isCommodityID ? " AND GoodsReceiptDetails.CommodityID = @CommodityID" : "") + (isCommodityIDs ? " AND GoodsReceiptDetails.CommodityID IN (SELECT Id FROM dbo.SplitToIntList (@CommodityIDs))" : "") + " AND (@OnlyApproved = 0 OR GoodsReceiptDetails.Approved = 1) AND (@OnlyIssuable = 0 OR Warehouses.Issuable = 1) AND GoodsReceiptDetails.WarehouseID = Warehouses.WarehouseID " + (isBatchID ? " AND GoodsReceiptDetails.BatchID = @BatchID" : "") + (isGoodsReceiptDetailIDs ? " AND GoodsReceiptDetails.GoodsReceiptDetailID NOT IN (SELECT Id FROM dbo.SplitToIntList (@GoodsReceiptDetailIDs))" : "") + "\r\n";
            queryString = queryString + "                   INNER JOIN Commodities ON GoodsReceiptDetails.CommodityID = Commodities.CommodityID " + "\r\n";
            if (!isBarcode) queryString = queryString + "   INNER JOIN BinLocations ON " + queryBlendingInstruction + " GoodsReceiptDetails.BinLocationID = BinLocations.BinLocationID " + "\r\n";

            queryString = queryString + "                   LEFT JOIN PurchaseRequisitions ON GoodsReceiptDetails.PurchaseRequisitionID = PurchaseRequisitions.PurchaseRequisitionID " + "\r\n";
            queryString = queryString + "                   LEFT JOIN GoodsArrivals ON GoodsReceiptDetails.GoodsArrivalID = GoodsArrivals.GoodsArrivalID " + "\r\n";
            queryString = queryString + "                   LEFT JOIN Warehouses WarehouseIssues ON GoodsReceiptDetails.WarehouseIssueID = WarehouseIssues.WarehouseID " + "\r\n";
            queryString = queryString + "                   LEFT JOIN WarehouseAdjustments ON GoodsReceiptDetails.WarehouseAdjustmentID = WarehouseAdjustments.WarehouseAdjustmentID " + "\r\n";
            queryString = queryString + "                   LEFT JOIN WarehouseAdjustmentTypes ON WarehouseAdjustments.WarehouseAdjustmentTypeID = WarehouseAdjustmentTypes.WarehouseAdjustmentTypeID " + "\r\n";

            queryString = queryString + "       ORDER BY    GoodsReceiptDetails.ExpiryDate, GoodsReceiptDetails.BatchEntryDate, GoodsReceiptDetails.GoodsReceiptDetailID " + "\r\n";

            queryString = queryString + "   END " + "\r\n";

            return queryString;
        }


        private void GetGoodsReceiptBarcodeAvailables()
        {
            string queryString;

            string queryBUILD = "               SELECT      BarcodeAvailables.GoodsArrivalPackageID, BarcodeAvailables.GoodsReceiptDetailID, BarcodeAvailables.EntryDate, BarcodeAvailables.Reference, BarcodeAvailables.BatchEntryDate, BarcodeAvailables.ExpiryDate, Commodities.CommodityID, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, BarcodeAvailables.WarehouseID, Warehouses.Code AS WarehouseCode, BarcodeAvailables.BinLocationID, BinLocations.Code AS BinLocationCode, " + "\r\n";
            queryBUILD = queryBUILD + "                     BarcodeAvailables.BatchID, BarcodeAvailables.BatchCode, BarcodeAvailables.LabCode, BarcodeAvailables.Barcode, BarcodeAvailables.UnitWeight, BarcodeAvailables.TareWeight, ROUND(BarcodeAvailables.Quantity - BarcodeAvailables.QuantityIssued, " + (int)GlobalEnums.rndQuantity + ") AS QuantityAvailables, " + "\r\n";
            queryBUILD = queryBUILD + "                     BarcodeAvailables.Approved, Labs.Approved AS LabApproved, Labs.Hold AS LabHold, Labs.InActive AS LabInActive, VoidTypes.Code AS LabInActiveCode, CAST(IIF(BarcodeAvailables.ExpiryDate >= CONVERT(DATE, GETDATE()), 0, 1) AS Bit) AS BatchExpiried " + "\r\n";
            queryBUILD = queryBUILD + "         FROM        @BarcodeAvailables BarcodeAvailables " + "\r\n";
            queryBUILD = queryBUILD + "                     INNER JOIN Commodities ON BarcodeAvailables.CommodityID = Commodities.CommodityID " + "\r\n";
            queryBUILD = queryBUILD + "                     LEFT  JOIN Warehouses ON BarcodeAvailables.WarehouseID = Warehouses.WarehouseID " + "\r\n";
            queryBUILD = queryBUILD + "                     LEFT  JOIN BinLocations ON BarcodeAvailables.BinLocationID = BinLocations.BinLocationID " + "\r\n";
            queryBUILD = queryBUILD + "                     LEFT  JOIN Labs ON BarcodeAvailables.LabID = Labs.LabID " + "\r\n";
            queryBUILD = queryBUILD + "                     LEFT  JOIN VoidTypes ON Labs.VoidTypeID = VoidTypes.VoidTypeID " + "\r\n";
            
            
            queryString = " @Barcode nvarchar(60) " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "       DECLARE     @BarcodeAvailables TABLE (GoodsArrivalPackageID int NULL, GoodsReceiptDetailID int NULL, EntryID int NOT NULL, EntryDate datetime NOT NULL, Reference nvarchar(50) NULL, BatchEntryDate datetime NULL, ExpiryDate datetime NULL, BinLocationID int NULL, CommodityID int NOT NULL, WarehouseID int NULL, LabID int NOT NULL, BatchID int NULL, BatchCode nvarchar(60) NULL, LabCode nvarchar(60) NULL, Barcode nvarchar(60) NULL, UnitWeight decimal(18, 2) NOT NULL, TareWeight decimal(18, 2) NOT NULL, Quantity decimal(18, 2) NOT NULL, QuantityIssued decimal(18, 2) NOT NULL, Approved bit NOT NULL) " + "\r\n";

            queryString = queryString + "       INSERT INTO @BarcodeAvailables (GoodsArrivalPackageID, GoodsReceiptDetailID, EntryID, EntryDate, Reference, BatchEntryDate, ExpiryDate, BinLocationID, CommodityID, WarehouseID, LabID, BatchID, BatchCode, LabCode, Barcode, UnitWeight, TareWeight, Quantity, QuantityIssued, Approved) " + "\r\n";
            queryString = queryString + "       SELECT      NULL AS GoodsArrivalPackageID, GoodsReceiptDetailID, GoodsReceiptDetailID AS EntryID, EntryDate, Reference, BatchEntryDate, ExpiryDate, BinLocationID, CommodityID, WarehouseID, LabID, BatchID, BatchCode, LabCode, Barcode, UnitWeight, TareWeight, Quantity, QuantityIssued, Approved " + "\r\n";
            queryString = queryString + "       FROM        GoodsReceiptDetails WHERE Barcode = @Barcode " + "\r\n";

            queryString = queryString + "       IF (@@ROWCOUNT = 0) " + "\r\n";
            queryString = queryString + "           INSERT INTO @BarcodeAvailables (GoodsArrivalPackageID, GoodsReceiptDetailID, EntryID, EntryDate, Reference, BatchEntryDate, ExpiryDate, BinLocationID, CommodityID, WarehouseID, LabID, BatchID, BatchCode, LabCode, Barcode, UnitWeight, TareWeight, Quantity, QuantityIssued, Approved) " + "\r\n";
            queryString = queryString + "           SELECT      GoodsArrivalPackageID, NULL AS GoodsReceiptDetailID, GoodsArrivalPackageID AS EntryID, EntryDate, Code AS Reference, BatchEntryDate, ExpiryDate, NULL AS BinLocationID, CommodityID, WarehouseID, LabID, BatchID, BatchCode, LabCode, Barcode, UnitWeight, TareWeight, Quantity, QuantityReceipted AS QuantityIssued, Approved " + "\r\n";
            queryString = queryString + "           FROM        GoodsArrivalPackages WHERE Barcode = @Barcode " + "\r\n";

            queryString = queryString + "       IF ((SELECT COUNT(*) FROM @BarcodeAvailables WHERE ROUND(Quantity - QuantityIssued, " + (int)GlobalEnums.rndQuantity + ") > 0) > 0) " + "\r\n";
            queryString = queryString + "           BEGIN " + "\r\n";
            queryString = queryString + "               " + queryBUILD + "\r\n";
            queryString = queryString + "               WHERE       ROUND(BarcodeAvailables.Quantity - BarcodeAvailables.QuantityIssued, " + (int)GlobalEnums.rndQuantity + ") > 0 " + "\r\n";
            queryString = queryString + "           END " + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           BEGIN " + "\r\n";
            queryString = queryString + "               " + queryBUILD + "\r\n";
            queryString = queryString + "               WHERE       BarcodeAvailables.EntryID = (SELECT MAX(EntryID) FROM @BarcodeAvailables) " + "\r\n";
            queryString = queryString + "           END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetGoodsReceiptBarcodeAvailables", queryString);
        }

        #region Generate Pending

        private class GenerateSQLPendingDetails
        {
            private readonly GlobalEnums.GoodsReceiptTypeID goodsReceiptTypeID;

            private readonly string primaryTables;
            private readonly string primaryTableDetails;

            private readonly string primaryKey;
            private readonly string paraPrimaryKey;

            private readonly string primaryKeyDetail;
            private readonly string paraPrimaryKeyDetails;

            private readonly string fieldNameWarehouseID;

            private readonly string primaryReference;
            private readonly string primaryEntryDate;

            private readonly TotalSmartPortalEntities totalSmartPortalEntities;

            public GenerateSQLPendingDetails(TotalSmartPortalEntities totalSmartPortalEntities, GlobalEnums.GoodsReceiptTypeID goodsReceiptTypeID, string primaryTables, string primaryTableDetails, string primaryKey, string paraPrimaryKey, string primaryKeyDetails, string paraPrimaryKeyDetails, string fieldNameWarehouseID, string primaryReference, string primaryEntryDate)
            {
                this.totalSmartPortalEntities = totalSmartPortalEntities;

                this.goodsReceiptTypeID = goodsReceiptTypeID;

                this.primaryTables = primaryTables;
                this.primaryTableDetails = primaryTableDetails;

                this.primaryKey = primaryKey;
                this.paraPrimaryKey = paraPrimaryKey;

                this.primaryKeyDetail = primaryKeyDetails;
                this.paraPrimaryKeyDetails = paraPrimaryKeyDetails;

                this.fieldNameWarehouseID = fieldNameWarehouseID;

                this.primaryReference = primaryReference;
                this.primaryEntryDate = primaryEntryDate;
            }





            public void GetPendingPickups(string myfunctionName)
            {
                string queryString = " @LocationID int " + "\r\n";
                //queryString = queryString + " WITH ENCRYPTION " + "\r\n";
                queryString = queryString + " AS " + "\r\n";

                queryString = queryString + "       SELECT          " + this.primaryTables + "." + this.primaryKey + ", " + this.primaryTables + ".Reference AS " + this.primaryReference + ", " + this.primaryTables + ".EntryDate AS " + this.primaryEntryDate + ", Warehouses.WarehouseID, Warehouses.Code AS WarehouseCode, Warehouses.Name AS WarehouseName, " + (int)this.goodsReceiptTypeID + " AS GoodsReceiptTypeID, (SELECT TOP 1 Name FROM GoodsReceiptTypes WHERE GoodsReceiptTypeID = " + (int)this.goodsReceiptTypeID + ") AS GoodsReceiptTypeName, " + this.primaryTables + ".Description, " + this.primaryTables + ".Remarks " + (this.goodsReceiptTypeID == GlobalEnums.GoodsReceiptTypeID.GoodsIssueTransfer ? ", GoodsIssues.VoucherCodes, SourceWarehouses.Name AS SourceWarehouseName " : "") + "\r\n";
                queryString = queryString + "       FROM           (SELECT " + this.primaryKey + ", " + fieldNameWarehouseID + (this.goodsReceiptTypeID == GlobalEnums.GoodsReceiptTypeID.GoodsIssueTransfer && fieldNameWarehouseID.IndexOf("WarehouseID") == -1 ? ", WarehouseID" : "") + ", Reference, EntryDate, Description, Remarks" + (this.goodsReceiptTypeID == GlobalEnums.GoodsReceiptTypeID.GoodsIssueTransfer ? ", VoucherCodes" : "") + " FROM " + this.primaryTables + " WHERE " + this.primaryKey + " IN (SELECT " + this.primaryKey + " FROM " + this.primaryTableDetails + " WHERE LocationID = @LocationID AND Approved = 1 AND ROUND(Quantity - QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") > 0)) AS " + this.primaryTables + "\r\n";
                queryString = queryString + "                       INNER JOIN Warehouses ON " + this.primaryTables + "." + fieldNameWarehouseID + " = Warehouses.WarehouseID " + "\r\n";
                if (this.goodsReceiptTypeID == GlobalEnums.GoodsReceiptTypeID.GoodsIssueTransfer)
                    queryString = queryString + "                   INNER JOIN Warehouses SourceWarehouses ON GoodsIssues.WarehouseID = SourceWarehouses.WarehouseID " + "\r\n";

                this.totalSmartPortalEntities.CreateStoredProcedure(myfunctionName, queryString);
            }

            public void GetPendingPickupWarehouses(string myfunctionName)
            {
                string queryString = " @LocationID int " + "\r\n";
                //queryString = queryString + " WITH ENCRYPTION " + "\r\n";
                queryString = queryString + " AS " + "\r\n";

                queryString = queryString + "       SELECT          Warehouses.WarehouseID, Warehouses.Code AS WarehouseCode, Warehouses.Name AS WarehouseName, " + (int)this.goodsReceiptTypeID + " AS GoodsReceiptTypeID, (SELECT TOP 1 Name FROM GoodsReceiptTypes WHERE GoodsReceiptTypeID = " + (int)this.goodsReceiptTypeID + ") AS GoodsReceiptTypeName " + "\r\n";
                queryString = queryString + "       FROM           (SELECT DISTINCT " + fieldNameWarehouseID + " FROM " + this.primaryTableDetails + " WHERE LocationID = @LocationID AND Approved = 1 AND ROUND(Quantity - QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") > 0) WarehousePENDING " + "\r\n";
                queryString = queryString + "                       INNER JOIN Warehouses ON WarehousePENDING." + fieldNameWarehouseID + " = Warehouses.WarehouseID " + "\r\n";

                this.totalSmartPortalEntities.CreateStoredProcedure(myfunctionName, queryString);
            }




















            public void GetPendingPickupDetails(string myfunctionName)
            {
                string queryString;

                queryString = " @LocationID Int, @GoodsReceiptID Int, " + this.paraPrimaryKey + " Int, @WarehouseID Int, " + this.paraPrimaryKeyDetails + " varchar(3999), @IsReadonly bit " + "\r\n";
                //queryString = queryString + " WITH ENCRYPTION " + "\r\n";
                queryString = queryString + " AS " + "\r\n";

                queryString = queryString + "   BEGIN " + "\r\n";

                queryString = queryString + "       IF  (" + this.paraPrimaryKey + " <> 0) " + "\r\n";
                queryString = queryString + "           " + this.BuildSQLPickup(true) + "\r\n";
                queryString = queryString + "       ELSE " + "\r\n";
                queryString = queryString + "           " + this.BuildSQLPickup(false) + "\r\n";

                queryString = queryString + "   END " + "\r\n";

                this.totalSmartPortalEntities.CreateStoredProcedure(myfunctionName, queryString);
            }

            private string BuildSQLPickup(bool isPickupID)
            {
                string queryString = "";
                queryString = queryString + "   BEGIN " + "\r\n";
                queryString = queryString + "       IF  (" + this.paraPrimaryKeyDetails + " <> '') " + "\r\n";
                queryString = queryString + "           " + this.BuildSQLPickupPickupDetailIDs(isPickupID, true) + "\r\n";
                queryString = queryString + "       ELSE " + "\r\n";
                queryString = queryString + "           " + this.BuildSQLPickupPickupDetailIDs(isPickupID, false) + "\r\n";
                queryString = queryString + "   END " + "\r\n";

                return queryString;
            }

            private string BuildSQLPickupPickupDetailIDs(bool isPickupID, bool isPickupDetailIDs)
            {
                string queryString = "";
                queryString = queryString + "   BEGIN " + "\r\n";

                queryString = queryString + "       IF (@GoodsReceiptID <= 0) " + "\r\n";
                queryString = queryString + "               BEGIN " + "\r\n";
                queryString = queryString + "                   " + this.BuildSQLNew(isPickupID, isPickupDetailIDs) + "\r\n";
                queryString = queryString + "                   ORDER BY " + this.primaryTableDetails + ".EntryDate, " + this.primaryTableDetails + "." + this.primaryKey + ", " + this.primaryTableDetails + "." + this.primaryKeyDetail + " " + "\r\n";
                queryString = queryString + "               END " + "\r\n";
                queryString = queryString + "       ELSE " + "\r\n";

                queryString = queryString + "               IF (@IsReadonly = 1) " + "\r\n";
                queryString = queryString + "                   BEGIN " + "\r\n";
                queryString = queryString + "                       " + this.BuildSQLEdit(isPickupID, isPickupDetailIDs) + "\r\n";
                queryString = queryString + "                       ORDER BY " + this.primaryTableDetails + ".EntryDate, " + this.primaryTableDetails + "." + this.primaryKey + ", " + this.primaryTableDetails + "." + this.primaryKeyDetail + " " + "\r\n";
                queryString = queryString + "                   END " + "\r\n";

                queryString = queryString + "               ELSE " + "\r\n"; //FULL SELECT FOR EDIT MODE

                queryString = queryString + "                   BEGIN " + "\r\n";
                queryString = queryString + "                       " + this.BuildSQLNew(isPickupID, isPickupDetailIDs) + " WHERE " + this.primaryTableDetails + "." + this.primaryKeyDetail + " NOT IN (SELECT " + this.primaryKeyDetail + " FROM GoodsReceiptDetails WHERE GoodsReceiptID = @GoodsReceiptID) " + "\r\n";
                queryString = queryString + "                       UNION ALL " + "\r\n";
                queryString = queryString + "                       " + this.BuildSQLEdit(isPickupID, isPickupDetailIDs) + "\r\n";
                queryString = queryString + "                       ORDER BY " + this.primaryTableDetails + ".EntryDate, " + this.primaryTableDetails + "." + this.primaryKey + ", " + this.primaryTableDetails + "." + this.primaryKeyDetail + " " + "\r\n";
                queryString = queryString + "                   END " + "\r\n";

                queryString = queryString + "   END " + "\r\n";

                return queryString;
            }

            private string BuildSQLNew(bool isPickupID, bool isPickupDetailIDs)
            {
                string queryString = "";

                queryString = queryString + "       SELECT      " + this.primaryTableDetails + "." + this.primaryKey + ", " + this.primaryTableDetails + "." + this.primaryKeyDetail + ", " + this.primaryTableDetails + ".Reference " + (this.goodsReceiptTypeID == GlobalEnums.GoodsReceiptTypeID.GoodsIssueTransfer ? " + ' [' + GoodsIssues.VoucherCodes + ']' " : "") + " AS " + this.primaryReference + ", " + this.primaryTableDetails + ".EntryDate AS " + this.primaryEntryDate + ", " + "\r\n";
                queryString = queryString + "                   " + (this.goodsReceiptTypeID == GlobalEnums.GoodsReceiptTypeID.GoodsIssueTransfer ? this.primaryTableDetails + ".LocationIssueID + 0" : "NULL") + " AS LocationIssueID, " + (this.goodsReceiptTypeID == GlobalEnums.GoodsReceiptTypeID.GoodsIssueTransfer ? this.primaryTableDetails + ".WarehouseID + 0" : "NULL") + " AS WarehouseIssueID, " + (this.goodsReceiptTypeID == GlobalEnums.GoodsReceiptTypeID.WarehouseAdjustments ? this.primaryTableDetails + ".WarehouseAdjustmentTypeID + 0" : "NULL") + " AS WarehouseAdjustmentTypeID, " + "\r\n";
                queryString = queryString + "                   Commodities.CommodityID, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, " + this.primaryTableDetails + ".BatchID, " + this.primaryTableDetails + ".BatchEntryDate, " + "\r\n";
                queryString = queryString + "                   ROUND(" + this.primaryTableDetails + ".Quantity - " + this.primaryTableDetails + ".QuantityReceipted,  " + (int)GlobalEnums.rndQuantity + ") AS QuantityRemains, CAST(0 AS decimal(18, 2)) AS Quantity, " + this.primaryTableDetails + ".Remarks, CAST(" + (this.goodsReceiptTypeID == GlobalEnums.GoodsReceiptTypeID.Pickup ? "1" : "0") + " AS bit) AS IsSelected " + "\r\n";

                queryString = queryString + "       FROM        " + this.primaryTableDetails + " " + "\r\n";
                queryString = queryString + "                   INNER JOIN Commodities ON " + (isPickupID ? " " + this.primaryTableDetails + "." + this.primaryKey + " = " + this.paraPrimaryKey + " " : "" + this.primaryTableDetails + ".LocationID = @LocationID AND " + this.primaryTableDetails + "." + fieldNameWarehouseID + " = @WarehouseID ") + " AND " + this.primaryTableDetails + (this.primaryTableDetails == "WarehouseAdjustmentDetails" ? ".Quantity > 0" : ".Approved = 1") + " AND ROUND(" + this.primaryTableDetails + ".Quantity - " + this.primaryTableDetails + ".QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") > 0 AND " + this.primaryTableDetails + ".CommodityID = Commodities.CommodityID " + (isPickupDetailIDs ? " AND " + this.primaryTableDetails + "." + this.primaryKeyDetail + " NOT IN (SELECT Id FROM dbo.SplitToIntList (" + this.paraPrimaryKeyDetails + "))" : "") + "\r\n";

                if (this.goodsReceiptTypeID == GlobalEnums.GoodsReceiptTypeID.GoodsIssueTransfer)
                    queryString = queryString + "               INNER JOIN GoodsIssues ON GoodsIssueTransferDetails.GoodsIssueID = GoodsIssues.GoodsIssueID " + "\r\n";

                return queryString;
            }

            private string BuildSQLEdit(bool isPickupID, bool isPickupDetailIDs)
            {
                string queryString = "";

                queryString = queryString + "       SELECT      " + this.primaryTableDetails + "." + this.primaryKey + ", " + this.primaryTableDetails + "." + this.primaryKeyDetail + ", " + this.primaryTableDetails + ".Reference " + (this.goodsReceiptTypeID == GlobalEnums.GoodsReceiptTypeID.GoodsIssueTransfer ? " + ' [' + GoodsIssues.VoucherCodes + ']' " : "") + " AS " + this.primaryReference + ", " + this.primaryTableDetails + ".EntryDate AS " + this.primaryEntryDate + ", " + "\r\n";
                queryString = queryString + "                   " + (this.goodsReceiptTypeID == GlobalEnums.GoodsReceiptTypeID.GoodsIssueTransfer ? this.primaryTableDetails + ".LocationIssueID + 0" : "NULL") + " AS LocationIssueID, " + (this.goodsReceiptTypeID == GlobalEnums.GoodsReceiptTypeID.GoodsIssueTransfer ? this.primaryTableDetails + ".WarehouseID + 0" : "NULL") + " AS WarehouseIssueID, " + (this.goodsReceiptTypeID == GlobalEnums.GoodsReceiptTypeID.WarehouseAdjustments ? this.primaryTableDetails + ".WarehouseAdjustmentTypeID + 0" : "NULL") + " AS WarehouseAdjustmentTypeID, " + "\r\n";
                queryString = queryString + "                   Commodities.CommodityID, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, " + this.primaryTableDetails + ".BatchID, " + this.primaryTableDetails + ".BatchEntryDate, " + "\r\n";
                queryString = queryString + "                   ROUND(" + this.primaryTableDetails + ".Quantity - " + this.primaryTableDetails + ".QuantityReceipted + GoodsReceiptDetails.Quantity,  " + (int)GlobalEnums.rndQuantity + ") AS QuantityRemains, CAST(0 AS decimal(18, 2)) AS Quantity, " + this.primaryTableDetails + ".Remarks, CAST(" + (this.goodsReceiptTypeID == GlobalEnums.GoodsReceiptTypeID.Pickup ? "1" : "0") + " AS bit) AS IsSelected " + "\r\n";

                queryString = queryString + "       FROM        " + this.primaryTableDetails + " " + "\r\n";
                queryString = queryString + "                   INNER JOIN GoodsReceiptDetails ON GoodsReceiptDetails.GoodsReceiptID = @GoodsReceiptID AND " + this.primaryTableDetails + "." + this.primaryKeyDetail + " = GoodsReceiptDetails." + this.primaryKeyDetail + "" + (isPickupDetailIDs ? " AND " + this.primaryTableDetails + "." + this.primaryKeyDetail + " NOT IN (SELECT Id FROM dbo.SplitToIntList (" + this.paraPrimaryKeyDetails + "))" : "") + "\r\n";

                if (this.goodsReceiptTypeID == GlobalEnums.GoodsReceiptTypeID.GoodsIssueTransfer)
                    queryString = queryString + "               INNER JOIN GoodsIssues ON GoodsIssueTransferDetails.GoodsIssueID = GoodsIssues.GoodsIssueID " + "\r\n";

                queryString = queryString + "                   INNER JOIN Commodities ON GoodsReceiptDetails.CommodityID = Commodities.CommodityID " + "\r\n";

                return queryString;
            }
        }
        #endregion Generate Pending
    }
}
