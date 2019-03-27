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
using System.Web;
using System.Reflection;
using System.Web.Mvc;
using Orchard.Core.Title.Models;

namespace MnLab.PdfVisualDesign.Binding.Drivers {


    public class ContentPartPropertyEditViewModel {
        public PropertyInfo PropertyInfo { get; set; }
        public ContentPart ContentPart { get; set; }
        public object Value { get; set; }
        public string Prefix { get; set; }
    }


    public interface IContentMemberAccessor {
        object GetValue();
        void SetValue(object value);
    }

    public class ContentFieldMemberAccessor : IContentMemberAccessor {
        string _memberName;
        ContentField _field;
        ContentPart _contentItem;
        public ContentFieldMemberAccessor(ContentPart contentItem, ContentField field, string memberName) {
            this._contentItem = contentItem;
            this._field = field;
            this._memberName = memberName;
        }

        public object GetValue() {
            return _field.Storage.Get<object>(null);
        }

        public void SetValue(object value) {
            _field.Storage.Set<object>(null, value);
        }
    }

    public class ContentPropertyMemberAccessor : IContentMemberAccessor {
        string _memberName;
        PropertyInfo _field;
        ContentPart _contentItem;
        public ContentPropertyMemberAccessor(ContentPart contentItem, PropertyInfo field, string memberName) {
            this._contentItem = contentItem;
            this._field = field;
            this._memberName = memberName;
        }

        public object GetValue() {
            return _field.GetValue(_contentItem);
        }

        public void SetValue(object value) {
            _field.SetValue(_contentItem, value);
        }
    }

    public class ContentDataMemberHelper {

        public ContentField Field;
        public PropertyInfo PropertyInfo;

        public ContentPart Part;
        public string Expression;

        public bool hasDataMember { get => Field != null || PropertyInfo != null; }

        public ContentDataMemberHelper(ContentPart contentItem, string expr) {
            this.Part = contentItem;
            this.Expression = expr;
        }


        public IContentMemberAccessor GetAccessor() {
            if (Field != null) {
                return new ContentFieldMemberAccessor(Part, Field, Expression);
            }
            else if (PropertyInfo != null) {
                return new ContentPropertyMemberAccessor(Part, PropertyInfo, Expression);
            }
            else {
                throw new InvalidOperationException("No data member");
            }
        }

        public static ContentDataMemberHelper FindFromContentItem(ContentItem contentItem, string partName, string partPropertyName) {
            if (partName == null) throw new ArgumentNullException(nameof(partName));
            if (partPropertyName == null) throw new ArgumentNullException(nameof(partPropertyName));
            var part = contentItem.Parts.FirstOrDefault(x => x.PartDefinition.Name == partName);
            if (part == null) return null;
            /*
               https://docs.orchardproject.net/en/latest/Documentation/Creating-a-custom-field-type/

            field in Orchard is dynamic added to a ContentPart by user
            the internal ContentPart own it's "hard code" Property like TitlePart.Title is a hard coded property
            it also storage the data, but it only can edit by hole part no single Property editor support (see TitlePartDriver), or I can add this feature by self.
            */
            var field = part?.Fields.FirstOrDefault(x => x.Name == partPropertyName);

            var helper = new ContentDataMemberHelper(part, partPropertyName);
            if (field != null) {
                helper.Field = field;
            }
            else {
                var property = part.GetType().GetProperty(partPropertyName);
                helper.PropertyInfo = property;
            }
            return helper;
        }
    }


    public class PropertyBindElementDriver : ElementDriver<PropertyBindElement> {


        private readonly IElementFilterProcessor _processor;

        private readonly IShapeFactory _shapeFactory;

        private readonly IEnumerable<IContentFieldDriver> _contentFieldDrivers;

        readonly IContentFieldDisplay _fieldDisplay;



        private readonly Lazy<IShapeTableLocator> _shapeTableLocator;

        private readonly IVirtualPathProvider _virtualPathProvider;

        IContentManager _contentManager;

        readonly IWorkContextAccessor _workContextAccessor;


        public PropertyBindElementDriver(IElementFilterProcessor processor, IContentFieldDisplay fieldDisplay,
            IShapeFactory shapeFactory,
             Lazy<IShapeTableLocator> shapeTableLocator,
             IVirtualPathProvider virtualPathProvider,
            IEnumerable<IContentFieldDriver> contentFieldDrivers,

            IContentManager contentManager,
            IWorkContextAccessor workContextAccessor

            ) {
            this._processor = processor;
            this._fieldDisplay = fieldDisplay;
            this._shapeFactory = shapeFactory;
            this._shapeTableLocator = shapeTableLocator;
            this._virtualPathProvider = virtualPathProvider;
            this._contentFieldDrivers = contentFieldDrivers;
            this._contentManager = contentManager;
            this._workContextAccessor = workContextAccessor;
        }

        private IEnumerable<IContentFieldDriver> GetFieldDrivers(string fieldName) {
            return _contentFieldDrivers.Where(x => x.GetType().BaseType.GenericTypeArguments[0].Name == fieldName);
        }

        dynamic BuildContentPropertyEditor(ContentPartPropertyEditViewModel viewModel, IUpdateModel updater, IValueProvider valueProvicer) {

            if (updater != null) {

                /*
                I can't recall the exact details, but there was a reason the ValueProvider was being modified with existing values from the element, and it had something to do with a certain scenario where the element's editor UI would not be rendered, yet its Update method was called, or something along those lines. However, your proposal does make things more "natural", so what I propose we do is merge in your PR and see how it works. I'm currently doing some Layouts development anyway, so if there are any issues I'll most likely spot them soon enough (and ask you to fix them ;))
                Thanks for spotting this.
                 */

                //var part = (TitlePart)model.ContentPart;
                var part = viewModel.ContentPart;

                if (!TryUpdateModel(updater, part, viewModel.Prefix, new string[] { viewModel.PropertyInfo.Name }, null)) {
                    //if (!updater.TryUpdateModel(model.ContentPart, model.Prefix, null, null)) {
                    updater.AddModelError("", T("Error update ContentPart Property '{0}.{1}'", viewModel.ContentPart.TypePartDefinition.PartDefinition.Name, viewModel.PropertyInfo.Name));
                }
            }

            dynamic sp = _shapeFactory;
            var editor = sp.EditorTemplate(TemplateName: $"Property.Property", Model: viewModel);
            return editor;
        }

        bool TryUpdateModel(IUpdateModel updater, object model, string prefix, string[] includeProperties, string[] excludeProperties) {
            Predicate<string> propertyFilter = (string propertyName) => IsPropertyAllowed(propertyName, includeProperties, excludeProperties);
            var type = model.GetType();
            var binder = System.Web.Mvc.ModelBinders.Binders.GetBinder(type);
            var controller = (Controller)updater;
            var bc = new ModelBindingContext {
                // Model = model,
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, type),
                ModelState = controller.ModelState,
                ModelName = prefix,
                PropertyFilter = propertyFilter,
                ValueProvider = controller.ValueProvider,
            };
            binder.BindModel(controller.ControllerContext, bc);
            return controller.ModelState.IsValid;
        }

        internal static bool IsPropertyAllowed(string propertyName, ICollection<string> includeProperties, ICollection<string> excludeProperties) {
            bool flag = includeProperties == null || includeProperties.Count == 0 || includeProperties.Contains(propertyName, StringComparer.OrdinalIgnoreCase);
            bool flag2 = excludeProperties != null && excludeProperties.Contains(propertyName, StringComparer.OrdinalIgnoreCase);
            return flag && !flag2;
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

            var partName = bindingInfo.ContentPartName;
            var partPropertyName = bindingInfo.ContentPartMemberExpression;

            var content = context.Content;
            var contentItem = content.ContentItem;
            if (content.Id > 0) {
                /*
               context.Content's Version is the publish version, if the content never Published, the field will be null.
               so I get the latest (if have draft) ContentItem from ContentManager
                */
                contentItem = _contentManager.Get(content.Id, VersionOptions.Latest);
            }

            var member = string.IsNullOrEmpty(partName) || string.IsNullOrEmpty(partPropertyName) ? null : ContentDataMemberHelper.FindFromContentItem(contentItem, partName, partPropertyName);

            var part = member?.Part;

            /*
               https://docs.orchardproject.net/en/latest/Documentation/Creating-a-custom-field-type/

            field in Orchard is dynamic added to a ContentPart by user
            the internal ContentPart own it's "hard code" Property like TitlePart.Title is a hard coded property
            it also storage the data, but it only can edit by hole part no single Property editor support (see TitlePartDriver), or I can add this feature by self.
            */
            var field = member?.Field;
            var property = member?.PropertyInfo;
            var hasDataMember = member?.hasDataMember??false;

            viewModel.Content = content;
            viewModel.Field = field;

            //var viewModel = new PropertyBindViewModel {
            //    ContentPartFieldExpression = element.ContentPartFieldExpression,
            //    ContentPartName = element.ContentPartName,
            //    ExampleValue = element.ExampleValue,
            //    Remark = element.Remark,
            //};

            if (context.Updater != null) {
                if (!context.Updater.TryUpdateModel(element, GetPrefix(context, nameof(PropertyBindViewModel.Binding)), null, null)) {
                    //updater.AddModelError("", T("ContentPart {0} not exists", partTypeName));
                    goto ret;
                }

                if (part == null) {
                    updater.AddModelError("", T("ContentPart {0} not exists", partName));
                    goto ret;
                }
                /*
                  Feature:
                  different Orchard Field can have different render (TextField,DateTimeField,DateRangeField)
                  and maybe has multi property

                */
                if (hasDataMember == false) {
                    updater.AddModelError("", T("Field {0} not exists in ContentPart {1}", partPropertyName, partName));
                    goto ret;
                }
                else {

                    Mapper.Map(bindingInfo, element);

                    //if (updater.TryUpdateModel(field, GetPrefix(context, "Value"), null, null)) {
                    //}
                    //else {
                    //}

                    //if (field is Orchard.Core.Common.Fields.TextField || field is Orchard.Fields.Fields.InputField) {
                    //}
                }
            }

            // next:

            //IEnumerable<IContentFieldDriver> drivers;
            if (hasDataMember) {

                // IContentFieldDriver

                var rootShape = _shapeFactory.Create("Content_Edit", Arguments.Empty(), () => new ZoneHolding(() => _shapeFactory.Create("ContentZone", Arguments.Empty())));

                var contentForCurrentFieldEditorBuild = ContentForFieldEditorBuild(field, part, content);
                //var contentForCurrentFieldEditorBuild = content;// ContentForFieldEditorBuild(field, contentPart, content);
                if (field != null) {
                    var drivers = GetFieldDrivers(field.FieldDefinition.Name);
                    if (updater != null) {

                        var workContext = _workContextAccessor.GetContext();
                        var theme = workContext.CurrentTheme;
                        var shapeTable = _shapeTableLocator.Value.Lookup(theme.Id);

                        var fieldEditorContext = new UpdateEditorContext(rootShape, contentForCurrentFieldEditorBuild, updater, "", _shapeFactory, shapeTable, GetPath());
                        fieldEditorContext.FindPlacement = (partType, differentiator, defaultLocation) => new PlacementInfo { Location = "1", Source = String.Empty };
                        var fieldEditorDriverResults = drivers.Select(driver => driver.UpdateEditorShape(fieldEditorContext));

                        /*
                        seems direct update the published version if no Draft,
                        and the date not updated.

                        but the requirement is:
                        if value not changed ignore (consider if value not changed and currently no draft, do not create a new draft version), if in draft update draft and save, if published, create a draft.

                         */
                        foreach (var item in fieldEditorDriverResults) {
                            item.Apply(fieldEditorContext);
                        }

                        //   _contentManager.Publish(contentItem);
                        //_contentManager.UpdateEditor(content,updater);

                        viewModel.EditorShape = fieldEditorContext.Shape.Zones["1"];
                    }
                    else {
                        //foreach (var driver in drivers) {
                        //    driver.BuildEditorShape(fieldContext);
                        //}
                        //var fieldEditorContext = new BuildEditorContext(_shapeFactory.Create("Content_Edit"), content, null, _shapeFactory);
                        var fieldEditorContext = new BuildEditorContext(rootShape, contentForCurrentFieldEditorBuild, "", _shapeFactory);
                        fieldEditorContext.FindPlacement = (partType, differentiator, defaultLocation) => new PlacementInfo { Location = "1", Source = String.Empty };

                        var fieldEditorDriverResults = drivers.Select(driver => driver.BuildEditorShape(fieldEditorContext));
                        foreach (var item in fieldEditorDriverResults) {
                            item.Apply(fieldEditorContext);
                        }
                        viewModel.EditorShape = fieldEditorContext.Shape.Zones["1"];
                    }
                }
                else if (property != null) {
                    var vm = new ContentPartPropertyEditViewModel {
                        ContentPart = part,
                        PropertyInfo = property,
                        Value = property.GetValue(part),
                        Prefix = $"{partName}.",
                    };

                    var propertyEditor = BuildContentPropertyEditor(vm, updater, context.ValueProvider);
                    //var propertyEditor = context.ShapeFactory.EditorTemplate(TemplateName: $"Property.Property", Model: vm);
                    viewModel.EditorShape = propertyEditor;
                }

            }

            ret:

            var editor = context.ShapeFactory.EditorTemplate(TemplateName: $"Elements.{nameof(PropertyBindElement)}", Model: viewModel);
            return Editor(context, editor);
        }


        private string GetPath() {
            return "";
            //return VirtualPathUtility.AppendTrailingSlash(_virtualPathProvider.ToAppRelative(_requestContext.HttpContext.Request.Path));
        }



        /// <summary>
        ///   IContentFieldDriver.BuildEditorShape method will build all shape of field from contentItem,
        /// </summary>
        /// <param name="field"></param>
        /// <param name="contentPart"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private static ContentPart ContentForFieldEditorBuild(ContentField field, ContentPart contentPart, IContent content) {
            /*
             */
            var part = new ContentPart() {
                TypePartDefinition = contentPart.TypePartDefinition,
            };
            if (field != null) {
                part.Weld(field);
            }

            var ct = new ContentItem() {
                ContentType = content.ContentItem.ContentType,
                TypeDefinition = content.ContentItem.TypeDefinition,
            };
            ct.Weld(part);

            var ct2 = new ContentPart() {
            };
            ct2.ContentItem = ct;
            return ct2;
        }

        private static string GetPrefix(ElementEditorContext context, string name) {
            return $"{(string.IsNullOrEmpty(context.Prefix) ? null : ".")}{name}";
        }


        protected override void OnDisplaying(PropertyBindElement element, ElementDisplayingContext context) {


            var bindingInfo = element;
            var partName = bindingInfo.ContentPartName;
            var partPropertyName = bindingInfo.ContentPartMemberExpression;

            var content = context.Content;
            var contentItem = content.ContentItem;
            //if (content.Id > 0) {
            //    /*
            //   context.Content's Version is the publish version, if the content never Published, the field will be null.
            //   so I get the latest (if have draft) ContentItem from ContentManager
            //    */
            //    contentItem = _contentManager.Get(content.Id, VersionOptions.Latest);
            //}


            var member = ContentDataMemberHelper.FindFromContentItem(contentItem, partName, partPropertyName);

            if (context.DisplayType == "Design") {
                context.ElementShape.Member = member;
                context.ElementShape.Context = context;
                context.ElementShape.Content = context.Content;
            }
        }

        //protected override void OnDisplaying(PropertyBindElement element, ElementDisplayingContext context) {
        //    context.ElementShape.HTML = element.Content;
        //}

        //protected override void OnCreatingDisplay(PropertyBindElement element, ElementCreatingDisplayShapeContext context) {
        //    base.OnCreatingDisplay(element, context);
        //}

    }
}