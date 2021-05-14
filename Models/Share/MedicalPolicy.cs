namespace Dental.Models.Share
{
    public class MedicalPolicy
    {

        public string SerialNumber { get; set; } // серия
        public string Number { get; set; } // номер
        /// <summary>
        /// Наименование медицинской страховой организации
        /// </summary>
        public string Name { get; set; } 
    }
}
