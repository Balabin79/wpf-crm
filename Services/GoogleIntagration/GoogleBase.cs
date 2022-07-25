using Dental.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.PeopleService.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Resources;

namespace Dental.Services.GoogleIntagration
{
    public abstract class GoogleBase
    {
        public const string APPLICATION_NAME = "B6 Sheduler";
        protected readonly ApplicationContext db;
        protected readonly string GoogleAccount;
        protected readonly string OrgName;
        protected int? IsUseGoogleIntegration { get; set; }
        protected UserCredential Credential { get; set; }
        protected readonly string[] scopes = new string[] 
        {
            PeopleServiceService.Scope.Contacts,
            CalendarService.Scope.Calendar, 
            CalendarService.Scope.CalendarEvents
        }; 

        public  GoogleBase()
        {
            try
            {
                db = new ApplicationContext();
                var settings = db.Settings.FirstOrDefault();
                if (settings == null || settings.IsUseGoogleIntegration == null || settings.GoogleAccount == null || scopes.Count() == 0) return;
                GoogleAccount = settings.GoogleAccount;
                IsUseGoogleIntegration = settings.IsUseGoogleIntegration;
                Init();
            }
            catch(Exception e)
            {

            }
        }

        public async void Init()
        {
            try
            {
                //string PathTo = @"pack://application:,,,/Resources/Auth/credentials.json";
                // string PathTo = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "B6\\Settings\\", "credentials.json");
                string PathTo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\Auth", "credentials.json");
                // Application.StartupPath + PathTo
                // Load client secrets.
                using (var stream = new FileStream(PathTo, FileMode.Open, FileAccess.Read))
                {
                    string credPath = "token.json";

                    Credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.FromStream(stream).Secrets,
                        scopes,
                        GoogleAccount,
                        CancellationToken.None,
                        new FileDataStore(credPath, true));
                }
            }
            catch (Exception e)
            {

            }
        }
    
        /*
        public CalendarService Get()
        {
            string PathTo = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "B6\\Settings\\Google", "credentials.json");
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
        */
        //}
    }
}
