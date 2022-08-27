using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Models.Base
{
    public interface IMedicalTemplate
    {
        int Id { get; set; }
        int? IsDir { get; set; }
        int? UpdatedAt { get; set; }
        int? CreatedAt { get; set; }
        int? ParentId { get; set; }
        IMedicalTemplate Parent { get; set; }
        string Name { get; set; }
        string Guid { get; set; }
    }
}
