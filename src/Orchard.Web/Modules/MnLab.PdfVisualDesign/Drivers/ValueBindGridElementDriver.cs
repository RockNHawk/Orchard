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

        /// <summary>
        /// Get ContentPart's IValueBindingDef from ContentPartDefinition
        /// </summary>
        /// <param name="def"></param>
        /// <returns></returns>
        static IList<IValueBindingDef> GetBindingItems(ContentPartDefinition def) {
            // Fields Def, Field can dynamic added by user
            var fields = def.Fields;
            /*
            Property is a .NET Property , it's hard-code in the ContentPart,sucha as TitlePart.Title is definded as:
            public class TitlePart : ContentPart{
                public string Title {get..., set...}
            }
             */
            var properties = def.GetType().GetProperties(BindingFlags.Public);

            var items = new List<IValueBindingDef>((fields?.Count() ?? 0) + properties.Count());
            if (fields != null) {
                foreach (var item in fields) {
                    // Field 之下还有子属性，这里只处理了它本身
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

            var content = context.Content;
          //  var contentItem = context.Content.ContentItem;
            var contentItem = content.GetLatestVersion(_contentManager);


            var bindingDefGroups = GetBindingDefGroups(contentItem);
            Dictionary<string, object> valueMaps = GetValueMaps(contentItem, bindingDefGroups);

            var viewModel = Mapper.Map(element, new ValueBindGridViewModel() {
                BindingDefSources = bindingDefGroups,
                DesignData = element.DesignData,
                ValueMaps = valueMaps,
            });
            //var viewModel = element;

            //var helper = new ContentDataMemberHelper(part, bindingDef);

            if (context.Updater != null) {


                var design = new ValueBindGridData();

                updater.TryUpdateModel(design, GetPrefix(context, $"{nameof(viewModel.DesignData)}"),null,null);

                var strAllCellValues = ((string[])context.ValueProvider.GetValue($"{nameof(viewModel.DesignData)}.{nameof(viewModel.DesignData.AllCellValues)}_JSON")?.RawValue)?[0];
                if (!string.IsNullOrEmpty(strAllCellValues)) {
                    design.AllCellValues = Newtonsoft.Json.JsonConvert.DeserializeObject<ValueBindingDef[][]>(strAllCellValues);
                }
                {
                    var strMergedCells = ((string[])context.ValueProvider.GetValue($"{nameof(viewModel.DesignData)}.{nameof(viewModel.DesignData.MergedCells)}_JSON")?.RawValue)?[0];
                    if (!string.IsNullOrEmpty(strMergedCells)) {
                        design.MergedCells = Newtonsoft.Json.JsonConvert.DeserializeObject<MergedCell[]>(strMergedCells);
                    }
                }

                {
                    var json = ((string[])context.ValueProvider.GetValue($"{nameof(viewModel.DesignData)}.{nameof(viewModel.DesignData.HeaderTexts)}_JSON")?.RawValue)?[0];
                    if (!string.IsNullOrEmpty(json)) {
                        design.HeaderTexts = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(json);
                    }
                }

                element.DesignData = viewModel.DesignData = design;

                //if (!context.Updater.TryUpdateModel(viewModel, context.Prefix, null, null)) {
                //    updater.AddModelError("", T("Could not update {0}", nameof(ValueBindGridData)));
                //    goto ret;
                //}
                //else {
                //    element.DesignData = viewModel.DesignData;
                //}

            }

            ret:

            var editor = context.ShapeFactory.EditorTemplate(TemplateName: $"Elements.{nameof(ValueBindGridElement)}", Model: viewModel);
            return Editor(context, editor);
        }

        public static IEnumerable<Grouping<ContentPartDefinition, IValueBindingDef>> GetBindingDefGroups(ContentItem contentItem) {
            // in 'Create' page, the content is empty
            return contentItem.TypeDefinition.Parts?
                .Select(x => x.PartDefinition)
                .GroupBy(x => x)
                // the key is ContentPartDefinition
                .Select(x => new Grouping<ContentPartDefinition, IValueBindingDef>(x.Key, GetBindingItems(x.Key)))
                .Where(x => x.Count() > 0);
                ;
        }

        /// <summary>
        /// 根据 bindingDefGroups 获取所有 Field/Property 的值
        /// </summary>
        /// <param name="contentItem"></param>
        /// <param name="bindingDefGroups"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetValueMaps(ContentItem contentItem, IEnumerable<Grouping<ContentPartDefinition, IValueBindingDef>> bindingDefGroups) {
            var valueMaps = new Dictionary<string, object>();
            foreach (var group in bindingDefGroups) {
                var partDef = group.Key;
                var part = contentItem.Parts.First(x => x.PartDefinition == partDef);
                foreach (var item in group) {
                    var helper = ContentPartDataMemberHelper.FindFromContentPart(part, item);
                    //var helper = new ContentDataMemberHelper(part, item);
                    try {
                        var value = helper.GetAccessor().GetObject();
                        //var value = helper.GetAccessor().GetValue();
                        valueMaps[item.Key] = value;
                    }
                    catch (Exception ex) {
                        valueMaps[item.Key] = ex;
                        //throw;
                    }
                }
            }

            return valueMaps;
        }

        protected override void OnDisplaying(ValueBindGridElement element, ElementDisplayingContext context) {

            var content = context.Content;
            //  var contentItem = context.Content.ContentItem;
            var contentItem = content.GetLatestVersion(_contentManager);

            var bindingDefGroups = GetBindingDefGroups(contentItem);
                // the bind member key and value map
            Dictionary<string, object> valueMaps = GetValueMaps(contentItem, bindingDefGroups);

            var viewModel = Mapper.Map(element, new ValueBindGridViewModel() {
                BindingDefSources = bindingDefGroups,
                DesignData = element.DesignData,
                ValueMaps = valueMaps,
            });

            context.ElementShape.ViewModel = viewModel;
        }

        //protected override void OnDisplaying(PropertyBindElement element, ElementDisplayingContext context) {
        //    context.ElementShape.HTML = element.Content;
        //}

        //protected override void OnCreatingDisplay(PropertyBindElement element, ElementCreatingDisplayShapeContext context) {
        //    base.OnCreatingDisplay(element, context);
        //}

        private static string GetPrefix(ElementEditorContext context, string name) {
            return $"{(string.IsNullOrEmpty(context.Prefix) ? null : ".")}{name}";
        }


    }
}