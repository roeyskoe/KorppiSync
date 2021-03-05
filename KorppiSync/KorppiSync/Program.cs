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
    class KorppiSync
    {
        static Gcal GoogleCalendar;
        public static void Main()
        {
            GoogleCalendar = new Gcal();

            GoogleCalendar.CreateEvent("Testitapahtuma", new DateTime(2021, 3, 6, 15, 0, 0), new DateTime(2021, 3, 6, 16, 0, 0), "4");

            Console.Read();

        }

    }
}