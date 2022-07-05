using Dental.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Services.AdditionalFieldsConverters
{
    public interface IAdditionalValue
    {
        string Value { get; set; }
        int? AdditionalFieldId { get; set; }
    }
}
