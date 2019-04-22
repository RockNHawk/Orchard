using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MnLab.Enterprise.Approval;
using MnLab.Enterprise.Approval.Events;
using Orchard.Mvc.Html;
using Orchard.ContentManagement;
using Orchard;

namespace MnLab.PdfVisualDesign.Services {
    /// <summary>
    /// Not work
    /// </summary>
    public class ApprovalEventHandler : Component, IApprovalEventHandler {

        public static string dir1 = "App_Data/Sites/Default/PDF";

        public void Approve(ApprovalApproveCommand command) {
            OnApproved(command);
        }

        public void Commit(CreateApprovalCommand command) {
        }

        public void Reject(ApprovalRejectCommand command) {
        }


        public static void OnApproved(ApprovalApproveCommand command) {
            var content = command.ContentItem;

            if (!content.Has<PdfGeneratePart>()) {
                return;
            }

            var part = content.As<PdfGeneratePart>();

            var url = ContentItemExtensions.ItemDisplayUrl(new System.Web.Mvc.UrlHelper(), command.ContentItem);

            var dir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir1);
            if (Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
            var destFileName = $"{content.Id}_{content.Version}.pdf";
            var destFilePath = System.IO.Path.Combine(dir, destFileName);
            if (File.Exists(destFilePath)) {
                File.Move(destFilePath, Path.Combine($"duplicate-{Guid.NewGuid().ToString("N")}-{destFilePath}"));
            }
            var ge = new PdfGenerater.PdfGenerater();

            try {
                Task.Run(() => ge.Generate1(url, destFilePath, null)).Wait();
            }
            catch (Exception ex) {
                throw;
            }

            part.FileName = destFileName;
            part.CreatedUtc = DateTime.UtcNow;
        }


    }
}