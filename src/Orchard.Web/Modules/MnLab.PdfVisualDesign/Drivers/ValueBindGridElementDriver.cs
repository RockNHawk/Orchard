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
using Orchard.ContentManagement.MetaData.Models;
using System.Collections;

namespace MnLab.PdfVisualDesign.Binding.Drivers {



    public class Grouping<TKey, TElement> : IGrouping<TKey, TElement> {

        IEnumerable<TElement> elements;
        public Grouping(TKey key, IEnumerable<TElement> elements) {
            this.Key = key;
            this.elements = elements;
        }
        public TKey Key { get; set; }

        public IEnumerator<TElement> GetEnumerator() {
            return elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }


    public class ValueBindGridElementDriver : ElementDriver<ValueBindGridElement> {


        private readonly IElementFilterProcessor _processor;

        private readonly IShapeFactory _shapeFactory;

        private readonly IEnumerable<IContentFieldDriver> _contentFieldDrivers;

        readonly IContentFieldDisplay _fieldDisplay;



        private readonly Lazy<IShapeTableLocator> _shapeTableLocator;

        private readonly IVirtualPathProvider _virtualPathProvider;

        IContentManager _contentManager;

        readonly IWorkContextAccessor _workContextAccessor;


        public ValueBindGridElementDriver(IElementFilterProcessor processor, IContentFieldDisplay fieldDisplay,
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


        static IList<IValueBindingDef> GetBindingItems(ContentPartDefinition def) {
            var fields = def.Fields;
            var properties = def.GetType().GetProperties(BindingFlags.Public);

            var items = new List<IValueBindingDef>((fields?.Count() ?? 0) + properties.Count());
            if (fields != null) {
                foreach (var item in fields) {
                    items.Add(new ValueBindingDef() {
                        ContentPartName = def.Name,
                        MemberExpression = item.Name,
                    });
                }
            }
            if (properties != null) {
                foreach (var item in properties) {
                    items.Add(new ValueBindingDef() {
                        ContentPartName = def.Name,
                        MemberExpression = item.Name,
                    });
                }
            }

            return items;
        }

        protected override EditorResult OnBuildEditor(ValueBindGridElement element, ElementEditorContext context) {
            var updater = context.Updater;


            var contentItem = context.Content.ContentItem;
            // in 'Create' page, the content is empty
            var bindingDefGroups = contentItem.TypeDefinition.Parts?
                .Select(x => x.PartDefinition)
                .GroupBy(x => x)
                .Select(x => new Grouping<ContentPartDefinition, IValueBindingDef>(x.Key, GetBindingItems(x.Key)))
                //.GroupBy(x => x, v => GetBindingItems(v))
                ;

            //var bindingDefs = bindingDefGroups.ToArray();


            var valueMaps = new Dictionary<string, object>();
            foreach (var group in bindingDefGroups) {
                var partDef = group.Key;
                var part = contentItem.Parts.First(x => x.PartDefinition == partDef);
                foreach (var item in group) {
                    var helper = ContentDataMemberHelper.FindFromContentPart(part, item);
                    //var helper = new ContentDataMemberHelper(part, item);
                    try {
                        var value = helper.GetAccessor().GetValue();
                        valueMaps[item.Key] = value;
                    }
                    catch (Exception ex) {
                        valueMaps[item.Key] = ex;
                        //throw;
                    }
                }
            }

            var viewModel = Mapper.Map(element, new ValueBindGridViewModel() {
                BindingDefSources = bindingDefGroups,
                DesignData = element.DesignData,
                ValueMaps = valueMaps,
            });
            //var viewModel = element;

            //var helper = new ContentDataMemberHelper(part, bindingDef);

            if (context.Updater != null) {

                var design = new ValueBindGridData();
                if (!context.Updater.TryUpdateModel(design, context.Prefix, null, null)) {
                    updater.AddModelError("", T("Could not update {0}", nameof(ValueBindGridData)));
                    goto ret;
                }
                else {
                    element.DesignData = design;
                }

            }

            ret:

            var editor = context.ShapeFactory.EditorTemplate(TemplateName: $"Elements.{nameof(ValueBindGridElement)}", Model: viewModel);
            return Editor(context, editor);
        }

        protected override void OnDisplaying(ValueBindGridElement element, ElementDisplayingContext context) {

        }

        //protected override void OnDisplaying(PropertyBindElement element, ElementDisplayingContext context) {
        //    context.ElementShape.HTML = element.Content;
        //}

        //protected override void OnCreatingDisplay(PropertyBindElement element, ElementCreatingDisplayShapeContext context) {
        //    base.OnCreatingDisplay(element, context);
        //}

    }
}