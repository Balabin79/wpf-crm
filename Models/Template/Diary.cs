﻿using System.ComponentModel.DataAnnotations.Schema;
using Dental.Interfaces;
using Dental.Models.Base;

namespace Dental.Models.Template
{
    [Table("Diary")]
    class Diary : TreeModelBase, ITreeViewCollection
    {

    }
}