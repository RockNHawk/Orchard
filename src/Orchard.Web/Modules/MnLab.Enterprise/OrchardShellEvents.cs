using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
//using MnLab.PdfVisualDesign.Binding.Elements;
//using MnLab.PdfVisualDesign.ViewModels;
using Orchard.Environment;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Orchard.Layouts.Framework.Display;
using Orchard.Environment.Configuration;
using Orchard.Environment.ShellBuilders;
using MnLab.Enterprise.Approval;using MnLab.Enterprise.Approval.Models;
using MnLab.Enterprise.Approval.Models;

namespace MnLab.Enterprise {

    /// <summary>
    /// Where can I initialize AutoMapper mappings in an Orchard module?
    /// https://stackoverflow.com/questions/13985267/where-can-i-initialize-automapper-mappings-in-an-orchard-module/16577695
    /// </summary>
    public class OrchardShellEvents : IOrchardShellEvents {

        public OrchardShellEvents(IOrchardHost orchardHost, ShellSettings settings) {

        }

        public void Activated() {
            Mapper.Initialize(cfg => {
                cfg.CreateMap<ApprovalPartRecord, ApprovalPart>();
                cfg.CreateMap<ApprovalPart, ApprovalPartRecord>();
                cfg.CreateMap<ApprovalPartRecord, ApprovalPartRecord>();
            });
        }

        public void Terminating() {
            //Do nothing
        }
    }
}