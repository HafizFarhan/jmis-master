using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    [Table("tbl_News")]
    public partial class News
    {
        [Key]
        [Column("News_Id")]
        public virtual int NewsId { get; set; }
        [Column("Subscriber_Id")]
        public virtual int SubscriberId { get; set; }
        [Column("News_Type_Id")]
        public virtual int NewsTypeId { get; set; }
        [Column("News_Heading")]
        public virtual string NewsHeading { get; set; }
        [Column("Reported_To")]
        public virtual string ReportedTo { get; set; }
        [Column("News_Description")]
        public virtual string NewsDescription { get; set; }
        [Column("News_Status_Id")]
        public virtual int? NewsStatusId { get; set; }
        [Column("News_Activation_Date")]
        public virtual DateTime? NewsActivationDate { get; set; }
        [Column("News_Deactivation_Date")]
        public virtual DateTime? NewsDeactivationDate { get; set; }
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
