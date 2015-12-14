(function () {
    var app = angular.module("evePiManager");

    var ProductsController = function ($scope, $location, $filter, apiService) {

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
        };

        var updateLevel = function (name) {
            $scope.currentLevel = name;
        };

        var loadLevel = function () {
            apiService.getProductsForLevel($scope.currentLevel)
                .then(onProductLoad, onError);
        };

        var startAnalysis = function () {
            $.connection.hub.start()
                .done(function () {
                    $.connection.analysisHub.server.start($scope.currentLevel);
                })
                .fail(function () { console.log('Could not Connect to SignalR Server!'); });
        };

        var onAnalysisUpdate = function (data) {
            $scope.$apply(function () {
                var found = $filter('getIndexByProperty')('ProductId', data.Item.ProductId, $scope.products);
                if (found != null) {
                    $scope.products[found] = data.Item;
                } else {
                    console.log('Not found for ' + data.Item.ProductId);
                }
            });
        };

        var onAnalysisComplete = function (data) {
            //console.log("Completed from remote server: ");
            $scope.$apply(function () {
                $filter('orderBy')($scope.products, $scope.productSortOrder)
            });
        };

        var showCanvas = function () {
            $location.path("/canvas");
        };

        var activate = function () {
            $.connection.hub.url = "http://localhost:23456/signalr";
            $.connection.analysisHub.client.updateAnalysisItem = onAnalysisUpdate;
            $.connection.analysisHub.client.analysisComplete = onAnalysisComplete;

            loadProductLevels();
            apiService.getProductsForLevel($scope.currentLevel).then(onProductLoad, onError);
        };

        $scope.productSortOrder = "-ProfitMargin";
        $scope.currentLevel = "Refined Materials";
        $scope.updateLevel = updateLevel;
        $scope.loadLevel = loadLevel;
        $scope.startAnalysis = startAnalysis;
        $scope.showCanvas = showCanvas;
        activate();
    };

    app.controller("ProductsController", ProductsController);

}());