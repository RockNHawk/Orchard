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

var MnUtil = {};
(function (util) {
    util.collection = {
        moveUpElement: function (arr, i) {
            if (i == 0) {
                return;
            }
            var el = arr[i];
            arr.splice(i - 1, 0, el);
            arr.splice(i + 1, 1);
            return arr;
        },
        moveDownElement: function (arr, i) {
            if (i == arr.length - 1) {
                return;
            }
            var el = arr[i];
            arr.splice(i + 2, 0, el);
            arr.splice(i, 1);
            return arr;
        }
    };

    util.isDefined = function (obj) {
        return typeof (obj) != 'undefined';
    };
    util.ifArray = function (arr) {
        return isDefined(arr) && arr.length;
    };

    util.func = {
        combine: function (func1, func2) {
            return function () {
                func1.apply(this, arguments);
                func2.apply(this, arguments);
            };
        }
    };

    util.waitDebugger = function (ret) {
        return typeof (obj) != 'undefined' ? ret : true;
    };
    util.isJson = function (obj) {
        return typeof (obj) == "object" && Object.prototype.toString.call(obj).toLowerCase() == "[object object]" && !obj.length;
    };
    util.parseJSONUnsafe = function (text) {
        if (isJson(text)) {
            return text;
        }
        try {
            return $.parseJSON(text);
        } catch (e) {
            return eval("(" + text + ")");
        }
    };
    util.htmlDecode = function (str) {
        return str ? str.replace(/&gt;/g, "&").replace(/&lt;/g, "<")
            .replace(/&gt;/g, ">").replace(/&nbsp;/g, " ")
            .replace(/'/g, "\'").replace(/&quot;/g, "\"")
            .replace("&#39;", "'") : "";
    };

    util.parseValueExpr = function (exprStr, thisArg) {
        if (exprStr == null) {
            return null;
        }
        var type = typeof (exprStr);
        if (type == undefined) {
            return null;
        }
        if (type != "string") {
            return exprStr;
        }
        var fn = new Function("return " + exprStr);
        return fn.apply(thisArg);
        //if (exprStr.startsWith("$") && exprStr.length > 1) {
        //    var fn = new Function("return " + exprStr.substring(1));
        //    return fn.apply(thisArg);
        //} else {
        //    return exprStr;
        //}
    };

    util.web = {
        webRoot: window.webRoot || "/",
        map: function (path) {
            if (!path) {
                return path;
            }
            var wr = WebPathUtility.webRoot;
            if (path.startsWith("~/")) {
                return wr + path.substring(2);
            } else if (path.startsWith("Attachments/")) {
                return wr + path;
            }
            return path;
        },
        // WebPathUtility.managementAction("actionName","controllerName",{Name:"test",Id:123})
        managementAction: function (actionName, controllerName, routeValues) {
            var ps = routeValues ? WebPathUtility.convertJsonToQueryString(routeValues) : null;
            return WebPathUtility.map("~/Management/" + controllerName + "/" + actionName) + ((ps ? ("?" + ps) : ps) || "");
        },
        convertJsonToQueryString: function (json, prefix) {
            //convertJsonToQueryString({ Name: 1, Children: [{ Age: 1 }, { Age: 2, Hobby: "eat" }], Info: { Age: 1, Height: 80 } })
            if (!json) return null;
            var str = "";
            for (var key in json) {
                var val = json[key];
                if (isJson(val)) {
                    str += WebPathUtility.convertJsonToQueryString(val, ((prefix || key) + "."));
                } else if (typeof (val) == "object" && ("length" in val)) {
                    for (var i = 0; i < val.length; i++) {
                        //debugger
                        str += WebPathUtility.convertJsonToQueryString(val[i], ((prefix || key) + "[" + i + "]."));
                    }
                }
                else {
                    str += "&" + ((prefix || "") + key) + "=" + val;
                }
            }
            return str ? str.substring(1) : str;
        }
    };

    var webPath = function (path) {
        if (!path) {
            return path;
        }
        if (path.startsWith("~/")) {
            return "/" + path.substring(2);
        } else if (path.startsWith("Attachments/")) {
            return "/" + path;
        }
        return path;
    };

    util.web.webPath = webPath;

})(MnUtil);