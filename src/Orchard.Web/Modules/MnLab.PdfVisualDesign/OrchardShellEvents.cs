using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using MnLab.PdfVisualDesign.Binding.Elements;
using MnLab.PdfVisualDesign.ViewModels;
using Orchard.Environment;

namespace MnLab.PdfVisualDesign {

    /// <summary>
    /// Where can I initialize AutoMapper mappings in an Orchard module?
    /// https://stackoverflow.com/questions/13985267/where-can-i-initialize-automapper-mappings-in-an-orchard-module/16577695
    /// </summary>
    public class OrchardShellEvents : IOrchardShellEvents {
        public void Activated() {
            Mapper.Initialize(cfg => {
                //cfg.AddProfile<AppProfile>();
                cfg.CreateMap<ValueBindingDef, PropertyBindElement>();
                cfg.CreateMap<PropertyBindElement, ValueBindingDef>();
            });
        }

        public void Terminating() {
            //Do nothing
        }
    }
}