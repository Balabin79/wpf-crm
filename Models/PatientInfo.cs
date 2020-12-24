using Dental.Models.Share;
using System;
using System.Collections.Generic;

namespace Dental.Models
{
    class PatientInfo
    {

        public PatientInfo ()
        {
            GenderList = new List<string> { "мужской", "женский" };
            AreaList = new List<string> { "городская местность", "сельская местность" };
            MaritalStatusList = new List<string> {
                "состоит в зарегистрированном браке",
                "состоит в незарегистрированном браке", 
                "не состоит в браке", 
                "неизвестно" 
            };

            GeneralEducationList = new List<string> { 
                "среднее (полное)", 
                "основное", 
                "начальное",
                "не имеет начального образования", 
                "неизвестно"
            };

            ProfessionalEducationList = new List<string> { 
                "высшее",
                "неполное высшее", 
                "среднее", 
                "начальное"
            };

            EmploymentList = new List<string> {
                "руководители и специалисты высшей квалификации", 
                "прочие специалисты",
                "квалифицированные рабочие",
                "неквалифицированные рабочие",
                "занятые на военной службе",
                "пенсионеры ",
                "студенты и учащиеся"
            };

            PaymentTypeList = new List<string> { 
                "ОМС", 
                "бюджет", 
                "платные услуги",
                "ДМС",
                "другое"
            };

            ExemptionCategoryList = new List<string> {
                "инвалид ВОВ",
                "участник ВОВ" ,
                "воин-интернационалист" ,
                "лицо, подвергшееся радиационному облучению" ,
                "в т.ч. в Чернобыле" ,
                "инвалид II группы" ,
                "инвалид III группы" ,
                "ребенок-инвалид" ,
                "инвалид с детства" ,
                "прочие"
            };

        }
        public string Number { get; set;  } //номер карты
        public DateTime? DocDate { get; set;  } //дата заполнения карты

        public string Firstname { get; set;  }
        public string Middlename { get; set;  }
        public string Lastname { get; set;  }
        public string Gender { get; set;  }
        public DateTime? BirthDay { get; set; }
        public string RegistrationAddress { get; set;  } // место регистрации
        public string Area { get; set; } // местность
        public string MaritalStatus { get; set; } // семейное положение
        public string ProfessionalEducation { get; set; } // профессиональное образование
        public string GeneralEducation { get; set; } // общее образование
        public string Employment { get; set; } // занятость
        public string PlaceOfWorkList { get; set; } // Место работы
        public MedicalPolicy PolisOMS { get; set; } // Полис ОМС
        public string Snils { get; set; }
        public string SerialNumberPassport { get; set;  } //серия паспорта
        public string NumberPassport { get; set;  } // номер паспорта
        public string WhoIssuedPassport { get; set;  } // кем выдан паспорт
        public string PaymentType { get; set;  } // вид оплаты
        public string ExemptionCategoryCode { get; set;  } // код категории льготы
        public string ExemptionCategory { get; set;  } // категория льготы



        public List<string> GenderList { get; set; }
        public List<string> AreaList { get; set; }
        public List<string> MaritalStatusList { get; set; }
        public List<string> GeneralEducationList { get; set; }
        public List<string> ProfessionalEducationList { get; set; }
        public List<string> EmploymentList { get; set; }
        public List<string> PaymentTypeList { get; set; }
        public List<string> ExemptionCategoryList { get; set; }


    }

}
