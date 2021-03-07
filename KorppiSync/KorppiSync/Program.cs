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
            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText("settings.json"));

            GoogleCalendar = new Gcal(data["calendarId"]);
            KorppiCal = new KorppiCal();
            
            List<Kalenterimerkinta> kalenterimerkinnat = await KorppiCal.GetKorppiCal(data["korppiUser"], data["korppiPass"]);

            Console.WriteLine("Lisätään kalenteriin...");
            Events events = GoogleCalendar.GetEvents();

            foreach (var kalenterimerkinta in kalenterimerkinnat)
            {
                Event e = EventByDate(events, kalenterimerkinta.AlkuAika);
                if(e is null)
                {
                    GoogleCalendar.CreateEvent(kalenterimerkinta.Otsikko, kalenterimerkinta.Paikka, kalenterimerkinta.Kuvaus, kalenterimerkinta.AlkuAika, kalenterimerkinta.LoppuAika, kalenterimerkinta.VariId);
                }
                else
                {
                    e.Summary = kalenterimerkinta.Otsikko;
                    e.ColorId = kalenterimerkinta.VariId;
                    e.Location = kalenterimerkinta.Paikka;
                    e.Description = kalenterimerkinta.Kuvaus;
                    GoogleCalendar.UpdateEvent(e);
                }
            }

            Console.WriteLine("valmis");
        }

        /// <summary>
        /// Onko kyseiselle ajanhetkelle jo olemassa tapahtumaa
        /// </summary>
        /// <param name="events">Tapahtumat</param>
        /// <param name="date">ajanhetki</param>
        /// <returns>tapahtuma jos sellainen on, muuten null</returns>
        public static Event EventByDate(Events events, DateTime date)
        {
            foreach (var item in events.Items)
            {
                if (item.Start.DateTime == date) return item;
            }
            return null;
        }
    }
}