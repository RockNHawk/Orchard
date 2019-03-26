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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Descriptors;
using Orchard.FileSystems.VirtualPath;
using Orchard;
using Orchard.ContentManagement.Handlers;
using Orchard.UI.Zones;

namespace MnLab.PdfVisualDesign.Binding.Drivers {


    public class CustomContentFieldDisplay : ContentDisplayBase, IContentFieldDisplay {
        private readonly IEnumerable<IContentFieldDriver> _contentFieldDrivers;

        public CustomContentFieldDisplay(
            IShapeFactory shapeFactory,
            Lazy<IShapeTableLocator> shapeTableLocator,
            RequestContext requestContext,
            IVirtualPathProvider virtualPathProvider,
            IWorkContextAccessor workContextAccessor,
            IEnumerable<IContentFieldDriver> contentFieldDrivers)
            : base(shapeFactory, shapeTableLocator, requestContext, virtualPathProvider, workContextAccessor) {

            _contentFieldDrivers = contentFieldDrivers;
        }

        public override string DefaultStereotype {
            get { return "ContentField"; }
        }

        public dynamic BuildDisplay(IContent content, ContentField field, string displayType, string groupId) {
            var context = BuildDisplayContext(content, displayType, groupId);
            var drivers = GetFieldDrivers(field.FieldDefinition.Name);

            drivers.Invoke(driver => {
                var result = Filter(driver.BuildDisplayShape(context), field);
                if (result != null)
                    result.Apply(context);
            }, Logger);

            return context.Shape;
        }

        public dynamic BuildEditor(IContent content, ContentField field, string groupId) {
            var context = BuildEditorContext(content, groupId);
            var drivers = GetFieldDrivers(field.FieldDefinition.Name);

            drivers.Invoke(driver => {
                var result = driver.BuildEditorShape(context);
                if (result != null)
                    result.Apply(context);
            }, Logger);

            return context.Shape;
        }

        public dynamic UpdateEditor(IContent content, ContentField field, IUpdateModel updater, string groupInfoId) {
            var context = UpdateEditorContext(content, updater, groupInfoId);
            var drivers = GetFieldDrivers(field.FieldDefinition.Name);

            drivers.Invoke(driver => {
                var result = driver.UpdateEditorShape(context);
                if (result != null)
                    result.Apply(context);
            }, Logger);

            return context.Shape;
        }

        private DriverResult Filter(DriverResult driverResult, ContentField field) {
            DriverResult result = null;
            var combinedResult = driverResult as CombinedResult;
            var contentShapeResult = driverResult as ContentShapeResult;

            if (combinedResult != null) {
                result = combinedResult.GetResults().SingleOrDefault(x => x.ContentField != null && x.ContentField.Name == field.Name);
            }
            else if (contentShapeResult != null) {
                result = contentShapeResult.ContentField != null && contentShapeResult.ContentField.Name == field.Name ? contentShapeResult : driverResult;
            }

            return result;
        }

        private IEnumerable<IContentFieldDriver> GetFieldDrivers(string fieldName) {
            return _contentFieldDrivers.Where(x => x.GetType().BaseType.GenericTypeArguments[0].Name == fieldName);
        }
    }


    public class PropertyBindElementDriver : ElementDriver<PropertyBindElement> {


        private readonly IShapeFactory _shapeFactory;

        private readonly IEnumerable<IContentFieldDriver> _contentFieldDrivers;


        readonly IContentFieldDisplay _fieldDisplay;
        private readonly IElementFilterProcessor _processor;
        public PropertyBindElementDriver(IElementFilterProcessor processor, IContentFieldDisplay fieldDisplay,
            IShapeFactory shapeFactory,
            IEnumerable<IContentFieldDriver> contentFieldDrivers

            ) {
            this._processor = processor;
            this._fieldDisplay = fieldDisplay;
            this._shapeFactory = shapeFactory;
            this._contentFieldDrivers = contentFieldDrivers;

        }

        private IEnumerable<IContentFieldDriver> GetFieldDrivers(string fieldName) {
            return _contentFieldDrivers.Where(x => x.GetType().BaseType.GenericTypeArguments[0].Name == fieldName);
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


            if (field != null) {


              
                var drivers = GetFieldDrivers(field.FieldDefinition.Name);

                var shape = _shapeFactory.Create("Content_Edit", Arguments.Empty(), () => new ZoneHolding(() => _shapeFactory.Create("ContentZone", Arguments.Empty())));
                //var fieldEditorContext = new BuildEditorContext(_shapeFactory.Create("Content_Edit"), content, null, _shapeFactory);
                var fieldEditorContext = new BuildEditorContext(shape, content, "", _shapeFactory);
                fieldEditorContext.FindPlacement = (partType, differentiator, defaultLocation) => new PlacementInfo { Location = "1", Source = String.Empty };

                //foreach (var driver in drivers) {
                //    driver.BuildEditorShape(fieldContext);
                //}

                viewModel.FieldDrivers = drivers.Select(driver => driver.BuildEditorShape(fieldEditorContext));

                foreach (var item in viewModel.FieldDrivers) {
                    item.Apply(fieldEditorContext);
                }
                viewModel.EditorShapeContext = fieldEditorContext;
            }


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

                    Mapper.Map(bindingInfo, element);

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