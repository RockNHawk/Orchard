var handsontableHelper = {};
(function (helper) {


    var parseCellValueIfIsJson = function (d) {
        if (d && typeof (d) === 'string' && d.startsWith('JSON::')) {
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


    var wrapCellValueJson = function (obj) {
        if (!obj) return obj;
        return 'JSON::' + JSON.stringify(obj);
    };

    /*
@renderer is like   Handsontable.renderers.AutocompleteRenderer
*/
    var createRender = function (type, ValueMaps, renderer) {

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

        return function (instance, td, row, col, prop, value, cellProperties) {
            if (value) {
                // debugger;

                try {
                    /*
                    if the data is from server (server JSON SerializeObject , value type is Object)
                    if the data is from local user inputed, calue type is string
                    */
                    var obj = value && (typeof value === 'string' ? parseCellValueIfIsJson(value) : value);

                    var key = obj['Key'];
                    if (key) {
                        // arguments[5] is value argument, relace it will change the cell display text
                        // arguments[5] = key;
                        var _type = type || (obj['BindType']);
                        var valueArgument;
                        switch (_type) {
                            case 'DisplayName':
                            default:
                                valueArgument = key;
                                break;
                            case 'Value':
                                if (ValueMaps) {
                                    var memberValue = ValueMaps[key];
                                    valueArgument = memberValue;
                                }
                                break;
                        }
                        arguments[5] = valueArgument;
                        renderer.apply(instance, arguments);
                        //  debugger
                        // $(td).html(val);
                        //return $("<th>"+value.Key+"</th>");
                    }
                } catch (e) {
                    console.error(e);
                }
            } else {
                renderer.apply(instance, arguments);
            }
            //return td;
            return td;
        };
    };

    helper.createRender = createRender;
    helper.parseCellValueIfIsJson = parseCellValueIfIsJson;
    helper.wrapCellValueJson = wrapCellValueJson;
})(handsontableHelper);



