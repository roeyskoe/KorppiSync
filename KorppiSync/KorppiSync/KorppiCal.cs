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

            DateTime nyt = DateTime.Now;
            DateTime loppuAika = nyt.AddMonths(1); // Haetaan tapahtumia kuukausi eteenpäin tästä hetkestä.
            string nytstr = nyt.ToString("dd.MM.yyyy");
            string loppustr = loppuAika.ToString("dd.MM.yyyy");
            string url = $"https://korppi.jyu.fi/kotka/calendar/calendar.jsp?date={nytstr}&enddate={loppustr}&type=list";

            await page.GoToAsync(url, navi);

            //await page.ScreenshotAsync("testi.png");
            // Suoritetaan sivulla vähän javascriptiä jotta saadaan kalenteridata mukavammassa muodossa
            string tekstit = (await page.EvaluateExpressionAsync(File.ReadAllText("kalenteri.js"))).ToString();

            Console.WriteLine("parsitaan...");
            return Format(tekstit);
        }

        /// <summary>
        /// Formatoi kalenteritapahtumat helpompaan muotoon
        /// </summary>
        /// <returns>(Alkuaika, loppuaika, kuka)</returns>
        private List<Kalenterimerkinta> Format(string teksti)
        {
            List<Kalenterimerkinta> kalenteri = new();

            string[] ajat = teksti.Split("\n");
            
            foreach (var rivi in ajat)
            {
                string[] rivisplit = rivi.Split("\t");
                string[] kellot = rivisplit[1].Split(" - ");
                string[][] pvmt = new string[][] { rivisplit[0].Split("."), kellot[0].Split(":"), kellot[1].Split(":") };

                DateTime alkuaika = new DateTime(int.Parse(pvmt[0][2]), int.Parse(pvmt[0][1]), int.Parse(pvmt[0][0]), int.Parse(pvmt[1][0]), int.Parse(pvmt[1][1]), 0);
                DateTime loppuaika = new DateTime(int.Parse(pvmt[0][2]), int.Parse(pvmt[0][1]), int.Parse(pvmt[0][0]), int.Parse(pvmt[2][0]), int.Parse(pvmt[2][1]), 0);

                Kalenterimerkinta km;
                if (rivisplit.Length > 2)
                    km = new Kalenterimerkinta(alkuaika, loppuaika, rivisplit[2], rivisplit[3]);
                else
                    km = new Kalenterimerkinta(alkuaika, loppuaika, rivisplit[2], "");

                if (rivisplit[2] != "Varattava aika")
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
        public string Paikka;
        public string VariId;
        public bool Varattu;

        public Kalenterimerkinta(DateTime alku, DateTime loppu, string otsikko, string paikka)
        {
            AlkuAika = alku;
            LoppuAika = loppu;
            Otsikko = otsikko;
            Paikka = paikka;
        }
    }


}
