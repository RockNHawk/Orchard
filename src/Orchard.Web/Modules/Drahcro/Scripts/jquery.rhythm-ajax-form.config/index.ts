"use strict";
/// <reference path="../jquery.rhythm-ajax-form/index.ts" />
declare var ModalEffects;


; (function ($: any) {


    $.showSuccess = function (options) {
        if (!options) {
            throw Error("options is null");
        }
        options = options.message ? options : { message: options }
        $.extend(options, { delay: 2000 }, options);

        var mdOpt: any = { theme: "success" };
        if (options.message) {
            mdOpt.title = options.message;
        }
        if (options.delay) {
            mdOpt.time = options.delay;
        }
        //debugger
        ModalEffects.show(mdOpt);
    };
    $.showFailure = function (options) {
        if (!options) {
            throw Error("options is null");
        }
        options = options.message ? options : { message: options }

        var mdOpt: any = { theme: "failure" };
        if (options.message) {
            mdOpt.title = options.message;
        }
        if (options.delay) {
            mdOpt.time = options.delay;
        }
        ModalEffects.show(mdOpt);
    };
    $.showError = function (options) {
        if (!options) {
            throw Error("options is null");
        }
        options = options.message ? options : { message: options }
        var mdOpt: any = { theme: "error" };
        if (options.message) {
            mdOpt.title = options.message;
        }
        if (options.delay) {
            mdOpt.time = options.delay;
        }
        ModalEffects.show(mdOpt);
    };



    // ajax Form  全局处理程序
    var handler: RhythmAjaxForm.AjaxHandlers = $.fn.ajaxForm.handler;
    handler.success = function (response, context) {
        //response.redirect = null;
        if (response.message) {
            $.showSuccess(response.message);
        }
    };
    handler.failure = function (response, context) {
        //alert("failureHandler:\r\n" + message);
        if (response.message) {
            $.showFailure(response.message);
        }
    };
    handler.unauthorized = function (response, context) {
        var url = response.signInUrl;
        if (response.message) {
            $.showError(response);
            if (url) {
                setTimeout(function () {
                    location.href = url
                }, 1500);
            }
        } else {
            if (url) location.href = url;
        }
    };
    handler.error = function (response, context) {
        if (response.message) {
            $.showError(response);
        } else {
            var $close = $('<a id="rhythm-ajax-iframe-error-close" style="position:absolute;z-index:9999;top:0;right:10px;height:10px;cursor:pointer;*cursor:hand;color:#000;bottom:10px;text-decoration:none;font-size:30px;font-weight:600;">X</a>');
            var tgt = context.target;
            var $tgt = $(tgt);
            if ($tgt.length) {
                //debugger
                tgt.style.display = "block";
                //$tgt.css("display", "block");
                $tgt.addClass("rhythm-ajax-iframe-error");
                $((tgt.contentDocument || tgt.contentWindow.document || $tgt.contents()[0]).body).append($close);

                $close.click(function () {
                    $tgt.slideUp(500);
                });

                // when esc pressed on parent window
                $(window).keypress(function (e) {
                    if (e.keyCode == 27) {
                        $tgt.slideUp(500);
                        $(window).unbind("keypress", arguments.callee)
                    }
                });

                // when esc pressed on ifrmae content window
                $(tgt.contentWindow || (<any>tgt).window).keypress(function (e) {
                    if (e.keyCode == 27) {
                        $tgt.slideUp(500);
                        $(tgt.contentWindow || (<any>tgt).window).unbind("keypress", arguments.callee)
                    }
                });
            }

            //$.showError({ message: (tgt.contentDocument || tgt.contentWindow.document).body, delay: 0 });
            //art.dialog({
            //    id: "ajax-error",
            //    title: false, lock: false, drag: true, background: '#EEEEEE', width: "100%",
            //    content: $(context.target).contents()[0].body
            //});
        }
    };

    function createDialog(id, message) {
        return {
            show: function () {

                $.blockUI({
                    message: '<i class="icon-spinner4 spinner"></i>',
                    timeout: 0,
                    overlayCSS: {
                        backgroundColor: '#fff',
                        opacity: 0.8,
                        cursor: 'wait'
                    },
                    css: {
                        border: 0,
                        padding: 0,
                        backgroundColor: 'transparent'
                    }
                });
            }
        };
    };

    var nullDialog = { show: function () { }, hide: function () { }, lock: function () { }, unlock: function () { }, content: function () { } };
    // 初始化为空，避免外部可能在 jQuery 还没有 document ready 的情况下调用
    var progressDialog: any = nullDialog;
    var redirectDialog: any = nullDialog;
    var refreshDialog: any = nullDialog;
    //var exceptionIdDialog = nullDialog;
    $(function () {
        if (typeof ($.blockUI) == "undefined") {
            console.warn("页面中不存在 $.blockUI JS 组件的引用，表单提交时将无法启动锁屏。");
            return;
        }
        progressDialog = createDialog("ajax-progress", "处理中...");
        redirectDialog = createDialog("ajax-redirect", "跳转中...");
        refreshDialog = createDialog("ajax-refresh", "正在刷新");
        //exceptionIdDialog = ;
    });

    var sl = function (isLock, context) {
        var el = $(context.form);
        if (isLock) {
            if (el.block) {
                el.block({
                    message: '<i class="icon-spinner4 spinner"></i>',
                    timeout: 0, //unblock after 2 seconds
                    overlayCSS: {
                        backgroundColor: '#fff',
                        opacity: 0.8,
                        cursor: 'wait'
                    },
                    css: {
                        border: 0,
                        padding: 0,
                        backgroundColor: 'transparent'
                    }
                });
            }
            //progressDialog.show();
        } else {
            if (el.unblock) {
                el.unblock();
            }
            // progressDialog.hide();
        }
    };
    var bl = function (isLock, context) {
        if (isLock) {
            setTimeout(function () {
                context.$disabledButtons = $(context.form).find("button,input[type='submit']").filter("*[disabled!='disabled']").addClass("_ajax_disabled").attr("disabled", true);
            }, 1);
        } else {
            if (typeof (context.$disabledButtons) != "undefined" && context.$disabledButtons.length) {
                context.$disabledButtons.removeClass("_ajax_disabled").attr("disabled", false);
            }
        }
    };

    var ajaxLocks = {
        screenLock: sl,
        buttonLock: bl,
        all: function (isLock, context) {
            sl.apply(this, arguments);
            bl.apply(this, arguments);
        }
    };
    window['ajaxLocks'] = ajaxLocks;


    var bindFormAsAjax = function (form, options?: RhythmAjaxForm.AjaxFormOptions) {
        var $form = $(form || this);
        //alert($form.length);
        // 默认全部 ajax 提交，如果 form 不要ajax提交，则需加一个“noajax” class
        if ($form.hasClass("noajax")) {
            return;
        }
        var defaultOptions: RhythmAjaxForm.AjaxFormOptions = {
            callbacks: {
                submit: function (context) {
                    //setTimeout(function () {
                    //    context.$disabledButtons = $form.find("button,input[type='submit']").filter("*[disabled!='disabled']").addClass("_ajax_disabled").attr("disabled", true);
                    //}, 1);
                },
                cancel: function (context) { },
                lock: function (isLock, context) {
                  //  debugger
                    return ajaxLocks.all.apply(this, arguments);
                },
                success: function (response, context) {
                    response = context.response;
                    if (response.redirect) {
                        // 过一会儿再解锁屏幕，避免用户狂点提交按钮
                        setTimeout(function () {
                            if (response && response.redirect) {
                                var isRefresh = (response.redirect == "." || response.redirect == window.location.href);
                                if (isRefresh) {
                                    refreshDialog.show();
                                } else {
                                    redirectDialog.show();
                                }
                            }
                        }, 500);
                    }
                },
                failure: function (response, context) {
                    //showAjaxError(response, context);
                },
                error: function (response, context) {
                    //showAjaxError(response, context);
                },
                dispose: function (context) {
                    //if (typeof (context.$disabledButtons) != "undefined" && context.$disabledButtons.length) {
                    //    context.$disabledButtons.removeClass("_ajax_disabled").attr("disabled", false);
                    //}
                }
            }
        };
        var opts = options ? $.extend(options, defaultOptions) : defaultOptions;
        $form.ajaxForm(opts);
    };

    var createAjaxForm = function (action, ajaxOptions: RhythmAjaxForm.AjaxFormOptions) {
        var $form = $("<form class='ajax-dynamic' action='" + action + "' method='post'></form>");
        if (ajaxOptions) {
            $form.attr("fn-ajax", ajaxOptions);
        }
        $(document.body).append($form);
        bindFormAsAjax($form);
        // 必须添加到 dom 后在添加 ajax class，否则会被 $("form.ajax").livequery 监听到，造成重复绑定
        return $form;
    };

    /* bind button click */
    $("a[fn-ajax]").livequery(function () {
        var $element = $(this);
        var href = $element.attr("href");
        var $form = createAjaxForm(href, $element.attr("fn-ajax"));
        $element.click(function (event) {
            $form.submit();
            return false;
        });
    });

    $("button[fn-form-ajax]").livequery(function () {
        var $element = $(this);
        var optionStr = $element.attr("fn-form-ajax");
        // debugger
        var options: RhythmAjaxForm.AjaxFormOptions = optionStr && JSON.parse(optionStr) || {};
        var form = this.form;
        if (form) {
            bindFormAsAjax(form, options);
            options.isDisabled = true;
            $element.click(function (event) {
                //  debugger
                options.isDisabled = false;
                setTimeout(function () {
                    //   debugger
                    options.isDisabled = true;
                }, 1);
                //$(form).submit();
                //return false;
            });
        }
    });

    // find ajax forms and bind
    $("form[fn-ajax]").livequery(bindFormAsAjax);


    (function () {
        //var o1 = { f: function () { alert("o1"); } };
        //var o2 = { f: function () { alert("o2"); } };
        //var od = $.merge(o1, o2);
        //od.f();
        //var currentFormButtons = [];
        var clickedButtons = [];
        $(".rhythm-form-button,[rhythm-form-button]").livequery(function () {
            /* TODO: url & data-form & fn-ajax 整合*/
            var btn = this;
            var $btn = $(this);
            var url = $btn.attr("data-url");
            var ajaxOptions = $btn.attr("fn-ajax") || $btn.attr("fn-ajax");
            //if (!ajaxOptions) {
            //    if (url) {
            //        $btn.click(function () {
            //            $btn.addClass("ajax-button-progress");
            //            window.location.href = url;
            //        });
            //    }
            //}
            if (url && !ajaxOptions) {
                $btn.click(function () {
                    $btn.addClass("ajax-button-progress");
                    window.location.href = url;
                });
                return;
            }
            var $form = url ? createAjaxForm(url, ajaxOptions)
                : $($btn.attr("data-rhythmform") || btn.form || document.getElementById("form") || "form:first");
            if (!$form || $form.length == 0) {
                return;
            }
            var options = $form[0].ajaxOptions || $form.data("ajax.options");
            var originalDispose = options ? options.dispose : null;
            $btn.click(function () {
                //alert("js s");
                var btnName = $btn.attr("name");
                var btnValue = $btn.attr("value");
                var buttonFormAction = $btn.attr("data-rhythmform-action");
                var formOriginalAction = $form.attr("action");
                if (buttonFormAction) {
                    if (formOriginalAction != buttonFormAction) {
                        $form.attr("data-original-action", formOriginalAction);
                    }
                    $form.attr("action", buttonFormAction);
                }
                //alert($form.prop("action"));
                //alert(formOriginalAction);
                //alert(buttonFormAction);
                var thisDispose = function () {
                    $btn.removeClass("ajax-button-progress");
                    if (buttonFormAction) {
                        $form.attr("action", formOriginalAction || "");
                    }
                    options.dispose = originalDispose;
                };
                // TODO: 替换的方式存在潜在缺陷，应定义为 event list
                if (originalDispose && originalDispose != thisDispose) {
                    options.dispose = function () {
                        thisDispose();
                        originalDispose.apply(this, arguments);
                    };
                } else {
                    options.dispose = thisDispose;
                }
                $btn.addClass("ajax-button-progress");
                if (btnName) {
                    var $namedInput = $form.find(Drahcro.StringUtility.format(":input[name='{0}']", btnName)).not("button[type!='submit']");
                    if ($namedInput.length) {
                        var $namedButton = $namedInput.filter("input[type='submit'],button[type='submit']");
                        if ($namedButton.length) {
                            $namedButton.val(btnValue);
                            $namedButton.click();
                        } else {
                            $namedInput.val(btnValue);
                            $form.submit();
                        }
                    } else {
                        $namedInput = $(Drahcro.StringUtility.format("<button type='submit' style='display:none;' name='{0}' value='{1}' ></button>", btnName, btnValue));
                        $form.append($namedInput);
                        $namedInput.click();
                    }
                } else {
                    $form.submit();
                }
            });

        });
    })();

    (function () {
        var validator = $.validator;
        if (!validator) { console.warn("$.validator not imported"); return; }
        var unobtrusive = validator.unobtrusive;
        if (!unobtrusive) { console.warn("$.validator.unobtrusive not imported"); return; }
        //debugger
        var options = unobtrusive.options || (unobtrusive.options = {});
        //options.errorClass = 'validation-error-label';
        //options.successClass = 'validation-valid-label';
        //options.validClass = "validation-valid-label";
        //options.highlight = function (element, errorClass) {
        //    debugger
        //    $(element).removeClass(errorClass);
        //};
        //options.unhighlight = function (element, errorClass) {
        //    debugger
        //    $(element).removeClass(errorClass);
        //};

        unobtrusive.listeners.push({
            error: function (form, label, input) {
                //input.removeClass(options.errorClass);
                label.addClass("validation-error-label");
            },
            success: function (form, label) {
                label.addClass("validation-valid-label").text("Successfully")
            }
        });

        //var handler = unobtrusive.handler;
        //if (!handler) { console.warn("$.validator.unobtrusive.handler undefinded"); return; }
        //var old = handler.onSuccess;
        //handler.onSuccess = function ($input, $valmsg) {
        //    old.apply(this, arguments);

        //}
    })();

})(jQuery);
