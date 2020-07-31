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

function voteIntegerValue(vote) {
    return vote.value === 'up'? 1 : -1;
}

function votesIntegerValue(votes) {
    return votes.reduce((acc, vote) => voteIntegerValue(vote) + acc, 0);
}

function hasVoted(user, project, type) {
    return !!project.votes.find(x => x.userId === user.id && x.value === type);
}

angular.module('ideaBoardApp', ['ngSanitize'])
    .constant('isAuthenticated', window.isAuthenticated)
    .constant('user', window.user)
    .controller('boardCtrl', ['$scope', '$http', 'isAuthenticated', 'user', async ($scope, $http, isAuthenticated, user) => {

        let self = {
            paginationSize: 5,
        };
        $scope.availablePages = [];
        $scope.projects = [];
        $scope.page = 0;
        $scope.query = new BoardQuery();
        $scope.isAuthenticated = isAuthenticated;
        $scope.user = user;
        $scope.votesIntegerValue = votesIntegerValue;
        $scope.hasVoted = hasVoted;

        self.getBoard = async () => {
            const {data: {projects, pages}} = await $http.get("/api/board", {params: $scope.query});
            $scope.projects = projects;
            $scope.pages = pages;
            $scope.availablePages = availablePages($scope.query.page, pages, self.paginationSize);
            $scope.$apply();
        };

        $scope.vote = async (id, type) => {
            const {data} = await $http.post(`/api/board/vote/${id}/${type}`);
            $scope.projects[$scope.projects.findIndex(({id: x}) => x === id)] = data;
            $scope.$apply();
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
