﻿using B6CRM.Services;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace B6CRM.Services.Files
{
    class UserFilesManagement : AbstractFilesManagement
    {
        public UserFilesManagement(string Guid) : base(Path.Combine(new Config().PathToFilesDirectory, Guid)) { }

    }
}
