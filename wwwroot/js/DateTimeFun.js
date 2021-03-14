function formatAMPM(date) {
    var hours = date.getHours();
    var minutes = date.getMinutes();
    var ampm = hours >= 12 ? 'pm' : 'am';
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    minutes = minutes < 10 ? '0' + minutes : minutes;
    var strTime = hours + ':' + minutes + " " + ampm;
    return strTime;
}

function GetUtcTime() {
    return new Date(new Date().toLocaleString("en-US", { timeZone: "Antarctica/Troll" }));
}

Date.prototype.subDays = function (days) {
    this.setDate(this.getDate() - days);
    return this;
}

Date.prototype.subHours = function (h) {
    this.setHours(this.getHours() - h);

    return this;
}

Date.prototype.subMins = function (m) {
    var result = new Date(this.getTime());
    result.setMinutes(result.getMinutes() - m);

    return result;
}

Date.prototype.subSecs = function (s) {
    var result = new Date(this.getTime());
    result.setSeconds(result.getSeconds() - s);

    return result;
}

Date.prototype.addDays = function (days) {
    this.setDate(this.getDate() + days);
    return this;
}

Date.prototype.addHours = function (h) {
    this.setHours(this.getHours() + h);

    return this;
}

Date.prototype.addMins = function (m) {
    var result = new Date(this.getTime());
    result.setMinutes(result.getMinutes() + m);

    return result;
}

Date.prototype.addSecs = function (s) {
    var result = new Date(this.getTime());
    result.setSeconds(result.getSeconds() + s);

    return result;
}

Date.prototype.format = function (fmt) {
    var o = {
        "M+": this.getMonth() + 1, //月份
        "d+": this.getDate(), //日
        "h+": this.getHours(), //小時
        "m+": this.getMinutes(), //分
        "s+": this.getSeconds(), //秒
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度
        "S": this.getMilliseconds(), //毫秒,
        'tt': this.getHours() < 12 ? 'am' : 'pm',
        'TT': this.getHours() < 12 ? 'AM' : 'PM'
    };
    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));

    return fmt;
}