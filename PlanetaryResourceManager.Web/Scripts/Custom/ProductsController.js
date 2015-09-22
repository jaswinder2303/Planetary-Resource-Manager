(function () {
    var app = angular.module("evePiManager");

    var ProductsController = function ($scope, $location, apiService) {

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
            //$location.path("/canvas");
            $.connection.hub.start()
                .done(function () {
                    $.connection.analysisHub.server.start($scope.currentLevel);
                })
                .fail(function () { console.log('Could not Connect to SignalR Server!'); });
        }

        var analysisUpdate = function (data) {
            console.log("Update from remote server: " + data.ProgressIndex);

            $()
        }

        var analysisComplete = function (data) {
            console.log("Update from remote server: " + data);
        }

        var activate = function () {
            $.connection.hub.url = "http://localhost:23456/signalr";
            $.connection.analysisHub.client.updateAnalysisItem = analysisUpdate;
            $.connection.analysisHub.client.analysisComplete = analysisComplete;

            loadProductLevels();
            apiService.getProducts().then(onProductLoad, onError);
        }

        $scope.productSortOrder = "+ProfitMargin";
        $scope.currentLevel = "Raw Materials";
        $scope.updateLevel = updateLevel;
        $scope.loadLevel = loadLevel;
        $scope.startAnalysis = startAnalysis;
        activate();
    };

    app.controller("ProductsController", ProductsController);

}());