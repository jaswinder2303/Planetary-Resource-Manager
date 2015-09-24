(function () {
    var app = angular.module("evePiManager");

    var dragDropController = function ($scope, $modal, $filter, apiService) {

        var onProductLoad = function (payload) {
            //debugger;
            var panelData = new kendo.data.HierarchicalDataSource({ data: [] });
            var treeData = new kendo.data.HierarchicalDataSource();

            payload.forEach(function (item) {
                treeData.add({
                    text: item.Product.Name,
                    id: item.ProductId
                });
            });

            $scope.products = treeData;
            $scope.dropHost = panelData;
            $scope.analysisItems = payload;

            $(".treeViewPanelSource").kendoTreeView({
                dragAndDrop: true,
                dataSource: $scope.products,
                drop: onItemDrop,
                dragstart: onDragStart
            });

            $(".treeViewPanelTarget").kendoTreeView({
                dragAndDrop: true,
                dataSource: $scope.dropHost,
                drop: onItemDrop,
                dragstart: onDragStart,
                select: handleClick,
                template: kendo.template($("#treeview-template").html())
            });
        };

        function onDragStart(e) {
            //prevent dragging from the right-hand side
            if ($(e.sourceNode).parentsUntil(".canvasPanel", ".leftPanel").length == 0) {
                e.preventDefault();
            }
        }

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
                $(".treeViewPanelTarget .k-button").bind('click', {selected: sourceItem.id}, openModal);
            });
        }

        var onError = function (reason) {
            $scope.error = "Could not fetch the data";
        };

        var loadLevel = function () {
            apiService.getProductsForLevel($scope.currentLevel)
                .then(onProductLoad, onError);
        };

        var handleClick = function(event){
            console.log("Doouble click triggered!");
        };

        var openModal = function (event) {

            var found = $filter('getByProperty')('ProductId', event.data.selected, $scope.analysisItems);

            if (found != null) {
                $scope.selectedItem = found;

                var modalInstance = $modal.open({
                    animation: true,
                    templateUrl: 'editor.html',
                    controller: 'EditorController',
                    resolve: {
                        data: function () {
                            return $scope.selectedItem;
                        }
                    }
                });

                modalInstance.result.then(function (selectedItem) {
                    //$scope.selected = selectedItem;
                }, function () {
                    console.log('Modal dismissed at: ' + new Date());
                });

            }
            else {
                console.log('Not found for ' + event.data.selected);
            } 
        };

        var activate = function () {
            $scope.currentLevel = "Refined Materials";
            $scope.updateText = "Select an item to drag";
            $scope.open = openModal;
            $scope.choose = handleClick;
            loadLevel();
        };

        activate();
    };

    app.controller("DragDropController", dragDropController);
}());