(function () {
    var app = angular.module("evePiManager");

    var dragDropController = function ($scope, apiService) {

        var onProductLoad = function (data) {
            $scope.products = new kendo.data.HierarchicalDataSource(data);
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