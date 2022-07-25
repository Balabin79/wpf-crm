using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Dental.Services.Google
{
    public class GoogleService
    {

        /* Global instance of the scopes required by this quickstart.
            If modifying these scopes, delete your previously saved token.json/ folder. */
        private static string[] CalendarScopes = { CalendarService.Scope.Calendar, CalendarService.Scope.CalendarEvents};

        private string ApplicationName = "B6 Sheduler";

        public CalendarService Get()
        {
            string PathTo = "pack://application:,,,/Resources/Auth/credentials.json";
            UserCredential credential;

            // Load client secrets.
            using (var stream = new FileStream(PathTo, FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    CalendarScopes, "alexbalabin79@gmail.com",
                    CancellationToken.None, new FileDataStore(credPath, true)).Result;
            }

            // Create Google Calendar API service.
            return new CalendarService(new BaseClientService.Initializer { HttpClientInitializer = credential, ApplicationName = ApplicationName });
        }
      
    }
}
