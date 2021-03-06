using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KorppiSync
{
    /// <summary>
    /// Keskustelee korpin kalenterin kanssa.
    /// </summary>
    public class KorppiCal
    {
        Page page;
        public Dictionary<string, string>[] ajat;

        public KorppiCal()
        {
            Init().Wait();
        }

        /// <summary>
        /// Initialisoi käytettävän nettiselaimen.
        /// </summary>
        /// <returns></returns>
        private async Task Init()
        {
            if(!Directory.Exists(".local-chromium"))
                Console.WriteLine("Selainta ladataan, tässä kestää hetki...");
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);

            Console.WriteLine("Käynnistetään selain...");
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });
            page = await browser.NewPageAsync();
        }

        /// <summary>
        /// Kirjautuu korppiin ja hakee kalenterin
        /// </summary>
        /// <param name="user">käyttäjätunnus</param>
        /// <param name="pass">salasana</param>
        /// <returns></returns>
        public async Task<List<Kalenterimerkinta>> GetKorppiCal(string user, string pass)
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

            //await page.ScreenshotAsync("testi.png");
            // Suoritetaan sivulla vähän javascriptiä jotta saadaan kalenteridata mukavammassa muodossa
            await page.EvaluateExpressionAsync(File.ReadAllText("kalenteri.js"));

            await Task.Delay(2000); // Pitää odottaa hetki että JS-kirjasto on varmasti ladattu

            string tekstit = "";
            try
            {
                tekstit = (await page.EvaluateExpressionAsync("Lue();")).ToString();
            }
            catch (Exception e) { Console.WriteLine(e); }

            // Tulee vähän rumassa muodossa.
            ajat = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>[]>(tekstit);
            Console.WriteLine("parsitaan...");
            return Format();
        }

        /// <summary>
        /// Formatoi kalenteritapahtumat helpompaan muotoon
        /// </summary>
        /// <returns>(Alkuaika, loppuaika, kuka)</returns>
        private List<Kalenterimerkinta> Format()
        {
            List<Kalenterimerkinta> kalenteri = new();

            foreach (var rivi in ajat)
            {
                foreach (var item in rivi)
                {
                    if(item.Value != "[+]" && item.Key != "Kello")
                    {
                        // pvm on muotoa "Ke 3.3."
                        string[] pvm = item.Key.Split(" ")[1].Split(".");
                        string[] arvo = item.Value.Replace("[+]", "").Split("\n");

                        string[] aika = arvo[0].Split(" - ");
                        string[][] ajat = new string[][] { aika[0].Split(":"), aika[1].Split(":") };

                        DateTime alkuaika = new DateTime(DateTime.Now.Year, int.Parse(pvm[1]), int.Parse(pvm[0]), int.Parse(ajat[0][0]), int.Parse(ajat[0][1]), 0);
                        DateTime loppuaika = new DateTime(alkuaika.Year, alkuaika.Month, alkuaika.Day, int.Parse(ajat[1][0]), int.Parse(ajat[1][1]), 0);

                        Kalenterimerkinta km = new Kalenterimerkinta(alkuaika, loppuaika, arvo[1]);
                       
                        if (arvo[1] != "Varattava aika")
                        {
                            km.Varattu = false;
                            km.VariId = "11";
                        }
                        else
                        {
                            km.Varattu = true;
                            km.VariId = "10";
                        }
                        kalenteri.Add(km);
                    }
                }
            }

            return kalenteri;
        }

    }

    /// <summary>
    /// Apuluokka kalenterimerkintöjen käsittelyyn
    /// </summary>
    public class Kalenterimerkinta
    {
        public DateTime AlkuAika;
        public DateTime LoppuAika;
        public string Otsikko;
        public string VariId;
        public bool Varattu;

        public Kalenterimerkinta(DateTime alku, DateTime loppu, string otsikko)
        {
            AlkuAika = alku;
            LoppuAika = loppu;
            Otsikko = otsikko;
        }
    }


}
