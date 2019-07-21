define(["gridDatasourceQuantityFailure"], (function (gridDatasourceQuantityFailure) {
    $(document).ready(function () {

        $("#kendoGridDetails").data("kendoGrid").dataSource.bind("change", function (e) {
            var gridDatasourceQuantityFailureInstance = new gridDatasourceQuantityFailure("kendoGridDetails");
            gridDatasourceQuantityFailureInstance.handleDataSourceChange(e);
        });

    });
}));
