using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    [Table("tbl_Users")]
    public partial class User
    {
        [Key]
        [Column("User_Id")]
        public virtual int UserId { get; set; }
        [Column("Short_Name")]
        public virtual string ShortName { get; set; }
        [Column("First_Name")]
        public virtual string FirstName { get; set; }
        [Column("Last_Name")]
        public virtual string LastName { get; set; }
        [Column("Email")]
        public virtual string Email { get; set; }
        [Column("Phone")]
        public virtual string Phone { get; set; }
        [Column("User_Login")]
        public virtual string UserLogin { get; set; }
        [Column("Password")]
        public virtual string Password { get; set; }
        [Column("Subscriber_Id")]
        public virtual int? SubscriberId { get; set; }
        [Column("Login_Attempts")]
        public virtual int? LoginAttempts { get; set; }
        [Column("Is_Locked")]
        public virtual bool IsLocked { get; set; }
        [Column("Is_First_Login")]
        public virtual bool IsFirstLogin { get; set; }
        [Column("Is_User_Login_Verified")]
        public virtual bool IsUserLoginVerified { get; set; }
        [Column("Pass")]
        public virtual string Pass { get; set; }
        [Column("Password_Reset_Request")]
        public virtual bool PasswordResetRequest { get; set; }
        [Column("Password_Reset_On")]
        public virtual DateTime? PasswordResetOn { get; set; }
        [Column("User_Type_Id")]
        public virtual int? UserTypeId { get; set; }
        [Column("Last_Logged_In")]
        public virtual DateTime? LastLoggedIn { get; set; }
        [Column("Active")]
        public virtual bool Active { get; set; } = true;
        [Column("Created_On")]
        public virtual DateTime? CreatedOn { get; set; }
        [Column("Created_By")]
        public virtual string CreatedBy { get; set; }
        [Column("Last_Modified_On")]
        public virtual DateTime? LastModifiedOn { get; set; }
        [Column("Last_Modified_By")]
        public virtual string LastModifiedBy { get; set; }
    }
}
