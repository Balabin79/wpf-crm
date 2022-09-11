using Dental.Infrastructures.Collection;
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
        public Tooth Tooth18 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 18, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth17 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 17, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth16 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 16, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth15 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 15, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth14 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 14, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth13 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 13, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth12 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 12, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth11 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 11, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth21 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 21, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth22 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 22, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth23 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 23, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth24 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 24, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth25 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 25, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth26 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 26, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth27 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 27, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth28 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 28, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth48 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 48, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth47 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 47, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth46 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 46, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth45 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 45, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth44 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 44, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth43 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 43, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth42 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 42, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth41 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 41, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth31 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 31, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth32 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 32, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth33 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 33, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth34 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 34, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth35 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 35, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth36 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 36, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth37 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 37, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth38 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 38, ToothImagePath = TeethImages.ImgPathGreen };

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
    }
}
