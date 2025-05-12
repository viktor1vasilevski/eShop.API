using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Base
{
    public abstract class AuditableBaseEntity : BaseEntity
    {
        public virtual string CreatedBy { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual string? LastModifiedBy { get; set; }
        public virtual DateTime? LastModified { get; set; }
    }
}
