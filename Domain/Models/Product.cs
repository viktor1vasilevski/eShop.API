using Domain.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Product : AuditableBaseEntity
    {
        public string? Brand { get; set; }
        public string? Description { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? UnitQuantity { get; set; }


        public Guid SubcategoryId { get; set; }
        public virtual Subcategory? Subcategory { get; set; }
    }
}
