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
    public class ChildTeeth : IDataErrorInfo
    {
        public Tooth Tooth55 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 55, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth54 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 54, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth53 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 53, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth52 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 52, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth51 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 51, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth61 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 61, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth62 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 62, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth63 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 63, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth64 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 64, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth65 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 65, ToothImagePath = TeethImages.ImgPathGreen };

        public Tooth Tooth85 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 85, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth84 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 84, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth83 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 83, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth82 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 82, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth81 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 81, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth71 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 71, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth72 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 72, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth73 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 73, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth74 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 74, ToothImagePath = TeethImages.ImgPathGreen };
        public Tooth Tooth75 { get; set; } = new Tooth() { Abbr = "З", ToothNumber = 75, ToothImagePath = TeethImages.ImgPathGreen };

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
    }
}
