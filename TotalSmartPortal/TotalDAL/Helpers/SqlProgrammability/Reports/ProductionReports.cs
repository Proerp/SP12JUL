using System;
using System.Text;

using TotalBase;
using TotalBase.Enums;
using TotalModel.Models;

namespace TotalDAL.Helpers.SqlProgrammability.Reports
{
    public class ProductionReports
    {
        private readonly TotalSmartPortalEntities totalSmartPortalEntities;

        public ProductionReports(TotalSmartPortalEntities totalSmartPortalEntities)
        {
            this.totalSmartPortalEntities = totalSmartPortalEntities;
        }

        public void RestoreProcedure()
        {
            this.FirmOrderJournals();
            this.PlannedItemLoss();
        }

        private void FirmOrderJournals()
        {
            string queryString = " @WarehouseID int, @FromDate DateTime, @ToDate DateTime " + "\r\n"; //Filter by @LocalWarehouseID to make this stored procedure run faster, but it may be removed without any effect the algorithm

            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";
            queryString = queryString + "    BEGIN " + "\r\n";

            queryString = queryString + "       DECLARE     @LocalWarehouseID int, @LocalFromDate DateTime, @LocalToDate DateTime " + "\r\n";
            queryString = queryString + "       SET         @LocalWarehouseID = @WarehouseID    SET @LocalFromDate = @FromDate      SET @LocalToDate = @ToDate " + "\r\n";

            queryString = queryString + "       SELECT      FirmOrders.FirmOrderID, FirmOrders.EntryDate, FirmOrders.Reference, FirmOrders.Code, FirmOrders.VoucherDate, FirmOrders.DeliveryDate, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, FirmOrders.Description, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, FirmOrderDetails.InActive, FirmOrderDetails.InActivePartial, FirmOrderDetails.QuantityRequested, FirmOrderDetails.QuantityOnhand, FirmOrderDetails.Quantity, ISNULL(SemifinishedProductSummaries.SemifinishedQuantity, 0) + ISNULL(FinishedProductSummaries.FinishedQuantityExcess, 0) - ISNULL(FinishedProductSummaries.FinishedQuantityShortage, 0) - ISNULL(FinishedProductSummaries.FinishedQuantityFailure, 0) AS QuantityProduced, " + "\r\n";
            queryString = queryString + "                   Items.Code AS ItemCode, MaterialIssueSummaries.ItemQuantity, MaterialIssueSummaries.ItemQuantitySemifinished, MaterialIssueSummaries.ItemQuantityFailure, MaterialIssueSummaries.ItemQuantityReceipted, MaterialIssueSummaries.ItemQuantityLoss, " + "\r\n";
            queryString = queryString + "                   SemifinishedProductSummaries.SemifinishedQuantity, SemifinishedProductSummaries.SemifinishedQuantityRemains, FinishedProductSummaries.FinishedQuantity, FinishedProductSummaries.FinishedQuantityExcess, FinishedProductSummaries.FinishedQuantity + FinishedProductSummaries.FinishedQuantityExcess AS FinishedQuantityTotal, FinishedProductSummaries.FinishedQuantityShortage, FinishedProductSummaries.FinishedQuantityFailure, FinishedProductSummaries.FinishedSwarfs " + "\r\n";

            queryString = queryString + "       FROM        FirmOrders " + "\r\n";
            queryString = queryString + "                   INNER JOIN FirmOrderDetails ON FirmOrders.FirmOrderID = FirmOrderDetails.FirmOrderID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Customers ON FirmOrders.CustomerID = Customers.CustomerID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Commodities ON FirmOrderDetails.CommodityID = Commodities.CommodityID " + "\r\n";

            queryString = queryString + "                   LEFT  JOIN (SELECT FirmOrderID, MIN(CommodityID) AS ItemID, SUM(Quantity) AS ItemQuantity, SUM(QuantitySemifinished) AS ItemQuantitySemifinished, SUM(QuantityFailure) AS ItemQuantityFailure, SUM(QuantityReceipted) AS ItemQuantityReceipted, SUM(QuantityLoss) AS ItemQuantityLoss FROM MaterialIssueDetails GROUP BY FirmOrderID) AS MaterialIssueSummaries ON FirmOrders.FirmOrderID = MaterialIssueSummaries.FirmOrderID " + "\r\n";
            queryString = queryString + "                   LEFT  JOIN Commodities AS Items ON MaterialIssueSummaries.ItemID = Items.CommodityID " + "\r\n";

            queryString = queryString + "                   LEFT  JOIN (SELECT FirmOrderDetailID, SUM(Quantity) AS SemifinishedQuantity, SUM(Quantity - QuantityFinished) AS SemifinishedQuantityRemains FROM SemifinishedProductDetails GROUP BY FirmOrderDetailID) SemifinishedProductSummaries ON FirmOrderDetails.FirmOrderDetailID = SemifinishedProductSummaries.FirmOrderDetailID " + "\r\n";
            queryString = queryString + "                   LEFT  JOIN (SELECT FirmOrderDetailID, SUM(Quantity) AS FinishedQuantity, SUM(QuantityFailure) AS FinishedQuantityFailure, SUM(QuantityExcess) AS FinishedQuantityExcess, SUM(QuantityShortage) AS FinishedQuantityShortage, SUM(Swarfs) AS FinishedSwarfs FROM FinishedProductDetails GROUP BY FirmOrderDetailID) AS FinishedProductSummaries ON FirmOrderDetails.FirmOrderDetailID = FinishedProductSummaries.FirmOrderDetailID " + "\r\n";

            queryString = queryString + "    END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("FirmOrderJournals", queryString);
        }

        private void PlannedItemLoss()
        {
            string queryString = " @FromDate DateTime, @ToDate DateTime " + "\r\n";

            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";
            queryString = queryString + "    BEGIN " + "\r\n";

            queryString = queryString + "       DECLARE     @LocalFromDate DateTime,        @LocalToDate DateTime " + "\r\n";
            queryString = queryString + "       SET         @LocalFromDate = @FromDate      SET @LocalToDate = @ToDate " + "\r\n";

            queryString = queryString + "       SELECT      FirmOrders.FirmOrderID, FirmOrders.EntryDate, FirmOrders.Reference, FirmOrders.Code, FirmOrders.VoucherDate, FirmOrders.DeliveryDate, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, Boms.Code AS BomCode, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, FirmOrders.TotalQuantity, UNIQUE_FirmOrders.QuantityLoss, UNIQUE_FirmOrders.QuantityFailure " + "\r\n";
            queryString = queryString + "       FROM        ( " + "\r\n";
            queryString = queryString + "                   SELECT      FirmOrderID, SUM(QuantityLoss) AS QuantityLoss, SUM(QuantityFailure) AS QuantityFailure " + "\r\n";
            queryString = queryString + "                   FROM        ( " + "\r\n";
            queryString = queryString + "                               SELECT      FirmOrderID, TotalQuantityFailure AS QuantityLoss, 0 AS QuantityFailure FROM SemifinishedItems WHERE TotalQuantityFailure > 0 AND EntryDate >= @LocalFromDate AND EntryDate <= @LocalToDate " + "\r\n";
            queryString = queryString + "                               UNION ALL   " + "\r\n";
            queryString = queryString + "                               SELECT      FirmOrderID, 0 AS QuantityLoss, QuantityFailure + QuantityShortage + Swarfs - QuantityExcess AS QuantityFailure FROM FinishedItemDetails WHERE EntryDate >= @LocalFromDate AND EntryDate <= @LocalToDate " + "\r\n";
            queryString = queryString + "                               ) RAW_FirmOrders GROUP BY FirmOrderID HAVING SUM(QuantityLoss) > 0 OR SUM(QuantityFailure) > 0 " + "\r\n";
            queryString = queryString + "                   ) UNIQUE_FirmOrders " + "\r\n";
            queryString = queryString + "                   INNER JOIN FirmOrders ON UNIQUE_FirmOrders.FirmOrderID = FirmOrders.FirmOrderID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Boms ON FirmOrders.BomID = Boms.BomID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Customers ON FirmOrders.CustomerID = Customers.CustomerID " + "\r\n";
            queryString = queryString + "                   INNER JOIN FirmOrderDetails ON FirmOrders.FirmOrderID = FirmOrderDetails.FirmOrderID " + "\r\n"; //HERE NMVNTaskID.PlannedItem = 680016008 ==> ONLY ONE ROW FirmOrderDetails FOR EACH FirmOrderID
            queryString = queryString + "                   INNER JOIN Commodities ON FirmOrderDetails.CommodityID = Commodities.CommodityID " + "\r\n";

            queryString = queryString + "    END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("PlannedItemLoss", queryString);
        }
    }
}
