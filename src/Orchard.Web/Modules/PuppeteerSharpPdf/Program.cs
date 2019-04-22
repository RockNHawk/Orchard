using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace PuppeteerSharpPdf {
    class MainClass {
        public static async Task Main(string[] args) {
            var options = new LaunchOptions {
                //      ExecutablePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules/local-chromium/win-x64/chrome.exe"),
                Headless = true
            };

            //   Console.WriteLine("下载chromium");
            //  await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            //   var url = "http://10.211.55.3:9090/p1";
            //  var url = "http://192.168.31.65:9090/p1";
            var url = "http://localhost:9090/p1";
            // var url = "http://qq.com/";
            var header = "";
            using (var browser = await Puppeteer.LaunchAsync(options))
            using (var page = await browser.NewPageAsync()) {
                await page.GoToAsync(url, 60 * 1000);

                Console.WriteLine("导出 PDF");
                await page.EmulateMediaAsync(MediaType.Screen);
                ViewPortOptions vpos = new ViewPortOptions();
                vpos.IsMobile = true;
                await page.SetViewportAsync(vpos);
                await page.SetJavaScriptEnabledAsync(true);
                //await page.WaitForSelectorAsync()
                //await page.setDefaultNavigationTimeout（timeout）
                await page.ReloadAsync();

                PdfOptions pdfop = new PdfOptions() {
                    DisplayHeaderFooter = true,
                    PrintBackground = true,
                    HeaderTemplate = "<b style='font-size: 6px'>海南华级电有限公司</b>",
                    FooterTemplate = "<b style='border-top: 1px solid #000;margin: 0 auto;'><div style='padding-top: 5px;'><span  style='font-size: 8px;'>上海南华级电有限公司</span><span  style='font-size: 6px'> 电话：+86 021-39126868 传真： +86 021-39126868 分机 808/818 网址：www.nanhua.com；E-mail:sales@nanhua.com 地址：上海嘉定区北路1755号9号楼 邮编：201802</span></div>" +
                    "<div style='width: 100%;text-align: center;padding-top: 5px;'> <span  style='font-size:12px;color: red;'>南华机电版权所有，如无南华书面授权，任何部分不得以任何形式复制或传播</span></div><div style='float: right;'><span class=\"pageNumber\" style='font-size: 8px'></span><span  style='font-size: 8px'>/<span><span class=\"totalPages\" style='font-size: 8px'></span></div></b>",
                    Format = PaperFormat.A4,
                    MarginOptions = new MarginOptions {
                        Top = "100px",
                        Bottom = "200px",
                        Right = "30px",
                        Left = "30px",
                    }
                };
                //pdfop.PrintBackground
                await page.WaitForTimeoutAsync(5000);
                await page.PdfAsync(Path.Combine(Directory.GetCurrentDirectory(), "google.pdf"), pdfop)
                    ;
                //await Page.EmulateMediaAsync(WaitForSelectorOptions)
                Console.WriteLine("Export 成功");

                if (!args.Any(arg => arg == "auto-exit")) {
                    Console.ReadLine();
                }
            }
        }

    }
}
