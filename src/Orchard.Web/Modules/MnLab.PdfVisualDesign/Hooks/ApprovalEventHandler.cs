using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MnLab.Enterprise.Approval;
using MnLab.Enterprise.Approval.Models;
using MnLab.Enterprise.Approval.Events;
using Orchard.Mvc.Html;
using Orchard.ContentManagement;
using Orchard;
using System.Web.Routing;
using PdfGenerater1 = PdfGenerater.PdfGenerater;
using Orchard.Logging;
using MnLab.PdfVisualDesign.Models;

namespace MnLab.PdfVisualDesign.Services {
    /// <summary>
    /// Not work
    /// </summary>
    public class ApprovalEventHandler : Component, IApprovalEventHandler {

        public static string dir1 = "App_Data/Sites/Default/PDF";


        IWorkContextAccessor _workContextAccessor;
        IOrchardServices _services;

        public ApprovalEventHandler(IWorkContextAccessor workContextAccessor, IOrchardServices services) {
            this._services = services;
            this._workContextAccessor = workContextAccessor;
        }


        public void Approve(ApprovalApproveCommand command) {
            OnApproved(command, _workContextAccessor, Logger);
            _services.TransactionManager.Cancel();
        }

        public void Commit(CreateApprovalCommand command) {
        }

        public void Reject(ApprovalRejectCommand command) {
        }


        public static void OnApproved(ApprovalApproveCommand command, IWorkContextAccessor workContextAccessor, ILogger Logger) {
            var content = command.ContentItem;

            //if (!content.Has<PdfGeneratePart>()) {
            //    return;
            //}

            if (!content.TypeDefinition.Parts.Any(x => x.PartDefinition.Name == nameof(PdfGeneratePart))) {
                return;
            }

            var rq = new RequestContext(workContextAccessor.GetContext().HttpContext, new RouteData());
            var urlHelper = new System.Web.Mvc.UrlHelper(rq);
            var url = ContentItemExtensions.ItemDisplayUrl(urlHelper, command.ContentItem);
            var absoluteUrl = $"{workContextAccessor.GetContext().CurrentSite.BaseUrl}{url}";

            var dir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir1);
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
            var destFileName = $"{content.Id}_{content.Version}.pdf";
            var destFilePath = System.IO.Path.Combine(dir, destFileName);
            if (File.Exists(destFilePath)) {
                File.Move(destFilePath, Path.Combine($"{destFilePath}.duplicate-{Guid.NewGuid().ToString("N")}.pdf"));
            }

            //var pdfOptions = new {
            //    Scale = 1,
            //    DisplayHeaderFooter = true,
            //    HeaderTemplate = "<b style='font-size: 6px'>南华机电有限公司</b>",
            //    FooterTemplate = "<b style='border-top: 1px solid #000;margin: 0 auto;'><div style='padding-top: 5px;'><span  style='font-size: 8px;'>上海南华机电有限公司</span><span  style='font-size: 6px'> 电话：+86 021-39126868 传真： +86 021-39126868 分机 808/818 网址：www.nanhua.com；E-mail:sales@nanhua.com 地址：上海嘉定区北路1755号9号楼 邮编：201802</span></div><div style='width: 100%;text-align: center;padding-top: 5px;'> <span  style='font-size:12px;color: red;'>南华机电版权所有，如无南华书面授权，任何部分不得以任何形式复制或传播</span></div><div style='float: right;'><span class=\"pageNumber\" style='font-size: 8px'></span><span  style='font-size: 8px'>/<span><span class=\"totalPages\" style='font-size: 8px'></span></div></b>",
            //    PrintBackground = true,
            //    Landscape = false,
            //    PageRanges = "",
            //    Format = new {
            //        Width = 8.27,
            //        Height = 11.7
            //    },
            //    //Width = null,
            //    //Height = null,
            //    MarginOptions = new {
            //        Top = "100px",
            //        Left = "30px",
            //        Bottom = "200px",
            //        Right = "30px"
            //    },
            //    PreferCSSPageSize = false
            //};

            //var launchOptions = new {
            //    ExecutablePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules/chromium/win-x64/chrome.exe".Replace('/', System.IO.Path.DirectorySeparatorChar)),
            //    Headless = true
            //};
            //var viewPortOptions = new { };

            try {
                PdfGenerater1 ge = new PdfGenerater1();
                Task.Run(() => ge.GenerateWithWeekReference(absoluteUrl, destFilePath,
                    ReadJsonString("launchOptions"),
                    ReadJsonString("viewPortOptions"),
                    ReadJsonString("pdfOptions")
                    //Newtonsoft.Json.JsonConvert.SerializeObject(launchOptions),
                    //Newtonsoft.Json.JsonConvert.SerializeObject(viewPortOptions),
                    //Newtonsoft.Json.JsonConvert.SerializeObject(pdfOptions)
                    )
                    ).Wait();
            }
            catch (Exception ex) {
                Logger?.Log(Orchard.Logging.LogLevel.Error, ex, "Generate PDF for Content {0}#{1}, Url:{2} Fail:{3}", content.ContentType, content.Id, absoluteUrl, ex.Message);
                throw;
            }

            var part = content.As<PdfGeneratePart>();
            if (part == null) {
                part = new PdfGeneratePart();
            }
            //if (part.Record==null) {

            //}

            part.FileName = destFileName;
            part.CreatedUtc = DateTime.UtcNow;

        }

        //public static T ReadJson<T>(string key) {
        //    return ReadJsonString<T>(path);
        //}

        private static string ReadJsonString(string key) {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules/MnLab.PdfVisualDesign/Assets/PdfGenerater/", key + ".json");
            var content = File.ReadAllText(path);
            return content;
        }

    }
}