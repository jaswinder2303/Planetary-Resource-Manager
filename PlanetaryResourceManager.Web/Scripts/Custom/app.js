(function () {

    var app = angular.module("evePiManager", ["ngRoute", "ui.bootstrap"]);

    app.config(function ($routeProvider, $httpProvider) {
        $routeProvider
        .when("/main", {
            templateUrl: "main.html",
            controller: "ProductsController"
        })
        .otherwise({ redirectTo: "/main" })
    });

}());