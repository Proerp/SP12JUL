﻿function cancelButton_Click() {
    window.parent.$("#myWindow").data("kendoWindow").close();
}

function handleOKEvent(goodsReceiptGridDataSource, pendingGoodsArrivalPackageGridDataSource) {
    if (goodsReceiptGridDataSource != undefined && pendingGoodsArrivalPackageGridDataSource != undefined) {
        var pendingGoodsArrivalPackageGridDataItems = pendingGoodsArrivalPackageGridDataSource.view();
        var goodsReceiptJSON = goodsReceiptGridDataSource.data().toJSON();
        for (var i = 0; i < pendingGoodsArrivalPackageGridDataItems.length; i++) {
            if (pendingGoodsArrivalPackageGridDataItems[i].IsSelected === true)
                _setParentInput(goodsReceiptJSON, pendingGoodsArrivalPackageGridDataItems[i]);
        }

        goodsReceiptJSON.push(new Object()); //Add a temporary empty row

        goodsReceiptGridDataSource.data(goodsReceiptJSON);

        var rawData = goodsReceiptGridDataSource.data()
        goodsReceiptGridDataSource.remove(rawData[rawData.length - 1]); //Remove the last row: this is the temporary empty row

        cancelButton_Click();
    }


    //http://www.telerik.com/forums/adding-multiple-rows-performance
    //By design the dataSource does not provide an opportunity for inserting multiple records via one operation. The performance is low, because each time when you add row through the addRow method, the DataSource throws its change event which forces the Grid to refresh and re-paint the content.
    //To avoid the problem you may try to modify the data of the DataSource manually.
    //var grid = $("#grid").data("kendoGrid");
    //var data = gr.dataSource.data().toJSON(); //the data of the DataSource

    ////change the data array
    ////any changes in the data array will not automatically reflect in the Grid

    //grid.dataSource.data(data); //set changed data as data of the Grid


    function _setParentInput(goodsReceiptJSON, goodsArrivalGridDataItem) {

        //var dataRow = goodsReceiptJSON.add({});

        var dataRow = new Object();

        dataRow.LocationID = null;
        dataRow.EntryDate = null;

        dataRow.GoodsReceiptDetailID = 0;
        dataRow.GoodsReceiptID = window.parent.$("#GoodsReceiptID").val();


        dataRow.MaterialIssueID = null;
        dataRow.MaterialIssueDetailID = null;
        dataRow.MaterialIssueEntryDate = null;
        dataRow.ProductionLinesCode = null;

        dataRow.WorkshiftName = null;
        dataRow.WorkshiftEntryDate = null;


        dataRow.FinishedItemID = null;
        dataRow.FinishedItemPackageID = null;
        dataRow.FinishedItemEntryDate = null;
        dataRow.SemifinishedItemReferences = null;

        dataRow.FinishedProductID = null;
        dataRow.FinishedProductPackageID = null;
        dataRow.FinishedProductEntryDate = null;
        dataRow.SemifinishedProductReferences = null;

        dataRow.FirmOrderReference = null;
        dataRow.FirmOrderCode = null;
        dataRow.FirmOrderSpecs = null;        


        dataRow.RecyclateID = null;
        dataRow.RecyclatePackageID = null;
        dataRow.RecyclateEntryDate = null;


        dataRow.PurchaseRequisitionID = null;
        dataRow.PurchaseRequisitionDetailID = null;
        dataRow.PurchaseRequisitionCode = null;
        dataRow.PurchaseRequisitionReference = null;
        dataRow.PurchaseRequisitionEntryDate = null;


        dataRow.GoodsArrivalID = goodsArrivalGridDataItem.GoodsArrivalID;
        dataRow.GoodsArrivalDetailID = goodsArrivalGridDataItem.GoodsArrivalDetailID;
        dataRow.GoodsArrivalPackageID = goodsArrivalGridDataItem.GoodsArrivalPackageID;
        dataRow.GoodsArrivalCode = goodsArrivalGridDataItem.GoodsArrivalCode;
        dataRow.GoodsArrivalReference = goodsArrivalGridDataItem.GoodsArrivalReference;
        dataRow.GoodsArrivalEntryDate = goodsArrivalGridDataItem.GoodsArrivalEntryDate;
        dataRow.PurchaseOrderCodes = goodsArrivalGridDataItem.PurchaseOrderCodes;
        dataRow.CustomerCode = goodsArrivalGridDataItem.CustomerCode;

        dataRow.WarehouseTransferID = null;
        dataRow.WarehouseTransferDetailID = null;
        dataRow.WarehouseTransferReference = null;
        dataRow.WarehouseTransferEntryDate = null;
        dataRow.GoodsReceiptReference = null;
        dataRow.GoodsReceiptEntryDate = null;

        dataRow.BatchID = goodsArrivalGridDataItem.BatchID;
        dataRow.BatchEntryDate = goodsArrivalGridDataItem.BatchEntryDate;


        dataRow.CommodityID = goodsArrivalGridDataItem.CommodityID;
        dataRow.CommodityName = goodsArrivalGridDataItem.CommodityName;
        dataRow.CommodityCode = goodsArrivalGridDataItem.CommodityCode;
        dataRow.CommodityTypeID = goodsArrivalGridDataItem.CommodityTypeID;

        dataRow.LabID = goodsArrivalGridDataItem.LabID;

        dataRow.Barcode = goodsArrivalGridDataItem.Barcode;
        dataRow.BatchCode = goodsArrivalGridDataItem.BatchCode;
        dataRow.SealCode = goodsArrivalGridDataItem.SealCode;
        dataRow.LabCode = goodsArrivalGridDataItem.LabCode;

        dataRow.BinLocationID = 1;
        dataRow.BinLocationCode = "DEFAULT";

        dataRow.ProductionDate = goodsArrivalGridDataItem.ProductionDate;
        dataRow.ExpiryDate = goodsArrivalGridDataItem.ExpiryDate;

        dataRow.UnitWeight = goodsArrivalGridDataItem.UnitWeight;
        dataRow.TareWeight = goodsArrivalGridDataItem.TareWeight;

        dataRow.QuantityRemains = goodsArrivalGridDataItem.QuantityRemains;
        dataRow.Quantity = goodsArrivalGridDataItem.QuantityRemains; //SET DEFAULT TOO!

        dataRow.Remarks = null;


        goodsReceiptJSON.push(dataRow);
    }
}

