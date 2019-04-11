using Amba.HtmlBlocks.Elements;
using Amba.HtmlBlocks.Models;
using Orchard.Environment.Extensions;
using Orchard.Layouts.Framework.Display;
using Orchard.Layouts.Framework.Drivers;
using Orchard.Layouts.Helpers;
using Orchard.Layouts.Services;
using Orchard.Layouts.ViewModels;
using AutoMapper;
using Amba.HtmlBlocks.Binding.Elements;

namespace Amba.HtmlBlocks.Binding.Drivers {


    public class TwolevelTitletDriver : ElementDriver<TwolevelTitle> {
        private readonly IElementFilterProcessor _processor;
        public TwolevelTitletDriver(IElementFilterProcessor processor) {
            _processor = processor;
        }
        protected override EditorResult OnBuildEditor(TwolevelTitle element, ElementEditorContext context) {


            var content = context.Content;
            //  var contentItem = context.Content.ContentItem;
            //var contentItem = content.GetLatestVersion(_contentManager);


            //var bindingDefGroups = GetBindingDefGroups(contentItem);
            //Dictionary<string, object> valueMaps = GetValueMaps(contentItem, bindingDefGroups);

            //var viewModel = Mapper.Map(element, new ValueBindGridViewModel() {
            //    BindingDefSources = bindingDefGroups,
            //    DesignData = element.DesignData,
            //    ValueMaps = valueMaps,
            //});

            var viewModel = Mapper.Map(element, new ValueTitleViewModel() {
                DesignData = element.DesignData
            });

            //};
            var editor = context.ShapeFactory.EditorTemplate(TemplateName: "Elements.TwolevelTitle", Model: viewModel);

            if (context.Updater != null) {
                context.Updater.TryUpdateModel(viewModel, context.Prefix, null, null);
                element.DesignData = viewModel.DesignData;
            }

            return Editor(context, editor);
        }

        protected override void OnDisplaying(TwolevelTitle element, ElementDisplayingContext context) {
            var viewModel = Mapper.Map(element, new ValueTitleViewModel() {
                //BindingDefSources = bindingDefGroups,
                DesignData = element.DesignData
                //ValueMaps = valueMaps,
            });
            context.ElementShape.viewMode =viewModel.DesignData;
        }

    }
}