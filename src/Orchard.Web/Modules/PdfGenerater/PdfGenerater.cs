﻿using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace PdfGenerater {
    public class PdfGenerater {

        public async Task Generate(string url, string destFile, string pdfOptionsJson) {
            var launchOptions = new LaunchOptions {
                Headless = true
            };
            PdfOptions pdfOptions = new PdfOptions() {
                DisplayHeaderFooter = true,
                PrintBackground = true,
                HeaderTemplate = "<b style='font-size: 6px'>南华机电有限公司</b>",
                FooterTemplate = "<b style='border-top: 1px solid #000;margin: 0 auto;'><div style='padding-top: 5px;'><span  style='font-size: 8px;'>上海南华机电有限公司</span><span  style='font-size: 6px'> 电话：+86 021-39126868 传真： +86 021-39126868 分机 808/818 网址：www.nanhua.com；E-mail:sales@nanhua.com 地址：上海嘉定区北路1755号9号楼 邮编：201802</span></div>" +
              "<div style='width: 100%;text-align: center;padding-top: 5px;'> <span  style='font-size:12px;color: red;'>南华机电版权所有，如无南华书面授权，任何部分不得以任何形式复制或传播</span></div><div style='float: right;'><span class=\"pageNumber\" style='font-size: 8px'></span><span  style='font-size: 8px'>/<span><span class=\"totalPages\" style='font-size: 8px'></span></div></b>",
                Format = PaperFormat.A4,
                MarginOptions = new MarginOptions {
                    Top = "100px",
                    Bottom = "200px",
                    Right = "30px",
                    Left = "30px",
                }
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(pdfOptions);
            Console.WriteLine(json);
            //Path.Combine(Directory.GetCurrentDirectory(), "google.pdf")
            await Generate(url, destFile, launchOptions, pdfOptions);
        }

        public async Task Generate(string url, string destFile, LaunchOptions launchOptions, PdfOptions pdfOptions) {

            //Console.WriteLine("下载chromium");
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            //var url = "http://localhost:30325/%E4%BA%A7%E5%93%81%E6%8F%8F%E8%BF%B0%E4%B8%8E%E5%BA%94%E7%94%A8";
            //var header = "";
            using (var browser = await Puppeteer.LaunchAsync(launchOptions))
            using (var page = await browser.NewPageAsync()) {
                await page.GoToAsync(url);

                //Console.WriteLine("导出 PDF");
                await page.EmulateMediaAsync(MediaType.Screen);
                ViewPortOptions vpos = new ViewPortOptions();
                vpos.IsMobile = true;
                await page.SetViewportAsync(vpos);
                await page.SetJavaScriptEnabledAsync(true);
                //await page.WaitForSelectorAsync()
                //await page.setDefaultNavigationTimeout（timeout）
                await page.ReloadAsync();


                //pdfop.PrintBackground
                await page.WaitForTimeoutAsync(5000);
                await page.PdfAsync(destFile, pdfOptions);
                //await Page.EmulateMediaAsync(WaitForSelectorOptions)
                //Console.WriteLine("Export 成功");

                //if (!args.Any(arg => arg == "auto-exit")) {
                //    Console.ReadLine();
                //}
            }
        }

    }
}
