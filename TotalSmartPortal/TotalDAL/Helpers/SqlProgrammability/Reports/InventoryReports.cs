﻿using System;
using System.Text;

using TotalBase;
using TotalBase.Enums;
using TotalModel.Models;

namespace TotalDAL.Helpers.SqlProgrammability.Reports
{
    public class InventoryReports
    {
        private readonly TotalSmartPortalEntities totalSmartPortalEntities;

        public InventoryReports(TotalSmartPortalEntities totalSmartPortalEntities)
        {
            this.totalSmartPortalEntities = totalSmartPortalEntities;
        }

        public void RestoreProcedure()
        {
            this.WarehouseJournals();
            this.WarehouseCards();
        }



        private void WarehouseJournals()
        {
            string queryString = " @WarehouseID int, @FromDate DateTime, @ToDate DateTime " + "\r\n"; //Filter by @LocalWarehouseID to make this stored procedure run faster, but it may be removed without any effect the algorithm

            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";
            queryString = queryString + "    BEGIN " + "\r\n";

            queryString = queryString + "       DECLARE     @LocalFromDate DateTime, @LocalToDate DateTime " + "\r\n";
            queryString = queryString + "       SET         @LocalFromDate = @FromDate          SET @LocalToDate = @ToDate " + "\r\n";

            queryString = queryString + "       DECLARE     @LocalWarehouseID int, @LocationID int " + "\r\n";
            queryString = queryString + "       SET         @LocalWarehouseID = @WarehouseID    SET @LocationID = 0    " + "\r\n";

            queryString = queryString + "       IF         (@WarehouseID <= 0 ) " + "\r\n";
            queryString = queryString + "                   " + this.WarehouseJournalBUILD(true) + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "                   " + this.WarehouseJournalBUILD(false) + "\r\n";

            queryString = queryString + "    END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("WarehouseJournals", queryString);
        }


        private string WarehouseJournalBUILD(bool allWarehouses)
        {
            string queryString = "" + "\r\n";

            queryString = queryString + "    BEGIN " + "\r\n";

            if (!allWarehouses)
                queryString = queryString + "   SET         @LocationID = (SELECT LocationID FROM Warehouses WHERE WarehouseID = @LocalWarehouseID) " + "\r\n";

            //IIF(GoodsReceiptDetails.EntryDate >= @LocalFromDate, '   ' + CONVERT(VARCHAR, @LocalFromDate, 103) + ' -> ' + CONVERT(VARCHAR, @LocalToDate, 103), '     DAU KY ' + CONVERT(VARCHAR, DATEADD (day, -1,  @LocalFromDate), 103)) AS GroupName, 

            queryString = queryString + "       SELECT      800000 + @LocalWarehouseID AS WarehouseCardID, WarehouseJournalMaster.GroupName, Commodities.CommodityID, Commodities.Code, Commodities.Name, Commodities.SalesUnit, Commodities.LeadTime, " + "\r\n";
            queryString = queryString + "                   WarehouseJournalMaster.GoodsReceiptDetailID, WarehouseJournalMaster.EntryDate, WarehouseJournalMaster.Reference, WarehouseJournalMaster.ReceiptCode, " + "\r\n";
            queryString = queryString + "                   ISNULL(Warehouses.LocationID, 0) AS LocationID, ISNULL(Warehouses.WarehouseCategoryID, 0) AS WarehouseCategoryID, ISNULL(Warehouses.WarehouseID, 0) AS WarehouseID, ISNULL(Warehouses.Name, '') AS WarehouseName, " + "\r\n";
            queryString = queryString + "                   Customers.CustomerTypeID, Customers.CustomerID, ISNULL(Customers.OfficialName, '') AS CustomerName, " + "\r\n";

            queryString = queryString + "                   WarehouseJournalMaster.QuantityBegin, WarehouseJournalMaster.QuantityInputINV, WarehouseJournalMaster.QuantityInputPRO, WarehouseJournalMaster.QuantityInputPRN, WarehouseJournalMaster.QuantityInputRTN, WarehouseJournalMaster.QuantityInputTRF, WarehouseJournalMaster.QuantityInputADJ, WarehouseJournalMaster.QuantityInputINV + WarehouseJournalMaster.QuantityInputPRO + WarehouseJournalMaster.QuantityInputPRN + WarehouseJournalMaster.QuantityInputRTN + WarehouseJournalMaster.QuantityInputTRF + WarehouseJournalMaster.QuantityInputADJ AS QuantityInput, " + "\r\n";
            queryString = queryString + "                   WarehouseJournalMaster.QuantityIssuedINV, WarehouseJournalMaster.QuantityIssuedPRO, WarehouseJournalMaster.QuantityIssuedTRF, WarehouseJournalMaster.QuantityIssuedADJ, WarehouseJournalMaster.QuantityIssuedINV + WarehouseJournalMaster.QuantityIssuedPRO + WarehouseJournalMaster.QuantityIssuedTRF + WarehouseJournalMaster.QuantityIssuedADJ AS QuantityIssued, " + "\r\n";
            queryString = queryString + "                   WarehouseJournalMaster.QuantityBegin + WarehouseJournalMaster.QuantityInputINV + WarehouseJournalMaster.QuantityInputPRO + WarehouseJournalMaster.QuantityInputPRN + WarehouseJournalMaster.QuantityInputRTN + WarehouseJournalMaster.QuantityInputTRF + WarehouseJournalMaster.QuantityInputADJ - WarehouseJournalMaster.QuantityIssuedINV - WarehouseJournalMaster.QuantityIssuedPRO - WarehouseJournalMaster.QuantityIssuedTRF - WarehouseJournalMaster.QuantityIssuedADJ AS QuantityEnd, " + "\r\n";
            queryString = queryString + "                   WarehouseJournalMaster.QuantityOnPurchasing, WarehouseJournalMaster.QuantityOnReceipt, " + "\r\n";
            queryString = queryString + "                   WarehouseJournalMaster.MovementMIN, WarehouseJournalMaster.MovementMAX, WarehouseJournalMaster.MovementAVG, " + "\r\n";

            queryString = queryString + "                   EntireCommodityCategories.CommodityCategoryID, " + "\r\n";
            queryString = queryString + "                   EntireCommodityCategories.Name1 AS CommodityCategory1, " + "\r\n";
            queryString = queryString + "                   EntireCommodityCategories.Name2 AS CommodityCategory2, " + "\r\n";
            queryString = queryString + "                   EntireCommodityCategories.Name3 AS CommodityCategory3 " + "\r\n";

            queryString = queryString + "       FROM       (" + "\r\n";

            //--BEGIN-INPUT-OUTPUT-END.END
            queryString = queryString + "                   SELECT  GoodsReceiptDetailUnionMaster.GroupName, GoodsReceiptDetails.EntryDate, GoodsReceiptDetails.GoodsReceiptDetailID, GoodsReceiptDetails.CommodityID, GoodsReceiptDetails.CustomerID, GoodsReceiptDetails.Reference, GoodsReceiptDetails.Code AS ReceiptCode, GoodsReceiptDetails.WarehouseID, " + "\r\n";
            queryString = queryString + "                           GoodsReceiptDetailUnionMaster.QuantityBegin, GoodsReceiptDetailUnionMaster.QuantityInputINV, GoodsReceiptDetailUnionMaster.QuantityInputPRO, GoodsReceiptDetailUnionMaster.QuantityInputPRN, GoodsReceiptDetailUnionMaster.QuantityInputRTN, GoodsReceiptDetailUnionMaster.QuantityInputTRF, GoodsReceiptDetailUnionMaster.QuantityInputADJ, GoodsReceiptDetailUnionMaster.QuantityIssuedINV, GoodsReceiptDetailUnionMaster.QuantityIssuedPRO, GoodsReceiptDetailUnionMaster.QuantityIssuedTRF, GoodsReceiptDetailUnionMaster.QuantityIssuedADJ, 0 AS QuantityOnPurchasing, 0 AS QuantityOnReceipt, GoodsReceiptDetailUnionMaster.MovementMIN, GoodsReceiptDetailUnionMaster.MovementMAX, GoodsReceiptDetailUnionMaster.MovementAVG " + "\r\n";

            // NOTE 24.APR.2007: VIEC TINH GIA TON KHO (GoodsReceiptDetails.AmountCostCUR + GoodsReceiptDetails.AmountClearanceCUR)/ GoodsReceiptDetails.Quantity AS UPriceCURInventory, (GoodsReceiptDetails.AmountCostUSD + GoodsReceiptDetails.AmountClearanceUSD)/ GoodsReceiptDetails.Quantity AS UPriceNMDInventory
            // SU DUNG CONG THUC TREN CHI TAM THOI MA THOI, CO THE DAN DEN SAI SO (SU DUNG TAM THOI DE IN BAO CAO KHO CO SO LIEU)
            // SAU NAY NEN SUA LAI, SU DUNG PHEP +/ - MA THOI
            // XEM SPWHAmountCostofsalesGet DE TINH LUONG REMAIN NHE

            queryString = queryString + "                   FROM   (" + "\r\n";
            queryString = queryString + "                           SELECT  GoodsReceiptDetailUnion.GroupName, GoodsReceiptDetailUnion.GoodsReceiptDetailID, " + "\r\n";
            queryString = queryString + "                                   SUM(QuantityBegin) AS QuantityBegin, SUM(QuantityInputINV) AS QuantityInputINV, SUM(QuantityInputPRO) AS QuantityInputPRO, SUM(QuantityInputPRN) AS QuantityInputPRN, SUM(QuantityInputRTN) AS QuantityInputRTN, SUM(QuantityInputTRF) AS QuantityInputTRF, SUM(QuantityInputADJ) AS QuantityInputADJ, SUM(QuantityIssuedINV) AS QuantityIssuedINV, SUM(QuantityIssuedPRO) AS QuantityIssuedPRO, SUM(QuantityIssuedTRF) AS QuantityIssuedTRF, SUM(QuantityIssuedADJ) AS QuantityIssuedADJ, " + "\r\n";
            queryString = queryString + "                                   MIN(MovementDate) AS MovementMIN, MAX(MovementDate) AS MovementMAX, SUM((QuantityIssuedINV + QuantityIssuedPRO + QuantityIssuedTRF + QuantityIssuedADJ) * MovementDate) / SUM(QuantityIssuedINV + QuantityIssuedPRO + QuantityIssuedTRF + QuantityIssuedADJ) AS MovementAVG " + "\r\n";
            queryString = queryString + "                           FROM    (" + "\r\n";





            //BEGINING
            //WHINPUT
            queryString = queryString + "                                   SELECT      ' DAU KY ' + CONVERT(VARCHAR, DATEADD (day, -1,  @LocalFromDate), 103) AS GroupName, GoodsReceiptDetails.GoodsReceiptDetailID, ROUND(GoodsReceiptDetails.Quantity - GoodsReceiptDetails.QuantityIssued, " + (int)GlobalEnums.rndQuantity + ") AS QuantityBegin, 0 AS QuantityInputINV, 0 AS QuantityInputPRO, 0 AS QuantityInputPRN, 0 AS QuantityInputRTN, 0 AS QuantityInputTRF, 0 AS QuantityInputADJ, 0 AS QuantityIssuedINV, 0 AS QuantityIssuedPRO, 0 AS QuantityIssuedTRF, 0 AS QuantityIssuedADJ, NULL AS MovementDate " + "\r\n";
            queryString = queryString + "                                   FROM        GoodsReceiptDetails " + "\r\n";
            queryString = queryString + "                                   WHERE       " + (allWarehouses ? "" : " GoodsReceiptDetails.WarehouseID = @LocalWarehouseID AND ") + " GoodsReceiptDetails.EntryDate < @LocalFromDate AND GoodsReceiptDetails.Quantity > GoodsReceiptDetails.QuantityIssued " + "\r\n";

            queryString = queryString + "                                   UNION ALL " + "\r\n";
            ////UNDO (CAC CAU SQL CHO INVOICE, WarehouseTransferDetails, WHADJUST, WHASSEMBLY LA HOAN TOAN GIONG NHAU. LUU Y T/H DAT BIET: WHADJUST.QUANTITY < 0)
            //UNDO MaterialIssueDetails
            queryString = queryString + "                                   SELECT      ' DAU KY ' + CONVERT(VARCHAR, DATEADD (day, -1,  @LocalFromDate), 103) AS GroupName, GoodsReceiptDetails.GoodsReceiptDetailID, MaterialIssueDetails.Quantity AS QuantityBegin, 0 AS QuantityInputINV, 0 AS QuantityInputPRO, 0 AS QuantityInputPRN, 0 AS QuantityInputRTN, 0 AS QuantityInputTRF, 0 AS QuantityInputADJ, 0 AS QuantityIssuedINV, 0 AS QuantityIssuedPRO, 0 AS QuantityIssuedTRF, 0 AS QuantityIssuedADJ, NULL AS MovementDate " + "\r\n";
            queryString = queryString + "                                   FROM        GoodsReceiptDetails INNER JOIN " + "\r\n";
            queryString = queryString + "                                               MaterialIssueDetails ON " + (allWarehouses ? "" : " GoodsReceiptDetails.WarehouseID = @LocalWarehouseID AND ") + "GoodsReceiptDetails.GoodsReceiptDetailID = MaterialIssueDetails.GoodsReceiptDetailID AND GoodsReceiptDetails.EntryDate < @LocalFromDate AND MaterialIssueDetails.EntryDate >= @LocalFromDate " + "\r\n";

            queryString = queryString + "                                   UNION ALL " + "\r\n";
            //UNDO WarehouseTransferDetails
            queryString = queryString + "                                   SELECT      ' DAU KY ' + CONVERT(VARCHAR, DATEADD (day, -1,  @LocalFromDate), 103) AS GroupName, GoodsReceiptDetails.GoodsReceiptDetailID, WarehouseTransferDetails.Quantity AS QuantityBegin, 0 AS QuantityInputINV, 0 AS QuantityInputPRO, 0 AS QuantityInputPRN, 0 AS QuantityInputRTN, 0 AS QuantityInputTRF, 0 AS QuantityInputADJ, 0 AS QuantityIssuedINV, 0 AS QuantityIssuedPRO, 0 AS QuantityIssuedTRF, 0 AS QuantityIssuedADJ, NULL AS MovementDate " + "\r\n";
            queryString = queryString + "                                   FROM        GoodsReceiptDetails INNER JOIN " + "\r\n";
            queryString = queryString + "                                               WarehouseTransferDetails ON " + (allWarehouses ? "" : " GoodsReceiptDetails.WarehouseID = @LocalWarehouseID AND ") + "GoodsReceiptDetails.GoodsReceiptDetailID = WarehouseTransferDetails.GoodsReceiptDetailID AND GoodsReceiptDetails.EntryDate < @LocalFromDate AND WarehouseTransferDetails.EntryDate >= @LocalFromDate " + "\r\n";

            queryString = queryString + "                                   UNION ALL " + "\r\n";
            //UNDO WarehouseAdjustmentDetails
            queryString = queryString + "                                   SELECT      ' DAU KY ' + CONVERT(VARCHAR, DATEADD (day, -1,  @LocalFromDate), 103) AS GroupName, GoodsReceiptDetails.GoodsReceiptDetailID, -WarehouseAdjustmentDetails.Quantity AS QuantityBegin, 0 AS QuantityInputINV, 0 AS QuantityInputPRO, 0 AS QuantityInputPRN, 0 AS QuantityInputRTN, 0 AS QuantityInputTRF, 0 AS QuantityInputADJ, 0 AS QuantityIssuedINV, 0 AS QuantityIssuedPRO, 0 AS QuantityIssuedTRF, 0 AS QuantityIssuedADJ, NULL AS MovementDate " + "\r\n";
            queryString = queryString + "                                   FROM        GoodsReceiptDetails INNER JOIN " + "\r\n";
            queryString = queryString + "                                               WarehouseAdjustmentDetails ON " + (allWarehouses ? "" : " GoodsReceiptDetails.WarehouseID = @LocalWarehouseID AND ") + " GoodsReceiptDetails.GoodsReceiptDetailID = WarehouseAdjustmentDetails.GoodsReceiptDetailID AND GoodsReceiptDetails.EntryDate < @LocalFromDate AND WarehouseAdjustmentDetails.EntryDate >= @LocalFromDate " + "\r\n";








            //INTPUT
            queryString = queryString + "                                   UNION ALL " + "\r\n";
            queryString = queryString + "                                   SELECT      CONVERT(VARCHAR, @LocalFromDate, 103) + ' -> ' + CONVERT(VARCHAR, @LocalToDate, 103) AS GroupName, GoodsReceiptDetails.GoodsReceiptDetailID, 0 AS QuantityBegin, " + "\r\n";
            queryString = queryString + "                                               CASE WHEN GoodsReceiptDetails.GoodsReceiptTypeID = " + (int)GlobalEnums.GoodsReceiptTypeID.PurchaseRequisition + " THEN GoodsReceiptDetails.Quantity ELSE 0 END AS QuantityInputINV, " + "\r\n";
            queryString = queryString + "                                               CASE WHEN GoodsReceiptDetails.GoodsReceiptTypeID = " + (int)GlobalEnums.GoodsReceiptTypeID.FinishedProduct + " THEN GoodsReceiptDetails.Quantity ELSE 0 END AS QuantityInputPRO, " + "\r\n";
            queryString = queryString + "                                               CASE WHEN GoodsReceiptDetails.GoodsReceiptTypeID = " + (int)GlobalEnums.GoodsReceiptTypeID.MaterialIssue + " THEN GoodsReceiptDetails.Quantity ELSE 0 END AS QuantityInputPRN, " + "\r\n";
            queryString = queryString + "                                               CASE WHEN GoodsReceiptDetails.GoodsReceiptTypeID = " + (int)GlobalEnums.GoodsReceiptTypeID.GoodsReturn + " THEN GoodsReceiptDetails.Quantity ELSE 0 END AS QuantityInputRTN, " + "\r\n";
            queryString = queryString + "                                               CASE WHEN GoodsReceiptDetails.GoodsReceiptTypeID = " + (int)GlobalEnums.GoodsReceiptTypeID.WarehouseTransfer + " THEN GoodsReceiptDetails.Quantity ELSE 0 END AS QuantityInputTRF, " + "\r\n";
            queryString = queryString + "                                               CASE WHEN GoodsReceiptDetails.GoodsReceiptTypeID = " + (int)GlobalEnums.GoodsReceiptTypeID.WarehouseAdjustments + " THEN GoodsReceiptDetails.Quantity ELSE 0 END AS QuantityInputADJ, " + "\r\n";
            queryString = queryString + "                                               0 AS QuantityIssuedINV, 0 AS QuantityIssuedPRO, 0 AS QuantityIssuedTRF, 0 AS QuantityIssuedADJ, NULL AS MovementDate " + "\r\n";
            queryString = queryString + "                                   FROM        GoodsReceiptDetails " + "\r\n";
            queryString = queryString + "                                   WHERE       " + (allWarehouses ? "" : " GoodsReceiptDetails.WarehouseID = @LocalWarehouseID AND ") + " GoodsReceiptDetails.EntryDate >= @LocalFromDate AND GoodsReceiptDetails.EntryDate <= @LocalToDate " + "\r\n";











            //OUTPUT (CAC CAU SQL CHO INVOICE, WarehouseTransferDetails, WHADJUST, WHASSEMBLY LA HOAN TOAN GIONG NHAU. LUU Y T/H DAT BIET: WHADJUST.QUANTITY < 0)
            queryString = queryString + "                                   UNION ALL " + "\r\n";
            //MaterialIssueDetails
            queryString = queryString + "                                   SELECT      CONVERT(VARCHAR, @LocalFromDate, 103) + ' -> ' + CONVERT(VARCHAR, @LocalToDate, 103) AS GroupName, MaterialIssueDetails.GoodsReceiptDetailID, 0 AS QuantityBegin, 0 AS QuantityInputINV, 0 AS QuantityInputPRO, 0 AS QuantityInputPRN, 0 AS QuantityInputRTN, 0 AS QuantityInputTRF, 0 AS QuantityInputADJ, 0 AS QuantityIssuedINV, MaterialIssueDetails.Quantity AS QuantityIssuedPRO, 0 AS QuantityIssuedTRF, 0 AS QuantityIssuedADJ, 0 AS MovementDate " + "\r\n"; //DATEDIFF(DAY, GoodsReceiptDetails.EntryDate, MaterialIssueDetails.EntryDate) AS MovementDate
            queryString = queryString + "                                   FROM        MaterialIssueDetails " + "\r\n";
            queryString = queryString + "                                   WHERE       " + (allWarehouses ? "" : "MaterialIssueDetails.WarehouseID = @LocalWarehouseID AND ") + "MaterialIssueDetails.EntryDate >= @LocalFromDate AND MaterialIssueDetails.EntryDate <= @LocalToDate " + "\r\n";

            queryString = queryString + "                                   UNION ALL " + "\r\n";
            //WarehouseTransferDetails
            queryString = queryString + "                                   SELECT      CONVERT(VARCHAR, @LocalFromDate, 103) + ' -> ' + CONVERT(VARCHAR, @LocalToDate, 103) AS GroupName, WarehouseTransferDetails.GoodsReceiptDetailID, 0 AS QuantityBegin, 0 AS QuantityInputINV, 0 AS QuantityInputPRO, 0 AS QuantityInputPRN, 0 AS QuantityInputRTN, 0 AS QuantityInputTRF, 0 AS QuantityInputADJ, 0 AS QuantityIssuedINV, 0 AS QuantityIssuedPRO, WarehouseTransferDetails.Quantity AS QuantityIssuedTRF, 0 AS QuantityIssuedADJ, 0 AS MovementDate " + "\r\n"; //DATEDIFF(DAY, GoodsReceiptDetails.EntryDate, WarehouseTransferDetails.EntryDate) AS MovementDate
            queryString = queryString + "                                   FROM        WarehouseTransferDetails " + "\r\n";
            queryString = queryString + "                                   WHERE       " + (allWarehouses ? "" : "WarehouseTransferDetails.WarehouseID = @LocalWarehouseID AND ") + "WarehouseTransferDetails.EntryDate >= @LocalFromDate AND WarehouseTransferDetails.EntryDate <= @LocalToDate " + "\r\n";

            queryString = queryString + "                                   UNION ALL " + "\r\n";
            //WarehouseAdjustmentDetails
            queryString = queryString + "                                   SELECT      CONVERT(VARCHAR, @LocalFromDate, 103) + ' -> ' + CONVERT(VARCHAR, @LocalToDate, 103) AS GroupName, WarehouseAdjustmentDetails.GoodsReceiptDetailID, 0 AS QuantityBegin, 0 AS QuantityInputINV, 0 AS QuantityInputPRO, 0 AS QuantityInputPRN, 0 AS QuantityInputRTN, 0 AS QuantityInputTRF, 0 AS QuantityInputADJ, 0 AS QuantityIssuedINV, 0 AS QuantityIssuedPRO, 0 AS QuantityIssuedTRF, -WarehouseAdjustmentDetails.Quantity AS QuantityIssuedADJ, 0 AS MovementDate " + "\r\n"; //DATEDIFF(DAY, GoodsReceiptDetails.EntryDate, WarehouseAdjustmentDetails.EntryDate) AS MovementDate
            queryString = queryString + "                                   FROM        WarehouseAdjustmentDetails " + "\r\n";
            queryString = queryString + "                                   WHERE       " + (allWarehouses ? "" : " WarehouseAdjustmentDetails.WarehouseID = @LocalWarehouseID AND ") + " WarehouseAdjustmentDetails.EntryDate >= @LocalFromDate AND WarehouseAdjustmentDetails.EntryDate <= @LocalToDate " + "\r\n";

            queryString = queryString + "                                   ) AS GoodsReceiptDetailUnion " + "\r\n";
            queryString = queryString + "                           GROUP BY GoodsReceiptDetailUnion.GroupName, GoodsReceiptDetailUnion.GoodsReceiptDetailID " + "\r\n";
            queryString = queryString + "                           ) AS GoodsReceiptDetailUnionMaster INNER JOIN " + "\r\n";
            queryString = queryString + "                           GoodsReceiptDetails ON GoodsReceiptDetailUnionMaster.GoodsReceiptDetailID = GoodsReceiptDetails.GoodsReceiptDetailID " + "\r\n";

            //--BEGIN-INPUT-OUTPUT-END.END
















            //////queryString = queryString + "                   UNION ALL " + "\r\n";
            ////////--ON SHIP.BEGIN
            //////queryString = queryString + "                   SELECT  CONVERT(smalldatetime, '" + new DateTime(1990, 1, 1).ToString("dd/MM/yyyy") + "', 103) AS EntryDate, 0 AS GoodsReceiptDetailID, PurchaseOrderDetails.CommodityID, PurchaseOrderDetails.CustomerID, '' AS Reference, '' AS ReceiptCode, 0 AS WarehouseID, " + "\r\n";
            //////queryString = queryString + "                           0 AS QuantityBegin, 0 AS QuantityInputINV, 0 AS QuantityInputPRO, 0 AS QuantityInputPRN, 0 AS QuantityInputRTN, 0 AS QuantityInputTRF, 0 AS QuantityInputADJ, 0 AS QuantityIssuedINV, 0 AS QuantityIssuedPRO, 0 AS QuantityIssuedTRF, 0 AS QuantityIssuedADJ, (PurchaseOrderDetails.Quantity - PurchaseOrderDetails.QuantityInvoice) AS QuantityOnPurchasing, 0 AS QuantityOnReceipt, 0 AS MovementMIN, 0 AS MovementMAX, 0 AS MovementAVG " + "\r\n";
            //////queryString = queryString + "                   FROM    PurchaseOrderDetails " + "\r\n";
            //////queryString = queryString + "                   WHERE   PurchaseOrderDetails.CommodityTypeID = " + (int)GlobalEnums.CommodityTypeID.Vehicles + (allWarehouses ? "" : " AND PurchaseOrderDetails.LocationID = @LocationID ") + " AND PurchaseOrderDetails.EntryDate <= @LocalToDate AND PurchaseOrderDetails.Quantity > PurchaseOrderDetails.QuantityInvoice " + "\r\n";

            //////queryString = queryString + "                   UNION ALL " + "\r\n";

            //////queryString = queryString + "                   SELECT  CONVERT(smalldatetime, '" + new DateTime(1990, 1, 1).ToString("dd/MM/yyyy") + "', 103) AS EntryDate, 0 AS GoodsReceiptDetailID, PurchaseInvoiceDetails.CommodityID, PurchaseOrders.CustomerID, '' AS Reference, '' AS ReceiptCode, 0 AS WarehouseID, " + "\r\n";
            //////queryString = queryString + "                           0 AS QuantityBegin, 0 AS QuantityInputINV, 0 AS QuantityInputPRO, 0 AS QuantityInputPRN, 0 AS QuantityInputRTN, 0 AS QuantityInputTRF, 0 AS QuantityInputADJ, 0 AS QuantityIssuedINV, 0 AS QuantityIssuedPRO, 0 AS QuantityIssuedTRF, 0 AS QuantityIssuedADJ, PurchaseInvoiceDetails.Quantity AS QuantityOnPurchasing, 0 AS QuantityOnReceipt, 0 AS MovementMIN, 0 AS MovementMAX, 0 AS MovementAVG " + "\r\n";
            //////queryString = queryString + "                   FROM    PurchaseOrders INNER JOIN " + "\r\n";
            //////queryString = queryString + "                           PurchaseInvoiceDetails ON PurchaseInvoiceDetails.CommodityTypeID = " + (int)GlobalEnums.CommodityTypeID.Vehicles + (allWarehouses ? "" : " AND PurchaseOrders.LocationID = @LocationID ") + " AND PurchaseOrders.PurchaseOrderID = PurchaseInvoiceDetails.PurchaseOrderID " + "\r\n";
            //////queryString = queryString + "                   WHERE   PurchaseOrders.EntryDate <= @LocalToDate AND PurchaseInvoiceDetails.EntryDate > @LocalToDate  " + "\r\n";
            ////////--ON SHIP.END













            //////queryString = queryString + "                   UNION ALL " + "\r\n";
            ////////--ON INPUT.BEGIN (CAC CAU SQL DUNG CHO EWHInputVoucherTypeID.EInvoice, EWHInputVoucherTypeID.EReturn, EWHInputVoucherTypeID.EWHTransfer, EWHInputVoucherTypeID.EWHAdjust, EWHInputVoucherTypeID.EWHAssemblyMaster, EWHInputVoucherTypeID.EWHAssemblyDetail LA HOAN TOAN GIONG NHAU)
            ////////EWHInputVoucherTypeID.EInvoice
            //////queryString = queryString + "                   SELECT  CONVERT(smalldatetime, '" + new DateTime(1990, 1, 1).ToString("dd/MM/yyyy") + "', 103) AS EntryDate, 0 AS GoodsReceiptDetailID, PurchaseInvoiceDetails.CommodityID, PurchaseInvoiceDetails.CustomerID, '' AS Reference, '' AS ReceiptCode, 0 AS WarehouseID, " + "\r\n";
            //////queryString = queryString + "                           0 AS QuantityBegin, 0 AS QuantityInputINV, 0 AS QuantityInputPRO, 0 AS QuantityInputPRN, 0 AS QuantityInputRTN, 0 AS QuantityInputTRF, 0 AS QuantityInputADJ, 0 AS QuantityIssuedINV, 0 AS QuantityIssuedPRO, 0 AS QuantityIssuedTRF, 0 AS QuantityIssuedADJ, 0 AS QuantityOnPurchasing, (PurchaseInvoiceDetails.Quantity - PurchaseInvoiceDetails.QuantityReceipt) AS QuantityOnReceipt, 0 AS MovementMIN, 0 AS MovementMAX, 0 AS MovementAVG " + "\r\n";
            //////queryString = queryString + "                   FROM    PurchaseInvoiceDetails " + "\r\n";
            //////queryString = queryString + "                   WHERE   PurchaseInvoiceDetails.CommodityTypeID = " + (int)GlobalEnums.CommodityTypeID.Vehicles + (allWarehouses ? "" : " AND PurchaseInvoiceDetails.LocationID = @LocationID ") + " AND PurchaseInvoiceDetails.EntryDate <= @LocalToDate AND PurchaseInvoiceDetails.Quantity > PurchaseInvoiceDetails.QuantityReceipt " + "\r\n";

            //////queryString = queryString + "                   UNION ALL " + "\r\n";

            //////queryString = queryString + "                   SELECT  CONVERT(smalldatetime, '" + new DateTime(1990, 1, 1).ToString("dd/MM/yyyy") + "', 103) AS EntryDate, 0 AS GoodsReceiptDetailID, GoodsReceiptDetails.CommodityID, GoodsReceiptDetails.CustomerID, '' AS Reference, '' AS ReceiptCode, 0 AS WarehouseID, " + "\r\n";
            //////queryString = queryString + "                           0 AS QuantityBegin, 0 AS QuantityInputINV, 0 AS QuantityInputPRO, 0 AS QuantityInputPRN, 0 AS QuantityInputRTN, 0 AS QuantityInputTRF, 0 AS QuantityInputADJ, 0 AS QuantityIssuedINV, 0 AS QuantityIssuedPRO, 0 AS QuantityIssuedTRF, 0 AS QuantityIssuedADJ, 0 AS QuantityOnPurchasing, GoodsReceiptDetails.Quantity AS QuantityOnReceipt, 0 AS MovementMIN, 0 AS MovementMAX, 0 AS MovementAVG " + "\r\n";
            //////queryString = queryString + "                   FROM    PurchaseInvoices INNER JOIN " + "\r\n";
            //////queryString = queryString + "                           GoodsReceiptDetails ON GoodsReceiptDetails.CommodityTypeID = " + (int)GlobalEnums.CommodityTypeID.Vehicles + (allWarehouses ? "" : " AND PurchaseInvoices.LocationID = @LocationID ") + " AND PurchaseInvoices.PurchaseInvoiceID = GoodsReceiptDetails.VoucherID AND GoodsReceiptDetails.GoodsReceiptTypeID = " + (int)GlobalEnums.GoodsReceiptTypeID.PurchaseInvoice + " AND PurchaseInvoices.EntryDate <= @LocalToDate AND GoodsReceiptDetails.EntryDate > @LocalToDate " + "\r\n";

            //////queryString = queryString + "                   UNION ALL " + "\r\n";
            ////////EWHInputVoucherTypeID.EWHTransfer
            //////queryString = queryString + "                   SELECT  CONVERT(smalldatetime, '" + new DateTime(1990, 1, 1).ToString("dd/MM/yyyy") + "', 103) AS EntryDate, 0 AS GoodsReceiptDetailID, StockTransferDetails.CommodityID, StockTransferDetails.CustomerID, '' AS Reference, '' AS ReceiptCode, 0 AS WarehouseID, " + "\r\n";
            //////queryString = queryString + "                           0 AS QuantityBegin, 0 AS QuantityInputINV, 0 AS QuantityInputPRO, 0 AS QuantityInputPRN, 0 AS QuantityInputRTN, 0 AS QuantityInputTRF, 0 AS QuantityInputADJ, 0 AS QuantityIssuedINV, 0 AS QuantityIssuedPRO, 0 AS QuantityIssuedTRF, 0 AS QuantityIssuedADJ, 0 AS QuantityOnPurchasing, (StockTransferDetails.Quantity - StockTransferDetails.QuantityReceipt) AS QuantityOnReceipt, 0 AS MovementMIN, 0 AS MovementMAX, 0 AS MovementAVG " + "\r\n";
            //////queryString = queryString + "                   FROM    StockTransfers INNER JOIN " + "\r\n";
            //////queryString = queryString + "                           StockTransferDetails ON StockTransfers.StockTransferID = StockTransferDetails.StockTransferID AND StockTransferDetails.CommodityTypeID = " + (int)GlobalEnums.CommodityTypeID.Vehicles + (allWarehouses ? "" : " AND StockTransfers.WarehouseID = @LocalWarehouseID ") + " AND StockTransferDetails.EntryDate <= @LocalToDate AND StockTransferDetails.Quantity > StockTransferDetails.QuantityReceipt " + "\r\n";

            //////queryString = queryString + "                   UNION ALL " + "\r\n";

            //////queryString = queryString + "                   SELECT  CONVERT(smalldatetime, '" + new DateTime(1990, 1, 1).ToString("dd/MM/yyyy") + "', 103) AS EntryDate, 0 AS GoodsReceiptDetailID, GoodsReceiptDetails.CommodityID, GoodsReceiptDetails.CustomerID, '' AS Reference, '' AS ReceiptCode, 0 AS WarehouseID, " + "\r\n";
            //////queryString = queryString + "                           0 AS QuantityBegin, 0 AS QuantityInputINV, 0 AS QuantityInputPRO, 0 AS QuantityInputPRN, 0 AS QuantityInputRTN, 0 AS QuantityInputTRF, 0 AS QuantityInputADJ, 0 AS QuantityIssuedINV, 0 AS QuantityIssuedPRO, 0 AS QuantityIssuedTRF, 0 AS QuantityIssuedADJ, 0 AS QuantityOnPurchasing, GoodsReceiptDetails.Quantity AS QuantityOnReceipt, 0 AS MovementMIN, 0 AS MovementMAX, 0 AS MovementAVG " + "\r\n";
            //////queryString = queryString + "                   FROM    StockTransfers INNER JOIN " + "\r\n";
            //////queryString = queryString + "                           GoodsReceiptDetails ON GoodsReceiptDetails.CommodityTypeID = " + (int)GlobalEnums.CommodityTypeID.Vehicles + (allWarehouses ? "" : " AND StockTransfers.WarehouseID = @LocalWarehouseID ") + " AND StockTransfers.StockTransferID = GoodsReceiptDetails.VoucherID AND GoodsReceiptDetails.GoodsReceiptTypeID = " + (int)GlobalEnums.GoodsReceiptTypeID.StockTransfer + " AND StockTransfers.EntryDate <= @LocalToDate AND GoodsReceiptDetails.EntryDate > @LocalToDate " + "\r\n";
            ////////--ON INPUT.END







            queryString = queryString + "                   ) AS WarehouseJournalMaster INNER JOIN " + "\r\n";


            queryString = queryString + "                   Commodities ON WarehouseJournalMaster.CommodityID = Commodities.CommodityID INNER JOIN " + "\r\n";
            queryString = queryString + "                   EntireCommodityCategories ON Commodities.CommodityCategoryID = EntireCommodityCategories.CommodityCategoryID LEFT JOIN " + "\r\n";

            queryString = queryString + "                   Warehouses ON WarehouseJournalMaster.WarehouseID = Warehouses.WarehouseID " + "\r\n";

            queryString = queryString + "                   LEFT JOIN Customers ON WarehouseJournalMaster.CustomerID = Customers.CustomerID " + "\r\n";

            queryString = queryString + "    END " + "\r\n";

            return queryString;

        }




        private void WarehouseCards()
        {
            string warehouseAdjustmentController = "                            WHEN " + (int)GlobalEnums.NmvnTaskID.OtherMaterialReceipt + " THEN '" + GlobalEnums.NmvnTaskID.OtherMaterialReceipt.ToString() + "s' WHEN " + (int)GlobalEnums.NmvnTaskID.OtherMaterialIssue + " THEN '" + GlobalEnums.NmvnTaskID.OtherMaterialIssue.ToString() + "s' WHEN " + (int)GlobalEnums.NmvnTaskID.MaterialAdjustment + " THEN '" + GlobalEnums.NmvnTaskID.MaterialAdjustment.ToString() + "s' " + "\r\n";
            warehouseAdjustmentController = warehouseAdjustmentController + "   WHEN " + (int)GlobalEnums.NmvnTaskID.OtherItemReceipt + " THEN '" + GlobalEnums.NmvnTaskID.OtherItemReceipt.ToString() + "s' WHEN " + (int)GlobalEnums.NmvnTaskID.OtherItemIssue + " THEN '" + GlobalEnums.NmvnTaskID.OtherItemIssue.ToString() + "s' WHEN " + (int)GlobalEnums.NmvnTaskID.ItemAdjustment + " THEN '" + GlobalEnums.NmvnTaskID.ItemAdjustment.ToString() + "s' " + "\r\n";
            warehouseAdjustmentController = warehouseAdjustmentController + "   WHEN " + (int)GlobalEnums.NmvnTaskID.OtherProductReceipt + " THEN '" + GlobalEnums.NmvnTaskID.OtherProductReceipt.ToString() + "s' WHEN " + (int)GlobalEnums.NmvnTaskID.OtherProductIssue + " THEN '" + GlobalEnums.NmvnTaskID.OtherProductIssue.ToString() + "s' WHEN " + (int)GlobalEnums.NmvnTaskID.ProductAdjustment + " THEN '" + GlobalEnums.NmvnTaskID.ProductAdjustment.ToString() + "s' END " + "\r\n";


            string queryString = " @WarehouseID int, @FromDate DateTime, @ToDate DateTime " + "\r\n"; //Filter by @LocalWarehouseID to make this stored procedure run faster, but it may be removed without any effect the algorithm

            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";
            queryString = queryString + "    BEGIN " + "\r\n";

            queryString = queryString + "       DECLARE     @LocalWarehouseID int, @LocalFromDate DateTime, @LocalToDate DateTime " + "\r\n";
            queryString = queryString + "       SET         @LocalWarehouseID = @WarehouseID    SET @LocalFromDate = @FromDate      SET @LocalToDate = @ToDate " + "\r\n";

            queryString = queryString + "       DECLARE     @LocationID int " + "\r\n";
            queryString = queryString + "       SET         @LocationID = (SELECT LocationID FROM Warehouses WHERE WarehouseID = @LocalWarehouseID) " + "\r\n";

            queryString = queryString + "       SELECT      800000 + @LocalWarehouseID AS WarehouseCardID, WarehouseJournalMaster.JournalTypeID, WarehouseJournalMaster.CustomerID, WarehouseJournalMaster.CustomerCode, WarehouseJournalMaster.CustomerName, WarehouseJournalMaster.Specs, WarehouseJournalMaster.Specification, WarehouseJournalMaster.FirmOrderCode, WarehouseJournalMaster.ControllerName, WarehouseJournalMaster.EntityID, WarehouseJournalMaster.GroupName, WarehouseJournalMaster.SubGroupName, WarehouseJournalMaster.GoodsReceiptDetailID, WarehouseJournalMaster.EntryDate, " + "\r\n";
            queryString = queryString + "                   Commodities.CommodityID, Commodities.Code, Commodities.CodePartA, Commodities.CodePartB, Commodities.CodePartC, Commodities.CodePartD, Commodities.CodePartE, Commodities.CodePartF, Commodities.Name, Commodities.SalesUnit, Commodities.LeadTime, WarehouseJournalMaster.Reference, WarehouseJournalMaster.Referral, WarehouseJournalMaster.ReceiptCode, WarehouseJournalMaster.Barcode, " + "\r\n";
            queryString = queryString + "                   ISNULL(Warehouses.LocationID, 0) AS LocationID, ISNULL(Warehouses.WarehouseCategoryID, 0) AS WarehouseCategoryID, ISNULL(Warehouses.WarehouseID, 0) AS WarehouseID, ISNULL(Warehouses.Name, '') AS WarehouseName, Warehouses.Bookable AS Bookable, " + "\r\n";
            queryString = queryString + "                   WarehouseJournalMaster.Description, WarehouseJournalMaster.QuantityDebit, WarehouseJournalMaster.QuantityCredit, " + "\r\n";

            queryString = queryString + "                   IIF(WarehouseJournalMaster.JournalTypeID = " + (int)GlobalEnums.WarehouseAdjustmentTypeID.ALL + ", WarehouseJournalMaster.QuantityDebit, 0) AS QuantityBEGIN, " + "\r\n";
            queryString = queryString + "                   IIF(WarehouseJournalMaster.JournalTypeID = " + (int)GlobalEnums.WarehouseAdjustmentTypeID.ALL + " OR WarehouseJournalMaster.JournalTypeID = " + (int)GlobalEnums.WarehouseAdjustmentTypeID.ReceiptPENDING + " OR WarehouseJournalMaster.JournalTypeID = " + (int)GlobalEnums.WarehouseAdjustmentTypeID.IssuePENDING + ", 0, WarehouseJournalMaster.QuantityDebit) AS QuantityRECEIPT, " + "\r\n";
            queryString = queryString + "                   IIF(WarehouseJournalMaster.JournalTypeID = " + (int)GlobalEnums.WarehouseAdjustmentTypeID.ALL + " OR WarehouseJournalMaster.JournalTypeID = " + (int)GlobalEnums.WarehouseAdjustmentTypeID.ReceiptPENDING + " OR WarehouseJournalMaster.JournalTypeID = " + (int)GlobalEnums.WarehouseAdjustmentTypeID.IssuePENDING + ", 0, WarehouseJournalMaster.QuantityCredit) AS QuantityISSUE, " + "\r\n";
            queryString = queryString + "                   IIF(WarehouseJournalMaster.JournalTypeID = " + (int)GlobalEnums.WarehouseAdjustmentTypeID.ReceiptPENDING + " OR WarehouseJournalMaster.JournalTypeID = " + (int)GlobalEnums.WarehouseAdjustmentTypeID.IssuePENDING + ", 0, WarehouseJournalMaster.QuantityDebit - WarehouseJournalMaster.QuantityCredit) AS QuantityEND, " + "\r\n";

            queryString = queryString + "                   IIF(WarehouseJournalMaster.JournalTypeID = " + (int)GlobalEnums.WarehouseAdjustmentTypeID.ReceiptPRO + ", WarehouseJournalMaster.QuantityDebit, 0) AS ReceiptPRO, " + "\r\n";
            queryString = queryString + "                   IIF(WarehouseJournalMaster.JournalTypeID = " + (int)GlobalEnums.WarehouseAdjustmentTypeID.ReceiptSAM + ", WarehouseJournalMaster.QuantityDebit, 0) AS ReceiptSAM, " + "\r\n";
            queryString = queryString + "                   IIF(WarehouseJournalMaster.JournalTypeID = " + (int)GlobalEnums.WarehouseAdjustmentTypeID.ReceiptRTN + ", WarehouseJournalMaster.QuantityDebit, 0) AS ReceiptRTN, " + "\r\n";
            queryString = queryString + "                   IIF(WarehouseJournalMaster.JournalTypeID = " + (int)GlobalEnums.WarehouseAdjustmentTypeID.ReceiptADJ + ", WarehouseJournalMaster.QuantityDebit, 0) AS ReceiptADJ, " + "\r\n";
            queryString = queryString + "                   IIF(WarehouseJournalMaster.JournalTypeID = " + (int)GlobalEnums.WarehouseAdjustmentTypeID.ReceiptPUR + ", WarehouseJournalMaster.QuantityDebit, 0) AS ReceiptPUR, " + "\r\n";
            queryString = queryString + "                   IIF(WarehouseJournalMaster.JournalTypeID = " + (int)GlobalEnums.WarehouseAdjustmentTypeID.ReceiptREG + ", WarehouseJournalMaster.QuantityDebit, 0) AS ReceiptREG, " + "\r\n";
            queryString = queryString + "                   IIF(WarehouseJournalMaster.JournalTypeID = " + (int)GlobalEnums.WarehouseAdjustmentTypeID.ReceiptOSS + ", WarehouseJournalMaster.QuantityDebit, 0) AS ReceiptOSS, " + "\r\n";
            queryString = queryString + "                   IIF(WarehouseJournalMaster.JournalTypeID = " + (int)GlobalEnums.WarehouseAdjustmentTypeID.ReceiptBOR + ", WarehouseJournalMaster.QuantityDebit, 0) AS ReceiptBOR, " + "\r\n";
            queryString = queryString + "                   IIF(WarehouseJournalMaster.JournalTypeID = " + (int)GlobalEnums.WarehouseAdjustmentTypeID.ReceiptFOI + ", WarehouseJournalMaster.QuantityDebit, 0) AS ReceiptFOI, " + "\r\n";
            queryString = queryString + "                   IIF(WarehouseJournalMaster.JournalTypeID = " + (int)GlobalEnums.WarehouseAdjustmentTypeID.ReceiptTXF + ", WarehouseJournalMaster.QuantityDebit, 0) AS ReceiptTXF, " + "\r\n";
            queryString = queryString + "                   IIF(WarehouseJournalMaster.JournalTypeID = " + (int)GlobalEnums.WarehouseAdjustmentTypeID.ReceiptOTH + " OR WarehouseJournalMaster.JournalTypeID = " + (int)GlobalEnums.WarehouseAdjustmentTypeID.BothReceiptIssues + ", WarehouseJournalMaster.QuantityDebit, 0) AS ReceiptOTH, " + "\r\n";

            queryString = queryString + "                   IIF(WarehouseJournalMaster.JournalTypeID = " + (int)GlobalEnums.WarehouseAdjustmentTypeID.IssueINV + ", WarehouseJournalMaster.QuantityCredit, 0) AS IssueINV, " + "\r\n";
            queryString = queryString + "                   IIF(WarehouseJournalMaster.JournalTypeID = " + (int)GlobalEnums.WarehouseAdjustmentTypeID.IssueOFS + ", WarehouseJournalMaster.QuantityCredit, 0) AS IssueOFS, " + "\r\n";
            queryString = queryString + "                   IIF(WarehouseJournalMaster.JournalTypeID = " + (int)GlobalEnums.WarehouseAdjustmentTypeID.IssueADJ + ", WarehouseJournalMaster.QuantityCredit, 0) AS IssueADJ, " + "\r\n";
            queryString = queryString + "                   IIF(WarehouseJournalMaster.JournalTypeID = " + (int)GlobalEnums.WarehouseAdjustmentTypeID.IssueSAM + ", WarehouseJournalMaster.QuantityCredit, 0) AS IssueSAM, " + "\r\n";
            queryString = queryString + "                   IIF(WarehouseJournalMaster.JournalTypeID = " + (int)GlobalEnums.WarehouseAdjustmentTypeID.IssueDST + ", WarehouseJournalMaster.QuantityCredit, 0) AS IssueDST, " + "\r\n";
            queryString = queryString + "                   IIF(WarehouseJournalMaster.JournalTypeID = " + (int)GlobalEnums.WarehouseAdjustmentTypeID.IssuePRO + ", WarehouseJournalMaster.QuantityCredit, 0) AS IssuePRO, " + "\r\n";
            queryString = queryString + "                   IIF(WarehouseJournalMaster.JournalTypeID = " + (int)GlobalEnums.WarehouseAdjustmentTypeID.IssueREG + ", WarehouseJournalMaster.QuantityCredit, 0) AS IssueREG, " + "\r\n";
            queryString = queryString + "                   IIF(WarehouseJournalMaster.JournalTypeID = " + (int)GlobalEnums.WarehouseAdjustmentTypeID.IssueRTN + ", WarehouseJournalMaster.QuantityCredit, 0) AS IssueRTN, " + "\r\n";
            queryString = queryString + "                   IIF(WarehouseJournalMaster.JournalTypeID = " + (int)GlobalEnums.WarehouseAdjustmentTypeID.IssueOSS + ", WarehouseJournalMaster.QuantityCredit, 0) AS IssueOSS, " + "\r\n";
            queryString = queryString + "                   IIF(WarehouseJournalMaster.JournalTypeID = " + (int)GlobalEnums.WarehouseAdjustmentTypeID.IssueTXF + ", WarehouseJournalMaster.QuantityCredit, 0) AS IssueTXF, " + "\r\n";
            queryString = queryString + "                   IIF(WarehouseJournalMaster.JournalTypeID = " + (int)GlobalEnums.WarehouseAdjustmentTypeID.IssueOTH + " OR WarehouseJournalMaster.JournalTypeID = " + (int)GlobalEnums.WarehouseAdjustmentTypeID.BothReceiptIssues + ", WarehouseJournalMaster.QuantityCredit, 0) AS IssueOTH, " + "\r\n";

            queryString = queryString + "                   IIF(WarehouseJournalMaster.JournalTypeID = " + (int)GlobalEnums.WarehouseAdjustmentTypeID.ReceiptPENDING + ", WarehouseJournalMaster.QuantityDebit, 0) AS ReceiptPENDING, " + "\r\n";
            queryString = queryString + "                   IIF(WarehouseJournalMaster.JournalTypeID = " + (int)GlobalEnums.WarehouseAdjustmentTypeID.IssuePENDING + ", WarehouseJournalMaster.QuantityCredit, 0) AS IssuePENDING, " + "\r\n";

            queryString = queryString + "                   EntireCommodityCategories.CommodityCategoryID, " + "\r\n";
            queryString = queryString + "                   EntireCommodityCategories.Name1 AS CommodityCategory1, " + "\r\n";
            queryString = queryString + "                   EntireCommodityCategories.Name2 AS CommodityCategory2, " + "\r\n";
            queryString = queryString + "                   EntireCommodityCategories.Name3 AS CommodityCategory3 " + "\r\n";

            queryString = queryString + "       FROM       (" + "\r\n";

            //--BEGIN-INPUT-OUTPUT-END.END
            //1.BEGINING
            ////////AT TDV, TOTALSMARTPORTAL: JUST HAVE SUMMARY AT BEGINNING
            //WHINPUT
            queryString = queryString + "                   SELECT      " + (int)GlobalEnums.WarehouseAdjustmentTypeID.ALL + " AS JournalTypeID, Customers.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, NULL AS Specs, NULL AS Specification, NULL AS FirmOrderCode, '' AS ControllerName, 0 AS EntityID, N'HH TẠI KHO' AS GroupName, N'DAU KY ' + CONVERT(VARCHAR, DATEADD (day, -1,  @LocalFromDate), 103) AS SubGroupName, GoodsReceiptDetails.GoodsReceiptDetailID, GoodsReceiptDetails.EntryDate, GoodsReceiptDetails.CommodityID, GoodsReceiptDetails.Reference, NULL AS Referral, GoodsReceiptDetails.Code AS ReceiptCode, GoodsReceiptDetails.Barcode, GoodsReceiptDetails.WarehouseID, N'' AS Description, ROUND(GoodsReceiptDetails.Quantity - GoodsReceiptDetails.QuantityIssued, " + (int)GlobalEnums.rndQuantity + ") AS QuantityDebit, 0 AS QuantityCredit " + "\r\n";
            queryString = queryString + "                   FROM        GoodsReceiptDetails " + "\r\n";
            queryString = queryString + "                               LEFT JOIN Customers ON GoodsReceiptDetails.CustomerID = Customers.CustomerID " + "\r\n";
            queryString = queryString + "                   WHERE       GoodsReceiptDetails.WarehouseID = @LocalWarehouseID AND GoodsReceiptDetails.EntryDate < @LocalFromDate AND GoodsReceiptDetails.Quantity > GoodsReceiptDetails.QuantityIssued " + "\r\n";

            queryString = queryString + "                   UNION ALL " + "\r\n";
            //UNDO MaterialIssueDetails
            queryString = queryString + "                   SELECT      " + (int)GlobalEnums.WarehouseAdjustmentTypeID.ALL + " AS JournalTypeID, Customers.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, NULL AS Specs, NULL AS Specification, NULL AS FirmOrderCode, '' AS ControllerName, 0 AS EntityID, N'HH TẠI KHO' AS GroupName, N'DAU KY ' + CONVERT(VARCHAR, DATEADD (day, -1,  @LocalFromDate), 103) AS SubGroupName, GoodsReceiptDetails.GoodsReceiptDetailID, GoodsReceiptDetails.EntryDate, GoodsReceiptDetails.CommodityID, GoodsReceiptDetails.Reference, NULL AS Referral, GoodsReceiptDetails.Code AS ReceiptCode, GoodsReceiptDetails.Barcode, GoodsReceiptDetails.WarehouseID, N'' AS Description, MaterialIssueDetails.Quantity AS QuantityDebit, 0 AS QuantityCredit " + "\r\n";
            queryString = queryString + "                   FROM        GoodsReceiptDetails " + "\r\n";
            queryString = queryString + "                               INNER JOIN MaterialIssueDetails ON GoodsReceiptDetails.WarehouseID = @LocalWarehouseID AND GoodsReceiptDetails.GoodsReceiptDetailID = MaterialIssueDetails.GoodsReceiptDetailID AND GoodsReceiptDetails.EntryDate < @LocalFromDate AND MaterialIssueDetails.EntryDate >= @LocalFromDate " + "\r\n";
            queryString = queryString + "                               LEFT JOIN Customers ON GoodsReceiptDetails.CustomerID = Customers.CustomerID " + "\r\n";

            queryString = queryString + "                   UNION ALL " + "\r\n";
            //UNDO PackageIssueDetails
            queryString = queryString + "                   SELECT      " + (int)GlobalEnums.WarehouseAdjustmentTypeID.ALL + " AS JournalTypeID, Customers.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, NULL AS Specs, NULL AS Specification, NULL AS FirmOrderCode, '' AS ControllerName, 0 AS EntityID, N'HH TẠI KHO' AS GroupName, N'DAU KY ' + CONVERT(VARCHAR, DATEADD (day, -1,  @LocalFromDate), 103) AS SubGroupName, GoodsReceiptDetails.GoodsReceiptDetailID, GoodsReceiptDetails.EntryDate, GoodsReceiptDetails.CommodityID, GoodsReceiptDetails.Reference, NULL AS Referral, GoodsReceiptDetails.Code AS ReceiptCode, GoodsReceiptDetails.Barcode, GoodsReceiptDetails.WarehouseID, N'' AS Description, PackageIssueDetails.Quantity AS QuantityDebit, 0 AS QuantityCredit " + "\r\n";
            queryString = queryString + "                   FROM        GoodsReceiptDetails " + "\r\n";
            queryString = queryString + "                               INNER JOIN PackageIssueDetails ON GoodsReceiptDetails.WarehouseID = @LocalWarehouseID AND GoodsReceiptDetails.GoodsReceiptDetailID = PackageIssueDetails.GoodsReceiptDetailID AND GoodsReceiptDetails.EntryDate < @LocalFromDate AND PackageIssueDetails.EntryDate >= @LocalFromDate " + "\r\n";
            queryString = queryString + "                               LEFT JOIN Customers ON GoodsReceiptDetails.CustomerID = Customers.CustomerID " + "\r\n";

            queryString = queryString + "                   UNION ALL " + "\r\n";
            //UNDO GoodsIssuePackages
            queryString = queryString + "                   SELECT      " + (int)GlobalEnums.WarehouseAdjustmentTypeID.ALL + " AS JournalTypeID, Customers.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, NULL AS Specs, NULL AS Specification, NULL AS FirmOrderCode, '' AS ControllerName, 0 AS EntityID, N'HH TẠI KHO' AS GroupName, N'DAU KY ' + CONVERT(VARCHAR, DATEADD (day, -1,  @LocalFromDate), 103) AS SubGroupName, GoodsReceiptDetails.GoodsReceiptDetailID, GoodsReceiptDetails.EntryDate, GoodsReceiptDetails.CommodityID, GoodsReceiptDetails.Reference, NULL AS Referral, GoodsReceiptDetails.Code AS ReceiptCode, GoodsReceiptDetails.Barcode, GoodsReceiptDetails.WarehouseID, N'' AS Description, GoodsIssuePackages.Quantity AS QuantityDebit, 0 AS QuantityCredit " + "\r\n";
            queryString = queryString + "                   FROM        GoodsReceiptDetails " + "\r\n";
            queryString = queryString + "                               INNER JOIN GoodsIssuePackages ON GoodsReceiptDetails.WarehouseID = @LocalWarehouseID AND GoodsReceiptDetails.GoodsReceiptDetailID = GoodsIssuePackages.GoodsReceiptDetailID AND GoodsReceiptDetails.EntryDate < @LocalFromDate AND GoodsIssuePackages.EntryDate >= @LocalFromDate " + "\r\n";
            queryString = queryString + "                               LEFT JOIN Customers ON GoodsReceiptDetails.CustomerID = Customers.CustomerID " + "\r\n";

            queryString = queryString + "                   UNION ALL " + "\r\n";
            //UNDO WarehouseTransferDetails
            queryString = queryString + "                   SELECT      " + (int)GlobalEnums.WarehouseAdjustmentTypeID.ALL + " AS JournalTypeID, Customers.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, NULL AS Specs, NULL AS Specification, NULL AS FirmOrderCode, '' AS ControllerName, 0 AS EntityID, N'HH TẠI KHO' AS GroupName, N'DAU KY ' + CONVERT(VARCHAR, DATEADD (day, -1,  @LocalFromDate), 103) AS SubGroupName, GoodsReceiptDetails.GoodsReceiptDetailID, GoodsReceiptDetails.EntryDate, GoodsReceiptDetails.CommodityID, GoodsReceiptDetails.Reference, NULL AS Referral, GoodsReceiptDetails.Code AS ReceiptCode, GoodsReceiptDetails.Barcode, GoodsReceiptDetails.WarehouseID, N'' AS Description, WarehouseTransferDetails.Quantity AS QuantityDebit, 0 AS QuantityCredit " + "\r\n";
            queryString = queryString + "                   FROM        GoodsReceiptDetails " + "\r\n";
            queryString = queryString + "                               INNER JOIN WarehouseTransferDetails ON GoodsReceiptDetails.WarehouseID = @LocalWarehouseID AND GoodsReceiptDetails.GoodsReceiptDetailID = WarehouseTransferDetails.GoodsReceiptDetailID AND GoodsReceiptDetails.EntryDate < @LocalFromDate AND WarehouseTransferDetails.EntryDate >= @LocalFromDate " + "\r\n";
            queryString = queryString + "                               LEFT JOIN Customers ON GoodsReceiptDetails.CustomerID = Customers.CustomerID " + "\r\n";

            queryString = queryString + "                   UNION ALL " + "\r\n";
            //UNDO WarehouseAdjustmentDetails
            queryString = queryString + "                   SELECT      " + (int)GlobalEnums.WarehouseAdjustmentTypeID.ALL + " AS JournalTypeID, Customers.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, NULL AS Specs, NULL AS Specification, NULL AS FirmOrderCode, '' AS ControllerName, 0 AS EntityID, N'HH TẠI KHO' AS GroupName, N'DAU KY ' + CONVERT(VARCHAR, DATEADD (day, -1,  @LocalFromDate), 103) AS SubGroupName, GoodsReceiptDetails.GoodsReceiptDetailID, GoodsReceiptDetails.EntryDate, GoodsReceiptDetails.CommodityID, GoodsReceiptDetails.Reference, NULL AS Referral, GoodsReceiptDetails.Code AS ReceiptCode, GoodsReceiptDetails.Barcode, GoodsReceiptDetails.WarehouseID, N'' AS Description, -WarehouseAdjustmentDetails.Quantity AS QuantityDebit, 0 AS QuantityCredit " + "\r\n";
            queryString = queryString + "                   FROM        GoodsReceiptDetails " + "\r\n";
            queryString = queryString + "                               INNER JOIN WarehouseAdjustmentDetails ON GoodsReceiptDetails.WarehouseID = @LocalWarehouseID AND GoodsReceiptDetails.GoodsReceiptDetailID = WarehouseAdjustmentDetails.GoodsReceiptDetailID AND GoodsReceiptDetails.EntryDate < @LocalFromDate AND WarehouseAdjustmentDetails.EntryDate >= @LocalFromDate " + "\r\n";
            queryString = queryString + "                               LEFT JOIN Customers ON GoodsReceiptDetails.CustomerID = Customers.CustomerID " + "\r\n";


            ////////AT VINH LONG, TOTALBIKEPORTALS: MUST HAVE DETAIL BEGINING
            ////////1.1.WHINPUT (MUST USE TWO SEPERATE SQL TO GET THE GoodsReceiptTypeID (VoucherID))
            ////////1.1.1.WHINPUT.PurchaseInvoice
            //////queryString = queryString + "                   SELECT     N'HH TẠI KHO' AS GroupName, N'DAU KY ' + CONVERT(VARCHAR, DATEADD (day, -1,  @LocalFromDate), 103) AS SubGroupName, GoodsReceiptDetails.GoodsReceiptDetailID, GoodsReceiptDetails.EntryDate, GoodsReceiptDetails.CommodityID, GoodsReceiptDetails.Reference, NULL AS Referral, GoodsReceiptDetails.Code AS ReceiptCode, GoodsReceiptDetails.Barcode, GoodsReceiptDetails.WarehouseID, Suppliers.Name + ', HĐ [' + ISNULL(PurchaseInvoices.VATInvoiceNo, '') + ' Ngày: ' + CONVERT(VARCHAR, PurchaseInvoices.EntryDate, 103) + ']' AS Description, ROUND(GoodsReceiptDetails.Quantity - GoodsReceiptDetails.QuantityIssued, " + (int)GlobalEnums.rndQuantity + ") AS QuantityDebit, 0 AS QuantityCredit " + "\r\n";
            //////queryString = queryString + "                   FROM        GoodsReceiptDetails INNER JOIN " + "\r\n";
            //////queryString = queryString + "                               PurchaseInvoices ON GoodsReceiptDetails.WarehouseID = @LocalWarehouseID AND GoodsReceiptDetails.VoucherID = PurchaseInvoices.PurchaseInvoiceID AND GoodsReceiptDetails.GoodsReceiptTypeID = " + (int)GlobalEnums.GoodsReceiptTypeID.PurchaseInvoice + " AND GoodsReceiptDetails.Quantity > GoodsReceiptDetails.QuantityIssued AND GoodsReceiptDetails.EntryDate < @LocalFromDate INNER JOIN " + "\r\n";
            //////queryString = queryString + "                               Customers Suppliers ON PurchaseInvoices.SupplierID = Suppliers.CustomerID " + "\r\n";

            //////queryString = queryString + "                   UNION ALL " + "\r\n";
            ////////1.1.2.WHINPUT.StockTransfer
            //////queryString = queryString + "                   SELECT     N'HH TẠI KHO' AS GroupName, N'DAU KY ' + CONVERT(VARCHAR, DATEADD (day, -1,  @LocalFromDate), 103) AS SubGroupName, GoodsReceiptDetails.GoodsReceiptDetailID, GoodsReceiptDetails.EntryDate, GoodsReceiptDetails.CommodityID, GoodsReceiptDetails.Reference, NULL AS Referral, GoodsReceiptDetails.Code AS ReceiptCode, GoodsReceiptDetails.Barcode, GoodsReceiptDetails.WarehouseID, 'NHAP VCNB: ' + Locations.Name + ', PX [' + StockTransfers.Reference + ' Ngày: ' + CONVERT(VARCHAR, StockTransfers.EntryDate, 103) + ']' AS Description, ROUND(GoodsReceiptDetails.Quantity - GoodsReceiptDetails.QuantityIssued, " + (int)GlobalEnums.rndQuantity + ") AS QuantityDebit, 0 AS QuantityCredit " + "\r\n";
            //////queryString = queryString + "                   FROM        GoodsReceiptDetails INNER JOIN " + "\r\n";
            //////queryString = queryString + "                               StockTransfers ON GoodsReceiptDetails.WarehouseID = @LocalWarehouseID AND GoodsReceiptDetails.VoucherID = StockTransfers.StockTransferID AND GoodsReceiptDetails.GoodsReceiptTypeID = " + (int)GlobalEnums.GoodsReceiptTypeID.StockTransfer + " AND GoodsReceiptDetails.Quantity > GoodsReceiptDetails.QuantityIssued AND GoodsReceiptDetails.EntryDate < @LocalFromDate INNER JOIN " + "\r\n";
            //////queryString = queryString + "                               Locations ON StockTransfers.LocationID = Locations.LocationID " + "\r\n";



            //////queryString = queryString + "                   UNION ALL " + "\r\n";
            ////////1.2.UNDO (CAC CAU SQL CHO INVOICE, StockTransferDetails, WHADJUST, WHASSEMBLY LA HOAN TOAN GIONG NHAU. LUU Y T/H DAT BIET: WHADJUST.QUANTITY < 0)
            ////////1.2.1.1.UNDO SalesInvoiceDetails (MUST USE TWO SEPERATE SQL TO GET THE GoodsReceiptTypeID (VoucherID)).PurchaseInvoice
            //////queryString = queryString + "                   SELECT     N'HH TẠI KHO' AS GroupName, N'DAU KY ' + CONVERT(VARCHAR, DATEADD (day, -1,  @LocalFromDate), 103) AS SubGroupName, GoodsReceiptDetails.GoodsReceiptDetailID, GoodsReceiptDetails.EntryDate, GoodsReceiptDetails.CommodityID, GoodsReceiptDetails.Reference, NULL AS Referral, GoodsReceiptDetails.Code AS ReceiptCode, GoodsReceiptDetails.Barcode, GoodsReceiptDetails.WarehouseID, Suppliers.Name + ', HĐ [' + ISNULL(PurchaseInvoices.VATInvoiceNo, '') + ' Ngày: ' + CONVERT(VARCHAR, PurchaseInvoices.EntryDate, 103) + ']' AS Description, SalesInvoiceDetails.Quantity AS QuantityDebit, 0 AS QuantityCredit " + "\r\n";
            //////queryString = queryString + "                   FROM        GoodsReceiptDetails INNER JOIN " + "\r\n";
            //////queryString = queryString + "                               SalesInvoiceDetails ON GoodsReceiptDetails.WarehouseID = @LocalWarehouseID AND GoodsReceiptDetails.GoodsReceiptDetailID = SalesInvoiceDetails.GoodsReceiptDetailID AND GoodsReceiptDetails.EntryDate < @LocalFromDate AND SalesInvoiceDetails.EntryDate >= @LocalFromDate INNER JOIN " + "\r\n";
            //////queryString = queryString + "                               PurchaseInvoices ON GoodsReceiptDetails.VoucherID = PurchaseInvoices.PurchaseInvoiceID AND GoodsReceiptDetails.GoodsReceiptTypeID = " + (int)GlobalEnums.GoodsReceiptTypeID.PurchaseInvoice + " INNER JOIN " + "\r\n";
            //////queryString = queryString + "                               Customers Suppliers ON PurchaseInvoices.SupplierID = Suppliers.CustomerID " + "\r\n";

            //////queryString = queryString + "                   UNION ALL " + "\r\n";
            ////////1.2.1.2.UNDO SalesInvoiceDetails (MUST USE TWO SEPERATE SQL TO GET THE GoodsReceiptTypeID (VoucherID)).StockTransfer
            //////queryString = queryString + "                   SELECT     N'HH TẠI KHO' AS GroupName, N'DAU KY ' + CONVERT(VARCHAR, DATEADD (day, -1,  @LocalFromDate), 103) AS SubGroupName, GoodsReceiptDetails.GoodsReceiptDetailID, GoodsReceiptDetails.EntryDate, GoodsReceiptDetails.CommodityID, GoodsReceiptDetails.Reference, NULL AS Referral, GoodsReceiptDetails.Code AS ReceiptCode, GoodsReceiptDetails.Barcode, GoodsReceiptDetails.WarehouseID, 'NHAP VCNB: ' + Locations.Name + ', PX [' + StockTransfers.Reference + ' Ngày: ' + CONVERT(VARCHAR, StockTransfers.EntryDate, 103) + ']' AS Description, SalesInvoiceDetails.Quantity AS QuantityDebit, 0 AS QuantityCredit " + "\r\n";
            //////queryString = queryString + "                   FROM        GoodsReceiptDetails INNER JOIN " + "\r\n";
            //////queryString = queryString + "                               SalesInvoiceDetails ON GoodsReceiptDetails.WarehouseID = @LocalWarehouseID AND GoodsReceiptDetails.GoodsReceiptDetailID = SalesInvoiceDetails.GoodsReceiptDetailID AND GoodsReceiptDetails.EntryDate < @LocalFromDate AND SalesInvoiceDetails.EntryDate >= @LocalFromDate INNER JOIN " + "\r\n";
            //////queryString = queryString + "                               StockTransfers ON GoodsReceiptDetails.VoucherID = StockTransfers.StockTransferID AND GoodsReceiptDetails.GoodsReceiptTypeID = " + (int)GlobalEnums.GoodsReceiptTypeID.StockTransfer + " INNER JOIN " + "\r\n";
            //////queryString = queryString + "                               Locations ON StockTransfers.LocationID = Locations.LocationID " + "\r\n";

            //////queryString = queryString + "                   UNION ALL " + "\r\n";
            ////////1.2.2.1.UNDO StockTransferDetails (MUST USE TWO SEPERATE SQL TO GET THE GoodsReceiptTypeID (VoucherID)).PurchaseInvoice
            //////queryString = queryString + "                   SELECT     N'HH TẠI KHO' AS GroupName, N'DAU KY ' + CONVERT(VARCHAR, DATEADD (day, -1,  @LocalFromDate), 103) AS SubGroupName, GoodsReceiptDetails.GoodsReceiptDetailID, GoodsReceiptDetails.EntryDate, GoodsReceiptDetails.CommodityID, GoodsReceiptDetails.Reference, NULL AS Referral, GoodsReceiptDetails.Code AS ReceiptCode, GoodsReceiptDetails.Barcode, GoodsReceiptDetails.WarehouseID, Suppliers.Name + ', HĐ [' + ISNULL(PurchaseInvoices.VATInvoiceNo, '') + ' Ngày: ' + CONVERT(VARCHAR, PurchaseInvoices.EntryDate, 103) + ']' AS Description, StockTransferDetails.Quantity AS QuantityDebit, 0 AS QuantityCredit " + "\r\n";
            //////queryString = queryString + "                   FROM        GoodsReceiptDetails INNER JOIN " + "\r\n";
            //////queryString = queryString + "                               StockTransferDetails ON GoodsReceiptDetails.WarehouseID = @LocalWarehouseID AND GoodsReceiptDetails.GoodsReceiptDetailID = StockTransferDetails.GoodsReceiptDetailID AND GoodsReceiptDetails.EntryDate < @LocalFromDate AND StockTransferDetails.EntryDate >= @LocalFromDate INNER JOIN " + "\r\n";
            //////queryString = queryString + "                               PurchaseInvoices ON GoodsReceiptDetails.VoucherID = PurchaseInvoices.PurchaseInvoiceID AND GoodsReceiptDetails.GoodsReceiptTypeID = " + (int)GlobalEnums.GoodsReceiptTypeID.PurchaseInvoice + " INNER JOIN " + "\r\n";
            //////queryString = queryString + "                               Customers Suppliers ON PurchaseInvoices.SupplierID = Suppliers.CustomerID " + "\r\n";

            //////queryString = queryString + "                   UNION ALL " + "\r\n";
            ////////1.2.2.2.UNDO StockTransferDetails (MUST USE TWO SEPERATE SQL TO GET THE GoodsReceiptTypeID (VoucherID)).StockTransfer
            //////queryString = queryString + "                   SELECT     N'HH TẠI KHO' AS GroupName, N'DAU KY ' + CONVERT(VARCHAR, DATEADD (day, -1,  @LocalFromDate), 103) AS SubGroupName, GoodsReceiptDetails.GoodsReceiptDetailID, GoodsReceiptDetails.EntryDate, GoodsReceiptDetails.CommodityID, GoodsReceiptDetails.Reference, NULL AS Referral, GoodsReceiptDetails.Code AS ReceiptCode, GoodsReceiptDetails.Barcode, GoodsReceiptDetails.WarehouseID, 'NHAP VCNB: ' + Locations.Name + ', PX [' + StockTransfers.Reference + ' Ngày: ' + CONVERT(VARCHAR, StockTransfers.EntryDate, 103) + ']' AS Description, StockTransferDetails.Quantity AS QuantityDebit, 0 AS QuantityCredit " + "\r\n";
            //////queryString = queryString + "                   FROM        GoodsReceiptDetails INNER JOIN " + "\r\n";
            //////queryString = queryString + "                               StockTransferDetails ON GoodsReceiptDetails.WarehouseID = @LocalWarehouseID AND GoodsReceiptDetails.GoodsReceiptDetailID = StockTransferDetails.GoodsReceiptDetailID AND GoodsReceiptDetails.EntryDate < @LocalFromDate AND StockTransferDetails.EntryDate >= @LocalFromDate INNER JOIN " + "\r\n";
            //////queryString = queryString + "                               StockTransfers ON GoodsReceiptDetails.VoucherID = StockTransfers.StockTransferID AND GoodsReceiptDetails.GoodsReceiptTypeID = " + (int)GlobalEnums.GoodsReceiptTypeID.StockTransfer + " INNER JOIN " + "\r\n";
            //////queryString = queryString + "                               Locations ON StockTransfers.LocationID = Locations.LocationID " + "\r\n";





            //2.INTPUT (MUST USE TWO SEPERATE SQL TO GET THE GoodsReceiptTypeID (VoucherID))

            //2.1.INTPUT.PurchaseRequisition
            queryString = queryString + "                   UNION ALL " + "\r\n";
            queryString = queryString + "                   SELECT      " + (int)GlobalEnums.WarehouseAdjustmentTypeID.ReceiptPUR + " AS JournalTypeID, Customers.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, GoodsReceiptDetails.Remarks AS Specs, GoodsReceiptDetails.Remarks AS Specification, NULL AS FirmOrderCode, 'Inventories/GoodsReceipts' AS ControllerName, GoodsReceiptDetails.GoodsReceiptID AS EntityID, N'HH TẠI KHO' AS GroupName, CONVERT(VARCHAR, @LocalFromDate, 103) + ' -> ' + CONVERT(VARCHAR, @LocalToDate, 103) AS SubGroupName, GoodsReceiptDetails.GoodsReceiptDetailID, GoodsReceiptDetails.EntryDate, GoodsReceiptDetails.CommodityID, GoodsReceiptDetails.Reference, NULL AS Referral, GoodsReceiptDetails.Code AS ReceiptCode, GoodsReceiptDetails.Barcode, GoodsReceiptDetails.WarehouseID, Customers.Name + ', SCT [' + ISNULL(PurchaseRequisitions.Code, '') + ' Ngày: ' + CONVERT(VARCHAR, PurchaseRequisitions.EntryDate, 103) + ']' + ISNULL(PurchaseRequisitions.Purposes, '') AS Description, GoodsReceiptDetails.Quantity AS QuantityDebit, 0 AS QuantityCredit " + "\r\n";
            queryString = queryString + "                   FROM        GoodsReceiptDetails INNER JOIN " + "\r\n";
            queryString = queryString + "                               PurchaseRequisitions ON GoodsReceiptDetails.WarehouseID = @LocalWarehouseID AND GoodsReceiptDetails.PurchaseRequisitionID = PurchaseRequisitions.PurchaseRequisitionID AND GoodsReceiptDetails.EntryDate >= @LocalFromDate AND GoodsReceiptDetails.EntryDate <= @LocalToDate INNER JOIN " + "\r\n";
            queryString = queryString + "                               Customers ON PurchaseRequisitions.CustomerID = Customers.CustomerID " + "\r\n";

            //2.1.INTPUT.GoodsArrival
            queryString = queryString + "                   UNION ALL " + "\r\n";
            queryString = queryString + "                   SELECT      " + (int)GlobalEnums.WarehouseAdjustmentTypeID.ReceiptPUR + " AS JournalTypeID, Customers.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, GoodsReceiptDetails.Remarks AS Specs, GoodsReceiptDetails.Remarks AS Specification, NULL AS FirmOrderCode, 'Inventories/' + IIF(GoodsArrivals.NmvnTaskID = " + (int)GlobalEnums.NmvnTaskID.MaterialArrival + ", 'MaterialReceipts', IIF(GoodsArrivals.NmvnTaskID = " + (int)GlobalEnums.NmvnTaskID.ItemArrival + ", 'ItemReceipts', 'ProductReceipts')) AS ControllerName, GoodsReceiptDetails.GoodsReceiptID AS EntityID, N'HH TẠI KHO' AS GroupName, CONVERT(VARCHAR, @LocalFromDate, 103) + ' -> ' + CONVERT(VARCHAR, @LocalToDate, 103) AS SubGroupName, GoodsReceiptDetails.GoodsReceiptDetailID, GoodsReceiptDetails.EntryDate, GoodsReceiptDetails.CommodityID, GoodsReceiptDetails.Reference, NULL AS Referral, GoodsReceiptDetails.Code AS ReceiptCode, GoodsReceiptDetails.Barcode, GoodsReceiptDetails.WarehouseID, Customers.Name + ', SCT [' + ISNULL(GoodsArrivals.Code, '') + ' Ngày: ' + CONVERT(VARCHAR, GoodsArrivals.EntryDate, 103) + ']' + ISNULL(GoodsArrivals.CustomsDeclaration, '') AS Description, GoodsReceiptDetails.Quantity AS QuantityDebit, 0 AS QuantityCredit " + "\r\n";
            queryString = queryString + "                   FROM        GoodsReceiptDetails INNER JOIN " + "\r\n";
            queryString = queryString + "                               GoodsArrivals ON GoodsReceiptDetails.WarehouseID = @LocalWarehouseID AND GoodsReceiptDetails.GoodsArrivalID = GoodsArrivals.GoodsArrivalID AND GoodsReceiptDetails.EntryDate >= @LocalFromDate AND GoodsReceiptDetails.EntryDate <= @LocalToDate INNER JOIN " + "\r\n";
            queryString = queryString + "                               Customers ON GoodsArrivals.CustomerID = Customers.CustomerID " + "\r\n";

            //2.1.INTPUT.Recyclate
            queryString = queryString + "                   UNION ALL " + "\r\n";
            queryString = queryString + "                   SELECT      " + (int)GlobalEnums.WarehouseAdjustmentTypeID.ReceiptREG + " AS JournalTypeID, Customers.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, GoodsReceiptDetails.Remarks AS Specs, GoodsReceiptDetails.Remarks AS Specification, NULL AS FirmOrderCode, 'Inventories/MaterialReceipts' AS ControllerName, GoodsReceiptDetails.GoodsReceiptID AS EntityID, N'HH TẠI KHO' AS GroupName, CONVERT(VARCHAR, @LocalFromDate, 103) + ' -> ' + CONVERT(VARCHAR, @LocalToDate, 103) AS SubGroupName, GoodsReceiptDetails.GoodsReceiptDetailID, GoodsReceiptDetails.EntryDate, GoodsReceiptDetails.CommodityID, GoodsReceiptDetails.Reference, NULL AS Referral, GoodsReceiptDetails.Code AS ReceiptCode, GoodsReceiptDetails.Barcode, GoodsReceiptDetails.WarehouseID, Customers.Name + ', SCT [' + ISNULL(Recyclates.Reference, '') + ' Ngày: ' + CONVERT(VARCHAR, Recyclates.EntryDate, 103) + '] ' + 'của ' + Workshifts.Code  + ', ngày: ' + CONVERT(VARCHAR, Recyclates.EntryDate, 103) AS Description, GoodsReceiptDetails.Quantity AS QuantityDebit, 0 AS QuantityCredit " + "\r\n";
            queryString = queryString + "                   FROM        GoodsReceiptDetails INNER JOIN " + "\r\n";
            queryString = queryString + "                               Recyclates ON GoodsReceiptDetails.WarehouseID = @LocalWarehouseID AND GoodsReceiptDetails.RecyclateID = Recyclates.RecyclateID AND GoodsReceiptDetails.EntryDate >= @LocalFromDate AND GoodsReceiptDetails.EntryDate <= @LocalToDate INNER JOIN " + "\r\n";
            queryString = queryString + "                               Workshifts ON Recyclates.WorkshiftID = Workshifts.WorkshiftID INNER JOIN " + "\r\n";
            queryString = queryString + "                               Customers ON Customers.CustomerID = 1 " + "\r\n";

            //2.2.INTPUT.FinishedProduct
            queryString = queryString + "                   UNION ALL " + "\r\n";
            queryString = queryString + "                   SELECT      " + (int)GlobalEnums.WarehouseAdjustmentTypeID.ReceiptPRO + " AS JournalTypeID, Customers.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, FirmOrders.Specs, FirmOrders.Specification, FirmOrders.Code AS FirmOrderCode, 'Inventories/ProductReceipts' AS ControllerName, GoodsReceiptDetails.GoodsReceiptID AS EntityID, N'HH TẠI KHO' AS GroupName, CONVERT(VARCHAR, @LocalFromDate, 103) + ' -> ' + CONVERT(VARCHAR, @LocalToDate, 103) AS SubGroupName, GoodsReceiptDetails.GoodsReceiptDetailID, GoodsReceiptDetails.EntryDate, GoodsReceiptDetails.CommodityID, GoodsReceiptDetails.Reference, NULL AS Referral, GoodsReceiptDetails.Code AS ReceiptCode, GoodsReceiptDetails.Barcode, GoodsReceiptDetails.WarehouseID, Customers.Name + ', KHSX: ' + FirmOrders.Reference + ' SCT [' + ISNULL(FirmOrders.Code, '') + ' Ngày: ' + CONVERT(VARCHAR, FirmOrders.EntryDate, 103) + ']' AS Description, GoodsReceiptDetails.Quantity AS QuantityDebit, 0 AS QuantityCredit " + "\r\n";
            queryString = queryString + "                   FROM        GoodsReceiptDetails INNER JOIN " + "\r\n";
            queryString = queryString + "                               FinishedProducts ON GoodsReceiptDetails.WarehouseID = @LocalWarehouseID AND GoodsReceiptDetails.FinishedProductID = FinishedProducts.FinishedProductID AND GoodsReceiptDetails.EntryDate >= @LocalFromDate AND GoodsReceiptDetails.EntryDate <= @LocalToDate INNER JOIN " + "\r\n";
            queryString = queryString + "                               Customers ON FinishedProducts.CustomerID = Customers.CustomerID INNER JOIN " + "\r\n";
            queryString = queryString + "                               FirmOrders ON FinishedProducts.FirmOrderID = FirmOrders.FirmOrderID " + "\r\n";

            //2.2.INTPUT.FinishedItem
            queryString = queryString + "                   UNION ALL " + "\r\n";
            queryString = queryString + "                   SELECT      " + (int)GlobalEnums.WarehouseAdjustmentTypeID.ReceiptPRO + " AS JournalTypeID, Customers.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, FirmOrders.Specs, FirmOrders.Specification, FirmOrders.Code AS FirmOrderCode, 'Inventories/ItemReceipts' AS ControllerName, GoodsReceiptDetails.GoodsReceiptID AS EntityID, N'HH TẠI KHO' AS GroupName, CONVERT(VARCHAR, @LocalFromDate, 103) + ' -> ' + CONVERT(VARCHAR, @LocalToDate, 103) AS SubGroupName, GoodsReceiptDetails.GoodsReceiptDetailID, GoodsReceiptDetails.EntryDate, GoodsReceiptDetails.CommodityID, GoodsReceiptDetails.Reference, NULL AS Referral, GoodsReceiptDetails.Code AS ReceiptCode, GoodsReceiptDetails.Barcode, GoodsReceiptDetails.WarehouseID, Customers.Name + ', KHSX: ' + FirmOrders.Reference + ' SCT [' + ISNULL(FirmOrders.Code, '') + ' Ngày: ' + CONVERT(VARCHAR, FirmOrders.EntryDate, 103) + ']' AS Description, GoodsReceiptDetails.Quantity AS QuantityDebit, 0 AS QuantityCredit " + "\r\n";
            queryString = queryString + "                   FROM        GoodsReceiptDetails INNER JOIN " + "\r\n";
            queryString = queryString + "                               FinishedItems ON GoodsReceiptDetails.WarehouseID = @LocalWarehouseID AND GoodsReceiptDetails.FinishedItemID = FinishedItems.FinishedItemID AND GoodsReceiptDetails.EntryDate >= @LocalFromDate AND GoodsReceiptDetails.EntryDate <= @LocalToDate INNER JOIN " + "\r\n";
            queryString = queryString + "                               Customers ON FinishedItems.CustomerID = Customers.CustomerID INNER JOIN " + "\r\n";
            queryString = queryString + "                               FirmOrders ON FinishedItems.FirmOrderID = FirmOrders.FirmOrderID " + "\r\n";

            //2.3.INTPUT.MaterialIssue
            queryString = queryString + "                   UNION ALL " + "\r\n";
            queryString = queryString + "                   SELECT      " + (int)GlobalEnums.WarehouseAdjustmentTypeID.ReceiptFOI + " AS JournalTypeID, Customers.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, FirmOrders.Specs, FirmOrders.Specification, FirmOrders.Code AS FirmOrderCode, 'Inventories/ItemReceipts' AS ControllerName, GoodsReceiptDetails.GoodsReceiptID AS EntityID, N'HH TẠI KHO' AS GroupName, CONVERT(VARCHAR, @LocalFromDate, 103) + ' -> ' + CONVERT(VARCHAR, @LocalToDate, 103) AS SubGroupName, GoodsReceiptDetails.GoodsReceiptDetailID, GoodsReceiptDetails.EntryDate, GoodsReceiptDetails.CommodityID, GoodsReceiptDetails.Reference, NULL AS Referral, GoodsReceiptDetails.Code AS ReceiptCode, GoodsReceiptDetails.Barcode, GoodsReceiptDetails.WarehouseID, N'MÁY ' + ProductionLines.Code + ' ' + Workshifts.Code + ' [' + CONVERT(VARCHAR, MaterialIssues.EntryDate, 103) + ']' +  ', KH: ' + Customers.Name AS Description, GoodsReceiptDetails.Quantity AS QuantityDebit, 0 AS QuantityCredit " + "\r\n";
            queryString = queryString + "                   FROM        GoodsReceiptDetails INNER JOIN " + "\r\n";
            queryString = queryString + "                               MaterialIssues ON GoodsReceiptDetails.WarehouseID = @LocalWarehouseID AND GoodsReceiptDetails.MaterialIssueID = MaterialIssues.MaterialIssueID AND GoodsReceiptDetails.EntryDate >= @LocalFromDate AND GoodsReceiptDetails.EntryDate <= @LocalToDate INNER JOIN " + "\r\n";
            queryString = queryString + "                               Customers ON MaterialIssues.CustomerID = Customers.CustomerID INNER JOIN " + "\r\n";
            queryString = queryString + "                               FirmOrders ON MaterialIssues.FirmOrderID = FirmOrders.FirmOrderID INNER JOIN " + "\r\n";
            queryString = queryString + "                               Workshifts ON MaterialIssues.WorkshiftID = Workshifts.WorkshiftID INNER JOIN " + "\r\n";
            queryString = queryString + "                               ProductionLines ON MaterialIssues.ProductionLineID = ProductionLines.ProductionLineID " + "\r\n";

            //2.4.INTPUT.GoodsReturn

            //2.5.INTPUT.WarehouseTransfer
            queryString = queryString + "                   UNION ALL " + "\r\n";
            queryString = queryString + "                   SELECT      " + (int)GlobalEnums.WarehouseAdjustmentTypeID.ReceiptTXF + " AS JournalTypeID, NULL AS CustomerID, NULL AS CustomerCode, NULL AS CustomerName, GoodsReceiptDetails.Remarks AS Specs, GoodsReceiptDetails.Remarks AS Specification, NULL AS FirmOrderCode, 'Inventories/' + IIF(WarehouseTransfers.NmvnTaskID = " + (int)GlobalEnums.NmvnTaskID.MaterialTransfer + ", 'MaterialReceipts', IIF(WarehouseTransfers.NmvnTaskID = " + (int)GlobalEnums.NmvnTaskID.ItemTransfer + ", 'ItemReceipts', 'ProductReceipts')) AS ControllerName, GoodsReceiptDetails.GoodsReceiptID AS EntityID, N'HH TẠI KHO' AS GroupName, CONVERT(VARCHAR, @LocalFromDate, 103) + ' -> ' + CONVERT(VARCHAR, @LocalToDate, 103) AS SubGroupName, GoodsReceiptDetails.GoodsReceiptDetailID, GoodsReceiptDetails.EntryDate, GoodsReceiptDetails.CommodityID, GoodsReceiptDetails.Reference, NULL AS Referral, GoodsReceiptDetails.Code AS ReceiptCode, GoodsReceiptDetails.Barcode, GoodsReceiptDetails.WarehouseID, 'NHAP VCNB: ' + Warehouses.Name + ', PX [' + WarehouseTransfers.Reference + ' Ngày: ' + CONVERT(VARCHAR, WarehouseTransfers.EntryDate, 103) + ']' AS Description, GoodsReceiptDetails.Quantity AS QuantityDebit, 0 AS QuantityCredit " + "\r\n";
            queryString = queryString + "                   FROM        GoodsReceiptDetails INNER JOIN " + "\r\n";
            queryString = queryString + "                               WarehouseTransfers ON GoodsReceiptDetails.WarehouseID = @LocalWarehouseID AND GoodsReceiptDetails.WarehouseTransferID = WarehouseTransfers.WarehouseTransferID AND GoodsReceiptDetails.EntryDate >= @LocalFromDate AND GoodsReceiptDetails.EntryDate <= @LocalToDate INNER JOIN " + "\r\n";
            queryString = queryString + "                               Warehouses ON WarehouseTransfers.WarehouseID = Warehouses.WarehouseID " + "\r\n";

            //2.6.INTPUT.WarehouseAdjustments
            queryString = queryString + "                   UNION ALL " + "\r\n";
            queryString = queryString + "                   SELECT      WarehouseAdjustments.WarehouseAdjustmentTypeID AS JournalTypeID, Customers.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, WarehouseAdjustments.AdjustmentJobs AS Specs, WarehouseAdjustments.AdjustmentJobs AS Specification, NULL AS FirmOrderCode, 'Inventories/' + CASE WarehouseAdjustments.NMVNTaskID " + warehouseAdjustmentController + " AS ControllerName, WarehouseAdjustments.WarehouseAdjustmentID AS EntityID, N'HH TẠI KHO' AS GroupName, CONVERT(VARCHAR, @LocalFromDate, 103) + ' -> ' + CONVERT(VARCHAR, @LocalToDate, 103) AS SubGroupName, GoodsReceiptDetails.GoodsReceiptDetailID, GoodsReceiptDetails.EntryDate, GoodsReceiptDetails.CommodityID, GoodsReceiptDetails.Reference, NULL AS Referral, GoodsReceiptDetails.Code AS ReceiptCode, GoodsReceiptDetails.Barcode, GoodsReceiptDetails.WarehouseID, Customers.Name + ' ' + WarehouseAdjustments.AdjustmentJobs AS Description, GoodsReceiptDetails.Quantity AS QuantityDebit, 0 AS QuantityCredit " + "\r\n";
            queryString = queryString + "                   FROM        GoodsReceiptDetails INNER JOIN " + "\r\n";
            queryString = queryString + "                               WarehouseAdjustments ON GoodsReceiptDetails.WarehouseID = @LocalWarehouseID AND GoodsReceiptDetails.WarehouseAdjustmentID = WarehouseAdjustments.WarehouseAdjustmentID AND GoodsReceiptDetails.EntryDate >= @LocalFromDate AND GoodsReceiptDetails.EntryDate <= @LocalToDate INNER JOIN " + "\r\n";
            queryString = queryString + "                               Customers ON WarehouseAdjustments.CustomerID = Customers.CustomerID " + "\r\n";




            //3.OUTPUT (CAC CAU SQL CHO INVOICE, WarehouseTransferDetails, WHADJUST, WHASSEMBLY LA HOAN TOAN GIONG NHAU. LUU Y T/H DAT BIET: WHADJUST.QUANTITY < 0)

            queryString = queryString + "                   UNION ALL " + "\r\n";
            //3.1.MaterialIssueDetails + "\r\n";
            queryString = queryString + "                   SELECT      " + (int)GlobalEnums.WarehouseAdjustmentTypeID.IssuePRO + " AS JournalTypeID, Customers.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, FirmOrders.Specs, FirmOrders.Specification, FirmOrders.Code AS FirmOrderCode, 'Inventories/' + IIF(MaterialIssueDetails.NmvnTaskID = " + (int)GlobalEnums.NmvnTaskID.MaterialStaging + ", 'MaterialStagings', IIF(MaterialIssueDetails.NmvnTaskID = " + (int)GlobalEnums.NmvnTaskID.ItemStaging + ", 'ItemStagings', 'ProductStagings')) AS ControllerName, MaterialIssueDetails.MaterialIssueID AS EntityID, N'HH TẠI KHO' AS GroupName, CONVERT(VARCHAR, @LocalFromDate, 103) + ' -> ' + CONVERT(VARCHAR, @LocalToDate, 103) AS SubGroupName, GoodsReceiptDetails.GoodsReceiptDetailID, MaterialIssueDetails.EntryDate, GoodsReceiptDetails.CommodityID, MaterialIssueDetails.Reference, GoodsReceiptDetails.Reference AS Referral, GoodsReceiptDetails.Code AS ReceiptCode, GoodsReceiptDetails.Barcode, GoodsReceiptDetails.WarehouseID, N'MÁY ' + ProductionLines.Code + ' ' + Workshifts.Code + ' [' + CONVERT(VARCHAR, MaterialIssueDetails.EntryDate, 103) + ']' +  ', KH: ' + Customers.Name AS Description, 0 AS QuantityDebit, MaterialIssueDetails.Quantity AS QuantityCredit " + "\r\n";
            queryString = queryString + "                   FROM        MaterialIssueDetails INNER JOIN " + "\r\n";
            queryString = queryString + "                               GoodsReceiptDetails ON MaterialIssueDetails.WarehouseID = @LocalWarehouseID AND MaterialIssueDetails.GoodsReceiptDetailID = GoodsReceiptDetails.GoodsReceiptDetailID AND MaterialIssueDetails.EntryDate >= @LocalFromDate AND MaterialIssueDetails.EntryDate <= @LocalToDate INNER JOIN " + "\r\n";
            queryString = queryString + "                               Customers ON MaterialIssueDetails.CustomerID = Customers.CustomerID INNER JOIN " + "\r\n";
            queryString = queryString + "                               FirmOrders ON MaterialIssueDetails.FirmOrderID = FirmOrders.FirmOrderID INNER JOIN " + "\r\n";
            queryString = queryString + "                               Workshifts ON MaterialIssueDetails.WorkshiftID = Workshifts.WorkshiftID INNER JOIN " + "\r\n";
            queryString = queryString + "                               ProductionLines ON MaterialIssueDetails.ProductionLineID = ProductionLines.ProductionLineID " + "\r\n";


            queryString = queryString + "                   UNION ALL " + "\r\n";
            //3.1.PackageIssueDetails + "\r\n";
            queryString = queryString + "                   SELECT      " + (int)GlobalEnums.WarehouseAdjustmentTypeID.IssuePRO + " AS JournalTypeID, Customers.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, BlendingInstructions.Jobs AS Specs, Commodities.Code + ' - ' + Commodities.OfficialCode AS Specification, BlendingInstructions.Code AS FirmOrderCode, 'Inventories/PackageIssues' AS ControllerName, PackageIssueDetails.PackageIssueID AS EntityID, N'HH TẠI KHO' AS GroupName, CONVERT(VARCHAR, @LocalFromDate, 103) + ' -> ' + CONVERT(VARCHAR, @LocalToDate, 103) AS SubGroupName, GoodsReceiptDetails.GoodsReceiptDetailID, PackageIssueDetails.EntryDate, GoodsReceiptDetails.CommodityID, PackageIssueDetails.Reference, GoodsReceiptDetails.Reference AS Referral, GoodsReceiptDetails.Code AS ReceiptCode, GoodsReceiptDetails.Barcode, GoodsReceiptDetails.WarehouseID, BlendingInstructions.Code + ', TP: ' + Commodities.Code AS Description, 0 AS QuantityDebit, PackageIssueDetails.Quantity AS QuantityCredit " + "\r\n";
            queryString = queryString + "                   FROM        PackageIssueDetails INNER JOIN " + "\r\n";
            queryString = queryString + "                               GoodsReceiptDetails ON PackageIssueDetails.WarehouseID = @LocalWarehouseID AND PackageIssueDetails.GoodsReceiptDetailID = GoodsReceiptDetails.GoodsReceiptDetailID AND PackageIssueDetails.EntryDate >= @LocalFromDate AND PackageIssueDetails.EntryDate <= @LocalToDate INNER JOIN " + "\r\n";
            queryString = queryString + "                               Customers ON 1 = Customers.CustomerID INNER JOIN " + "\r\n";
            queryString = queryString + "                               BlendingInstructions ON PackageIssueDetails.BlendingInstructionID = BlendingInstructions.BlendingInstructionID INNER JOIN " + "\r\n";
            queryString = queryString + "                               Commodities ON BlendingInstructions.CommodityID = Commodities.CommodityID " + "\r\n";


            queryString = queryString + "                   UNION ALL " + "\r\n";
            //3.1.GoodsIssuePackages + "\r\n";
            queryString = queryString + "                   SELECT      " + (int)GlobalEnums.WarehouseAdjustmentTypeID.IssueINV + " AS JournalTypeID, Customers.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, NULL AS Specs, NULL AS Specification, NULL AS FirmOrderCode, 'Inventories/GoodsIssues' AS ControllerName, GoodsIssuePackages.GoodsIssueID AS EntityID, N'HH TẠI KHO' AS GroupName, CONVERT(VARCHAR, @LocalFromDate, 103) + ' -> ' + CONVERT(VARCHAR, @LocalToDate, 103) AS SubGroupName, GoodsReceiptDetails.GoodsReceiptDetailID, GoodsIssuePackages.EntryDate, GoodsReceiptDetails.CommodityID, GoodsIssuePackages.Reference, GoodsReceiptDetails.Reference AS Referral, GoodsReceiptDetails.Code AS ReceiptCode, GoodsReceiptDetails.Barcode, GoodsReceiptDetails.WarehouseID, Customers.Name AS Description, 0 AS QuantityDebit, GoodsIssuePackages.Quantity AS QuantityCredit " + "\r\n";
            queryString = queryString + "                   FROM        GoodsIssuePackages INNER JOIN " + "\r\n";
            queryString = queryString + "                               GoodsReceiptDetails ON GoodsIssuePackages.WarehouseID = @LocalWarehouseID AND GoodsIssuePackages.GoodsReceiptDetailID = GoodsReceiptDetails.GoodsReceiptDetailID AND GoodsIssuePackages.EntryDate >= @LocalFromDate AND GoodsIssuePackages.EntryDate <= @LocalToDate INNER JOIN " + "\r\n";
            queryString = queryString + "                               Customers ON GoodsIssuePackages.CustomerID = Customers.CustomerID " + "\r\n";


            queryString = queryString + "                   UNION ALL " + "\r\n";
            //3.2.WarehouseTransferDetails
            queryString = queryString + "                   SELECT      " + (int)GlobalEnums.WarehouseAdjustmentTypeID.IssueTXF + " AS JournalTypeID, NULL AS CustomerID, NULL AS CustomerCode, NULL AS CustomerName, WarehouseTransferDetails.Remarks AS Specs, WarehouseTransferDetails.Remarks AS Specification, NULL AS FirmOrderCode, 'Inventories/' + IIF(WarehouseTransferDetails.NmvnTaskID = " + (int)GlobalEnums.NmvnTaskID.MaterialTransfer + ", 'MaterialTransfers', IIF(WarehouseTransferDetails.NmvnTaskID = " + (int)GlobalEnums.NmvnTaskID.ItemTransfer + ", 'ItemTransfers', 'ProductTransfers')) AS ControllerName, WarehouseTransferDetails.WarehouseTransferID AS EntityID, N'HH TẠI KHO' AS GroupName, CONVERT(VARCHAR, @LocalFromDate, 103) + ' -> ' + CONVERT(VARCHAR, @LocalToDate, 103) AS SubGroupName, GoodsReceiptDetails.GoodsReceiptDetailID, WarehouseTransferDetails.EntryDate, GoodsReceiptDetails.CommodityID, WarehouseTransferDetails.Reference, GoodsReceiptDetails.Reference AS Referral, GoodsReceiptDetails.Code AS ReceiptCode, GoodsReceiptDetails.Barcode, GoodsReceiptDetails.WarehouseID, 'XUAT VCNB: ' + WarehouseReceipts.Name AS Description, 0 AS QuantityDebit, WarehouseTransferDetails.Quantity AS QuantityCredit " + "\r\n";
            queryString = queryString + "                   FROM        WarehouseTransferDetails INNER JOIN " + "\r\n";
            queryString = queryString + "                               GoodsReceiptDetails ON WarehouseTransferDetails.WarehouseID = @LocalWarehouseID AND WarehouseTransferDetails.GoodsReceiptDetailID = GoodsReceiptDetails.GoodsReceiptDetailID AND WarehouseTransferDetails.EntryDate >= @LocalFromDate AND WarehouseTransferDetails.EntryDate <= @LocalToDate INNER JOIN " + "\r\n";
            queryString = queryString + "                               Warehouses WarehouseReceipts ON WarehouseTransferDetails.WarehouseReceiptID = WarehouseReceipts.WarehouseID " + "\r\n";

            queryString = queryString + "                   UNION ALL " + "\r\n";
            //3.3.WarehouseAdjustmentDetails + "\r\n";
            queryString = queryString + "                   SELECT      WarehouseAdjustments.WarehouseAdjustmentTypeID AS JournalTypeID, Customers.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, WarehouseAdjustments.AdjustmentJobs AS Specs, WarehouseAdjustments.AdjustmentJobs AS Specification, NULL AS FirmOrderCode, 'Inventories/' + CASE WarehouseAdjustmentDetails.NMVNTaskID " + warehouseAdjustmentController + " AS ControllerName, WarehouseAdjustmentDetails.WarehouseAdjustmentID AS EntityID, N'HH TẠI KHO' AS GroupName, CONVERT(VARCHAR, @LocalFromDate, 103) + ' -> ' + CONVERT(VARCHAR, @LocalToDate, 103) AS SubGroupName, GoodsReceiptDetails.GoodsReceiptDetailID, WarehouseAdjustmentDetails.EntryDate, GoodsReceiptDetails.CommodityID, WarehouseAdjustmentDetails.Reference, GoodsReceiptDetails.Reference AS Referral, GoodsReceiptDetails.Code AS ReceiptCode, GoodsReceiptDetails.Barcode, GoodsReceiptDetails.WarehouseID, WarehouseAdjustmentTypes.Code + ' ' + Customers.Name + ' ' + ISNULL(WarehouseAdjustments.AdjustmentJobs, '') AS Description, 0 AS QuantityDebit, -WarehouseAdjustmentDetails.Quantity AS QuantityCredit " + "\r\n";
            queryString = queryString + "                   FROM        WarehouseAdjustmentDetails INNER JOIN " + "\r\n";
            queryString = queryString + "                               WarehouseAdjustments ON WarehouseAdjustmentDetails.WarehouseAdjustmentID = WarehouseAdjustments.WarehouseAdjustmentID INNER JOIN  " + "\r\n";
            queryString = queryString + "                               GoodsReceiptDetails ON WarehouseAdjustmentDetails.WarehouseID = @LocalWarehouseID AND WarehouseAdjustmentDetails.GoodsReceiptDetailID = GoodsReceiptDetails.GoodsReceiptDetailID AND WarehouseAdjustmentDetails.EntryDate >= @LocalFromDate AND WarehouseAdjustmentDetails.EntryDate <= @LocalToDate INNER JOIN " + "\r\n";
            queryString = queryString + "                               Customers ON WarehouseAdjustmentDetails.CustomerID = Customers.CustomerID INNER JOIN " + "\r\n";
            queryString = queryString + "                               WarehouseAdjustmentTypes ON WarehouseAdjustments.WarehouseAdjustmentTypeID = WarehouseAdjustmentTypes.WarehouseAdjustmentTypeID " + "\r\n";

            //--BEGIN-INPUT-OUTPUT-END.END







            ////////B.PENDING
            ////////B.1.PENDING.ON SHIP
            //////queryString = queryString + "                   UNION ALL " + "\r\n";
            ////////--ON SHIP.BEGIN
            //////queryString = queryString + "                   SELECT      N'TỒN SX [DỞ DANG]' AS GroupName, 'DA DAT HANG' AS SubGroupName, GoodsReceiptDetails.GoodsReceiptDetailID, PurchaseOrderDetails.EntryDate, PurchaseOrderDetails.CommodityID, '' AS Reference, GoodsReceiptDetails.Reference AS Referral, '' AS ReceiptCode, GoodsReceiptDetails.Barcode, 0 AS WarehouseID, Suppliers.Name + ', ĐH [' + PurchaseOrders.ConfirmReference + ' Ngày XN: ' + CONVERT(VARCHAR, PurchaseOrders.ConfirmDate, 103) + ']' AS Description, PurchaseOrderDetails.Quantity - PurchaseOrderDetails.QuantityInvoice AS QuantityDebit, 0 AS QuantityCredit " + "\r\n";
            //////queryString = queryString + "                   FROM        PurchaseOrderDetails INNER JOIN " + "\r\n";
            //////queryString = queryString + "                               PurchaseOrders ON PurchaseOrderDetails.LocationID = @LocationID AND PurchaseOrderDetails.EntryDate <= @LocalToDate AND PurchaseOrderDetails.Quantity > PurchaseOrderDetails.QuantityInvoice AND PurchaseOrderDetails.PurchaseOrderID = PurchaseOrders.PurchaseOrderID INNER JOIN " + "\r\n";
            //////queryString = queryString + "                               Customers Suppliers ON PurchaseOrderDetails.SupplierID = Suppliers.CustomerID " + "\r\n";

            //////queryString = queryString + "                   UNION ALL " + "\r\n";

            //////queryString = queryString + "                   SELECT      N'TỒN SX [DỞ DANG]' AS GroupName, 'DA DAT HANG' AS SubGroupName, GoodsReceiptDetails.GoodsReceiptDetailID, PurchaseOrders.EntryDate, PurchaseInvoiceDetails.CommodityID, '' AS Reference, GoodsReceiptDetails.Reference AS Referral, '' AS ReceiptCode, GoodsReceiptDetails.Barcode, 0 AS WarehouseID, Suppliers.Name + ', ĐH [' + PurchaseOrders.ConfirmReference + ' Ngày XN: ' + CONVERT(VARCHAR, PurchaseOrders.ConfirmDate, 103) + ']' AS Description, PurchaseInvoiceDetails.Quantity AS QuantityDebit, 0 AS QuantityCredit " + "\r\n";
            //////queryString = queryString + "                   FROM        PurchaseOrders INNER JOIN " + "\r\n";
            //////queryString = queryString + "                               PurchaseInvoiceDetails ON PurchaseOrders.LocationID = @LocationID AND PurchaseOrders.PurchaseOrderID = PurchaseInvoiceDetails.PurchaseOrderID AND PurchaseOrders.EntryDate <= @LocalToDate AND PurchaseInvoiceDetails.EntryDate > @LocalToDate INNER JOIN " + "\r\n";
            //////queryString = queryString + "                               Customers Suppliers ON PurchaseOrders.SupplierID = Suppliers.CustomerID " + "\r\n";
            ////////--ON SHIP.END

            ////////B.1.PENDING.ON RECEIPT
            //////queryString = queryString + "                   UNION ALL " + "\r\n";
            ////////--ON INPUT.BEGIN (CAC CAU SQL DUNG CHO EWHInputVoucherTypeID.EInvoice, EWHInputVoucherTypeID.EReturn, EWHInputVoucherTypeID.EWHTransfer, EWHInputVoucherTypeID.EWHAdjust, EWHInputVoucherTypeID.EWHAssemblyMaster, EWHInputVoucherTypeID.EWHAssemblyDetail LA HOAN TOAN GIONG NHAU)
            ////////EWHInputVoucherTypeID.EInvoice
            //////queryString = queryString + "                   SELECT      N'TỒN SX [DỞ DANG]' AS GroupName, 'CHO NHAP KHO' AS SubGroupName, GoodsReceiptDetails.GoodsReceiptDetailID, PurchaseInvoiceDetails.EntryDate, PurchaseInvoiceDetails.CommodityID, '' AS Reference, GoodsReceiptDetails.Reference AS Referral, '' AS ReceiptCode, GoodsReceiptDetails.Barcode, 0 AS WarehouseID, Suppliers.Name + ', HĐ [' + ISNULL(PurchaseInvoices.VATInvoiceNo, '') + ' Ngày: ' + CONVERT(VARCHAR, PurchaseInvoices.EntryDate, 103) + ']' AS Description, PurchaseInvoiceDetails.Quantity - PurchaseInvoiceDetails.QuantityReceipt AS QuantityDebit, 0 AS QuantityCredit " + "\r\n";
            //////queryString = queryString + "                   FROM        PurchaseInvoiceDetails INNER JOIN " + "\r\n";
            //////queryString = queryString + "                               PurchaseInvoices ON PurchaseInvoiceDetails.LocationID = @LocationID AND PurchaseInvoiceDetails.EntryDate <= @LocalToDate AND PurchaseInvoiceDetails.Quantity > PurchaseInvoiceDetails.QuantityReceipt AND PurchaseInvoiceDetails.PurchaseInvoiceID = PurchaseInvoices.PurchaseInvoiceID INNER JOIN " + "\r\n";
            //////queryString = queryString + "                               Customers Suppliers ON PurchaseInvoiceDetails.SupplierID = Suppliers.CustomerID " + "\r\n";
            //////queryString = queryString + "                               " + "\r\n";

            //////queryString = queryString + "                   UNION ALL " + "\r\n";

            //////queryString = queryString + "                   SELECT      N'TỒN SX [DỞ DANG]' AS GroupName, 'CHO NHAP KHO' AS SubGroupName, GoodsReceiptDetails.GoodsReceiptDetailID, PurchaseInvoices.EntryDate, GoodsReceiptDetails.CommodityID, '' AS Reference, GoodsReceiptDetails.Reference AS Referral, '' AS ReceiptCode, GoodsReceiptDetails.Barcode, 0 AS WarehouseID, Suppliers.Name + ', HĐ [' + ISNULL(PurchaseInvoices.VATInvoiceNo, '') + ' Ngày: ' + CONVERT(VARCHAR, PurchaseInvoices.EntryDate, 103) + ']' AS Description, GoodsReceiptDetails.Quantity AS QuantityDebit, 0 AS QuantityCredit " + "\r\n";
            //////queryString = queryString + "                   FROM        PurchaseInvoices INNER JOIN " + "\r\n";
            //////queryString = queryString + "                               GoodsReceiptDetails ON PurchaseInvoices.LocationID = @LocationID AND PurchaseInvoices.PurchaseInvoiceID = GoodsReceiptDetails.VoucherID AND GoodsReceiptDetails.GoodsReceiptTypeID = " + (int)GlobalEnums.GoodsReceiptTypeID.PurchaseInvoice + " AND PurchaseInvoices.EntryDate <= @LocalToDate AND GoodsReceiptDetails.EntryDate > @LocalToDate INNER JOIN " + "\r\n";
            //////queryString = queryString + "                               Customers Suppliers ON PurchaseInvoices.SupplierID = Suppliers.CustomerID " + "\r\n";


            //B.1.PENDING.ON FinishedProducts
            queryString = queryString + "                   UNION ALL " + "\r\n";

            queryString = queryString + "                   SELECT      " + (int)GlobalEnums.WarehouseAdjustmentTypeID.ReceiptPENDING + " AS JournalTypeID, Customers.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, FirmOrders.Specs, FirmOrders.Specification, FirmOrders.Code AS FirmOrderCode, 'Productions/FinishedProducts' AS ControllerName, FinishedProductPackages.FinishedProductID AS EntityID, N'TỒN SX [DỞ DANG]' AS GroupName, 'X.' + CONVERT(VARCHAR, @LocalToDate, 103) AS SubGroupName, NULL AS GoodsReceiptDetailID, FinishedProductPackages.EntryDate, FinishedProductPackages.CommodityID, NULL AS Reference, NULL AS Referral, NULL AS ReceiptCode, NULL AS Barcode, 0 AS WarehouseID, Customers.Name + ', KHSX: ' + FirmOrders.Reference + ' SCT [' + ISNULL(FirmOrders.Code, '') + ' Ngày: ' + CONVERT(VARCHAR, FirmOrders.EntryDate, 103) + ']' AS Description, FinishedProductPackages.Quantity - FinishedProductPackages.QuantityReceipted AS QuantityDebit, 0 AS QuantityCredit " + "\r\n";
            queryString = queryString + "                   FROM        FinishedProductPackages INNER JOIN " + "\r\n";
            queryString = queryString + "                               Customers ON FinishedProductPackages.LocationID = @LocationID AND @LocalWarehouseID = 3 AND FinishedProductPackages.EntryDate <= @LocalToDate AND FinishedProductPackages.Quantity > FinishedProductPackages.QuantityReceipted AND FinishedProductPackages.CustomerID = Customers.CustomerID INNER JOIN " + "\r\n";
            queryString = queryString + "                               FirmOrders ON FinishedProductPackages.FirmOrderID = FirmOrders.FirmOrderID " + "\r\n";

            queryString = queryString + "                   UNION ALL " + "\r\n";

            queryString = queryString + "                   SELECT      " + (int)GlobalEnums.WarehouseAdjustmentTypeID.ReceiptPENDING + " AS JournalTypeID, Customers.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, FirmOrders.Specs, FirmOrders.Specification, FirmOrders.Code AS FirmOrderCode, 'Productions/FinishedProducts' AS ControllerName, FinishedProducts.FinishedProductID AS EntityID, N'TỒN SX [DỞ DANG]' AS GroupName, 'X.' + CONVERT(VARCHAR, @LocalToDate, 103) AS SubGroupName, NULL AS GoodsReceiptDetailID, FinishedProducts.EntryDate, GoodsReceiptDetails.CommodityID, NULL AS Reference, NULL AS Referral, NULL AS ReceiptCode, NULL AS Barcode, 0 AS WarehouseID, Customers.Name + ', KHSX: ' + FirmOrders.Reference + ' SCT [' + ISNULL(FirmOrders.Code, '') + ' Ngày: ' + CONVERT(VARCHAR, FirmOrders.EntryDate, 103) + ']' AS Description, GoodsReceiptDetails.Quantity AS QuantityDebit, 0 AS QuantityCredit " + "\r\n";
            queryString = queryString + "                   FROM        FinishedProducts INNER JOIN " + "\r\n";
            queryString = queryString + "                               GoodsReceiptDetails ON FinishedProducts.LocationID = @LocationID AND @LocalWarehouseID = 3 AND FinishedProducts.FinishedProductID = GoodsReceiptDetails.FinishedProductID AND FinishedProducts.EntryDate <= @LocalToDate AND GoodsReceiptDetails.EntryDate > @LocalToDate INNER JOIN " + "\r\n";
            queryString = queryString + "                               Customers ON FinishedProducts.CustomerID = Customers.CustomerID INNER JOIN " + "\r\n";
            queryString = queryString + "                               FirmOrders ON FinishedProducts.FirmOrderID = FirmOrders.FirmOrderID " + "\r\n";




            ////MaterialIssueDetails
            queryString = queryString + "                   UNION ALL " + "\r\n";

            queryString = queryString + "                   SELECT      " + (int)GlobalEnums.WarehouseAdjustmentTypeID.ReceiptPENDING + " AS JournalTypeID, Customers.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, FirmOrders.Specs, FirmOrders.Specification, FirmOrders.Code AS FirmOrderCode, 'Inventories/' + IIF(MaterialIssueDetails.NmvnTaskID = " + (int)GlobalEnums.NmvnTaskID.MaterialStaging + ", 'MaterialStagings', IIF(MaterialIssueDetails.NmvnTaskID = " + (int)GlobalEnums.NmvnTaskID.ItemStaging + ", 'ItemStagings', 'ProductStagings')) AS ControllerName, MaterialIssueDetails.MaterialIssueID AS EntityID, N'TỒN SX [DỞ DANG]' AS GroupName, 'X.' + CONVERT(VARCHAR, @LocalToDate, 103) AS SubGroupName, NULL AS GoodsReceiptDetailID, MaterialIssueDetails.EntryDate, GoodsReceiptDetails.CommodityID, MaterialIssueDetails.Reference, GoodsReceiptDetails.Reference AS Referral, GoodsReceiptDetails.Code AS ReceiptCode, GoodsReceiptDetails.Barcode, GoodsReceiptDetails.WarehouseID, N'MÁY ' + ProductionLines.Code + ' ' + Workshifts.Code + ' [' + CONVERT(VARCHAR, MaterialIssueDetails.EntryDate, 103) + ']' +  ', KH: ' + Customers.Name AS Description, ROUND(MaterialIssueDetails.Quantity - MaterialIssueDetails.QuantitySemifinished - MaterialIssueDetails.QuantityFailure - MaterialIssueDetails.QuantityReceipted - MaterialIssueDetails.QuantityLoss, " + (int)GlobalEnums.rndQuantity + ") AS QuantityDebit, 0 AS QuantityCredit " + "\r\n";
            queryString = queryString + "                   FROM        MaterialIssueDetails INNER JOIN " + "\r\n";
            queryString = queryString + "                               GoodsReceiptDetails ON MaterialIssueDetails.WarehouseID = @LocalWarehouseID AND MaterialIssueDetails.GoodsReceiptDetailID = GoodsReceiptDetails.GoodsReceiptDetailID AND MaterialIssueDetails.EntryDate <= @LocalToDate AND ROUND(MaterialIssueDetails.Quantity - MaterialIssueDetails.QuantitySemifinished - MaterialIssueDetails.QuantityFailure - MaterialIssueDetails.QuantityReceipted - MaterialIssueDetails.QuantityLoss, " + (int)GlobalEnums.rndQuantity + ") > 0 INNER JOIN " + "\r\n";
            queryString = queryString + "                               Customers ON MaterialIssueDetails.CustomerID = Customers.CustomerID INNER JOIN " + "\r\n";
            queryString = queryString + "                               FirmOrders ON MaterialIssueDetails.FirmOrderID = FirmOrders.FirmOrderID INNER JOIN " + "\r\n";
            queryString = queryString + "                               Workshifts ON MaterialIssueDetails.WorkshiftID = Workshifts.WorkshiftID INNER JOIN " + "\r\n";
            queryString = queryString + "                               ProductionLines ON MaterialIssueDetails.ProductionLineID = ProductionLines.ProductionLineID " + "\r\n";

            queryString = queryString + "                   UNION ALL " + "\r\n";

            queryString = queryString + "                   SELECT      " + (int)GlobalEnums.WarehouseAdjustmentTypeID.ReceiptPENDING + " AS JournalTypeID, Customers.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, FirmOrders.Specs, FirmOrders.Specification, FirmOrders.Code AS FirmOrderCode, 'Inventories/' + IIF(MaterialIssues.NmvnTaskID = " + (int)GlobalEnums.NmvnTaskID.MaterialStaging + ", 'MaterialStagings', IIF(MaterialIssues.NmvnTaskID = " + (int)GlobalEnums.NmvnTaskID.ItemStaging + ", 'ItemStagings', 'ProductStagings')) AS ControllerName, MaterialIssues.MaterialIssueID AS EntityID, N'TỒN SX [DỞ DANG]' AS GroupName, 'X.' + CONVERT(VARCHAR, @LocalToDate, 103) AS SubGroupName, NULL AS GoodsReceiptDetailID, MaterialIssues.EntryDate, GoodsReceiptDetails.CommodityID, MaterialIssues.Reference, GoodsReceiptDetails.Reference AS Referral, GoodsReceiptDetails.Code AS ReceiptCode, GoodsReceiptDetails.Barcode, GoodsReceiptDetails.WarehouseID, N'MÁY ' + ProductionLines.Code + ' ' + Workshifts.Code + ' [' + CONVERT(VARCHAR, MaterialIssues.EntryDate, 103) + ']' +  ', KH: ' + Customers.Name AS Description, GoodsReceiptDetails.Quantity AS QuantityDebit, 0 AS QuantityCredit " + "\r\n";
            queryString = queryString + "                   FROM        GoodsReceiptDetails INNER JOIN " + "\r\n";
            queryString = queryString + "                               MaterialIssues ON GoodsReceiptDetails.WarehouseID = @LocalWarehouseID AND GoodsReceiptDetails.MaterialIssueID = MaterialIssues.MaterialIssueID AND MaterialIssues.EntryDate <= @LocalToDate AND GoodsReceiptDetails.EntryDate > @LocalToDate INNER JOIN " + "\r\n";
            queryString = queryString + "                               Customers ON MaterialIssues.CustomerID = Customers.CustomerID INNER JOIN " + "\r\n";
            queryString = queryString + "                               FirmOrders ON MaterialIssues.FirmOrderID = FirmOrders.FirmOrderID INNER JOIN " + "\r\n";
            queryString = queryString + "                               Workshifts ON MaterialIssues.WorkshiftID = Workshifts.WorkshiftID INNER JOIN " + "\r\n";
            queryString = queryString + "                               ProductionLines ON MaterialIssues.ProductionLineID = ProductionLines.ProductionLineID " + "\r\n";

            ////////queryString = queryString + "                   UNION ALL " + "\r\n";
            ////////EWHInputVoucherTypeID.EWHTransfer
            //////queryString = queryString + "                   SELECT      N'TỒN SX [DỞ DANG]' AS GroupName, 'CHO NHAP KHO' AS SubGroupName, NULL AS GoodsReceiptDetailID, StockTransfers.EntryDate, StockTransferDetails.CommodityID, '' AS Reference, GoodsReceiptDetails.Reference AS Referral, '' AS ReceiptCode, NULL AS Barcode, 0 AS WarehouseID, 'NHAP VCNB: ' + Locations.Name + ', PX [' + StockTransfers.Reference + ' Ngày: ' + CONVERT(VARCHAR, StockTransfers.EntryDate, 103) + ']' AS Description, StockTransferDetails.Quantity - StockTransferDetails.QuantityReceipt AS QuantityDebit, 0 AS QuantityCredit " + "\r\n";
            //////queryString = queryString + "                   FROM        StockTransfers INNER JOIN " + "\r\n";
            //////queryString = queryString + "                               StockTransferDetails ON StockTransfers.StockTransferID = StockTransferDetails.StockTransferID AND StockTransfers.WarehouseID = @LocalWarehouseID AND StockTransferDetails.EntryDate <= @LocalToDate AND StockTransferDetails.Quantity > StockTransferDetails.QuantityReceipt INNER JOIN " + "\r\n";
            //////queryString = queryString + "                               Locations ON StockTransfers.LocationID = Locations.LocationID " + "\r\n";

            //////queryString = queryString + "                   UNION ALL " + "\r\n";

            //////queryString = queryString + "                   SELECT      N'TỒN SX [DỞ DANG]' AS GroupName, 'CHO NHAP KHO' AS SubGroupName, NULL AS GoodsReceiptDetailID, StockTransfers.EntryDate, GoodsReceiptDetails.CommodityID, '' AS Reference, GoodsReceiptDetails.Reference AS Referral, '' AS ReceiptCode, NULL AS Barcode, 0 AS WarehouseID, 'NHAP VCNB: ' + Locations.Name + ', PX [' + StockTransfers.Reference + ' Ngày: ' + CONVERT(VARCHAR, StockTransfers.EntryDate, 103) + ']' AS Description, GoodsReceiptDetails.Quantity AS QuantityDebit, 0 AS QuantityCredit " + "\r\n";
            //////queryString = queryString + "                   FROM        StockTransfers INNER JOIN " + "\r\n";
            //////queryString = queryString + "                               GoodsReceiptDetails ON StockTransfers.WarehouseID = @LocalWarehouseID AND StockTransfers.StockTransferID = GoodsReceiptDetails.VoucherID AND GoodsReceiptDetails.GoodsReceiptTypeID = " + (int)GlobalEnums.GoodsReceiptTypeID.StockTransfer + " AND StockTransfers.EntryDate <= @LocalToDate AND GoodsReceiptDetails.EntryDate > @LocalToDate INNER JOIN " + "\r\n";
            //////queryString = queryString + "                               Locations ON StockTransfers.LocationID = Locations.LocationID " + "\r\n";
            ////////--ON INPUT.END





            //C.1.ON ISSUE.ON DeliveryAdvices
            queryString = queryString + "                   UNION ALL " + "\r\n";

            queryString = queryString + "                   SELECT      " + (int)GlobalEnums.WarehouseAdjustmentTypeID.IssuePENDING + " AS JournalTypeID, Customers.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, NULL AS Specs, NULL AS Specification, NULL AS FirmOrderCode, 'Sales/DeliveryAdvices' AS ControllerName, DeliveryAdviceDetails.DeliveryAdviceID AS EntityID, N'TỒN SX [DỞ DANG]' AS GroupName, 'X.' + CONVERT(VARCHAR, @LocalToDate, 103) AS SubGroupName, NULL AS GoodsReceiptDetailID, DeliveryAdviceDetails.EntryDate, DeliveryAdviceDetails.CommodityID, NULL AS Reference, NULL AS Referral, NULL AS ReceiptCode, NULL AS Barcode, DeliveryAdviceDetails.WarehouseID, Customers.Name AS Description, 0 AS QuantityDebit, DeliveryAdviceDetails.Quantity - DeliveryAdviceDetails.QuantityIssue AS QuantityCredit " + "\r\n";
            queryString = queryString + "                   FROM        DeliveryAdviceDetails INNER JOIN " + "\r\n";
            queryString = queryString + "                               Customers ON DeliveryAdviceDetails.LocationID = @LocationID AND DeliveryAdviceDetails.WarehouseID = @LocalWarehouseID AND DeliveryAdviceDetails.EntryDate <= @LocalToDate AND DeliveryAdviceDetails.InActive = 0 AND DeliveryAdviceDetails.InActivePartial = 0 AND DeliveryAdviceDetails.InActiveIssue = 0 AND ROUND(DeliveryAdviceDetails.Quantity - DeliveryAdviceDetails.QuantityIssue, " + (int)GlobalEnums.rndQuantity + ") > 0 AND DeliveryAdviceDetails.CustomerID = Customers.CustomerID " + "\r\n";

            queryString = queryString + "                   UNION ALL " + "\r\n";

            queryString = queryString + "                   SELECT      " + (int)GlobalEnums.WarehouseAdjustmentTypeID.IssuePENDING + " AS JournalTypeID, Customers.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, NULL AS Specs, NULL AS Specification, NULL AS FirmOrderCode, 'Sales/DeliveryAdvices' AS ControllerName, DeliveryAdvices.DeliveryAdviceID AS EntityID, N'TỒN SX [DỞ DANG]' AS GroupName, 'X.' + CONVERT(VARCHAR, @LocalToDate, 103) AS SubGroupName, NULL AS GoodsReceiptDetailID, DeliveryAdvices.EntryDate, GoodsIssueDetails.CommodityID, NULL AS Reference, NULL AS Referral, NULL AS ReceiptCode, NULL AS Barcode, GoodsIssueDetails.WarehouseID, Customers.Name AS Description, 0 AS QuantityDebit, GoodsIssueDetails.Quantity AS QuantityCredit " + "\r\n";
            queryString = queryString + "                   FROM        DeliveryAdvices INNER JOIN " + "\r\n";
            queryString = queryString + "                               GoodsIssueDetails ON DeliveryAdvices.LocationID = @LocationID AND GoodsIssueDetails.WarehouseID = @LocalWarehouseID AND DeliveryAdvices.DeliveryAdviceID = GoodsIssueDetails.DeliveryAdviceID AND DeliveryAdvices.EntryDate <= @LocalToDate AND GoodsIssueDetails.EntryDate > @LocalToDate INNER JOIN " + "\r\n";
            queryString = queryString + "                               Customers ON DeliveryAdvices.CustomerID = Customers.CustomerID " + "\r\n";

            //C.2.ON ISSUE.ON TransferOrders


            queryString = queryString + "                   ) AS WarehouseJournalMaster INNER JOIN " + "\r\n";

            queryString = queryString + "                   Commodities ON WarehouseJournalMaster.CommodityID = Commodities.CommodityID INNER JOIN " + "\r\n";
            queryString = queryString + "                   EntireCommodityCategories ON Commodities.CommodityCategoryID = EntireCommodityCategories.CommodityCategoryID LEFT JOIN " + "\r\n";

            queryString = queryString + "                   Warehouses ON WarehouseJournalMaster.WarehouseID = Warehouses.WarehouseID " + "\r\n";

            queryString = queryString + "    END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("WarehouseCards", queryString);

        }



        public string GET_WarehouseCard_BUILD_SQL(string commoditiesBalanceTable, string warehouseID, string fromDate, string toDate)
        {
            string queryString = "";

            if (commoditiesBalanceTable != null && commoditiesBalanceTable != "" && commoditiesBalanceTable != "@CommoditiesAvailable")
                queryString = queryString + "       DECLARE " + commoditiesBalanceTable + " TABLE (WarehouseID int NULL, CommodityID int NULL, QuantityBalance decimal(18, 2) NULL, Bookable bit NULL) " + "\r\n";

            queryString = queryString + "       DECLARE @WarehouseCardTable TABLE " + "\r\n";
            queryString = queryString + "      (WarehouseCardID int NULL, JournalTypeID int NULL, CustomerID int NULL, CustomerCode nvarchar(50) NULL, CustomerName nvarchar(100) NULL, Specs nvarchar(800) NULL, Specification nvarchar(800) NULL, FirmOrderCode nvarchar(50) NULL, ControllerName nvarchar(50) NULL, EntityID int NULL, GroupName nvarchar(50) NULL, SubGroupName nvarchar(50) NULL, GoodsReceiptDetailID	int NULL, EntryDate	datetime NULL, " + "\r\n";
            queryString = queryString + "       CommodityID int NULL, Code nvarchar(50) NULL, CodePartA nvarchar(20) NULL, CodePartB nvarchar(20) NULL, CodePartC nvarchar(20) NULL, CodePartD nvarchar(20) NULL, CodePartE nvarchar(20) NULL, CodePartF nvarchar(20) NULL, Name nvarchar(200) NULL, SalesUnit nvarchar(10) NULL, LeadTime int NULL, Reference nvarchar(10) NULL, Referral nvarchar(10) NULL, ReceiptCode nvarchar(50) NULL, Barcode nvarchar(60) NULL, LocationID int NULL, WarehouseCategoryID int NULL, WarehouseID int NULL, WarehouseName nvarchar(60) NULL, Bookable bit NULL, Description nvarchar(100) NULL, " + "\r\n";
            queryString = queryString + "       QuantityDebit decimal(18, 2) NULL, QuantityCredit decimal(18, 2) NULL, QuantityBEGIN decimal(18, 2) NULL, QuantityRECEIPT decimal(18, 2) NULL, QuantityISSUE decimal(18, 2) NULL, QuantityEND decimal(18, 2) NULL, ReceiptPRO decimal(18, 2) NULL, ReceiptSAM decimal(18, 2) NULL, ReceiptRTN decimal(18, 2) NULL, ReceiptADJ decimal(18, 2) NULL, ReceiptPUR decimal(18, 2) NULL, ReceiptREG decimal(18, 2) NULL, ReceiptOSS decimal(18, 2) NULL, ReceiptBOR decimal(18, 2) NULL, ReceiptFOI decimal(18, 2) NULL, ReceiptTXF decimal(18, 2) NULL, ReceiptOTH decimal(18, 2) NULL, " + "\r\n";
            queryString = queryString + "       IssueINV decimal(18, 2) NULL, IssueOFS decimal(18, 2) NULL, IssueADJ decimal(18, 2) NULL, IssueSAM decimal(18, 2) NULL, IssueDST decimal(18, 2) NULL, IssuePRO decimal(18, 2) NULL, IssueREG decimal(18, 2) NULL, IssueRTN decimal(18, 2) NULL, IssueOSS decimal(18, 2) NULL, IssueTXF decimal(18, 2) NULL, IssueOTH decimal(18, 2) NULL, ReceiptPENDING decimal(18, 2) NULL, IssuePENDING decimal(18, 2) NULL, CommodityCategoryID int NULL, CommodityCategory1 nvarchar(100) NULL, CommodityCategory2 nvarchar(100) NULL, CommodityCategory3 nvarchar(100) NULL) " + "\r\n";

            queryString = queryString + "       INSERT INTO @WarehouseCardTable EXEC WarehouseCards " + warehouseID + ", " + fromDate + ", " + toDate + "\r\n";

            if (commoditiesBalanceTable != null && commoditiesBalanceTable != "")
                queryString = queryString + "   INSERT INTO " + commoditiesBalanceTable + " SELECT WarehouseID, CommodityID, SUM(QuantityEND + ReceiptPENDING - IssuePENDING) AS QuantityAvailable, CAST(MAX(IIF(Bookable = 1, 1, 0)) AS bit) AS Bookable FROM @WarehouseCardTable GROUP BY WarehouseID, CommodityID HAVING SUM(QuantityEND + ReceiptPENDING - IssuePENDING) > 0" + "\r\n";

            return queryString;
        }

    }
}
