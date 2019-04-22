using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using MnLab.PdfVisualDesign.Binding.Elements;
using MnLab.PdfVisualDesign.ViewModels;
using MnLab.Enterprise;
using MnLab.Enterprise.Approval;
using Orchard.Environment;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Orchard.Layouts.Framework.Display;
using Orchard.Environment.Configuration;
using Orchard.Environment.ShellBuilders;
using Orchard.Mvc.Html;
using Orchard.ContentManagement;
using Orchard;
using System.IO;
using System.Threading.Tasks;
using MnLab.PdfVisualDesign.Services;
using Orchard.Logging;
using Orchard.Localization;

namespace MnLab.PdfVisualDesign {

    /// <summary>
    /// Where can I initialize AutoMapper mappings in an Orchard module?
    /// https://stackoverflow.com/questions/13985267/where-can-i-initialize-automapper-mappings-in-an-orchard-module/16577695
    /// </summary>
    public class OrchardShellEvents : IOrchardShellEvents {


        IWorkContextAccessor _workContextAccessor;
        public Localizer T { get; set; }

        public ILogger Logger { get; set; }

        public OrchardShellEvents(IWorkContextAccessor workContextAccessor, IOrchardHost orchardHost, ShellSettings settings) {
            this._workContextAccessor = workContextAccessor;

              ContentApprovalService.Approved += OnApproved;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;

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

        private void OnApproved(object sender, ApprovalApproveCommand e) {
            var command = e;
            ApprovalEventHandler.OnApproved(command, _workContextAccessor,Logger);
        }



        public void Terminating() {
            //Do nothing
        }
    }
}