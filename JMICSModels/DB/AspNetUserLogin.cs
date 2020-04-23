using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    [Table("AspNetUserLogins")]
    public partial class AspNetUserLogin
    {
        [Key]
        [Column("LoginProvider")]
        public virtual string LoginProvider { get; set; }
        [Column("ProviderKey")]
        public virtual string ProviderKey { get; set; }
        [Column("ProviderDisplayName")]
        public virtual string ProviderDisplayName { get; set; }
        [Column("UserId")]
        public virtual string UserId { get; set; }
        public virtual AspNetUser AspNetUser { get; set; }
    }
}
