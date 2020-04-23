using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
	[Table("tbl_News_Feeds")]
	public partial class NewsFeed
	{
		[Key]
		[Column("News_Feed_Id")]
		public virtual int NewsFeedId { get; set; }
		[Column("Subscriber_Id")]
		public virtual int SubscriberId { get; set; }

		[Column("News_Feed_Type_Id")]
		[Required(ErrorMessage = "News Feed Type Required")]
		public virtual int NewsFeedTypeId { get; set; }

		[Column("News_Source_Url")]
		[Required(ErrorMessage = "Source URL Required")]
		public virtual string NewsSourceUrl { get; set; }

		[Column("Reported_To")]
		[Required(ErrorMessage = "Reported To Required")]
		public virtual string ReportedTo { get; set; }
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
