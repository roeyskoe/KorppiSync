using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace KorppiSync
{
    class KorppiSync
    {
        static Gcal GoogleCalendar;
        static KorppiCal KorppiCal;
        public static async Task Main()
        {
            
            GoogleCalendar = new Gcal();
            KorppiCal = new KorppiCal();
            /*
            GoogleCalendar.CreateEvent("Testitapahtuma", new DateTime(2021, 3, 6, 15, 0, 0), new DateTime(2021, 3, 6, 16, 0, 0), "4");
            Console.WriteLine("a");
            */
            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(System.IO.File.ReadAllText("settings.json"));

            await KorppiCal.GetKorppiCal(data["korppiUser"], data["korppiPass"]);

            Console.WriteLine("valmis");

        }

    }
}