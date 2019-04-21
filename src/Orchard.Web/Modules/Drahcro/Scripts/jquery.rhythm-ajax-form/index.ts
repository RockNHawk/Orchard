"use strict";
/**
 * jQuery plugin for posting form into iframe (support post input[type='file']).
 * author: laurel 664856248@qq.com
**/

/// <reference path="../jquery.d.ts" />
/// <reference path="../base/index.ts" />

module RhythmAjaxForm {

    export class ModelStateUtility {
        public static getValdationMsg($contanier: JQuery, key): JQuery {
            var selector = "span[data-valmsg-for='" + key + "']";
            var $valmsg = $contanier.find(selector);
            // 如果本form内找不到，就到document范围内找
            return $valmsg.length ? $valmsg : null;
        }

        public static getElementsByAttribute($contanier: JQuery, attrbuteName: string, key: string): JQuery {
            var keyLower = key.toLowerCase();;
            return $contanier.find("*[" + attrbuteName + "]").filter(function () {
                var name = this[attrbuteName] || $(this).attr(attrbuteName);
                return name && name.toLowerCase() == keyLower;
            });
        }

        public static getInput($contanier: JQuery, key: string): JQuery {
            //var $input = $contanier.find("*[name='" + key + "']");
            var $input = ModelStateUtility.getElementsByAttribute($contanier, "name", key);
            if ($input.length) {
                return $input;
            }
            // filedName[0],filedName[1]....
            if (key.indexOf('[') != -1 && key.indexOf(']') != -1) {
                //var key = "xxxxxxx---x[1439][99443439]";
                var arrayIndexRegExp = new RegExp("\\[\\d*\\]", "ig");
                // 如果有多个“[1][2]”取得最后一个
                var lastArrayIndex = key.lastIndexOf("[");
                var indexMatchResult = arrayIndexRegExp.exec(key.substring(lastArrayIndex))[0];
                var inputName = key.substring(0, lastArrayIndex);
                var inputIndex: number = parseInt(indexMatchResult.substring(1, indexMatchResult.indexOf("]")));
                //alert(arrayIndex);
                //alert(key.length);
                //alert(lastArrayIndex);
                //alert(key.substring(0, lastArrayIndex));
                var $inputs = ModelStateUtility.getElementsByAttribute($contanier, "name", inputName);
                if ($inputs.length > inputIndex) {
                    return jQuery($inputs[inputIndex]);
                }
            }
            return null;
        }

        public static getModelStateKeys($form: JQuery): Array<string> {
            var keys = $form.data("modelStateKeys");
            if (keys == null) {
                keys = [];
                $form.data("modelStateKeys", keys);
            }
            return keys;
        }

        public static removeModelState(modelStateKeys: Array<string>, $contanier: JQuery) {
            var $doc = $(document);
            // 移除之前 ModelSatet 验证不通过的提示层
            for (var i = 0; i < modelStateKeys.length; i++) {
                var key: string = modelStateKeys[i];
                var $input = ModelStateUtility.getInput($contanier, key) || ModelStateUtility.getInput($doc, key);
                var $valmsg = ModelStateUtility.getValdationMsg($contanier, key) || ModelStateUtility.getValdationMsg($doc, key);
                debugger

                if ($valmsg) {
                    $valmsg.html("").removeClass("field-validation-error");
                }
                if ($input) {
                    $input.closest(".form-group").addClass("has-error");
                }
            }
            modelStateKeys.length = 0;
        }

        // 处理 MVC 返回的 ModelState 验证信息。
        public static showModelState(response: AjaxResponse, context: AjaxContext) {
            var $form: JQuery = $(context.form);
            var modelState = response.modelState;
            var previusModelStateKeys = ModelStateUtility.getModelStateKeys($form);
            ModelStateUtility.removeModelState(previusModelStateKeys, $form);
            if (!modelState) {
                return;
            }
            var $doc = $(document);
            // 处理 MVC ModelState 验证结果。
            var nonSpanDisplayErrors = [];
            for (var key in modelState) {
                if (key == "__proto__") {
                    continue;
                }
                var errors = modelState[key].Errors;
                if (!errors || errors.length == 0) {
                    continue;
                }
                previusModelStateKeys.push(key);
                var firstError = errors[0];
                var errorMessage = firstError.ErrorMessage || (firstError.Exception ? firstError.Exception.Message : null);

                var $input = ModelStateUtility.getInput($form, key) || ModelStateUtility.getInput($doc, key);
                var $valmsg = ModelStateUtility.getValdationMsg($form, key) || ModelStateUtility.getValdationMsg($doc, key);
                //debugger;
                //alert($input.length);
                //var $input = $("input[name='" + key + "'],textarea[]");
                // 如果 界面上有 data-valmsg-for 提示层
                if ($valmsg && $valmsg.length > 0) {
                    $valmsg
                        .removeClass("field-validation-valid")
                        .addClass("field-validation-error")
                        .html(errorMessage);
                    //测试
                    $input.closest(".form-group").addClass("has-error");
                } else if ($input && $input.length > 0) {
                    // 如果没有提示层，则创建提示层
                    $valmsg = $("<span class='field-validation-error' />");
                    $valmsg.attr("data-valmsg-for", key);
                    $valmsg.html(errorMessage);
                    if ($input[0].tagName == "SELECT") {
                        $input.next(".select2-container").after($valmsg);
                    } else {
                        $input.after($valmsg);
                    }
                    //测试
                    $input.closest(".form-group").addClass("has-error");
                } else {
                    nonSpanDisplayErrors.push(errorMessage);
                }
            }
            if (nonSpanDisplayErrors.length) {
                var msg = "";
                for (var i = 0; i < nonSpanDisplayErrors.length; i++) {
                    if (nonSpanDisplayErrors[i]) {
                        msg += nonSpanDisplayErrors[i] + "\r\n";
                    }
                }
                if (msg) {
                    alert(msg);
                }
            }
        }
    }/* eof ModelStateUtility */

    export enum StatusCodes {
        ok = 200,
        unauthorized = 401,
        forbidden = 403,
        notFound = 404,
        methodNotAllowed = 405,
        internalServerError = 500
    }
    // 不能是 enum，因为服务端返回的是success、failure字符串
    var OperationStatus = {
        success: "success",
        failure: "failure"
    };
    //export enum OperationStatus {
    //    success= 0,
    //    failure= 1
    //}
    export interface AjaxFormOptions {
        isDisabled?: boolean,
        noIframe?: boolean,
        url?: string,
        callbacks?: AjaxCallbacks,
    }
    export interface AjaxCallbacks {
        //target?: HTMLIFrameElement;
        submit?: (context: AjaxContext) => void;
        lock?: (isLock: boolean, context: AjaxContext) => void;
        cancel?: (context: AjaxContext) => void;
        load?: (context: AjaxContext) => void;
        showModelState?: (context: AjaxContext) => void;
        success?: (data: any, context: AjaxContext) => void;
        //redirect?: (data: any, context: AjaxContext) => void;
        failure?: (data: any, context: AjaxContext) => void;
        unauthorized?: (data: any, context: AjaxContext) => void;
        error?: (data: any, context: AjaxContext) => void;
        dispose?: (context: AjaxContext) => void;
    }
    export interface AjaxResponse {
        contentDocument?: Document;
        //contentType?: string;
        statusCode?: StatusCodes;
        operationStatus?;
        message?: string;
        redirect?: string;
        messageShowTime?: Number;
        modelState?: any;
        datas?: any;
        customHtml?: string;
    }
    export interface AjaxContext {
        target: HTMLIFrameElement;
        form: HTMLFormElement;
        url: string;
        options: AjaxFormOptions;
        response?: AjaxResponse;
    }

    export class Utility {

        public static parseResponseContent(responseContextText: String, resp: AjaxResponse): AjaxResponse {
            resp = resp || {};
            //var responseContextText = {
            //    "modelState": {
            //        "Code": { "Value": null, "Errors": [{ "Exception": null, "ErrorMessage": "工号必须要填写" }] },
            //        "Name": { "Value": null, "Errors": [{ "Exception": null, "ErrorMessage": "姓名必须要填写" }] },
            //        "Password": { "Value": null, "Errors": [{ "Exception": null, "ErrorMessage": "密码必须要填写" }] }
            //    },
            //    "succeed": true, "operationStatus": "success", "message": "保存成功", "redirect": "/"
            //};
            //try {
            var respJson: AjaxResponse = responseContextText ? Drahcro.ObjectUtility.parseJSON(responseContextText) : {};
            respJson.modelState = respJson.modelState && respJson.modelState.length ? Drahcro.ObjectUtility.parseJSON(Drahcro.Base64Utility.decode(respJson.modelState)) : null;
            respJson.datas = respJson.datas && respJson.datas.length ? Drahcro.ObjectUtility.parseJSON(Drahcro.Base64Utility.decode(respJson.datas)) : null;
            $.extend(resp, respJson.datas);/* 为了兼容之前的代码 */
            $.extend(resp, respJson);
            //} catch (e) {
            //    Handler.dispose.apply(this, [context]);
            //    return;
            //}
            if (window.console) {
                console.log("ajax from response json:");
                console.log(resp);
            }
            // fix opera
            // 服务端返回 & ，但填入 html node dom后，opera 自动修改为 &amp;
            if (resp.redirect) {
                resp.redirect = resp.redirect.replace('&amp;', "&");
            }
            return resp;
        }

        //public static isErrorResponse(responseContent: String): Boolean {
        //  return responseContent && responseContent.length && responseContent.trim().indexOf("<!DOCTYPE html>") == 0
        //}

        public static createIframe(id): JQuery {
            return $('<iframe id="' + id + '" name="' + id + '" class="rhythm-ajax-iframe" style="display:none"  src="javascript:void(0)" />').appendTo("body");
            ////return $('<iframe id="' + id + '" name="' + id + '" class="rhythm-ajax-iframe" style="display:none"  src="http://120.55.76.16:8011/CrossDomainProxy.aspx" />').appendTo("body");
        }

        public static isRequireLock(lock: any): Boolean {
            //return (typeof (lock) == typeof (Boolean) && lock) || typeof (lock) != "undefined";
            return typeof (lock) != "undefined" && lock;
        }


        public static isStartsWithDoctype(str: string): Boolean {
            var docType = "<!DOCTYPE HTML>";
            if (!str || str.length < docType.length) {
                return false;
            }
            var docIndex = str.toUpperCase().indexOf(docType);
            if (docIndex == 0) {
                return true;
            }
            if (docIndex == -1) {
                return false;
            }
            //var htmlComment = str.substring(0, docIndex).trim();
            //if (htmlComment.lastIndexOf("-->") != (htmlComment.length - "-->".length)) {
            if (str.indexOf("-->") > docIndex) {
                return false;
            }
            return true;
        }

    }/* eof Utility */

    /**
    * 默认的服务器返回结果处理程序，外部也可以自定义、替换处理程序。
    */
    export class Handler implements AjaxCallbacks {

        public static cancel(context: AjaxContext) {
            var args = arguments;
            var options = context.options;
            var callbacks = options && options.callbacks;
            try {
                if (callbacks && callbacks.cancel) {
                    callbacks.cancel.apply(this, args);
                }
            } finally {
                Handler.dispose.apply(this, [context]);
            }
        }
        public static load(response, context) { }
        public static success(response, context) {
            if (response.message) {
                alert("success:\r\n" + response.message);
            }
        }
        public static failure(response, context) {
            if (response.message) {
                alert("failure:\r\n" + response.message);
            }
        }
        public static unauthorized(response, context) {
            if (response.message) {
                alert("failure:\r\n" + response.message);
            }
        }
        public static error(response, context) {
            if (response.message) {
                alert("error:\r\n" + response.message);
            }
        }
        public static dispose(context: AjaxContext) {
            var options = context.options;
            var callbacks = options && options.callbacks;
            if (Utility.isRequireLock(callbacks.lock)) {
                callbacks.lock.apply(this, [false, context]);
            }
            if (callbacks && callbacks.dispose) {
                callbacks.dispose.apply(this, arguments);
            }
            if ($.fn.ajaxForm.current == context) {
                $.fn.ajaxForm.current = null;
            }
        }

        public static handle(context: AjaxContext) {
            var resp: AjaxResponse = context.response;
            if (resp.customHtml) {
                // TODO: 服务端应该base64 编码一下 html code 再传输， 因为 html code 中可能存在意外符号
                $(document.body).append(Drahcro.Base64Utility.decode(resp.customHtml));
            }

            var form: HTMLFormElement = context.form;
            var options = context.options;// || {};
            var callbacks = (options && options.callbacks) || {};
            var resp: AjaxResponse = context.response;
            var args = [resp, context];

            Handler.load.apply(form, args);
            if (callbacks.load) {
                callbacks.load.apply(form, args);
            }

            if (!callbacks.showModelState || callbacks.showModelState.apply(form, args)) {
                // 处理 MVC 返回的 ModelState 验证信息。
                ModelStateUtility.showModelState.apply(form, args);
            }

            switch (resp.statusCode) {
                // 如果是 200 OK （服务器成功响应）
                case StatusCodes.ok:
                    switch (resp.operationStatus) {
                        case OperationStatus.success:
                            Handler.success.apply(form, args);
                            if (callbacks.success) {
                                callbacks.success.apply(form, args);
                            }
                            if (resp.redirect) {
                                setTimeout(function () {
                                    // 当前页面
                                    if (resp.redirect == ".") {
                                        // 刷新页面
                                        window.location.reload(false);
                                        // TODO: TEST & # 锚点处理
                                        //window.location.href = window.location.href;
                                    } else {
                                        //window.location.replace();
                                        window.location.href = resp.redirect;
                                    }
                                }, <any>(resp.messageShowTime ? resp.messageShowTime : (resp.message ? 1500 : 1)));
                            }
                            break;
                        case OperationStatus.failure:
                        default:
                            Handler.failure.apply(form, args);
                            if (callbacks.failure) {
                                callbacks.failure.apply(form, args);
                            }
                            break;
                    }
                    break;
                case StatusCodes.unauthorized:
                    Handler.unauthorized.apply(form, args);
                    if (callbacks.unauthorized) {
                        callbacks.unauthorized.apply(form, args);
                    }
                    break;
                // 服务器内部错误
                case StatusCodes.internalServerError:
                default:
                    Handler.error.apply(form, args);
                    if (callbacks.error) {
                        callbacks.error.apply(form, args);
                    }
                    break;
            }
        }


    }/* eof Handler */


    ; (function ($: JQueryStatic, doc: HTMLDocument, history: History) {

        //var historyLength = 0;

        $.fn.ajaxForm = function (options: AjaxFormOptions) {
            var $els = this;
            for (var i = 0; i < $els.length; i++) {
                var form: HTMLFormElement = $els[i];
                options = bindFormAjax($, form, options, history);
            }
            return this;
        };

        $.fn.ajaxForm.handler = Handler;
        $.fn.ajaxForm.current = null;
        $.fn.ajaxForm.crossDomain = false;

    })(jQuery, document, history);/* eof jQuery ajaxForm */



    function bindFormAjax($: JQueryStatic, form: HTMLFormElement, options: AjaxFormOptions, history: History) {
        var $form = $(form);
        $("<input value='XMLHttpRequest' name='X-Requested-With' type='hidden' /><input value='RhythmAjaxForm' name='X-Requested-With-Rhythm-Ajax-Form' type='hidden' />").appendTo($form);

        // 获取自定义 options
        var inlineOptionsAttr = $form.attr("on-ajax") || $form.attr("fn-ajax");
        var inlineOptions: AjaxFormOptions = inlineOptionsAttr ? Drahcro.ObjectUtility.parseJSON(Drahcro.StringUtility.htmlDecode(inlineOptionsAttr)) : null;
        var _options: AjaxFormOptions = options ? (inlineOptions ? Drahcro.ObjectUtility.combineObject(options, inlineOptions) : options) : {};
        var callbacks = _options.callbacks

        $form.data("ajax.options", callbacks);
        (<any>form).ajaxOptions = callbacks;

        //var targetSelector = (form.target ? "#" + form.target : null) || options.target;
        //var $target: JQuery = targetSelector ? $(targetSelector) : Utility.createIframe("rhythm-ajax-from-iframe-" + $.now());
        var $target: JQuery = Utility.createIframe("rhythm-ajax-from-iframe-" + $.now());
        var target: HTMLIFrameElement = <any>($target[0]);
        //window.onpopstate = function () {
        //    alert(1);
        //}
        //$(window).on("popstate", function (e) {
        //    debugger
        //    historyLength++;
        //});
        // Submit listener.
        $form.submit(function () {
            //debugger
            if (_options.isDisabled) return;

            var form: HTMLFormElement = this;
            var originalTarget = form.target;
            form.target = target.id;
            setTimeout(function () {
                form.target = originalTarget;
            }, 1);

            var url = _options.url;
            if (url) {
                var originalUrl = $form.attr('action');
                $form.attr('action', url);
                setTimeout(function () {
                    $form.attr('action', originalUrl);
                }, 1);
            }

            var context: AjaxContext = {
                form: form,
                url: form.action,
                options: _options,
                target: target
            };
            //alert($(context.form).html());
            $.fn.ajaxForm.current = context;
            var validator = $form.data("validator");
            if (validator) {
                // 是否验本地 JS 证通过
                var isValid = validator.form();
                if (!isValid) {
                    Handler.cancel.apply(form, [context]);
                    return;
                }
            }
            // TODO: should try-finally
            if (callbacks.submit && callbacks.submit.apply(form, [context]) === false) {
                Handler.cancel.apply(form, [context]);
                return false;
            }
            if (Utility.isRequireLock(callbacks.lock)) {
                callbacks.lock.apply(this, [true, context]);
            }
            //target.contentWindow.onpopstate = function (e) {
            //    alert(2)
            //historyLength++;
            //};
            //Utility.isStartsWithHtml("<!doctype html>");
            //Utility.isStartsWithHtml("<!-- --><!doctype html>");
            //Utility.isStartsWithHtml("<!-- --><!-- -->\r\n<!doctype html>");
            // iframe document loaded
            $target.load(function () {
                //debugger;
                $target.unbind('load');
                //var $newIframe = Utility.createIframe(target.id);
                //options
                var iframe: HTMLIFrameElement = this;
                var contentWindow: Window = iframe.contentWindow;
                try {
                    //if (!window["cc"]) {
                    //    window["cc"] = contentWindow;
                    //} else {
                    //    alert("window eq:"+(window["cc"] == contentWindow?"1":"0"));
                    //}
                    var contentDocument: HTMLDocument = iframe.contentDocument || contentWindow.document; // cross domain will throw error
                    var resp: AjaxResponse = { contentDocument: contentDocument };
                    resp.operationStatus = OperationStatus.success;
                    context.response = resp;
                    //var $contentDocument: JQuery = $target.contents(); // ie6,7 not support contentDocument
                    //var contentDocument: HTMLDocument = <any>($contentDocument[0]);// cross domain will throw error
                    //var resp: AjaxResponse = { contentDocument: contentDocument };
                    //context.response = resp;
                    //resp.operationStatus = OperationStatus.success;
                    //var $contentBody = $contentDocument.find('body');
                    //if (!$contentBody.length) $contentBody = $contentDocument;
                    //var respPureText = $contentBody.text();
                    //if (respPureText && respPureText.length && !contentDocument.doctype) {
                    //    Utility.parseResponseContent(respPureText, resp);
                    //}
                    var root = contentDocument.documentElement;
                    var contentElement = (contentDocument.body || root);
                    var respHtml = root.outerHTML;
                    var respPureText = contentElement.innerText || contentElement.innerHTML;
                    var hasDocType = contentDocument.doctype || Utility.isStartsWithDoctype(respHtml);
                    if (respPureText && respPureText.length && (!hasDocType || (respPureText.indexOf("operationStatus") != -1) && respPureText.indexOf("statusCode") != -1)) {
                        Utility.parseResponseContent(respPureText, resp);
                    }
                    if (navigator.userAgent.indexOf("MicroMessenger") == -1) {
                        //  alert(1);
                        ////history.back();
                        //contentWindow.history.back();
                        //var html = root.innerHTML;
                        //$target.remove().appendTo(doc.body); // destory iframe, browser auto clean  history
                        //if (hasDocType) {
                        //// iframe remove 后重新添加，里面的内容会丢失，并且 contentWindow / contentDocument 引用会变化，
                        //// 为了让界面能够在服务端返回错误时把异常信息显示出来，这里把 remove 前的 html 复制到 reAppend 之后的 iframe 内，
                        //// 不过这里有副作用，如果 html 里面含 script 的话。
                        //// target.contentDocument.documentElement.innerHTML = html;
                        //    (<any>target).srcdoc = html;
                        //}
                    }
                    else {
                        // weixin bug, iframe destroyed still keep history
                        history.back();
                    }
                    Handler.handle(context);
                }
                finally {
                    Handler.dispose.apply(this, [context]);
                    //$target.remove();
                    //$target = Utility.createIframe("rhythm-ajax-from-iframe-" + $.now());
                    //target = <any>$target[0];
                    //form.target = target.id;
                }
                //history.length = history.length - 1;
                //setTimeout(function () {
                //    //history.back();
                //    if (contentWindow.history.length > 1) {
                //    contentWindow.history.back();
                //    }
                //}, 100);
            });
            //} else {
            //}
        });
        return _options;
    }
}