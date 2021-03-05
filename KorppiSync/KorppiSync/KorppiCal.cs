using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KorppiSync
{
    public class KorppiCal
    {
        Page page;

        public KorppiCal()
        {
            Init().Wait();
        }

        private async Task Init()
        {
            Console.WriteLine("Käynnistetään selain...");
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });
            page = await browser.NewPageAsync();
        }

        public async Task GetKorppiCal(string user, string pass)
        {
            NavigationOptions navi = new NavigationOptions();
            navi.WaitUntil = new WaitUntilNavigation[] { WaitUntilNavigation.Networkidle0 };

            Console.WriteLine("Avataan korppi");

            await page.GoToAsync("https://korppi.jyu.fi", navi);
            var navigationTask = page.WaitForNavigationAsync();

            await page.ClickAsync("#toOpenId");
            await navigationTask;

            Console.WriteLine("Kirjaudutaan sisään");
            await page.TypeAsync("#username", user);
            await page.TypeAsync("#password", pass);
            navigationTask = page.WaitForNavigationAsync();
            await page.ClickAsync("#loginbutton");
            await navigationTask;

            Console.WriteLine("Avataan kalenteri");
            await page.GoToAsync("https://korppi.jyu.fi/kotka/calendar/calendar.jsp", navi);

            await page.ScreenshotAsync("testi.png");
            var tekstit = await page.EvaluateExpressionAsync(testi);

            Console.WriteLine(tekstit.ToString());
        }

        /// <summary>
        /// Kalenterin sisältö tekstinä.
        /// </summary>
        static string testi = "function testi(){ return document.getElementsByClassName(\"calendartable\")[0].textContent; } testi();";
    }
}
