using Dental.Infrastructures.Logs;
using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("ClientsSubscribes")]
    public class ClientsSubscribes : AbstractBaseModel, IDataErrorInfo, ITreeModel
    {
        [Display(Name = "Название")]
        public string Name
        {
            get => _Name;
            set => _Name = value?.Trim();
        }
        private string _Name;

        public string Comment { get; set; }
        public string Content { get; set; }
        public string DateSubscribe { get; set; }

        public int? ParentId { get; set; }
        public int? IsDir { get; set; }

        public int? SubscribeTypeId { get; set; }
        public TypeSubscribe TypeSubscribe { get; set; }

        public int? ClientGroupId { get; set; }
        public ClientsGroup ClientsGroup { get; set; }

        public int? StatusSubscribeId { get; set; }
        public StatusSubscribe StatusSubscribe { get; set; }

        public int SendCnt { get; set; } = 0;
        public int NotSendCnt { get; set; } = 0;


        public Employee Employee { get; set; }
        public int? EmployeeId { get; set; }


        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone()
        {
            try
            {
                return new ClientsSubscribes
                {
                    Id = this.Id,
                    Name = this.Name,
                    Comment = this.Comment,
                    Content = this.Content,
                    DateSubscribe = this.DateSubscribe,
                    Guid = this.Guid,
                    Employee = this.Employee,
                    ParentId = this.ParentId,
                    IsDir = this.IsDir,
                    SubscribeTypeId = this.SubscribeTypeId,
                    ClientGroupId = this.ClientGroupId,
                    StatusSubscribeId = this.StatusSubscribeId,
                    SendCnt = this.SendCnt,
                    NotSendCnt = this.NotSendCnt
                };
            } catch(Exception ex)
            {
                (new ViewModelLog(ex)).run();
                return new ClientsRequests();
            }

        }

        public ClientsSubscribes Copy(ClientsSubscribes model)
        {
            model.Id = this.Id;
            model.Name = this.Name;
            model.Guid = this.Guid;
            model.Content = this.Content;
            model.Comment = this.Comment;
            model.DateSubscribe = this.DateSubscribe;
            model.Employee = this.Employee;
            model.EmployeeId = this.EmployeeId;
            model.ParentId = this.ParentId;
            model.IsDir = this.IsDir;
            model.SubscribeTypeId = this.SubscribeTypeId;
            model.ClientGroupId = this.ClientGroupId;
            model.StatusSubscribeId = this.StatusSubscribeId;
            model.SendCnt = this.SendCnt;
            model.NotSendCnt = this.NotSendCnt;
            return model;
        }


        public override bool Equals(object other)
        {
            if (other == null)
                return false;

            //Если ссылки указывают на один и тот же адрес, то их идентичность гарантирована.
            if (object.ReferenceEquals(this, other))
                return true;

            if (this.GetType() != other.GetType())
                return false;

            return this.Equals(other as ClientsSubscribes);
        }
        public bool Equals(ClientsSubscribes other)
        {
            NotIsChanges = true;
            if (other == null)
                return false;

            if (object.ReferenceEquals(this, other))
                return true;

            if (this.GetType() != other.GetType())
                return false;           

            StringParamsIsEquel(this.Name, other.Name);
            StringParamsIsEquel(this.Guid, other.Guid);
            StringParamsIsEquel(this.Content, other.Content);
            StringParamsIsEquel(this.Comment, other.Comment);
            StringParamsIsEquel(this.DateSubscribe, other.DateSubscribe);
            StringParamsIsEquel(this.Employee.Guid, other.Employee.Guid);
            if (this.EmployeeId != other.EmployeeId) return false;
            if (this.IsDir != other.IsDir) return false;
            if (this.ParentId != other.ParentId) return false;
            if (this.SubscribeTypeId != other.SubscribeTypeId) return false;
            if (this.ClientGroupId != other.ClientGroupId) return false;
            if (this.SendCnt != other.SendCnt) return false;
            if (this.NotSendCnt != other.NotSendCnt) return false;
            if (this.StatusSubscribeId != other.StatusSubscribeId) return false;
                return NotIsChanges;
        }

        private void StringParamsIsEquel(string param1, string param2)
        {
            if (string.IsNullOrEmpty(param1) && string.IsNullOrEmpty(param2)) return;
            if (string.Compare(param1, param2, StringComparison.CurrentCulture) == 0) return;
            NotIsChanges = false;
        }

        [NotMapped]
        public bool NotIsChanges { get; set; } = true;

    }
}
