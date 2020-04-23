using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
	[Table("vw_News_Feeds")]
	public partial class NewsFeedView : NewsFeed
	{
		[Column("Subscriber_Name")]
		public virtual string SubscriberName { get; set; }

		[Column("News_Feed_Type_Name")]
		public virtual string NewsFeedTypeName { get; set; }
	}
}
