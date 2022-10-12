using Dental.Models;
using Dental.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Services
{
    static class Reestr
    {
        public static void Update(object p)
        {
            try
            {
                using (var db = new ApplicationContext())
                {
                    int id = 0;
                    if (int.TryParse(p?.ToString(), out int res)) id = res;

                    if (p is IModel)
                    {
                        var attr = p?.GetType().GetCustomAttributes(false);
                        if (attr.Length > 0)
                        {
                            var name = attr.OfType<TableAttribute>()?.ToArray().Select(f => f.Name).FirstOrDefault();
                            id = (int)Enum.Parse(typeof(Tables), name?.ToString());
                        }
                    }

                    if (id == 0) return;
                    var value = db.Reestr.FirstOrDefault(f => f.Table == id);
                    if (value == null)
                    {

                        db.Reestr.Add(
                            new Models.Reestr()
                            {
                                UpdatedAt = GetTimestamp(),
                                Table = id,
                                IsSynchronized = 0
                            });
                    }
                    else
                    {
                        value.UpdatedAt = GetTimestamp();
                        value.IsSynchronized = 0;
                    }
                    db.SaveChanges();
                }
            }
            catch { }
        }

        static int GetTimestamp() => (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
    }
}
