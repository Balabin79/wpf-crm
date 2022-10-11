using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Models.Base
{
    public interface IModel
    {
        int Id { get; set; }
        int? CreatedAt { get; set; }
        int? UpdatedAt { get; set; }
        string Guid { get; set; }
    }
}
