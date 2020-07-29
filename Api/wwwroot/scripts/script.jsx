angular.module('ideaBoardApp', ['ngSanitize'])
    .controller('boardCtrl', ['$scope', '$http', async ($scope, $http) => {

        let self = {
            paginationSize: 5,
        };
        $scope.availablePages = [];
        $scope.projects = [];
        $scope.page = 0;
        $scope.query = {
            sort: 'Date',        // or Vote
            order: 'Descending', // or Ascending
            page: 1              // first page
        };

        $scope.getBoard = async () => {
            const {data: { projects, pages }} = await $http.get("/api/board", {params: $scope.query});
            $scope.projects = projects;
            $scope.pages = pages;
            $scope.availablePages = [$scope.query.page - 3, $scope.query.page - 2, $scope.query.page - 1, $scope.query.page, $scope.query.page + 1, $scope.query.page + 2, $scope.query.page + 3]
                .filter(x => x >= 1 && x <= $scope.pages)
                .slice(0, self.paginationSize);
        };

        $scope.upVote = async (id) => {
            const {data} = await $http.post(`/api/board/${id}/up`);
            $scope.projects[$scope.projects.findIndex(({id: x}) => x === id)] = data;
        };

        $scope.downVote = async (id) => {
            $scope.projects = await $http.post(`/api/board/${id}/down`);
        };

        self.init = async () => {
            await $scope.getBoard();
        };

        self.init().then();
    }]);
