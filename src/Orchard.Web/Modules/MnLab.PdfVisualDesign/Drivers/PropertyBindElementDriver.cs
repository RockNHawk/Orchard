using MnLab.PdfVisualDesign.HtmlBlocks.Elements;
using MnLab.PdfVisualDesign.ViewModels;
using Orchard.Environment.Extensions;
using Orchard.Layouts.Framework.Display;
using Orchard.Layouts.Framework.Drivers;
using Orchard.Layouts.Helpers;
using Orchard.Layouts.Services;
using Orchard.Layouts.ViewModels;

namespace MnLab.PdfVisualDesign.HtmlBlocks.Drivers {


    public class PropertyBindElementDriver : ElementDriver<PropertyBindElement> {
        private readonly IElementFilterProcessor _processor;
        public PropertyBindElementDriver(IElementFilterProcessor processor)
        {
            _processor = processor;
        }

        protected override EditorResult OnBuildEditor(PropertyBindElement element, ElementEditorContext context)
        {
            var viewModel = new PropertyBindViewModel {
                //Text = element.Content
            };
            var editor = context.ShapeFactory.EditorTemplate(TemplateName: $"Elements.{nameof(PropertyBindElement)}", Model: viewModel);

            if (context.Updater != null) {
                context.Updater.TryUpdateModel(viewModel, context.Prefix, null, null);
                element.PropertyExpression = viewModel.PropertyExpression;
            }
            
            return Editor(context, editor);
        }

        protected override void OnDisplaying(PropertyBindElement element, ElementDisplayingContext context)
        {            
            context.ElementShape.HTML = element.Content;
        }
        
    }
}