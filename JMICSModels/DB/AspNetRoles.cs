using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    [Table("AspNetRoles")]
    public partial class AspNetRole
    {
        [Key]
        [Column("Id")]
        public virtual string Id { get; set; }
        [Column("Name")]
        public virtual string Name { get; set; }
        [Column("NormalizedName")]
        public virtual string NormalizedName { get; set; }
        [Column("ConcurrencyStamp")]
        public virtual string ConcurrencyStamp { get; set; }
        public virtual IEnumerable<AspNetRoleClaim> AspNetRoleClaims { get; set; }
        public virtual IEnumerable<AspNetUserRole> AspNetUserRoles { get; set; }
    }
}
