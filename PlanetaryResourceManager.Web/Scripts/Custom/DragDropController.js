(function () {
    var app = angular.module("evePiManager");

    var dragDropController = function ($scope, apiService) {

        var onProductLoad = function (payload) {
            //debugger;
            var panelData = new kendo.data.HierarchicalDataSource({ data: [] });
            var treeData = new kendo.data.HierarchicalDataSource();
            //treeData.add({ text: "Item 1" });

            payload.forEach(function (item) {
                treeData.add({
                    text: item.Product.Name
                });
            });

            $scope.products = treeData;
            $scope.dropHost = panelData;

            $(".treeViewPanel").kendoTreeView({
                dragAndDrop: true,
            });
        };

        var onError = function (reason) {
            $scope.error = "Could not fetch the data";
        };

        var loadLevel = function () {
            apiService.getProductsForLevel($scope.currentLevel)
                .then(onProductLoad, onError);
        }

        $scope.currentLevel = "Raw Materials";
        loadLevel();
    };

    app.controller("DragDropController", dragDropController);
}());