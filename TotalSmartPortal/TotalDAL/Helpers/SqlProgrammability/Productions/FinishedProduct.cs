﻿using TotalBase;
using TotalBase.Enums;
using TotalModel.Models;

namespace TotalDAL.Helpers.SqlProgrammability.Productions
{
    public class FinishedProduct
    {
        private readonly TotalSmartPortalEntities totalSmartPortalEntities;

        public FinishedProduct(TotalSmartPortalEntities totalSmartPortalEntities)
        {
            this.totalSmartPortalEntities = totalSmartPortalEntities;
        }

        public void RestoreProcedure()
        {
            this.GetFinishedProductIndexes();

            this.GetFinishedProductPendingFirmOrders();

            this.GetFinishedProductViewDetails();

            this.FinishedProductSaveRelative();
            this.FinishedProductPostSaveValidate();

            this.FinishedProductApproved();
            this.FinishedProductEditable();

            this.FinishedProductToggleApproved();

            this.FinishedProductInitReference();

            this.FinishedProductSheet();
        }


        private void GetFinishedProductIndexes()
        {
            string queryString;

            queryString = " @AspUserID nvarchar(128), @FromDate DateTime, @ToDate DateTime " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";
            queryString = queryString + "    BEGIN " + "\r\n";

            queryString = queryString + "       SELECT      FinishedProducts.FinishedProductID, CAST(FinishedProducts.EntryDate AS DATE) AS EntryDate, FinishedProducts.Reference, Locations.Code AS LocationCode, Customers.Code AS CustomerCode, Customers.Name AS CustomerName, FirmOrders.Reference AS FirmOrderReference, FirmOrders.Code AS FirmOrderCode, FirmOrders.Specs AS FirmOrderSpecs, FinishedProducts.Description, FinishedProducts.WorkshiftID, Workshifts.Name AS WorkshiftName, Workshifts.EntryDate AS WorkshiftEntryDate, FinishedProducts.TotalQuantity + FinishedProducts.TotalQuantityExcess AS TotalQuantity, FinishedProducts.TotalQuantityFailure, FinishedProducts.TotalQuantityExcess, FinishedProducts.TotalQuantityShortage, FinishedProducts.TotalSwarfs, FinishedProducts.Approved " + "\r\n";
            queryString = queryString + "       FROM        FinishedProducts " + "\r\n";
            queryString = queryString + "                   INNER JOIN Locations ON FinishedProducts.EntryDate >= @FromDate AND FinishedProducts.EntryDate <= @ToDate AND FinishedProducts.OrganizationalUnitID IN (SELECT AccessControls.OrganizationalUnitID FROM AccessControls INNER JOIN AspNetUsers ON AccessControls.UserID = AspNetUsers.UserID WHERE AspNetUsers.Id = @AspUserID AND AccessControls.NMVNTaskID = " + (int)TotalBase.Enums.GlobalEnums.NmvnTaskID.FinishedProduct + " AND AccessControls.AccessLevel > 0) AND Locations.LocationID = FinishedProducts.LocationID " + "\r\n";
            queryString = queryString + "                   INNER JOIN FirmOrders ON FinishedProducts.FirmOrderID = FirmOrders.FirmOrderID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Customers ON FinishedProducts.CustomerID = Customers.CustomerID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Workshifts ON FinishedProducts.WorkshiftID = Workshifts.WorkshiftID " + "\r\n";
            queryString = queryString + "       " + "\r\n";

            queryString = queryString + "    END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetFinishedProductIndexes", queryString);
        }

        #region X


        private void GetFinishedProductViewDetails()
        {
            string queryString;

            queryString = " @FinishedProductID Int, @LocationID Int, @FirmOrderID Int, @IsReadonly bit " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";
            queryString = queryString + "   BEGIN " + "\r\n";

            queryString = queryString + "       IF (@FinishedProductID <= 0) " + "\r\n";
            queryString = queryString + "               BEGIN " + "\r\n";
            queryString = queryString + "                   " + this.BUILDSQLNew() + "\r\n";
            queryString = queryString + "                   ORDER BY CommodityCode, SemifinishedProductEntryDate, SemifinishedProductDetails.SemifinishedProductID, SemifinishedProductDetails.SemifinishedProductDetailID " + "\r\n";
            queryString = queryString + "               END " + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";

            queryString = queryString + "               IF (@IsReadonly = 1) " + "\r\n";
            queryString = queryString + "                   BEGIN " + "\r\n";
            queryString = queryString + "                       " + this.BUILDSQLEdit() + "\r\n";
            queryString = queryString + "                       ORDER BY CommodityCode, SemifinishedProductEntryDate, SemifinishedProductDetails.SemifinishedProductID, SemifinishedProductDetails.SemifinishedProductDetailID " + "\r\n";
            queryString = queryString + "                   END " + "\r\n";

            queryString = queryString + "               ELSE " + "\r\n"; //FULL SELECT FOR EDIT MODE

            queryString = queryString + "                   BEGIN " + "\r\n";
            queryString = queryString + "                       " + this.BUILDSQLNew() + " AND SemifinishedProductDetails.SemifinishedProductDetailID NOT IN (SELECT SemifinishedProductDetailID FROM FinishedProductDetails WHERE FinishedProductID = @FinishedProductID) " + "\r\n";
            queryString = queryString + "                       UNION ALL " + "\r\n";
            queryString = queryString + "                       " + this.BUILDSQLEdit() + "\r\n";
            queryString = queryString + "                       ORDER BY CommodityCode, SemifinishedProductEntryDate, SemifinishedProductDetails.SemifinishedProductID, SemifinishedProductDetails.SemifinishedProductDetailID " + "\r\n";
            queryString = queryString + "                   END " + "\r\n";

            queryString = queryString + "   END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetFinishedProductViewDetails", queryString);
        }

        private string BUILDSQLNew()
        {
            string queryString = "";

            queryString = queryString + "       SELECT      0 AS FinishedProductDetailID, 0 AS FinishedProductID, SemifinishedProductDetails.FirmOrderID, SemifinishedProductDetails.FirmOrderDetailID, SemifinishedProductDetails.PlannedOrderID, SemifinishedProductDetails.PlannedOrderDetailID, SemifinishedProductDetails.SemifinishedProductID, SemifinishedProductDetails.SemifinishedProductDetailID, SemifinishedProductDetails.SemifinishedHandoverID, SemifinishedProductDetails.EntryDate AS SemifinishedProductEntryDate, SemifinishedProducts.Reference AS SemifinishedProductReference, Commodities.CommodityID, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, Commodities.CommodityTypeID, SemifinishedProductDetails.PiecePerPack, 0.0 AS PackageUnitWeights, N'' AS Remarks, " + "\r\n";
            queryString = queryString + "                   SemifinishedProductDetails.WorkshiftID, Workshifts.EntryDate AS WorkshiftEntryDate, Workshifts.Code AS WorkshiftCode, GoodsReceiptDetails.Reference AS GoodsReceiptReference, GoodsReceiptDetails.Code AS GoodsReceiptCode, GoodsReceiptDetails.BatchEntryDate, ROUND(SemifinishedProductDetails.Quantity - SemifinishedProductDetails.QuantityFinished, " + (int)GlobalEnums.rndQuantity + ") AS QuantityRemains, 0.0 AS Quantity, 0.0 AS QuantityFailure, 0.0 AS QuantityExcess, 0.0 AS QuantityShortage, 0.0 AS Swarfs " + "\r\n";

            queryString = queryString + "       FROM        SemifinishedProductDetails " + "\r\n"; //AND SemifinishedProductDetails.LocationID = @LocationID 
            queryString = queryString + "                   INNER JOIN Commodities ON SemifinishedProductDetails.FirmOrderID = @FirmOrderID AND SemifinishedProductDetails.Approved = 1 AND SemifinishedProductDetails.HandoverApproved = 1 AND ROUND(SemifinishedProductDetails.Quantity - SemifinishedProductDetails.QuantityFinished, " + (int)GlobalEnums.rndQuantity + ") > 0 AND SemifinishedProductDetails.CommodityID = Commodities.CommodityID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Workshifts ON SemifinishedProductDetails.WorkshiftID = Workshifts.WorkshiftID " + "\r\n";
            queryString = queryString + "                   INNER JOIN GoodsReceiptDetails ON SemifinishedProductDetails.GoodsReceiptDetailID = GoodsReceiptDetails.GoodsReceiptDetailID " + "\r\n";
            queryString = queryString + "                   INNER JOIN SemifinishedProducts ON SemifinishedProductDetails.SemifinishedProductID = SemifinishedProducts.SemifinishedProductID " + "\r\n";

            return queryString;
        }

        private string BUILDSQLEdit()
        {
            string queryString = "";

            queryString = queryString + "       SELECT      FinishedProductDetails.FinishedProductDetailID, FinishedProductDetails.FinishedProductID, SemifinishedProductDetails.FirmOrderID, SemifinishedProductDetails.FirmOrderDetailID, SemifinishedProductDetails.PlannedOrderID, SemifinishedProductDetails.PlannedOrderDetailID, SemifinishedProductDetails.SemifinishedProductID, SemifinishedProductDetails.SemifinishedProductDetailID, SemifinishedProductDetails.SemifinishedHandoverID, SemifinishedProductDetails.EntryDate AS SemifinishedProductEntryDate, SemifinishedProducts.Reference AS SemifinishedProductReference, Commodities.CommodityID, Commodities.Code AS CommodityCode, Commodities.Name AS CommodityName, Commodities.CommodityTypeID, FinishedProductDetails.PiecePerPack, FinishedProductDetails.PackageUnitWeights, FinishedProductDetails.Remarks, " + "\r\n";
            queryString = queryString + "                   SemifinishedProductDetails.WorkshiftID, Workshifts.EntryDate AS WorkshiftEntryDate, Workshifts.Code AS WorkshiftCode, GoodsReceiptDetails.Reference AS GoodsReceiptReference, GoodsReceiptDetails.Code AS GoodsReceiptCode, GoodsReceiptDetails.BatchEntryDate, ROUND(SemifinishedProductDetails.Quantity - SemifinishedProductDetails.QuantityFinished + FinishedProductDetails.Quantity + FinishedProductDetails.QuantityFailure + FinishedProductDetails.QuantityShortage, " + (int)GlobalEnums.rndQuantity + ") AS QuantityRemains, FinishedProductDetails.Quantity, FinishedProductDetails.QuantityFailure, FinishedProductDetails.QuantityExcess, FinishedProductDetails.QuantityShortage, FinishedProductDetails.Swarfs " + "\r\n";

            queryString = queryString + "       FROM        FinishedProductDetails " + "\r\n";
            queryString = queryString + "                   INNER JOIN SemifinishedProductDetails ON FinishedProductDetails.FinishedProductID = @FinishedProductID AND FinishedProductDetails.SemifinishedProductDetailID = SemifinishedProductDetails.SemifinishedProductDetailID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Commodities ON SemifinishedProductDetails.CommodityID = Commodities.CommodityID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Workshifts ON SemifinishedProductDetails.WorkshiftID = Workshifts.WorkshiftID " + "\r\n";
            queryString = queryString + "                   INNER JOIN GoodsReceiptDetails ON SemifinishedProductDetails.GoodsReceiptDetailID = GoodsReceiptDetails.GoodsReceiptDetailID " + "\r\n";
            queryString = queryString + "                   INNER JOIN SemifinishedProducts ON SemifinishedProductDetails.SemifinishedProductID = SemifinishedProducts.SemifinishedProductID " + "\r\n";

            return queryString;
        }
       


        private void GetFinishedProductPendingFirmOrders()
        {
            string queryString = " @LocationID int " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "       SELECT          FirmOrders.FirmOrderID, FirmOrders.EntryDate AS FirmOrderEntryDate, FirmOrders.Reference AS FirmOrderReference, FirmOrders.Code AS FirmOrderCode, FirmOrders.Specification AS FirmOrderSpecification, FirmOrders.CustomerID, Customers.Code AS CustomerCode, Customers.Name AS CustomerName " + "\r\n";
            queryString = queryString + "       FROM            FirmOrders " + "\r\n";//LocationID = @LocationID AND 
            queryString = queryString + "                       INNER JOIN Customers ON FirmOrders.FirmOrderID IN (SELECT DISTINCT FirmOrderID FROM SemifinishedProductDetails WHERE Approved = 1 AND HandoverApproved = 1 AND ROUND(Quantity - QuantityFinished, " + (int)GlobalEnums.rndQuantity + ") > 0) AND FirmOrders.CustomerID = Customers.CustomerID " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("GetFinishedProductPendingFirmOrders", queryString);
        }

        private void FinishedProductSaveRelative()
        {
            string queryString = " @EntityID int, @SaveRelativeOption int " + "\r\n"; //SaveRelativeOption: 1: Update, -1:Undo
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";
            queryString = queryString + "   BEGIN  " + "\r\n";

            queryString = queryString + "       DECLARE @msg NVARCHAR(300) ";

            queryString = queryString + "       IF (@SaveRelativeOption = 1) " + "\r\n";
            queryString = queryString + "           BEGIN " + "\r\n";
            #region UPDATE WorkshiftID
            queryString = queryString + "               DECLARE         @EntryDate Datetime, @Reference nvarchar(10), @ShiftID int, @WorkshiftID int " + "\r\n";
            queryString = queryString + "               SELECT          @EntryDate = CONVERT(date, EntryDate), @Reference = Reference, @ShiftID = ShiftID FROM FinishedProducts WHERE FinishedProductID = @EntityID " + "\r\n";
            queryString = queryString + "               SET             @WorkshiftID = (SELECT TOP 1 WorkshiftID FROM Workshifts WHERE EntryDate = @EntryDate AND ShiftID = @ShiftID) " + "\r\n";

            queryString = queryString + "               IF             (@WorkshiftID IS NULL) " + "\r\n";
            queryString = queryString + "                   BEGIN ";
            queryString = queryString + "                       INSERT INTO     Workshifts(EntryDate, ShiftID, Code, Name, WorkingHours, Remarks) SELECT @EntryDate, ShiftID, Code, Name, WorkingHours, Remarks FROM Shifts WHERE ShiftID = @ShiftID " + "\r\n";
            queryString = queryString + "                       SELECT          @WorkshiftID = SCOPE_IDENTITY(); " + "\r\n";
            queryString = queryString + "                   END ";

            queryString = queryString + "               UPDATE          FinishedProducts        SET WorkshiftID = @WorkshiftID WHERE FinishedProductID = @EntityID " + "\r\n";
            queryString = queryString + "               UPDATE          FinishedProductDetails  SET WorkshiftID = @WorkshiftID, Reference = @Reference WHERE FinishedProductID = @EntityID " + "\r\n";
            #endregion UPDATE WorkshiftID
            queryString = queryString + "           END " + "\r\n";


            queryString = queryString + "       UPDATE          SemifinishedProductDetails " + "\r\n";
            queryString = queryString + "       SET             SemifinishedProductDetails.QuantityFinished = ROUND(SemifinishedProductDetails.QuantityFinished + (FinishedProductDetails.Quantity + FinishedProductDetails.QuantityFailure + FinishedProductDetails.QuantityShortage) * @SaveRelativeOption, " + (int)GlobalEnums.rndQuantity + ") " + "\r\n";
            queryString = queryString + "       FROM            FinishedProductDetails " + "\r\n";
            queryString = queryString + "                       INNER JOIN SemifinishedProductDetails ON SemifinishedProductDetails.Approved = 1 AND SemifinishedProductDetails.HandoverApproved = 1 AND FinishedProductDetails.FinishedProductID = @EntityID AND FinishedProductDetails.SemifinishedProductDetailID = SemifinishedProductDetails.SemifinishedProductDetailID " + "\r\n";

            queryString = queryString + "       IF @@ROWCOUNT = (SELECT COUNT(*) FROM FinishedProductDetails WHERE FinishedProductID = @EntityID) " + "\r\n";
            queryString = queryString + "           BEGIN " + "\r\n";
            queryString = queryString + "               UPDATE  FirmOrderDetails " + "\r\n";
            queryString = queryString + "               SET     FirmOrderDetails.QuantityFinished = ROUND(FirmOrderDetails.QuantityFinished + (FinishedProductSummaries.Quantity + FinishedProductSummaries.QuantityFailure + FinishedProductSummaries.QuantityShortage) * @SaveRelativeOption, " + (int)GlobalEnums.rndQuantity + "), " + "\r\n";
            queryString = queryString + "                       FirmOrderDetails.QuantityFailure = ROUND(FirmOrderDetails.QuantityFailure + FinishedProductSummaries.QuantityFailure * @SaveRelativeOption, " + (int)GlobalEnums.rndQuantity + "), " + "\r\n";
            queryString = queryString + "                       FirmOrderDetails.QuantityExcess = ROUND(FirmOrderDetails.QuantityExcess + FinishedProductSummaries.QuantityExcess * @SaveRelativeOption, " + (int)GlobalEnums.rndQuantity + "), " + "\r\n";
            queryString = queryString + "                       FirmOrderDetails.QuantityShortage = ROUND(FirmOrderDetails.QuantityShortage + FinishedProductSummaries.QuantityShortage * @SaveRelativeOption, " + (int)GlobalEnums.rndQuantity + "), " + "\r\n";
            queryString = queryString + "                       FirmOrderDetails.Swarfs = ROUND(FirmOrderDetails.Swarfs + FinishedProductSummaries.Swarfs * @SaveRelativeOption, " + (int)GlobalEnums.rndQuantity + ") " + "\r\n";
            queryString = queryString + "               FROM    FirmOrderDetails INNER JOIN (SELECT FirmOrderDetailID, SUM(Quantity) AS Quantity, SUM(QuantityFailure) AS QuantityFailure, SUM(QuantityExcess) AS QuantityExcess, SUM(QuantityShortage) AS QuantityShortage, SUM(Swarfs) AS Swarfs FROM FinishedProductDetails WHERE FinishedProductID = @EntityID GROUP BY FirmOrderDetailID) AS FinishedProductSummaries ON FirmOrderDetails.FirmOrderDetailID = FinishedProductSummaries.FirmOrderDetailID ; " + "\r\n";

            queryString = queryString + "               UPDATE  FirmOrders " + "\r\n";
            queryString = queryString + "               SET     FirmOrders.TotalQuantityFinished = ROUND(FirmOrders.TotalQuantityFinished + (FinishedProductSummaries.TotalQuantity + FinishedProductSummaries.TotalQuantityFailure + FinishedProductSummaries.TotalQuantityShortage) * @SaveRelativeOption, " + (int)GlobalEnums.rndQuantity + "), " + "\r\n";
            queryString = queryString + "                       FirmOrders.TotalQuantityFailure = ROUND(FirmOrders.TotalQuantityFailure + FinishedProductSummaries.TotalQuantityFailure * @SaveRelativeOption, " + (int)GlobalEnums.rndQuantity + "), " + "\r\n";
            queryString = queryString + "                       FirmOrders.TotalQuantityExcess = ROUND(FirmOrders.TotalQuantityExcess + FinishedProductSummaries.TotalQuantityExcess * @SaveRelativeOption, " + (int)GlobalEnums.rndQuantity + "), " + "\r\n";
            queryString = queryString + "                       FirmOrders.TotalQuantityShortage = ROUND(FirmOrders.TotalQuantityShortage + FinishedProductSummaries.TotalQuantityShortage * @SaveRelativeOption, " + (int)GlobalEnums.rndQuantity + "), " + "\r\n";
            queryString = queryString + "                       FirmOrders.TotalSwarfs = ROUND(FirmOrders.TotalSwarfs + FinishedProductSummaries.TotalSwarfs * @SaveRelativeOption, " + (int)GlobalEnums.rndQuantity + ") " + "\r\n";
            queryString = queryString + "               FROM    FirmOrders INNER JOIN (SELECT FirmOrderID, SUM(Quantity) AS TotalQuantity, SUM(QuantityFailure) AS TotalQuantityFailure, SUM(QuantityExcess) AS TotalQuantityExcess, SUM(QuantityShortage) AS TotalQuantityShortage, SUM(Swarfs) AS TotalSwarfs FROM FinishedProductDetails WHERE FinishedProductID = @EntityID GROUP BY FirmOrderID) AS FinishedProductSummaries ON FirmOrders.FirmOrderID = FinishedProductSummaries.FirmOrderID ; " + "\r\n";
            queryString = queryString + "           END " + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           BEGIN " + "\r\n";
            queryString = queryString + "               SET         @msg = N'Phiếu BTP không tồn tại, chưa duyệt hoặc đã hủy' ; " + "\r\n";
            queryString = queryString + "               THROW       61001,  @msg, 1; " + "\r\n";
            queryString = queryString + "           END " + "\r\n";




            queryString = queryString + "       IF ((SELECT Approved FROM FinishedProducts WHERE FinishedProductID = @EntityID AND Approved = 1) = 1) " + "\r\n";
            queryString = queryString + "           BEGIN " + "\r\n";
            queryString = queryString + "               UPDATE      FinishedProducts  SET Approved = 0 WHERE FinishedProductID = @EntityID AND Approved = 1" + "\r\n"; //CLEAR APPROVE BEFORE CALL FinishedProductToggleApproved
            queryString = queryString + "               IF @@ROWCOUNT = 1 " + "\r\n";
            queryString = queryString + "                   EXEC        FinishedProductToggleApproved @EntityID, 1 " + "\r\n";
            queryString = queryString + "               ELSE " + "\r\n";
            queryString = queryString + "                   BEGIN " + "\r\n";
            queryString = queryString + "                       SET         @msg = N'Dữ liệu không tồn tại hoặc đã duyệt'; " + "\r\n";
            queryString = queryString + "                       THROW       61001,  @msg, 1; " + "\r\n";
            queryString = queryString + "                   END " + "\r\n";
            queryString = queryString + "           END " + "\r\n";

            queryString = queryString + "   END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("FinishedProductSaveRelative", queryString);
        }

        private void FinishedProductPostSaveValidate()
        {
            string[] queryArray = new string[3];

            queryArray[0] = " SELECT TOP 1 @FoundEntity = N'Vui lòng nhập trọng lượng kiện' FROM FinishedProductDetails WHERE FinishedProductID = @EntityID AND ((Quantity <> 0 OR QuantityFailure <> 0 OR QuantityExcess <> 0 OR QuantityShortage <> 0) AND PackageUnitWeights = 0) ";

            queryArray[1] = " SELECT TOP 1 @FoundEntity = N'Ngày sản xuất phôi: ' + CAST(SemifinishedProducts.EntryDate AS nvarchar) FROM FinishedProductDetails INNER JOIN SemifinishedProducts ON FinishedProductDetails.FinishedProductID = @EntityID AND FinishedProductDetails.SemifinishedProductID = SemifinishedProducts.SemifinishedProductID AND FinishedProductDetails.EntryDate < SemifinishedProducts.EntryDate ";
            queryArray[2] = " SELECT TOP 1 @FoundEntity = N'Số lượng đóng gói vượt quá số lượng phôi: ' + CAST(ROUND(Quantity - QuantityFinished, " + (int)GlobalEnums.rndQuantity + ") AS nvarchar) FROM SemifinishedProductDetails WHERE (ROUND(Quantity - QuantityFinished, " + (int)GlobalEnums.rndQuantity + ") < 0) ";

            this.totalSmartPortalEntities.CreateProcedureToCheckExisting("FinishedProductPostSaveValidate", queryArray);
        }




        private void FinishedProductApproved()
        {
            string[] queryArray = new string[1];

            queryArray[0] = " SELECT TOP 1 @FoundEntity = FinishedProductID FROM FinishedProducts WHERE FinishedProductID = @EntityID AND Approved = 1";

            this.totalSmartPortalEntities.CreateProcedureToCheckExisting("FinishedProductApproved", queryArray);
        }


        private void FinishedProductEditable()
        {
            string[] queryArray = new string[3];

            queryArray[0] = " SELECT TOP 1 @FoundEntity = FinishedProductID FROM FinishedProductDetails WHERE FinishedProductID = @EntityID AND NOT FinishedHandoverID IS NULL ";
            queryArray[1] = " SELECT TOP 1 @FoundEntity = FinishedProductID FROM FinishedHandoverDetails WHERE FinishedProductID = @EntityID ";
            queryArray[2] = " SELECT TOP 1 @FoundEntity = FinishedProductPackages.FinishedProductID FROM RecyclateDetails INNER JOIN FinishedProductPackages ON RecyclateDetails.FinishedProductPackageID = FinishedProductPackages.FinishedProductPackageID WHERE FinishedProductPackages.FinishedProductID = @EntityID ";

            this.totalSmartPortalEntities.CreateProcedureToCheckExisting("FinishedProductEditable", queryArray);
        }

        private void FinishedProductToggleApproved()
        {
            string queryString = " @EntityID int, @Approved bit " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";

            queryString = queryString + "       UPDATE      FinishedProducts  SET Approved = @Approved, ApprovedDate = GetDate() WHERE FinishedProductID = @EntityID AND Approved = ~@Approved" + "\r\n";

            queryString = queryString + "       IF @@ROWCOUNT = 1 " + "\r\n";
            queryString = queryString + "           BEGIN " + "\r\n";
            queryString = queryString + "               UPDATE          FinishedProductDetails  SET Approved = @Approved WHERE FinishedProductID = @EntityID ; " + "\r\n";


            #region INIT FinishedProductPackages
            queryString = queryString + "               IF (@Approved = 1) " + "\r\n";
            queryString = queryString + "                   BEGIN " + "\r\n";
            queryString = queryString + "                       INSERT INTO     FinishedProductPackages (FinishedProductID, EntryDate, Reference, LocationID, ShiftID, WorkshiftID, CustomerID, FirmOrderID, PlannedOrderID, CommodityID, CommodityTypeID, BatchID, BatchEntryDate, PiecePerPack, PackageUnitWeights, Quantity, QuantityFailure, QuantityExcess, QuantityShortage, Swarfs, QuantityReceipted, Packages, OddPackages, QuantityWeights, QuantityFailureWeights, QuantityExcessWeights, QuantityShortageWeights, RecycleWeights, RecycleLoss, Remarks, Approved, HandoverApproved, SemifinishedProductReferences) " + "\r\n";
            queryString = queryString + "                       SELECT          MIN(FinishedProductID) AS FinishedProductID, MIN(EntryDate) AS EntryDate, MIN(Reference) AS Reference, MIN(LocationID) AS LocationID, MIN(ShiftID) AS ShiftID, MIN(WorkshiftID) AS WorkshiftID, MIN(CustomerID) AS CustomerID, MIN(FirmOrderID) AS FirmOrderID, MIN(PlannedOrderID) AS PlannedOrderID, CommodityID, MIN(CommodityTypeID) AS CommodityTypeID, 0 AS BatchID, MIN(EntryDate) AS BatchEntryDate, MIN(PiecePerPack) AS PiecePerPack, MIN(PackageUnitWeights) AS PackageUnitWeights, ROUND(SUM(Quantity + QuantityExcess), " + (int)GlobalEnums.rndQuantity + ") AS Quantity, ROUND(SUM(QuantityFailure), " + (int)GlobalEnums.rndQuantity + ") AS QuantityFailure, ROUND(SUM(QuantityExcess), " + (int)GlobalEnums.rndQuantity + ") AS QuantityExcess, ROUND(SUM(QuantityShortage), " + (int)GlobalEnums.rndQuantity + ") AS QuantityShortage, ROUND(SUM(Swarfs), " + (int)GlobalEnums.rndQuantity + ") AS Swarfs, 0 AS QuantityReceipted, IIF(MIN(PiecePerPack) <> 0, CAST(ROUND(SUM(Quantity + QuantityExcess), " + (int)GlobalEnums.rndQuantity + ") AS int) / MIN(PiecePerPack), 0) AS Packages, IIF(MIN(PiecePerPack) <> 0, ROUND(SUM(Quantity + QuantityExcess), " + (int)GlobalEnums.rndQuantity + ") % MIN(PiecePerPack), 0) AS OddPackages, IIF(MIN(PiecePerPack) <> 0, ROUND(SUM(Quantity + QuantityExcess) * MIN(PackageUnitWeights)/ MIN(PiecePerPack), " + (int)GlobalEnums.rndWeight + "), 0) AS QuantityWeights, IIF(MIN(PiecePerPack) <> 0, ROUND(SUM(QuantityFailure) * MIN(PackageUnitWeights)/ MIN(PiecePerPack), " + (int)GlobalEnums.rndWeight + "), 0) AS QuantityFailureWeights, IIF(MIN(PiecePerPack) <> 0, ROUND(SUM(QuantityExcess) * MIN(PackageUnitWeights)/ MIN(PiecePerPack), " + (int)GlobalEnums.rndWeight + "), 0) AS QuantityExcessWeights, IIF(MIN(PiecePerPack) <> 0, ROUND(SUM(QuantityShortage) * MIN(PackageUnitWeights)/ MIN(PiecePerPack), " + (int)GlobalEnums.rndWeight + "), 0) AS QuantityShortageWeights, 0 AS RecycleWeights, 0 AS RecycleLoss, MAX(Remarks) AS Remarks, 1 AS Approved, 0 AS HandoverApproved, " + "\r\n";

            queryString = queryString + "                                       STUFF   ((SELECT ', ' + SemifinishedProducts.Reference " + "\r\n";
            queryString = queryString + "                                       FROM    FinishedProductDetails INNER JOIN SemifinishedProducts ON FinishedProductDetails.SemifinishedProductID = SemifinishedProducts.SemifinishedProductID " + "\r\n";
            queryString = queryString + "                                       WHERE   FinishedProductDetails.FinishedProductID = @EntityID AND FinishedProductDetails.CommodityID = FinishedProductDetailResults.CommodityID " + "\r\n";
            queryString = queryString + "                                       FOR XML PATH(''),TYPE).value('(./text())[1]','VARCHAR(MAX)'),1,1,'') AS SemifinishedProductReferences " + "\r\n";

            queryString = queryString + "                       FROM            FinishedProductDetails  FinishedProductDetailResults " + "\r\n";
            queryString = queryString + "                       WHERE           FinishedProductID = @EntityID " + "\r\n";
            queryString = queryString + "                       GROUP BY        CommodityID; " + "\r\n";

            queryString = queryString + "                       UPDATE          FinishedProductDetails " + "\r\n";
            queryString = queryString + "                       SET             FinishedProductDetails.FinishedProductPackageID = FinishedProductPackages.FinishedProductPackageID " + "\r\n";
            queryString = queryString + "                       FROM            FinishedProductDetails INNER JOIN FinishedProductPackages ON FinishedProductDetails.FinishedProductID = @EntityID AND FinishedProductDetails.FinishedProductID = FinishedProductPackages.FinishedProductID AND FinishedProductDetails.CommodityID = FinishedProductPackages.CommodityID; " + "\r\n";
            queryString = queryString + "                   END " + "\r\n";

            queryString = queryString + "               ELSE " + "\r\n";
            queryString = queryString + "                   BEGIN " + "\r\n";
            queryString = queryString + "                       UPDATE          FinishedProductDetails SET FinishedProductPackageID = NULL WHERE FinishedProductID = @EntityID ; " + "\r\n";
            queryString = queryString + "                       DELETE FROM     FinishedProductPackages WHERE FinishedProductID = @EntityID ; " + "\r\n";
            queryString = queryString + "                   END " + "\r\n";
            #endregion INIT FinishedProductPackages


            queryString = queryString + "           END " + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           BEGIN " + "\r\n";
            queryString = queryString + "               DECLARE     @msg NVARCHAR(300) = N'Dữ liệu không tồn tại hoặc đã ' + iif(@Approved = 0, N'hủy', '')  + N' duyệt' ; " + "\r\n";
            queryString = queryString + "               THROW       61001,  @msg, 1; " + "\r\n";
            queryString = queryString + "           END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("FinishedProductToggleApproved", queryString);
        }

        private void FinishedProductInitReference()
        {
            SimpleInitReference simpleInitReference = new SimpleInitReference("FinishedProducts", "FinishedProductID", "Reference", ModelSettingManager.ReferenceLength, ModelSettingManager.ReferencePrefix(GlobalEnums.NmvnTaskID.FinishedProduct));
            this.totalSmartPortalEntities.CreateTrigger("FinishedProductInitReference", simpleInitReference.CreateQuery());
        }


        private void FinishedProductSheet()
        {
            string queryString;

            queryString = " @WorkshiftID int, @FinishedProductID int, @FromDate DateTime, @ToDate DateTime " + "\r\n";
            queryString = queryString + " WITH ENCRYPTION " + "\r\n";
            queryString = queryString + " AS " + "\r\n";
            queryString = queryString + "    BEGIN " + "\r\n";
            queryString = queryString + "       SET NOCOUNT ON" + "\r\n";

            queryString = queryString + "       DECLARE     @LocalWorkshiftID int, @LocalFinishedProductID int, @LocalFromDate DateTime, @LocalToDate DateTime      " + "\r\n";
            queryString = queryString + "       SET         @LocalWorkshiftID = @WorkshiftID    SET @LocalFinishedProductID = @FinishedProductID        SET @LocalFromDate = @FromDate      SET @LocalToDate = @ToDate " + "\r\n";

            queryString = queryString + "       IF  (@LocalWorkshiftID <> 0) " + "\r\n";
            queryString = queryString + "           " + this.FinishedProductSheetSQL(true, false) + "\r\n";
            queryString = queryString + "       ELSE " + "\r\n";
            queryString = queryString + "           BEGIN " + "\r\n";
            queryString = queryString + "               IF  (@LocalFinishedProductID <> 0) " + "\r\n";
            queryString = queryString + "                   " + this.FinishedProductSheetSQL(false, true) + "\r\n";
            queryString = queryString + "               ELSE " + "\r\n";
            queryString = queryString + "                   " + this.FinishedProductSheetSQL(false, false) + "\r\n";
            queryString = queryString + "           END " + "\r\n";


            queryString = queryString + "       SET NOCOUNT OFF" + "\r\n";
            queryString = queryString + "    END " + "\r\n";

            this.totalSmartPortalEntities.CreateStoredProcedure("FinishedProductSheet", queryString);
        }

        private string FinishedProductSheetSQL(bool workshiftID, bool finishedProductID)
        {
            string queryString = " " + "\r\n";
            queryString = queryString + "       SELECT      FinishedProducts.FinishedProductID, FinishedProductPackages.FinishedProductPackageID, FinishedProducts.EntryDate, FinishedProducts.Reference, Workshifts.EntryDate AS WorkshiftEntryDate, Workshifts.Code AS WorkshiftCode, FirmOrders.Reference AS FirmOrderReference, FirmOrders.Code AS FirmOrderCode, FirmOrders.EntryDate AS FirmOrderEntryDate, FirmOrders.DeliveryDate, Customers.Name AS CustomerName, " + "\r\n";
            queryString = queryString + "                   Commodities.Code, Commodities.Name, CommodityCategories.Code AS CommodityCategoryCode, CommodityClasses.Code AS CommodityClassCode, Molds.Code AS MoldCode, FirmOrderDetails.FirmOrderQuantity, FinishedProductSummaries.FirmOrderQuantityReceipted, FirmOrderDetails.FirmOrderQuantity - FinishedProductSummaries.FirmOrderQuantityReceipted AS FirmOrderQuantityRemains, FinishedProductSummaries.FinishedQuantityRemains + ISNULL(SemifinishedProductSummaries.SemifinishedQuantityRemains, 0) AS FinishedQuantityRemains, FinishedProductPackages.PiecePerPack, FinishedProductPackages.PackageUnitWeights, FinishedProductPackages.Quantity, FinishedProductPackages.Packages, FinishedProductPackages.OddPackages, FinishedProductPackages.QuantityFailure, FinishedProductPackages.Swarfs, CrucialWorkers.Name AS CrucialWorkerName, CrucialWorkers.LastName AS CrucialWorkerLastName, FinishedProducts.Description " + "\r\n";

            queryString = queryString + "       FROM        FinishedProducts " + "\r\n";
            queryString = queryString + "                   INNER JOIN FinishedProductPackages ON " + this.FinishedProductSheetOption(workshiftID, finishedProductID) + " AND FinishedProducts.FinishedProductID = FinishedProductPackages.FinishedProductID " + "\r\n";
            queryString = queryString + "                   INNER JOIN FirmOrders ON FinishedProductPackages.FirmOrderID = FirmOrders.FirmOrderID " + "\r\n";

            queryString = queryString + "                   INNER JOIN (SELECT FirmOrderID, CommodityID, MIN(MoldID) AS MoldID, SUM(Quantity) AS FirmOrderQuantity FROM FirmOrderDetails WHERE FirmOrderID IN (SELECT DISTINCT FirmOrderID FROM FinishedProducts WHERE " + this.FinishedProductSheetOption(workshiftID, finishedProductID) + ") GROUP BY FirmOrderID, CommodityID) FirmOrderDetails ON FinishedProductPackages.FirmOrderID = FirmOrderDetails.FirmOrderID AND FinishedProductPackages.CommodityID = FirmOrderDetails.CommodityID " + "\r\n";
            queryString = queryString + "                   INNER JOIN (SELECT FirmOrderID, CommodityID, SUM(QuantityReceipted) AS FirmOrderQuantityReceipted, SUM(Quantity - QuantityReceipted) AS FinishedQuantityRemains FROM FinishedProductPackages WHERE FirmOrderID IN (SELECT DISTINCT FirmOrderID FROM FinishedProducts WHERE " + this.FinishedProductSheetOption(workshiftID, finishedProductID) + ") GROUP BY FirmOrderID, CommodityID) FinishedProductSummaries ON FinishedProductPackages.FirmOrderID = FinishedProductSummaries.FirmOrderID AND FinishedProductPackages.CommodityID = FinishedProductSummaries.CommodityID " + "\r\n";

            queryString = queryString + "                   INNER JOIN Molds ON FirmOrderDetails.MoldID = Molds.MoldID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Customers ON FinishedProductPackages.CustomerID = Customers.CustomerID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Commodities ON FinishedProductPackages.CommodityID = Commodities.CommodityID " + "\r\n";
            queryString = queryString + "                   INNER JOIN CommodityClasses ON Commodities.CommodityClassID = CommodityClasses.CommodityClassID " + "\r\n";
            queryString = queryString + "                   INNER JOIN CommodityCategories ON Commodities.CommodityCategoryID = CommodityCategories.CommodityCategoryID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Workshifts ON FinishedProducts.WorkshiftID = Workshifts.WorkshiftID " + "\r\n";
            queryString = queryString + "                   INNER JOIN Employees AS CrucialWorkers ON FinishedProducts.CrucialWorkerID = CrucialWorkers.EmployeeID " + "\r\n";

            queryString = queryString + "                   LEFT JOIN (SELECT FirmOrderID, CommodityID, SUM(Quantity - QuantityFinished) AS SemifinishedQuantityRemains FROM SemifinishedProductDetails WHERE FirmOrderID IN (SELECT DISTINCT FirmOrderID FROM FinishedProducts WHERE " + this.FinishedProductSheetOption(workshiftID, finishedProductID) + ") AND ROUND(Quantity - QuantityFinished, " + (int)GlobalEnums.rndQuantity + ") > 0 GROUP BY FirmOrderID, CommodityID) SemifinishedProductSummaries ON FinishedProductPackages.FirmOrderID = SemifinishedProductSummaries.FirmOrderID AND FinishedProductPackages.CommodityID = SemifinishedProductSummaries.CommodityID " + "\r\n";

            queryString = queryString + "       ORDER BY    FirmOrders.Code, Commodities.Name " + "\r\n";
            
            return queryString;
        }

        private string FinishedProductSheetOption(bool workshiftID, bool finishedProductID) { return (workshiftID ? "FinishedProducts.WorkshiftID = @LocalWorkshiftID" : (finishedProductID ? "FinishedProducts.FinishedProductID = @LocalFinishedProductID" : "FinishedProducts.EntryDate >= @LocalFromDate AND FinishedProducts.EntryDate <= @LocalToDate")); }

        #endregion
    }
}
