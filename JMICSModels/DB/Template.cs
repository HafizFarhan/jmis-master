using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
	[Table("tbl_Templates")]
	public partial class Template
	{
		[Key]
		[Column("Id")]
		public virtual int Id { get; set; }
		[Column("Template_Type_Id")]
		public virtual int? TemplateTypeId { get; set; }
		[Column("Reporting_Datetime")]
		public virtual DateTime? ReportingDatetime { get; set; }
		[Column("Subscriber_Id")]
		public virtual int? SubscriberId { get; set; }
		[Required(ErrorMessage = "Addressed To Required")]
		public virtual int[] AddressedToArray { get; set; }
		[Column("Addressed_To")]
		public virtual string AddressedTo { get; set; }
		[Column("Remarks")]
		public virtual string Remarks { get; set; }
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
