using Dental.Models.Base;
using Dental.Interfaces;
using DevExpress.Mvvm;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    public class PatientTeeth : ViewModelBase, IDataErrorInfo
    { 
        public PatientTeeth()
        {
            Tooth18 = new Tooth() { Abbr = "", ToothNumber = 18, ToothImagePath = ImgPathGreen };
            Tooth17 = new Tooth() { Abbr = "", ToothNumber = 17, ToothImagePath = ImgPathGreen };
            Tooth16 = new Tooth() { Abbr = "", ToothNumber = 16, ToothImagePath = ImgPathGreen };
            Tooth15 = new Tooth() { Abbr = "", ToothNumber = 15, ToothImagePath = ImgPathGreen };
            Tooth14 = new Tooth() { Abbr = "", ToothNumber = 14, ToothImagePath = ImgPathGreen };
            Tooth13 = new Tooth() { Abbr = "", ToothNumber = 13, ToothImagePath = ImgPathGreen };
            Tooth12 = new Tooth() { Abbr = "", ToothNumber = 12, ToothImagePath = ImgPathGreen };
            Tooth11 = new Tooth() { Abbr = "", ToothNumber = 11, ToothImagePath = ImgPathGreen };
            Tooth21 = new Tooth() { Abbr = "", ToothNumber = 21, ToothImagePath = ImgPathGreen };
            Tooth22 = new Tooth() { Abbr = "", ToothNumber = 22, ToothImagePath = ImgPathGreen };
            Tooth23 = new Tooth() { Abbr = "", ToothNumber = 23, ToothImagePath = ImgPathGreen };
            Tooth24 = new Tooth() { Abbr = "", ToothNumber = 24, ToothImagePath = ImgPathGreen };
            Tooth25 = new Tooth() { Abbr = "", ToothNumber = 25, ToothImagePath = ImgPathGreen };
            Tooth26 = new Tooth() { Abbr = "", ToothNumber = 26, ToothImagePath = ImgPathGreen };
            Tooth27 = new Tooth() { Abbr = "", ToothNumber = 27, ToothImagePath = ImgPathGreen };
            Tooth28 = new Tooth() { Abbr = "", ToothNumber = 28, ToothImagePath = ImgPathGreen };
            Tooth48 = new Tooth() { Abbr = "", ToothNumber = 48, ToothImagePath = ImgPathGreen };
            Tooth47 = new Tooth() { Abbr = "", ToothNumber = 47, ToothImagePath = ImgPathGreen };
            Tooth46 = new Tooth() { Abbr = "", ToothNumber = 46, ToothImagePath = ImgPathGreen };
            Tooth45 = new Tooth() { Abbr = "", ToothNumber = 45, ToothImagePath = ImgPathGreen };
            Tooth44 = new Tooth() { Abbr = "", ToothNumber = 44, ToothImagePath = ImgPathGreen };
            Tooth43 = new Tooth() { Abbr = "", ToothNumber = 43, ToothImagePath = ImgPathGreen };
            Tooth42 = new Tooth() { Abbr = "", ToothNumber = 42, ToothImagePath = ImgPathGreen };
            Tooth41 = new Tooth() { Abbr = "", ToothNumber = 41, ToothImagePath = ImgPathGreen };
            Tooth31 = new Tooth() { Abbr = "", ToothNumber = 31, ToothImagePath = ImgPathGreen };
            Tooth32 = new Tooth() { Abbr = "", ToothNumber = 32, ToothImagePath = ImgPathGreen };
            Tooth33 = new Tooth() { Abbr = "", ToothNumber = 33, ToothImagePath = ImgPathGreen };
            Tooth34 = new Tooth() { Abbr = "", ToothNumber = 34, ToothImagePath = ImgPathGreen };
            Tooth35 = new Tooth() { Abbr = "", ToothNumber = 35, ToothImagePath = ImgPathGreen };
            Tooth36 = new Tooth() { Abbr = "", ToothNumber = 36, ToothImagePath = ImgPathGreen };
            Tooth37 = new Tooth() { Abbr = "", ToothNumber = 37, ToothImagePath = ImgPathGreen };
            Tooth38 = new Tooth() { Abbr = "", ToothNumber = 38, ToothImagePath = ImgPathGreen };
        }

        public PatientTeeth(PatientTeeth patientTeeth)
        {
            Tooth18 = patientTeeth.Tooth18;
            Tooth17 = patientTeeth.Tooth17;
            Tooth16 = patientTeeth.Tooth16;
            Tooth15 = patientTeeth.Tooth15;
            Tooth14 = patientTeeth.Tooth14;
            Tooth13 = patientTeeth.Tooth13;
            Tooth12 = patientTeeth.Tooth12;
            Tooth11 = patientTeeth.Tooth11;

            Tooth21 = patientTeeth.Tooth21;
            Tooth22 = patientTeeth.Tooth22;
            Tooth23 = patientTeeth.Tooth23;
            Tooth24 = patientTeeth.Tooth24;
            Tooth25 = patientTeeth.Tooth25;
            Tooth26 = patientTeeth.Tooth26;
            Tooth27 = patientTeeth.Tooth27;
            Tooth28 = patientTeeth.Tooth28;

            Tooth48 = patientTeeth.Tooth48;
            Tooth47 = patientTeeth.Tooth47;
            Tooth46 = patientTeeth.Tooth46;
            Tooth45 = patientTeeth.Tooth45;
            Tooth44 = patientTeeth.Tooth44;
            Tooth43 = patientTeeth.Tooth43;
            Tooth42 = patientTeeth.Tooth42;
            Tooth41 = patientTeeth.Tooth41;

            Tooth31 = patientTeeth.Tooth31;
            Tooth32 = patientTeeth.Tooth32;
            Tooth33 = patientTeeth.Tooth33;
            Tooth34 = patientTeeth.Tooth34;
            Tooth35 = patientTeeth.Tooth35;
            Tooth36 = patientTeeth.Tooth36;
            Tooth37 = patientTeeth.Tooth37;
            Tooth38 = patientTeeth.Tooth38;
        }

        public Tooth Tooth18 {
            get => _Tooth18;
            set => SetValue(ref _Tooth18, value); 
        }
        private Tooth _Tooth18;

        public Tooth Tooth17
        {
            get => _Tooth17;
            set => SetValue(ref _Tooth17, value);
        }
        private Tooth _Tooth17;

        public Tooth Tooth16
        {
            get => _Tooth16;
            set => SetValue(ref _Tooth16, value);
        }
        private Tooth _Tooth16;

        public Tooth Tooth15
        {
            get => _Tooth15;
            set => SetValue(ref _Tooth15, value);
        }
        private Tooth _Tooth15;

        public Tooth Tooth14
        {
            get => _Tooth14;
            set => SetValue(ref _Tooth14, value);
        }
        private Tooth _Tooth14;

        public Tooth Tooth13
        {
            get => _Tooth13;
            set => SetValue(ref _Tooth13, value);
        }
        private Tooth _Tooth13;

        public Tooth Tooth12
        {
            get => _Tooth12;
            set => SetValue(ref _Tooth12, value);
        }
        private Tooth _Tooth12;

        public Tooth Tooth11
        {
            get => _Tooth11;
            set => SetValue(ref _Tooth11, value);
        }
        private Tooth _Tooth11;

        public Tooth Tooth21
        {
            get => _Tooth21;
            set => SetValue(ref _Tooth21, value);
        }
        private Tooth _Tooth21;

        public Tooth Tooth22
        {
            get => _Tooth22;
            set => SetValue(ref _Tooth22, value);
        }
        private Tooth _Tooth22;

        public Tooth Tooth23
        {
            get => _Tooth23;
            set => SetValue(ref _Tooth23, value);
        }
        private Tooth _Tooth23;

        public Tooth Tooth24
        {
            get => _Tooth24;
            set => SetValue(ref _Tooth24, value);
        }
        private Tooth _Tooth24;

        public Tooth Tooth25
        {
            get => _Tooth25;
            set => SetValue(ref _Tooth25, value);
        }
        private Tooth _Tooth25;

        public Tooth Tooth26
        {
            get => _Tooth26;
            set => SetValue(ref _Tooth26, value);
        }
        private Tooth _Tooth26;

        public Tooth Tooth27
        {
            get => _Tooth27;
            set => SetValue(ref _Tooth27, value);
        }
        private Tooth _Tooth27;

        public Tooth Tooth28
        {
            get => _Tooth28;
            set => SetValue(ref _Tooth28, value);
        }
        private Tooth _Tooth28;

        public Tooth Tooth48
        {
            get => _Tooth48;
            set => SetValue(ref _Tooth48, value);
        }
        private Tooth _Tooth48;

        public Tooth Tooth47
        {
            get => _Tooth47;
            set => SetValue(ref _Tooth47, value);
        }
        private Tooth _Tooth47;

        public Tooth Tooth46
        {
            get => _Tooth46;
            set => SetValue(ref _Tooth46, value);
        }
        private Tooth _Tooth46;

        public Tooth Tooth45
        {
            get => _Tooth45;
            set => SetValue(ref _Tooth45, value);
        }
        private Tooth _Tooth45;

        public Tooth Tooth44
        {
            get => _Tooth44;
            set => SetValue(ref _Tooth44, value);
        }
        private Tooth _Tooth44;

        public Tooth Tooth43
        {
            get => _Tooth43;
            set => SetValue(ref _Tooth43, value);
        }
        private Tooth _Tooth43;

        public Tooth Tooth42
        {
            get => _Tooth42;
            set => SetValue(ref _Tooth42, value);
        }
        private Tooth _Tooth42;

        public Tooth Tooth41
        {
            get => _Tooth41;
            set => SetValue(ref _Tooth41, value);
        }
        private Tooth _Tooth41;

        public Tooth Tooth31
        {
            get => _Tooth31;
            set => SetValue(ref _Tooth31, value);
        }
        private Tooth _Tooth31;

        public Tooth Tooth32
        {
            get => _Tooth32;
            set => SetValue(ref _Tooth32, value);
        }
        private Tooth _Tooth32;

        public Tooth Tooth33
        {
            get => _Tooth33;
            set => SetValue(ref _Tooth33, value);
        }
        private Tooth _Tooth33;

        public Tooth Tooth34
        {
            get => _Tooth34;
            set => SetValue(ref _Tooth34, value);
        }
        private Tooth _Tooth34;

        public Tooth Tooth35
        {
            get => _Tooth35;
            set => SetValue(ref _Tooth35, value);
        }
        private Tooth _Tooth35;

        public Tooth Tooth36
        {
            get => _Tooth36;
            set => SetValue(ref _Tooth36, value);
        }
        private Tooth _Tooth36;

        public Tooth Tooth37
        {
            get => _Tooth37;
            set => SetValue(ref _Tooth37, value);
        }
        private Tooth _Tooth37;

        public Tooth Tooth38
        {
            get => _Tooth38;
            set => SetValue(ref _Tooth38, value);
        }
        private Tooth _Tooth38;

        public static string ImgPathGreen { get => "pack://application:,,,/Resources/Icons/Template/tooth-green.png"; }
        public static string ImgPathYellow { get => "pack://application:,,,/Resources/Icons/Template/tooth-yel.png"; }
        public static string ImgPathRed { get => "pack://application:,,,/Resources/Icons/Template/tooth-red.png"; }
        public static string ImgPathGray { get => "pack://application:,,,/Resources/Icons/Template/tooth-gray.png"; }
        public static string ImgPathImp { get => "pack://application:,,,/Resources/Icons/Template/implant.png"; }
        
        public static string Periodontit { get => "П"; }
        public static string Pulpit { get => "Пл"; }
        public static string Radiks { get => "Р"; }
        public static string Caries { get => "К"; }
        public static string Plomba { get => "Пл"; }
        public static string Coronka { get => "Кор"; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
    }
}
