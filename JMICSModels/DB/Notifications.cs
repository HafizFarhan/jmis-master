using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    [Table("tbl_Notifications")]

    public partial class Notifications
    {
        [Key]
        [Column("Id")]
        public virtual int Id { get; set; }
        [Column("Notification_Content")]
        public virtual string NotificationContent { get; set; }
        [Column("Is_Read")]
        public virtual bool IsRead { get; set; } = false;
        [Column("Read_On")]
        public virtual DateTime? ReadOn { get; set; }
        [Column("Read_By")]
        public virtual string ReadBy { get; set; }
        [Column("Subscriber_Id")]
        public virtual int SubscriberId { get; set; }
        [Column("Notification_Type")]
        public virtual string NotificationType { get; set; }
        [Column("Report_Id")]
        public virtual int ReportId { get; set; }
        [Column("Active")]
        public virtual bool Active { get; set; } = true;
        [Column("Created_On")]
        public virtual DateTime CreatedOn { get; set; }
        [Column("Created_By")]
        public virtual string CreatedBy { get; set; }
        [Column("Last_Modified_On")]
        public virtual DateTime? LastModifiedOn { get; set; }
        [Column("Last_Modified_By")]
        public virtual string LastModifiedBy { get; set; }
    }
}
