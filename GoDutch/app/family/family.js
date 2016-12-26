(function () {
    angular
        .module('GoDutch.Family', [])
        .service('familyService', ['$http', function ($http) {
            this.getAll = function () {
                return $http.get('http://GoDutch/api/families/')
            }
        }])
        .controller('Family', ['familyService',
            function (familyService) {
                var vm = this;

                init();

                function init(){
                    loadAllFamilies();
                }

                function loadAllFamilies() {
                    familyService.getAll().then(function (response) {
                        vm.families = response.data;
                    });
                }
            }
            
        ]);
})();