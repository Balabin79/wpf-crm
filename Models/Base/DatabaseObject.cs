using System.ComponentModel.DataAnnotations;

namespace Dental.Models.Base
{
    public abstract class DatabaseObject //: IDataErrorInfo
    {
        //protected DatabaseObject();

        [Key]
        public long Id { get; set; }
    }
}
