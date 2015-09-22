(function () {

    var app = angular.module("evePiManager", ["ngRoute", "ui.bootstrap", "kendo.directives"]);

    app.config(function ($routeProvider, $httpProvider) {
        $routeProvider
        .when("/main", {
            templateUrl: "main.html",
            controller: "ProductsController"
        })
        .when("/canvas", {
            templateUrl: "canvas.html",
            controller: "DragDropController"
        })
        .otherwise({ redirectTo: "/main" })
    });

    app.filter('getByProperty', function () {
        return function (propertyName, propertyValue, collection) {
            var i = 0, len = collection.length;
            for (; i < len; i++) {
                var current = collection[i][propertyName];
                if (collection[i][propertyName] == +propertyValue) {
                    return collection[i];
                }
            }
            return null;
        }
    });

    app.filter('getIndexByProperty', function () {
        return function (propertyName, propertyValue, collection) {
            var i = 0, len = collection.length;
            for (; i < len; i++) {
                var current = collection[i][propertyName];
                if (collection[i][propertyName] == +propertyValue) {
                    return i;
                }
            }
            return null;
        }
    });
}());