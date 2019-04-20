/**
 * modalEffects.js v1.0.0
 * http://www.codrops.com
 *
 * Licensed under the MIT license.
 * http://www.opensource.org/licenses/mit-license.php
 * 
 * Copyright 2013, Codrops
 * http://www.codrops.com
 */

var ModalEffects;
(function (ModalEffects) {
    var overlay;//= document.getElementById('md-overlay') || document.body;

    function removeModal(modal, hasPerspective) {
        classie.remove(modal, 'md-show');
        if (typeof (hasPerspective) == "boolean" ? hasPerspective : classie.has(modal, 'md-setperspective')) {
            classie.remove(document.documentElement, 'md-perspective');
        }
    }

    function showModalHandler(modal, overlay) {
        classie.add(modal, 'md-show');
        var hasPerspective = classie.has(modal, 'md-setperspective');
        //overlay.removeEventListener('click', removeModal);
        overlay.addEventListener('click', function () {
            removeModal(modal, hasPerspective);
            overlay.removeEventListener('click', arguments.callee);
        });

        if (hasPerspective) {
            setTimeout(function () {
                classie.add(document.documentElement, 'md-perspective');
            }, 25);
        }
    }

    ModalEffects.show = function (options) {
        //title, content, type
        var type = options.type || "stickyup";
        var theme = options.theme || "success";
        var modal = document.getElementById("modal-" + type);
        if (modal == null) {
            return;
        }
        var close = modal.querySelector('.md-close');
        var title = modal.querySelector('.md-content-title');
        var text = modal.querySelector('.md-content-text');

        classie.removeClass(modal, "success");
        classie.removeClass(modal, "failure");
        classie.removeClass(modal, "error");
        classie.addClass(modal, theme);

        classie.removeClass(overlay, "success");
        classie.removeClass(overlay, "failure");
        classie.removeClass(overlay, "error");
        classie.addClass(overlay, theme);

        if (options.title) {
            title.innerHTML = options.title;
        }
        if (options.text) {
            text.innerHTML = options.text;
        }
        close.addEventListener('click', function (ev) {
            ev.stopPropagation();
            removeModal(modal);
        });
        showModalHandler(modal, overlay);

        if (options.time) {
            setTimeout(function () {
                removeModal(modal);
            }, options.time);
        }
        return modal;
    };

    function initTrigger(overlay) {
        var triggers = document.querySelectorAll('.md-trigger');
        if (triggers.length == 0) {
            return;
        }
        [].slice.call(triggers).forEach(function (el, i) {
            var modal = document.getElementById(el.getAttribute('data-modal')),
                close = modal.querySelector('.md-close');

            el.addEventListener('click', function () {
                showModalHandler(modal, overlay);
            });

            close.addEventListener('click', function (ev) {
                ev.stopPropagation();
                removeModal(modal);
            });
        });
    }

    $(function () {
        overlay = document.getElementById('md-overlay') || document.body;
        initTrigger(overlay);
        //alert(overlay);
    });
})(ModalEffects || (ModalEffects = {}));
