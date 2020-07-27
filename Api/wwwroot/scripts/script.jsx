angular.module('ideaBoardApp', [])
    .controller('boardCtrl', ['$scope', '$http', async ($scope, $http) => {

        let self = {};
        $scope.projects = [];
        $scope.query = {
            sort: 'Date',        // or Vote
            order: 'Descending', // or Ascending
            page: 1              // first page
        };

        self.getProjects = async () => {
            $scope.projects = await $http.get("/api/board", {params: $scope.query});
        };

        self.upVote = async (id) => {
            const { data } = await $http.post(`/api/board/${id}/up`);
            $scope.projects[$scope.projects.findIndex(({ id: x }) => x === id)] = data;
        };

        self.downVote = async (id) => {
            $scope.projects = await $http.post(`/api/board/${id}/down`);
        };

        self.init = async () => {
            await self.getProjects();
        };
        
        await self.init();
    }]);
