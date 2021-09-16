using Dental.Models.Base;
using Dental.Interfaces;
using DevExpress.Mvvm;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    /*[Table("Price")]*/
    public class Teeth// : AbstractBaseModel, IDataErrorInfo
    {
        public Tooth Tooth18 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 18, ToothImagePath = imgPath };
        public Tooth Tooth17 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 17, ToothImagePath = imgPath };
        public Tooth Tooth16 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 16, ToothImagePath = imgPath };
        public Tooth Tooth15 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 15, ToothImagePath = imgPath };
        public Tooth Tooth14 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 14, ToothImagePath = imgPath };
        public Tooth Tooth13 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 13, ToothImagePath = imgPath };
        public Tooth Tooth12 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 12, ToothImagePath = imgPath };
        public Tooth Tooth11 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 11, ToothImagePath = imgPath };
        public Tooth Tooth21 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 21, ToothImagePath = imgPath };
        public Tooth Tooth22 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 22, ToothImagePath = imgPath };
        public Tooth Tooth23 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 23, ToothImagePath = imgPath };
        public Tooth Tooth24 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 24, ToothImagePath = imgPath };
        public Tooth Tooth25 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 25, ToothImagePath = imgPath };
        public Tooth Tooth26 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 26, ToothImagePath = imgPath };
        public Tooth Tooth27 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 27, ToothImagePath = imgPath };
        public Tooth Tooth28 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 28, ToothImagePath = imgPath };
        public Tooth Tooth48 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 48, ToothImagePath = imgPath };
        public Tooth Tooth47 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 47, ToothImagePath = imgPath };
        public Tooth Tooth46 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 46, ToothImagePath = imgPath };
        public Tooth Tooth45 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 45, ToothImagePath = imgPath };
        public Tooth Tooth44 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 44, ToothImagePath = imgPath };
        public Tooth Tooth43 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 43, ToothImagePath = imgPath };
        public Tooth Tooth42 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 42, ToothImagePath = imgPath };
        public Tooth Tooth41 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 41, ToothImagePath = imgPath };
        public Tooth Tooth31 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 31, ToothImagePath = imgPath };
        public Tooth Tooth32 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 32, ToothImagePath = imgPath };
        public Tooth Tooth33 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 33, ToothImagePath = imgPath };
        public Tooth Tooth34 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 34, ToothImagePath = imgPath };
        public Tooth Tooth35 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 35, ToothImagePath = imgPath };
        public Tooth Tooth36 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 36, ToothImagePath = imgPath };
        public Tooth Tooth37 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 37, ToothImagePath = imgPath };
        public Tooth Tooth38 { get; set; } = new Tooth() { Abbr = "", ToothNumber = 38, ToothImagePath = imgPath };

        private  static readonly string imgPath = "pack://application:,,,/Resources/Icons/Template/tooth_green.png";
        //public string Error { get => string.Empty; }
        //public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
    }
}
