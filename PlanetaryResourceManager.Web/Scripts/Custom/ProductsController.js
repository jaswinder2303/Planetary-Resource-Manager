(function () {
    var app = angular.module("evePiManager");

    var ProductsController = function ($scope, apiService) {

        var onProductLoad = function (data) {
            $scope.products = data;
        };

        var onError = function (reason) {
            $scope.error = "Could not fetch the data";
        };

        $scope.productSortOrder = "+ProfitMargin";
        apiService.getProducts().then(onProductLoad, onError);
    };

    app.controller("ProductsController", ProductsController);

}());