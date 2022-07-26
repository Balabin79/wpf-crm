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
        protected readonly Settings GoogleAccount;
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
                GoogleAccount = db.Settings.FirstOrDefault();
                if (GoogleAccount == null || GoogleAccount?.IsUseGoogleIntegration == null || GoogleAccount?.GoogleAccount == null || scopes.Count() == 0) return;
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
                string PathTo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\Auth", "credentials.json");
                using (var stream = new FileStream(PathTo, FileMode.Open, FileAccess.Read))
                {
                    string credPath = "token.json";

                    Credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.FromStream(stream).Secrets,
                        scopes,
                        GoogleAccount.GoogleAccount,
                        CancellationToken.None,
                        new FileDataStore(credPath, true));
                }
            }
            catch (Exception e)
            {

            }
        }
    }
}
