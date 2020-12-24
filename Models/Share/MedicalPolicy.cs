using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Models.Share
{
    class MedicalPolicy
    {

        public string SerialNumber { get; set; } // серия
        public string Number { get; set; } // номер
        /// <summary>
        /// Наименование медицинской страховой организации
        /// </summary>
        public string Name { get; set; } 
    }
}
