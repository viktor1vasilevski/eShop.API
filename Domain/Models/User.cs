using Domain.Models.Base;
using eShop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain.Models
{
    public class User : AuditableBaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public Role Role { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string SaltKey { get; set; }


        //public virtual ICollection<Order>? Orders { get; set; }
        //public virtual ICollection<UserBasket>? UserBaskets { get; set; }
        //public virtual ICollection<Comment>? Comments { get; set; }
    }
}
