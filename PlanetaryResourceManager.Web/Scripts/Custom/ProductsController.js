(function () {
    var app = angular.module("evePiManager");

    var ProductsController = function ($scope, apiService) {

        var onProductLoad = function (data) {
            $scope.products = data;
        };

        var onLevelLoad = function (data) {
            $scope.levels = data;
        }

        var onError = function (reason) {
            $scope.error = "Could not fetch the data";
        };

        var loadProductLevels = function () {
            apiService.getLevels()
                .then(onLevelLoad, onError);
        }

        var updateLevel = function (name) {
            $scope.currentLevel = name;
        }

        var loadLevel = function () {
            apiService.getProductsForLevel($scope.currentLevel)
                .then(onProductLoad, onError);
        }

        var startAnalysis = function () {

        }

        $scope.productSortOrder = "+ProfitMargin";
        $scope.currentLevel = "Raw Materials"
        $scope.updateLevel = updateLevel;
        $scope.loadLevel = loadLevel;
        $scope.startAnalysis = startAnalysis;
        loadProductLevels();
        apiService.getProducts().then(onProductLoad, onError);
    };

    app.controller("ProductsController", ProductsController);

}());