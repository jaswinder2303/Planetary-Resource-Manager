(function () {

    var apiService = function ($http) {

        var getProducts = function () {
            return $http.get("http://localhost:23456/api/products")
              .then(function (response) {
                  return response.data
              })
        }

        var getProduct = function (name) {
            return $http.get("http://localhost:23456/api/product/" + name)
              .then(function (response) {
                  return response.data
              })
        }

        return {
            getProduct: getProduct,
            getProducts: getProducts
        };
    };

    var module = angular.module("evePiManager");
    module.factory("apiService", apiService);

}());