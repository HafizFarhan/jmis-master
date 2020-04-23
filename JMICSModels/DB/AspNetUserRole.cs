using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    [Table("AspNetUserRoles")]
    public partial class AspNetUserRole
    {
        [Key]
        [Column("UserId")]
        public virtual string UserId { get; set; }
        [Column("RoleId")]
        public virtual string RoleId { get; set; }
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual AspNetRole AspNetRole { get; set; }
    }
}
