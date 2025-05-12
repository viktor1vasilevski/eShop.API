using Domain.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Category : AuditableBaseEntity
    {
        public string Name { get; set; }


        public virtual ICollection<Subcategory>? Subcategories { get; set; }
    }
}
