//declare module String {
//    export var format: any;
//}
//interface String {
//    format(...replacements: string[]): string;
//}

//if (!String.prototype.format) {
//    String.prototype.format = function () {
//        var args = arguments;
//        return this.replace(/{(\d+)}/g, function (match, number) {
//            return typeof args[number] != 'undefined'
//                ? args[number]
//                : match
//                ;
//        });
//    };
//}

function StringFormat(str: string, ...val: string[]) {
    for (let index = 0; index < val.length; index++) {
        str = str.replace(`{${index}}`, val[index]);
    }
    return str;
}

/*
     * the Field's Editor generated input name not always use Field.PropertyXxx,
     *
     * such as TextField.Value 's Editor generated input is:
     *
     * <input name="PartName.FieldName.Text" /> (.Text not .Value)
     *
     * then the Driver map the Text's value to field.Value
     *
     * \Orchard.Web\Core\Common\Drivers\TextFieldDriver.cs
     *  if (updater.TryUpdateModel(viewModel, GetPrefix(field, part), null, null)) {
                var settings = field.PartFieldDefinition.Settings.GetModel<TextFieldSettings>();

                field.Value = viewModel.Text;

                if (settings.Required && String.IsNullOrWhiteSpace(field.Value)) {
                    updater.AddModelError("Text", T("The field {0} is mandatory", T(field.DisplayName)));
                }PropertyNameToEditorInputName
            }
* */
var defaultPropertyNameToEditorInputNameMap = {
    "NHProductDescriptionPart.Description.Value": "Text",
    //"NHProductDescriptionPart.ProductName.Value": "Text",
};
function mapPropertyNameToEditorInputName(field, baseKey, propName, propertyNameToEditorInputNameMap) {
    var map = propertyNameToEditorInputNameMap || defaultPropertyNameToEditorInputNameMap;
    var key = StringFormat('{0}.{1}', baseKey, propName);
    // var propValue = field[propName];
    var mappedName = (map && map[key]);
    if (mappedName) return mappedName;

    switch (propName) {
        case 'DisplayName':
        case 'PartFieldDefinition':
        case 'FieldDefinition':
        case 'Storage':
            return null;
        default:
            var FieldDefinition = field.FieldDefinition;
            if (FieldDefinition) {
                var fieldType = FieldDefinition.Name;
                switch (fieldType) {
                    case 'TextField':
                        switch (propName) {
                            case 'Value':
                                return 'Text';
                        }
                        break;
                }
            }
    }

    return null;
}

function syncContentPart(url, propertyNameToEditorInputNameMap, form) {
    var dataExample = {
        "NHProductDescriptionPart.Description": {
            "Value": null,
            "Name": "Description",
            "DisplayName": "Description",
            "PartFieldDefinition": {
                "Name": "Description",
                "DisplayName": "Description",
                "FieldDefinition": {
                    "Name": "TextField"
                },
                "Settings": {
                    "DisplayName": "Description",
                    "TextFieldSettings.Required": "False",
                    "ChoiceListFieldSettings.Options": "",
                    "ChoiceListFieldSettings.ListMode": "dropdown"
                }
            },
            "FieldDefinition": {
                "Name": "TextField"
            },
            "Storage": {}
        },
        "NHProductDescriptionPart.ProductName": {
            "Value": "11111111",
            "Name": "ProductName",
            "DisplayName": "Product Name",
            "PartFieldDefinition": {
                "Name": "ProductName",
                "DisplayName": "Product Name",
                "FieldDefinition": {
                    "Name": "TextField"
                },
                "Settings": {
                    "DisplayName": "Product Name",
                    "TextFieldSettings.Required": "False",
                    "ChoiceListFieldSettings.Options": "",
                    "ChoiceListFieldSettings.ListMode": "dropdown"
                }
            },
            "FieldDefinition": {
                "Name": "TextField"
            },
            "Storage": {}
        },
        "NHProductManualTemplate.boolean1": {
            "Value": false,
            "Name": "boolean1",
            "DisplayName": "boolean1",
            "PartFieldDefinition": {
                "Name": "boolean1",
                "DisplayName": "boolean1",
                "FieldDefinition": {
                    "Name": "BooleanField"
                },
                "Settings": {
                    "DisplayName": "boolean1",
                    "ChoiceListFieldSettings.Options": "",
                    "ChoiceListFieldSettings.ListMode": "dropdown",
                    "BooleanFieldSettings.Optional": "False",
                    "BooleanFieldSettings.OnLabel": "Yes",
                    "BooleanFieldSettings.OffLabel": "No",
                    "BooleanFieldSettings.SelectionMode": "Checkbox",
                    "BooleanFieldSettings.DefaultValue": ""
                }
            },
            "FieldDefinition": {
                "Name": "BooleanField"
            },
            "Storage": {}
        },
        "NHProductManualTemplate.Choice2": {
            "Options": null,
            "SelectedValue": "2",
            "Name": "Choice2",
            "DisplayName": "Choice2",
            "PartFieldDefinition": {
                "Name": "Choice2",
                "DisplayName": "Choice2",
                "FieldDefinition": {
                    "Name": "ChoiceListField"
                },
                "Settings": {
                    "DisplayName": "Choice2",
                    "ChoiceListFieldSettings.Options": "1;2;3",
                    "ChoiceListFieldSettings.ListMode": "dropdown"
                }
            },
            "FieldDefinition": {
                "Name": "ChoiceListField"
            },
            "Storage": {}
        },
        "NHProductManualTemplate.number1": {
            "Value": null,
            "Name": "number1",
            "DisplayName": "number1",
            "PartFieldDefinition": {
                "Name": "number1",
                "DisplayName": "number1",
                "FieldDefinition": {
                    "Name": "NumericField"
                },
                "Settings": {
                    "DisplayName": "number1",
                    "ChoiceListFieldSettings.Options": "",
                    "ChoiceListFieldSettings.ListMode": "dropdown",
                    "NumericFieldSettings.Required": "False",
                    "NumericFieldSettings.Scale": "0",
                    "NumericFieldSettings.Minimum": "",
                    "NumericFieldSettings.Maximum": ""
                }
            },
            "FieldDefinition": {
                "Name": "NumericField"
            },
            "Storage": {}
        },
        "NHProductManualTemplate.TempalteSupportField": {
            "HTML": null,
            "Name": "TempalteSupportField",
            "DisplayName": "TempalteSupportField",
            "PartFieldDefinition": {
                "Name": "TempalteSupportField",
                "DisplayName": "TempalteSupportField",
                "FieldDefinition": {
                    "Name": "TempalteSupportField"
                },
                "Settings": {
                    "DisplayName": "TempalteSupportField",
                    "ChoiceListFieldSettings.Options": "",
                    "ChoiceListFieldSettings.ListMode": "dropdown"
                }
            },
            "FieldDefinition": {
                "Name": "TempalteSupportField"
            },
            "Storage": {}
        }
    };


    //var $formSubmits = form && $(form).find("button,input[type='button'],input[type='submit']").filter(':enabled');
    //if ($formSubmits) $formSubmits.attr('disabled', 'disabled');

    $.ajax({
        type: "GET",
        url: url,
        timeout: 3000,
        //data: "name=John&location=Boston",
        success: function (data) {
            if (data) {
                // baseKey is:PartType.FieldName
                for (var baseKey in data) {
                    var field = data[baseKey];
                    for (var propName in field) {
                        var propValue = field[propName];
                        var propMapName = mapPropertyNameToEditorInputName(field, baseKey, propName, propertyNameToEditorInputNameMap) || propName;
                        if (!propMapName) continue
                        var inputFullName = StringFormat('{0}.{1}', baseKey, propMapName);
                        var $input = $(StringFormat("[name='{0}']", inputFullName));
                        if ($input.length) {
                            $input.val(propValue);

                            //$input.hide();
                            /// hide the fieldset
                              $input.parent().hide();
                            // // find the label
                            // $(StringFormat("label[for='{0}']", inputFullName.replace(/./ig,'_'))).hide();
                        }
                    }
                }
            }
        },
        complete: function () {
            //if ($formSubmits) $formSubmits.removeAttr('disabled');
            setTimeout(() => {
                syncContentPart(url, propertyNameToEditorInputNameMap, form);
            }, 500);
        }
    });


    //setInterval(function () {
    //    //debugger

    //    $.get(url, (data) => {

    //    });

    //}, 1000);
}
