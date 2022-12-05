﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Models.Base
{
    public interface IInvoiceItem
    {
        int Id{ get; set; }
        string Name { get; set; }
        string Code { get; set; }
    }
}
