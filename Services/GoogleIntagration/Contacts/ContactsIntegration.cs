using Dental.Models;
using Dental.Models.Base;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using Google.Apis.PeopleService.v1.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static Google.Apis.PeopleService.v1.PeopleResource;

namespace Dental.Services.GoogleIntagration.Contacts
{
    public class ContactsIntegration
    {

        public void Run()
        {         
            ClientsIntegration();
            //EmployeesIntegration();

            //ClearClientContacts();
            //ClearEmployeeContacts();
        }

        private GoogleContacts Google { get; set; } = new GoogleContacts();

        private async void ClientsIntegration()
        {
            using (var db = new ApplicationContext())
            {
                try
                {
                    string clientsRootDirResourceId = await GetClientsRootDir();

                    var queue = db.ClientContactsQueue.Include(f => f.Client).Where(f => f.SendingStatusId != (int)ParamEnums.SendingStatus.Sended).ToArray();                   
                   
                    foreach (var i in queue)
                    {
                        try 
                        {
                            if (i.EventTypeId == (int)ParamEnums.EventType.Added)
                            {                             

                                var person = new Person() 
                                { 
                                    EmailAddresses = new List<EmailAddress>() { new EmailAddress() { Value = i?.Client?.Email, DisplayName = i?.Client?.FullName } },
                                    PhoneNumbers = new List<PhoneNumber>() { new PhoneNumber() { Value = i?.Client?.Phone } },
                                    Birthdays = new List<Birthday>() { new Birthday() { Text = i?.Client.BirthDate } },
                                    Names = new List<Name>() { new Name { DisplayName = i?.Client?.FullName, GivenName = i?.Client?.FirstName, FamilyName = i?.Client?.LastName, MiddleName = i?.Client?.MiddleName} },
                                    Memberships = new List<Membership>() { new Membership() { ContactGroupMembership = new ContactGroupMembership() { ContactGroupResourceName = clientsRootDirResourceId } } }
 
                                };
                                Person resource = await Google.CreateContact(person);
                                db.GoogleClientsContact.Add(new GoogleClientsContacts()
                                {
                                    Client = i?.Client,
                                    ClientGuid = i?.Client?.Guid,
                                    ClientId = i?.Client?.Id,
                                    ResourceId = resource.ResourceName,
                                    ContactGroupId = resource.Memberships.Count > 0 ? resource.Memberships[0].ContactGroupMembership.ContactGroupResourceName : "",
                                    Name = i?.Client.FullName
                                });
                                db.ClientContactsQueue.FirstOrDefault(f => f.Id == i.Id).SendingStatusId = (int)ParamEnums.SendingStatus.Sended;
                                db.SaveChanges();
                            }
                        } 
                        catch (Exception e) 
                        {
                            i.SendingStatusId = (int)ParamEnums.SendingStatus.Error;
                            db.SaveChanges();
                        }


                        // есть или нет контакт, если есть обновить, иначе - создать (действие edited)
                       // var contact = db.GoogleClientsContacts.FirstOrDefault(f => f.ClientId == i.ClientId);
                    }
                }
                catch(Exception e)
                {

                }
            }
        }

        #region Создание и/или получение корневой директории клиентов/сотрудников
        private async Task<string> GetClientsRootDir()
        {
            using (var db = new ApplicationContext())
            {
                try
                {
                    GoogleRootDir dir = db.GoogleRootDir.FirstOrDefault(f => f.DirType == (int)ParamEnums.DirType.ClientsRootDir);
                    return string.IsNullOrEmpty(dir.ResourceId) ? await CreateClientsRootDir(dir) : dir.ResourceId;
                }
                catch (Exception e)
                {
                    ThemedMessageBox.Show(title: "Внимание", text: "В настройках интеграции не заполнены контакты клиентов!",
                            icon: MessageBoxImage.Error);
                    return "";
                }
            }
        }

        private async Task<string> CreateClientsRootDir(GoogleRootDir dir)
        {
            try
            {
                var response = await Google.CreateGroup((dir.LocalDirName ?? "Контакты клиентов"));

                using (var db = new ApplicationContext())
                {

                    GoogleRootDir directory = db.GoogleRootDir.FirstOrDefault(f => f.DirType == (int)ParamEnums.DirType.ClientsRootDir);
                    directory.ResourceId = response.ResourceName;
                    db.SaveChanges();
                }
                    return response.ResourceName;
            }
            catch (Exception e)
            {
                //if (e.)
                ThemedMessageBox.Show(title: "Внимание", text: "Ошибка при попытке создать удаленную папку \"Контакты клиента\"!", icon: MessageBoxImage.Error);
                return "";
            }
        }

        private async Task<string> GetEmployeesRootDir()
        {
            using (var db = new ApplicationContext())
            {
                try
                {
                    GoogleRootDir dir = db.GoogleRootDir.FirstOrDefault(f => f.DirType == (int)ParamEnums.DirType.EmployeesRootDir);
                    return string.IsNullOrEmpty(dir.ResourceId) ? await CreateEmployeesRootDir(dir) : dir.ResourceId;
                }
                catch (Exception e)
                {
                    ThemedMessageBox.Show(title: "Внимание", text: "В настройках интеграции не заполнены контакты сотрудников!",
                            icon: MessageBoxImage.Error);
                    return "";
                }
            }
        }

        private async Task<string> CreateEmployeesRootDir(GoogleRootDir dir)
        {
            try
            {
                var response = await Google.CreateGroup((dir.LocalDirName ?? "Контакты сотрудников"));

                using (var db = new ApplicationContext())
                {

                    GoogleRootDir directory = db.GoogleRootDir.FirstOrDefault(f => f.DirType == (int)ParamEnums.DirType.EmployeesRootDir);
                    directory.ResourceId = response.ResourceName;
                    db.SaveChanges();
                }
                return response.ResourceName;
            }
            catch (Exception e)
            {
                //if (e.)
                ThemedMessageBox.Show(title: "Внимание", text: "Ошибка при попытке создать удаленную папку \"Контакты сотрудников\"!", icon: MessageBoxImage.Error);
                return "";
            }
        }
        #endregion

        /// <summary>
        /// //////////////////
        /// </summary>

        private void EmployeesIntegration()
        {
            using (var db = new ApplicationContext())
            {
                var employeeContacts = db.EmployeeContactsQueue.ToArray();
            }
        }


        private void ClearClientContacts()
        {
            using (var db = new ApplicationContext())
            {
                try
                {
                    db.ClientContactsQueue.Where(f => f.SendingStatusId == (int)ParamEnums.SendingStatus.Sended)?.ForEach(f => db.Entry(f).State = EntityState.Deleted);
                    db.SaveChanges();
                }
                catch (Exception e)
                {

                }
            }
        }

        private void ClearEmployeeContacts()
        {
            using (var db = new ApplicationContext())
            {
                try
                {
                    db.EmployeeContactsQueue.Where(f => f.SendingStatusId == (int)ParamEnums.SendingStatus.Sended)?.ForEach(f => db.Entry(f).State = EntityState.Deleted);
                    db.SaveChanges();
                }
                catch (Exception e)
                {

                }
            }
        }
    }
}
