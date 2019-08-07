﻿using System;
using System.Text;

using TotalBase;
using TotalBase.Enums;
using TotalModel.Models;

namespace TotalDAL.Helpers.SqlProgrammability.Purchases
{
    public class GoodsArrival
    {
        private readonly TotalSmartPortalEntities totalSmartPortalEntities;

        public GoodsArrival(TotalSmartPortalEntities totalSmartPortalEntities)
        {
            this.totalSmartPortalEntities = totalSmartPortalEntities;
        }

        public void RestoreProcedure()
        {
            this.GetGoodsArrivalIndexes();
            this.GetGoodsArrivalViewDetails();

            this.GetGoodsArrivalPendingCustomers();
            this.GetGoodsArrivalPendingPurchaseOrders();
            this.GetGoodsArrivalPendingPurchaseOrderDetails();

            this.GoodsArrivalSaveRelative();
            this.GoodsArrivalPostSaveValidate();

            this.GoodsArrivalApproved();
            this.GoodsArrivalEditable();

            this.GoodsArrivalToggleApproved();
            this.GoodsArrivalChangeExpiryDate();

            this.GetBarcodeBases();
            this.GetBarcodeSymbologies();
            this.SetBarcodeSymbologies();

            this.GoodsArrivalInitReference();

            this.GoodsArrivalSheet();
        }


        private void GetGoodsArrivalIndexes()
        {
            string queryString;

            queryString = " @NMVNTaskID int, @AspUserID nvarchar(128), @FromDate DateTime, @ToDate DateTime, @PendingOnly bit " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";
            queryString = queryString + "    BEGIN " + "\r\n";

            queryString = queryString + "       IF  (@PendingOnly = 1) " + "\r\n";
            queryString = queryString + "           " + this.GetGoodsArrivalIndexSQL(true) + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           " + this.GetGoodsArrivalIndexSQL(false) + "\r\n";

            queryString = queryString + "    END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetGoodsArrivalIndexes", queryString);
        }

        private string GetGoodsArrivalIndexSQL(bool pendingOnly)
        {
            string queryString = "";

            queryString = queryString + "       SELECT      GoodsArrivals.GoodsArrivalID, CAST(GoodsArrivals.EntryDate AS DATE) AS EntryDate, GoodsArrivals.Reference, GoodsArrivals.Code, GoodsArrivals.PackingList, GoodsArrivals.CustomsDeclaration, GoodsArrivals.PurchaseOrderID, GoodsArrivals.PurchaseOrderCodes, GoodsArrivals.PurchaseOrderReferences, PurchaseOrders.EntryDate AS PurchaseOrderEntryDate, Locations.Code AS LocationCode, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, Transporters.Name AS TransporterName, GoodsArrivals.Caption, GoodsArrivals.Description, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, GoodsArrivalDetails.BatchCode, GoodsArrivalDetails.LabCode, GoodsArrivalDetails.Quantity, GoodsArrivalDetails.Packages, GoodsArrivals.TotalQuantity, GoodsArrivals.TotalQuantityReceipted, GoodsArrivals.TotalPackages, GoodsArrivals.Approved " + "\r\n";
            queryString = queryString + "       FROM        GoodsArrivals " + "\r\n";
            queryString = queryString + "                   INNER JOIN Locations ON GoodsArrivals.NMVNTaskID = @NMVNTaskID AND " + (pendingOnly ? "GoodsArrivals.GoodsArrivalID IN (SELECT GoodsArrivalID FROM GoodsArrivalDetails WHERE ROUND(Quantity - QuantityReceipted, " + (int)GlobalEnums.rndQuantity + ") > 0)" : "GoodsArrivals.EntryDate >= @FromDate AND GoodsArrivals.EntryDate <= @ToDate") + " AND GoodsArrivals.OrganizationalUnitID IN (SELECT AccessControls.OrganizationalUnitID FROM AccessControls INNER JOIN AspNetUsers ON AccessControls.UserID = AspNetUsers.UserID WHERE AspNetUsers.Id = @AspUserID AND AccessControls.NMVNTaskID = @NMVNTaskID AND AccessControls.AccessLevel > 0) AND Locations.LocationID = GoodsArrivals.LocationID " + "\r\n";
            queryString = queryString + "                   INNER JOIN GoodsArrivalDetails ON GoodsArrivals.GoodsArrivalID = GoodsArrivalDetails.GoodsArrivalID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Commodities ON GoodsArrivalDetails.CommodityID = Commodities.CommodityID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Customers ON GoodsArrivals.CustomerID = Customers.CustomerID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Customers Transporters ON GoodsArrivals.TransporterID = Transporters.CustomerID " + "\r\n";
            queryString = queryString + "                   LEFT  JOIN PurchaseOrders ON GoodsArrivals.PurchaseOrderID = PurchaseOrders.PurchaseOrderID " + "\r\n";

            return queryString;
        }

        #region X


        private void GetGoodsArrivalViewDetails()
        {
            string queryString;

            queryString = " @GoodsArrivalID Int " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";
            queryString = queryString + "    BEGIN " + "\r\n";

            queryString = queryString + "       SELECT      GoodsArrivalDetails.GoodsArrivalDetailID, GoodsArrivalDetails.GoodsArrivalID, GoodsArrivalDetails.PurchaseOrderID, GoodsArrivalDetails.PurchaseOrderDetailID, PurchaseOrders.Reference AS PurchaseOrderReference, PurchaseOrders.Code AS PurchaseOrderCode, PurchaseOrders.EntryDate AS PurchaseOrderEntryDate, " + "\r\n";
            queryString = queryString + "                   Commodities.CommodityID, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, Commodities.Shelflife, GoodsArrivalDetails.CommodityTypeID, " + "\r\n";
            queryString = queryString + "                   ROUND(ISNULL(PurchaseOrderDetails.Quantity, 0) - ISNULL(PurchaseOrderDetails.QuantityArrived, 0) + ISNULL(GoodsArrivalSummaries.Quantity, 0), " + (int)GlobalEnums.rndQuantity + ") AS QuantityRemains, " + "\r\n";
            queryString = queryString + "                   GoodsArrivalDetails.Quantity, GoodsArrivalDetails.UnitWeight, GoodsArrivalDetails.TareWeight, GoodsArrivalDetails.Packages, GoodsArrivalDetails.SealCode, GoodsArrivalDetails.BatchCode, GoodsArrivalDetails.LabCode, GoodsArrivalDetails.ProductionDate, GoodsArrivalDetails.ExpiryDate, GoodsArrivalDetails.BatchID, GoodsArrivalDetails.BatchEntryDate, GoodsArrivalDetails.Remarks " + "\r\n";

            queryString = queryString + "       FROM        GoodsArrivalDetails " + "\r\n";
            queryString = queryString + "                   INNER JOIN Commodities ON GoodsArrivalDetails.GoodsArrivalID = @GoodsArrivalID AND GoodsArrivalDetails.CommodityID = Commodities.CommodityID " + "\r\n";

            queryString = queryString + "                   LEFT JOIN PurchaseOrderDetails ON GoodsArrivalDetails.PurchaseOrderDetailID = PurchaseOrderDetails.PurchaseOrderDetailID " + "\r\n";
            queryString = queryString + "                   LEFT JOIN PurchaseOrders ON PurchaseOrderDetails.PurchaseOrderID = PurchaseOrders.PurchaseOrderID " + "\r\n";
            queryString = queryString + "                   LEFT JOIN (SELECT PurchaseOrderDetailID, SUM(Quantity) AS Quantity FROM GoodsArrivalDetails WHERE GoodsArrivalID = @GoodsArrivalID AND NOT PurchaseOrderDetailID IS NULL GROUP BY PurchaseOrderDetailID) AS GoodsArrivalSummaries ON GoodsArrivalDetails.PurchaseOrderDetailID = GoodsArrivalSummaries.PurchaseOrderDetailID " + "\r\n";

            queryString = queryString + "       ORDER BY    GoodsArrivalDetails.SerialID " + "\r\n";

            queryString = queryString + "    END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetGoodsArrivalViewDetails", queryString);
        }





        #region Y

        private void GetGoodsArrivalPendingPurchaseOrders()
        {
            string queryString = " @LocationID int, @NMVNTaskID int " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "       SELECT          PurchaseOrders.PurchaseOrderID, PurchaseOrders.Reference AS PurchaseOrderReference, PurchaseOrders.Code AS PurchaseOrderCode, PurchaseOrders.EntryDate AS PurchaseOrderEntryDate, PurchaseOrders.VoucherDate AS PurchaseOrderVoucherDate, PurchaseOrders.DeliveryDate AS PurchaseOrderDeliveryDate, PurchaseOrders.Caption, PurchaseOrders.Description, PurchaseOrders.Remarks, " + "\r\n";
            queryString = queryString + "                       PurchaseOrders.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, Customers.OfficialName AS CustomerOfficialName, Customers.Birthday AS CustomerBirthday, Customers.VATCode AS CustomerVATCode, Customers.AttentionName AS CustomerAttentionName, Customers.Telephone AS CustomerTelephone, Customers.BillingAddress AS CustomerBillingAddress, Customers.ShippingAddress AS CustomerShippingAddress, " + "\r\n";
            queryString = queryString + "                       PurchaseOrders.TransporterID, Transporters.Code AS TransporterCode, Transporters.Name AS TransporterName, Transporters.OfficialName AS TransporterOfficialName, Transporters.Birthday AS TransporterBirthday, Transporters.VATCode AS TransporterVATCode, Transporters.AttentionName AS TransporterAttentionName, Transporters.Telephone AS TransporterTelephone, Transporters.BillingAddress AS TransporterBillingAddress, Transporters.ShippingAddress AS TransporterShippingAddress, Warehouses.WarehouseID, Warehouses.Code AS WarehouseCode, Warehouses.Name AS WarehouseName " + "\r\n";

            queryString = queryString + "       FROM            PurchaseOrders " + "\r\n";
            queryString = queryString + "                       INNER JOIN Customers ON PurchaseOrders.PurchaseOrderID IN (SELECT PurchaseOrderID FROM PurchaseOrderDetails WHERE LocationID = @LocationID AND NMVNTaskID = @NMVNTaskID - 5 AND Approved = 1 AND InActive = 0 AND InActivePartial = 0  AND ROUND(Quantity - QuantityArrived, " + (int)GlobalEnums.rndQuantity + ") > 0) AND PurchaseOrders.CustomerID = Customers.CustomerID " + "\r\n";
            queryString = queryString + "                       INNER JOIN Customers Transporters ON PurchaseOrders.TransporterID = Transporters.CustomerID " + "\r\n";

            queryString = queryString + "                       INNER JOIN Warehouses ON Warehouses.WarehouseID = " + (GlobalEnums.DMC ? "1" : "IIF(@NMVNTaskID = " + (int)GlobalEnums.NmvnTaskID.MaterialArrival + ", 1, 2) ") + "\r\n";

            queryString = queryString + "       ORDER BY        PurchaseOrders.PurchaseOrderID " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetGoodsArrivalPendingPurchaseOrders", queryString);
        }

        private void GetGoodsArrivalPendingCustomers()
        {
            string queryString = " @LocationID int, @NMVNTaskID int " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "       SELECT          CustomerTransporterPENDING.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, Customers.OfficialName AS CustomerOfficialName, Customers.Birthday AS CustomerBirthday, Customers.VATCode AS CustomerVATCode, Customers.AttentionName AS CustomerAttentionName, Customers.Telephone AS CustomerTelephone, Customers.BillingAddress AS CustomerBillingAddress, Customers.ShippingAddress AS CustomerShippingAddress, " + "\r\n";
            queryString = queryString + "                       CustomerTransporterPENDING.TransporterID, Transporters.Code AS TransporterCode, Transporters.Name AS TransporterName, Transporters.OfficialName AS TransporterOfficialName, Transporters.Birthday AS TransporterBirthday, Transporters.VATCode AS TransporterVATCode, Transporters.AttentionName AS TransporterAttentionName, Transporters.Telephone AS TransporterTelephone, Transporters.BillingAddress AS TransporterBillingAddress, Transporters.ShippingAddress AS TransporterShippingAddress, Warehouses.WarehouseID, Warehouses.Code AS WarehouseCode, Warehouses.Name AS WarehouseName, N'' AS Description, N'' AS Remarks " + "\r\n";

            queryString = queryString + "       FROM           (SELECT DISTINCT CustomerID, TransporterID FROM PurchaseOrders WHERE PurchaseOrderID IN (SELECT PurchaseOrderID FROM PurchaseOrderDetails WHERE LocationID = @LocationID AND NMVNTaskID = @NMVNTaskID - 5 AND Approved = 1 AND InActive = 0 AND InActivePartial = 0  AND ROUND(Quantity - QuantityArrived, " + (int)GlobalEnums.rndQuantity + ") > 0)) CustomerTransporterPENDING " + "\r\n";
            queryString = queryString + "                       INNER JOIN Customers ON CustomerTransporterPENDING.CustomerID = Customers.CustomerID " + "\r\n";
            queryString = queryString + "                       INNER JOIN Customers Transporters ON CustomerTransporterPENDING.TransporterID = Transporters.CustomerID " + "\r\n";

            queryString = queryString + "                       INNER JOIN Warehouses ON Warehouses.WarehouseID = " + (GlobalEnums.DMC ? "1" : "IIF(@NMVNTaskID = " + (int)GlobalEnums.NmvnTaskID.MaterialArrival + ", 1, 2) ") + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetGoodsArrivalPendingCustomers", queryString);
        }



        private void GetGoodsArrivalPendingPurchaseOrderDetails()
        {
            string queryString;

            queryString = " @LocationID Int, @NMVNTaskID int, @GoodsArrivalID Int, @PurchaseOrderID Int, @CustomerID Int, @TransporterID Int, @PurchaseOrderDetailIDs varchar(3999) " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "   BEGIN " + "\r\n";

            queryString = queryString + "       IF  (@PurchaseOrderID <> 0) " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLPurchaseOrder(true) + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLPurchaseOrder(false) + "\r\n";

            queryString = queryString + "   END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetGoodsArrivalPendingPurchaseOrderDetails", queryString);
        }

        private string BuildSQLPurchaseOrder(bool isPurchaseOrderID)
        {
            string queryString = "";
            queryString = queryString + "   BEGIN " + "\r\n";
            queryString = queryString + "       IF  (@PurchaseOrderDetailIDs <> '') " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLPurchaseOrderPurchaseOrderDetailIDs(isPurchaseOrderID, true) + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLPurchaseOrderPurchaseOrderDetailIDs(isPurchaseOrderID, false) + "\r\n";
            queryString = queryString + "   END " + "\r\n";

            return queryString;
        }

        private string BuildSQLPurchaseOrderPurchaseOrderDetailIDs(bool isPurchaseOrderID, bool isPurchaseOrderDetailIDs)
        {
            string queryString = "";
            queryString = queryString + "   BEGIN " + "\r\n";

            queryString = queryString + "       IF (@GoodsArrivalID <= 0) " + "\r\n";
            queryString = queryString + "               BEGIN " + "\r\n";
            queryString = queryString + "                   " + this.BuildSQLNew(isPurchaseOrderID, isPurchaseOrderDetailIDs) + "\r\n";
            queryString = queryString + "                   ORDER BY PurchaseOrders.EntryDate, PurchaseOrders.PurchaseOrderID, PurchaseOrderDetails.PurchaseOrderDetailID " + "\r\n";
            queryString = queryString + "               END " + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "               BEGIN " + "\r\n";
            queryString = queryString + "                   " + this.BuildSQLNew(isPurchaseOrderID, isPurchaseOrderDetailIDs) + " WHERE PurchaseOrderDetails.PurchaseOrderDetailID NOT IN (SELECT PurchaseOrderDetailID FROM GoodsArrivalDetails WHERE GoodsArrivalID = @GoodsArrivalID) " + "\r\n";
            queryString = queryString + "                   UNION ALL " + "\r\n";
            queryString = queryString + "                   " + this.BuildSQLEdit(isPurchaseOrderID, isPurchaseOrderDetailIDs) + "\r\n";
            queryString = queryString + "                   ORDER BY PurchaseOrders.EntryDate, PurchaseOrders.PurchaseOrderID, PurchaseOrderDetails.PurchaseOrderDetailID " + "\r\n";
            queryString = queryString + "               END " + "\r\n";

            queryString = queryString + "   END " + "\r\n";

            return queryString;
        }

        private string BuildSQLNew(bool isPurchaseOrderID, bool isPurchaseOrderDetailIDs)
        {
            string queryString = "";

            queryString = queryString + "       SELECT      PurchaseOrders.PurchaseOrderID, PurchaseOrderDetails.PurchaseOrderDetailID, PurchaseOrders.Reference AS PurchaseOrderReference, PurchaseOrders.Code AS PurchaseOrderCode, PurchaseOrders.EntryDate AS PurchaseOrderEntryDate, " + "\r\n";
            queryString = queryString + "                   Commodities.CommodityID, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, Commodities.Shelflife, Commodities.CommodityTypeID, Commodities.Weight AS UnitWeight, Commodities.TareWeight, " + "\r\n";
            queryString = queryString + "                   ROUND(PurchaseOrderDetails.Quantity - PurchaseOrderDetails.QuantityArrived, " + (int)GlobalEnums.rndQuantity + ") AS QuantityRemains, " + "\r\n";
            queryString = queryString + "                   0.0 AS Quantity, " + (GlobalEnums.CBPP ? "NULL" : "'#'") + " AS SealCode, " + (GlobalEnums.CBPP ? "NULL" : "'#'") + " AS BatchCode, " + (GlobalEnums.CBPP ? "NULL" : "'#'") + " AS LabCode, PurchaseOrderDetails.ProductionDate, PurchaseOrderDetails.ExpiryDate, PurchaseOrders.Description, PurchaseOrderDetails.Remarks, CAST(1 AS bit) AS IsSelected " + "\r\n";

            queryString = queryString + "       FROM        PurchaseOrders " + "\r\n";
            queryString = queryString + "                   INNER JOIN PurchaseOrderDetails ON " + (isPurchaseOrderID ? " PurchaseOrders.PurchaseOrderID = @PurchaseOrderID " : "PurchaseOrders.LocationID = @LocationID AND PurchaseOrders.NMVNTaskID = @NMVNTaskID - 5 AND PurchaseOrders.CustomerID = @CustomerID AND PurchaseOrders.TransporterID = @TransporterID ") + " AND PurchaseOrderDetails.Approved = 1 AND PurchaseOrderDetails.InActive = 0 AND PurchaseOrderDetails.InActivePartial = 0 AND ROUND(PurchaseOrderDetails.Quantity - PurchaseOrderDetails.QuantityArrived, " + (int)GlobalEnums.rndQuantity + ") > 0 AND PurchaseOrders.PurchaseOrderID = PurchaseOrderDetails.PurchaseOrderID" + (isPurchaseOrderDetailIDs ? " AND PurchaseOrderDetails.PurchaseOrderDetailID NOT IN (SELECT Id FROM dbo.SplitToIntList (@PurchaseOrderDetailIDs))" : "") + "\r\n";
            queryString = queryString + "                   INNER JOIN Commodities ON PurchaseOrderDetails.CommodityID = Commodities.CommodityID " + "\r\n";

            return queryString;
        }

        private string BuildSQLEdit(bool isPurchaseOrderID, bool isPurchaseOrderDetailIDs)
        {
            string queryString = "";
            //NO NEED TO UNDO QuantityAvailable -THE WAREHOUSE BALANCE- FOR THIS EDIT QUERY: BECAUSE: THIS STORED PROCEDURE ONLY BE CALLED WHEN Approved = 0 => BECAUSE OF THIS (HAVE NOT UPPROVED YET): THIS DELIVERYADVICE QUANTITY DOES NOT EFFECT THE WAREHOUSE BALANCE
            queryString = queryString + "       SELECT      PurchaseOrders.PurchaseOrderID, PurchaseOrderDetails.PurchaseOrderDetailID, PurchaseOrders.Reference AS PurchaseOrderReference, PurchaseOrders.Code AS PurchaseOrderCode, PurchaseOrders.EntryDate AS PurchaseOrderEntryDate, " + "\r\n";
            queryString = queryString + "                   Commodities.CommodityID, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, Commodities.Shelflife, Commodities.CommodityTypeID, GoodsArrivalDetails.UnitWeight, GoodsArrivalDetails.TareWeight, " + "\r\n";
            queryString = queryString + "                   ROUND(PurchaseOrderDetails.Quantity - PurchaseOrderDetails.QuantityArrived + GoodsArrivalDetails.Quantity, " + (int)GlobalEnums.rndQuantity + ") AS QuantityRemains, " + "\r\n";
            queryString = queryString + "                   0.0 AS Quantity, " + (GlobalEnums.CBPP ? "NULL" : "'#'") + " AS SealCode, " + (GlobalEnums.CBPP ? "NULL" : "'#'") + " AS BatchCode, " + (GlobalEnums.CBPP ? "NULL" : "'#'") + " AS LabCode, GoodsArrivalDetails.ProductionDate, GoodsArrivalDetails.ExpiryDate, PurchaseOrders.Description, PurchaseOrderDetails.Remarks, CAST(1 AS bit) AS IsSelected " + "\r\n";

            queryString = queryString + "       FROM        PurchaseOrderDetails " + "\r\n";
            queryString = queryString + "                   INNER JOIN GoodsArrivalDetails ON GoodsArrivalDetails.GoodsArrivalID = @GoodsArrivalID AND PurchaseOrderDetails.PurchaseOrderDetailID = GoodsArrivalDetails.PurchaseOrderDetailID" + (isPurchaseOrderDetailIDs ? " AND PurchaseOrderDetails.PurchaseOrderDetailID NOT IN (SELECT Id FROM dbo.SplitToIntList (@PurchaseOrderDetailIDs))" : "") + "\r\n";
            queryString = queryString + "                   INNER JOIN Commodities ON PurchaseOrderDetails.CommodityID = Commodities.CommodityID " + "\r\n";
            queryString = queryString + "                   INNER JOIN PurchaseOrders ON PurchaseOrderDetails.PurchaseOrderID = PurchaseOrders.PurchaseOrderID " + "\r\n";

            return queryString;
        }

        #endregion Y




        private void GoodsArrivalSaveRelative()
        {
            string queryString = " @EntityID int, @SaveRelativeOption int " + "\r\n"; //SaveRelativeOption: 1: Update, -1:Undo
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "   DECLARE     @msg NVARCHAR(300) ; " + "\r\n";

            queryString = queryString + "   IF (SELECT HasPurchaseOrder FROM GoodsArrivals WHERE GoodsArrivalID = @EntityID) = 1 " + "\r\n";
            queryString = queryString + "       BEGIN " + "\r\n";

            queryString = queryString + "           DECLARE         @GoodsArrivalSummaries TABLE (PurchaseOrderDetailID int NOT NULL, Quantity decimal(18, 2) NOT NULL) " + "\r\n";
            queryString = queryString + "           INSERT INTO     @GoodsArrivalSummaries (PurchaseOrderDetailID, Quantity) SELECT PurchaseOrderDetailID, SUM(Quantity) AS Quantity FROM GoodsArrivalDetails WHERE GoodsArrivalID = @EntityID GROUP BY PurchaseOrderDetailID " + "\r\n";

            queryString = queryString + "           UPDATE          PurchaseOrderDetails " + "\r\n";
            queryString = queryString + "           SET             PurchaseOrderDetails.QuantityArrived = ROUND(PurchaseOrderDetails.QuantityArrived + GoodsArrivalSummaries.Quantity * @SaveRelativeOption, " + (int)GlobalEnums.rndQuantity + ") " + "\r\n";
            queryString = queryString + "           FROM            @GoodsArrivalSummaries AS GoodsArrivalSummaries " + "\r\n";
            queryString = queryString + "                           INNER JOIN PurchaseOrderDetails ON GoodsArrivalSummaries.PurchaseOrderDetailID = PurchaseOrderDetails.PurchaseOrderDetailID AND ((PurchaseOrderDetails.Approved = 1 AND PurchaseOrderDetails.InActive = 0 AND PurchaseOrderDetails.InActivePartial = 0) OR @SaveRelativeOption = -1) " + "\r\n";

            queryString = queryString + "           IF @@ROWCOUNT <> (SELECT COUNT(*) FROM @GoodsArrivalSummaries) " + "\r\n";
            queryString = queryString + "               BEGIN " + "\r\n";
            queryString = queryString + "                   SET     @msg = N'Đơn hàng không tồn tại, chưa duyệt hoặc đã hủy' ; " + "\r\n";
            queryString = queryString + "                   THROW   61001,  @msg, 1; " + "\r\n";
            queryString = queryString + "               END " + "\r\n";
            queryString = queryString + "       END " + "\r\n";


            queryString = queryString + "       UPDATE      Commodities SET Commodities.TareWeight = GoodsArrivalDetails.TareWeight FROM Commodities INNER JOIN (SELECT CommodityID, MIN(TareWeight) AS TareWeight FROM GoodsArrivalDetails WHERE GoodsArrivalID = @EntityID GROUP BY CommodityID) GoodsArrivalDetails ON Commodities.CommodityID = GoodsArrivalDetails.CommodityID; " + "\r\n";

            queryString = queryString + "       IF (@SaveRelativeOption = 1) " + "\r\n";
            queryString = queryString + "           BEGIN " + "\r\n";
            queryString = queryString + "               INSERT INTO Batches (EntryDate, Code, GoodsArrivalID, GoodsArrivalDetailID) SELECT GoodsArrivals.EntryDate, " + (GlobalEnums.CBPP ? "GoodsArrivalDetails.BatchCode" : "IIF(GoodsArrivalDetails.BatchCode <> '#', GoodsArrivalDetails.BatchCode, GoodsArrivals.Reference + '/' + RIGHT(CAST(YEAR(GoodsArrivals.EntryDate) AS nvarchar), 2))") + ", GoodsArrivals.GoodsArrivalID, GoodsArrivalDetails.GoodsArrivalDetailID FROM GoodsArrivals INNER JOIN GoodsArrivalDetails ON GoodsArrivals.GoodsArrivalID = GoodsArrivalDetails.GoodsArrivalID WHERE GoodsArrivals.GoodsArrivalID = @EntityID AND GoodsArrivalDetails.GoodsArrivalDetailID NOT IN (SELECT GoodsArrivalDetailID FROM Batches WHERE GoodsArrivalID = @EntityID) " + "\r\n";
            queryString = queryString + "               UPDATE Batches SET Batches.EntryDate = GoodsArrivalDetails.EntryDate, Batches.Code = " + (GlobalEnums.CBPP ? "GoodsArrivalDetails.BatchCode" : "IIF(GoodsArrivalDetails.BatchCode <> '#', GoodsArrivalDetails.BatchCode, GoodsArrivals.Reference + '/' + RIGHT(CAST(YEAR(GoodsArrivals.EntryDate) AS nvarchar), 2))") + " FROM GoodsArrivals INNER JOIN GoodsArrivalDetails ON GoodsArrivalDetails.GoodsArrivalID = @EntityID AND GoodsArrivals.GoodsArrivalID = GoodsArrivalDetails.GoodsArrivalID INNER JOIN Batches ON GoodsArrivalDetails.GoodsArrivalDetailID = Batches.GoodsArrivalDetailID " + "\r\n";
            queryString = queryString + "               UPDATE GoodsArrivalDetails SET GoodsArrivalDetails.BatchID = Batches.BatchID, GoodsArrivalDetails.BatchEntryDate = Batches.EntryDate FROM GoodsArrivalDetails INNER JOIN Batches ON GoodsArrivalDetails.GoodsArrivalID = @EntityID AND GoodsArrivalDetails.GoodsArrivalDetailID = Batches.GoodsArrivalDetailID " + "\r\n";
            queryString = queryString + "           END " + "\r\n";


            queryString = queryString + "       IF ((SELECT Approved FROM GoodsArrivals WHERE GoodsArrivalID = @EntityID AND Approved = 1) = 1) " + "\r\n";
            queryString = queryString + "           BEGIN " + "\r\n";
            queryString = queryString + "               UPDATE      GoodsArrivals  SET Approved = 0 WHERE GoodsArrivalID = @EntityID AND Approved = 1" + "\r\n"; //CLEAR APPROVE BEFORE CALL GoodsArrivalToggleApproved
            queryString = queryString + "               IF @@ROWCOUNT = 1 " + "\r\n";
            queryString = queryString + "                   EXEC        GoodsArrivalToggleApproved @EntityID, 1 " + "\r\n";
            queryString = queryString + "               ELSE " + "\r\n";
            queryString = queryString + "                   BEGIN " + "\r\n";
            queryString = queryString + "                       SET     @msg = N'Dữ liệu không tồn tại hoặc đã duyệt'; " + "\r\n";
            queryString = queryString + "                       THROW   61001,  @msg, 1; " + "\r\n";
            queryString = queryString + "                   END " + "\r\n";
            queryString = queryString + "           END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GoodsArrivalSaveRelative", queryString);

        }

        private void GoodsArrivalPostSaveValidate()
        {
            string[] queryArray = new string[2];

            queryArray[0] = " SELECT TOP 1 @FoundEntity = N'Ngày đặt hàng: ' + CAST(PurchaseOrders.EntryDate AS nvarchar) FROM GoodsArrivalDetails INNER JOIN PurchaseOrders ON GoodsArrivalDetails.GoodsArrivalID = @EntityID AND GoodsArrivalDetails.PurchaseOrderID = PurchaseOrders.PurchaseOrderID AND GoodsArrivalDetails.EntryDate < PurchaseOrders.EntryDate ";
            queryArray[1] = " SELECT TOP 1 @FoundEntity = N'Số lượng xuất vượt quá số lượng đặt hàng: ' + CAST(ROUND(Quantity - QuantityArrived, " + (int)GlobalEnums.rndQuantity + ") AS nvarchar) FROM PurchaseOrderDetails WHERE (ROUND((Quantity * 1.1) - QuantityArrived, " + (int)GlobalEnums.rndQuantity + ") < 0) ";

            this.totalSmartPortalEntities.CreateProcedureToCheckExisting("GoodsArrivalPostSaveValidate", queryArray);
        }




        private void GoodsArrivalApproved()
        {
            string[] queryArray = new string[1];

            queryArray[0] = " SELECT TOP 1 @FoundEntity = GoodsArrivalID FROM GoodsArrivals WHERE GoodsArrivalID = @EntityID AND Approved = 1";

            this.totalSmartPortalEntities.CreateProcedureToCheckExisting("GoodsArrivalApproved", queryArray);
        }


        private void GoodsArrivalEditable()
        {
            string[] queryArray = new string[2];

            queryArray[0] = " SELECT TOP 1 @FoundEntity = GoodsArrivalID FROM GoodsArrivals WHERE GoodsArrivalID = @EntityID AND (InActive = 1 OR InActivePartial = 1)"; //Don't allow approve after void
            queryArray[1] = " SELECT TOP 1 @FoundEntity = GoodsArrivalID FROM GoodsReceiptDetails WHERE GoodsArrivalID = @EntityID ";

            this.totalSmartPortalEntities.CreateProcedureToCheckExisting("GoodsArrivalEditable", queryArray);
        }


        private void GoodsArrivalToggleApproved()
        {
            string queryString = " @EntityID int, @Approved bit " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "       UPDATE      GoodsArrivals  SET Approved = @Approved, ApprovedDate = GetDate(), InActive = 0, InActivePartial = 0, InActiveDate = NULL, VoidTypeID = NULL WHERE GoodsArrivalID = @EntityID AND Approved = ~@Approved" + "\r\n";

            queryString = queryString + "       IF @@ROWCOUNT = 1 " + "\r\n";
            queryString = queryString + "           BEGIN " + "\r\n";
            queryString = queryString + "               UPDATE          GoodsArrivalDetails  SET Approved = @Approved, InActive = 0, InActivePartial = 0, InActivePartialDate = NULL, VoidTypeID = NULL WHERE GoodsArrivalID = @EntityID ; " + "\r\n";


            #region INIT GoodsArrivalPackages
            queryString = queryString + "               IF (@Approved = 1) " + "\r\n";
            queryString = queryString + "                   BEGIN " + "\r\n";

            #region INIT Labs
            queryString = queryString + "                       INSERT INTO     Labs (EntryDate, Code, GoodsArrivalID, UserID, PreparedPersonID, OrganizationalUnitID, LocationID, ApproverID, TotalQuantity, Description, Remarks, CreatedDate, EditedDate, Approved, ApprovedDate, VoidTypeID, InActive, InActiveDate, Hold, HoldDate, CommodityCodes, CommodityNames, SealCodes, BatchCodes) " + "\r\n";
            queryString = queryString + "                       SELECT          MIN(GoodsArrivals.EntryDate), GoodsArrivalDetails.LabCode, @EntityID, MIN(GoodsArrivals.UserID), MIN(GoodsArrivals.PreparedPersonID), MIN(GoodsArrivals.OrganizationalUnitID), MIN(GoodsArrivals.LocationID), MIN(GoodsArrivals.ApproverID), SUM(GoodsArrivalDetails.Quantity), MIN(GoodsArrivals.Description), MIN(GoodsArrivals.Remarks), MIN(GoodsArrivals.CreatedDate), MIN(GoodsArrivals.EditedDate), 0 AS Approved, NULL AS ApprovedDate, NULL AS VoidTypeID, 0 AS InActive, NULL AS InActiveDate, 1 AS Hold, MIN(GoodsArrivals.EntryDate) AS HoldDate, " + "\r\n";

            queryString = queryString + "                                       LTRIM(STUFF   ((SELECT ', ' + Commodities.Code " + "\r\n";
            queryString = queryString + "                                       FROM    GoodsArrivalDetails GoodsArrivalCommodities INNER JOIN Commodities ON GoodsArrivalCommodities.CommodityID = Commodities.CommodityID " + "\r\n";
            queryString = queryString + "                                       WHERE   GoodsArrivalCommodities.GoodsArrivalID = @EntityID AND GoodsArrivalCommodities.LabCode = GoodsArrivalDetails.LabCode " + "\r\n";
            queryString = queryString + "                                       FOR XML PATH(''),TYPE).value('(./text())[1]','NVARCHAR(MAX)'),1,1,'')) AS CommodityCodes, " + "\r\n";

            queryString = queryString + "                                       LTRIM(STUFF   ((SELECT ', ' + Commodities.Name " + "\r\n";
            queryString = queryString + "                                       FROM    GoodsArrivalDetails GoodsArrivalCommodities INNER JOIN Commodities ON GoodsArrivalCommodities.CommodityID = Commodities.CommodityID " + "\r\n";
            queryString = queryString + "                                       WHERE   GoodsArrivalCommodities.GoodsArrivalID = @EntityID AND GoodsArrivalCommodities.LabCode = GoodsArrivalDetails.LabCode " + "\r\n";
            queryString = queryString + "                                       FOR XML PATH(''),TYPE).value('(./text())[1]','NVARCHAR(MAX)'),1,1,'')) AS CommodityNames, " + "\r\n";

            queryString = queryString + "                                       LTRIM(STUFF   ((SELECT ', ' + GoodsArrivalSealCodes.SealCode " + "\r\n";
            queryString = queryString + "                                       FROM    GoodsArrivalDetails GoodsArrivalSealCodes " + "\r\n";
            queryString = queryString + "                                       WHERE   GoodsArrivalSealCodes.GoodsArrivalID = @EntityID AND GoodsArrivalSealCodes.LabCode = GoodsArrivalDetails.LabCode " + "\r\n";
            queryString = queryString + "                                       FOR XML PATH(''),TYPE).value('(./text())[1]','NVARCHAR(MAX)'),1,1,'')) AS SealCodes, " + "\r\n";

            queryString = queryString + "                                       LTRIM(STUFF   ((SELECT ', ' + GoodsArrivalBatchCodes.BatchCode " + "\r\n";
            queryString = queryString + "                                       FROM    GoodsArrivalDetails GoodsArrivalBatchCodes " + "\r\n";
            queryString = queryString + "                                       WHERE   GoodsArrivalBatchCodes.GoodsArrivalID = @EntityID AND GoodsArrivalBatchCodes.LabCode = GoodsArrivalDetails.LabCode " + "\r\n";
            queryString = queryString + "                                       FOR XML PATH(''),TYPE).value('(./text())[1]','NVARCHAR(MAX)'),1,1,'')) AS BatchCodes " + "\r\n";

            queryString = queryString + "                       FROM            GoodsArrivals INNER JOIN GoodsArrivalDetails ON GoodsArrivals.GoodsArrivalID = @EntityID AND GoodsArrivals.GoodsArrivalID = GoodsArrivalDetails.GoodsArrivalID " + "\r\n";
            queryString = queryString + "                       GROUP BY        GoodsArrivalDetails.LabCode; " + "\r\n";

            queryString = queryString + "                       UPDATE          GoodsArrivalDetails SET GoodsArrivalDetails.LabID = Labs.LabID " + "\r\n";
            queryString = queryString + "                       FROM            GoodsArrivalDetails INNER JOIN Labs ON GoodsArrivalDetails.GoodsArrivalID = @EntityID AND GoodsArrivalDetails.GoodsArrivalID = Labs.GoodsArrivalID AND GoodsArrivalDetails.LabCode = Labs.Code; " + "\r\n";

            #endregion INIT Labs


            queryString = queryString + "                       DECLARE         @EntryDate datetime, @GoodsArrivalID int, @GoodsArrivalDetailID int, @NMVNTaskID int, @LocationID int, @CustomerID int, @TransporterID int, @PurchaseOrderID int, @PurchaseOrderDetailID int, @CommodityID int, @CommodityCode nvarchar(50), @CommodityTypeID int, @WarehouseID int, @SerialID int, @LabID int, @Code nvarchar(60), @SealCode nvarchar(60), @BatchCode nvarchar(60), @LabCode nvarchar(60), @Barcode nvarchar(60), @ProductionDate datetime, @ExpiryDate datetime, @BatchID int, @BatchEntryDate datetime, @Quantity decimal(18, 2), @QuantityReceipted decimal(18, 2), @UnitWeight decimal(18, 2), @TareWeight decimal(18, 2), @Packages decimal(18, 2), @Remarks nvarchar(100), @VoidTypeID int, @InActive bit, @InActivePartial bit, @InActivePartialDate datetime, @PackageBarcode nvarchar(60), @BarcodeSerialID int; " + "\r\n";

            queryString = queryString + "                       DECLARE         CURSORGoodsArrivalDetails CURSOR LOCAL FOR SELECT GoodsArrivalDetails.EntryDate, GoodsArrivalDetails.GoodsArrivalID, GoodsArrivalDetails.GoodsArrivalDetailID, GoodsArrivalDetails.NMVNTaskID, GoodsArrivalDetails.LocationID, GoodsArrivalDetails.CustomerID, GoodsArrivalDetails.TransporterID, GoodsArrivalDetails.PurchaseOrderID, GoodsArrivalDetails.PurchaseOrderDetailID, GoodsArrivalDetails.CommodityID, Commodities.Code AS CommodityCode, GoodsArrivalDetails.CommodityTypeID, GoodsArrivalDetails.WarehouseID, GoodsArrivalDetails.SerialID, GoodsArrivalDetails.LabID, GoodsArrivalDetails.Code, GoodsArrivalDetails.SealCode, " + (GlobalEnums.CBPP ? "GoodsArrivalDetails.BatchCode" : "Batches.Code") + " AS BatchCode, GoodsArrivalDetails.LabCode, GoodsArrivalDetails.Barcode, GoodsArrivalDetails.ProductionDate, GoodsArrivalDetails.ExpiryDate, GoodsArrivalDetails.BatchID, GoodsArrivalDetails.BatchEntryDate, GoodsArrivalDetails.Quantity, GoodsArrivalDetails.QuantityReceipted, GoodsArrivalDetails.UnitWeight, GoodsArrivalDetails.TareWeight, GoodsArrivalDetails.Packages, GoodsArrivalDetails.Remarks, GoodsArrivalDetails.VoidTypeID, GoodsArrivalDetails.InActive, GoodsArrivalDetails.InActivePartial, GoodsArrivalDetails.InActivePartialDate FROM GoodsArrivalDetails INNER JOIN Batches ON GoodsArrivalDetails.GoodsArrivalID = @EntityID AND GoodsArrivalDetails.GoodsArrivalDetailID = Batches.GoodsArrivalDetailID INNER JOIN Commodities ON GoodsArrivalDetails.CommodityID = Commodities.CommodityID ORDER BY GoodsArrivalDetails.SerialID; " + "\r\n";
            queryString = queryString + "                       OPEN            CURSORGoodsArrivalDetails; " + "\r\n";
            queryString = queryString + "                       FETCH NEXT FROM CURSORGoodsArrivalDetails INTO @EntryDate, @GoodsArrivalID, @GoodsArrivalDetailID, @NMVNTaskID, @LocationID, @CustomerID, @TransporterID, @PurchaseOrderID, @PurchaseOrderDetailID, @CommodityID, @CommodityCode, @CommodityTypeID, @WarehouseID, @SerialID, @LabID, @Code, @SealCode, @BatchCode, @LabCode, @Barcode, @ProductionDate, @ExpiryDate, @BatchID, @BatchEntryDate, @Quantity, @QuantityReceipted, @UnitWeight, @TareWeight, @Packages, @Remarks, @VoidTypeID, @InActive, @InActivePartial, @InActivePartialDate; " + "\r\n";

            queryString = queryString + "                       WHILE @@FETCH_STATUS = 0 " + "\r\n";
            queryString = queryString + "                           BEGIN " + "\r\n";
            queryString = queryString + "                               SET @BarcodeSerialID = ISNULL((SELECT MAX(BarcodeSerialID) FROM GoodsArrivalPackages WHERE CommodityID = @CommodityID AND LabCode = @LabCode), 0) " + "\r\n";

            queryString = queryString + "                               WHILE @Packages > 0 " + "\r\n";
            queryString = queryString + "                                   BEGIN " + "\r\n";

            queryString = queryString + "                                       SET @BarcodeSerialID = @BarcodeSerialID + 1; SET @PackageBarcode = @CommodityCode + @LabCode + ISNULL(@Barcode, '') + RIGHT(CAST(1000 + CAST(@BarcodeSerialID AS INT) AS NVARCHAR), 3) " + "\r\n";

            queryString = queryString + "                                       INSERT INTO GoodsArrivalPackages(EntryDate, GoodsArrivalID, GoodsArrivalDetailID, NMVNTaskID, LocationID, CustomerID, TransporterID, PurchaseOrderID, PurchaseOrderDetailID, CommodityID, CommodityTypeID, WarehouseID, SerialID, LabID, Code, SealCode, BatchCode, LabCode, Barcode, BarcodeSerialID, ProductionDate, ExpiryDate, BatchID, BatchEntryDate, Quantity, QuantityReceipted, UnitWeight, TareWeight, Packages, Remarks, VoidTypeID, Approved, InActive, InActivePartial, InActivePartialDate) " + "\r\n";
            queryString = queryString + "                                       VALUES                          (@EntryDate, @GoodsArrivalID, @GoodsArrivalDetailID, @NMVNTaskID, @LocationID, @CustomerID, @TransporterID, @PurchaseOrderID, @PurchaseOrderDetailID, @CommodityID, @CommodityTypeID, @WarehouseID, @SerialID, @LabID, @Code, @SealCode, @BatchCode, @LabCode, @PackageBarcode, @BarcodeSerialID, @ProductionDate, @ExpiryDate, @BatchID, @BatchEntryDate, @UnitWeight, 0, @UnitWeight, @TareWeight, 1, @Remarks, @VoidTypeID, @Approved, @InActive, @InActivePartial, @InActivePartialDate); " + "\r\n";

            queryString = queryString + "                                       INSERT INTO Barcodes(BarcodeTypeID, Code, GoodsArrivalID, GoodsArrivalDetailID, GoodsArrivalPackageID) " + "\r\n";
            queryString = queryString + "                                       VALUES              (600, @PackageBarcode, @GoodsArrivalID, @GoodsArrivalDetailID, @@IDENTITY); " + "\r\n";

            queryString = queryString + "                                       SET @Packages = @Packages - 1 " + "\r\n";
            queryString = queryString + "                                   END " + "\r\n";

            queryString = queryString + "                               FETCH NEXT FROM CURSORGoodsArrivalDetails INTO @EntryDate, @GoodsArrivalID, @GoodsArrivalDetailID, @NMVNTaskID, @LocationID, @CustomerID, @TransporterID, @PurchaseOrderID, @PurchaseOrderDetailID, @CommodityID, @CommodityCode, @CommodityTypeID, @WarehouseID, @SerialID, @LabID, @Code, @SealCode, @BatchCode, @LabCode, @Barcode, @ProductionDate, @ExpiryDate, @BatchID, @BatchEntryDate, @Quantity, @QuantityReceipted, @UnitWeight, @TareWeight, @Packages, @Remarks, @VoidTypeID, @InActive, @InActivePartial, @InActivePartialDate; " + "\r\n";
            queryString = queryString + "                           END " + "\r\n";

            queryString = queryString + "                   END " + "\r\n";

            queryString = queryString + "               ELSE " + "\r\n";
            queryString = queryString + "                   BEGIN " + "\r\n";
            queryString = queryString + "                       UPDATE          GoodsArrivalDetails SET LabID = NULL WHERE GoodsArrivalID = @EntityID ; " + "\r\n";

            queryString = queryString + "                       DELETE FROM     Barcodes WHERE GoodsArrivalID = @EntityID ; " + "\r\n";
            queryString = queryString + "                       DELETE FROM     GoodsArrivalPackages WHERE GoodsArrivalID = @EntityID ; " + "\r\n";
            queryString = queryString + "                       DELETE FROM     Labs WHERE GoodsArrivalID = @EntityID ; " + "\r\n";
            queryString = queryString + "                   END " + "\r\n";
            #endregion INIT GoodsArrivalPackages



            queryString = queryString + "           END " + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           BEGIN " + "\r\n";
            queryString = queryString + "               DECLARE     @msg NVARCHAR(300) = N'Dữ liệu không tồn tại hoặc đã ' + iif(@Approved = 0, N'hủy', '')  + N' duyệt' ; " + "\r\n";
            queryString = queryString + "               THROW       61001,  @msg, 1; " + "\r\n";
            queryString = queryString + "           END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GoodsArrivalToggleApproved", queryString);
        }

        private void GoodsArrivalChangeExpiryDate()
        {
            string queryString = " @GoodsArrivalID int, @GoodsArrivalDetailID int, @ExpiryDate datetime, @Remarks nvarchar(100) " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "       IF (NOT @Remarks IS NULL) BEGIN SET @Remarks = LTRIM(RTRIM(@Remarks)) IF (@Remarks = '') SET @Remarks = NULL END " + "\r\n";
            queryString = queryString + "       UPDATE      GoodsArrivalDetails     SET ExpiryDate = @ExpiryDate, Remarks = @Remarks WHERE GoodsArrivalID = @GoodsArrivalID AND GoodsArrivalDetailID = @GoodsArrivalDetailID ; " + "\r\n";

            queryString = queryString + "       IF @@ROWCOUNT = 1 " + "\r\n";
            queryString = queryString + "           BEGIN " + "\r\n";
            queryString = queryString + "               DECLARE @BatchID int = (SELECT BatchID FROM GoodsArrivalDetails WHERE GoodsArrivalDetailID = @GoodsArrivalDetailID); " + "\r\n";
            queryString = queryString + "               UPDATE GoodsArrivalPackages SET ExpiryDate = @ExpiryDate WHERE BatchID = @BatchID; " + "\r\n";
            queryString = queryString + "               UPDATE GoodsReceiptDetails  SET ExpiryDate = @ExpiryDate WHERE BatchID = @BatchID; " + "\r\n";
            queryString = queryString + "           END " + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           BEGIN " + "\r\n";
            queryString = queryString + "               DECLARE     @msg NVARCHAR(300) = N'Dữ liệu không tồn tại' ; " + "\r\n";
            queryString = queryString + "               THROW       61001,  @msg, 1; " + "\r\n";
            queryString = queryString + "           END " + "\r\n";
            this.totalSmartPortalEntities.CreateStoredProcedure("GoodsArrivalChangeExpiryDate", queryString);
        }

        private void GetBarcodeBases()
        {
            string queryString = " @GoodsArrivalID int " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "       SELECT          BarcodeID, BarcodeTypeID, Code, GoodsArrivalID, GoodsArrivalDetailID, GoodsArrivalPackageID " + "\r\n";
            queryString = queryString + "       FROM            Barcodes " + "\r\n";
            queryString = queryString + "       WHERE           GoodsArrivalID = @GoodsArrivalID " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetBarcodeBases", queryString);
        }

        private void GetBarcodeSymbologies()
        {
            string queryString = " @BarcodeID int " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "       SELECT          Symbologies " + "\r\n";
            queryString = queryString + "       FROM            Barcodes " + "\r\n";
            queryString = queryString + "       WHERE           BarcodeID = @BarcodeID " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetBarcodeSymbologies", queryString);
        }

        private void SetBarcodeSymbologies()
        {
            string queryString = " @BarcodeID int, @Symbologies nvarchar(MAX) " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "       UPDATE          Barcodes " + "\r\n";
            queryString = queryString + "       SET             Symbologies = @Symbologies " + "\r\n";
            queryString = queryString + "       WHERE           BarcodeID = @BarcodeID " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("SetBarcodeSymbologies", queryString);
        }


        private void GoodsArrivalInitReference()
        {
            SimpleInitReference simpleInitReference = new SimpleInitReference("GoodsArrivals", "GoodsArrivalID", "Reference", ModelSettingManager.ReferenceLength, ModelSettingManager.ReferencePrefix(GlobalEnums.NmvnTaskID.ItemArrival));
            this.totalSmartPortalEntities.CreateTrigger("GoodsArrivalInitReference", simpleInitReference.CreateQuery());
        }


        private void GoodsArrivalSheet()
        {
            string queryString = " @GoodsArrivalID int, @GoodsArrivalPackageID int " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";
            queryString = queryString + "    BEGIN " + "\r\n";

            queryString = queryString + "       DECLARE         @LocalGoodsArrivalID int, @LocalGoodsArrivalPackageID int     SET @LocalGoodsArrivalID = @GoodsArrivalID    SET @LocalGoodsArrivalPackageID = @GoodsArrivalPackageID " + "\r\n";

            queryString = queryString + "       IF  (@LocalGoodsArrivalPackageID > 0) " + "\r\n";
            queryString = queryString + "           " + this.GoodsArrivalSheetSQL(true) + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           " + this.GoodsArrivalSheetSQL(false) + "\r\n";

            queryString = queryString + "    END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GoodsArrivalSheet", queryString);
        }

        private string GoodsArrivalSheetSQL(bool isGoodsArrivalPackageID)
        {
            string queryString = "";
            queryString = queryString + "   BEGIN " + "\r\n";

            queryString = queryString + "       SELECT          GoodsArrivals.GoodsArrivalID, GoodsArrivals.EntryDate, GoodsArrivals.Reference, GoodsArrivals.Code, GoodsArrivals.InvoiceDate, GoodsArrivals.Description, " + "\r\n";
            queryString = queryString + "                       GoodsArrivalPackages.GoodsArrivalPackageID, GoodsArrivalPackages.CommodityID, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, Commodities.OriginalName AS CommodityOriginalName, CommodityCategories.Name AS CommodityCategoryName, Commodities.Weight, Commodities.Origin, Commodities.Shelflife, Commodities.Specification, Commodities.Description AS CommodityDescription, Commodities.Remarks AS CommodityRemarks, Commodities.Caption AS CommodityCaption, CommodityIcons.Icons AS CommodityIcons, " + "\r\n";
            queryString = queryString + "                       GoodsArrivalPackages.SealCode, GoodsArrivalPackages.BatchCode, GoodsArrivalPackages.LabCode, GoodsArrivalPackages.Barcode, Barcodes.Symbologies, GoodsArrivalPackages.ProductionDate, GoodsArrivalPackages.ExpiryDate, GoodsArrivalPackages.Quantity, IIF(GoodsArrivals.CreatedDate < CONVERT(datetime, '2019-02-20 02:00:00', 120), Commodities.Weight, GoodsArrivalPackages.Quantity) AS UnitWeight, GoodsArrivalPackages.TareWeight " + "\r\n";

            queryString = queryString + "       FROM            GoodsArrivals " + "\r\n";
            queryString = queryString + "                       INNER JOIN GoodsArrivalPackages ON " + (isGoodsArrivalPackageID ? "GoodsArrivalPackages.GoodsArrivalPackageID = @LocalGoodsArrivalPackageID" : "GoodsArrivals.GoodsArrivalID = @LocalGoodsArrivalID") + " AND GoodsArrivals.GoodsArrivalID = GoodsArrivalPackages.GoodsArrivalID " + "\r\n";
            queryString = queryString + "                       INNER JOIN Commodities ON GoodsArrivalPackages.CommodityID = Commodities.CommodityID " + "\r\n";
            queryString = queryString + "                       INNER JOIN CommodityCategories ON Commodities.CommodityCategoryID = CommodityCategories.CommodityCategoryID " + "\r\n";

            queryString = queryString + "                       LEFT  JOIN CommodityIcons ON Commodities.CommodityIconID = CommodityIcons.CommodityIconID " + "\r\n";
            queryString = queryString + "                       LEFT  JOIN Barcodes ON GoodsArrivalPackages.GoodsArrivalPackageID = Barcodes.GoodsArrivalPackageID " + "\r\n";

            queryString = queryString + "       ORDER BY        GoodsArrivalPackages.GoodsArrivalDetailID, GoodsArrivalPackages.Barcode " + "\r\n";

            queryString = queryString + "   END " + "\r\n";

            return queryString;
        }

        #endregion
    }
}
