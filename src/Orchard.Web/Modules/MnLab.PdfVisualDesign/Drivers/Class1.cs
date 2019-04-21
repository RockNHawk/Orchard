using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.DisplayManagement.Descriptors;

namespace MnLab.PdfVisualDesign.Drivers {
    public class EditorFieldShapeProvider : IShapeTableProvider {
        public void Discover(ShapeTableBuilder builder) {
            builder.Describe("SummaryAdmin")
                .OnDisplaying(displaying => {
                    var shape = displaying.Shape;
                });
        }
    }

}