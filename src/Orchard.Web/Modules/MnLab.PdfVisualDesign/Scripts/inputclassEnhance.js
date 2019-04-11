//+function ($) {
function includeCss(filename) {
    var head = document.getElementsByTagName('head')[0];
    var link = document.createElement('link');
    link.href = filename;
    link.rel = 'Stylesheet';
    link.type = 'text/css';
    head.prepend(link)
}
//includeCss("../style/style.css")
//var emlist = document.styleSheets[0].cssRules || document.styleSheets[0].rules;
////window.onload = function () {
//
window.onload = function () {
    $("input[ng-model='element.htmlClass']").each(function (index, eunm) {

        includeCss("~/Scripts/handsontable/handsontable.css")
        var emlist = document.styleSheets[0].cssRules || document.styleSheets[0].rules;
        //根据输入匹配，显示下拉框
        eunm.selectedIndex = -1;
        eunm.focus();
        eunm.onkeyup = function (event) {
            var lul = $(eunm).after('<ul id="strul"></ul>');
            var ul = document.getElementById("strul");
            ul.style.position = "absolute";
            ul.style.top = (eunm.offsetTop + eunm.offsetHeight) + "px";
            ul.style.left = eunm.offsetLeft + "px";
            var arr = ["a", "aa", "aaa", "b", "bb", "bbb", "aab"];
            var len = arr.length;
            ul.innerHTML = "";
            ul.style.borderWidth = "1px";
            ul.style.borderStyle = "solid";
            ul.style.borderColor = "#999";
            $(ul).css("width", $(eunm).width())
            var value = eunm.value;
            if (value) {
                var reg = new RegExp("^" + value + "+");//当输入框为空的时候会报错
                for (var i = 0; i < len; i++) {
                    if (reg.test(arr[i])) {
                        var li = document.createElement("li");
                        //匹配部分变粗
                        var matchlen = value.length;
                        var string = arr[i].substr(matchlen);
                        li.innerHTML = "<strong>" + value + "</strong>" + string;
                        li.style.listStyle = "none";
                        ul.appendChild(li);
                    }
                }
            }
            //获取所有的li节点
            var li = document.getElementsByTagName("li");
            for (var j = 0, lilen = li.length; j < lilen; j++) {
                li[j].onmouseover = function () {
                    this.style.backgroundColor = "#ececec";
                }
                li[j].onmouseout = function () {
                    this.style.backgroundColor = "#fff";
                }
                li[j].onclick = function () {
                    eunm.value = this.innerText || this.textContent;
                    ul.innerHTML = "";
                    ul.style.border = "none";
                }
            }

            eunm.options = li;
            //与自带事件冲突
            //event = event || window.event;//获取事件
            //event.cancelBubble = true;

            ////var kc=event.keyCode||event.charCode;
            //if (event.keyCode == 38) {
            //    window.event.cancelBubble = true; 
            //}
            //if (event.preventDefault) {
            //    event.preventDefault();
            //} else {
            //    event.returnValue = false;
            //}
            //if (event.stopPropagation) {
            //    event.stopPropagation();
            //} else {
            //    event.cancelBubble = true;
            //}
            //switch (event.keyCode) {
            //    case 38://上箭头
            //        clearcolor(this);
            //        this.selectedIndex--;
            //        if (this.selectedIndex < 0) {
            //            this.selectedIndex = this.options.length - 1;
            //        }
            //        this.value=this.options[this.selectedIndex].innerHTML;
            //        paintcolor(this);
            //        break;
            //    case 40://下箭头
            //        clearcolor(this);
            //        this.selectedIndex++;
            //        if (this.selectedIndex > this.options.length - 1) {
            //            this.selectedIndex = 0;
            //        }
            //        this.value=this.options[this.selectedIndex].innerHTML;
            //        paintcolor(this);
            //        break;
            //    case 13://回车
            //        if (this.selectedIndex >= 0) {
            //            var str = this.options[this.selectedIndexa].innerHTML;
            //            this.value = str.replace(/[<strong><\/strong>]/g, "");
            //        }
            //        ul.innerHTML = "";
            //        ul.style.border = "none";
            //        break;
            //}

        }
    });
}
function clearcolor(target) {
    if (target.selectedIndex >= 0) {
        target.options[target.selectedIndex].style.backgroundColor = "#fff";
    }
}
function paintcolor(target) {
    if (target.selectedIndex >= 0) {
        target.options[target.selectedIndex].style.backgroundColor = "#ececec";
    }
}

//}