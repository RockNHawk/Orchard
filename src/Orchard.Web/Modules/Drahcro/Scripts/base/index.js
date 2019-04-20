"use strict";
/**
 * jQuery plugin for posting form into iframe (support post input[type='file']).
 * author: laurel 664856248@qq.com
**/
/// <reference path="../jquery.d.ts" />
function isNullOrUndefined(x) {
    if (x === undefined) {
        return true;
    }
    if (x === null) {
        return true;
    }
    return false;
}
var Drahcro;
(function (Drahcro) {
    /**
    *  Base64 encode / decode
    *  @author haitao.tu
    *  @date   2010-04-26
    *  @email  tuhaitao@foxmail.com
    */
    var Base64Utility = /** @class */ (function () {
        function Base64Utility() {
        }
        // private method for UTF-8 decoding  
        Base64Utility._utf8_decode = function (utftext) {
            var str = "";
            var i = 0;
            while (i < utftext.length) {
                var c = utftext.charCodeAt(i);
                if (c < 128) {
                    str += String.fromCharCode(c);
                    i++;
                }
                else if ((c > 191) && (c < 224)) {
                    var c2 = utftext.charCodeAt(i + 1);
                    str += String.fromCharCode(((c & 31) << 6) | (c2 & 63));
                    i += 2;
                }
                else {
                    var c2 = utftext.charCodeAt(i + 1);
                    var c3 = utftext.charCodeAt(i + 2);
                    str += String.fromCharCode(((c & 15) << 12) | ((c2 & 63) << 6) | (c3 & 63));
                    i += 3;
                }
            }
            return str;
        };
        // public method for decoding  
        Base64Utility.decode = function (input) {
            var output = "";
            var chr1, chr2, chr3;
            var enc1, enc2, enc3, enc4;
            var i = 0;
            input = input.replace(/[^A-Za-z0-9\+\/\=]/g, "");
            var keyStr = Base64Utility._keyStr;
            while (i < input.length) {
                enc1 = keyStr.indexOf(input.charAt(i++));
                enc2 = keyStr.indexOf(input.charAt(i++));
                enc3 = keyStr.indexOf(input.charAt(i++));
                enc4 = keyStr.indexOf(input.charAt(i++));
                chr1 = (enc1 << 2) | (enc2 >> 4);
                chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
                chr3 = ((enc3 & 3) << 6) | enc4;
                output = output + String.fromCharCode(chr1);
                if (enc3 != 64) {
                    output = output + String.fromCharCode(chr2);
                }
                if (enc4 != 64) {
                    output = output + String.fromCharCode(chr3);
                }
            }
            output = Base64Utility._utf8_decode(output);
            return output;
        };
        // private property  
        Base64Utility._keyStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
        return Base64Utility;
    }()); /* eof Base64Utility */
    Drahcro.Base64Utility = Base64Utility;
    var StringUtility = /** @class */ (function () {
        function StringUtility() {
        }
        StringUtility.format = function (format) {
            var args = [];
            for (var _i = 1; _i < arguments.length; _i++) {
                args[_i - 1] = arguments[_i];
            }
            for (var i = 0; i < args.length; i++) {
                var reg = new RegExp("\\{" + (i - 1) + "\\}", "ig");
                format = format.replace(reg, args[i] + "");
            }
            return format;
        };
        StringUtility.htmlDecode = function (str) {
            return str ? str.replace(/&gt;/g, "&").replace(/&lt;/g, "<")
                .replace(/&gt;/g, ">").replace(/&nbsp;/g, " ")
                .replace(/'/g, "\'").replace(/&quot;/g, "\"")
                .replace("&#39;", "'") : "";
        };
        return StringUtility;
    }());
    Drahcro.StringUtility = StringUtility;
    var FunctionUtility = /** @class */ (function () {
        function FunctionUtility() {
        }
        FunctionUtility.combine = function (func1, func2) {
            return function () {
                func1.apply(this, arguments);
                func2.apply(this, arguments);
            };
        };
        FunctionUtility.parseStringToFunction = function (exprStr, thisArg) {
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
        ;
        return FunctionUtility;
    }());
    Drahcro.FunctionUtility = FunctionUtility;
    var ObjectUtility = /** @class */ (function () {
        function ObjectUtility() {
        }
        ObjectUtility.isDefined = function (obj) {
            return typeof (obj) != 'undefined';
        };
        ObjectUtility.ifArray = function (arr) {
            return this.isDefined(arr) && arr.length;
        };
        /**
        *  判断一个对象是不是json
        */
        ObjectUtility.isJson = function (obj) {
            return typeof (obj) == "object" && Object.prototype.toString.call(obj).toLowerCase() == "[object object]" && !obj.length;
        };
        /// <summary>
        /// 转换成JSON
        /// </summary>
        ObjectUtility.parseJSON = function (text) {
            if (this.isJson(text)) {
                return text;
            }
            return $.parseJSON(text);
        };
        ObjectUtility.parseJSONUnsafe = function (text) {
            if (this.isJson(text)) {
                return text;
            }
            try {
                return $.parseJSON(text);
            }
            catch (e) {
                return eval("(" + text + ")");
            }
        };
        ObjectUtility.combineObject = function (target, obj1) {
            if (!target) {
                return obj1;
            }
            if (target == obj1) {
                return target;
            }
            for (var key in obj1) {
                target[key] = obj1[key];
            }
            return target;
        };
        return ObjectUtility;
    }()); /* eof Utility */
    Drahcro.ObjectUtility = ObjectUtility;
})(Drahcro || (Drahcro = {}));
//# sourceMappingURL=index.js.map