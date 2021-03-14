var app = new Vue({
    el: '#formAddArticle',
    data: {
        formData: {
            title: '',
            desc: ''
        },
        checkedTags: [],
        totalTags: [],
        addTag: '',
        createArticleId: ''
    },
    methods: {
        _uuid: function () {
            var d = Date.now();
            if (typeof performance !== 'undefined' && typeof performance.now === 'function') {
                d += performance.now(); //use high-precision timer if available
            }
            return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
                var r = (d + Math.random() * 16) % 16 | 0;
                d = Math.floor(d / 16);
                return (c === 'x' ? r : (r & 0x3 | 0x8)).toString(16);
            });
        },
        submitCreateTag: function () {

            var context = this;

            var postUrl = '/api-entities/blog/tag';

            var ckeditorDesc = CKEDITOR.instances.formDesc.getData();

            context.totalTags.map(function (item, index) {

                context.checkedTags.map(function (checkedItem, checkedIndex) {

                    var addModel = {
                        Name: item.Name,
                        Description: ckeditorDesc,
                        CreateDate: context.$refs.formDate.value,
                        Abstract: context.$refs.formAbstract.value,
                        BannerUrl: context.$refs.formBannerURL.value
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
                })

            })
        },
        submitCreateArticle: function () {

            var context = this;

            var postUrl = '/api-entities/blog/article';

            var ckeditorDesc = CKEDITOR.instances.formDesc.getData();

            var articleId = context._uuid();
            context.createArticleId = articleId;
            var addModel = {
                Id: articleId,
                Title: context.$refs.formTitle.value,
                Description: ckeditorDesc,
                CreateDate: context.$refs.formDate.value,
                Abstract: context.$refs.formAbstract.value,
                BannerUrl: context.$refs.formBannerURL.value,
                Status: context.$refs.formStatus.value
            };

            axios.post(postUrl, addModel)
                .then(function (response) {

                    var postTagUrl = '/api-entities/blog/tag';

                    context.checkedTags.map(function (item, index) {

                        var tagModel = {
                            TagID: context._uuid(),
                            ArticleID: context.createArticleId,
                            Name: item.Name,
                            Description: ckeditorDesc
                        };

                        axios.post(postTagUrl, tagModel)
                            .then(function (response) {
                                console.log(response);
                            })
                            .catch(function (error) {
                                console.log(error);
                                alert('something error');
                            });
                        alert('add success');
                    });

                })
                .catch(function (error) {
                    console.log(error);
                    alert('something error');
                });
        },
        getTotalTags: function () {
            var context = this;
            var getTagsUrl = '/api-entities/blog/tags';

            axios.get(getTagsUrl)
                .then(function (response) {

                    context.totalTags = response.data;
                })
                .catch(function (error) {
                    // handle error
                    console.log(error);
                })
                .finally(function () {
                    // always executed
                });
        },
        AddTag: function () {
            var context = this;

            //var tempTag = "";
            //context.addTag = tempTag;

            var tempArr = [];

            var isDouble = false;

            if (context.totalTags.length !== 0) {
                context.totalTags.map(function (item) {
                    tempArr.push(item);
                    if (item.Name === context.addTag) {
                        isDouble = true;
                    }
                })

                if (!isDouble) {
                    var tagModel = {
                        Title: '',
                        Description: '',
                        Name: context.addTag
                    };

                    tempArr.push(tagModel);
                    context.totalTags = tempArr;
                } else {
                    alert('Insert tag fail, the tag is double');
                }

            }

        }
    },
    created: function () {
        this.getTotalTags();
    }
})