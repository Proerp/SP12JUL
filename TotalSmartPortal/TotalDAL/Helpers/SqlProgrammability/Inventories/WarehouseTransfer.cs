﻿using System;
using System.Text;

using TotalBase;
using TotalBase.Enums;
using TotalModel.Models;

namespace TotalDAL.Helpers.SqlProgrammability.Inventories
{
    public class WarehouseTransfer
    {
        private readonly TotalSmartPortalEntities totalSmartPortalEntities;

        public WarehouseTransfer(TotalSmartPortalEntities totalSmartPortalEntities)
        {
            this.totalSmartPortalEntities = totalSmartPortalEntities;
        }

        public void RestoreProcedure()
        {
            this.GetWarehouseTransferIndexes();

            this.GetWarehouseTransferViewDetails();

            this.GetWarehouseTransferAvailableWarehouses();
            this.GetWarehouseTransferPendingWarehouses();
            this.GetWarehouseTransferPendingTransferOrders();
            this.GetWarehouseTransferPendingTransferOrderDetails();

            this.WarehouseTransferSaveRelative();
            this.WarehouseTransferPostSaveValidate();

            this.WarehouseTransferApproved();
            this.WarehouseTransferEditable();

            this.WarehouseTransferToggleApproved();

            this.WarehouseTransferInitReference();

            this.WarehouseTransferSheet();
        }


        private void GetWarehouseTransferIndexes()
        {
            string queryString;

            queryString = " @NMVNTaskID int, @ModuleDetailID int, @AspUserID nvarchar(128), @FromDate DateTime, @ToDate DateTime " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";
            queryString = queryString + "    BEGIN " + "\r\n";

            queryString = queryString + "       SELECT      WarehouseTransfers.WarehouseTransferID, CAST(WarehouseTransfers.EntryDate AS DATE) AS EntryDate, WarehouseTransfers.Reference, Locations.Code AS LocationCode, Workshifts.Name AS WorkshiftName, Users.FirstName AS UserFirstName, Users.LastName AS UserLastName, WarehouseTransfers.WarehouseTransferJobs, WarehouseTransfers.Caption, WarehouseTransfers.Description, WarehouseTransfers.TotalRows, WarehouseTransfers.TotalQuantity, WarehouseTransfers.Approved, " + "\r\n";
            queryString = queryString + "                   Warehouses.Code AS WarehouseCode, WarehouseReceipts.Code AS WarehouseReceiptCode, TransferOrders.Reference AS TransferOrdersReference, TransferOrders.EntryDate AS TransferOrderEntryDate " + "\r\n";
            queryString = queryString + "       FROM        WarehouseTransfers " + "\r\n";
            queryString = queryString + "                   INNER JOIN Locations ON WarehouseTransfers.NMVNTaskID = @NMVNTaskID AND WarehouseTransfers.ModuleDetailID = @ModuleDetailID AND WarehouseTransfers.EntryDate >= @FromDate AND WarehouseTransfers.EntryDate <= @ToDate AND WarehouseTransfers.OrganizationalUnitID IN (SELECT AccessControls.OrganizationalUnitID FROM AccessControls INNER JOIN AspNetUsers ON AccessControls.UserID = AspNetUsers.UserID WHERE AspNetUsers.Id = @AspUserID AND AccessControls.NMVNTaskID = @NMVNTaskID AND AccessControls.AccessLevel > 0) AND Locations.LocationID = WarehouseTransfers.LocationID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Warehouses ON WarehouseTransfers.WarehouseID = Warehouses.WarehouseID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Warehouses WarehouseReceipts ON WarehouseTransfers.WarehouseReceiptID = WarehouseReceipts.WarehouseID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Workshifts ON WarehouseTransfers.WorkshiftID = Workshifts.WorkshiftID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Users ON WarehouseTransfers.UserID = Users.UserID " + "\r\n";
            queryString = queryString + "                   LEFT JOIN TransferOrders ON WarehouseTransfers.TransferOrderID = TransferOrders.TransferOrderID " + "\r\n";

            queryString = queryString + "    END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetWarehouseTransferIndexes", queryString);
        }


        private void GetWarehouseTransferViewDetails()
        {
            string queryString;

            queryString = " @WarehouseTransferID Int " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";
            queryString = queryString + "    BEGIN " + "\r\n";

            queryString = queryString + "       SELECT      WarehouseTransferDetails.WarehouseTransferDetailID, WarehouseTransferDetails.WarehouseTransferID, TransferOrderDetails.TransferOrderID, TransferOrderDetails.TransferOrderDetailID, Commodities.CommodityID, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, Commodities.CommodityTypeID, " + "\r\n";
            queryString = queryString + "                   GoodsReceiptDetails.GoodsReceiptID, GoodsReceiptDetails.GoodsReceiptDetailID, GoodsReceiptDetails.Reference AS GoodsReceiptReference, GoodsReceiptDetails.Code AS GoodsReceiptCode, GoodsReceiptDetails.EntryDate AS GoodsReceiptEntryDate, GoodsReceiptDetails.BatchID, GoodsReceiptDetails.BatchEntryDate, GoodsReceiptDetails.LabID, GoodsReceiptDetails.ProductionDate, GoodsReceiptDetails.ExpiryDate, GoodsReceiptDetails.Barcode, GoodsReceiptDetails.BatchCode, GoodsReceiptDetails.SealCode, GoodsReceiptDetails.LabCode, GoodsReceiptDetails.BinLocationID AS BinLocationIssuedID, BinLocationIssues.Code AS BinLocationIssuedCode, WarehouseTransferDetails.BinLocationID, BinLocations.Code AS BinLocationCode, " + "\r\n";
            queryString = queryString + "                   ISNULL(TransferOrderDetails.Quantity, 0) AS QuantityTO, ISNULL(ROUND(ISNULL(TransferOrderDetails.Quantity, 0) - ISNULL(TransferOrderDetails.QuantityIssued, 0) + ISNULL(Issued_TransferOrderDetails.QuantityIssued, 0), " + (int)GlobalEnums.rndQuantity + "), 0.0) AS TransferOrderRemains, ROUND(GoodsReceiptDetails.Quantity - GoodsReceiptDetails.QuantityIssued + ISNULL(Issued_GoodsReceiptDetails.QuantityIssued, 0), " + (int)GlobalEnums.rndQuantity + ") AS QuantityAvailables, WarehouseTransferDetails.Quantity, WarehouseTransferDetails.Remarks " + "\r\n";

            queryString = queryString + "       FROM        WarehouseTransferDetails " + "\r\n";
            queryString = queryString + "                   INNER JOIN Commodities ON WarehouseTransferDetails.WarehouseTransferID = @WarehouseTransferID AND WarehouseTransferDetails.CommodityID = Commodities.CommodityID " + "\r\n";
            queryString = queryString + "                   INNER JOIN BinLocations ON WarehouseTransferDetails.BinLocationID = BinLocations.BinLocationID " + "\r\n";

            queryString = queryString + "                   INNER JOIN GoodsReceiptDetails ON WarehouseTransferDetails.GoodsReceiptDetailID = GoodsReceiptDetails.GoodsReceiptDetailID " + "\r\n";
            queryString = queryString + "                   INNER JOIN (SELECT GoodsReceiptDetailID, SUM(Quantity) AS QuantityIssued FROM WarehouseTransferDetails WHERE WarehouseTransferID = @WarehouseTransferID GROUP BY GoodsReceiptDetailID) AS Issued_GoodsReceiptDetails ON WarehouseTransferDetails.GoodsReceiptDetailID = Issued_GoodsReceiptDetails.GoodsReceiptDetailID " + "\r\n";
            queryString = queryString + "                   INNER JOIN BinLocations BinLocationIssues ON GoodsReceiptDetails.BinLocationID = BinLocationIssues.BinLocationID " + "\r\n";

            queryString = queryString + "                   LEFT JOIN TransferOrderDetails ON WarehouseTransferDetails.TransferOrderDetailID = TransferOrderDetails.TransferOrderDetailID " + "\r\n";
            queryString = queryString + "                   LEFT JOIN (SELECT TransferOrderDetailID, SUM(Quantity) AS QuantityIssued FROM WarehouseTransferDetails WHERE WarehouseTransferID = @WarehouseTransferID GROUP BY TransferOrderDetailID) AS Issued_TransferOrderDetails ON WarehouseTransferDetails.TransferOrderDetailID = Issued_TransferOrderDetails.TransferOrderDetailID " + "\r\n";

            queryString = queryString + "       ORDER BY    WarehouseTransferDetails.WarehouseTransferDetailID " + "\r\n";

            queryString = queryString + "    END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetWarehouseTransferViewDetails", queryString);
        }





        private void GetWarehouseTransferAvailableWarehouses()
        {
            string queryString = " @LocationID int, @NMVNTaskID int " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "       SELECT          CAST(0 AS bit) AS HasTransferOrder, NULL AS BlendingInstructionID, " + (GlobalEnums.CBPP ? "IIF(Warehouses.LocationID = 1, N'Chuyển pha chế', N'Trả kho')" : "N'Chuyển kho'") + " AS Categories, Warehouses.WarehouseID, Warehouses.Code AS WarehouseCode, Warehouses.Name AS WarehouseName, Warehouses.LocationID AS LocationIssuedID, WarehouseReceipts.WarehouseID AS WarehouseReceiptID, WarehouseReceipts.Code AS WarehouseReceiptCode, WarehouseReceipts.Name AS WarehouseReceiptName, WarehouseReceipts.LocationID AS LocationReceiptID " + "\r\n";
            queryString = queryString + "       FROM            Warehouses  " + "\r\n";
            queryString = queryString + "                       INNER JOIN Warehouses AS WarehouseReceipts ON Warehouses.WarehouseID <> WarehouseReceipts.WarehouseID " + "\r\n";
            queryString = queryString + "       WHERE           Warehouses.WarehouseID IN (" + (GlobalEnums.DMC ? "1,2,3" : "1, 6") + ") AND WarehouseReceipts.WarehouseID IN (" + (GlobalEnums.DMC ? "1,2,3" : "1, 6") + ") " + "\r\n";

            queryString = queryString + "       UNION ALL       " + "\r\n";

            queryString = queryString + "       SELECT          CAST(0 AS bit) AS HasTransferOrder, NULL AS BlendingInstructionID, N'Chuyển vị trí' AS Categories, Warehouses.WarehouseID, Warehouses.Code AS WarehouseCode, Warehouses.Name AS WarehouseName, Warehouses.LocationID AS LocationIssuedID, Warehouses.WarehouseID AS WarehouseReceiptID, Warehouses.Code AS WarehouseReceiptCode, Warehouses.Name AS WarehouseReceiptName, Warehouses.LocationID AS LocationReceiptID " + "\r\n";
            queryString = queryString + "       FROM            Warehouses  " + "\r\n";
            queryString = queryString + "       WHERE           Warehouses.WarehouseID IN (6) " + "\r\n";

            queryString = queryString + "       UNION ALL       " + "\r\n";

            queryString = queryString + "       SELECT          CAST(0 AS bit) AS HasTransferOrder, 0 AS BlendingInstructionID, N'Chuyển " + (GlobalEnums.CBPP ? "pha chế" : "kho") + " theo lệnh' AS Categories, Warehouses.WarehouseID, Warehouses.Code AS WarehouseCode, Warehouses.Name AS WarehouseName, Warehouses.LocationID AS LocationIssuedID, WarehouseReceipts.WarehouseID AS WarehouseReceiptID, WarehouseReceipts.Code AS WarehouseReceiptCode, WarehouseReceipts.Name AS WarehouseReceiptName, WarehouseReceipts.LocationID AS LocationReceiptID " + "\r\n";
            queryString = queryString + "       FROM            Warehouses  " + "\r\n";
            queryString = queryString + "                       INNER JOIN Warehouses AS WarehouseReceipts ON Warehouses.WarehouseID <> WarehouseReceipts.WarehouseID " + "\r\n";
            queryString = queryString + "       WHERE           Warehouses.WarehouseID IN (1) AND WarehouseReceipts.WarehouseID IN (6) " + "\r\n";

            queryString = queryString + "       ORDER BY        Categories, WarehouseID " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetWarehouseTransferAvailableWarehouses", queryString);
        }

        private void GetWarehouseTransferPendingTransferOrders()
        {
            string queryString = " @LocationID int, @NMVNTaskID int " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "       SELECT          CAST(1 AS bit) AS HasTransferOrder, TransferOrders.TransferOrderID, TransferOrders.Reference AS TransferOrderReference, TransferOrders.EntryDate AS TransferOrderEntryDate, TransferOrders.Caption, TransferOrders.TransferOrderJobs, " + "\r\n";
            queryString = queryString + "                       TransferOrders.WarehouseID, Warehouses.Code AS WarehouseCode, Warehouses.Name AS WarehouseName, Warehouses.LocationID AS LocationIssuedID, TransferOrders.WarehouseReceiptID, WarehouseReceipts.Code AS WarehouseReceiptCode, WarehouseReceipts.Name AS WarehouseReceiptName, WarehouseReceipts.LocationID AS LocationReceiptID " + "\r\n";

            queryString = queryString + "       FROM            TransferOrders " + "\r\n";
            queryString = queryString + "                       INNER JOIN Warehouses ON TransferOrders.TransferOrderID IN (SELECT DISTINCT TransferOrderID FROM TransferOrderDetails WHERE LocationIssuedID = @LocationID AND NMVNTaskID = @NMVNTaskID - 1000000 AND Approved = 1 AND InActive = 0 AND InActivePartial = 0 AND ROUND(Quantity - QuantityIssued, " + (int)GlobalEnums.rndQuantity + ") > 0) " + (GlobalEnums.CBPP ? " AND EntryDate >= DATEADD(day, DATEDIFF(day, 0, GETDATE()) - 2, 0) " : "") + " AND TransferOrders.WarehouseID = Warehouses.WarehouseID " + "\r\n";
            queryString = queryString + "                       INNER JOIN Warehouses WarehouseReceipts ON TransferOrders.WarehouseReceiptID = WarehouseReceipts.WarehouseID " + "\r\n";
            queryString = queryString + "       ORDER BY        TransferOrders.EntryDate DESC " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetWarehouseTransferPendingTransferOrders", queryString);
        }

        private void GetWarehouseTransferPendingWarehouses()
        {
            string queryString = " @LocationID int, @NMVNTaskID int " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "       SELECT          CAST(1 AS bit) AS HasTransferOrder, PendingWarehouses.WarehouseID, Warehouses.Code AS WarehouseCode, Warehouses.Name AS WarehouseName, Warehouses.LocationID AS LocationIssuedID, PendingWarehouses.WarehouseReceiptID, WarehouseReceipts.Code AS WarehouseReceiptCode, WarehouseReceipts.Name AS WarehouseReceiptName, WarehouseReceipts.LocationID AS LocationReceiptID " + "\r\n";
            queryString = queryString + "       FROM           (SELECT WarehouseID, WarehouseReceiptID FROM TransferOrderDetails WHERE LocationIssuedID = @LocationID AND NMVNTaskID = @NMVNTaskID - 1000000 AND Approved = 1 AND InActive = 0 AND InActivePartial = 0 " + (GlobalEnums.CBPP ? " AND EntryDate >= DATEADD(day, DATEDIFF(day, 0, GETDATE()) - 2, 0) " : "") + " AND ROUND(Quantity - QuantityIssued, " + (int)GlobalEnums.rndQuantity + ") > 0 GROUP BY WarehouseID, WarehouseReceiptID) PendingWarehouses " + "\r\n";
            queryString = queryString + "                       INNER JOIN Warehouses ON PendingWarehouses.WarehouseID = Warehouses.WarehouseID " + "\r\n";
            queryString = queryString + "                       INNER JOIN Warehouses WarehouseReceipts ON PendingWarehouses.WarehouseReceiptID = WarehouseReceipts.WarehouseID " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetWarehouseTransferPendingWarehouses", queryString);
        }


        private void GetWarehouseTransferPendingTransferOrderDetails()
        {
            string queryString;

            queryString = " @WebAPI bit, @LocationID Int, @NMVNTaskID int, @WarehouseTransferID Int, @TransferOrderID Int, @WarehouseID Int, @WarehouseReceiptID Int, @Barcode nvarchar(60), @GoodsReceiptDetailIDs varchar(3999) " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "   BEGIN " + "\r\n";

            queryString = queryString + "       IF  (@TransferOrderID > 0) " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLPendingDetails(true) + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLPendingDetails(false) + "\r\n";

            queryString = queryString + "   END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetWarehouseTransferPendingTransferOrderDetails", queryString);
        }

        private string BuildSQLPendingDetails(bool isTransferOrderID)
        {
            string queryString = "";
            queryString = queryString + "   BEGIN " + "\r\n";

            if (GlobalEnums.CBPP)
            {
                queryString = queryString + "       IF  (@WarehouseReceiptID = 6) " + "\r\n"; //@WarehouseReceiptID = 6: KPC
                queryString = queryString + "           " + this.BuildSQLPendingDetails(isTransferOrderID, true) + "\r\n";
                queryString = queryString + "       ELSE " + "\r\n";
                queryString = queryString + "           " + this.BuildSQLPendingDetails(isTransferOrderID, false) + "\r\n";
            }
            else
                queryString = queryString + "           " + this.BuildSQLPendingDetails(isTransferOrderID, false) + "\r\n";
            
            queryString = queryString + "   END " + "\r\n";

            return queryString;
        }

        private string BuildSQLPendingDetails(bool isTransferOrderID, bool isLabOK)
        {
            string queryString = "";
            queryString = queryString + "   BEGIN " + "\r\n";

            queryString = queryString + "       IF  (NOT @Barcode IS NULL AND @Barcode <> '' AND @Barcode <> '0') " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLPendingDetails(isTransferOrderID, isLabOK, true) + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLPendingDetails(isTransferOrderID, isLabOK, false) + "\r\n";

            queryString = queryString + "   END " + "\r\n";

            return queryString;
        }

        private string BuildSQLPendingDetails(bool isTransferOrderID, bool isLabOK, bool isBarcode)
        {
            string queryString = "";
            queryString = queryString + "   BEGIN " + "\r\n";

            queryString = queryString + "       IF  (@GoodsReceiptDetailIDs <> '') " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLPendingDetails(isTransferOrderID, isLabOK, isBarcode, true) + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLPendingDetails(isTransferOrderID, isLabOK, isBarcode, false) + "\r\n";

            queryString = queryString + "   END " + "\r\n";

            return queryString;
        }

        private string BuildSQLPendingDetails(bool isTransferOrderID, bool isLabOK, bool isBarcode, bool isGoodsReceiptDetailIDs)
        {
            string queryString = "";
            queryString = queryString + "   BEGIN " + "\r\n";

            queryString = queryString + "       IF (@WarehouseTransferID <= 0 OR @WebAPI = 1) " + "\r\n";
            queryString = queryString + "               BEGIN " + "\r\n";
            queryString = queryString + "                   " + this.BuildSQLNew(isTransferOrderID, isLabOK, isBarcode, isGoodsReceiptDetailIDs) + "\r\n";
            queryString = queryString + "                   ORDER BY TransferOrderDetails.TransferOrderDetailID " + "\r\n";
            queryString = queryString + "               END " + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";

            queryString = queryString + "               BEGIN " + "\r\n";
            queryString = queryString + "                   " + this.BuildSQLNew(isTransferOrderID, isLabOK, isBarcode, isGoodsReceiptDetailIDs) + " WHERE TransferOrderDetails.TransferOrderDetailID NOT IN (SELECT TransferOrderDetailID FROM WarehouseTransferDetails WHERE WarehouseTransferID = @WarehouseTransferID) " + "\r\n";
            queryString = queryString + "                   UNION ALL " + "\r\n";
            queryString = queryString + "                   " + this.BuildSQLEdit(isTransferOrderID, isLabOK, isBarcode, isGoodsReceiptDetailIDs) + "\r\n";
            queryString = queryString + "                   ORDER BY TransferOrderDetails.TransferOrderDetailID " + "\r\n";
            queryString = queryString + "               END " + "\r\n";

            queryString = queryString + "   END " + "\r\n";

            return queryString;
        }

        private string BuildSQLNew(bool isTransferOrderID, bool isLabOK, bool isBarcode, bool isGoodsReceiptDetailIDs)
        {
            string queryString = "";

            queryString = queryString + "       SELECT      TransferOrderDetails.TransferOrderDetailID, TransferOrderDetails.TransferOrderID, Commodities.CommodityID, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, Commodities.OfficialCode, Commodities.CodePartA, Commodities.CodePartB, Commodities.CodePartC, Commodities.CodePartD, Commodities.CodePartE, Commodities.CodePartF, Commodities.CommodityTypeID, Commodities.Weight, " + "\r\n";
            queryString = queryString + "                   GoodsReceiptDetails.GoodsReceiptID, GoodsReceiptDetails.GoodsReceiptDetailID, GoodsReceipts.Reference AS GoodsReceiptReference, GoodsReceipts.Code AS GoodsReceiptCode, GoodsReceipts.EntryDate AS GoodsReceiptEntryDate, GoodsReceiptDetails.BatchID, GoodsReceiptDetails.BatchEntryDate, GoodsReceiptDetails.Barcode, GoodsReceiptDetails.BatchCode, GoodsReceiptDetails.SealCode, GoodsReceiptDetails.LabCode, GoodsReceiptDetails.LabID, GoodsReceiptDetails.ProductionDate, GoodsReceiptDetails.ExpiryDate, GoodsReceiptDetails.BinLocationID, BinLocations.Code AS BinLocationCode, " + "\r\n";
            queryString = queryString + "                   TransferOrderDetails.Quantity AS QuantityTO, ROUND(TransferOrderDetails.Quantity - TransferOrderDetails.QuantityIssued, " + (int)GlobalEnums.rndQuantity + ") AS TransferOrderRemains, ROUND(GoodsReceiptDetails.Quantity - GoodsReceiptDetails.QuantityIssued, " + (int)GlobalEnums.rndQuantity + ") AS QuantityAvailables, 0 AS Quantity, TransferOrderDetails.Remarks, CAST(0 AS bit) AS IsSelected " + "\r\n";

            queryString = queryString + "       FROM        TransferOrderDetails " + "\r\n";
            queryString = queryString + "                   INNER JOIN Commodities ON " + (isTransferOrderID ? "TransferOrderDetails.TransferOrderID = @TransferOrderID" : "TransferOrderDetails.NMVNTaskID = @NMVNTaskID - 1000000 AND TransferOrderDetails.WarehouseID = @WarehouseID AND TransferOrderDetails.WarehouseReceiptID = @WarehouseReceiptID") + " AND TransferOrderDetails.Approved = 1 AND TransferOrderDetails.InActive = 0 AND TransferOrderDetails.InActivePartial = 0 AND ROUND(TransferOrderDetails.Quantity - TransferOrderDetails.QuantityIssued, " + (int)GlobalEnums.rndQuantity + ") > 0 AND TransferOrderDetails.CommodityID = Commodities.CommodityID " + "\r\n";

            queryString = queryString + "                   LEFT JOIN GoodsReceiptDetails ON GoodsReceiptDetails.WarehouseID = @WarehouseID AND TransferOrderDetails.CommodityID = GoodsReceiptDetails.CommodityID AND GoodsReceiptDetails.Approved = 1 AND ROUND(GoodsReceiptDetails.Quantity - GoodsReceiptDetails.QuantityIssued, " + (int)GlobalEnums.rndQuantity + ") > 0 " + (isLabOK ? " AND GoodsReceiptDetails.ExpiryDate <= GETDATE() AND GoodsReceiptDetails.LabID IN (SELECT LabID FROM Labs WHERE Approved = 1 AND InActive = 0 AND Hold = 0)" : "") + (isBarcode ? " AND GoodsReceiptDetails.Barcode = @Barcode" : "") + (isGoodsReceiptDetailIDs ? " AND GoodsReceiptDetails.GoodsReceiptDetailID NOT IN (SELECT Id FROM dbo.SplitToIntList (@GoodsReceiptDetailIDs))" : "") + "\r\n";
            queryString = queryString + "                   LEFT JOIN GoodsReceipts ON GoodsReceiptDetails.GoodsReceiptID = GoodsReceipts.GoodsReceiptID " + "\r\n";
            queryString = queryString + "                   LEFT JOIN BinLocations ON GoodsReceiptDetails.BinLocationID = BinLocations.BinLocationID " + "\r\n";

            return queryString;
        }

        private string BuildSQLEdit(bool isTransferOrderID, bool isLabOK, bool isBarcode, bool isGoodsReceiptDetailIDs)
        {
            string queryString = "";

            queryString = queryString + "       SELECT      TransferOrderDetails.TransferOrderDetailID, TransferOrderDetails.TransferOrderID, Commodities.CommodityID, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, Commodities.OfficialCode, Commodities.CodePartA, Commodities.CodePartB, Commodities.CodePartC, Commodities.CodePartD, Commodities.CodePartE, Commodities.CodePartF, Commodities.CommodityTypeID, Commodities.Weight, " + "\r\n";
            queryString = queryString + "                   GoodsReceiptDetails.GoodsReceiptID, GoodsReceiptDetails.GoodsReceiptDetailID, GoodsReceipts.Reference AS GoodsReceiptReference, GoodsReceipts.Code AS GoodsReceiptCode, GoodsReceipts.EntryDate AS GoodsReceiptEntryDate, GoodsReceiptDetails.BatchID, GoodsReceiptDetails.BatchEntryDate, GoodsReceiptDetails.Barcode, GoodsReceiptDetails.BatchCode, GoodsReceiptDetails.SealCode, GoodsReceiptDetails.LabCode, GoodsReceiptDetails.LabID, GoodsReceiptDetails.ProductionDate, GoodsReceiptDetails.ExpiryDate, GoodsReceiptDetails.BinLocationID, BinLocations.Code AS BinLocationCode, " + "\r\n";
            queryString = queryString + "                   TransferOrderDetails.Quantity AS QuantityTO, ROUND(TransferOrderDetails.Quantity - TransferOrderDetails.QuantityIssued + WarehouseTransferDetails.Quantity, " + (int)GlobalEnums.rndQuantity + ") AS TransferOrderRemains, ROUND(GoodsReceiptDetails.Quantity - GoodsReceiptDetails.QuantityIssued + ISNULL(IssuedGoodsReceiptDetails.Quantity, 0), " + (int)GlobalEnums.rndQuantity + ") AS QuantityAvailables, 0 AS Quantity, TransferOrderDetails.Remarks, CAST(0 AS bit) AS IsSelected " + "\r\n";

            queryString = queryString + "       FROM        TransferOrderDetails " + "\r\n";
            queryString = queryString + "                   INNER JOIN (SELECT TransferOrderDetailID, SUM(Quantity) AS Quantity FROM WarehouseTransferDetails WHERE WarehouseTransferID = @WarehouseTransferID GROUP BY TransferOrderDetailID) AS WarehouseTransferDetails ON TransferOrderDetails.TransferOrderDetailID = WarehouseTransferDetails.TransferOrderDetailID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Commodities ON TransferOrderDetails.CommodityID = Commodities.CommodityID " + "\r\n";

            queryString = queryString + "                   LEFT JOIN GoodsReceiptDetails ON GoodsReceiptDetails.WarehouseID = @WarehouseID AND TransferOrderDetails.CommodityID = GoodsReceiptDetails.CommodityID AND GoodsReceiptDetails.Approved = 1 AND (ROUND(GoodsReceiptDetails.Quantity - GoodsReceiptDetails.QuantityIssued, " + (int)GlobalEnums.rndQuantity + ") > 0 OR GoodsReceiptDetails.GoodsReceiptDetailID IN (SELECT GoodsReceiptDetailID FROM WarehouseTransferDetails WHERE WarehouseTransferID = @WarehouseTransferID)) " + (isLabOK ? " AND GoodsReceiptDetails.ExpiryDate <= GETDATE() AND GoodsReceiptDetails.LabID IN (SELECT LabID FROM Labs WHERE Approved = 1 AND InActive = 0 AND Hold = 0)" : "") + (isBarcode ? " AND GoodsReceiptDetails.Barcode = @Barcode" : "") + (isGoodsReceiptDetailIDs ? " AND GoodsReceiptDetails.GoodsReceiptDetailID NOT IN (SELECT Id FROM dbo.SplitToIntList (@GoodsReceiptDetailIDs))" : "") + "\r\n";
            queryString = queryString + "                   LEFT JOIN (SELECT GoodsReceiptDetailID, SUM(Quantity) AS Quantity FROM WarehouseTransferDetails WHERE WarehouseTransferID = @WarehouseTransferID GROUP BY GoodsReceiptDetailID) AS IssuedGoodsReceiptDetails ON GoodsReceiptDetails.GoodsReceiptDetailID = IssuedGoodsReceiptDetails.GoodsReceiptDetailID " + "\r\n";
            queryString = queryString + "                   LEFT JOIN GoodsReceipts ON GoodsReceiptDetails.GoodsReceiptID = GoodsReceipts.GoodsReceiptID " + "\r\n";
            queryString = queryString + "                   LEFT JOIN BinLocations ON GoodsReceiptDetails.BinLocationID = BinLocations.BinLocationID " + "\r\n";

            return queryString;
        }


        private void WarehouseTransferSaveRelative()
        {
            string queryString = " @EntityID int, @SaveRelativeOption int " + "\r\n"; //SaveRelativeOption: 1: Update, -1:Undo
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "       BEGIN  " + "\r\n";

            queryString = queryString + "           DECLARE @msg NVARCHAR(300) ";


            queryString = queryString + "           IF (@SaveRelativeOption = 1) " + "\r\n";
            queryString = queryString + "               BEGIN " + "\r\n";

            queryString = queryString + "                   UPDATE          WarehouseTransferDetails " + "\r\n";
            queryString = queryString + "                   SET             WarehouseTransferDetails.Reference = WarehouseTransfers.Reference " + "\r\n";
            queryString = queryString + "                   FROM            WarehouseTransfers INNER JOIN WarehouseTransferDetails ON WarehouseTransfers.WarehouseTransferID = @EntityID AND WarehouseTransfers.WarehouseTransferID = WarehouseTransferDetails.WarehouseTransferID " + "\r\n";

            #region UPDATE WorkshiftID
            queryString = queryString + "                   DECLARE         @EntryDate Datetime, @ShiftID int, @WorkshiftID int " + "\r\n";
            queryString = queryString + "                   SELECT          @EntryDate = CONVERT(date, EntryDate), @ShiftID = ShiftID FROM WarehouseTransfers WHERE WarehouseTransferID = @EntityID " + "\r\n";
            queryString = queryString + "                   SET             @WorkshiftID = (SELECT TOP 1 WorkshiftID FROM Workshifts WHERE EntryDate = @EntryDate AND ShiftID = @ShiftID) " + "\r\n";

            queryString = queryString + "                   IF             (@WorkshiftID IS NULL) " + "\r\n";
            queryString = queryString + "                       BEGIN ";
            queryString = queryString + "                           INSERT INTO     Workshifts(EntryDate, ShiftID, Code, Name, WorkingHours, Remarks) SELECT @EntryDate, ShiftID, Code, Name, WorkingHours, Remarks FROM Shifts WHERE ShiftID = @ShiftID " + "\r\n";
            queryString = queryString + "                           SELECT          @WorkshiftID = SCOPE_IDENTITY(); " + "\r\n";
            queryString = queryString + "                       END ";

            queryString = queryString + "                   UPDATE          WarehouseTransfers        SET WorkshiftID = @WorkshiftID WHERE WarehouseTransferID = @EntityID " + "\r\n";
            queryString = queryString + "                   UPDATE          WarehouseTransferDetails  SET WorkshiftID = @WorkshiftID WHERE WarehouseTransferID = @EntityID " + "\r\n";
            #endregion UPDATE WorkshiftID

            queryString = queryString + "               END " + "\r\n";



            queryString = queryString + "           DECLARE         @WarehouseTransferDetails TABLE (GoodsReceiptDetailID int NOT NULL PRIMARY KEY, Quantity decimal(18, 2) NOT NULL)" + "\r\n";
            queryString = queryString + "           INSERT INTO     @WarehouseTransferDetails (GoodsReceiptDetailID, Quantity) SELECT GoodsReceiptDetailID, SUM(Quantity) AS Quantity FROM WarehouseTransferDetails WHERE WarehouseTransferID = @EntityID GROUP BY GoodsReceiptDetailID " + "\r\n";

            #region UPDATE GoodsReceiptDetails
            queryString = queryString + "           UPDATE          GoodsReceiptDetails " + "\r\n";
            queryString = queryString + "           SET             GoodsReceiptDetails.QuantityIssued = ROUND(GoodsReceiptDetails.QuantityIssued + WarehouseTransferDetails.Quantity * @SaveRelativeOption, " + (int)GlobalEnums.rndQuantity + ") " + "\r\n";
            queryString = queryString + "           FROM            GoodsReceiptDetails " + "\r\n";
            queryString = queryString + "                           INNER JOIN @WarehouseTransferDetails WarehouseTransferDetails ON GoodsReceiptDetails.GoodsReceiptDetailID = WarehouseTransferDetails.GoodsReceiptDetailID AND GoodsReceiptDetails.Approved = 1 " + "\r\n";

            queryString = queryString + "           IF @@ROWCOUNT <> (SELECT COUNT(*) FROM @WarehouseTransferDetails) " + "\r\n";
            queryString = queryString + "               BEGIN " + "\r\n";
            queryString = queryString + "                   SET         @msg = N'Phiếu nhập kho đã hủy, chưa duyệt hoặc đã xóa.' ; " + "\r\n";
            queryString = queryString + "                   THROW       61001,  @msg, 1; " + "\r\n";
            queryString = queryString + "               END " + "\r\n";
            #endregion

            #region TRANSFERORDERS
            queryString = queryString + "           DECLARE         @IssueTransferOrderDetails TABLE (TransferOrderDetailID int NOT NULL PRIMARY KEY, Quantity decimal(18, 2) NOT NULL)" + "\r\n";
            queryString = queryString + "           INSERT INTO     @IssueTransferOrderDetails (TransferOrderDetailID, Quantity) SELECT TransferOrderDetailID, SUM(Quantity) AS Quantity FROM WarehouseTransferDetails WHERE WarehouseTransferID = @EntityID AND NOT TransferOrderDetailID IS NULL GROUP BY TransferOrderDetailID " + "\r\n";

            queryString = queryString + "           IF (NOT (SELECT MAX(TransferOrderDetailID) FROM @IssueTransferOrderDetails) IS NULL) " + "\r\n";
            queryString = queryString + "               BEGIN  " + "\r\n";
            queryString = queryString + "                   UPDATE          TransferOrderDetails " + "\r\n";
            queryString = queryString + "                   SET             TransferOrderDetails.QuantityIssued = ROUND(TransferOrderDetails.QuantityIssued + IssueTransferOrderDetails.Quantity * @SaveRelativeOption, " + (int)GlobalEnums.rndQuantity + ") " + "\r\n";
            queryString = queryString + "                   FROM            TransferOrderDetails " + "\r\n";
            queryString = queryString + "                                   INNER JOIN @IssueTransferOrderDetails IssueTransferOrderDetails ON ((TransferOrderDetails.Approved = 1 AND TransferOrderDetails.InActive = 0 AND TransferOrderDetails.InActivePartial = 0) OR @SaveRelativeOption = -1) AND TransferOrderDetails.TransferOrderDetailID = IssueTransferOrderDetails.TransferOrderDetailID " + "\r\n";

            queryString = queryString + "                   IF @@ROWCOUNT <> (SELECT COUNT(*) FROM @IssueTransferOrderDetails) " + "\r\n";
            queryString = queryString + "                       BEGIN " + "\r\n";
            queryString = queryString + "                           SET         @msg = N'Lệnh điều hàng đã hủy, chưa duyệt hoặc đã xóa.' ; " + "\r\n";
            queryString = queryString + "                           THROW       61001,  @msg, 1; " + "\r\n";
            queryString = queryString + "                       END " + "\r\n";
            queryString = queryString + "               END  " + "\r\n";
            #endregion

            queryString = queryString + "       END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("WarehouseTransferSaveRelative", queryString);
        }

        private void WarehouseTransferPostSaveValidate()
        {
            string[] queryArray = new string[3];
            //*****GlobalEnums.CBPP: CHỈ CÓ MỘT BỘ PHẬN WAREHOUSE MỚI SỬ DỤNG MODULE CHUYỂN KHO ==> DO ĐÓ: KHÔNG CẦN XEM XÉT ĐIỀU KIỆN: WarehouseTransferDetails.LocationID <> WarehouseTransferDetails.LocationIssuedID
            queryArray[0] = " SELECT TOP 1 @FoundEntity = N'Ngày nhập kho: ' + CAST(GoodsReceipts.EntryDate AS nvarchar) + N', hoặc chọn kho nhập không đúng.' FROM WarehouseTransferDetails INNER JOIN GoodsReceipts ON WarehouseTransferDetails.WarehouseTransferID = @EntityID AND WarehouseTransferDetails.GoodsReceiptID = GoodsReceipts.GoodsReceiptID AND (WarehouseTransferDetails.EntryDate < GoodsReceipts.EntryDate" + (GlobalEnums.CBPP ? "" : " OR WarehouseTransferDetails.LocationID <> WarehouseTransferDetails.LocationIssuedID OR WarehouseTransferDetails.WarehouseID = WarehouseTransferDetails.WarehouseReceiptID") + ") ";
            queryArray[1] = " SELECT TOP 1 @FoundEntity = N'Lệnh điều hàng: ' + CAST(TransferOrders.EntryDate AS nvarchar) FROM WarehouseTransferDetails INNER JOIN TransferOrders ON WarehouseTransferDetails.WarehouseTransferID = @EntityID AND WarehouseTransferDetails.TransferOrderID = TransferOrders.TransferOrderID AND WarehouseTransferDetails.EntryDate < TransferOrders.EntryDate ";
            queryArray[2] = " SELECT TOP 1 @FoundEntity = N'Số lượng xuất vượt quá số lượng tồn kho: ' + CAST(ROUND(Quantity - QuantityIssued, " + (int)GlobalEnums.rndQuantity + ") AS nvarchar) FROM GoodsReceiptDetails WHERE (ROUND(Quantity - QuantityIssued, " + (int)GlobalEnums.rndQuantity + ") < 0) ";

            this.totalSmartPortalEntities.CreateProcedureToCheckExisting("WarehouseTransferPostSaveValidate", queryArray);
        }




        private void WarehouseTransferApproved()
        {
            string[] queryArray = new string[1];

            queryArray[0] = " SELECT TOP 1 @FoundEntity = WarehouseTransferID FROM WarehouseTransfers WHERE WarehouseTransferID = @EntityID AND Approved = 1";

            this.totalSmartPortalEntities.CreateProcedureToCheckExisting("WarehouseTransferApproved", queryArray);
        }


        private void WarehouseTransferEditable()
        {
            string[] queryArray = null; //IMPORTANT: THESE QUERIES ARE COPIED FROM GoodsReceiptEditable (THE SAME: GoodsReceiptEditable, WarehouseAdjustmentEditable, WarehouseTransferEditable)
            string[] queryOneStep = new string[4];
            string[] queryTwoStep = new string[2];

            queryOneStep[0] = " SELECT TOP 1 @FoundEntity = GoodsReceiptID FROM MaterialIssueDetails WHERE GoodsReceiptID = @GoodsReceiptID ";
            queryOneStep[1] = " SELECT TOP 1 @FoundEntity = GoodsReceiptID FROM WarehouseTransferDetails WHERE GoodsReceiptID = @GoodsReceiptID ";
            queryOneStep[2] = " SELECT TOP 1 @FoundEntity = GoodsReceiptID FROM WarehouseAdjustmentDetails WHERE GoodsReceiptID = @GoodsReceiptID ";
            queryOneStep[3] = " SELECT TOP 1 @FoundEntity = GoodsReceiptID FROM PackageIssueDetails WHERE GoodsReceiptID = @GoodsReceiptID ";

            queryTwoStep[0] = " SELECT TOP 1 @FoundEntity = WarehouseTransferID FROM GoodsReceipts WHERE WarehouseTransferID = @EntityID ";
            queryTwoStep[1] = " SELECT TOP 1 @FoundEntity = WarehouseTransferID FROM GoodsReceiptDetails WHERE WarehouseTransferID = @EntityID ";


            string queryString = "       DECLARE @GoodsReceiptID int " + "\r\n";

            queryString = queryString + "       IF ((SELECT OneStep FROM WarehouseTransfers WHERE WarehouseTransferID = @EntityID) = 1) " + "\r\n";
            queryString = queryString + "           BEGIN " + "\r\n";
            queryString = queryString + "               SELECT TOP 1 @GoodsReceiptID = GoodsReceiptID FROM GoodsReceipts WHERE WarehouseTransferID = @EntityID " + "\r\n";
            queryString = queryString + "               IF (@GoodsReceiptID IS NULL) BEGIN SELECT @FoundEntity AS FoundEntity    RETURN 0 END " + "\r\n";
            queryString = queryString + "               " + this.totalSmartPortalEntities.SqlToCheckExisting(queryOneStep) + "\r\n";
            queryString = queryString + "           END " + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           BEGIN " + "\r\n";
            queryString = queryString + "               " + this.totalSmartPortalEntities.SqlToCheckExisting(queryTwoStep) + "\r\n";
            queryString = queryString + "           END " + "\r\n";

            this.totalSmartPortalEntities.CreateProcedureToCheckExisting("WarehouseTransferEditable", queryArray, queryString);
        }

        private void WarehouseTransferToggleApproved()
        {
            string queryString = " @EntityID int, @Approved bit " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "       UPDATE      WarehouseTransfers  SET Approved = @Approved, ApprovedDate = GetDate() WHERE WarehouseTransferID = @EntityID AND Approved = ~@Approved" + "\r\n";

            queryString = queryString + "       IF @@ROWCOUNT = 1 " + "\r\n";
            queryString = queryString + "           BEGIN " + "\r\n";
            queryString = queryString + "               UPDATE          WarehouseTransferDetails  SET Approved = @Approved WHERE WarehouseTransferID = @EntityID ; " + "\r\n";

            queryString = queryString + "               UPDATE          GoodsReceipts  SET Approved = @Approved, ApprovedDate = GetDate() WHERE WarehouseTransferID = @EntityID " + "\r\n";
            queryString = queryString + "               UPDATE          GoodsReceiptDetails  SET Approved = @Approved WHERE WarehouseTransferID = @EntityID ; " + "\r\n";

            queryString = queryString + "           END " + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           BEGIN " + "\r\n";
            queryString = queryString + "               DECLARE     @msg NVARCHAR(300) = N'Dữ liệu không tồn tại hoặc đã ' + iif(@Approved = 0, N'hủy', '')  + N' duyệt' ; " + "\r\n";
            queryString = queryString + "               THROW       61001,  @msg, 1; " + "\r\n";
            queryString = queryString + "           END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("WarehouseTransferToggleApproved", queryString);
        }


        private void WarehouseTransferInitReference()
        {
            SimpleInitReference simpleInitReference = new WarehouseTransferInitReference("WarehouseTransfers", "WarehouseTransferID", "Reference", ModelSettingManager.ReferenceLength, ModelSettingManager.ReferencePrefix(GlobalEnums.NmvnTaskID.WarehouseTransfer));
            this.totalSmartPortalEntities.CreateTrigger("WarehouseTransferInitReference", simpleInitReference.CreateQuery());

            string queryString = " @WarehouseTransferID int " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";
            queryString = queryString + "       SELECT TOP 1 Reference FROM WarehouseTransfers WHERE WarehouseTransferID = @WarehouseTransferID " + "\r\n";
            this.totalSmartPortalEntities.CreateStoredProcedure("WarehouseTransferGetReference", queryString);
        }



        private void WarehouseTransferSheet()
        {
            string queryString = " @WarehouseTransferID int " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";
            queryString = queryString + "    BEGIN " + "\r\n";

            queryString = queryString + "       DECLARE         @LocalWarehouseTransferID int    SET @LocalWarehouseTransferID = @WarehouseTransferID" + "\r\n";

            queryString = queryString + "       SELECT          WarehouseTransfers.WarehouseTransferID, WarehouseTransfers.EntryDate, WarehouseTransfers.Reference, WarehouseTransfers.NMVNTaskID, Warehouses.Name AS WarehouseName, WarehouseReceipts.Name AS WarehouseReceiptName, WarehouseTransfers.Description, " + "\r\n";
            queryString = queryString + "                       WarehouseTransferDetails.CommodityID, WarehouseTransferDetails.CommodityTypeID, Commodities.Code, Commodities.CodePartA, Commodities.CodePartB, Commodities.CodePartC, Commodities.CodePartD, Commodities.CodePartE, Commodities.CodePartF, Commodities.Name AS CommodityName, Commodities.SalesUnit, WarehouseTransferDetails.BatchEntryDate, WarehouseTransferDetails.Quantity, WarehouseTransferDetails.Remarks " + "\r\n";

            queryString = queryString + "       FROM            WarehouseTransfers " + "\r\n";
            queryString = queryString + "                       INNER JOIN WarehouseTransferDetails ON WarehouseTransfers.WarehouseTransferID = @LocalWarehouseTransferID AND WarehouseTransfers.WarehouseTransferID = WarehouseTransferDetails.WarehouseTransferID " + "\r\n";
            queryString = queryString + "                       INNER JOIN Commodities ON WarehouseTransferDetails.CommodityID = Commodities.CommodityID " + "\r\n";
            queryString = queryString + "                       INNER JOIN Warehouses ON WarehouseTransfers.WarehouseID = Warehouses.WarehouseID " + "\r\n";
            queryString = queryString + "                       INNER JOIN Warehouses AS WarehouseReceipts ON WarehouseTransfers.WarehouseReceiptID = WarehouseReceipts.WarehouseID " + "\r\n";

            queryString = queryString + "       ORDER BY        WarehouseTransferDetails.WarehouseTransferDetailID " + "\r\n";


            queryString = queryString + "    END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("WarehouseTransferSheet", queryString);

        }
    }
}

