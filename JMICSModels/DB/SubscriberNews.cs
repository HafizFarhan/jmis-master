using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    [Table("tbl_Subscriber_News")]
    public partial class SubscriberNews
    {
        [Key]
        [Column("Id")]
        public virtual int Id { get; set; }
        [Column("News_Id")]
        public virtual int NewsId { get; set; }
        [Column("Subscriber_Id")]
        public virtual int SubscriberId { get; set; }
        [Column("Active")]
        public virtual bool Active { get; set; } = true;
    }
}
