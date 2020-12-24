using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
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
