class BoardQuery {
    sort = 'Date';        // or Vote
    order = 'Descending'; // or Ascending
    index = 0;            // first page
    pageSize = 10;        // page size

    get page() {
        return this.index + 1;
    }

    set page(value) {
        this.index = value - 1;
    }
}

function availablePages(x, y, z) {
    if (x <= z / 2) {
        return _.range(1, z+1);
    }
    if (x >= (y - z / 2)) {
        return _.range(y - z + 1, y + 1);
    }
    return _.range(Math.ceil(x - z / 2), Math.floor(x + z / 2 +1));
}

function voteIntegerValue(votes) {
    return votes.reduce((acc, x) => x + acc, 0);
}


angular.module('ideaBoardApp', ['ngSanitize'])
    .constant('isAuthenticated', window.isAuthenticated)
    .controller('boardCtrl', ['$scope', '$http', 'isAuthenticated', async ($scope, $http, isAuthenticated) => {

        let self = {
            paginationSize: 5,
        };
        $scope.availablePages = [];
        $scope.projects = [];
        $scope.page = 0;
        $scope.query = new BoardQuery();
        $scope.isAuthenticated = isAuthenticated;
        $scope.voteIntegerValue = voteIntegerValue;

        self.getBoard = async () => {
            const {data: {projects, pages}} = await $http.get("/api/board", {params: $scope.query});
            $scope.projects = projects;
            $scope.pages = pages;
            $scope.availablePages = availablePages($scope.query.page, pages, self.paginationSize);
            $scope.$apply();
        };

        $scope.upVote = async (id) => {
            const {data} = await $http.post(`/api/board/${id}/up`);
            $scope.projects[$scope.projects.findIndex(({id: x}) => x === id)] = data;
        };

        $scope.downVote = async (id) => {
            $scope.projects = await $http.post(`/api/board/${id}/down`);
        };

        $scope.goToPage = async (page) => {
            $scope.query.page = page;
            await self.getBoard();
        };

        self.init = async () => {
            await self.getBoard();
        };
        
        $scope.refresh = self.init;

        self.init().then();
    }]);
