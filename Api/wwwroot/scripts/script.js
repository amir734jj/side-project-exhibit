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

function MarkdownInput(self) {
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

function isMyOwnPost(user, project) {
    return user.id === project.user.id;
}

function isMyOwnComment(user, comment) {
    return user.id === comment.user.id;
}

class MarkDownToText {
    /* Using lodash escape implementation: https://github.com/lodash/lodash/blob/master/escape.js */
    escapeHtml = (string) => {
        return angular.element("<textarea/>").html(string).text();
    };
    blockFn = (block) => block + '\n';
    inlineFn = (text) => text;
    newlineFn = () => '\n';
    emptyFn = () => '';
    renderer = {
        // Block elements
        code: this.blockFn,
        blockquote: this.blockFn,
        html: this.emptyFn,
        heading: this.blockFn,
        hr: this.emptyFn,
        list: this.blockFn,
        listitem: (text) => this.blockFn(text),
        paragraph: this.blockFn,
        table: (header, body) => this.blockFn(header) + this.blockFn(body),
        tablerow: this.blockFn,
        tablecell: this.blockFn,
        // Inline elements
        strong: this.inlineFn,
        em: this.inlineFn,
        codespan: this.inlineFn,
        br: this.newlineFn,
        del: this.inlineFn,
        link: (_0, _1, text) => this.inlineFn(text),
        image: (_0, _2, text) => this.inlineFn(text),
        text: this.inlineFn,
    };

    /**
     * Converts markdown to plaintext. Accepts an option object with the following
     * fields:
     *
     *  - escapeHtml (default: true) Escapes HTML in the final string
     *  - gfp (default: true) Uses github flavor markdown (passed through to marked)
     *  - pedantic (default: false) Conform to markdown.pl (passed through to marked)
     *
     * @param markdown the markdown to convert
     * @param options  the options to apply
     * @returns the unmarked string (plain text)
     */
    markdownToTxt(markdown, options = {
        escapeHtml: true,
        gfm: true,
        pedantic: false,
    }) {
        if (markdown) {
            const unmarked = marked(markdown, {
                gfm: options.gfm,
                pedantic: options.pedantic,
                renderer: this.renderer,
            });
            if (options.escapeHtml) {
                return this.escapeHtml(unmarked);
            }
            return unmarked;
        }
        return '';
    }
}

angular.module('ideaBoardApp', ['ngSanitize', 'ngTagsInput', 'ui.toggle'])
    .constant('isAuthenticated', window.isAuthenticated)
    .constant('user', window.user)
    .directive('validateBeforeGoing', ["$window", function ($window) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                angular.element(element).on("click", (e) => {
                    if (!$window.confirm(attrs.message)) {
                        e.preventDefault();
                    }
                });
            }
        }
    }])
    .directive('toolTip', ["$window", function ($window) {
        return {
            restrict: 'A',
            scope: true,
            link: function (scope, element, attrs) {
                if (JSON.parse(attrs.toolTipCondition)) {
                    angular.element(element)
                        .attr("title", attrs.toolTipMessage)
                        .data("toggle", 'tooltip');
                } else {
                    angular.element(element)
                        .removeAttr("title")
                        .removeData("toggle", 'tooltip');
                }
            }
        }
    }])
    .controller('darkModeCtrl', ['$scope', "$window", function ($scope, $window) {
        $scope.darkMode = false;
        
        const darkStyleDom = angular.element('<link rel="stylesheet" href="/styles/themes/darkly.bootstrap.min.css" type="text/css" />');

        $window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', e => {
            $scope.darkMode = !!e.matches;
        });

        $scope.$watch('darkMode', function(newValue, oldValue) {
            if (newValue) {
                angular.element('head').append(darkStyleDom);
            } else {
                darkStyleDom.remove();
            }
        });

        if ($window.matchMedia && $window.matchMedia('(prefers-color-scheme: dark)').matches) {
            $scope.darkMode = true;
        }
    }])
    .controller('boardCtrl', ['$scope', '$http', 'isAuthenticated', 'user', async ($scope, $http, isAuthenticated, user) => {

        let self = {
            paginationSize: 5,
        };
        
        $scope.loading = true;
        $scope.initialized = false;
        $scope.availablePages = [];
        $scope.projects = [];
        $scope.page = 1;
        $scope.query = new BoardQuery();
        $scope.isAuthenticated = isAuthenticated;
        $scope.user = user;
        $scope.votesIntegerValue = votesIntegerValue;
        $scope.hasVoted = hasVoted;
        $scope.isMyOwnPost = isMyOwnPost;

        const markDownToText = new MarkDownToText();
        $scope.markdownToTxt = (...args) => _.take(markDownToText.markdownToTxt(...args).split("\n"), 5).join("\n");

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
            $scope.loading = false;
            $scope.$apply();
        };

        $scope.refresh = self.init;

        self.init().then();
    }])
    .controller("addProjectCtrl", ["$scope", "$window", "$http", function ($scope, $window, $http) {

        $scope.titleError = false;
        $scope.descriptionError = false;
        $scope.categoriesError = false;

        $scope.categories = [];
        $scope.loadTags = async (query) => {
            const {data} = await $http.get("/api/category");
            const categories = data.map(x => x.name).filter(x => x.toLowerCase().includes(query))
            return {data: categories};
        };

        MarkdownInput($scope);

        $scope.saveChanges = async ($event) => {
            $scope.titleError = !($scope.title && $scope.title.length >= 5);
            $scope.descriptionError = !($scope.editor.src && $scope.editor.src.length >= 20);
            $scope.categoriesError = !($scope.categories && $scope.categories.length >= 1);

            if ($scope.titleError || $scope.descriptionError || $scope.categoriesError) {
                $event.preventDefault();
                return false;
            }

            await $http.post('/projects/add', {
                title: $scope.title,
                description: $scope.editor.src,
                categories: $scope.categories
            });

            $window.location.href = '/projects';
        };

    }])
    .controller("updateProjectCtrl", ["$scope", "$window", "$http", function ($scope, $window, $http) {

        $scope.titleError = false;
        $scope.descriptionError = false;
        $scope.categoriesError = false;
        $scope.loading = true;

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

        MarkdownInput($scope);

        $scope.saveChanges = async ($event) => {
            $scope.titleError = !($scope.title && $scope.title.length >= 5);
            $scope.descriptionError = !($scope.editor.src && $scope.editor.src.length >= 20);
            $scope.categoriesError = !($scope.categories && $scope.categories.length >= 1);

            if ($scope.titleError || $scope.descriptionError || $scope.categoriesError) {
                $event.preventDefault();
                return false;
            }

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
            $scope.loading = false;
            $scope.$apply();
        };

        self.init().then();
    }])
    .controller("viewProjectCtrl", ["$scope", "$window", "$http", "isAuthenticated", function ($scope, $window, $http, isAuthenticated) {
        const self = {};

        const projectId = _.chain($window.location.href.split('/'))
            .filter(x => x)
            .last()
            .value();

        $scope.loading = true;
        $scope.comment = '';
        $scope.marked = marked;
        $scope.project = null;
        $scope.isAuthenticated = isAuthenticated;
        $scope.user = user;
        $scope.votesIntegerValue = votesIntegerValue;
        $scope.hasVoted = hasVoted;
        $scope.isMyOwnPost = isMyOwnPost;
        $scope.moment = moment;
        $scope.isMyOwnComment = isMyOwnComment;
        $scope.newCommentMode = true;   // false for edit mode
        self.editCommentId = null;
        $scope.isAuthenticated = isAuthenticated;
        $scope.hasError = false;

        $scope.vote = async (id, type) => {
            const {data} = await $http.post(`/api/board/vote/${id}/${type}`);
            $scope.project = data;
            $scope.$apply();
        };

        self.getProject = async () => {
            const {data: project} = await $http.get(`/api/project/${projectId}/`);
            $scope.project = project;
            $scope.loading = false;
            $scope.$apply();
        };

        $scope.addComment = async () => {
            $scope.hasError = !($scope.comment && $scope.comment.length >= 15);
            if ($scope.hasError) return;

            const {data} = $scope.newCommentMode ? await $http.post(`/api/board/comment/${projectId}`, {
                comment: $scope.comment
            }) : await $http.put(`/api/board/comment/${projectId}/${self.editCommentId}`, {
                comment: $scope.comment
            });
            $scope.newCommentMode = true;
            self.editCommentId = null;
            $scope.comment = '';
            $scope.project = data;
            $scope.$apply();
        };

        $scope.deleteComment = async (comment) => {
            const {data} = await $http.delete(`/api/board/comment/${projectId}/${comment.id}`);
            $scope.project = data;
            $scope.$apply();
        }

        $scope.editComment = async (comment) => {
            $scope.comment = comment.text;
            self.editCommentId = comment.id;
            $scope.newCommentMode = false;
            angular.element('html, body').animate({scrollTop: angular.element("#editCommentBlock").offset().top}, 'slow');
        }

        $scope.clear = () => {
            $scope.hasError = false;
            $scope.comment = '';
            $scope.newCommentMode = true;
        }

        self.init = self.getProject;

        self.init().then();
    }])
    .controller("adminUpdateProjectCtrl", ["$scope", "$window", "$http", function ($scope, $window, $http) {

        $scope.titleError = false;
        $scope.descriptionError = false;
        $scope.categoriesError = false;
        $scope.loading = true;

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

        MarkdownInput($scope);

        $scope.saveChanges = async ($event) => {
            $scope.titleError = !($scope.title && $scope.title.length >= 5);
            $scope.descriptionError = !($scope.editor.src && $scope.editor.src.length >= 20);
            $scope.categoriesError = !($scope.categories && $scope.categories.length >= 1);

            if ($scope.titleError || $scope.descriptionError || $scope.categoriesError) {
                $event.preventDefault();
                return false;
            }

            await $http.put(`/admin/project/${projectId}`, {
                id: projectId,
                title: $scope.title,
                description: $scope.editor.src,
                categories: $scope.categories
            });

            $window.location.href = '/admin';
        };

        self.init = async () => {
            const {data: {title, description, categories}} = await $http.get(`/admin/project/${projectId}/json`);
            $scope.title = title;
            $scope.editor.src = description;
            $scope.categories = categories;
            $scope.loading = false;
            $scope.$apply();
        };

        self.init().then();
    }]);