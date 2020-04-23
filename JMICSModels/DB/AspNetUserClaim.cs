using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    [Table("AspNetUserClaims")]
    public partial class AspNetUserClaim
    {
        [Key]
        [Column("Id")]
        public virtual int Id { get; set; }
        [Column("UserId")]
        public virtual string UserId { get; set; }
        [Column("ClaimType")]
        public virtual string ClaimType { get; set; }
        [Column("ClaimValue")]
        public virtual string ClaimValue { get; set; }
        public virtual AspNetUser AspNetUser { get; set; }
    }
}
