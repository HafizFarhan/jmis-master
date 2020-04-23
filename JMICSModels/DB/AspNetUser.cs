using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    [Table("AspNetUsers")]
    public partial class AspNetUser
    {
        [Key]
        [Column("Id")]
        public virtual string Id { get; set; }
        [Column("UserName")]
        public virtual string UserName { get; set; }
        [Column("NormalizedUserName")]
        public virtual string NormalizedUserName { get; set; }
        [Column("Email")]
        public virtual string Email { get; set; }
        [Column("NormalizedEmail")]
        public virtual string NormalizedEmail { get; set; }
        [Column("EmailConfirmed")]
        public virtual bool EmailConfirmed { get; set; }
        [Column("PasswordHash")]
        public virtual string PasswordHash { get; set; }
        [Column("SecurityStamp")]
        public virtual string SecurityStamp { get; set; }
        [Column("ConcurrencyStamp")]
        public virtual string ConcurrencyStamp { get; set; }
        [Column("PhoneNumber")]
        public virtual string PhoneNumber { get; set; }
        [Column("PhoneNumberConfirmed")]
        public virtual bool PhoneNumberConfirmed { get; set; }
        [Column("TwoFactorEnabled")]
        public virtual bool TwoFactorEnabled { get; set; }
        [Column("LockoutEnd")]
        public virtual string LockoutEnd { get; set; }
        [Column("LockoutEnabled")]
        public virtual bool LockoutEnabled { get; set; }
        [Column("AccessFailedCount")]
        public virtual int AccessFailedCount { get; set; }
        [Column("Subscriber_Id")]
        public virtual int? SubscriberId { get; set; }
        public virtual IEnumerable<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual IEnumerable<AspNetUserRole> AspNetUserRoles { get; set; }
        public virtual IEnumerable<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual IEnumerable<AspNetUserToken> AspNetUserTokens { get; set; }
    }
}
