class BoardQuery {
    sort = 'Date';        // or Vote
    order = 'Descending'; // or Ascending
    index = 0;            // first page
    pageSize = 10;        // page size
    category = '';        // category
    keyword = '';         // keyword

    get page() {
        return this.index + 1;
    }

    set page(value) {
        this.index = value - 1;
    }
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
            const {data: {projects, pages, categories}} = await $http.get("/api/board", {params: $scope.query});
            $scope.projects = projects;
            $scope.pages = pages;
            $scope.availablePages = availablePages($scope.query.page, pages, self.paginationSize);
            $scope.categories = categories;
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
    }])
    .controller("markdownEditorController", ["$scope", function ($scope) {
        $scope.editor = {
            src: '',
            parsed: ''
        };
        
        $scope.preview = true;

        $scope.textChange = function () {
            $scope.editor.parsed = marked($scope.editor.src);
        };

        $scope.onPublish = function () {
            alert("Write your own publish script here!");
        };

        $scope.onEditor = function (param) {
            const sel = $scope.getSelection();
            let aUrl;
            switch (param) {
                case "bold":
                    if ($scope.hasSelection()) {
                        // enhance
                        $scope.insertText("**" + sel.text + "**", sel.start, sel.end);
                    } else {
                        // add new
                        $scope.insertPlacehodler("**bold**", 2, 2);
                    }
                    break;
                case "italic":
                    if ($scope.hasSelection()) {
                        // enhance
                        $scope.insertText("_" + sel.text + "_", sel.start, sel.end);
                    } else {
                        // add new
                        $scope.insertPlacehodler("_italic_", 1, 1);
                    }
                    break;
                case "underline":
                    if ($scope.hasSelection()) {
                        // enhance
                        $scope.insertText("<u>" + sel.text + "</u>", sel.start, sel.end);
                    } else {
                        // add new
                        $scope.insertPlacehodler("<u>underline</u>", 3, 4);
                    }
                    break;
                case "list":
                    sel.target.value += "\n";
                    $scope.insertPlacehodler("- First item", 2, 0);
                    break;
                case "list-2":
                    sel.target.value += "\n";
                    $scope.insertPlacehodler("1. First numbered item", 3, 0);
                    break;
                case "header":
                    sel.target.value += "\n";
                    $scope.insertPlacehodler("# Header", 2, 0);
                    break;
                case "url":

                    let iUrl = prompt("Enter URL here:");
                    if (iUrl === "") {
                        iUrl = "http://codedaily.vn";
                    }
                    sel.target.value += "\n";
                    // insert new
                    aUrl = "[text](" + iUrl + ")";
                    $scope.insertPlacehodler(aUrl, 1, iUrl.length + 3);

                    break;
                case "img":

                    iUrl = prompt("Enter image URL here:");
                    if (iUrl === "") {
                        iUrl = "http://codedaily.vn";
                    }
                    sel.target.value += "\n";
                    // insert new
                    aUrl = "![image text](" + iUrl + ")";
                    $scope.insertPlacehodler(aUrl, 2, iUrl.length + 3);

                    break;
                case "code":
                    if ($scope.hasSelection()) {
                        // enhance
                        $scope.insertText("`" + sel.text + "`", sel.start, sel.end);
                    } else {
                        // add new
                        sel.target.value += "\n";
                        $scope.insertPlacehodler("<pre class='brush: language'>code here</pre>", 19, 17);
                    }
                    break;
                case "horline":
                    sel.target.value += "\n---";
                    sel.target.focus();
                    break;
                case "quote":
                    if ($scope.hasSelection()) {
                        // enhance
                        $scope.insertText("> " + sel.text, sel.start, sel.end);
                    } else {
                        // add new
                        $scope.insertPlacehodler("> quote", 2, 0);
                    }
                    break;
                case "strikethrough":
                    if ($scope.hasSelection()) {
                        // enhance
                        $scope.insertText("~~" + sel.text + "~~", sel.start, sel.end);
                    } else {
                        // add new
                        $scope.insertPlacehodler("~~strikethrough~~", 2, 2);
                    }
                    break;
            }
        };

        $scope.hasSelection = function () {
            const ta = document.getElementById("mark-editor");
            return ta.selectionStart !== ta.textLength;

        };

        $scope.getSelection = function () {
            const ta = document.getElementById("mark-editor");

            return {
                target: ta,
                start: ta.selectionStart,
                end: ta.selectionEnd,
                text: ta.value.substring(ta.selectionStart, ta.selectionEnd)
            };
        };

        $scope.insertPlacehodler = function (text, padLeft, padRight) {
            const ta = document.getElementById("mark-editor");
            ta.focus();
            ta.value += text;
            ta.selectionStart = ta.textLength - text.length + padLeft;
            ta.selectionEnd = ta.textLength - padRight;

        };

        $scope.insertText = function (text, start, end) {
            const ta = document.getElementById("mark-editor");
            ta.focus();
            const leftText = ta.value.substring(0, start);
            const rightText = ta.value.substring(end);
            ta.value = leftText + text + rightText;
        };

    }]);