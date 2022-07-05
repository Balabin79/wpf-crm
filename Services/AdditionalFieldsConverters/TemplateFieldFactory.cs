using Dental.Models;
using Dental.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Services.AdditionalFieldsConverters
{
    public class TemplateFieldFactory
    {
        public TemplateFieldFactory(ICollection<IAdditionalValue> values, ICollection<AdditionalField> fields)
        {
            this.values = values;
            this.fields = fields;
        }

        public ICollection<TemplateField> Run()
        {
            foreach (var val in values)
            {
                var item = new TemplateField();
                var field = fields.FirstOrDefault(f => f.Id == val.AdditionalFieldId);
                if (field == null) continue;

                // получаем ссылку на шаблон этого поля и записываем его в TemplateField
                /*код*/

                //значение
                item.Value = val.Value;
                Result.Add(item);
            }
            return Result;
        }

        private ICollection<TemplateField> Result { get; set; } = new List<TemplateField>();
        private readonly ICollection<IAdditionalValue> values;
        private readonly ICollection<AdditionalField> fields;
    }
}
