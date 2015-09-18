(function () {

    var apiService = function ($http) {

        var getProducts = function () {
            return $http.get("http://localhost:23456/api/products/getallproducts")
              .then(function (response) {
                  return response.data
              })
        }

        var getProductsForLevel = function (name) {
            return $http.get("http://localhost:23456/api/products/loadallproducts/" + name)
              .then(function (response) {
                  return response.data
              })
        }

        var getProductLevels = function (name) {
            return $http.get("http://localhost:23456/api/products/loadproductlevels")
              .then(function (response) {
                  return response.data
              })
        }

        return {
            getProductsForLevel: getProductsForLevel,
            getLevels: getProductLevels,
            getProducts: getProducts,
        };
    };

    var module = angular.module("evePiManager");
    module.factory("apiService", apiService);

}());