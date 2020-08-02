class BoardQuery {
    sort = 'Date';        // or Vote
    order = 'Descending'; // or Ascending
    index = 0;            // first page
    pageSize = 10;        // page size
    keyword = '';         // keyword
    category = '';        // no category

    get page() {
        return this.index + 1;
    }

    set page(value) {
        this.index = value - 1;
    }
}

function Markdown(self) {
    self.editor = {
        src: '',
        parsed: ''
    };
    self.preview = false;

    self.textChange = () => {
        self.editor.parsed = marked(self.editor.src);
    };

    self.onEditor = (param) => {
        const sel = self.getSelection();
        let aUrl;
        switch (param) {
            case "bold":
                if (self.hasSelection()) {
                    // enhance
                    self.insertText("**" + sel.text + "**", sel.start, sel.end);
                } else {
                    // add new
                    self.insertPlacehodler("**bold**", 2, 2);
                }
                break;
            case "italic":
                if (self.hasSelection()) {
                    // enhance
                    self.insertText("_" + sel.text + "_", sel.start, sel.end);
                } else {
                    // add new
                    self.insertPlacehodler("_italic_", 1, 1);
                }
                break;
            case "underline":
                if (self.hasSelection()) {
                    // enhance
                    self.insertText("<u>" + sel.text + "</u>", sel.start, sel.end);
                } else {
                    // add new
                    self.insertPlacehodler("<u>underline</u>", 3, 4);
                }
                break;
            case "list":
                sel.target.value += "\n";
                self.insertPlacehodler("- First item", 2, 0);
                break;
            case "list-2":
                sel.target.value += "\n";
                self.insertPlacehodler("1. First numbered item", 3, 0);
                break;
            case "header":
                sel.target.value += "\n";
                self.insertPlacehodler("# Header", 2, 0);
                break;
            case "url":

                let iUrl = prompt("Enter URL here:");
                if (iUrl === "") {
                    iUrl = "http://codedaily.vn";
                }
                sel.target.value += "\n";
                // insert new
                aUrl = "[text](" + iUrl + ")";
                self.insertPlacehodler(aUrl, 1, iUrl.length + 3);

                break;
            case "img":

                iUrl = prompt("Enter image URL here:");
                if (iUrl === "") {
                    iUrl = "http://codedaily.vn";
                }
                sel.target.value += "\n";
                // insert new
                aUrl = "![image text](" + iUrl + ")";
                self.insertPlacehodler(aUrl, 2, iUrl.length + 3);

                break;
            case "code":
                if (self.hasSelection()) {
                    // enhance
                    self.insertText("`" + sel.text + "`", sel.start, sel.end);
                } else {
                    // add new
                    sel.target.value += "\n";
                    self.insertPlacehodler("<pre class='brush: language'>code here</pre>", 19, 17);
                }
                break;
            case "horline":
                sel.target.value += "\n---";
                sel.target.focus();
                break;
            case "quote":
                if (self.hasSelection()) {
                    // enhance
                    self.insertText("> " + sel.text, sel.start, sel.end);
                } else {
                    // add new
                    self.insertPlacehodler("> quote", 2, 0);
                }
                break;
            case "strikethrough":
                if (self.hasSelection()) {
                    // enhance
                    self.insertText("~~" + sel.text + "~~", sel.start, sel.end);
                } else {
                    // add new
                    self.insertPlacehodler("~~strikethrough~~", 2, 2);
                }
                break;
        }
    };

    self.hasSelection = () => {
        const ta = angular.element("#mark-editor")[0];
        return ta.selectionStart !== ta.textLength;
    };

    self.getSelection = () => {
        const ta = angular.element("#mark-editor")[0];

        return {
            target: ta,
            start: ta.selectionStart,
            end: ta.selectionEnd,
            text: ta.value.substring(ta.selectionStart, ta.selectionEnd)
        };
    };

    self.insertPlacehodler = (text, padLeft, padRight) => {
        const ta = angular.element("#mark-editor")[0];
        ta.focus();
        ta.value += text;
        ta.selectionStart = ta.textLength - text.length + padLeft;
        ta.selectionEnd = ta.textLength - padRight;
    };

    self.insertText = (text, start, end) => {
        const ta = angular.element("#mark-editor")[0];
        ta.focus();
        const leftText = ta.value.substring(0, start);
        const rightText = ta.value.substring(end);
        ta.value = leftText + text + rightText;
    };
}


function availablePages(currentPage, pages, window) {
    if (pages <= window) {
        return _.range(1, pages + 1);
    }

    if (currentPage <= window / 2) {
        return _.range(1, window + 1);
    }
    if (currentPage >= (pages - window / 2)) {
        return _.range(pages - window + 1, pages + 1);
    }
    return _.range(Math.ceil(currentPage - window / 2), Math.floor(currentPage + window / 2 + 1));
}

function voteIntegerValue(vote) {
    return vote.value === 'up' ? 1 : -1;
}

function votesIntegerValue(votes) {
    return votes.reduce((acc, vote) => voteIntegerValue(vote) + acc, 0);
}

function hasVoted(user, project, type) {
    return !!project.votes.find(x => x.userId === user.id && x.value === type);
}

angular.module('ideaBoardApp', ['ngSanitize', 'ngTagsInput'])
    .constant('isAuthenticated', window.isAuthenticated)
    .constant('user', window.user)
    .controller('boardCtrl', ['$scope', '$http', 'isAuthenticated', 'user', async ($scope, $http, isAuthenticated, user) => {

        let self = {
            paginationSize: 5,
        };
        $scope.initialized = false;
        $scope.availablePages = [];
        $scope.projects = [];
        $scope.page = 1;
        $scope.query = new BoardQuery();
        $scope.isAuthenticated = isAuthenticated;
        $scope.user = user;
        $scope.votesIntegerValue = votesIntegerValue;
        $scope.hasVoted = hasVoted;

        self.getBoard = async () => {
            const {data: {projects, pages, categories}} = await $http.get("/api/board", {params: $scope.query});
            $scope.projects = projects;
            $scope.pages = pages;
            $scope.availablePages = availablePages($scope.query.page, pages, self.paginationSize);
            $scope.categories = categories.map(({name}) => name);
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
            $scope.initialized = true;
        };

        $scope.refresh = self.init;

        self.init().then();
    }])
    .controller("addProjectCtrl", ["$scope", "$window", "$http", function ($scope, $window, $http) {

        $scope.categories = [];
        $scope.loadTags = async (query) => {
            const {data} = await $http.get("/api/category");
            const categories = data.map(x => x.name).filter(x => x.toLowerCase().includes(query))
            return {data: categories};
        };

        Markdown($scope);

        $scope.saveChanges = async () => {
            await $http.post('/projects/add', {
                title: $scope.title,
                description: $scope.editor.src,
                categories: $scope.categories
            });

            $window.location.href = '/projects';
        };

    }])
    .controller("updateProjectCtrl", ["$scope", "$window", "$http", function ($scope, $window, $http) {

        $scope.categories = [];
        $scope.loadTags = async (query) => {
            const {data} = await $http.get("/api/category");
            const categories = data.map(x => x.name).filter(x => x.toLowerCase().includes(query))
            return {data: categories};
        };

        const self = {};

        const projectId = _.chain($window.location.href.split('/'))
            .filter(x => x)
            .last()
            .value();

        Markdown($scope);

        $scope.saveChanges = async () => {
            await $http.put(`/projects/update/${projectId}`, {
                id: projectId,
                title: $scope.title,
                description: $scope.editor.src,
                categories: $scope.categories
            });

            $window.location.href = '/projects';
        };

        self.init = async () => {
            const {data: {title, description, categories}} = await $http.get(`/projects/${projectId}/json`);
            $scope.title = title;
            $scope.editor.src = description;
            $scope.categories = categories;
            $scope.$apply();
        };

        self.init().then();
    }]);