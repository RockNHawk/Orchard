using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Layouts.Framework.Display;
using Orchard.Layouts.Services;

namespace MnLab.PdfVisualDesign.Hooks {
    public class ElementEventHandler: ElementEventHandlerBase {
        public override void Creating(ElementCreatingContext context) {
            
            base.Creating(context);
        }


        public override void Displaying(ElementDisplayingContext context) {
            if (context.DisplayType=="Design") {
                context.DisplayType = null;
            }
            base.Displaying(context);
        }

        public override void Created(ElementCreatedContext context) {
            //if (context.ElementDescriptor.type) {

            //}
            base.Created(context);
        }

    }
}