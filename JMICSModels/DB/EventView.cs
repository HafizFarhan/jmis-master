using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    [Table("vw_Events")]
    public class EventView : Event
    {
        [Column("Event_Type_Name")]
        public virtual string EventTypeName { get; set; }
        [Column("Subscriber_Name")]
        public virtual string SubscriberName { get; set; }
    }
}
