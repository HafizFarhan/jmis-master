using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    [Table("vw_Notifications")]
    public class NotificationView : Notifications
    {
        [Column("Subscriber_Name")]
        public virtual string SubscriberName { get; set; }
    }
}
