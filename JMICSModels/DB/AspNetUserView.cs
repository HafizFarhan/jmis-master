using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    [Table("vw_AspNetUsers")]
    public class AspNetUserView : AspNetUser
    {
        [Column("Subscriber_Name")]
        public virtual string SubscriberName { get; set; }
    }
}
