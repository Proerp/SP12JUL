using TotalBase;
using TotalBase.Enums;
using TotalModel.Models;

namespace TotalDAL.Helpers.SqlProgrammability.Accounts
{
    public class WarehouseInvoice
    {
        private readonly TotalSmartPortalEntities totalSmartPortalEntities;

        public WarehouseInvoice(TotalSmartPortalEntities totalSmartPortalEntities)
        {
            this.totalSmartPortalEntities = totalSmartPortalEntities;
        }

        public void RestoreProcedure()
        {
            this.GetWarehouseInvoiceIndexes();

            this.GetWarehouseInvoiceViewDetails();

            this.GetPendingStockTransfers();
            this.GetPendingStockTransferConsumers();
            this.GetPendingStockTransferReceivers();

            this.GetPendingStockTransferDetails();

            this.WarehouseInvoiceSaveRelative();
            this.WarehouseInvoicePostSaveValidate();

            this.WarehouseInvoiceInitReference();

            this.WarehouseInvoiceSheet();
        }

        private void GetWarehouseInvoiceIndexes()
        {
            string queryString;

            queryString = " @AspUserID nvarchar(128), @FromDate DateTime, @ToDate DateTime " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";
            queryString = queryString + "    BEGIN " + "\r\n";

            queryString = queryString + "       SELECT      WarehouseInvoices.WarehouseInvoiceID, CAST(WarehouseInvoices.EntryDate AS DATE) AS EntryDate, WarehouseInvoices.Reference, WarehouseInvoices.VATInvoiceNo, WarehouseInvoices.VATInvoiceDate, WarehouseInvoices.Code, WarehouseInvoices.CustomerPO, Locations.Code AS LocationCode, WarehouseInvoices.StockTransferReferences, StockTransfers.EntryDate AS StockTransferEntryDate, Customers.Name AS CustomerName, Customers.BillingAddress, WarehouseInvoices.Description, WarehouseInvoices.TotalQuantity, WarehouseInvoices.TotalListedGrossAmount, WarehouseInvoices.TotalGrossAmount " + "\r\n";
            queryString = queryString + "       FROM        WarehouseInvoices " + "\r\n";
            queryString = queryString + "                   INNER JOIN Locations ON WarehouseInvoices.EntryDate >= @FromDate AND WarehouseInvoices.EntryDate <= @ToDate AND WarehouseInvoices.OrganizationalUnitID IN (SELECT AccessControls.OrganizationalUnitID FROM AccessControls INNER JOIN AspNetUsers ON AccessControls.UserID = AspNetUsers.UserID WHERE AspNetUsers.Id = @AspUserID AND AccessControls.NMVNTaskID = " + (int)TotalBase.Enums.GlobalEnums.NmvnTaskID.WarehouseInvoice + " AND AccessControls.AccessLevel > 0) AND Locations.LocationID = WarehouseInvoices.LocationID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Customers ON WarehouseInvoices.CustomerID = Customers.CustomerID " + "\r\n";
            queryString = queryString + "                   LEFT JOIN  StockTransfers ON WarehouseInvoices.StockTransferID = StockTransfers.StockTransferID " + "\r\n";
            queryString = queryString + "       " + "\r\n";

            queryString = queryString + "    END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetWarehouseInvoiceIndexes", queryString);
        }

        private void GetWarehouseInvoiceViewDetails()
        {
            string queryString;

            queryString = " @WarehouseInvoiceID Int " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";
            queryString = queryString + "    BEGIN " + "\r\n";

            queryString = queryString + "       SELECT      WarehouseInvoiceDetails.WarehouseInvoiceDetailID, WarehouseInvoiceDetails.WarehouseInvoiceID, WarehouseInvoiceDetails.StockTransferID, WarehouseInvoiceDetails.StockTransferDetailID, StockTransfers.Reference AS StockTransferReference, StockTransfers.Code AS StockTransferCode, StockTransfers.EntryDate AS StockTransferEntryDate, Commodities.CommodityID, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, Commodities.CommodityTypeID, WarehouseInvoiceDetails.CalculatingTypeID, WarehouseInvoiceDetails.VATbyRow, ROUND(StockTransferDetails.Quantity - StockTransferDetails.QuantityInvoiced + WarehouseInvoiceDetails.Quantity, 0) AS QuantityRemains, ROUND(StockTransferDetails.FreeQuantity - StockTransferDetails.FreeQuantityInvoiced + WarehouseInvoiceDetails.FreeQuantity, 0) AS FreeQuantityRemains, StockTransferDetails.ControlFreeQuantity, " + "\r\n";
            queryString = queryString + "                   WarehouseInvoiceDetails.Quantity, WarehouseInvoiceDetails.FreeQuantity, WarehouseInvoiceDetails.ListedPrice, WarehouseInvoiceDetails.DiscountPercent, WarehouseInvoiceDetails.UnitPrice, WarehouseInvoiceDetails.TradeDiscountRate, WarehouseInvoiceDetails.VATPercent, WarehouseInvoiceDetails.ListedGrossPrice, WarehouseInvoiceDetails.GrossPrice, WarehouseInvoiceDetails.ListedAmount, WarehouseInvoiceDetails.Amount, WarehouseInvoiceDetails.ListedVATAmount, WarehouseInvoiceDetails.VATAmount, WarehouseInvoiceDetails.ListedGrossAmount, WarehouseInvoiceDetails.GrossAmount, WarehouseInvoiceDetails.IsBonus, WarehouseInvoiceDetails.Remarks " + "\r\n";

            queryString = queryString + "       FROM        WarehouseInvoiceDetails " + "\r\n";
            queryString = queryString + "                   INNER JOIN StockTransferDetails ON WarehouseInvoiceDetails.WarehouseInvoiceID = @WarehouseInvoiceID AND WarehouseInvoiceDetails.StockTransferDetailID = StockTransferDetails.StockTransferDetailID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Commodities ON StockTransferDetails.CommodityID = Commodities.CommodityID " + "\r\n";
            queryString = queryString + "                   INNER JOIN StockTransfers ON StockTransferDetails.StockTransferID = StockTransfers.StockTransferID " + "\r\n";
            queryString = queryString + "       " + "\r\n";

            queryString = queryString + "    END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetWarehouseInvoiceViewDetails", queryString);
        }








        private void GetPendingStockTransfers()
        {
            string queryString = " @LocationID int " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";
            queryString = queryString + "       SELECT          TOP 200 StockTransfers.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, Receivers.CustomerID AS ReceiverID, Receivers.Code AS ReceiverCode, Receivers.Name AS ReceiverName, Receivers.VATCode AS ReceiverVATCode, Receivers.AttentionName AS ReceiverAttentionName, Receivers.Telephone AS ReceiverTelephone, Receivers.BillingAddress AS ReceiverBillingAddress, ReceiverEntireTerritories.EntireName AS ReceiverEntireTerritoryEntireName, " + "\r\n";
            queryString = queryString + "                       StockTransfers.StockTransferID, StockTransfers.Reference AS StockTransferReference, StockTransfers.Code AS StockTransferCode, StockTransfers.EntryDate AS StockTransferEntryDate, Customers.Code AS StockTransferCustomerCode, Customers.Name AS StockTransferCustomerName, StockTransfers.PaymentTermID, StockTransfers.TradePromotionID, TradePromotions.Specs AS TradePromotionSpecs, StockTransfers.TradeDiscountRate, StockTransfers.VATPercent, StockTransfers.Description, StockTransfers.Remarks " + "\r\n";

            queryString = queryString + "       FROM            StockTransfers " + "\r\n";
            queryString = queryString + "                       INNER JOIN Customers ON StockTransfers.StockTransferID IN (SELECT StockTransferID FROM StockTransferDetails WHERE Approved = 1 AND LocationID = @LocationID AND EntryDate > DATEADD(day, -50, GetDate()) AND ROUND(Quantity - QuantityInvoiced, " + (int)GlobalEnums.rndQuantity + ") > 0 OR ROUND(FreeQuantity - FreeQuantityInvoiced, " + (int)GlobalEnums.rndQuantity + ") > 0) AND StockTransfers.CustomerID = Customers.CustomerID " + "\r\n";
            queryString = queryString + "                       INNER JOIN Customers Receivers ON StockTransfers.ReceiverID = Receivers.CustomerID " + "\r\n";
            queryString = queryString + "                       INNER JOIN EntireTerritories ReceiverEntireTerritories ON Receivers.TerritoryID = ReceiverEntireTerritories.TerritoryID " + "\r\n";
            queryString = queryString + "                       LEFT  JOIN Promotions AS TradePromotions ON StockTransfers.TradePromotionID = TradePromotions.PromotionID " + "\r\n";
            queryString = queryString + "       ORDER BY        StockTransfers.EntryDate DESC " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetPendingStockTransfers", queryString);
        }

        private void GetPendingStockTransferConsumers()
        {
            string queryString = " @LocationID int " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "       SELECT          Customers.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, Customers.VATCode AS CustomerVATCode, Customers.AttentionName AS CustomerAttentionName, Customers.Telephone AS CustomerTelephone, Customers.BillingAddress AS CustomerBillingAddress, CustomerEntireTerritories.EntireName AS CustomerEntireTerritoryEntireName, CustomerCategories.PaymentTermID, PENDINGCustomers.TradePromotionID, TradePromotions.Specs AS TradePromotionSpecs, PENDINGCustomers.TradeDiscountRate, PENDINGCustomers.VATPercent " + "\r\n";

            queryString = queryString + "       FROM            Customers " + "\r\n";
            queryString = queryString + "                       INNER JOIN (SELECT DISTINCT CustomerID, TradePromotionID, TradeDiscountRate, " + (GlobalEnums.VATbyRow ? "0.0 AS" : "") + " VATPercent FROM StockTransfers WHERE StockTransferID IN (SELECT StockTransferID FROM StockTransferDetails WHERE LocationID = @LocationID AND Approved = 1 AND (ROUND(Quantity - QuantityInvoiced, " + (int)GlobalEnums.rndQuantity + ") > 0 OR ROUND(FreeQuantity - FreeQuantityInvoiced, " + (int)GlobalEnums.rndQuantity + ") > 0))) PENDINGCustomers ON Customers.CustomerID = PENDINGCustomers.CustomerID " + "\r\n";
            queryString = queryString + "                       INNER JOIN EntireTerritories CustomerEntireTerritories ON Customers.TerritoryID = CustomerEntireTerritories.TerritoryID " + "\r\n";
            queryString = queryString + "                       INNER JOIN CustomerCategories ON Customers.CustomerCategoryID = CustomerCategories.CustomerCategoryID " + "\r\n";
            queryString = queryString + "                       LEFT  JOIN Promotions AS TradePromotions ON PENDINGCustomers.TradePromotionID = TradePromotions.PromotionID " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetPendingStockTransferConsumers", queryString);
        }


        private void GetPendingStockTransferReceivers()
        {
            string queryString = " @LocationID int " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "       SELECT          PENDINGReceivers.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, PENDINGReceivers.ReceiverID, Receivers.Code AS ReceiverCode, Receivers.Name AS ReceiverName, Receivers.VATCode AS ReceiverVATCode, Receivers.AttentionName AS ReceiverAttentionName, Receivers.Telephone AS ReceiverTelephone, Receivers.BillingAddress AS ReceiverBillingAddress, ReceiverEntireTerritories.EntireName AS ReceiverEntireTerritoryEntireName, ReceiverCategories.PaymentTermID, PENDINGReceivers.TradePromotionID, TradePromotions.Specs AS TradePromotionSpecs, PENDINGReceivers.TradeDiscountRate, PENDINGReceivers.VATPercent " + "\r\n";

            queryString = queryString + "       FROM            Customers " + "\r\n";
            queryString = queryString + "                       INNER JOIN (SELECT DISTINCT CustomerID, ReceiverID, TradePromotionID, TradeDiscountRate, " + (GlobalEnums.VATbyRow ? "0.0 AS" : "") + " VATPercent FROM StockTransfers WHERE StockTransferID IN (SELECT StockTransferID FROM StockTransferDetails WHERE LocationID = @LocationID AND Approved = 1 AND (ROUND(Quantity - QuantityInvoiced, " + (int)GlobalEnums.rndQuantity + ") > 0 OR ROUND(FreeQuantity - FreeQuantityInvoiced, " + (int)GlobalEnums.rndQuantity + ") > 0))) PENDINGReceivers ON Customers.CustomerID = PENDINGReceivers.CustomerID " + "\r\n";
            queryString = queryString + "                       INNER JOIN Customers Receivers ON PENDINGReceivers.ReceiverID = Receivers.CustomerID " + "\r\n";
            queryString = queryString + "                       INNER JOIN EntireTerritories ReceiverEntireTerritories ON Receivers.TerritoryID = ReceiverEntireTerritories.TerritoryID " + "\r\n";
            queryString = queryString + "                       INNER JOIN CustomerCategories ReceiverCategories ON Receivers.CustomerCategoryID = ReceiverCategories.CustomerCategoryID " + "\r\n";
            queryString = queryString + "                       LEFT  JOIN Promotions AS TradePromotions ON PENDINGReceivers.TradePromotionID = TradePromotions.PromotionID " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetPendingStockTransferReceivers", queryString);
        }


        //WE HAVE 3 OPTIONS TO FILTER StockTransfers TO ISSUE INVOICE: GetPendingStockTransfers, GetPendingStockTransferConsumers, GetPendingStockTransferReceivers
        //ALL OF THESE 3 OPTIONS: WE ALWAYS HAVE A SPECIFIC CustomerID. THIS MEANS: 
        //+GetPendingStockTransfers: WE CAN ISSUE INVOICE FOR A SPECIFIC GOODSISSUE (OF THE SELECTED CUSTOMER)
        //+GetPendingStockTransferConsumers: WE CAN ISSUE INVOICE FOR A SPECIFIC CUSTOMER (COMBINE MULTI GOODSISSUES OF THE SELECTED CUSTOMER)
        //+GetPendingStockTransferReceivers: WE CAN ISSUE INVOICE FOR A SPECIFIC CUSTOMER WITH A SPECIFIC RECEIVER (COMBINE MULTI GOODSISSUES OF THE SELECTED CUSTOMER AND THE SELECTED RECEIVER)
        private void GetPendingStockTransferDetails()
        {
            string queryString;

            queryString = " @WarehouseInvoiceID Int, @LocationID Int, @StockTransferID Int, @CustomerID Int, @ReceiverID Int, @TradePromotionID int, @VATPercent decimal(18, 2), @CommodityTypeID int, @AspUserID nvarchar(128), @FromDate DateTime, @ToDate DateTime, @StockTransferDetailIDs varchar(3999), @IsReadonly bit " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "   BEGIN " + "\r\n";
            queryString = queryString + "       IF  (@StockTransferID <> 0) " + "\r\n";
            queryString = queryString + "           " + this.GetPGIDsBuildSQLStockTransfer(true) + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           " + this.GetPGIDsBuildSQLStockTransfer(false) + "\r\n";
            queryString = queryString + "   END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetPendingStockTransferDetails", queryString);
        }


        private string GetPGIDsBuildSQLStockTransfer(bool isStockTransferID)
        {
            string queryString = "";
            queryString = queryString + "   BEGIN " + "\r\n";
            queryString = queryString + "       IF  (@CustomerID <> 0 AND @ReceiverID IS NULL) " + "\r\n";
            queryString = queryString + "           " + this.GetPGIDsBuildSQLStockTransferCustomer(isStockTransferID, true) + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           " + this.GetPGIDsBuildSQLStockTransferCustomer(isStockTransferID, false) + "\r\n";
            queryString = queryString + "   END " + "\r\n";

            return queryString;
        }

        private string GetPGIDsBuildSQLStockTransferCustomer(bool isStockTransferID, bool isCustomerID)
        {
            string queryString = "";
            queryString = queryString + "   BEGIN " + "\r\n";
            queryString = queryString + "       IF  (@CustomerID <> 0 AND @ReceiverID <> 0) " + "\r\n";
            queryString = queryString + "           " + this.GetPGIDsBuildSQLStockTransferReceiver(isStockTransferID, isCustomerID, true) + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           " + this.GetPGIDsBuildSQLStockTransferReceiver(isStockTransferID, isCustomerID, false) + "\r\n";
            queryString = queryString + "   END " + "\r\n";

            return queryString;
        }

        private string GetPGIDsBuildSQLStockTransferReceiver(bool isStockTransferID, bool isCustomerID, bool isReceiverID)
        {
            string queryString = "";
            queryString = queryString + "   BEGIN " + "\r\n";
            queryString = queryString + "       IF  (@CommodityTypeID <> 0) " + "\r\n";
            queryString = queryString + "           " + this.GetPGIDsBuildSQLStockTransferCustomerCommodityType(isStockTransferID, isCustomerID, isReceiverID, true) + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           " + this.GetPGIDsBuildSQLStockTransferCustomerCommodityType(isStockTransferID, isCustomerID, isReceiverID, false) + "\r\n";
            queryString = queryString + "   END " + "\r\n";

            return queryString;
        }

        private string GetPGIDsBuildSQLStockTransferCustomerCommodityType(bool isStockTransferID, bool isCustomerID, bool isReceiverID, bool isCommodityTypeID)
        {
            string queryString = "";
            queryString = queryString + "   BEGIN " + "\r\n";
            queryString = queryString + "       IF  (@StockTransferDetailIDs <> '') " + "\r\n";
            queryString = queryString + "           " + this.GetPGIDsBuildSQLStockTransferCustomerCommodityTypeStockTransferDetailIDs(isStockTransferID, isCustomerID, isReceiverID, isCommodityTypeID, true) + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           " + this.GetPGIDsBuildSQLStockTransferCustomerCommodityTypeStockTransferDetailIDs(isStockTransferID, isCustomerID, isReceiverID, isCommodityTypeID, false) + "\r\n";
            queryString = queryString + "   END " + "\r\n";

            return queryString;
        }

        private string GetPGIDsBuildSQLStockTransferCustomerCommodityTypeStockTransferDetailIDs(bool isStockTransferID, bool isCustomerID, bool isReceiverID, bool isCommodityTypeID, bool isStockTransferDetailIDs)
        {
            string queryString = "";
            queryString = queryString + "   BEGIN " + "\r\n";

            queryString = queryString + "       IF (@WarehouseInvoiceID <= 0) " + "\r\n";
            queryString = queryString + "               BEGIN " + "\r\n";
            queryString = queryString + "                   " + this.GetPGIDsBuildSQLNew(isStockTransferID, isCustomerID, isReceiverID, isCommodityTypeID, isStockTransferDetailIDs) + "\r\n";
            queryString = queryString + "                   ORDER BY StockTransfers.EntryDate, StockTransfers.StockTransferID, StockTransferDetails.StockTransferDetailID " + "\r\n";
            queryString = queryString + "               END " + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";

            queryString = queryString + "               IF (@IsReadonly = 1) " + "\r\n";
            queryString = queryString + "                   BEGIN " + "\r\n";
            queryString = queryString + "                       " + this.GetPGIDsBuildSQLEdit(isStockTransferID, isCustomerID, isReceiverID, isCommodityTypeID, isStockTransferDetailIDs) + "\r\n";
            queryString = queryString + "                       ORDER BY StockTransfers.EntryDate, StockTransfers.StockTransferID, StockTransferDetails.StockTransferDetailID " + "\r\n";
            queryString = queryString + "                   END " + "\r\n";

            queryString = queryString + "               ELSE " + "\r\n"; //FULL SELECT FOR EDIT MODE

            queryString = queryString + "                   BEGIN " + "\r\n";
            queryString = queryString + "                       " + this.GetPGIDsBuildSQLNew(isStockTransferID, isCustomerID, isReceiverID, isCommodityTypeID, isStockTransferDetailIDs) + " AND StockTransferDetails.StockTransferDetailID NOT IN (SELECT StockTransferDetailID FROM WarehouseInvoiceDetails WHERE WarehouseInvoiceID = @WarehouseInvoiceID) " + "\r\n";
            queryString = queryString + "                       UNION ALL " + "\r\n";
            queryString = queryString + "                       " + this.GetPGIDsBuildSQLEdit(isStockTransferID, isCustomerID, isReceiverID, isCommodityTypeID, isStockTransferDetailIDs) + "\r\n";
            queryString = queryString + "                       ORDER BY StockTransfers.EntryDate, StockTransfers.StockTransferID, StockTransferDetails.StockTransferDetailID " + "\r\n";
            queryString = queryString + "                   END " + "\r\n";

            queryString = queryString + "   END " + "\r\n";

            return queryString;
        }

        private string GetPGIDsBuildSQLNew(bool isStockTransferID, bool isCustomerID, bool isReceiverID, bool isCommodityTypeID, bool isStockTransferDetailIDs)
        {
            string queryString = "";

            queryString = queryString + "       SELECT      StockTransfers.StockTransferID, StockTransferDetails.StockTransferDetailID, StockTransfers.Reference AS StockTransferReference, StockTransfers.Code AS StockTransferCode, StockTransfers.EntryDate AS StockTransferEntryDate, Commodities.CommodityID, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, Commodities.CommodityTypeID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, Customers.BillingAddress, Receivers.Code AS ReceiverCode, Receivers.Name AS ReceiverName, StockTransferDetails.CalculatingTypeID, StockTransferDetails.VATbyRow, ROUND(StockTransferDetails.Quantity - StockTransferDetails.QuantityInvoiced, 0) AS QuantityRemains, ROUND(StockTransferDetails.FreeQuantity - StockTransferDetails.FreeQuantityInvoiced, 0) AS FreeQuantityRemains, StockTransferDetails.ControlFreeQuantity, " + "\r\n";
            queryString = queryString + "                   0.0 AS Quantity, 0.0 AS FreeQuantity, StockTransferDetails.ListedPrice, StockTransferDetails.DiscountPercent, StockTransferDetails.UnitPrice, StockTransferDetails.TradeDiscountRate, StockTransferDetails.VATPercent, StockTransferDetails.ListedGrossPrice, StockTransferDetails.GrossPrice, 0.0 AS ListedAmount, 0.0 AS Amount, 0.0 AS ListedVATAmount, 0.0 AS VATAmount, 0.0 AS ListedGrossAmount, 0.0 AS GrossAmount, StockTransferDetails.IsBonus, CAST(1 AS bit) AS IsSelected " + "\r\n";

            queryString = queryString + "       FROM        StockTransferDetails " + "\r\n";
            queryString = queryString + "                   INNER JOIN Commodities ON " + (isStockTransferID ? " StockTransferDetails.StockTransferID = @StockTransferID " : "") + (!isStockTransferID && isCustomerID ? " StockTransferDetails.CustomerID = @CustomerID " : "") + (!isStockTransferID && !isCustomerID && isReceiverID ? " StockTransferDetails.CustomerID = @CustomerID AND StockTransferDetails.ReceiverID = @ReceiverID " : "") + (!isStockTransferID && !isCustomerID && !isReceiverID ? " StockTransferDetails.StockTransferID IN (SELECT StockTransferID FROM StockTransfers WHERE EntryDate >= @FromDate AND EntryDate <= @ToDate AND LocationID = @LocationID AND OrganizationalUnitID IN (SELECT AccessControls.OrganizationalUnitID FROM AccessControls INNER JOIN AspNetUsers ON AccessControls.UserID = AspNetUsers.UserID WHERE AspNetUsers.Id = @AspUserID AND AccessControls.NMVNTaskID = " + (int)TotalBase.Enums.GlobalEnums.NmvnTaskID.StockTransfer + " AND AccessControls.AccessLevel = 2)) " : "") + (!GlobalEnums.VATbyRow && !isStockTransferID ? " AND StockTransferDetails.VATPercent = @VATPercent " : "") + " AND (ROUND(StockTransferDetails.Quantity - StockTransferDetails.QuantityInvoiced, 0) > 0 OR ROUND(StockTransferDetails.FreeQuantity - StockTransferDetails.FreeQuantityInvoiced, 0) > 0) AND StockTransferDetails.CommodityID = Commodities.CommodityID AND StockTransferDetails.Approved = 1 " + (isCommodityTypeID ? " AND Commodities.CommodityTypeID = @CommodityTypeID" : "") + (isStockTransferDetailIDs ? " AND StockTransferDetails.StockTransferDetailID NOT IN (SELECT Id FROM dbo.SplitToIntList (@StockTransferDetailIDs))" : "") + "\r\n";
            queryString = queryString + "                   INNER JOIN Customers ON StockTransferDetails.CustomerID = Customers.CustomerID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Customers Receivers ON StockTransferDetails.ReceiverID = Receivers.CustomerID " + "\r\n";
            queryString = queryString + "                   INNER JOIN StockTransfers ON StockTransferDetails.StockTransferID = StockTransfers.StockTransferID " + (!isStockTransferID ? " AND (StockTransfers.TradePromotionID = @TradePromotionID OR (StockTransfers.TradePromotionID IS NULL AND @TradePromotionID IS NULL)) " : "") + "\r\n";

            return queryString;
        }

        private string GetPGIDsBuildSQLEdit(bool isStockTransferID, bool isCustomerID, bool isReceiverID, bool isCommodityTypeID, bool isStockTransferDetailIDs)
        {
            string queryString = "";

            queryString = queryString + "       SELECT      StockTransfers.StockTransferID, StockTransferDetails.StockTransferDetailID, StockTransfers.Reference AS StockTransferReference, StockTransfers.Code AS StockTransferCode, StockTransfers.EntryDate AS StockTransferEntryDate, Commodities.CommodityID, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, Commodities.CommodityTypeID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, Customers.BillingAddress, Receivers.Code AS ReceiverCode, Receivers.Name AS ReceiverName, WarehouseInvoiceDetails.CalculatingTypeID, WarehouseInvoiceDetails.VATbyRow, ROUND(StockTransferDetails.Quantity - StockTransferDetails.QuantityInvoiced + WarehouseInvoiceDetails.Quantity, 0) AS QuantityRemains, ROUND(StockTransferDetails.FreeQuantity - StockTransferDetails.FreeQuantityInvoiced + WarehouseInvoiceDetails.FreeQuantity, 0) AS FreeQuantityRemains, StockTransferDetails.ControlFreeQuantity, " + "\r\n";
            queryString = queryString + "                   WarehouseInvoiceDetails.Quantity, WarehouseInvoiceDetails.FreeQuantity, WarehouseInvoiceDetails.ListedPrice, WarehouseInvoiceDetails.DiscountPercent, WarehouseInvoiceDetails.UnitPrice, WarehouseInvoiceDetails.TradeDiscountRate, WarehouseInvoiceDetails.VATPercent, WarehouseInvoiceDetails.ListedGrossPrice, WarehouseInvoiceDetails.GrossPrice, WarehouseInvoiceDetails.ListedAmount, WarehouseInvoiceDetails.Amount, WarehouseInvoiceDetails.ListedVATAmount, WarehouseInvoiceDetails.VATAmount, WarehouseInvoiceDetails.ListedGrossAmount, WarehouseInvoiceDetails.GrossAmount, WarehouseInvoiceDetails.IsBonus, CAST(1 AS bit) AS IsSelected " + "\r\n";

            queryString = queryString + "       FROM        StockTransferDetails " + "\r\n";
            queryString = queryString + "                   INNER JOIN WarehouseInvoiceDetails ON WarehouseInvoiceDetails.WarehouseInvoiceID = @WarehouseInvoiceID AND StockTransferDetails.StockTransferDetailID = WarehouseInvoiceDetails.StockTransferDetailID AND StockTransferDetails.Approved = 1 " + (!isStockTransferID && !isCustomerID && !isReceiverID ? " AND StockTransferDetails.StockTransferID IN (SELECT StockTransferID FROM StockTransfers WHERE EntryDate >= @FromDate AND EntryDate <= @ToDate AND LocationID = @LocationID AND OrganizationalUnitID IN (SELECT AccessControls.OrganizationalUnitID FROM AccessControls INNER JOIN AspNetUsers ON AccessControls.UserID = AspNetUsers.UserID WHERE AspNetUsers.Id = @AspUserID AND AccessControls.NMVNTaskID = " + (int)TotalBase.Enums.GlobalEnums.NmvnTaskID.StockTransfer + " AND AccessControls.AccessLevel = 2)) " : "") + (isStockTransferDetailIDs ? " AND StockTransferDetails.StockTransferDetailID NOT IN (SELECT Id FROM dbo.SplitToIntList (@StockTransferDetailIDs))" : "") + "\r\n";
            queryString = queryString + "                   INNER JOIN Commodities ON StockTransferDetails.CommodityID = Commodities.CommodityID " + (isCommodityTypeID ? " AND Commodities.CommodityTypeID = @CommodityTypeID" : "") + "\r\n";
            queryString = queryString + "                   INNER JOIN Customers ON StockTransferDetails.CustomerID = Customers.CustomerID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Customers Receivers ON StockTransferDetails.ReceiverID = Receivers.CustomerID " + "\r\n";
            queryString = queryString + "                   INNER JOIN StockTransfers ON StockTransferDetails.StockTransferID = StockTransfers.StockTransferID " + "\r\n";

            return queryString;
        }


        private void WarehouseInvoiceSaveRelative()
        {
            string queryString = " @EntityID int, @SaveRelativeOption int " + "\r\n"; //SaveRelativeOption: 1: Update, -1:Undo
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";
            queryString = queryString + "    BEGIN " + "\r\n";

            queryString = queryString + "       UPDATE          StockTransferDetails " + "\r\n";
            queryString = queryString + "       SET             StockTransferDetails.QuantityInvoiced = ROUND(StockTransferDetails.QuantityInvoiced + WarehouseInvoiceDetails.Quantity * @SaveRelativeOption, 0), StockTransferDetails.FreeQuantityInvoiced = ROUND(StockTransferDetails.FreeQuantityInvoiced + WarehouseInvoiceDetails.FreeQuantity * @SaveRelativeOption, 0) " + "\r\n";
            queryString = queryString + "       FROM            WarehouseInvoiceDetails INNER JOIN " + "\r\n";
            queryString = queryString + "                       StockTransferDetails ON WarehouseInvoiceDetails.WarehouseInvoiceID = @EntityID AND WarehouseInvoiceDetails.StockTransferDetailID = StockTransferDetails.StockTransferDetailID " + "\r\n";

            queryString = queryString + "    END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("WarehouseInvoiceSaveRelative", queryString);
        }

        private void WarehouseInvoicePostSaveValidate()
        {
            string[] queryArray = new string[3];

            queryArray[0] = " SELECT TOP 1 @FoundEntity = N'Ngày xuất kho: ' + CAST(StockTransferDetails.EntryDate AS nvarchar) FROM WarehouseInvoiceDetails INNER JOIN StockTransferDetails ON WarehouseInvoiceDetails.WarehouseInvoiceID = @EntityID AND WarehouseInvoiceDetails.StockTransferDetailID = StockTransferDetails.StockTransferDetailID AND WarehouseInvoiceDetails.EntryDate < StockTransferDetails.EntryDate ";
            queryArray[1] = " SELECT TOP 1 @FoundEntity = N'Ngày xuất kho: ' + CAST(CAST(StockTransferDetails.EntryDate AS Date) AS nvarchar) + N' (Ngày HĐ phải sau ngày xuất kho)' FROM WarehouseInvoiceDetails INNER JOIN StockTransferDetails ON WarehouseInvoiceDetails.WarehouseInvoiceID = @EntityID AND WarehouseInvoiceDetails.StockTransferDetailID = StockTransferDetails.StockTransferDetailID AND WarehouseInvoiceDetails.VATInvoiceDate < CAST(StockTransferDetails.EntryDate AS Date) ";
            queryArray[2] = " SELECT TOP 1 @FoundEntity = N'Số lượng xuất hóa đơn vượt quá số lượng xuất kho: ' + CAST(ROUND(StockTransferDetails.Quantity - StockTransferDetails.QuantityInvoiced, " + (int)GlobalEnums.rndQuantity + ") AS nvarchar) + ' OR free quantity: ' + CAST(ROUND(StockTransferDetails.FreeQuantity - StockTransferDetails.FreeQuantityInvoiced, " + (int)GlobalEnums.rndQuantity + ") AS nvarchar) FROM WarehouseInvoiceDetails INNER JOIN StockTransferDetails ON WarehouseInvoiceDetails.WarehouseInvoiceID = @EntityID AND WarehouseInvoiceDetails.StockTransferDetailID = StockTransferDetails.StockTransferDetailID AND (ROUND(StockTransferDetails.Quantity - StockTransferDetails.QuantityInvoiced, " + (int)GlobalEnums.rndQuantity + ") < 0 OR ROUND(StockTransferDetails.FreeQuantity - StockTransferDetails.FreeQuantityInvoiced, " + (int)GlobalEnums.rndQuantity + ") < 0) ";

            this.totalSmartPortalEntities.CreateProcedureToCheckExisting("WarehouseInvoicePostSaveValidate", queryArray);
        }



        private void WarehouseInvoiceInitReference()
        {
            SimpleInitReference simpleInitReference = new SimpleInitReference("WarehouseInvoices", "WarehouseInvoiceID", "Reference", ModelSettingManager.ReferenceLength, ModelSettingManager.ReferencePrefix(GlobalEnums.NmvnTaskID.WarehouseInvoice));
            this.totalSmartPortalEntities.CreateTrigger("WarehouseInvoiceInitReference", simpleInitReference.CreateQuery());
        }

        private void WarehouseInvoiceSheet()
        {
            string queryString = " @WarehouseInvoiceID int " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";
            queryString = queryString + "    BEGIN " + "\r\n";

            queryString = queryString + "       DECLARE         @LocalWarehouseInvoiceID int    SET @LocalWarehouseInvoiceID = @WarehouseInvoiceID" + "\r\n";
            queryString = queryString + "       DECLARE         @CustomerCategoryID int    SELECT  @CustomerCategoryID = Customers.CustomerCategoryID FROM WarehouseInvoices INNER JOIN Customers ON WarehouseInvoices.WarehouseInvoiceID = @LocalWarehouseInvoiceID AND WarehouseInvoices.CustomerID = Customers.CustomerID " + "\r\n";

            queryString = queryString + "       SELECT          WarehouseInvoices.WarehouseInvoiceID, WarehouseInvoices.EntryDate, WarehouseInvoices.Reference, WarehouseInvoices.Code AS WarehouseInvoiceCode, WarehouseInvoices.VATInvoiceNo, WarehouseInvoices.VATInvoiceDate, WarehouseInvoices.VATInvoiceSeries, WarehouseInvoices.CustomerPO, " + "\r\n";
            queryString = queryString + "                       Customers.CustomerCategoryID, Customers.CustomerID, Customers.OfficialName AS CustomerOfficialName, Customers.VATCode, Customers.BillingAddress, PaymentTerms.Name AS PaymentTermName, " + "\r\n";
            queryString = queryString + "                       WarehouseInvoiceCollections.WarehouseInvoiceDetailID, WarehouseInvoiceCollections.IsFreebie, Commodities.CommodityID, Commodities.Code, Commodities.CodePartA, Commodities.CodePartB, ISNULL(Commodities.CodePartA + CASE WHEN @CustomerCategoryID = 999 THEN '' ELSE ' ' + Commodities.CodePartB END, WarehouseInvoiceCollections.LineDescription) AS LineDescription, CommoditySKU.CodeSKU, Commodities.OfficialName, Commodities.SalesUnit, " + "\r\n";
            queryString = queryString + "                       WarehouseInvoiceCollections.Quantity, WarehouseInvoiceCollections.ListedPrice, WarehouseInvoiceCollections.ListedAmount, WarehouseInvoiceCollections.DiscountPercent, ROUND(WarehouseInvoiceCollections.ListedAmount - WarehouseInvoiceCollections.Amount, " + (int)GlobalEnums.rndAmount + ") AS DiscountAmount, WarehouseInvoiceCollections.UnitPrice, WarehouseInvoiceCollections.Amount, " + (GlobalEnums.VATbyRow ? "WarehouseInvoiceCollections.VATPercent" : "WarehouseInvoices.VATPercent") + " AS VATPercent, WarehouseInvoiceCollections.VATAmount, WarehouseInvoiceCollections.GrossAmount, " + "\r\n";
            queryString = queryString + "                       WarehouseInvoices.TotalQuantity, WarehouseInvoices.TotalAmount, WarehouseInvoices.TotalVATAmount, WarehouseInvoices.TotalGrossAmount, dbo.SayVND(WarehouseInvoices.TotalGrossAmount) AS TotalGrossAmountInWords, WarehouseInvoices.Description " + "\r\n";

            queryString = queryString + "       FROM            (SELECT WarehouseInvoiceID, WarehouseInvoiceDetailID, CommodityID, N'' AS LineDescription, Quantity, ListedPrice, ListedAmount, DiscountPercent, UnitPrice, Amount, VATPercent, VATAmount, GrossAmount, 0 AS IsFreebie FROM WarehouseInvoiceDetails WHERE WarehouseInvoiceID = @LocalWarehouseInvoiceID " + "\r\n";
            queryString = queryString + "                       UNION ALL " + "\r\n";
            queryString = queryString + "                       SELECT WarehouseInvoiceID, WarehouseInvoiceDetailID, CommodityID, N'' AS LineDescription, FreeQuantity AS Quantity, 0 AS ListedPrice, 0 AS ListedAmount, 0 AS DiscountPercent, 0 AS UnitPrice, 0 AS Amount, 0 AS VATPercent, 0 AS VATAmount, 0 AS GrossAmount, 1 AS IsFreebie FROM WarehouseInvoiceDetails WHERE WarehouseInvoiceID = @LocalWarehouseInvoiceID AND FreeQuantity <> 0 " + "\r\n";
            queryString = queryString + "                       UNION ALL " + "\r\n";
            queryString = queryString + "                       SELECT WarehouseInvoiceID, -1 AS WarehouseInvoiceDetailID, -1 AS CommodityID, N'Chiết khấu: ' + CAST(CAST(TradeDiscountRate AS Decimal(18, 1)) AS nvarchar(10)) + '%' AS LineDescription, 0 AS Quantity, 0 AS ListedPrice, -TradeDiscountAmount AS ListedAmount, 0 AS DiscountPercent, 0 AS UnitPrice, -TradeDiscountAmount AS Amount, 0 AS VATPercent, 0 AS VATAmount, 0 AS GrossAmount, 2 AS IsFreebie FROM WarehouseInvoices WHERE WarehouseInvoiceID = @LocalWarehouseInvoiceID AND TradeDiscountAmount <> 0) AS WarehouseInvoiceCollections " + "\r\n";

            queryString = queryString + "                       INNER JOIN WarehouseInvoices ON WarehouseInvoiceCollections.WarehouseInvoiceID = WarehouseInvoices.WarehouseInvoiceID " + "\r\n";
            queryString = queryString + "                       INNER JOIN Customers ON WarehouseInvoices.CustomerID = Customers.CustomerID " + "\r\n";
            queryString = queryString + "                       INNER JOIN PaymentTerms ON WarehouseInvoices.PaymentTermID = PaymentTerms.PaymentTermID " + "\r\n";
            queryString = queryString + "                       LEFT JOIN Commodities ON WarehouseInvoiceCollections.CommodityID = Commodities.CommodityID " + "\r\n";
            queryString = queryString + "                       LEFT JOIN CommoditySKU ON CommoditySKU.CustomerCategoryID = @CustomerCategoryID AND Commodities.CodePartA = CommoditySKU.CodePartA AND Commodities.CodePartB = CommoditySKU.CodePartB " + "\r\n";

            queryString = queryString + "    END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("WarehouseInvoiceSheet", queryString);
        }

    }
}
