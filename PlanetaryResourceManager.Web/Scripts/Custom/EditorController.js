(function () {
    var app = angular.module('evePiManager');

    var editorController = function ($scope, $modalInstance, data) {
        var activate = function () {
            $scope.modalData = data;

            $scope.ok = function () {
                $modalInstance.close($scope.modalData);
            };

            $scope.cancel = function () {
                $modalInstance.dismiss('cancel');
            };
        };

        activate();
    };

    var productView = function () {
        return {
            restrict: 'E',
            scope: {
                item: '='
            },
            templateUrl: 'product.html'
        };
    };

    app.controller("EditorController", editorController);
    app.directive('productEditor', productView);
}());