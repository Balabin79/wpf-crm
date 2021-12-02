using System;
using Dental.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.Linq;

namespace Dental.Models
{
    [Table("EmployesSpecialities")]
    public class EmployesSpecialities : AbstractBaseModel, IDataErrorInfo
    {

        public int? EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int? SpecialityId { get; set; }
        public Speciality Speciality { get; set; }

        public string EmployeeGuid { get; set; }


        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

      

        public EmployesSpecialities Copy(EmployesSpecialities model)
        {
            model.Id = this.Id;
            model.Guid = this.Guid;
            model.Employee = this.Employee;
            model.EmployeeId = this.EmployeeId;
            model.SpecialityId = this.SpecialityId;
            model.Speciality = this.Speciality;
            model.EmployeeGuid = this.EmployeeGuid;
            return model;
        }

        public override bool Equals(object other)
        {
            //Последовательность проверки должна быть именно такой.
            //Если не проверить на null объект other, то other.GetType() может выбросить //NullReferenceException.            
            if (other == null)
                return false;

            //Если ссылки указывают на один и тот же адрес, то их идентичность гарантирована.
            if (object.ReferenceEquals(this, other))
                return true;

            //Если класс находится на вершине иерархии или просто не имеет наследников, то можно просто
            //сделать Vehicle tmp = other as Vehicle; if(tmp==null) return false; 
            //Затем вызвать экземплярный метод, сразу передав ему объект tmp.
            if (this.GetType() != other.GetType())
                return false;

            return this.Equals(other as EmployesSpecialities);
        }
        public bool Equals(EmployesSpecialities other)
        {
            try {
                NotIsChanges = true;
                if (other == null)
                    return false;

                //Здесь сравнение по ссылкам необязательно.
                //Если вы уверены, что многие проверки на идентичность будут отсекаться на проверке по ссылке - //можно имплементировать.
                if (object.ReferenceEquals(this, other))
                    return true;

                //Если по логике проверки, экземпляры родительского класса и класса потомка могут считаться равными,
                //то проверять на идентичность необязательно и можно переходить сразу к сравниванию полей.
                if (this.GetType() != other.GetType())
                    return false;

                StringParamsIsEquel(this.Guid, other.Guid);
                StringParamsIsEquel(this.Employee?.Guid, other?.Guid);
                StringParamsIsEquel(this.EmployeeGuid, other?.EmployeeGuid);
                //IsEqualsEmployeeSpecialities(Specialities, other?.Specialities);

                if (this.EmployeeId != other.EmployeeId)
                {
                    NotIsChanges = false;
                }
                //if (!NotIsChanges) FieldsChanges.Add("Должности сотрудника");
                return NotIsChanges;
            }
            catch(Exception e)
            {
                return false;
            }
            
        }

        private void StringParamsIsEquel(string param1, string param2)
        {
            if (string.IsNullOrEmpty(param1) && string.IsNullOrEmpty(param2)) return;
            if (string.Compare(param1, param2, StringComparison.CurrentCulture) == 0) return;
            NotIsChanges = false;
        }

        private void IsEqualsEmployeeSpecialities(List<Speciality> param1, List<Speciality> param2)
        {
            if (param1.Count() != param2.Count) {
                NotIsChanges = false;
                return;
            }
            for (int i = 0; i < param1.Count(); i++)
            {
                if (param1[i].Guid != param2[i].Guid)
                {
                    NotIsChanges = false;
                    return;
                }
            }
        }


        [NotMapped]
        public bool NotIsChanges { get; set; } = true;

        [NotMapped]
        public List<string> FieldsChanges { get; set; } = new List<string>();
    }
}
