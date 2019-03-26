using AutoMapper;
using MnLab.PdfVisualDesign.Binding.Elements;
using MnLab.PdfVisualDesign.ViewModels;
using Orchard.Environment.Extensions;
using Orchard.Layouts.Framework.Display;
using Orchard.Layouts.Framework.Drivers;
using Orchard.Layouts.Helpers;
using Orchard.Layouts.Services;
using Orchard.Layouts.ViewModels;
using System.Linq;
using System;

namespace MnLab.PdfVisualDesign.Binding.Drivers {


    public class PropertyBindElementDriver : ElementDriver<PropertyBindElement> {


        readonly IContentFieldDisplay _fieldDisplay;
        private readonly IElementFilterProcessor _processor;
        public PropertyBindElementDriver(IElementFilterProcessor processor, IContentFieldDisplay fieldDisplay) {
            this._processor = processor;
            this._fieldDisplay = fieldDisplay;
        }

        protected override EditorResult OnBuildEditor(PropertyBindElement element, ElementEditorContext context) {

            var updater = context.Updater;

            var bindingInfo = Mapper.Map(element, new FieldBindingInfo());

            var viewModel = new PropertyBindViewModel {
                Binding = bindingInfo,
                //Content = content,
                //Field = field,
                FieldDisplay = _fieldDisplay,
            };

            if (context.Updater != null) {
                if (!context.Updater.TryUpdateModel(bindingInfo, GetPrefix(context, nameof(PropertyBindViewModel.Binding)), null, null)) {
                    //updater.AddModelError("", T("ContentPart {0} not exists", partTypeName));
                    goto ret;
                }
                else {
                }
            }

            var partTypeName = bindingInfo.ContentPartTypeName;
            var partPropertyName = bindingInfo.ContentPartFieldExpression;

            //context.Content.ContentItem.GetContentField("{}");

            // context.Content.ContentItem.TypeDefinition.Parts.First().PartDefinition.Fields.First()
            //  .FieldDefinition

            //context.Content.ContentItem.ContentManager.GetContentTypeDefinitions()
            //     .First().Parts

            //Orchard.Fields.Fields.

            //Orchard.Core.Common.Fields.TextField

            var content = context.Content;
            var contentItem = content.ContentItem;

            /*
               https://docs.orchardproject.net/en/latest/Documentation/Creating-a-custom-field-type/
            */
            var contentPart = contentItem.Parts.FirstOrDefault(x => x.PartDefinition.Name == partTypeName);
            var field = contentPart?.Fields.FirstOrDefault(x => x.Name == partPropertyName);

            viewModel.Content = content;
            viewModel.Field = field;


            //var viewModel = new PropertyBindViewModel {
            //    ContentPartFieldExpression = element.ContentPartFieldExpression,
            //    ContentPartTypeName = element.ContentPartTypeName,
            //    ExampleValue = element.ExampleValue,
            //    Remark = element.Remark,
            //};

            if (context.Updater != null) {
                if (!context.Updater.TryUpdateModel(element, GetPrefix(context, nameof(PropertyBindViewModel.Binding)), null, null)) {
                    //updater.AddModelError("", T("ContentPart {0} not exists", partTypeName));
                    goto ret;
                }

                if (contentPart == null) {
                    updater.AddModelError("", T("ContentPart {0} not exists", partTypeName));
                    goto ret;
                }
                /*
                  Feature:
                  different Orchard Field can have different render (TextField,DateTimeField,DateRangeField)
                  and maybe has multi property

                */
                if (field == null) {
                    updater.AddModelError("", T("Field {0} not exists in ContentPart {1} not exists", partPropertyName, partTypeName));
                    goto ret;
                }
                else {

                    Mapper.Map(bindingInfo,element);

                    if (updater.TryUpdateModel(field, GetPrefix(context, "Value"), null, null)) {

                    }
                    else {

                    }

                    if (field is Orchard.Core.Common.Fields.TextField f) {
                        //f.Value = 
                    }
                    //if (field is Orchard.Core.Common.Fields.TextField || field is Orchard.Fields.Fields.InputField) {

                    //}

                    //Orchard.ContentManagement.ContentField


                    //contentPart.
                }


                //if (context.Updater.TryUpdateModel(element, GetPrefix(context, nameof(PropertyBindViewModel.Value)), null, null)) {

                //}
                //else {

                //}

                //Mapper.Map(viewModel,element);
                //element.ContentPartFieldExpression = viewModel.ContentPartFieldExpression;
            }

        ret:
            var editor = context.ShapeFactory.EditorTemplate(TemplateName: $"Elements.{nameof(PropertyBindElement)}", Model: viewModel);
            return Editor(context, editor);
        }

        private static string GetPrefix(ElementEditorContext context, string name) {
            return $"{(string.IsNullOrEmpty(context.Prefix) ? null : ".")}{name}";
        }

        protected override void OnDisplaying(PropertyBindElement element, ElementDisplayingContext context) {
            context.ElementShape.HTML = element.Content;
        }

    }
}