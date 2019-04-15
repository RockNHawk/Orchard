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
using Orchard.Localization;
using Orchard.Logging;
using MnLab.PdfVisualDesign.Elements;
using Newtonsoft.Json.Linq;

namespace MnLab.PdfVisualDesign.Drivers {

    public class PlaceholderModelMap : LayoutModelMapBase<Placeholder> {
        protected override void ToElement(Placeholder element, JToken node) {
            base.ToElement(element, node);
            element.PlaceId = node["PlaceId"].Value<string>();
        }

        public override void FromElement(Placeholder element, DescribeElementsContext describeContext, JToken node) {
            base.FromElement(element, describeContext, node);
            node["PlaceId"] = element.PlaceId;
        }
    }


    public class PlaceholderDriver : ElementDriver<Placeholder> {


        private readonly IElementFilterProcessor _processor;

        private readonly IShapeFactory _shapeFactory;

        private readonly IEnumerable<IContentFieldDriver> _contentFieldDrivers;

        readonly IContentFieldDisplay _fieldDisplay;



        private readonly Lazy<IShapeTableLocator> _shapeTableLocator;

        private readonly IVirtualPathProvider _virtualPathProvider;

        IContentManager _contentManager;

        readonly IWorkContextAccessor _workContextAccessor;

        // public Localizer T { get; set; }

        //  public ILogger Logger { get; set; }

        public PlaceholderDriver(IElementFilterProcessor processor, IContentFieldDisplay fieldDisplay,
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
            // T = NullLocalizer.Instance;
            // Logger = NullLogger.Instance;
        }
 

        //protected override EditorResult OnBuildEditor(Placeholder element, ElementEditorContext context) {
        //    var updater = context.Updater;
           

        //    var editor = context.ShapeFactory.EditorTemplate(TemplateName: $"Elements/{nameof(Placeholder)}", Model: null);
        //    return Editor(context, editor);
        //}
  
        protected override void OnDisplaying(Placeholder element, ElementDisplayingContext context) {

            //var viewModel = Mapper.Map(element, new ValueBindGridViewModel() {
            //    BindingDefSources = bindingDefGroups,
            //    DesignData = element.DesignData,
            //    ValueMaps = valueMaps,
            //});

            //context.ElementShape.ViewModel = viewModel;
        }


        private static string GetPrefix(ElementEditorContext context, string name) {
            return $"{(string.IsNullOrEmpty(context.Prefix) ? null : ".")}{name}";
        }


    }
}