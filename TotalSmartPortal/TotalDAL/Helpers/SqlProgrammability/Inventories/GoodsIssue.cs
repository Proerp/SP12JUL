﻿using TotalBase;
using TotalBase.Enums;
using TotalModel.Models;

namespace TotalDAL.Helpers.SqlProgrammability.Inventories
{

    public class GoodsIssue
    {
        private readonly TotalSmartPortalEntities totalSmartPortalEntities;

        public GoodsIssue(TotalSmartPortalEntities totalSmartPortalEntities)
        {
            this.totalSmartPortalEntities = totalSmartPortalEntities;
        }

        public void RestoreProcedure()
        {
            this.GetGoodsIssueIndexes();

            this.GetPendingDeliveryAdvices();
            this.GetPendingDeliveryAdviceCustomers();
            this.GetPendingDeliveryAdviceDetails();

            this.GetPendingDeliveryAdviceDescriptions();

            this.GetGoodsIssueViewDetails();
            this.GetGoodsIssueViewPackages();

            this.GoodsIssueSaveRelative();
            this.GoodsIssuePostSaveValidate();

            this.GoodsIssueEditable();
            this.GoodsIssueApproved();

            this.GoodsIssueToggleApproved();

            this.GoodsIssueInitReference();

            this.GoodsIssueSheet();
        }

        private void GetGoodsIssueIndexes()
        {
            string queryString;

            queryString = " @AspUserID nvarchar(128), @FromDate DateTime, @ToDate DateTime " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";
            queryString = queryString + "    BEGIN " + "\r\n";

            queryString = queryString + "       SELECT      GoodsIssues.GoodsIssueID, CAST(GoodsIssues.EntryDate AS DATE) AS EntryDate, GoodsIssues.Reference, GoodsIssues.Code AS GoodsIssueCode, Locations.Code AS LocationCode, Customers.Name AS CustomerName, CASE WHEN GoodsIssues.Addressee <> '' OR GoodsIssues.CustomerID <> GoodsIssues.ReceiverID THEN IIF(GoodsIssues.Addressee <> '', GoodsIssues.Addressee, Receivers.Name) ELSE '' END AS ReceiverDescription, GoodsIssues.DeliveryAdviceReferences, GoodsIssues.TotalQuantity, GoodsIssues.TotalFreeQuantity, GoodsIssues.TotalListedGrossAmount, GoodsIssues.TotalGrossAmount, GoodsIssues.Description, GoodsIssues.Approved " + "\r\n";
            queryString = queryString + "       FROM        GoodsIssues " + "\r\n";
            queryString = queryString + "                   INNER JOIN Locations ON GoodsIssues.EntryDate >= @FromDate AND GoodsIssues.EntryDate <= @ToDate AND GoodsIssues.OrganizationalUnitID IN (SELECT AccessControls.OrganizationalUnitID FROM AccessControls INNER JOIN AspNetUsers ON AccessControls.UserID = AspNetUsers.UserID WHERE AspNetUsers.Id = @AspUserID AND AccessControls.NMVNTaskID = " + (int)TotalBase.Enums.GlobalEnums.NmvnTaskID.GoodsIssue + " AND AccessControls.AccessLevel > 0) AND Locations.LocationID = GoodsIssues.LocationID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Customers Customers ON GoodsIssues.CustomerID = Customers.CustomerID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Customers Receivers ON GoodsIssues.ReceiverID = Receivers.CustomerID " + "\r\n";
            queryString = queryString + "       " + "\r\n";

            queryString = queryString + "    END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetGoodsIssueIndexes", queryString);
        }

        private void GetPendingDeliveryAdvices()
        {
            string queryString = " @LocationID int " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";
            queryString = queryString + "       SELECT          DeliveryAdvices.DeliveryAdviceID, DeliveryAdvices.Reference AS DeliveryAdviceReference, DeliveryAdvices.Code AS DeliveryAdviceCode, DeliveryAdvices.EntryDate AS DeliveryAdviceEntryDate, DeliveryAdvices.PaymentTermID, DeliveryAdvices.WarehouseID, Warehouses.Code AS WarehouseCode, Warehouses.Name AS WarehouseName, DeliveryAdvices.ShippingAddress, DeliveryAdvices.Addressee, DeliveryAdvices.TradePromotionID, TradePromotions.Specs AS TradePromotionSpecs, DeliveryAdvices.TradeDiscountRate, DeliveryAdvices.VATPercent, DeliveryAdvices.Description, DeliveryAdvices.Remarks, " + "\r\n";
            queryString = queryString + "                       DeliveryAdvices.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, Customers.VATCode AS CustomerVATCode, Customers.AttentionName AS CustomerAttentionName, Customers.Telephone AS CustomerTelephone, Customers.BillingAddress AS CustomerBillingAddress, CustomerEntireTerritories.EntireName AS CustomerEntireTerritoryEntireName, " + "\r\n";
            queryString = queryString + "                       DeliveryAdvices.ReceiverID, Receivers.Code AS ReceiverCode, Receivers.Name AS ReceiverName, Receivers.VATCode AS ReceiverVATCode, Receivers.AttentionName AS ReceiverAttentionName, Receivers.Telephone AS ReceiverTelephone, Receivers.BillingAddress AS ReceiverBillingAddress, ReceiverEntireTerritories.EntireName AS ReceiverEntireTerritoryEntireName " + "\r\n";

            queryString = queryString + "       FROM            DeliveryAdvices " + "\r\n";
            queryString = queryString + "                       INNER JOIN Customers ON DeliveryAdvices.DeliveryAdviceID IN (SELECT DeliveryAdviceID FROM DeliveryAdviceDetails WHERE LocationID = @LocationID AND Approved = 1 AND InActive = 0 AND InActivePartial = 0 AND InActiveIssue = 0 AND (ROUND(Quantity - QuantityIssue, " + (int)GlobalEnums.rndQuantity + ") > 0 OR ROUND(FreeQuantity - FreeQuantityIssue, " + (int)GlobalEnums.rndQuantity + ") > 0)) AND DeliveryAdvices.CustomerID = Customers.CustomerID " + "\r\n";
            queryString = queryString + "                       INNER JOIN EntireTerritories CustomerEntireTerritories ON Customers.TerritoryID = CustomerEntireTerritories.TerritoryID " + "\r\n";
            queryString = queryString + "                       INNER JOIN Customers Receivers ON DeliveryAdvices.ReceiverID = Receivers.CustomerID " + "\r\n";
            queryString = queryString + "                       INNER JOIN EntireTerritories ReceiverEntireTerritories ON Receivers.TerritoryID = ReceiverEntireTerritories.TerritoryID " + "\r\n";
            queryString = queryString + "                       INNER JOIN Warehouses ON DeliveryAdvices.WarehouseID = Warehouses.WarehouseID " + "\r\n";
            queryString = queryString + "                       LEFT  JOIN Promotions AS TradePromotions ON DeliveryAdvices.TradePromotionID = TradePromotions.PromotionID " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetPendingDeliveryAdvices", queryString);
        }

        private void GetPendingDeliveryAdviceCustomers()
        {
            string queryString = " @LocationID int " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";
            queryString = queryString + "       SELECT          CASE WHEN Customers.PaymentTermID <> -1 THEN Customers.PaymentTermID ELSE CustomerCategories.PaymentTermID END AS PaymentTermID, CustomerReceiverPENDING.WarehouseID, Warehouses.Code AS WarehouseCode, Warehouses.Name AS WarehouseName, CustomerReceiverPENDING.ShippingAddress, CustomerReceiverPENDING.Addressee, CustomerReceiverPENDING.TradePromotionID, TradePromotions.Specs AS TradePromotionSpecs, CustomerReceiverPENDING.TradeDiscountRate, CustomerReceiverPENDING.VATPercent, " + "\r\n";
            queryString = queryString + "                       Customers.CustomerID AS CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, Customers.VATCode AS CustomerVATCode, Customers.AttentionName AS CustomerAttentionName, Customers.Telephone AS CustomerTelephone, Customers.BillingAddress AS CustomerBillingAddress, CustomerEntireTerritories.EntireName AS CustomerEntireTerritoryEntireName, " + "\r\n";
            queryString = queryString + "                       Receivers.CustomerID AS ReceiverID, Receivers.Code AS ReceiverCode, Receivers.Name AS ReceiverName, Receivers.VATCode AS ReceiverVATCode, Receivers.AttentionName AS ReceiverAttentionName, Receivers.Telephone AS ReceiverTelephone, Receivers.BillingAddress AS ReceiverBillingAddress, ReceiverEntireTerritories.EntireName AS ReceiverEntireTerritoryEntireName " + "\r\n";

            queryString = queryString + "       FROM           (SELECT DISTINCT CustomerID, ReceiverID, WarehouseID, ShippingAddress, Addressee, TradePromotionID, TradeDiscountRate, " + (GlobalEnums.VATbyRow ? "0.0 AS" : "") + " VATPercent FROM DeliveryAdvices WHERE DeliveryAdviceID IN (SELECT DeliveryAdviceID FROM DeliveryAdviceDetails WHERE LocationID = @LocationID AND Approved = 1 AND InActive = 0 AND InActivePartial = 0 AND InActiveIssue = 0 AND (ROUND(Quantity - QuantityIssue, " + (int)GlobalEnums.rndQuantity + ") > 0 OR ROUND(FreeQuantity - FreeQuantityIssue, " + (int)GlobalEnums.rndQuantity + ") > 0))) CustomerReceiverPENDING " + "\r\n";
            queryString = queryString + "                       INNER JOIN Customers ON CustomerReceiverPENDING.CustomerID = Customers.CustomerID " + "\r\n";
            queryString = queryString + "                       INNER JOIN Customers Receivers ON CustomerReceiverPENDING.ReceiverID = Receivers.CustomerID " + "\r\n";
            queryString = queryString + "                       INNER JOIN CustomerCategories ON Customers.CustomerCategoryID = CustomerCategories.CustomerCategoryID " + "\r\n";
            queryString = queryString + "                       INNER JOIN EntireTerritories CustomerEntireTerritories ON Customers.TerritoryID = CustomerEntireTerritories.TerritoryID " + "\r\n";
            queryString = queryString + "                       INNER JOIN EntireTerritories ReceiverEntireTerritories ON Receivers.TerritoryID = ReceiverEntireTerritories.TerritoryID " + "\r\n";
            queryString = queryString + "                       INNER JOIN Warehouses ON CustomerReceiverPENDING.WarehouseID = Warehouses.WarehouseID " + "\r\n";
            queryString = queryString + "                       LEFT  JOIN Promotions AS TradePromotions ON CustomerReceiverPENDING.TradePromotionID = TradePromotions.PromotionID " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetPendingDeliveryAdviceCustomers", queryString);
        }

        private void GetPendingDeliveryAdviceDescriptions()
        {
            string queryString = " @LocationID Int, @CustomerID Int, @ReceiverID Int, @WarehouseID Int, @ShippingAddress nvarchar(200), @Addressee nvarchar(200), @TradePromotionID int, @VATPercent decimal(18, 2) " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "       SELECT DeliveryAdviceID, Reference, Description, Remarks FROM DeliveryAdvices WHERE CustomerID = @CustomerID AND ReceiverID = @ReceiverID AND WarehouseID = @WarehouseID AND ShippingAddress = @ShippingAddress AND Addressee = @Addressee AND (TradePromotionID = @TradePromotionID OR (TradePromotionID IS NULL AND @TradePromotionID IS NULL)) " + (GlobalEnums.VATbyRow ? "" : "AND VATPercent = @VATPercent") + " AND DeliveryAdviceID IN (SELECT DeliveryAdviceID FROM DeliveryAdviceDetails WHERE LocationID = @LocationID AND Approved = 1 AND InActive = 0 AND InActivePartial = 0 AND InActiveIssue = 0 AND (ROUND(Quantity - QuantityIssue, " + (int)GlobalEnums.rndQuantity + ") > 0 OR ROUND(FreeQuantity - FreeQuantityIssue, " + (int)GlobalEnums.rndQuantity + ") > 0)) " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetPendingDeliveryAdviceDescriptions", queryString);
        }

        #region GetGoodsIssueViewDetails

        private void GetGoodsIssueViewDetails()
        {
            string queryString;

            SqlProgrammability.Inventories.Inventories inventories = new Inventories(this.totalSmartPortalEntities);
            SqlProgrammability.Reports.InventoryReports inventoryReports = new Reports.InventoryReports(this.totalSmartPortalEntities);

            queryString = " @GoodsIssueID Int, @LocationID Int, @DeliveryAdviceID Int, @CustomerID Int, @ReceiverID Int, @WarehouseID Int, @ShippingAddress nvarchar(200), @Addressee nvarchar(200), @TradePromotionID int, @VATPercent decimal(18, 2), @IsReadonly bit " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "   BEGIN " + "\r\n";

            queryString = queryString + "       DECLARE     @EntryDate DateTime       DECLARE @WarehouseIDList varchar(35)         DECLARE @CommodityIDList varchar(3999)           DECLARE @WarehouseClassList varchar(555) " + "\r\n";

            queryString = queryString + "       IF (@GoodsIssueID > 0) ";
            queryString = queryString + "           SELECT  @EntryDate = EntryDate FROM GoodsIssues WHERE GoodsIssueID = @GoodsIssueID " + "\r\n";
            queryString = queryString + "       IF (@EntryDate IS NULL) ";
            queryString = queryString + "           SELECT  @EntryDate = GetDate() " + "\r\n";

            queryString = queryString + "       IF (@DeliveryAdviceID > 0) ";
            queryString = queryString + "           BEGIN " + "\r\n";
            queryString = queryString + "               SELECT      @WarehouseIDList = STUFF((SELECT ',' + CAST(WarehouseID AS varchar)  FROM (SELECT DISTINCT WarehouseID FROM DeliveryAdviceDetails WHERE DeliveryAdviceID = @DeliveryAdviceID) DistinctWarehouses FOR XML PATH('')) ,1,1,'') " + "\r\n";
            queryString = queryString + "               SELECT      @CommodityIDList = STUFF((SELECT ',' + CAST(CommodityID AS varchar)  FROM DeliveryAdviceDetails WHERE DeliveryAdviceID = @DeliveryAdviceID FOR XML PATH('')) ,1,1,'') " + "\r\n";
            queryString = queryString + "           END " + "\r\n";
            queryString = queryString + "       ELSE ";
            queryString = queryString + "           BEGIN " + "\r\n";
            queryString = queryString + "               DECLARE     @WarehouseCommodities TABLE (WarehouseID int NOT NULL, CommodityID int NOT NULL) " + "\r\n";
            queryString = queryString + "               INSERT INTO @WarehouseCommodities       SELECT      DeliveryAdviceDetails.WarehouseID, DeliveryAdviceDetails.CommodityID        FROM    DeliveryAdviceDetails INNER JOIN DeliveryAdvices ON DeliveryAdviceDetails.LocationID = @LocationID AND DeliveryAdviceDetails.CustomerID = @CustomerID AND DeliveryAdviceDetails.ReceiverID = @ReceiverID AND DeliveryAdviceDetails.WarehouseID = @WarehouseID AND DeliveryAdvices.ShippingAddress = @ShippingAddress AND DeliveryAdvices.Addressee = @Addressee AND (DeliveryAdvices.TradePromotionID = @TradePromotionID OR (DeliveryAdvices.TradePromotionID IS NULL AND @TradePromotionID IS NULL)) " + (GlobalEnums.VATbyRow ? "" : "AND DeliveryAdvices.VATPercent = @VATPercent") + " AND DeliveryAdviceDetails.Approved = 1 AND DeliveryAdviceDetails.InActive = 0 AND DeliveryAdviceDetails.InActivePartial = 0 AND DeliveryAdviceDetails.InActiveIssue = 0 AND (ROUND(DeliveryAdviceDetails.Quantity - DeliveryAdviceDetails.QuantityIssue, " + (int)GlobalEnums.rndQuantity + ") > 0 OR ROUND(DeliveryAdviceDetails.FreeQuantity - DeliveryAdviceDetails.FreeQuantityIssue, " + (int)GlobalEnums.rndQuantity + ") > 0) AND DeliveryAdviceDetails.DeliveryAdviceID = DeliveryAdvices.DeliveryAdviceID " + "\r\n";
            queryString = queryString + "               INSERT INTO @WarehouseCommodities       SELECT      GoodsIssueDetails.WarehouseID, GoodsIssueDetails.CommodityID                FROM    GoodsIssueDetails WHERE GoodsIssueID = @GoodsIssueID ";

            queryString = queryString + "               SELECT      @WarehouseIDList = STUFF((SELECT ',' + CAST(WarehouseID AS varchar)  FROM (SELECT DISTINCT WarehouseID FROM @WarehouseCommodities) PendingWarehouses FOR XML PATH('')) ,1,1,'') " + "\r\n";
            queryString = queryString + "               SELECT      @CommodityIDList = STUFF((SELECT ',' + CAST(CommodityID AS varchar)  FROM (SELECT DISTINCT CommodityID FROM @WarehouseCommodities) PendingCommodities FOR XML PATH('')) ,1,1,'') " + "\r\n";
            queryString = queryString + "           END " + "\r\n";

            queryString = queryString + "       SELECT      @WarehouseClassList = STUFF((SELECT ',' + CAST(WarehouseClassID AS varchar) FROM Warehouses WHERE WarehouseID IN (SELECT * FROM FNSplitUpIds(@WarehouseIDList))        FOR XML PATH('')) ,1,1,'') " + "\r\n";

            if (GlobalEnums.SKUWarehouse)
                queryString = queryString + "   " + inventories.GET_WarehouseJournal_BUILD_SQL("@CommoditiesBalance", "@EntryDate", "@EntryDate", "@WarehouseIDList", "@CommodityIDList", "0", "0", "@WarehouseClassList", null) + "\r\n";
            else
                queryString = queryString + "   " + inventoryReports.GET_WarehouseCard_BUILD_SQL("@CommoditiesBalance", "@WarehouseID", "@EntryDate", "@EntryDate") + "\r\n";

            queryString = queryString + "       IF (@DeliveryAdviceID > 0) ";
            queryString = queryString + "           " + this.BuildSQL(true);
            queryString = queryString + "       ELSE ";
            queryString = queryString + "           " + this.BuildSQL(false);

            queryString = queryString + "   END " + "\r\n";

            //System.Diagnostics.Debug.WriteLine("---");
            //System.Diagnostics.Debug.WriteLine(queryString);

            this.totalSmartPortalEntities.CreateStoredProcedure("GetGoodsIssueViewDetails", queryString);

        }

        private string BuildSQL(bool isDeliveryAdviceID)
        {
            string queryString = "";

            queryString = queryString + "   BEGIN " + "\r\n";
            queryString = queryString + "       IF (@GoodsIssueID <= 0) " + "\r\n";
            queryString = queryString + "           BEGIN " + "\r\n";
            queryString = queryString + "               " + this.BuildSQLNew(isDeliveryAdviceID) + "\r\n";
            queryString = queryString + "               ORDER BY Commodities.CommodityTypeID, DeliveryAdviceDetails.DeliveryAdviceID, DeliveryAdviceDetails.DeliveryAdviceDetailID " + "\r\n";
            queryString = queryString + "           END " + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";

            queryString = queryString + "           IF (@IsReadonly = 1) " + "\r\n";
            queryString = queryString + "               BEGIN " + "\r\n";
            queryString = queryString + "                   " + this.BuildSQLEdit(isDeliveryAdviceID) + "\r\n";
            queryString = queryString + "                   ORDER BY Commodities.CommodityTypeID, DeliveryAdviceDetails.DeliveryAdviceID, DeliveryAdviceDetails.DeliveryAdviceDetailID " + "\r\n";
            queryString = queryString + "               END " + "\r\n";

            queryString = queryString + "           ELSE " + "\r\n"; //FULL SELECT FOR EDIT MODE

            queryString = queryString + "               BEGIN " + "\r\n";
            queryString = queryString + "                   " + this.BuildSQLNew(isDeliveryAdviceID) + " WHERE DeliveryAdviceDetails.DeliveryAdviceDetailID NOT IN (SELECT DeliveryAdviceDetailID FROM GoodsIssueDetails WHERE GoodsIssueID = @GoodsIssueID) " + "\r\n";
            queryString = queryString + "                   UNION ALL " + "\r\n";
            queryString = queryString + "                   " + this.BuildSQLEdit(isDeliveryAdviceID) + "\r\n";
            queryString = queryString + "                   ORDER BY Commodities.CommodityTypeID, DeliveryAdviceDetails.DeliveryAdviceID, DeliveryAdviceDetails.DeliveryAdviceDetailID " + "\r\n";
            queryString = queryString + "               END " + "\r\n";

            queryString = queryString + "   END " + "\r\n";

            return queryString;
        }

        private string BuildSQLNew(bool isDeliveryAdviceID)
        {
            string queryString;

            queryString = "                     SELECT          DeliveryAdviceDetails.DeliveryAdviceID, DeliveryAdvices.Reference AS DeliveryAdviceReference, DeliveryAdvices.Code AS DeliveryAdviceCode, DeliveryAdviceDetails.EntryDate AS DeliveryAdviceEntryDate, 0 AS GoodsIssueDetailID, 0 AS GoodsIssueID, DeliveryAdviceDetails.DeliveryAdviceDetailID, NULL AS VoidTypeID, CAST(NULL AS nvarchar(50)) AS VoidTypeCode, CAST(NULL AS nvarchar(50)) AS VoidTypeName, NULL AS VoidClassID, " + "\r\n";
            queryString = queryString + "                       DeliveryAdviceDetails.CommodityID, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, Commodities.CommodityTypeID, Warehouses.WarehouseID, Warehouses.Code AS WarehouseCode, DeliveryAdviceDetails.CalculatingTypeID, DeliveryAdviceDetails.VATbyRow, ROUND(DeliveryAdviceDetails.Quantity - DeliveryAdviceDetails.QuantityIssue, 0) AS QuantityRemains, ROUND(DeliveryAdviceDetails.FreeQuantity - DeliveryAdviceDetails.FreeQuantityIssue, 0) AS FreeQuantityRemains,  " + "\r\n";
            queryString = queryString + "                       IIF(Commodities.CommodityTypeID = " + (int)GlobalEnums.CommodityTypeID.Services + ", DeliveryAdviceDetails.Quantity + DeliveryAdviceDetails.FreeQuantity, ROUND(ISNULL(CommoditiesBalance.QuantityBalance, 0) + CASE WHEN DeliveryAdviceDetails.Approved = 1 AND DeliveryAdviceDetails.InActive = 0 AND DeliveryAdviceDetails.InActivePartial = 0 AND DeliveryAdviceDetails.InActiveIssue = 0 THEN DeliveryAdviceDetails.Quantity + DeliveryAdviceDetails.FreeQuantity - DeliveryAdviceDetails.QuantityIssue - DeliveryAdviceDetails.FreeQuantityIssue ELSE 0 END, 0)) AS QuantityAvailable, DeliveryAdviceDetails.ControlFreeQuantity, " + "\r\n";
            queryString = queryString + "                       0.0 AS Quantity, 0.0 AS FreeQuantity, DeliveryAdviceDetails.ListedPrice, DeliveryAdviceDetails.DiscountPercent, DeliveryAdviceDetails.UnitPrice, DeliveryAdviceDetails.TradeDiscountRate, DeliveryAdviceDetails.VATPercent, DeliveryAdviceDetails.ListedGrossPrice, DeliveryAdviceDetails.GrossPrice, 0.0 AS ListedAmount, 0.0 AS Amount, 0.0 AS ListedVATAmount, 0.0 AS VATAmount, 0.0 AS ListedGrossAmount, 0.0 AS GrossAmount, DeliveryAdviceDetails.IsBonus, DeliveryAdviceDetails.Remarks " + "\r\n";

            queryString = queryString + "       FROM            DeliveryAdviceDetails " + "\r\n";
            queryString = queryString + "                       INNER JOIN " + (isDeliveryAdviceID ? "Commodities" : "DeliveryAdvices") + " ON " + (isDeliveryAdviceID ? "DeliveryAdviceDetails.DeliveryAdviceID = @DeliveryAdviceID" : "DeliveryAdviceDetails.LocationID = @LocationID AND DeliveryAdviceDetails.CustomerID = @CustomerID AND DeliveryAdviceDetails.ReceiverID = @ReceiverID AND DeliveryAdviceDetails.WarehouseID = @WarehouseID AND DeliveryAdvices.ShippingAddress = @ShippingAddress AND DeliveryAdvices.Addressee = @Addressee AND (DeliveryAdvices.TradePromotionID = @TradePromotionID OR (DeliveryAdvices.TradePromotionID IS NULL AND @TradePromotionID IS NULL)) " + (GlobalEnums.VATbyRow ? "" : "AND DeliveryAdvices.VATPercent = @VATPercent")) + " AND DeliveryAdviceDetails.Approved = 1 AND DeliveryAdviceDetails.InActive = 0 AND DeliveryAdviceDetails.InActivePartial = 0 AND DeliveryAdviceDetails.InActiveIssue = 0 AND (ROUND(DeliveryAdviceDetails.Quantity - DeliveryAdviceDetails.QuantityIssue, " + (int)GlobalEnums.rndQuantity + ") > 0 OR ROUND(DeliveryAdviceDetails.FreeQuantity - DeliveryAdviceDetails.FreeQuantityIssue, " + (int)GlobalEnums.rndQuantity + ") > 0) AND " + (isDeliveryAdviceID ? "DeliveryAdviceDetails.CommodityID = Commodities.CommodityID" : "DeliveryAdviceDetails.DeliveryAdviceID = DeliveryAdvices.DeliveryAdviceID") + "\r\n";
            queryString = queryString + "                       " + (isDeliveryAdviceID ? "" : "INNER JOIN Commodities ON DeliveryAdviceDetails.CommodityID = Commodities.CommodityID ") + "\r\n";
            queryString = queryString + "                       INNER JOIN Warehouses ON DeliveryAdviceDetails.WarehouseID = Warehouses.WarehouseID " + "\r\n";
            queryString = queryString + "                       " + (isDeliveryAdviceID ? "INNER JOIN DeliveryAdvices ON DeliveryAdviceDetails.DeliveryAdviceID = DeliveryAdvices.DeliveryAdviceID " : "") + "\r\n";
            queryString = queryString + "                       LEFT JOIN @CommoditiesBalance CommoditiesBalance ON DeliveryAdviceDetails.WarehouseID = CommoditiesBalance.WarehouseID AND DeliveryAdviceDetails.CommodityID = CommoditiesBalance.CommodityID " + "\r\n";

            return queryString;
        }

        private string BuildSQLEdit(bool isDeliveryAdviceID)
        {
            string queryString;

            queryString = "                     SELECT          DeliveryAdviceDetails.DeliveryAdviceID, DeliveryAdvices.Reference AS DeliveryAdviceReference, DeliveryAdvices.Code AS DeliveryAdviceCode, DeliveryAdviceDetails.EntryDate AS DeliveryAdviceEntryDate, GoodsIssueDetails.GoodsIssueDetailID, GoodsIssueDetails.GoodsIssueID, DeliveryAdviceDetails.DeliveryAdviceDetailID, VoidTypes.VoidTypeID, VoidTypes.Code AS VoidTypeCode, VoidTypes.Name AS VoidTypeName, VoidTypes.VoidClassID, " + "\r\n";
            queryString = queryString + "                       DeliveryAdviceDetails.CommodityID, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, Commodities.CommodityTypeID, Warehouses.WarehouseID, Warehouses.Code AS WarehouseCode, GoodsIssueDetails.CalculatingTypeID, GoodsIssueDetails.VATbyRow, ROUND(DeliveryAdviceDetails.Quantity - DeliveryAdviceDetails.QuantityIssue + GoodsIssueDetails.Quantity, 0) AS QuantityRemains, ROUND(DeliveryAdviceDetails.FreeQuantity - DeliveryAdviceDetails.FreeQuantityIssue + GoodsIssueDetails.FreeQuantity, 0) AS FreeQuantityRemains, " + "\r\n";
            queryString = queryString + "                       IIF(Commodities.CommodityTypeID = " + (int)GlobalEnums.CommodityTypeID.Services + ", DeliveryAdviceDetails.Quantity + DeliveryAdviceDetails.FreeQuantity, ROUND(ISNULL(CommoditiesBalance.QuantityBalance, 0) + GoodsIssueDetails.Quantity  + GoodsIssueDetails.FreeQuantity + CASE WHEN DeliveryAdviceDetails.Approved = 1 AND DeliveryAdviceDetails.InActive = 0 AND DeliveryAdviceDetails.InActivePartial = 0 AND DeliveryAdviceDetails.InActiveIssue = 0 THEN DeliveryAdviceDetails.Quantity + DeliveryAdviceDetails.FreeQuantity - DeliveryAdviceDetails.QuantityIssue - DeliveryAdviceDetails.FreeQuantityIssue ELSE 0 END, 0)) AS QuantityAvailable, DeliveryAdviceDetails.ControlFreeQuantity, " + "\r\n";
            queryString = queryString + "                       GoodsIssueDetails.Quantity, GoodsIssueDetails.FreeQuantity, GoodsIssueDetails.ListedPrice, GoodsIssueDetails.DiscountPercent, GoodsIssueDetails.UnitPrice, GoodsIssueDetails.TradeDiscountRate, GoodsIssueDetails.VATPercent, GoodsIssueDetails.ListedGrossPrice, GoodsIssueDetails.GrossPrice, GoodsIssueDetails.ListedAmount, GoodsIssueDetails.Amount, GoodsIssueDetails.ListedVATAmount, GoodsIssueDetails.VATAmount, GoodsIssueDetails.ListedGrossAmount, GoodsIssueDetails.GrossAmount, GoodsIssueDetails.IsBonus, GoodsIssueDetails.Remarks " + "\r\n";

            queryString = queryString + "       FROM            GoodsIssueDetails " + "\r\n";
            queryString = queryString + "                       INNER JOIN DeliveryAdviceDetails ON GoodsIssueDetails.GoodsIssueID = @GoodsIssueID AND GoodsIssueDetails.DeliveryAdviceDetailID = DeliveryAdviceDetails.DeliveryAdviceDetailID " + "\r\n";
            queryString = queryString + "                       INNER JOIN Commodities ON GoodsIssueDetails.CommodityID = Commodities.CommodityID " + "\r\n";
            queryString = queryString + "                       INNER JOIN Warehouses ON DeliveryAdviceDetails.WarehouseID = Warehouses.WarehouseID " + "\r\n";
            queryString = queryString + "                       INNER JOIN DeliveryAdvices ON DeliveryAdviceDetails.DeliveryAdviceID = DeliveryAdvices.DeliveryAdviceID " + "\r\n";
            queryString = queryString + "                       LEFT JOIN VoidTypes ON GoodsIssueDetails.VoidTypeID = VoidTypes.VoidTypeID " + "\r\n";
            queryString = queryString + "                       LEFT JOIN @CommoditiesBalance CommoditiesBalance ON DeliveryAdviceDetails.WarehouseID = CommoditiesBalance.WarehouseID AND DeliveryAdviceDetails.CommodityID = CommoditiesBalance.CommodityID " + "\r\n";

            return queryString;
        }

        #endregion GetGoodsIssueViewDetails

        #region GetGoodsIssueViewPackages

        private void GetGoodsIssueViewPackages()
        {
            string queryString;

            queryString = " @GoodsIssueID Int " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";
            queryString = queryString + "    BEGIN " + "\r\n";

            queryString = queryString + "       SELECT      GoodsIssuePackages.GoodsIssuePackageID, GoodsIssuePackages.GoodsIssueID, DeliveryAdviceDetails.DeliveryAdviceID, DeliveryAdviceDetails.DeliveryAdviceDetailID, Commodities.CommodityID, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, Commodities.CommodityTypeID, " + "\r\n";
            queryString = queryString + "                   GoodsReceiptDetails.GoodsReceiptID, GoodsReceiptDetails.GoodsReceiptDetailID, GoodsReceiptDetails.Reference AS GoodsReceiptReference, GoodsReceiptDetails.Code AS GoodsReceiptCode, GoodsReceiptDetails.EntryDate AS GoodsReceiptEntryDate, GoodsReceiptDetails.ExpiryDate, GoodsReceiptDetails.WarehouseID, GoodsReceiptDetails.BatchID, GoodsReceiptDetails.BatchEntryDate, GoodsIssuePackages.BinLocationID, BinLocations.Code AS BinLocationCode, GoodsIssuePackages.Barcode, GoodsIssuePackages.BatchCode, GoodsIssuePackages.SealCode, GoodsIssuePackages.LabCode, GoodsReceiptDetails.UnitWeight, GoodsReceiptDetails.TareWeight, " + "\r\n";
            queryString = queryString + "                   ROUND(DeliveryAdviceDetails.Quantity - DeliveryAdviceDetails.QuantityIssue + Issued_DeliveryAdviceDetails.QuantityIssued, " + (int)GlobalEnums.rndQuantity + ") AS QuantityRemains, ROUND(GoodsReceiptDetails.Quantity - GoodsReceiptDetails.QuantityIssued + Issued_GoodsReceiptDetails.QuantityIssued, " + (int)GlobalEnums.rndQuantity + ") AS QuantityAvailables, GoodsIssuePackages.Quantity, GoodsIssuePackages.Remarks " + "\r\n";

            queryString = queryString + "       FROM        GoodsIssuePackages " + "\r\n";
            queryString = queryString + "                   INNER JOIN DeliveryAdviceDetails ON GoodsIssuePackages.GoodsIssueID = @GoodsIssueID AND GoodsIssuePackages.DeliveryAdviceDetailID = DeliveryAdviceDetails.DeliveryAdviceDetailID " + "\r\n";
            queryString = queryString + "                   INNER JOIN (SELECT DeliveryAdviceDetailID, SUM(Quantity) AS QuantityIssued FROM GoodsIssuePackages WHERE GoodsIssueID = @GoodsIssueID GROUP BY DeliveryAdviceDetailID) AS Issued_DeliveryAdviceDetails ON GoodsIssuePackages.DeliveryAdviceDetailID = Issued_DeliveryAdviceDetails.DeliveryAdviceDetailID " + "\r\n";

            queryString = queryString + "                   INNER JOIN Commodities ON GoodsIssuePackages.CommodityID = Commodities.CommodityID " + "\r\n";
            queryString = queryString + "                   INNER JOIN BinLocations ON GoodsIssuePackages.BinLocationID = BinLocations.BinLocationID " + "\r\n";

            queryString = queryString + "                   INNER JOIN GoodsReceiptDetails ON GoodsIssuePackages.GoodsReceiptDetailID = GoodsReceiptDetails.GoodsReceiptDetailID " + "\r\n";
            queryString = queryString + "                   INNER JOIN (SELECT GoodsReceiptDetailID, SUM(Quantity) AS QuantityIssued FROM GoodsIssuePackages WHERE GoodsIssueID = @GoodsIssueID GROUP BY GoodsReceiptDetailID) AS Issued_GoodsReceiptDetails ON GoodsIssuePackages.GoodsReceiptDetailID = Issued_GoodsReceiptDetails.GoodsReceiptDetailID " + "\r\n";

            queryString = queryString + "       ORDER BY    GoodsIssuePackages.GoodsIssueID, GoodsIssuePackages.GoodsIssuePackageID " + "\r\n";

            queryString = queryString + "    END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetGoodsIssueViewPackages", queryString);
        }


        private void GetPendingDeliveryAdviceDetails()
        {
            string queryString;

            queryString = " @WebAPI bit, @LocationID Int, @GoodsIssueID Int, @DeliveryAdviceDetailID Int, @WarehouseID Int, @Barcode nvarchar(60), @GoodsReceiptDetailIDs varchar(3999) " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "   BEGIN " + "\r\n";
            queryString = queryString + "       IF  (NOT @Barcode IS NULL AND @Barcode <> '' AND @Barcode <> '0') " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLPendingDetails(true) + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLPendingDetails(false) + "\r\n";

            queryString = queryString + "   END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetPendingDeliveryAdviceDetails", queryString);
        }

        private string BuildSQLPendingDetails(bool isBarcode)
        {
            string queryString = "";
            queryString = queryString + "   BEGIN " + "\r\n";

            queryString = queryString + "       IF  (@GoodsReceiptDetailIDs <> '' AND @GoodsReceiptDetailIDs <> '0') " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLPendingDetails(isBarcode, true) + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           " + this.BuildSQLPendingDetails(isBarcode, false) + "\r\n";

            queryString = queryString + "   END " + "\r\n";

            return queryString;
        }

        private string BuildSQLPendingDetails(bool isBarcode, bool isGoodsReceiptDetailIDs)
        {
            string queryString = "";
            queryString = queryString + "   BEGIN " + "\r\n";

            queryString = queryString + "       IF (@GoodsIssueID <= 0 OR @WebAPI = 1) " + "\r\n"; //@WebAPI = 1: SHOULD SAVE BEFORE CALL NEXT
            queryString = queryString + "               BEGIN " + "\r\n";
            queryString = queryString + "                   " + this.BuildSQLNew(isBarcode, isGoodsReceiptDetailIDs) + "\r\n";
            queryString = queryString + "                   ORDER BY DeliveryAdviceDetails.DeliveryAdviceDetailID " + "\r\n";
            queryString = queryString + "               END " + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";

            queryString = queryString + "               BEGIN " + "\r\n";
            queryString = queryString + "                   " + this.BuildSQLNew(isBarcode, isGoodsReceiptDetailIDs) + " WHERE DeliveryAdviceDetails.DeliveryAdviceDetailID NOT IN (SELECT DeliveryAdviceDetailID FROM GoodsIssuePackages WHERE GoodsIssueID = @GoodsIssueID) " + "\r\n";
            queryString = queryString + "                   UNION ALL " + "\r\n";
            queryString = queryString + "                   " + this.BuildSQLEdit(isBarcode, isGoodsReceiptDetailIDs) + "\r\n";
            queryString = queryString + "                   ORDER BY DeliveryAdviceDetails.DeliveryAdviceDetailID " + "\r\n";
            queryString = queryString + "               END " + "\r\n";

            queryString = queryString + "   END " + "\r\n";

            return queryString;
        }

        private string BuildSQLNew(bool isBarcode, bool isGoodsReceiptDetailIDs)
        {
            string queryString = "";

            queryString = queryString + "       SELECT      DeliveryAdviceDetails.DeliveryAdviceID, DeliveryAdviceDetails.DeliveryAdviceDetailID, Commodities.CommodityID, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, Commodities.OfficialCode, Commodities.CommodityTypeID, Commodities.Weight, " + "\r\n";
            queryString = queryString + "                   GoodsReceiptDetails.GoodsReceiptID, GoodsReceiptDetails.GoodsReceiptDetailID, GoodsReceipts.Reference AS GoodsReceiptReference, GoodsReceipts.Code AS GoodsReceiptCode, GoodsReceipts.EntryDate AS GoodsReceiptEntryDate, GoodsReceiptDetails.ExpiryDate, GoodsReceiptDetails.WarehouseID, GoodsReceiptDetails.BatchID, GoodsReceiptDetails.BatchEntryDate, GoodsReceiptDetails.SealCode, GoodsReceiptDetails.BatchCode, GoodsReceiptDetails.LabCode, GoodsReceiptDetails.Barcode, GoodsReceiptDetails.BinLocationID, BinLocations.Code AS BinLocationCode, GoodsReceiptDetails.UnitWeight, GoodsReceiptDetails.TareWeight, " + "\r\n";
            queryString = queryString + "                   ROUND(DeliveryAdviceDetails.Quantity + DeliveryAdviceDetails.FreeQuantity - DeliveryAdviceDetails.QuantityIssue - DeliveryAdviceDetails.FreeQuantityIssue, " + (int)GlobalEnums.rndQuantity + ") AS QuantityRemains, ROUND(GoodsReceiptDetails.Quantity - GoodsReceiptDetails.QuantityIssued, " + (int)GlobalEnums.rndQuantity + ") AS QuantityAvailables, 0 AS Quantity, CAST(0 AS bit) AS IsSelected " + "\r\n";

            queryString = queryString + "       FROM        DeliveryAdviceDetails " + "\r\n";
            queryString = queryString + "                   INNER JOIN Commodities ON DeliveryAdviceDetails.DeliveryAdviceDetailID = @DeliveryAdviceDetailID AND DeliveryAdviceDetails.Approved = 1 AND DeliveryAdviceDetails.InActive = 0 AND DeliveryAdviceDetails.InActivePartial = 0 AND ROUND(DeliveryAdviceDetails.Quantity + DeliveryAdviceDetails.FreeQuantity - DeliveryAdviceDetails.QuantityIssue - DeliveryAdviceDetails.FreeQuantityIssue, " + (int)GlobalEnums.rndQuantity + ") > 0 AND DeliveryAdviceDetails.CommodityID = Commodities.CommodityID " + "\r\n";

            queryString = queryString + "                   INNER JOIN GoodsReceiptDetails ON GoodsReceiptDetails.WarehouseID = @WarehouseID AND DeliveryAdviceDetails.CommodityID = GoodsReceiptDetails.CommodityID AND GoodsReceiptDetails.Approved = 1 AND ROUND(GoodsReceiptDetails.Quantity - GoodsReceiptDetails.QuantityIssued, " + (int)GlobalEnums.rndQuantity + ") > 0 " + (true ? " AND GoodsReceiptDetails.ExpiryDate >= CONVERT(DATE, GETDATE()) AND GoodsReceiptDetails.LabID IN (SELECT LabID FROM Labs WHERE Approved = 1 AND InActive = 0 AND Hold = 0)" : "") + (isBarcode ? " AND GoodsReceiptDetails.Barcode = @Barcode" : "") + (isGoodsReceiptDetailIDs ? " AND GoodsReceiptDetails.GoodsReceiptDetailID NOT IN (SELECT Id FROM dbo.SplitToIntList (@GoodsReceiptDetailIDs))" : "") + "\r\n";
            queryString = queryString + "                   INNER JOIN GoodsReceipts ON GoodsReceiptDetails.GoodsReceiptID = GoodsReceipts.GoodsReceiptID " + "\r\n";
            queryString = queryString + "                   INNER JOIN BinLocations ON GoodsReceiptDetails.BinLocationID = BinLocations.BinLocationID " + "\r\n";

            return queryString;
        }

        private string BuildSQLEdit(bool isBarcode, bool isGoodsReceiptDetailIDs)
        {
            string queryString = "";

            queryString = queryString + "       SELECT      DeliveryAdviceDetails.DeliveryAdviceID, DeliveryAdviceDetails.DeliveryAdviceDetailID, Commodities.CommodityID, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, Commodities.OfficialCode, Commodities.CommodityTypeID, Commodities.Weight, " + "\r\n";
            queryString = queryString + "                   GoodsReceiptDetails.GoodsReceiptID, GoodsReceiptDetails.GoodsReceiptDetailID, GoodsReceipts.Reference AS GoodsReceiptReference, GoodsReceipts.Code AS GoodsReceiptCode, GoodsReceipts.EntryDate AS GoodsReceiptEntryDate, GoodsReceiptDetails.ExpiryDate, GoodsReceiptDetails.WarehouseID, GoodsReceiptDetails.BatchID, GoodsReceiptDetails.BatchEntryDate, GoodsReceiptDetails.SealCode, GoodsReceiptDetails.BatchCode, GoodsReceiptDetails.LabCode, GoodsReceiptDetails.Barcode, GoodsReceiptDetails.BinLocationID, BinLocations.Code AS BinLocationCode, GoodsReceiptDetails.UnitWeight, GoodsReceiptDetails.TareWeight, " + "\r\n";
            queryString = queryString + "                   ROUND(DeliveryAdviceDetails.Quantity + DeliveryAdviceDetails.FreeQuantity - DeliveryAdviceDetails.QuantityIssue - DeliveryAdviceDetails.FreeQuantityIssue + GoodsIssuePackages.Quantity, " + (int)GlobalEnums.rndQuantity + ") AS QuantityRemains, ROUND(GoodsReceiptDetails.Quantity - GoodsReceiptDetails.QuantityIssued + ISNULL(IssuedGoodsReceiptDetails.Quantity, 0), " + (int)GlobalEnums.rndQuantity + ") AS QuantityAvailables, 0 AS Quantity, CAST(0 AS bit) AS IsSelected " + "\r\n";

            queryString = queryString + "       FROM        DeliveryAdviceDetails " + "\r\n";
            queryString = queryString + "                   INNER JOIN (SELECT DeliveryAdviceDetailID, SUM(Quantity) AS Quantity FROM GoodsIssuePackages WHERE GoodsIssueID = @GoodsIssueID AND DeliveryAdviceDetailID = @DeliveryAdviceDetailID GROUP BY DeliveryAdviceDetailID) AS GoodsIssuePackages ON DeliveryAdviceDetails.DeliveryAdviceDetailID = GoodsIssuePackages.DeliveryAdviceDetailID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Commodities ON DeliveryAdviceDetails.CommodityID = Commodities.CommodityID " + "\r\n";

            queryString = queryString + "                   INNER JOIN GoodsReceiptDetails ON GoodsReceiptDetails.WarehouseID = @WarehouseID AND DeliveryAdviceDetails.CommodityID = GoodsReceiptDetails.CommodityID AND GoodsReceiptDetails.Approved = 1 AND (ROUND(GoodsReceiptDetails.Quantity - GoodsReceiptDetails.QuantityIssued, " + (int)GlobalEnums.rndQuantity + ") > 0 OR GoodsReceiptDetails.GoodsReceiptDetailID IN (SELECT GoodsReceiptDetailID FROM GoodsIssuePackages WHERE GoodsIssueID = @GoodsIssueID)) " + (true ? " AND GoodsReceiptDetails.ExpiryDate >= CONVERT(DATE, GETDATE()) AND GoodsReceiptDetails.LabID IN (SELECT LabID FROM Labs WHERE Approved = 1 AND InActive = 0 AND Hold = 0)" : "") + (isBarcode ? " AND GoodsReceiptDetails.Barcode = @Barcode" : "") + (isGoodsReceiptDetailIDs ? " AND GoodsReceiptDetails.GoodsReceiptDetailID NOT IN (SELECT Id FROM dbo.SplitToIntList (@GoodsReceiptDetailIDs))" : "") + "\r\n";
            queryString = queryString + "                   INNER JOIN (SELECT GoodsReceiptDetailID, SUM(Quantity) AS Quantity FROM GoodsIssuePackages WHERE GoodsIssueID = @GoodsIssueID GROUP BY GoodsReceiptDetailID) AS IssuedGoodsReceiptDetails ON GoodsReceiptDetails.GoodsReceiptDetailID = IssuedGoodsReceiptDetails.GoodsReceiptDetailID " + "\r\n";
            queryString = queryString + "                   INNER JOIN GoodsReceipts ON GoodsReceiptDetails.GoodsReceiptID = GoodsReceipts.GoodsReceiptID " + "\r\n";
            queryString = queryString + "                   INNER JOIN BinLocations ON GoodsReceiptDetails.BinLocationID = BinLocations.BinLocationID " + "\r\n";

            return queryString;
        }
        #endregion GetGoodsIssueViewPackages



        private void GoodsIssueSaveRelative()
        {
            string queryString = " @EntityID int, @SaveRelativeOption int " + "\r\n"; //SaveRelativeOption: 1: Update, -1:Undo
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "       DECLARE @msg NVARCHAR(300) ";

            queryString = queryString + "           IF (@SaveRelativeOption = 1) " + "\r\n";
            queryString = queryString + "               BEGIN " + "\r\n";
            queryString = queryString + "                   UPDATE          GoodsIssuePackages " + "\r\n";
            queryString = queryString + "                   SET             GoodsIssuePackages.Reference = GoodsIssues.Reference " + "\r\n";
            queryString = queryString + "                   FROM            GoodsIssues INNER JOIN GoodsIssuePackages ON GoodsIssues.GoodsIssueID = @EntityID AND GoodsIssues.GoodsIssueID = GoodsIssuePackages.GoodsIssueID " + "\r\n";
            queryString = queryString + "               END " + "\r\n";


            #region UPDATE GoodsReceiptDetails
            queryString = queryString + "       DECLARE         @GoodsIssuePackages TABLE (GoodsReceiptDetailID int NOT NULL PRIMARY KEY, Quantity decimal(18, 2) NOT NULL)" + "\r\n";
            queryString = queryString + "       INSERT INTO     @GoodsIssuePackages (GoodsReceiptDetailID, Quantity) SELECT GoodsReceiptDetailID, SUM(Quantity) AS Quantity FROM GoodsIssuePackages WHERE GoodsIssueID = @EntityID GROUP BY GoodsReceiptDetailID " + "\r\n";

            queryString = queryString + "       UPDATE          GoodsReceiptDetails " + "\r\n";
            queryString = queryString + "       SET             GoodsReceiptDetails.QuantityIssued = ROUND(GoodsReceiptDetails.QuantityIssued + GoodsIssuePackages.Quantity * @SaveRelativeOption, " + (int)GlobalEnums.rndQuantity + ") " + "\r\n";
            queryString = queryString + "       FROM            GoodsReceiptDetails " + "\r\n";
            queryString = queryString + "                       INNER JOIN @GoodsIssuePackages GoodsIssuePackages ON GoodsReceiptDetails.GoodsReceiptDetailID = GoodsIssuePackages.GoodsReceiptDetailID AND GoodsReceiptDetails.Approved = 1 " + "\r\n";

            queryString = queryString + "       IF @@ROWCOUNT <> (SELECT COUNT(*) FROM @GoodsIssuePackages) " + "\r\n";
            queryString = queryString + "           BEGIN " + "\r\n";
            queryString = queryString + "               SET         @msg = N'Phiếu nhập kho đã hủy, chưa duyệt hoặc đã xóa.' ; " + "\r\n";
            queryString = queryString + "               THROW       61001,  @msg, 1; " + "\r\n";
            queryString = queryString + "           END " + "\r\n";
            #endregion


            queryString = queryString + "       UPDATE          DeliveryAdviceDetails " + "\r\n";
            queryString = queryString + "       SET             DeliveryAdviceDetails.QuantityIssue = ROUND(DeliveryAdviceDetails.QuantityIssue + GoodsIssueDetails.Quantity * @SaveRelativeOption, " + (int)GlobalEnums.rndQuantity + "), DeliveryAdviceDetails.FreeQuantityIssue = ROUND(DeliveryAdviceDetails.FreeQuantityIssue + GoodsIssueDetails.FreeQuantity * @SaveRelativeOption, " + (int)GlobalEnums.rndQuantity + ") " + "\r\n";
            queryString = queryString + "       FROM            GoodsIssueDetails " + "\r\n";
            queryString = queryString + "                       INNER JOIN DeliveryAdviceDetails ON ((DeliveryAdviceDetails.Approved = 1 AND DeliveryAdviceDetails.InActive = 0 AND DeliveryAdviceDetails.InActivePartial = 0 AND DeliveryAdviceDetails.InActiveIssue = 0) OR @SaveRelativeOption = -1) AND (GoodsIssueDetails.Quantity <> 0 OR GoodsIssueDetails.FreeQuantity <> 0) AND GoodsIssueDetails.GoodsIssueID = @EntityID AND GoodsIssueDetails.DeliveryAdviceDetailID = DeliveryAdviceDetails.DeliveryAdviceDetailID " + "\r\n";

            queryString = queryString + "       IF @@ROWCOUNT = (SELECT COUNT(*) FROM GoodsIssueDetails WHERE GoodsIssueID = @EntityID AND (GoodsIssueDetails.Quantity <> 0 OR GoodsIssueDetails.FreeQuantity <> 0)) " + "\r\n";
            queryString = queryString + "           BEGIN " + "\r\n";
            queryString = queryString + "               UPDATE      DeliveryAdvices SET DeliveryAdvices.TotalQuantityIssue = ROUND(TotalDeliveryAdviceDetails.TotalQuantityIssue, " + (int)GlobalEnums.rndQuantity + "), DeliveryAdvices.TotalFreeQuantityIssue = ROUND(TotalDeliveryAdviceDetails.TotalFreeQuantityIssue, " + (int)GlobalEnums.rndQuantity + ") FROM DeliveryAdvices INNER JOIN (SELECT DeliveryAdviceID, SUM(QuantityIssue) AS TotalQuantityIssue, SUM(FreeQuantityIssue) AS TotalFreeQuantityIssue FROM DeliveryAdviceDetails WHERE DeliveryAdviceID IN (SELECT DeliveryAdviceID FROM GoodsIssueDetails WHERE GoodsIssueID = @EntityID) GROUP BY DeliveryAdviceID) TotalDeliveryAdviceDetails ON DeliveryAdvices.DeliveryAdviceID = TotalDeliveryAdviceDetails.DeliveryAdviceID; " + "\r\n";

            queryString = queryString + "               UPDATE      DeliveryAdviceDetails SET DeliveryAdviceDetails.InActiveIssue = 0 WHERE DeliveryAdviceDetails.DeliveryAdviceDetailID IN (SELECT DeliveryAdviceDetailID FROM GoodsIssueDetails WHERE GoodsIssueID = @EntityID) " + "\r\n";
            queryString = queryString + "               UPDATE      DeliveryAdviceDetails SET DeliveryAdviceDetails.InActiveIssue = DeliveryAdviceInActiveIssue.InActiveIssue " + "\r\n";
            queryString = queryString + "               FROM        DeliveryAdviceDetails " + "\r\n";
            queryString = queryString + "                           INNER JOIN (SELECT GoodsIssueDetails.DeliveryAdviceDetailID, MAX(CAST(VoidTypes.InActive AS int)) AS InActiveIssue FROM GoodsIssueDetails INNER JOIN VoidTypes ON GoodsIssueDetails.VoidTypeID = VoidTypes.VoidTypeID AND GoodsIssueDetails.GoodsIssueID <> (@EntityID * -@SaveRelativeOption) AND GoodsIssueDetails.DeliveryAdviceDetailID IN (SELECT DeliveryAdviceDetailID FROM GoodsIssueDetails WHERE GoodsIssueID = @EntityID) GROUP BY GoodsIssueDetails.DeliveryAdviceDetailID) AS DeliveryAdviceInActiveIssue " + "\r\n"; //THIS GoodsIssueDetails.GoodsIssueID <> (@EntityID * -@SaveRelativeOption) => EXCLUSIVE @EntityID WHEN UNDO
            queryString = queryString + "                           ON DeliveryAdviceDetails.DeliveryAdviceDetailID = DeliveryAdviceInActiveIssue.DeliveryAdviceDetailID " + "\r\n";


            //UPDATE ERmgrVCP.BEGIN
            queryString = queryString + "               UPDATE      ERmgrVCPDeliveryAdviceDetails SET ERmgrVCPDeliveryAdviceDetails.QuantityIssue = DeliveryAdviceDetails.QuantityIssue, ERmgrVCPDeliveryAdviceDetails.FreeQuantityIssue = DeliveryAdviceDetails.FreeQuantityIssue, ERmgrVCPDeliveryAdviceDetails.InActiveIssue = DeliveryAdviceDetails.InActiveIssue FROM ERmgrVCP.dbo.DeliveryAdviceDetails AS ERmgrVCPDeliveryAdviceDetails INNER JOIN DeliveryAdviceDetails ON ERmgrVCPDeliveryAdviceDetails.DeliveryAdviceDetailID = DeliveryAdviceDetails.DeliveryAdviceDetailID AND DeliveryAdviceDetails.DeliveryAdviceDetailID IN (SELECT DeliveryAdviceDetailID FROM GoodsIssueDetails WHERE GoodsIssueID = @EntityID) " + "\r\n";

            queryString = queryString + "               IF          (@SaveRelativeOption =  1)    EXEC        ERmgrVCP.dbo.GoodsIssueSaveRelative @EntityID, @SaveRelativeOption "; //WHEN SAVE: SHOULD ADD TO ERmgrVCP FIRST, THEN CALL SPSKUBalanceUpdate
            queryString = queryString + "               EXEC        ERmgrVCP.dbo.SPSKUBalanceUpdate     @SaveRelativeOption, 0, 0, @EntityID, 0, 0 ";
            queryString = queryString + "               IF          (@SaveRelativeOption = -1)    EXEC        ERmgrVCP.dbo.GoodsIssueSaveRelative @EntityID, @SaveRelativeOption "; //WHEN UNDO: SHOULD REMOVE FROM ERmgrVCP LATER, AFTER CALL SPSKUBalanceUpdate
            //UPDATE ERmgrVCP.END


            queryString = queryString + "           END " + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           BEGIN " + "\r\n";
            queryString = queryString + "               SET         @msg = N'Đề nghị giao hàng không tồn tại, chưa duyệt hoặc đã hủy' ; " + "\r\n";
            queryString = queryString + "               THROW       61001,  @msg, 1; " + "\r\n";
            queryString = queryString + "           END " + "\r\n";


            this.totalSmartPortalEntities.CreateStoredProcedure("GoodsIssueSaveRelative", queryString);


            queryString = " USE ERmgrVCP    GO " + "\r\n";
            queryString = queryString + " DROP PROC GoodsIssueSaveRelative " + "\r\n";
            queryString = queryString + " CREATE PROC GoodsIssueSaveRelative " + "\r\n";
            queryString = queryString + " @EntityID int, @SaveRelativeOption int " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "       IF (@SaveRelativeOption = 1) " + "\r\n";
            queryString = queryString + "           BEGIN " + "\r\n";
            queryString = queryString + "               INSERT INTO GoodsIssues SELECT * FROM TotalSmartPortal.dbo.GoodsIssues WHERE GoodsIssueID = @EntityID " + "\r\n";
            queryString = queryString + "               INSERT INTO GoodsIssueDetails SELECT * FROM TotalSmartPortal.dbo.GoodsIssueDetails WHERE GoodsIssueID = @EntityID " + "\r\n";
            queryString = queryString + "           END " + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           BEGIN " + "\r\n";
            queryString = queryString + "               DELETE FROM GoodsIssueDetails WHERE GoodsIssueID = @EntityID " + "\r\n";
            queryString = queryString + "               DELETE FROM GoodsIssues WHERE GoodsIssueID = @EntityID " + "\r\n";
            queryString = queryString + "           END " + "\r\n";
            System.Diagnostics.Debug.WriteLine("--------");
            System.Diagnostics.Debug.WriteLine(queryString);
        }

        private void GoodsIssuePostSaveValidate()
        {
            string[] queryArray = new string[4];

            queryArray[0] = " SELECT TOP 1 @FoundEntity = N'Ngày phiếu yêu cầu: ' + CAST(DeliveryAdvices.EntryDate AS nvarchar) FROM GoodsIssueDetails INNER JOIN DeliveryAdvices ON GoodsIssueDetails.GoodsIssueID = @EntityID AND GoodsIssueDetails.DeliveryAdviceID = DeliveryAdvices.DeliveryAdviceID AND GoodsIssueDetails.EntryDate < DeliveryAdvices.EntryDate ";
            queryArray[1] = " SELECT TOP 1 @FoundEntity = N'Số lượng xuất kho vượt quá số lượng yêu cầu: ' + CAST(ROUND(Quantity - QuantityIssue, " + (int)GlobalEnums.rndQuantity + ") AS nvarchar) + ' OR free quantity: ' + CAST(ROUND(FreeQuantity - FreeQuantityIssue, " + (int)GlobalEnums.rndQuantity + ") AS nvarchar) FROM DeliveryAdviceDetails WHERE (ROUND(Quantity - QuantityIssue, " + (int)GlobalEnums.rndQuantity + ") < 0) OR (ROUND(FreeQuantity - FreeQuantityIssue, " + (int)GlobalEnums.rndQuantity + ") < 0) ";
            queryArray[2] = " SELECT TOP 1 @FoundEntity = N'Ngày nhập kho: ' + CAST(GoodsReceipts.EntryDate AS nvarchar) FROM GoodsIssuePackages INNER JOIN GoodsReceipts ON GoodsIssuePackages.GoodsIssueID = @EntityID AND GoodsIssuePackages.GoodsReceiptID = GoodsReceipts.GoodsReceiptID AND GoodsIssuePackages.EntryDate < GoodsReceipts.EntryDate ";
            queryArray[3] = " SELECT TOP 1 @FoundEntity = N'Số lượng xuất vượt quá số lượng tồn kho: ' + CAST(ROUND(Quantity - QuantityIssued, " + (int)GlobalEnums.rndQuantity + ") AS nvarchar) FROM GoodsReceiptDetails WHERE (ROUND(Quantity - QuantityIssued, " + (int)GlobalEnums.rndQuantity + ") < 0) ";

            this.totalSmartPortalEntities.CreateProcedureToCheckExisting("GoodsIssuePostSaveValidate", queryArray);
        }


        private void GoodsIssueApproved()
        {
            string[] queryArray = new string[1];

            queryArray[0] = " SELECT TOP 1 @FoundEntity = GoodsIssueID FROM GoodsIssues WHERE GoodsIssueID = @EntityID AND Approved = 1";

            this.totalSmartPortalEntities.CreateProcedureToCheckExisting("GoodsIssueApproved", queryArray);
        }


        private void GoodsIssueEditable()
        {
            string[] queryArray = new string[4];

            queryArray[0] = " SELECT TOP 1 @FoundEntity = GoodsIssueID FROM HandlingUnitDetails WHERE GoodsIssueID = @EntityID ";
            queryArray[1] = " SELECT TOP 1 @FoundEntity = GoodsIssueID FROM AccountInvoiceDetails WHERE GoodsIssueID = @EntityID ";
            queryArray[2] = " SELECT TOP 1 @FoundEntity = GoodsIssueID FROM ReceiptDetails WHERE GoodsIssueID = @EntityID ";
            queryArray[3] = " SELECT TOP 1 @FoundEntity = GoodsIssueID FROM SalesReturnDetails WHERE GoodsIssueID = @EntityID ";

            this.totalSmartPortalEntities.CreateProcedureToCheckExisting("GoodsIssueEditable", queryArray);
        }


        private void GoodsIssueToggleApproved()
        {
            string queryString = " @EntityID int, @Approved bit " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "       UPDATE      GoodsIssues  SET Approved = @Approved, ApprovedDate = GetDate() WHERE GoodsIssueID = @EntityID AND Approved = ~@Approved" + "\r\n";

            queryString = queryString + "       IF @@ROWCOUNT = 1 " + "\r\n";
            queryString = queryString + "           BEGIN " + "\r\n";
            queryString = queryString + "               UPDATE      GoodsIssueDetails  SET Approved = @Approved WHERE GoodsIssueID = @EntityID ; " + "\r\n";
            queryString = queryString + "               UPDATE      GoodsIssuePackages  SET Approved = @Approved WHERE GoodsIssueID = @EntityID ; " + "\r\n";
            queryString = queryString + "           END " + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           BEGIN " + "\r\n";
            queryString = queryString + "               DECLARE     @msg NVARCHAR(300) = N'Dữ liệu không tồn tại hoặc đã ' + iif(@Approved = 0, N'hủy', '')  + N' duyệt' ; " + "\r\n";
            queryString = queryString + "               THROW       61001,  @msg, 1; " + "\r\n";
            queryString = queryString + "           END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GoodsIssueToggleApproved", queryString);
        }


        private void GoodsIssueInitReference()
        {
            SimpleInitReference simpleInitReference = new SimpleInitReference("GoodsIssues", "GoodsIssueID", "Reference", ModelSettingManager.ReferenceLength, ModelSettingManager.ReferencePrefix(GlobalEnums.NmvnTaskID.GoodsIssue));
            this.totalSmartPortalEntities.CreateTrigger("GoodsIssueInitReference", simpleInitReference.CreateQuery());
        }


        private void GoodsIssueSheet()
        {
            string queryString = " @GoodsIssueID int " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";
            queryString = queryString + "    BEGIN " + "\r\n";

            queryString = queryString + "       DECLARE         @LocalGoodsIssueID int    SET @LocalGoodsIssueID = @GoodsIssueID" + "\r\n";

            queryString = queryString + "       SELECT          GoodsIssues.GoodsIssueID, GoodsIssues.EntryDate, GoodsIssues.Reference, GoodsIssues.Code AS GoodsIssueCode, GoodsIssues.PaymentTermID, PaymentTerms.Name AS PaymentTermName, GoodsIssues.Description, GoodsIssues.Remarks, GoodsIssueDetails.Remarks AS RemarkDetails, " + "\r\n";
            queryString = queryString + "                       Customers.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, Customers.Telephone AS CustomerTelephone, Customers.BillingAddress AS CustomerBillingAddress, Receivers.CustomerID AS ReceiverID, Receivers.Code AS ReceiverCode, Receivers.Name AS ReceiverName, Receivers.Telephone AS ReceiverTelephone, Receivers.AttentionName AS ReceiverAttentionName, Receivers.BillingAddress AS ReceiverBillingAddress, GoodsIssues.ShippingAddress, GoodsIssues.Addressee, " + "\r\n";
            queryString = queryString + "                       Commodities.CommodityID, Commodities.Code AS CommodityCode, Commodities.CodePartA, Commodities.CodePartB, Commodities.CodePartC, Commodities.CodePartD, Commodities.Name AS CommodityName, Commodities.SalesUnit, GoodsIssueDetails.WarehouseID, Warehouses.Code AS WarehouseCode, " + "\r\n";
            queryString = queryString + "                       GoodsIssueDetails.Quantity, GoodsIssueDetails.FreeQuantity, GoodsIssueDetails.ListedGrossPrice, GoodsIssueDetails.GrossPrice, GoodsIssueDetails.ListedGrossAmount, GoodsIssueDetails.GrossAmount, " + "\r\n";
            queryString = queryString + "                       GoodsIssues.VATPercent, GoodsIssues.TotalQuantity, GoodsIssues.TotalFreeQuantity, GoodsIssues.TotalListedAmount, GoodsIssues.TotalAmount, GoodsIssues.TotalListedVATAmount, GoodsIssues.TotalVATAmount, GoodsIssues.TotalListedGrossAmount, GoodsIssues.TotalGrossAmount, GoodsIssues.SumListedGrossAmount, GoodsIssues.SumGrossAmount " + "\r\n";

            queryString = queryString + "       FROM            GoodsIssues " + "\r\n";
            queryString = queryString + "                       INNER JOIN GoodsIssueDetails ON GoodsIssues.GoodsIssueID = @LocalGoodsIssueID AND GoodsIssues.GoodsIssueID = GoodsIssueDetails.GoodsIssueID " + "\r\n";
            queryString = queryString + "                       INNER JOIN Commodities ON GoodsIssueDetails.CommodityID = Commodities.CommodityID " + "\r\n";
            queryString = queryString + "                       INNER JOIN Warehouses ON GoodsIssueDetails.WarehouseID = Warehouses.WarehouseID " + "\r\n";
            queryString = queryString + "                       INNER JOIN Customers ON GoodsIssues.CustomerID = Customers.CustomerID " + "\r\n";
            queryString = queryString + "                       INNER JOIN Customers AS Receivers ON GoodsIssues.ReceiverID = Receivers.CustomerID " + "\r\n";
            queryString = queryString + "                       INNER JOIN PaymentTerms ON GoodsIssues.PaymentTermID = PaymentTerms.PaymentTermID " + "\r\n";

            queryString = queryString + "       ORDER BY        Commodities.CommodityTypeID, GoodsIssueDetails.DeliveryAdviceID, GoodsIssueDetails.DeliveryAdviceDetailID " + "\r\n";
            

            queryString = queryString + "    END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GoodsIssueSheet", queryString);

        }

    }
}
