"use strict";


declare interface ValueBindingDef {
    ContentPartName?: any;
    MemberExpression?: any;
    DefaultValue?: any;
    StaticValue?: any;
    SetValue?: any;
    Description?: any;
    Remark?: any;
    BindType: string;
    Key?: any;
    DisplayName?: any;
}


declare interface HandsontableData {
    uniqueId: string;
    allCellValues: Array<Array<ValueBindingDef>>,
    valueMaps: any,
    mergedCells: [],
    cellDropdownData: [],
    columnHeaderTexts: Array<string>,
}

declare var Handsontable: any;
declare var angular: any;
declare var MnUtil: any;
declare var $: any;
//var aaaa = 11;


//var handsontableHelper = {};
//(function (helper) {

class HandsontableCustomHelper {


    isCellValueJsonExpr(d) {
        if (d && (typeof (d) === 'string' && d.indexOf('JSON::') === 0)) {
            return true;
        } else {
            return false;
        }
    };


    parseCellValueIfIsTextExpr(d: any) {
        if (d && typeof (d) === 'string' && d.indexOf('JSON::') === 0) {
            try {
                var obj = JSON.parse(d.substring('JSON::'.length));
                return obj;
            } catch (e) {
                debugger;
            }
        } else {
            return d;
        }
    };


    wrapCellValueJsonToTextExpr(obj) {
        if (!obj) return obj;

        // handsometable 的 cell value 除非首次通过 tableCfg.data 指定，否则不能为 object
        return 'JSON::' + JSON.stringify(obj);
        //  return obj;
    };

    getCellDisplayText(tableType, row, col, cellValueAccrssor: (row, col) => ValueBindingDef, valueMaps) {
        var valueDef = cellValueAccrssor(row, col);
        if (!valueDef) {
            return;
        }
        return this.getCellDisplayText2(tableType, valueDef, valueMaps);
    }

    getCellDisplayText2(tableType, valueDef: ValueBindingDef, valueMaps): string {
        if (!valueDef) {
            return;
        }
        // debugger;
        try {


            var key = valueDef['Key'];
            var DisplayName = valueDef['DisplayName'];
            var _type = valueDef['BindType'];
            var valueArgument: string;
            var memberValue = valueMaps && valueMaps[key] && valueMaps[key].Value;
            if (tableType =='ValueEdit') {
                if ('SetValue' in valueDef) {
                    memberValue = valueDef.SetValue;
                }
            }
            //switch (mode) {
            //    case 'BindingEdit':
            //    default:
            if (key) {
                // arguments[5] is value argument, relace it will change the cell display text
                // arguments[5] = key;
                switch (_type) {
                    case 'DisplayName':
                    default:
                        valueArgument = DisplayName || key;
                        break;
                  //  case 'SetValue':
                    case 'Value':
                        if (memberValue || tableType == 'ValueEdit') {
                            valueArgument = memberValue;
                        } else {
                            // debugger
                            valueArgument = `(${DisplayName})`;
                        }
                        break;
                }
            } else {
                switch (_type) {
                    default:
                        break;
                    /// 保存直接输入值，不绑定时的值，此时 BindType 为 "StaticValue"，其它字段为空
                    case 'StaticValue':
                        valueArgument = valueDef['StaticValue'];
                        break;
                }
            }

            // legacy fix: some old value key is not null
            switch (_type) {
                default:
                    break;
                /// 保存直接输入值，不绑定时的值，此时 BindType 为 "StaticValue"，其它字段为空
                case 'StaticValue':
                    valueArgument = valueDef['StaticValue'];
                    break;
            }

            // break;
            //    case 'ValueEdit':
            //        switch (_type) {
            //            case 'Value':
            //            default:
            //                valueArgument = memberValue;
            //                break;
            //            /// 保存直接输入值，不绑定时的值，此时 BindType 为 "StaticValue"，其它字段为空
            //            case 'StaticValue':
            //                valueArgument = obj['StaticValue'];
            //                break;
            //        }
            //        break;
            //}

            return <string><any>valueArgument;

        } catch (e) {
            console.error(e);
            throw e;
        }
    }


    /*
     * @renderer is like   Handsontable.renderers.AutocompleteRenderer
    // createRender(mode, cellValues: ValueBindingDef[][], valueMaps, renderer) {
     * */
    createRender(mode, cellValueAccrssor: (row, col) => ValueBindingDef, valueMaps, renderer) {
        var _that = this;
        return function (instance, td, row, col, prop, value, cellProperties) {
            if (!value) return;
            /*
           if the data is from server (server JSON SerializeObject , value type is Object)
           if the data is from local user inputed, calue type is string
           */
            var valueDef = value && (typeof value === 'string' ? _that.parseCellValueIfIsTextExpr(value) : value);
            var displayText;
            if (MnUtil.isJson(valueDef)) {
                displayText = _that.getCellDisplayText2(mode, valueDef, valueMaps);
            } else {
                // if (typeof (valueDef) == 'string') {
                displayText = value;
            }
            //var displayText = _that.getCellDisplayText(mode, row, col, cellValueAccrssor, valueMaps);
            //debugger
            arguments[5] = displayText;//|| ' ';
            renderer.apply(instance, arguments);
            return td;
        };
    };

    mode: string;

    private scope: any;
    sourceData: HandsontableData;
    changedData: HandsontableData;

    init(data: HandsontableData) {
        this.sourceData = data;
        var changed = this.changedData = { ...data };
        if (!changed.allCellValues) {
            changed.allCellValues = [];
        }
    }

    private createController(option: HandsontableData) {

        var uniqueId = option.uniqueId,
            allCellValues = option.allCellValues,
            valueMaps = option.valueMaps,
            mergedCells = option.mergedCells,
            cellDropdownData = option.cellDropdownData,
            columnHeaderTexts = option.columnHeaderTexts;


        //  debugger
        var gridApp = angular.module('ValueBindGrid_' + uniqueId, []);


        var _that = this;

        var ngController = gridApp.controller('ValueBindGridController_' + uniqueId, ($scope) => {
            this.scope = $scope;

            $scope.allCellValues_JSON = allCellValues && JSON.stringify(allCellValues);
            $scope.mergedCells_JSON = mergedCells && JSON.stringify(mergedCells);

            $scope.applyTableValueChange = (newData) => {
                $scope.$apply(() => {
                    $scope.allCellValues_JSON = newData && JSON.stringify(newData);
                });
            }

            $scope.applyTableMergeChange = (mergedCellsNew) => {
                //debugger
                $scope.$apply(() => {
                    $scope.mergedCells_JSON = mergedCellsNew && JSON.stringify(mergedCellsNew);
                });
            };

            //$scope.allCellValues = allCellValues;
            //$scope.mergedCells = mergedCells;


            $scope.columnHeaderTexts_JSON = columnHeaderTexts && JSON.stringify(columnHeaderTexts);

            $scope.appplyHeaderChange = function (index, newText, headers) {
                $scope.columnHeaderTexts_JSON = headers && JSON.stringify(headers);
            };


            _that.mode = $scope.mode = 'BindingEdit';
            $scope.switchMode = (mode?: string) => {
                var newMode = $scope.mode = mode || ($scope.mode == 'ValueEdit' ? 'BindingEdit' : 'ValueEdit');
                _that.mode = newMode;
                //switch (newMode) {
                //    case 'ValueEdit':
                //        var valueEditTableContainer = this.valueEditTableContainer;
                //        _that.createValueEditTable(valueEditTableContainer, this.changedData || this.sourceData);
                //    default:
                //        break;
                //    case 'BindingEdit':
                //        break;
                //}
            };

        });


        return ngController;
    }

    processTableValueChange(tableType,changes: any[][]) {
        var scope = this.scope;
        var isBindingEdit = scope && scope.mode == 'BindingEdit';
        var isValueEdit = !isBindingEdit;

        var oldData = this.changedData.allCellValues;

        //var allValues = _that.changedData.allCellValues;
        ////  debugger
        //changes.forEach(([row, col, oldValue, newValue]) => {
        //    var sourceDef = allValues[row][col];
        //    if (sourceDef == null) {

        //    } else if (typeof sourceDef == 'undefined') {

        //    } else {
        //        if (sourceDef.BindType == 'Value') {
        //            sourceDef.SetValue = newValue;
        //            // _that.changedData.allCellValues[row][col]=
        //        }
        //    }
        //});

        /**
         *
         * */
        var changedItems = [];
        changes.forEach((value, index) => {
            var info = changes[index];
            var [row, col, oldValue, newValue] = info;

            var isValueChanged = oldValue != newValue;
            if (isValueChanged) {
                changedItems.push(info);
            }
            if (row + 1 > oldData.length) {
                for (var i = oldData.length; i < row + 1; i++) {
                    oldData[i] = [];
                }
                //  oldData.length = row;
            }

            var oldColumns = oldData[row];
            //if (!oldCells) {
            //    oldCells = [];
            //}
            if (col+1 > oldColumns.length) {
                for (var i = oldColumns.length; i < col + 1; i++) {
                    oldColumns[i] = null;
                }
            }
            var oldValueObj = oldColumns[col];

            // debugger
            // 不管有沒有 change，都需要算一下 text ？
            if (newValue) {
                var obj: ValueBindingDef;

                // 這個 if 分支目前不會進入
                if (MnUtil.isJson(newValue)) {
                    obj = <any>newValue;
                    // 无需更改
                    // array[j] = d;//parseCellValueIfIsJsonExpr(d);
                }
                // 用戶 Dropdown 選擇綁定后，會進入此分支
                else if (this.isCellValueJsonExpr(newValue)) {
                    // 用户下拉选择绑定 Expression 后，cell value 为 JSON Expr 
                    obj = this.parseCellValueIfIsTextExpr(newValue);
                }
                else {
                    var sourceDef = oldValueObj;
                    //debugger
                    if (isValueEdit && sourceDef && sourceDef.BindType == 'Value') {
                        sourceDef.SetValue = newValue;
                        obj = sourceDef;
                    } else {

                        // 用戶手動輸入自定義文本（不是從 Dropdown 内選擇）
                        // 用户手动输入任意字符串
                        obj = { BindType: "StaticValue", StaticValue: newValue };
                    }
                }
                var text = this.getCellDisplayText2(tableType, obj, this.changedData.valueMaps);
                // [[row, prop, oldVal, newVal], ...]
                // table 只能處理 string 類型數據，無法處理 Object
                changes[index][3] = text

                // 把對象更新到這裏，後面 angular 會更新到表單上
                oldColumns[col] = obj;
            } else {
                oldColumns[col] = newValue;
            }
        });

        //debugger
        //  this.fixNewData(changes, newData, oldData, this.scope && this.scope.mode == 'BindingEdit');
        // this.changedData.allCellValues = newData;

        if (scope && changedItems.length > 0) {
            // angular 更新
            scope.applyTableValueChange(oldData);
        }

        return changedItems;
    }

    onDesignTableMergeChange(mergedCellsNew) {
        this.changedData.mergedCells = mergedCellsNew;
        if (this.scope) {
            this.scope.applyTableMergeChange(mergedCellsNew);
        }
    }

    ///**
    // *
    // * 全量 fix data 是必要的，因为 fix 后的对象没有 update 到 table
    // * 所以下次 change 还需要 fix 上次的 change
    // * @param newData
    // */
    //private fixNewData(changes, newData: Array<Array<ValueBindingDef>>, oldData: Array<Array<ValueBindingDef>>, isBindingEditMode: boolean) {
    //    if (newData) {
    //        // debugger
    //        for (var row = 0; row < newData.length; row++) {
    //            var cells = newData[row];
    //            if (cells) {
    //                for (var col = 0; col < cells.length; col++) {
    //                    var value = cells[col];
    //                    if (value) {
    //                        // 当 handsometable 首次初始化时（通常是刷新页面），其 cell value 从指定的数据源中读取（从数据库获取的数据序列号为 JSON Object），为对象
    //                        if (MnUtil.isJson(value)) {
    //                            // 无需更改
    //                            // array[j] = d;//parseCellValueIfIsJsonExpr(d);
    //                        }
    //                        else if (this.isCellValueJsonExpr(value)) {
    //                            // 用户下拉选择绑定 Expression 后，cell value 为 JSON Expr 
    //                            cells[col] = this.parseCellValueIfIsTextExpr(value);
    //                        }
    //                        else {
    //                            var sourceDef = oldData[row][col];
    //                            if (sourceDef) {
    //                                if (!isBindingEditMode && sourceDef.BindType == 'Value') {
    //                                    sourceDef.SetValue = value;
    //                                    // _that.changedData.allCellValues[row][col]=
    //                                } else {
    //                                    // 用户手动输入任意字符串
    //                                    cells[col] = { BindType: "StaticValue", StaticValue: value };
    //                                }
    //                            } else {
    //                                cells[col] = { BindType: "StaticValue", StaticValue: value };
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}



    //private fixEditNewData(oldData: Array<Array<ValueBindingDef>>, newData: Array<Array<ValueBindingDef>>) {


    //    for (var i = 0; i < newData.length; i++) {
    //        var array = newData[i];
    //        if (array) {
    //            for (var j = 0; j < array.length; j++) {
    //                var d = array[j];
    //                if (d) {
    //                    // 当 handsometable 首次初始化时（通常是刷新页面），其 cell value 从指定的数据源中读取（从数据库获取的数据序列号为 JSON Object），为对象
    //                    if (MnUtil.isJson(d)) {
    //                        // 无需更改
    //                        // array[j] = d;//parseCellValueIfIsJsonExpr(d);
    //                    }
    //                    else if (this.isCellValueJsonExpr(d)) {
    //                        // 用户下拉选择绑定 Expression 后，cell value 为 JSON Expr 
    //                        array[j] = this.parseCellValueIfIsJsonExpr(d);
    //                    }
    //                    else {
    //                        if (sourceDef.BindType == 'Value') {
    //                            sourceDef.SetValue = newValue;
    //                            // _that.changedData.allCellValues[row][col]=
    //                        } else {

    //                        }
    //                        // 用户手动输入任意字符串
    //                        array[j] = { BindType: "StaticValue", StaticValue: d };
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    var sourceDef = oldData[row][col];
    //    if (sourceDef == null) {

    //    } else if (typeof sourceDef == 'undefined') {

    //    } else {
    //        if (sourceDef.BindType == 'Value') {
    //            sourceDef.SetValue = newValue;
    //            // _that.changedData.allCellValues[row][col]=
    //        }
    //    }
    //}


    onDesignTableHeaderChange(index, newText, headers) {
        this.changedData.columnHeaderTexts = headers;
        if (this.scope) this.scope.appplyHeaderChange(index, newText, headers);
    }

    /**
     *
     *
        //v https://handsontable.com/docs/7.0.0/tutorial-using-callbacks.html
        //var hooks = ['afterChange', 'afterRemoveRow', 'afterRemoveCol', 'afterMergeCells',' afterUnmergeCells'];

     * @param uniqueId
     * @param tableCfg
     * @param eventNames
     * @param changeCallback
     */
    private bindTableEvent(uniqueId, tableCfg, eventNames: Array<string>, changeCallback) {

        //['']
        //var hooks = Handsontable.hooks.getRegistered()
        //    .filter(x => x.indexOf('Mouse') != -1 || x.indexOf('Scroll') != -1);

        // var girdTable;
        eventNames.forEach(function (hook) {
            // console.log("hook", arguments);
            // var exists = tableCfg['afterOnCellMouseDown'];
            var exists = tableCfg[hook];
            tableCfg[hook] = function (tableInstance) {
                //debugger
                if (exists) exists.apply(this, arguments);
                // if (tableInstance.getPlugin) {
                // this is handsometable instance
                changeCallback(this, arguments);
                //  }
            }
        });
    };

    getColumns(columnDef, allowEdit?: boolean) {
        var columns = [];
        var columnHeaderTexts = this.sourceData.columnHeaderTexts;
        var allCellValues = this.sourceData.allCellValues;
        if (columnHeaderTexts) {
            for (var i = 0; i < columnHeaderTexts.length; i++) {
                columns.push(columnDef);
            }
            // debugger
            // if columns length is 0,handsometable will throw Exception,if columns lenth less than data's column length , the column will not display. 
            if (columns.length == 0) {
                var firstRow = allCellValues && allCellValues[0];
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
        return columns;
    }


    createTable(container) {

    }

    autoRowSize = true;
    autoColumnSize = true;


    bindingEditTable;
    valueEditTable;
    createEditTable(bindingEditContainer, valueEditContainer) {
        var option = this.sourceData;
        var scopeAccesser = this.createController(option);
        this.bindingEditTable = this.createBindingEditTable(bindingEditContainer);
        this.valueEditTable = this.createValueEditTable(valueEditContainer, this.changedData);
    }

    getCellValue(row, col) {
        var cells = this.changedData.allCellValues[row];
        return cells && cells[col];
    }



    getTableDataAndColumnConfig(data: HandsontableData, columnDef) {
        var uniqueId = data.uniqueId,
            allCellValues = data.allCellValues,
            valueMaps = data.valueMaps,
            mergedCells = data.mergedCells,
            cellDropdownData = data.cellDropdownData,
            columnHeaderTexts = data.columnHeaderTexts;

        var allCellTexts = this.getCellTexts(allCellValues);

        var obj = {
            autoRowSize: this.autoRowSize,
            autoColumnSize: this.autoColumnSize,
            data: allCellTexts,
            colHeaders: columnHeaderTexts,//['Data Member (First Column)', '', 'Data Member (Second Column)', ''],
            // columns: columns,
            //  columns: [columnDef, columnDef, columnDef, columnDef],
            mergeCells: mergedCells || [],
        };

        if (columnDef) {
            var columns = this.getColumns(columnDef);
            obj['columns'] = columns;
        }
        return obj;
    }

    createBindingEditTable(container) {
        var data = this.sourceData;

        var uniqueId = data.uniqueId,
            allCellValues = data.allCellValues,
            valueMaps = data.valueMaps,
            mergedCells = data.mergedCells,
            cellDropdownData = data.cellDropdownData,
            columnHeaderTexts = data.columnHeaderTexts;


        var _that = this;

        //   var scopeAccesser = createNgBindController(uniqueId, tableCfg, () => girdTable, allCellValues, mergedCells);

        var columnDef = {
            // renderer: Handsontable.renderers.AutocompleteRenderer,
            renderer: this.createRender(null, (row, col) => this.getCellValue(row, col), valueMaps, Handsontable.renderers.AutocompleteRenderer),
            type: 'handsontable',
            //editor: 'text',
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
                    // debugger
                    // 這是 handsontable 的回調，不是 column 的，因此如果用戶采用手動輸入的方式，不會觸發此方法
                    var selection = this.getSelectedLast();
                    //console.log("getValue", selection);
                    //// debugger
                    //// Get the row's data object, object is from BindingDefSources
                    var obj = this.getSourceDataAtRow(selection[0]);


                    // return obj;

                    // 'JSON::'+JSON.stringify(obj);
                    var ret = _that.wrapCellValueJsonToTextExpr(obj);
                    return ret;

                    //var ret = <string><any>_that.getCellDisplayText2(null, obj, valueMaps);
                    //// console.log('getValue', ret);
                    //if (ret && ret.indexOf('Object') !== -1) {
                    //    //debugger
                    //}
                    //return ret;

                    //return obj.ContentPartName + '.' + obj.MemberExpression;

                    // the value only accept plain type
                    //return obj.Key;
                    //return obj;
                }
            }
        };

        //  debugger

        var _that = this;

        ///*
        /*
           *
           * https://stackoverflow.com/questions/18348437/how-do-i-edit-the-header-text-of-a-handsontable
           *
           *
    https://stackoverflow.com/questions/32212596/prevent-handsontable-cells-from-being-selected-on-column-header-click
    */
        var afterOnCellMouseDown = function (event, coords, th) {

            // 鼠标左键
            // if (event.button !== 0 || event.button !== 1) return;
            //if (!event || event.button === 2) return;
            //  debugger;

            console.log("even.button:" + event.button + 'window.event.button:' + (<any>(window.event)).button);

            // debugger
            // only allow column header edit , do not allow row header edit
            if (!coords) return;

            let instance = this,
                isCol = coords.row === -1,
                isRow = coords.col === -1
                ;

            if (!(isCol || isRow)) return;

            // fix bug , hover 也会触发此回调
            if (th.__fixafterOnCellMouseDown) {
                return;
            }

            th.__fixafterOnCellMouseDown = 1;


            $(th).click(() => {
                console.log("th click");

                let input = document.createElement('input'),
                    rect = th.getBoundingClientRect(),
                    addListeners = (events, headers, index) => {
                        events.split(' ').forEach(e => {
                            input.addEventListener(e, () => {
                                var newText = headers[index] = input.value;
                                instance.updateSettings(isCol ? {
                                    colHeaders: headers
                                } : {
                                        rowHeaders: headers
                                    });

                                // debugger
                                _that.onDesignTableHeaderChange(index, newText, headers);

                                setTimeout(() => {
                                    if (input.parentNode)
                                        input.parentNode.removeChild(input)
                                });
                            })
                        })
                    },
                    appendInput = () => {
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
                input.value = th.querySelector(
                    isCol ? '.colHeader' : '.rowHeader'
                ).innerText;
                appendInput();
                setTimeout(() => {
                    input.select();
                    addListeners('change blur', instance[
                        isCol ? 'getColHeader' : 'getRowHeader'
                    ](), coords[isCol ? 'col' : 'row']);
                });

            });

        };
        //*/

        var cfg = this.getTableDataAndColumnConfig(data, columnDef);

        var tableCfg = {
            ...cfg,

            licenseKey: 'non-commercial-and-evaluation',

            afterOnCellMouseDown: afterOnCellMouseDown,
            //data: allCellValues || Handsontable.helper.createSpreadsheetData(5, 2),

            //autoRowSize: this.autoRowSize,
            //autoColumnSize: this.autoColumnSize,
            //data: allCellTexts,
            //colHeaders: columnHeaderTexts,//['Data Member (First Column)', '', 'Data Member (Second Column)', ''],
            //columns: columns,
            ////  columns: [columnDef, columnDef, columnDef, columnDef],
            //mergeCells: mergedCells || [],

            //  colWidths: [47, 47, 47, 47, 47, 47, 47, 47, 47, 47, 47, 47, 47, 47, 47, 47],
            //  rowHeaders: true,
            contextMenu: true,
            allowInsertColumn: true,
            allowInsertRow: true,
            // colHeaders: true,

            //mergeCells: [
            //    { row: 1, col: 1, rowspan: 3, colspan: 3 },
            //    { row: 3, col: 4, rowspan: 2, colspan: 2 },
            //    { row: 5, col: 6, rowspan: 3, colspan: 3 }
            //],
            //afterMergeCells: function (range, parent, auto) {
            //    //    debugger
            //    console.log("afterMergeCells", range, parent, auto);

            //    scope.applyTableValueChange(girdTable);
            //}
        };

        this.bindTableValueChange('BindingEdit',tableCfg);


        this.bindTableEvent(uniqueId, tableCfg, ['afterMergeCells', 'afterUnmergeCells'], (tableInstance) => {
            var mc = tableInstance.getPlugin('mergeCells');
            var mergedCellsCollection = mc.mergedCellsCollection;
            var mergedCellsNew = mergedCellsCollection && mergedCellsCollection.mergedCells;
            this.onDesignTableMergeChange(mergedCellsNew);
        });


        var girdTable;
        girdTable = new Handsontable(container, tableCfg);

        return girdTable;
    };


    existsValueEditTable: any;


    valueEditTableContainer: any;

    /**
     * 当 handsometable 首次初始化时（通常是刷新页面），其 data 从指定的数据源中读取，如果 cell value 為 Object，則 handsometable 會有無法修改 cell value 的 bug，所以這裏要把它的 value fix 為 text
     * */
    private getCellTexts(allCellValues: ValueBindingDef[][]) {
        if (!allCellValues) return [];
        var allCellTexts = [];
        for (var i = 0; i < allCellValues.length; i++) {
            var cells = allCellValues[i];
            var texts = allCellTexts[i] = [];
            for (var j = 0; j < cells.length; j++) {
                // texts.push(this.getCellDisplayText2(null, cells[j], valueMaps));
                texts[j] = (this.wrapCellValueJsonToTextExpr(cells[j]));
            }
        }
        return allCellTexts;
    }

    /**
     *
     * 编辑绑定的 DataMemebr 的值
     * 允许编辑值，不允许修改绑定或添加删除行/列
     * @param container
     * @param data
     */
    createValueEditTable(container, data: HandsontableData) {
        //var option = this.option;

        var exists = this.existsValueEditTable;
        if (exists) {
            if (exists.destory) {
                exists.destory();
            }
            this.existsValueEditTable = null;
            $(container).html();
        }

        var uniqueId = data.uniqueId,
            allCellValues = data.allCellValues,
            valueMaps = data.valueMaps,
            mergedCells = data.mergedCells,
            cellDropdownData = data.cellDropdownData,
            columnHeaderTexts = data.columnHeaderTexts;

        var _that = this;

        var columnDef = {
            // renderer: Handsontable.renderers.AutocompleteRenderer,
            renderer: this.createRender('ValueEdit', (row, col) => this.getCellValue(row, col), valueMaps, Handsontable.renderers.TextRenderer),
        };

        //   var columns = this.getColumns(columnDef);

        var cfg = this.getTableDataAndColumnConfig(data, columnDef);

        var tableCfg = {
            ...cfg,
            licenseKey: 'non-commercial-and-evaluation',

            //autoRowSize: this.autoRowSize,
            //autoColumnSize: this.autoColumnSize,
            //columns: columns,
            //mergeCells: mergedCells || [],
            //data: allCellValues,

            //  colWidths: [47, 47, 47, 47, 47, 47, 47, 47, 47, 47, 47, 47, 47, 47, 47, 47],
            rowHeaders: false,
            contextMenu: false,
            allowInsertColumn: false,
            allowInsertRow: false,
            colHeaders: columnHeaderTexts,
        };


        this.bindTableValueChange('ValueEdit',tableCfg);

        //this.bindTableChange(uniqueId, tableCfg, null, (tableInstance) => {
        //    // 源数据
        //    var sourceData = tableInstance.getSourceData();

        //    // 用户编辑修改后的数据
        //    var newData = tableInstance.getData();

        //    // this.fixDesignNewData(newData);

        //    //var updateValues = [];
        //    //if (sourceData) {
        //    //    // debugger
        //    //    for (var i = 0; i < sourceData.length; i++) {
        //    //        var array = sourceData[i];
        //    //        if (array) {
        //    //            for (var j = 0; j < array.length; j++) {
        //    //                var d = array[j];
        //    //                var def = MnUtil.isJson(d) ? d : this.isCellValueJsonExpr(d) ? this.parseCellValueIfIsJsonExpr(d) : null;
        //    //                if (def) {
        //    //                    if (def.BindType == 'ValueEdit') {
        //    //                        var newValue = newData[i][j];
        //    //                        var edited = { ...def };
        //    //                        edited.Value = newValue;
        //    //                        updateValues.push(edited);
        //    //                    }
        //    //                    //array[j] = { BindType: "StaticValue", StaticValue: d };
        //    //                }
        //    //            }
        //    //        }
        //    //    }
        //    //}



        //    if (true) {

        //    }

        //});

        // debugger
        var girdTable = new Handsontable(container, tableCfg);

        this.existsValueEditTable = girdTable;

        return girdTable;
    };


    private bindTableValueChange(tableType,tableCfg) {
        var _that = this;

        //https://handsontable.com/docs/7.0.0/Hooks.html#event:beforeChange
        tableCfg['beforeChange'] = function (changes) {
            var tableInstance = this;
            if (changes) {
                // var newData = tableInstance.getData();
                var changedItems = _that.processTableValueChange(tableType,changes);
                if (changedItems && changedItems.length) {
                    var currentMode = _that.mode;
                    if (currentMode) {
                        var table;
                        switch (currentMode) {
                            case 'ValueEdit':
                                table = _that.bindingEditTable;
                                break;
                            case 'BindingEdit':
                                table = _that.valueEditTable;
                                break;
                        }
                        table.updateSettings(_that.getTableDataAndColumnConfig(_that.changedData, null))
                    }
                }
            }


        };
    }

    //helper.createDesignTable = createDesignTable;
    //helper.createNgBindController = createNgBindController;
    //helper.bindTableChange = bindTableChange;

    //helper.createRender = createRender;
    //helper.parseCellValueIfIsJsonExpr = parseCellValueIfIsJsonExpr;
    //helper.wrapCellValueJson = wrapCellValueJson;

}

//})(handsontableHelper);



var handsontableHelper = new HandsontableCustomHelper();
