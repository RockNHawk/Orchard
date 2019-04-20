//var exports = {};


function isNullOrUndefined(x) {
    if (x === undefined) {
        return true;
    }
    if (x === null) {
        return true;
    }
    return false;
}

String.prototype.contains = function (str) { return (this.indexOf(str) > -1); };
String.prototype.trim = function (s) { if (s) return this.trimEnd(s).trimStart(s); else return this.replace(/(^[ \t\n\r]*)|([ \t\n\r]*$)/g, ''); };
String.prototype.trimEnd = function (s) { if (this.endsWith(s)) { return this.substring(0, this.length - s.length); } return this; };
String.prototype.trimStart = function (s) { if (this.startsWith(s)) { return this.slice(s.length); } return this; };
String.prototype.startsWith = function (str) { return (this.indexOf(str) == 0); };
String.prototype.endsWith = function (str) { return (str.length <= this.length && this.substr(this.length - str.length, str.length) == str); };
String.prototype.remove = function (start, l) { var str1 = this.substring(0, start); var str2 = this.substring(start + l, this.length); return str1 + str2; }
String.prototype.insert = function (index, str) { var str1 = this.substring(0, index); var str2 = this.substring(index, this.length); return str1 + str + str2; }
String.prototype.getHashCode = function () { var h = 31; var i = 0; var l = this.length; while (i < l) h ^= (h << 5) + (h >> 2) + this.charCodeAt(i++); return h; }
String.isNullOrEmpty = function (str) { return str; };
String.format = function () { var str = arguments[0]; for (var i = 1; i < arguments.length; i++) { var reg = new RegExp("\\{" + (i - 1) + "\\}", "ig"); str = str.replace(reg, arguments[i]); } return str; };
Array.prototype.contains = function (val) { for (var i = 0; i < this.length; i++) { if (val == this[i]) return true; } return false; };
Array.prototype.clear = function () {
    this.splice(0, this.length);
    return this;
};
String.ellipsis = function (str, maxLength) {
    return str.length > maxLength ? str.substring(0, maxLength) + "..." : str;
};
Date.prototype.format = function (format) {
    var o = {
        "M+": this.getMonth() + 1, //month
        "d+": this.getDate(),    //day
        "h+": this.getHours(),   //hour
        "m+": this.getMinutes(), //minute
        "s+": this.getSeconds(), //second
        "q+": Math.floor((this.getMonth() + 3) / 3),  //quarter
        "S": this.getMilliseconds() //millisecond
    };
    if (/(y+)/.test(format)) format = format.replace(RegExp.$1,
        (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o) if (new RegExp("(" + k + ")").test(format))
        format = format.replace(RegExp.$1,
            RegExp.$1.length == 1 ? o[k] :
                ("00" + o[k]).substr(("" + o[k]).length));
    return format;
};