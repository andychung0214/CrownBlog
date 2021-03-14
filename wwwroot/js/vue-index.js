var app = new Vue({
    el: '#main',
    data: {
        articles: {},
        totalArticles: {},
        totalComments: {},
        sidebarTags: {},
        sidebarURL: '',
        sidebarDates: {},
        sidebarURL: '',
        pageIndex: 1,
        pageSize: 4,
        pageTotal: 0,
        articleTotal: 0,
        currentPage: 0,
        isActive: false,
        pageClass: true,
        pathName: '',
        inputKeyword: sessionStorage.getItem('searchKeyword') || '',
        keywordURL: '',
        targetCurrent: '',
        targetPrev: '',
        targetNext: ''
    },
    methods: {
        getPathStatus: function (lastPathName) {
            var context = this;

            context.pathName = window.location.pathname;

            if (context.pathName.indexOf(lastPathName) >= 0) {
                return true;
            } else {
                return false;
            }
        },
        dateFormat: function (date) {
            return moment(date).format('MMMM Do YYYY, h:mm:ss a');
        },
        changePageIndex: function (index) {
            var context = this;
            context.getArticles(index);
        },
        submitCreateArticle: function () {

            var context = this;

            var postUrl = '/api-mvc/blog/article';

            var ckeditorDesc = CKEDITOR.instances.formDesc.getData();


            var addModel = {
                Title: context.$refs.formTitle.value,
                Description: ckeditorDesc,
                CreateDt: context.$refs.formDate.value,
                Abstract: context.$refs.formAbstract.value
            };

            axios.post(postUrl, addModel)
                .then(function (response) {
                    console.log(response);
                    alert('add success');
                })
                .catch(function (error) {
                    console.log(error);
                    alert('something error');
                });
        },
        getSidebarTags: function () {
            var context = this;
            var url = '/api-entities/blog/all-tags?_=adv';

            axios.get(url).then(function (response) {
                context.sidebarTags = response.data;
            }).catch(function (error) {
                // handle error
                console.log(error);
            }).finally(function () {
                // always executed
            });
        },
        getSidebarDates: function () {
            var context = this;
            var url = '/api-entities/blog/all-dates?_=adv';

            axios.get(url).then(function (response) {
                context.sidebarDates = response.data;

            }).catch(function (error) {

            }).finally(function () {

            });

        },
        jumpCategory: function (categoryName) {
            //const url = new URL(location.href);
            //console.log(url.searchParams.get('categoryName')); // true
            location.href = "/blog/category?categoryName=" + categoryName;
        },
        jumpMonth: function (month) {
            location.href = "/blog/category?categoryMonth=" + month;
        },
        getTotalComments: function () {
            var context = this;
            var articlesUrl = '/api-entities/blog/total-messages?_=crown';

            axios.get(articlesUrl).then(function (response) {

                context.totalComments = response.data;
            })
                .catch(function (error) {
                    // handle error
                    console.log(error);
                })
                .finally(function () {
                    // always executed
                });
        },
        getTotalArticles: function () {
            var context = this;
            var articlesUrl = '/api-entities/blog/total-articles?_=crown';

            axios.get(articlesUrl).then(function (response) {

                context.totalArticles = response.data;
            }).catch(function (error) {
                // handle error
                    console.log(error);
            }).finally(function () {
                // always executed
            });
        },
        getArticles: function (currentPage, pageSize, enable_count) {
            var context = this;
            //var articlesUrl = '/api-mvc/blog/articles';
            var articlesUrl = '/api-entities/blog/articles?_=adv';

            context.pageIndex = currentPage || 1;
            articlesUrl += '&pageIndex=' + context.pageIndex;

            context.pageSize = context.pageSize || 4;
            articlesUrl += '&pageSize=' + context.pageSize;

            axios.get(articlesUrl, {
                params: {
                    page_index: context.pageIndex || 0,
                    page_size: context.pageSize || 4,
                    enable_count: context.enable_count || '1'
                }
            }).then(function (response) {
                // handle success
                //console.log(response);
                //response.data.map(function (articleResponse) {
                //    context.arrArticle.push(articleResponse.items);
                //});

                context.articles = response.data;

                context.pageIndex = currentPage;
                context.currentPage = currentPage;
                if (context.currentPage !== null && context.currentPage !== undefined) {
                    context.targetCurrent = '?page=' + context.currentPage;
                    context.targetPrev = '?page=' + (context.currentPage - 1);
                    context.targetNext = '?page=' + (context.currentPage + 1);
                }
                context.getTotalArticles();

                context.articleTotal = context.totalArticles.length;

                if (context.pageSize > 0 && !Number.isNaN(parseInt(context.articleTotal)) && !Number.isNaN(parseInt(context.pageSize))) {
                    context.pageTotal = Math.floor(context.articleTotal / context.pageSize);
                    context.pageTotal++;
                }

                isActive = true;

                context.totalCount = context.articleTotal;
            })
                .catch(function (error) {
                    // handle error
                    console.log(error);
                })
                .finally(function () {
                    // always executed
                });
        },
        viewDetail: function (id) {
            var context = this;

            var arrIds = [];
            var currentIndex = 0;
            var preId = '';
            var postId = ''

            for (var i = 0; i < context.totalArticles.length; i++) {
                if (context.totalArticles[i].Id.indexOf(id) >= 0) {
                    currentIndex = i;
                };
            };

            for (var i = 0; i < context.totalArticles.length; i++) {
                if (i === (currentIndex - 1)) {
                    preId = context.totalArticles[i].Id
                } else if (i === (currentIndex + 1)) {
                    postId = context.totalArticles[i].Id
                };
            };

            if (preId !== '' && preId !== undefined && postId !== '' && postId !== undefined) {
                window.location = '/blog/details/' + id + '?preId=' + preId + '&postId=' + postId;
            } else if (preId === '' || preId === undefined) {
                window.location = '/blog/details/' + id + '?postId=' + postId;
            } else if (postId === '' || postId === undefined) {
                window.location = '/blog/details/' + id + '?preId=' + preId;
            }
        }
    },
    created: function () {
        var context = this;

        context.getTotalArticles();
        context.getTotalComments();
    },
    mounted: function () {
        var context = this;

        context.getSidebarDates();
        context.getSidebarTags();
        context.getArticles(1);
    },
    computed: {
        isShowPagination: function () {
            return this.articleTotal > this.pageSize;
        },
        isLoading: function () {
            return this.articles.length > 0 ? false : true;
        }
    },
    filters: {
        moment: function (date) {
            //return moment(date).format('MMMM Do YYYY, h:mm:ss a');
            return moment(date).format('MMMM D,YYYY');
        }
    },
    watch: {
        'inputKeyword': function () {
            var context = this;
            sessionStorage.setItem('searchKeyword', context.inputKeyword);
            context.keywordURL = '/blog/category?keyword=' + context.inputKeyword;
        }
    }
})