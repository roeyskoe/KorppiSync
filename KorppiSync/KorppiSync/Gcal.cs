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
using System.Threading;
using System.Threading.Tasks;
namespace KorppiSync
{
    /// <summary>
    /// Vastaa Googlen kalenterin kanssa kommunikoinnista.
    /// </summary>
    public class Gcal
    {
        public CalendarService service;
        static string[] Scopes = { CalendarService.Scope.CalendarEvents }; // Ohjelman toimivaltuudet
        static string ApplicationName = "Korpin kalenterisynkronointi Googlekalenteriin";
        string calendarId;

        /// <summary>
        /// Initialisoidaan googlekalenteri
        /// </summary>
        public Gcal(string calendarId)
        {
            this.calendarId = calendarId;
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Calendar API service.
            service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        /// <summary>
        /// Luodaan uusi tapahtuma
        /// </summary>
        /// <param name="title"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="colorId"></param>
        public Event CreateEvent(string title, string location, string description, DateTime start, DateTime end, string colorId = "1", bool noinsert = false)
        {
            Event e = new Event();
            e.Summary = title;
            e.Location = location;
            e.Description = description;

            EventDateTime ed1 = new EventDateTime();
            ed1.DateTime = start;
            e.Start = ed1;

            EventDateTime ed2 = new EventDateTime();
            ed2.DateTime = end;
            e.End = ed2;

            e.ColorId = colorId;
            if(!noinsert)
                service.Events.Insert(e, calendarId).Execute();
            return e;
        }

        /// <summary>
        /// Päivittää tapahtuman tiedot.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="calendarId"></param>
        public void UpdateEvent(Event e)
        {
            service.Events.Update(e, calendarId, e.Id).Execute();
        }

        /// <summary>
        /// Hakee tapahtumat meneillään olevalta viikolta ja sitä uudemmat.
        /// </summary>
        /// <param name="calendarId">KalenteriID</param>
        /// <param name="maxResults"></param>
        /// <returns></returns>
        public Events GetEvents(int maxResults = 100)
        {
            EventsResource.ListRequest request = service.Events.List(calendarId);
            request.TimeMin = DateTime.Now.AddDays(-7);
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = maxResults;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            Events events = request.Execute();
            return events;
        }
    }
}
