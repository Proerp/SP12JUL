﻿function cancelButton_Click() {
    window.parent.$("#myWindow").data("kendoWindow").close();
}

function handleOKEvent(packageIssueGridDataSource, pendingProductionOrderDetailGridDataSource, closeWhenFinished) {
    if (packageIssueGridDataSource != undefined && pendingProductionOrderDetailGridDataSource != undefined) {
        var pendingProductionOrderDetailGridDataItems = pendingProductionOrderDetailGridDataSource.view();
        var packageIssueJSON = packageIssueGridDataSource.data().toJSON();
        for (var i = 0; i < pendingProductionOrderDetailGridDataItems.length; i++) {
            if (pendingProductionOrderDetailGridDataItems[i].IsSelected === true && pendingProductionOrderDetailGridDataItems[i].GoodsReceiptDetailID != null)
                _setParentInput(packageIssueJSON, pendingProductionOrderDetailGridDataItems[i]);
        }

        packageIssueJSON.push(new Object()); //Add a temporary empty row

        packageIssueGridDataSource.data(packageIssueJSON);

        var rawData = packageIssueGridDataSource.data()
        packageIssueGridDataSource.remove(rawData[rawData.length - 1]); //Remove the last row: this is the temporary empty row

        if (closeWhenFinished)
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


    function _setParentInput(packageIssueJSON, productionOrderGridDataItem) {

        //var dataRow = packageIssueJSON.add({});

        var dataRow = new Object();

        dataRow.LocationID = null;
        dataRow.EntryDate = null;

        dataRow.PackageIssueDetailID = 0;
        dataRow.PackageIssueID = window.parent.$("#PackageIssueID").val();

        dataRow.BlendingInstructionDetailID = productionOrderGridDataItem.BlendingInstructionDetailID;

        dataRow.CommodityID = productionOrderGridDataItem.CommodityID;
        dataRow.CommodityName = productionOrderGridDataItem.CommodityName;
        dataRow.CommodityCode = productionOrderGridDataItem.CommodityCode;
        dataRow.CommodityTypeID = productionOrderGridDataItem.CommodityTypeID;

        dataRow.GoodsReceiptID = productionOrderGridDataItem.GoodsReceiptID;
        dataRow.GoodsReceiptDetailID = productionOrderGridDataItem.GoodsReceiptDetailID;
        dataRow.GoodsReceiptCode = productionOrderGridDataItem.GoodsReceiptCode;
        dataRow.GoodsReceiptReference = productionOrderGridDataItem.GoodsReceiptReference;
        dataRow.GoodsReceiptEntryDate = productionOrderGridDataItem.GoodsReceiptEntryDate;
        dataRow.ExpiryDate = productionOrderGridDataItem.ExpiryDate;

        dataRow.Barcode = productionOrderGridDataItem.Barcode;
        dataRow.BatchCode = productionOrderGridDataItem.BatchCode;
        dataRow.SealCode = productionOrderGridDataItem.SealCode;
        dataRow.LabCode = productionOrderGridDataItem.LabCode;

        dataRow.BinLocationID = productionOrderGridDataItem.BinLocationID;
        dataRow.BinLocationCode = productionOrderGridDataItem.BinLocationCode;

        dataRow.QuantityBIS = productionOrderGridDataItem.QuantityBIS;
        dataRow.QuantityAvailables = productionOrderGridDataItem.QuantityAvailables;
        dataRow.QuantityRemains = productionOrderGridDataItem.QuantityRemains;
        dataRow.Quantity = 0;

        dataRow.BatchID = productionOrderGridDataItem.BatchID;
        dataRow.BatchEntryDate = productionOrderGridDataItem.BatchEntryDate;

        dataRow.PackageIssueImage1ID = null;
        dataRow.PackageIssueImage2ID = null;

        dataRow.Remarks = null;


        packageIssueJSON.push(dataRow);
    }
}

