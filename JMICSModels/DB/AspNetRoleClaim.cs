using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    [Table("AspNetRoleClaims")]
    public partial class AspNetRoleClaim
    {
        [Key]
        [Column("Id")]
        public virtual int Id { get; set; }
        [Column("RoleId")]
        public virtual string RoleId { get; set; }
        [Column("ClaimType")]
        public virtual string ClaimType { get; set; }
        [Column("ClaimValue")]
        public virtual string ClaimValue { get; set; }
        public virtual AspNetRole AspNetRole { get; set; }
    }
}
