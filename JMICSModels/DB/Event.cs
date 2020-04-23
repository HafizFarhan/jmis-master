using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    [Table("tbl_Events")]
    public partial class Event
    {
        [Key]
        [Column("Event_Id")]
        public virtual int EventId { get; set; }
        [Column("Event_Type_Id")]
        public virtual int EventTypeId { get; set; }
        [Column("Subscriber_Id")]
        public virtual int SubscriberId { get; set; }
        [Column("Event_Description")]
        public virtual string EventDescription { get; set; }
        [Column("Active")]
        public virtual bool Active { get; set; } = true;
        [Column("Created_On")]
        public virtual DateTime CreatedOn { get; set; }
        [Column("Created_By")]
        public virtual string CreatedBy { get; set; }
    }
}
