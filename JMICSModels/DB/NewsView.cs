using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    [Table("vw_News")]
    public partial class NewsView : News
    {
        [Column("Subscriber_Name")]
        public virtual string SubscriberName { get; set; }

        [Column("News_Type_Name")]
        public virtual string NewsTypeName { get; set; }

        [Column("News_Status")]
        public virtual string NewsStatus { get; set; }

    }
}
