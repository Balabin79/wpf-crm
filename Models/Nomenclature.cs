﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using Dental.Models.Base;
using System.ComponentModel.DataAnnotations;
using DevExpress.Mvvm;
using System.Reflection;
using Dental.Interfaces;

namespace Dental.Models
{
    [Table("Nomenclature")]
    class Nomenclature : AbstractBaseModel, ITreeModel, IDataErrorInfo, ITreeViewCollection
    {
        [Required]
        [MaxLength(255)]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Код")]
        public string Code { get; set; }

        [Display(Name = "Артикул")]
        public string VendorCode { get; set; }

        public int? ParentId { get; set; }
        public int IsDir { get; set; }

        public int? UnitId { get; set; }
        public Unit Unit { get; set; }

        public int? NumberInPack { get; set; } // кол-во в упаковке

        [Display(Name = "Штрих-Код")]
        public string BarCode { get; set; }



        // код, артикул, номенклатурная группа , ед.изм., кол-во при выборе некоторых полей,

        public bool this[PropertyInfo prop, Nomenclature item]
        {
            get
            {
                switch (prop.Name)
                {
                    case "Id": return item.Id == Id;
                    case "Name": return item.Name == Name;
                    case "Description": return item.Description == Description;
                    case "Code": return item.Code == Code;
                    case "VendorCode": return item.VendorCode == VendorCode;
                    case "ParentId": return item.ParentId == ParentId;
                    case "IsDir": return item.IsDir == IsDir;
                    case "Unit": return item.Unit == Unit;
                    default: return true;
                }
            }
        }

        public void Copy(Nomenclature copy)
        {
            Name = copy.Name;
            Description = copy.Description;
            Code = copy.Code;
            VendorCode = copy.VendorCode;
            ParentId = copy.ParentId;
            IsDir = copy.IsDir;
            Unit = copy.Unit;
        }

        public string Error
        {
            get
            {
                return string.Empty;
            }
        }

        public string this[string columnName]
        {
            get
            {
                return IDataErrorInfoHelper.GetErrorText(this, columnName);
            }
        }
    }  
}
