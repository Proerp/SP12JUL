require(["xlsxNmvn", "xlsxWorkbook"], function (xlsxNmvn, xlsxWorkbook) {

    $(document).ready(function () {
        var xlf = document.getElementById('xlf');

        if (xlf != null) {
            if (xlf.addEventListener) {
                xlf.addEventListener('change', handleFile, false);
            }
        }
    });




    process_wb = function (wb) {

        var jsonWorkBook = JSON.stringify(to_json(wb), 2, 2); //jsonWorkBook = to_formulae(wb); //jsonWorkBook = to_csv(wb);
        if (jsonWorkBook != undefined && wb.SheetNames[0]) { jsonWorkBook = jsonWorkBook.replace(wb.SheetNames[0], "ImportSheet"); }

        var excelRowCollection = JSON.parse(jsonWorkBook);

        var xlsxWorkbookInstance = new xlsxWorkbook(["Barcode"]);
        if (xlsxWorkbookInstance.checkValidColumn(excelRowCollection.ImportSheet)) {

            var gridDataSource = $("#kendoGridDetails").data("kendoGrid").dataSource;

            for (i = 0; i < excelRowCollection.ImportSheet.length; i++) {

                var dataRow = gridDataSource.add({});
                var excelRow = excelRowCollection.ImportSheet[i];

                _getCommoditiesByCode(dataRow, excelRow);
            }
        }
        else {
            alert("Lỗi import dữ liệu. Vui lòng kiểm tra file excel cẩn thận trước khi thử import lại");
        }



        function _getCommoditiesByCode(dataRow, excelRow) {
            return $.ajax({
                url: window.urlGoodsReceiptApi,
                data: JSON.stringify({ "locationID": requireConfig.pageOptions.LocationID, "warehouseID": $("#Warehouse_WarehouseID").val(), "warehouseReceiptID": $("#WarehouseReceipt_WarehouseID").val() != undefined ? $("#WarehouseReceipt_WarehouseID").val() : null, "commodityID": null, "commodityIDs": "", "batchID": null, "blendingInstructionID": $("#BlendingInstructionID").val() != undefined ? $("#BlendingInstructionID").val() : null, "barcode": excelRow["Barcode"], "goodsReceiptDetailIDs": "", "onlyApproved": true, "onlyIssuable": true }),

                type: 'POST',
                contentType: 'application/json;',
                dataType: 'json',
                success: function (result) {
                    if (result.CommodityID > 0) {
                        
                        dataRow.CommodityID = result.CommodityID;
                        dataRow.CommodityName = result.CommodityName;
                        dataRow.CommodityCode = result.CommodityCode;
                        dataRow.CommodityTypeID = result.CommodityTypeID;

                        dataRow.TransferOrderID = result.TransferOrderID === undefined ? null : result.TransferOrderID;
                        dataRow.TransferOrderDetailID = result.TransferOrderDetailID === undefined ? null : result.TransferOrderDetailID;

                        dataRow.GoodsReceiptID = result.GoodsReceiptID;
                        dataRow.GoodsReceiptDetailID = result.GoodsReceiptDetailID;
                        dataRow.GoodsReceiptCode = result.GoodsReceiptCode;
                        dataRow.GoodsReceiptReference = result.GoodsReceiptReference;
                        dataRow.GoodsReceiptEntryDate = new Date(result.GoodsReceiptEntryDate.match(/\d+/)[0] * 1); //result.GoodsReceiptEntryDate                                - get the string from the JSON -> result.GoodsReceiptEntryDate.match(/\d+/)[0]                - extract the numeric part -> result.GoodsReceiptEntryDate.match(/\d+/)[0] * 1            - convert it to a numeric type -> new Date(result.GoodsReceiptEntryDate.match(/\d+/)[0] * 1)) - Create a date object

                        dataRow.BatchID = result.BatchID;
                        dataRow.BatchEntryDate = new Date(result.BatchEntryDate.match(/\d+/)[0] * 1);

                        dataRow.Barcode = result.Barcode;
                        dataRow.BatchCode = result.BatchCode;
                        dataRow.SealCode = result.SealCode;
                        dataRow.LabCode = result.LabCode;

                        dataRow.BinLocationIssuedID = result.BinLocationID;
                        dataRow.BinLocationIssuedCode = result.BinLocationCode;

                        dataRow.BinLocationID = window.$("#CBPP").val() == 'True' ? ($("#WarehouseReceipt_WarehouseID").val() == 6 ? 77 : null) : 0;
                        dataRow.BinLocationCode = window.$("#CBPP").val() == 'True' ? ($("#WarehouseReceipt_WarehouseID").val() == 6 ? "G01" : null) : "#";

                        dataRow.QuantityTO = result.QuantityTO === undefined ? 0 : result.QuantityTO;
                        dataRow.TransferOrderRemains = result.TransferOrderRemains === undefined ? 0 : result.TransferOrderRemains;
                        dataRow.QuantityRemains = result.QuantityRemains;
                        dataRow.QuantityAvailables = result.QuantityAvailables;
                        dataRow.Quantity = result.QuantityAvailables; //INIT BY THE WHOLE QuantityAvailables

                    }
                    else
                        dataRow.set("CommodityName", result.CommodityName);
                },
                error: function (jqXHR, textStatus) {
                    dataRow.set("CommodityName", textStatus);
                }
            });
        }



    }




});