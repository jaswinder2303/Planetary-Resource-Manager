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

}());