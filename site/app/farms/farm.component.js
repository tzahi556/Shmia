(function () {
    var app = angular.module('app');

    app.component('farm', {
        templateUrl: 'app/farms/farm.template.html',
        controller: FarmController,
        bindings: {
            farm: '<'
        }
    });

    function FarmController($scope, farmsService, $state) {

        var self = this;
        this.submit = _submit.bind(this);
        this.delete = _delete.bind(this);

        this.addNewTag = _addNewTag.bind(this);
        this.removeTags = _removeTags.bind(this);
        this.initNewTags = _initNewTags.bind(this);


        function init() {
            self.farm.Meta = self.farm.Meta || {};
            self.farm.Meta.StartDate = self.farm.Meta.StartDate ? new Date(self.farm.Meta.StartDate) : null;
            self.farm.Meta.EndDate = self.farm.Meta.EndDate ? new Date(self.farm.Meta.EndDate) : null;
            self.farm.IsHiyuvInHashlama = (self.farm.IsHiyuvInHashlama) ? self.farm.IsHiyuvInHashlama.toString() : "0";
            self.farm.Style = (self.farm.Style) ? self.farm.Style.toString() : "0";

            self.initNewTags();

        }

        init();

        function _submit() {
            if ($scope.farmForm.$valid) {
                farmsService.updateFarm(this.farm).then(function (farm) {
                    this.farm = farm;
                    init();
                    alert('נשמר בהצלחה');
                }.bind(this));
            }
        }

        function _delete() {
            if (confirm('האם למחוק את החווה?')) {
                farmsService.deleteFarm(this.farm.Id).then(function () {
                    $state.go('farms');
                });
            }
        }



        function _initNewTags() {

            self.farm.Meta.farmTags = self.farm.Meta.farmTags || [];
            self.newfarmTag = null;
          
            //if ($scope.newHorseForm != null) {
            //    $scope.newHorseForm.$setPristine();
            //}
        }

        function _addNewTag() {

          
            //for (var i in this.userhorses) {
            //    if (this.userhorses[i].HorseId == this.newHorse.Id) {
            //        return false;
            //    }
            //}
           
            self.farm.Meta.farmTags.push({ tag_name: self.newfarmTag.tag_name, tag_id: self.newfarmTag.tag_id });
            //self.newFarmTags.tag_name = "";
            //self.newFarmTags.tag_id = "";
            self.initNewTags();
        }

        function _removeTags(tag) {
            var farmTags = self.farm.Meta.farmTags;
            for (var i in farmTags) {
                if (farmTags[i].tag_id == tag.tag_id && farmTags[i].tag_name == tag.tag_name) {
                    farmTags.splice(i, 1);
                }
            }
        }
    }

})();