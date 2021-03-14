var app = new Vue({
    el: '#formEditArticle',
    data: {
        titleData: '',
        abstractData: '',
        descData: '',
        tagsData: '',
        dateData: '',
        bannerUrlData: ''
    },
    methods: {
        getArticleById: function () {
            var context = this;
            var articlesUrl = '/api-entities/blog/article?id=' + window.articleId;
            axios.get(articlesUrl)
                .then(function (response) {
                    context.titleData = response.data[0].Title;
                    context.descData = response.data[0].Description;
                    context.abstractData = response.data[0].Abstract;
                    context.articleUrl = response.data[0].Url;
                    context.bannerUrlData = response.data[0].BannerUrl;
                    context.dateData = moment(response.data[0].CreateDate).format('YYYY-MM-DD HH:mm:ss');

                })
                .catch(function (error) {
                    // handle error
                    console.log(error);
                })
                .finally(function () {
                    // always executed
                });
        },
        CancelEditArticle: function () {
            window.location = '/blog/Index'; 
        },
        SubmitEditArticle: function () {

            var context = this;
            var ckeditorDesc = CKEDITOR.instances.formDesc.getData();

            var editModel = {
                Title: context.$refs.formTitle.value,
                Description: ckeditorDesc,
                CreateDate: context.$refs.formDate.value,
                Abstract: context.$refs.formAbstract.value,
                BannerUrl: context.$refs.formBannerURL.value,
                Status: context.$refs.formStatus.value
            };


            var editUrl = '/api-entities/blog/article/' + window.articleId

            axios.put(editUrl, editModel)
                .then(function (response) {
                    console.log(response);
                    alert('edit success');
                })
                .catch(function (error) {
                    console.log(error);
                    alert('something error');
                });
        }
    },
    created: function () {
        this.getArticleById();
    }
})