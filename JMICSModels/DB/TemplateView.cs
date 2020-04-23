using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    [Table("vw_Templates")]
    public class TemplateView : Template
    {
        [Column("Subscriber_Code")]
        public virtual string SubscriberCode { get; set; }

        [Column("Template_Type_Name")]
        public virtual string TemplateTypeName { get; set; }

        [Column("Addressed_To_Codes")]
        public virtual string AddressedToCodes { get; set; }
    }
}
