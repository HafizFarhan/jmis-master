using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    [Table("tbl_Event_Types")]
    public partial class EventType
    {
        [Key]
        [Column("Event_Type_Id")]
        public virtual int EventTypeId { get; set; }
        [Column("Event_Type_Name")]
        public virtual string EventTypeName { get; set; }
        [Column("Active")]
        public virtual bool Active { get; set; } = true;
        [Column("Created_On")]
        public virtual DateTime CreatedOn { get; set; }
        [Column("Created_By")]
        public virtual string CreatedBy { get; set; }
    }
}
