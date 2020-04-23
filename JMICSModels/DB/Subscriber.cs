using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    [Table("tbl_Subscribers")]
    public partial class Subscriber
    {
        [Key]
        [Column("Subscriber_Id")]
        public virtual int SubscriberId { get; set; }
        [Column("Subscriber_Name")]
        [Required(ErrorMessage = "Name Required")]
        public virtual string SubscriberName { get; set; }
        [Column("Subscriber_Code")]
        [Required(ErrorMessage = "Code Name Required")]
        public virtual string SubscriberCode { get; set; }
        [Column("Address")]
        [Required(ErrorMessage = "Address Required")]
        public virtual string Address { get; set; }
        [Column("State_Code")]
        [Required(ErrorMessage = "StateCode Required")]
        public virtual string StateCode { get; set; }
        [Column("City")]
        [Required(ErrorMessage = "City Required")]
        public virtual string City { get; set; }
        [Column("Zip")]
        [Required(ErrorMessage = "Zip Required")]
        public virtual string Zip { get; set; }
        [Column("Email")]
        [Required(ErrorMessage = "Email Required")]
        public virtual string Email { get; set; }
        [Column("Phone")]
        [Required(ErrorMessage = "Phone Required")]
        public virtual string Phone { get; set; }
        [Column("Subscriber_TimeZone")]
        [Required(ErrorMessage = "TimeZone Required")]
        public virtual string SubscriberTimeZone { get; set; }
        [Column("Is_Approved")]
        public virtual bool IsApproved { get; set; }
        [Column("Active")]
        public virtual bool Active { get; set; }
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
