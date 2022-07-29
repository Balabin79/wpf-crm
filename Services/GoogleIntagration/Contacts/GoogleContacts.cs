using DevExpress.Mvvm.Native;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.PeopleService.v1;
using Google.Apis.PeopleService.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Google.Apis.PeopleService.v1.PeopleResource;

namespace Dental.Services.GoogleIntagration.Contacts
{
    public class GoogleContacts : GoogleBase
    {
        public GoogleContacts() : base() => Service = new PeopleServiceService(new BaseClientService.Initializer { HttpClientInitializer = Credential, ApplicationName = APPLICATION_NAME });

        private PeopleServiceService Service { get; }
        public ObservableCollection<ContactGroup> Groups { get; private set; }

        #region Группы контактов
        public async Task<ObservableCollection<ContactGroup>> List()
        {
            var response = await Service.ContactGroups.List().ExecuteAsync();
            return response.ContactGroups?.Where(f => f.GroupType == "USER_CONTACT_GROUP")?.ToObservableCollection();
        }

        public async Task<ContactGroup> CreateGroup(string nameGroup)
        {
            var group = new CreateContactGroupRequest() { ContactGroup = new ContactGroup() { Name = nameGroup } };
            return await Service.ContactGroups.Create(group).ExecuteAsync();
        }

        public async void UpdateGroup(string resourceName, string nameGroup)
        {
            var list = await List();
            var contact = list.FirstOrDefault(f => f.ResourceName == resourceName);
            if (contact != null)
            {
                contact.Name = nameGroup;
                await Service.ContactGroups.Update(new UpdateContactGroupRequest() { ContactGroup = contact }, resourceName).ExecuteAsync();
            }
        }

        public async void DeleteGroup(string resourceName) => await Service.ContactGroups.Delete(resourceName).ExecuteAsync();

        public async Task<ContactGroup> GetGroup(string resourceName) => await Service.ContactGroups.Get(resourceName).ExecuteAsync();

        public async void BatchGetGroup(string[] ResourceNames)
        {
            try
            {
                var groups = Service.ContactGroups.BatchGet();
                groups.MaxMembers = 10;
                groups.ResourceNames = ResourceNames;
                var response = await groups.ExecuteAsync();
            }
            catch (Exception e)
            {

            }
        }
        #endregion

        #region Контакты
        public async void BatchCreateContacts()
        {

        }

        public async void BatchDeleteContacts()
        {

        }

        public async void BatchUpdateContacts()
        {

        }

        public async Task<Person> CreateContact(Person contact) => await Service.People.CreateContact(contact).ExecuteAsync();
        

        public async void DeleteContact()
        {

        }

        public async void UpdateContact()
        {

        }

        public async void UpdateContactPhoto()
        {

        }

        public async void DeleteContactPhoto()
        {

        }
        #endregion

    }
}
