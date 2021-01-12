using Dental.Repositories.Template;
using Dental.ViewModels;
using DevExpress.Xpf.Core;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows;
using System.Windows.Input;
using Dental.Interfaces;

namespace Dental.Models.Template
{
    [Table("Diary")]
    class Diary : ViewModelBase
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("ParentId")]
        public int? ParentId { get; set; }

        [Required]
        [Column("Name")]
        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [Column("Dir")]
        [Display(Name = "Директория")]
        public int Dir { get; set; }

        [NotMapped]
        public string ImageSource
        {
            get { 
               // return @"..\Resources\Icons\Closed_Folder_32.png";
                return "pack://application:,,,/Resources/Icons/Template/diary.png";
                
            }

            set
            {
                imageSource = value;
            }
        }
        private string imageSource;

        private static ObservableCollection<Diary> diaryList;

        [NotMapped]
        public static ObservableCollection<Diary> DiaryList { 
            get
            {
                if (diaryList == null)
                {
                    diaryList = DiaryRepository.Diary;
                }
                return diaryList;
            } 
       
            set
            {
                diaryList = value;
            }

        }




        public Diary()
        {
           // DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
        }

        /*
        public ICommand DeleteCommand { get; }
        private bool CanDeleteCommandExecute(object p) => true;
        private void OnDeleteCommandExecuted(object p)
        {
            //bool isNew = true; // это новая форма, т.е. нужно создать новые модели, а не загружать сущ-щие данные
            try
            {
                var response = ThemedMessageBox.Show(title: "Подтверждение действия", text: "Вы уверены что хотите удалить роль?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Exclamation);
                if (response.ToString() == "Yes")
                {
                    Role role = (Role)p;
                    RoleRepository.Delete(role);
                    //ListRoles.Remove(role);
                }
            }
            catch (Exception e)
            {

                // записать в текстовой лог в каком месте возникла ошибка (название класса и строка) и e.Message
            }}*/
  
    }
}
