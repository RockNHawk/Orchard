"use strict";
//var aaaa = 11;
//var handsontableHelper = {};
//(function (helper) {
var HandsontableCustomHelper = /** @class */ (function () {
    function HandsontableCustomHelper() {
    }
    HandsontableCustomHelper.prototype.isCellValueJsonExpr = function (d) {
        if (d && (typeof (d) === 'string' && d.indexOf('JSON::') === 0)) {
            return true;
        }
        else {
            return false;
        }
    };
    ;
    HandsontableCustomHelper.prototype.parseCellValueIfIsJsonExpr = function (d) {
        if (d && typeof (d) === 'string' && d.indexOf('JSON::') === 0) {
            try {
                var obj = JSON.parse(d.substring('JSON::'.length));
                return obj;
            }
            catch (e) {
                debugger;
            }
        }
        else {
            return d;
        }
    };
    ;
    HandsontableCustomHelper.prototype.wrapCellValueJson = function (obj) {
        if (!obj)
            return obj;
        // handsometable 的 cell value 除非首次通过 tableCfg.data 指定，否则不能为 object
        return 'JSON::' + JSON.stringify(obj);
        //  return obj;
    };
    ;
    /*
@renderer is like   Handsontable.renderers.AutocompleteRenderer
*/
    HandsontableCustomHelper.prototype.createRender = function (type, valueMaps, renderer) {
        /*

   /*
        https://github.com/valor-software/ng2-handsontable/issues/373
        not work well


        Native cell renderers
        https://handsontable.com/docs/7.0.0/tutorial-cell-function.html

       // $td= $(td);//.empty();//.addClass('htAutocomplete');
    //if(value != null){
    //var escaped = Handsontable.helper.stringify(value);
        // value must be a string type
       // var obj = JSON.parse(value);
       // debugger
       // $td.html(obj['Key']); //empty is needed because you are rendering to an existing cell
   // }
      //  $td.append('<div class="htAutocompleteArrow">▼</div>'); //empty is needed because you are rendering to an existing cell
      //  $td.append('▼'); //empty is needed because you are rendering to an existing cell

*/
        var _that = this;
        return function (instance, td, row, col, prop, value, cellProperties) {
            console.log('render [' + row + ',' + col + ']value', value);
            //if (isNullOrUndefined(value) && valueMaps) {
            //    debugger
            //   // value = valueMaps[row][col];
            //}
            if (value) {
                // debugger;
                try {
                    /*
                    if the data is from server (server JSON SerializeObject , value type is Object)
                    if the data is from local user inputed, calue type is string
                    */
                    var obj = value && (typeof value === 'string' ? _that.parseCellValueIfIsJsonExpr(value) : value);
                    // 
                    if (typeof (obj) == 'string') {
                        //   arguments[5] = valueArgument;
                        renderer.apply(instance, arguments);
                    }
                    else {
                        var key = obj['Key'];
                        var _type = type || (obj['BindType']);
                        var valueArgument = value;
                        ;
                        if (key) {
                            // arguments[5] is value argument, relace it will change the cell display text
                            // arguments[5] = key;
                            switch (_type) {
                                case 'DisplayName':
                                default:
                                    valueArgument = key;
                                    break;
                                case 'Value':
                                    if (valueMaps) {
                                        var memberValue = valueMaps[key];
                                        valueArgument = memberValue;
                                    }
                                    break;
                            }
                        }
                        else {
                            //  debugger
                            // $(td).html(val);
                            //return $("<th>"+value.Key+"</th>");
                        }
                        // some old value key is not null
                        switch (_type) {
                            default:
                                break;
                            /// 保存直接输入值，不绑定时的值，此时 BindType 为 "StaticValue"，其它字段为空
                            case 'StaticValue':
                                valueArgument = obj['StaticValue'];
                                break;
                        }
                        arguments[5] = valueArgument;
                        renderer.apply(instance, arguments);
                    }
                }
                catch (e) {
                    console.error(e);
                }
            }
            else {
                renderer.apply(instance, arguments);
            }
            //return td;
            return td;
        };
    };
    ;
    HandsontableCustomHelper.prototype.createNgBindController = function (tableCfg, option) {
        var _this = this;
        var uniqueId = option.uniqueId, AllCellValues = option.allCellValues, valueMaps = option.valueMaps, mergedCells = option.mergedCells, cellDropdownData = option.cellDropdownData, columnHeaderTexts = option.columnHeaderTexts;
        //  debugger
        var gridApp = angular.module('ValueBindGrid_' + uniqueId, []);
        var ngController = gridApp.controller('ValueBindGridController_' + uniqueId, function ($scope) {
            _this.scope = $scope;
            $scope.allCellValues_JSON = AllCellValues && JSON.stringify(AllCellValues);
            $scope.mergedCells_JSON = mergedCells && JSON.stringify(mergedCells);
            $scope.appplyTableChange = function (newData, mergedCellsNew) {
                //   debugger
                // console.log("mergedCellsCollection", mc);
                //debugger
                $scope.$apply(function () {
                    //scope.mergedCells = mergedCellsNew;
                    $scope.mergedCells_JSON = mergedCellsNew && JSON.stringify(mergedCellsNew);
                    if (newData) {
                        // debugger
                        for (var i = 0; i < newData.length; i++) {
                            var array = newData[i];
                            if (array) {
                                for (var j = 0; j < array.length; j++) {
                                    var d = array[j];
                                    if (d) {
                                        // 当 handsometable 首次初始化时（通常是刷新页面），其 cell value 从指定的数据源中读取（从数据库获取的数据序列号为 JSON Object），为对象
                                        if (MnUtil.isJson(d)) {
                                            // 无需更改
                                            // array[j] = d;//parseCellValueIfIsJsonExpr(d);
                                        }
                                        else if (this.isCellValueJsonExpr(d)) {
                                            // 用户下拉选择绑定 Expression 后，cell value 为 JSON Expr 
                                            array[j] = this.parseCellValueIfIsJsonExpr(d);
                                        }
                                        else {
                                            // 用户手动输入任意字符串
                                            array[j] = { BindType: "StaticValue", StaticValue: d };
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // console.info('newData', newData);
                    $scope.allCellValues_JSON = newData && JSON.stringify(newData);
                });
            };
            //$scope.AllCellValues = AllCellValues;
            //$scope.mergedCells = mergedCells;
            $scope.columnHeaderTexts_JSON = columnHeaderTexts && JSON.stringify(columnHeaderTexts);
            $scope.appplyHeaderChange = function (index, newText, headers) {
                $scope.columnHeaderTexts_JSON = headers && JSON.stringify(headers);
            };
        });
        var _that = this;
        ///*
        var afterOnCellMouseDown = function (event, coords, th) {
            // debugger
            // only allow column header edit , do not allow row header edit
            if (!coords)
                return;
            var instance = this, isCol = coords.row === -1, isRow = coords.col === -1;
            if (!(isCol || isRow))
                return;
            //if (!isCol) {
            //    return;
            //}
            var input = document.createElement('input'), rect = th.getBoundingClientRect(), addListeners = function (events, headers, index) {
                events.split(' ').forEach(function (e) {
                    input.addEventListener(e, function () {
                        var neText = headers[index] = input.value;
                        instance.updateSettings(isCol ? {
                            colHeaders: headers
                        } : {
                            rowHeaders: headers
                        });
                        // debugger
                        if (_that.scope)
                            _that.scope.appplyHeaderChange(index, neText, headers);
                        setTimeout(function () {
                            if (input.parentNode)
                                input.parentNode.removeChild(input);
                        });
                    });
                });
            }, appendInput = function () {
                input.setAttribute('type', 'text');
                input.style.cssText = '' +
                    'position:absolute;' +
                    'left:' + rect.left + 'px;' +
                    'top:' + rect.top + 'px;' +
                    'width:' + (rect.width - 4) + 'px;' +
                    'height:' + (rect.height - 4) + 'px;' +
                    'z-index:1060;';
                document.body.appendChild(input);
            };
            input.value = th.querySelector(isCol ? '.colHeader' : '.rowHeader').innerText;
            appendInput();
            setTimeout(function () {
                input.select();
                addListeners('change blur', instance[isCol ? 'getColHeader' : 'getRowHeader'](), coords[isCol ? 'col' : 'row']);
            });
        };
        tableCfg.afterOnCellMouseDown = afterOnCellMouseDown;
        //*/
        this.bindTableChange(uniqueId, tableCfg, null, function (tableInstance) {
            //    var scope = scopeAccesser();
            if (_this.scope) {
                //  debugger
                var mc = tableInstance.getPlugin('mergeCells');
                var mergedCellsCollection = mc.mergedCellsCollection;
                var mergedCellsNew = mergedCellsCollection && mergedCellsCollection.mergedCells;
                //// 源数据？
                // hot.getSourceData();
                // 用户编辑修改后的数据？
                var newData = tableInstance.getData();
                _this.scope.appplyTableChange(newData, mergedCellsNew);
            }
        });
        //  return () => scope;
    };
    HandsontableCustomHelper.prototype.bindTableChange = function (uniqueId, tableCfg, tableAccessor, changeCallback) {
        //['']
        var hooks = Handsontable.hooks.getRegistered()
            .filter(function (x) { return x.indexOf('Mouse') != -1 || x.indexOf('Scroll') != -1; });
        var girdTable;
        hooks.forEach(function (hook) {
            // console.log("hook", arguments);
            var exists = tableCfg['afterOnCellMouseDown'];
            tableCfg[hook] = function (tableInstance) {
                //debugger
                if (exists)
                    exists.apply(this, arguments);
                // if (tableInstance.getPlugin) {
                // this is handsometable instance
                changeCallback(this, arguments);
                //  }
            };
        });
    };
    ;
    HandsontableCustomHelper.prototype.createDesignTable = function (container, option) {
        var uniqueId = option.uniqueId, AllCellValues = option.allCellValues, valueMaps = option.valueMaps, mergedCells = option.mergedCells, cellDropdownData = option.cellDropdownData, columnHeaderTexts = option.columnHeaderTexts;
        var _that = this;
        var columnDef = {
            // renderer: Handsontable.renderers.AutocompleteRenderer,
            renderer: this.createRender(null, valueMaps, Handsontable.renderers.AutocompleteRenderer),
            type: 'handsontable',
            handsontable: {
                search: true,
                // https://handsontable.com/docs/7.0.0/demo-hiding-columns.html
                //columns: [0, 1],
                //columns: ['ContentPartName','MemberExpression'],
                //indicators: true,
                columns: [
                    { data: 'BindType' },
                    { data: 'ContentPartName' },
                    { data: 'MemberExpression' }
                ],
                colHeaders: ['Bind Type', 'Part Type', 'Member Name'],
                autoColumnSize: true,
                data: cellDropdownData,
                /*
                // https://github.com/handsontable/handsontable/issues/865
                renderer:function (instance, td, row, col, prop, value, cellProperties) {
            if(value != null){
            var escaped = Handsontable.helper.stringify(value);
                $(td).empty().append(value['ContentPartName']); //empty is needed because you are rendering to an existing cell
            }
            return td;
        },
                */
                getValue: function () {
                    var selection = this.getSelectedLast();
                    // debugger
                    // Get the row's data object, object is from BindingDefSources
                    var obj = this.getSourceDataAtRow(selection[0]);
                    /*
string ContentPartName { get; set; }
string MemberExpression { get; set; }
*/
                    var ret = _that.wrapCellValueJson(obj);
                    console.log('getValue', ret);
                    return ret;
                    //return 'JSON::'+JSON.stringify(obj);
                    //return obj.ContentPartName + '.' + obj.MemberExpression;
                    // the value only accept plain type
                    //return obj.Key;
                    //return obj;
                }
            }
        };
        var columns = [];
        if (columnHeaderTexts) {
            for (var i = 0; i < columnHeaderTexts.length; i++) {
                columns.push(columnDef);
            }
            debugger;
            // if columns length is 0,handsometable will throw Exception,if columns lenth less than data's column length , the column will not display. 
            if (columns.length == 0) {
                var firstRow = AllCellValues && AllCellValues[0];
                if (firstRow) {
                    for (var i = 0; i < firstRow.length; i++) {
                        columns.push(columnDef);
                    }
                }
                if (columns.length == 0) {
                    columns.push(columnDef);
                }
            }
        }
        //  debugger
        var tableCfg = {
            licenseKey: 'non-commercial-and-evaluation',
            //afterGetColHeader: function (col, TH) {
            //   // debugger
            //    // nothing for first column
            //    if (col == -1) {
            //        return;
            //    }
            //    var instance = this;
            //    // create input element
            //    var input = document.createElement('input');
            //    input.type = 'text';
            //    input.value = TH.firstChild.textContent;
            //    TH.appendChild(input);
            //    $(input).change(function (e) {
            //        debugger
            //        var headers = instance.getColHeader();
            //        headers[col] = input.value;
            //        instance.updateSettings({
            //            colHeaders: headers
            //        });
            //    });
            //    TH.style.position = 'relative';
            //    TH.firstChild.style.display = 'none';
            //},
            /*
             *
             * https://stackoverflow.com/questions/18348437/how-do-i-edit-the-header-text-of-a-handsontable
             *
             *
https://stackoverflow.com/questions/32212596/prevent-handsontable-cells-from-being-selected-on-column-header-click
*/
            //data: AllCellValues || Handsontable.helper.createSpreadsheetData(5, 2),
            data: AllCellValues,
            //  colWidths: [47, 47, 47, 47, 47, 47, 47, 47, 47, 47, 47, 47, 47, 47, 47, 47],
            rowHeaders: true,
            contextMenu: true,
            allowInsertColumn: true,
            allowInsertRow: true,
            // colHeaders: true,
            colHeaders: columnHeaderTexts,
            columns: columns,
            //  columns: [columnDef, columnDef, columnDef, columnDef],
            mergeCells: mergedCells || [],
        };
        //v https://handsontable.com/docs/7.0.0/tutorial-using-callbacks.html
        //var hooks = ['afterChange', 'afterRemoveRow', 'afterRemoveCol', 'afterMergeCells',' afterUnmergeCells'];
        // debugger
        var girdTable;
        //   var scopeAccesser = createNgBindController(uniqueId, tableCfg, () => girdTable, AllCellValues, mergedCells);
        var scopeAccesser = this.createNgBindController(tableCfg, option);
        girdTable = new Handsontable(container, tableCfg);
        return girdTable;
    };
    ;
    return HandsontableCustomHelper;
}());
//})(handsontableHelper);
var handsontableHelper = new HandsontableCustomHelper();
//# sourceMappingURL=handsontable_custom.js.map