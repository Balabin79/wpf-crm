using Dental.Models;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dental.Services.Google.ViewModels.CalendarList
{
    public class CalendarListInsertSettings : BaseViewModel
    {
        public bool ColorRgbFormat
        {
            get { return GetProperty(() => ColorRgbFormat); }
            set { SetProperty(() => ColorRgbFormat, value); }
        }

        public string Id
        {
            get { return GetProperty(() => Id); }
            set { SetProperty(() => Id, value); }
        }

        public string BackgroundColor
        {
            get { return GetProperty(() => BackgroundColor); }
            set { SetProperty(() => BackgroundColor, value); }
        }

        public string ColorId
        {
            get { return GetProperty(() => ColorId); }
            set { SetProperty(() => ColorId, value); }
        }

        public List<string> DefaultReminders
        {
            get { return GetProperty(() => DefaultReminders); }
            set { SetProperty(() => DefaultReminders, value); }
        }

        public string DefaultRemindersMethod
        {
            get { return GetProperty(() => DefaultRemindersMethod); }
            set { SetProperty(() => DefaultRemindersMethod, value); }
        }

        public int DefaultRemindersMinutes
        {
            get { return GetProperty(() => DefaultRemindersMinutes); }
            set { SetProperty(() => DefaultRemindersMinutes, value); }
        }

        public string ForegroundColor
        {
            get { return GetProperty(() => ForegroundColor); }
            set { SetProperty(() => ForegroundColor, value); }
        }

        public bool Hidden
        {
            get { return GetProperty(() => Hidden); }
            set { SetProperty(() => Hidden, value); }
        }

        public object NotificationSettings
        {
            get { return GetProperty(() => NotificationSettings); }
            set { SetProperty(() => NotificationSettings, value); }
        }

        public string NotificationSettingsMethod { get; set; } = "email";

        public string NotificationSettingsType
        {
            get { return GetProperty(() => NotificationSettingsType); }
            set { SetProperty(() => NotificationSettingsType, value); }
        }

        public bool Selected
        {
            get { return GetProperty(() => Selected); }
            set { SetProperty(() => Selected, value); }
        }

        public string SummaryOverride
        {
            get { return GetProperty(() => SummaryOverride); }
            set { SetProperty(() => SummaryOverride, value); }
        }

        public string[] DefaultRemindersMethodValues = { "email", "popup" }; //для поля select
        public string[] NotificationSettingsTypeValues = { " eventCreation", "eventChange", "eventCancellation", "eventResponse", "agenda" }; //для поля select
    }
}
