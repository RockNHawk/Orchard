using System;
using System.Collections.Generic;
using System.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Layouts.Elements;
using Orchard.Layouts.Framework.Display;
using Orchard.Layouts.Framework.Elements;
using Orchard.Layouts.Services;
using Orchard.UI.Zones;
using Orchard.Utility.Extensions;
using ContentItem = Orchard.ContentManagement.ContentItem;

namespace MnLab.PdfVisualDesign {
    public class CustomElementDisplay : Component, IElementDisplay {
        private readonly IShapeFactory _shapeFactory;
        private readonly IElementEventHandler _elementEventHandlerHandler;

        ElementDisplay _original;
        public CustomElementDisplay(IShapeFactory shapeFactory, IElementEventHandler elementEventHandlerHandler) {
            _shapeFactory = shapeFactory;
            _elementEventHandlerHandler = elementEventHandlerHandler;
            _original = new ElementDisplay(shapeFactory, elementEventHandlerHandler);
        }

        public dynamic DisplayElement(
            Element element,
            IContent content,
            string displayType = null,
            IUpdateModel updater = null) {
            if (displayType == "Design") {
                displayType = null;
            }
            return _original.DisplayElement(element, content, displayType, updater);
        }

        public dynamic DisplayElements(IEnumerable<Element> elements, IContent content, string displayType = null, IUpdateModel updater = null) {
            if (displayType == "Design") {
                displayType = null;
            }
            return _original.DisplayElements(elements, content, displayType, updater);
        }

    }
}