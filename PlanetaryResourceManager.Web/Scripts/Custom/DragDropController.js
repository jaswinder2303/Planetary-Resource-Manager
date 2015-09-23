(function () {
    var app = angular.module("evePiManager");

    var dragDropController = function ($scope, apiService) {

        var onProductLoad = function (payload) {
            //debugger;
            var panelData = new kendo.data.HierarchicalDataSource({ data: [] });
            var treeData = new kendo.data.HierarchicalDataSource();
            //treeData.add({ text: "Item 1" });

            payload.forEach(function (item) {
                treeData.add({
                    text: item.Product.Name
                });
            });

            $scope.products = treeData;
            $scope.dropHost = panelData;

            $(".treeViewPanel").kendoTreeView({
                dragAndDrop: true,
                drop: onItemDrop
            });
        };

        function onItemDrop(e) {
            e.preventDefault();

            var sourceItem = this.dataItem(e.sourceNode).toJSON();
            var destinationNode = $(e.destinationNode);
            var targetTree = destinationNode.closest("[data-role='treeview']").data("kendoTreeView");

            if (e.dropPosition == "before") {
                targetTree.insertBefore(sourceItem, destinationNode);
            } else if (e.dropPosition == "after") {
                targetTree.insertAfter(sourceItem, destinationNode);
            } else {
                targetTree.append(sourceItem, destinationNode);
            }
            $scope.$apply(function () {
                $scope.updateText = "You compeleted a drag and drop!";
            });
        }

        var onError = function (reason) {
            $scope.error = "Could not fetch the data";
        };

        var loadLevel = function () {
            apiService.getProductsForLevel($scope.currentLevel)
                .then(onProductLoad, onError);
        };

        var activate = function () {
            $scope.currentLevel = "Refined Materials";
            $scope.updateText = "Select an item to drag";
            loadLevel();
        };

        activate();
    };

    app.controller("DragDropController", dragDropController);
}());