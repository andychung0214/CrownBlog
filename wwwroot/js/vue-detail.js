var app = new Vue({
    el: '#sectionDetail',
    data: {
        articleTitle : '',
        articleDescription: '',
        articleAbstract: '',
        articleUrl: '',
        articleStatus: '',
        articleCreatedDate: '',
        articleBannerUrl: '',
        preArticleTitle: '',
        postArticleTitle: '',
        articlePrevUrl: '',
        articleNextUrl: '',
        pathName: '',
        commentName: '',
        commentContent: '',
        commentDate: ''
    },
    methods: {
        postComment: function () {
            alert('post comment');
        },
        getPathStatus: function (lastPathName) {
            var context = this;

            context.pathName = window.location.pathname;

            if (context.pathName.indexOf(lastPathName) >= 0) {
                return true;
            } else {
                return false;
            }
        },
        submitCreateMessage: function () {

            var context = this;
            var postUrl = '/api-entities/blog/message/' + window.articleId;;

            var addModel = {
                Name: context.$refs.formCommentName.value,
                Address: context.$refs.formCommentEmail.value,
                Subject: context.$refs.formCommentSubject.value,
                Comment: context.$refs.formCommentMessage.value
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
        getMessage: function () {
            var context = this;

            var articlesUrl = '/api-entities/blog/message?id=' + window.articleId;
            //var articlesUrl = '/api-entities/blog/article?id=' + sessionStorage["id"];


            axios.get(articlesUrl)
                .then(function (response) {
                    context.articleTitle = response.data[0].Title;
                    context.articleDescription = response.data[0].Description;
                    context.articleAbstract = response.data[0].Abstract;
                    context.articleUrl = response.data[0].Url;
                    context.articleBannerUrl = response.data[0].BannerUrl;
                    context.commentDate = moment(response.data[0].articleCreatedDate).format('YYYY-MM-DD HH:mm:ss');
                })
                .catch(function (error) {
                    // handle error
                    console.log(error);
                })
                .finally(function () {
                    // always executed
                });
        },
        getArticle: function () {
            var context = this;

            var articlesUrl = '/api-entities/blog/article?id=' + window.articleId;
            //var articlesUrl = '/api-entities/blog/article?id=' + sessionStorage["id"];


            axios.get(articlesUrl)
                .then(function (response) {
                    context.articleTitle = response.data[0].Title;
                    context.articleDescription = response.data[0].Description;
                    context.articleAbstract = response.data[0].Abstract;
                    context.articleUrl = response.data[0].Url;
                    context.articleBannerUrl = response.data[0].BannerUrl;
                    context.articleCreatedDate = moment(response.data[0].CreateDt).format('YYYY-MM-DD HH:mm:ss');
                    context.articlePrevUrl = response.data[0].BannerUrl;
                    context.articleNextUrl = response.data[0].BannerUrl;
                })
                .catch(function (error) {
                    // handle error
                    console.log(error);
                })
                .finally(function () {
                    // always executed
                });
        },
        getPreArticle: function () {
            var context = this;

            var articlesUrl = '/api-entities/blog/article?id=' + window.preArticleId;

            axios.get(articlesUrl)
                .then(function (response) {
                    context.preArticleTitle = response.data[0].Title;
                })
                .catch(function (error) {
                    // handle error
                    console.log(error);
                })
                .finally(function () {
                    // always executed
                });
        },
        getPostArticle: function () {
            var context = this;

            var articlesUrl = '/api-entities/blog/article?id=' + window.postArticleId;

            axios.get(articlesUrl)
                .then(function (response) {
                    context.postArticleTitle = response.data[0].Title;
                })
                .catch(function (error) {
                    // handle error
                    console.log(error);
                })
                .finally(function () {
                    // always executed
                });
        },
        viewDetail: function () {
            alert('click detail');
        },
        editDetail: function (id) {
            window.location = '/blog/edit/' + window.articleId;
        }
    },
    created: function () {
        this.getArticle();
        this.getPreArticle();
        this.getPostArticle();
    },
    filters: {
        moment: function (date) {
            return moment(date).format('LLL');
        }
    }
});