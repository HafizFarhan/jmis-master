using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    [Table("AspNetUserTokens")]
    public partial class AspNetUserToken
    {
        [Key]
        [Column("UserId")]
        public virtual string UserId { get; set; }
        [Column("LoginProvider")]
        public virtual string LoginProvider { get; set; }
        [Column("Name")]
        public virtual string Name { get; set; }
        [Column("Value")]
        public virtual string Value { get; set; }
        public virtual AspNetUser AspNetUser { get; set; }
    }
}
