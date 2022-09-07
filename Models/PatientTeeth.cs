using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Models
{
    [Serializable]
    public class PatientTeeth : IDataErrorInfo
    {
        public Tooth Tooth18 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 18, ToothImagePath = ImgPathGreen };
        public Tooth Tooth17 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 17, ToothImagePath = ImgPathGreen };
        public Tooth Tooth16 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 16, ToothImagePath = ImgPathGreen };
        public Tooth Tooth15 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 15, ToothImagePath = ImgPathGreen };
        public Tooth Tooth14 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 14, ToothImagePath = ImgPathGreen };
        public Tooth Tooth13 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 13, ToothImagePath = ImgPathGreen };
        public Tooth Tooth12 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 12, ToothImagePath = ImgPathGreen };
        public Tooth Tooth11 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 11, ToothImagePath = ImgPathGreen };
        public Tooth Tooth21 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 21, ToothImagePath = ImgPathGreen };
        public Tooth Tooth22 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 22, ToothImagePath = ImgPathGreen };
        public Tooth Tooth23 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 23, ToothImagePath = ImgPathGreen };
        public Tooth Tooth24 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 24, ToothImagePath = ImgPathGreen };
        public Tooth Tooth25 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 25, ToothImagePath = ImgPathGreen };
        public Tooth Tooth26 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 26, ToothImagePath = ImgPathGreen };
        public Tooth Tooth27 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 27, ToothImagePath = ImgPathGreen };
        public Tooth Tooth28 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 28, ToothImagePath = ImgPathGreen };
        public Tooth Tooth48 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 48, ToothImagePath = ImgPathGreen };
        public Tooth Tooth47 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 47, ToothImagePath = ImgPathGreen };
        public Tooth Tooth46 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 46, ToothImagePath = ImgPathGreen };
        public Tooth Tooth45 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 45, ToothImagePath = ImgPathGreen };
        public Tooth Tooth44 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 44, ToothImagePath = ImgPathGreen };
        public Tooth Tooth43 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 43, ToothImagePath = ImgPathGreen };
        public Tooth Tooth42 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 42, ToothImagePath = ImgPathGreen };
        public Tooth Tooth41 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 41, ToothImagePath = ImgPathGreen };
        public Tooth Tooth31 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 31, ToothImagePath = ImgPathGreen };
        public Tooth Tooth32 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 32, ToothImagePath = ImgPathGreen };
        public Tooth Tooth33 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 33, ToothImagePath = ImgPathGreen };
        public Tooth Tooth34 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 34, ToothImagePath = ImgPathGreen };
        public Tooth Tooth35 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 35, ToothImagePath = ImgPathGreen };
        public Tooth Tooth36 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 36, ToothImagePath = ImgPathGreen };
        public Tooth Tooth37 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 37, ToothImagePath = ImgPathGreen };
        public Tooth Tooth38 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 38, ToothImagePath = ImgPathGreen };

        public static string ImgPathGreen { get => "pack://application:,,,/Resources/Icons/teeth/tooth-green.png"; }
        public static string ImgPathYellow { get => "pack://application:,,,/Resources/Icons/teeth/tooth-yel.png"; }
        public static string ImgPathRed { get => "pack://application:,,,/Resources/Icons/teeth/tooth-red.png"; }
        public static string ImgPathGray { get => "pack://application:,,,/Resources/Icons/teeth/tooth-gray.png"; }
        public static string ImgPathImp { get => "pack://application:,,,/Resources/Icons/teeth/implant.png"; }

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
