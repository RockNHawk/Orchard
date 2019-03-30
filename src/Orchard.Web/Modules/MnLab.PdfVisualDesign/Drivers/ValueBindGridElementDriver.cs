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

        protected override EditorResult OnBuildEditor(ValueBindGridElement element, ElementEditorContext context) {
            var updater = context.Updater;

            var viewModel = Mapper.Map(element, new ValueBindGridViewModel());
            //var viewModel = element;

            if (context.Updater != null) {

                var design = new ValueBindGridData();
                if (!context.Updater.TryUpdateModel(design, context.Prefix, null, null)) {
                    updater.AddModelError("", T("Could not update {0}", nameof(ValueBindGridData)));
                    goto ret;
                }
                else {
                    element.Design = design;
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