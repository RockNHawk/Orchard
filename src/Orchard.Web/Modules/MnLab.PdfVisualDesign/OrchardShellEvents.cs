using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using MnLab.PdfVisualDesign.Binding.Elements;
using MnLab.PdfVisualDesign.ViewModels;
using Orchard.Environment;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Orchard.Layouts.Framework.Display;
using Orchard.Environment.Configuration;
using Orchard.Environment.ShellBuilders;

namespace MnLab.PdfVisualDesign {

    /// <summary>
    /// Where can I initialize AutoMapper mappings in an Orchard module?
    /// https://stackoverflow.com/questions/13985267/where-can-i-initialize-automapper-mappings-in-an-orchard-module/16577695
    /// </summary>
    public class OrchardShellEvents : IOrchardShellEvents {

        public OrchardShellEvents(IOrchardHost orchardHost, ShellSettings settings) {

            /*
            Cannot resolve parameter  ShellContext 
            var registration = RegistrationBuilder
                       .ForType<CustomElementDisplay>()
                       .ExternallyOwned()
                       .As<IElementDisplay>()
                       .InstancePerRequest()
                       .CreateRegistration();

            var shellContext = orchardHost.GetShellContext(settings);

            shellContext.LifetimeScope.ComponentRegistry.Register(registration);

             */
        }

        public void Activated() {
            Mapper.Initialize(cfg => {
                //cfg.AddProfile<AppProfile>();
                cfg.CreateMap<ValueDef, ValueBindItemElement>();
                cfg.CreateMap<ValueBindItemElement, ValueDef>();

                cfg.CreateMap<ValueBindGridElement, ValueBindGridViewModel>();

            });
        }

        public void Terminating() {
            //Do nothing
        }
    }
}