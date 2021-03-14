var app = new Vue({
    el: '#topHeader',
    data: {
        isActive: false,
        isNavClass: true,
        pathName: '',
        activeTab: '',
        active_el: 0
    },
    methods:  {
        getPathStatus: function (lastPathName) {
            var context = this;

            context.pathName = window.location.pathname;

            if (context.pathName.indexOf(lastPathName) >= 0) {
                context.isActive = true;
            } else {
                context.isActive = false;
            }
        },
        activate: function (el) {
            this.active_el = el;
            switch (el) {
                case 1:
                    location.href = "/blog/index";
                    break;
                case 2:
                    location.href = "/blog/archive";
                    break;
                case 3:
                    location.href = "/blog/category";
                    break;
                case 4:
                    location.href = "/blog/category";
                    break;
                case 5:
                    location.href = "/blog/contact";
                    break;
                default:
                    break;
            }
        }
    }
})