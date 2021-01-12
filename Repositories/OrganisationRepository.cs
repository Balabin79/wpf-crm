using Dental.Interfaces;
using Dental.Models;
using System.Collections.ObjectModel;

namespace Dental.Repositories
{
    class OrganisationRepository
    {
        public static ObservableCollection<Organization> GetFakeOrganizations()
        {
            return new ObservableCollection<Organization>
                {
                    new Organization {
                        Inn = "1234567890",
                        Kpp = "12345",
                        Name = "OOO Улыбка",
                        ShortName = "Улыбка",
                        Address = "г.Самара, ул.Лесная, дом 12",
                        Phone = "89873652456",
                        Email = "ulib@ya.ru",
                        Bik = "123456",
                        AccountNumber = "12345678965423",
                        BankName = "Сбербанк",
                        Сertificate = "asdrt",
                        Ogrn = "1423356688789",
                        GeneralDirector = "Иван Иванов Иванович",
                        License = "dsadaasasas",
                        WhoIssuedBy = "sacasccccscscsc"
                    },
                    new Organization {
                        Inn = "0987654321",
                        Kpp = "54321",
                        Name = "OOO Дентел",
                        ShortName = "Дентел",
                        Address = "г.Тольятти, ул.Советская, дом 125",
                        Phone = "89658235645",
                        Email = "dental@yahoo.com",
                        Bik = "654321",
                        AccountNumber = "12345678965423",
                        BankName = "Совкомбанк",
                        Сertificate = "erfgregregre",
                        Ogrn = "9875642956565",
                        GeneralDirector = "Петр Петров Петрович",
                        License = "adscdc 4545 fedfef 55",
                        WhoIssuedBy = "btgbrtrbtbtntt ttnnt grtrr"
                    }
                };
        }

    }
}
