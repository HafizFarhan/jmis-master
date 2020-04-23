using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
	[Table("tbl_Drop_Info_Sharing_Reports")]
	public partial class DropInfoSharingReport
	{
		[Key]
		[Column("Id")]
		public virtual int Id { get; set; }
		[Column("COI_Id")]
		public virtual int COIId { get; set; }
		[Column("PR_Id")]
		public virtual int PRId { get; set; }
		[Column("Title")]
		public virtual string Title { get; set; }
		[Column("Reporting_Datetime")]
		public virtual DateTime? ReportingDatetime { get; set; }
		[Column("Subscriber_Id")]
		public virtual int? SubscriberId { get; set; }

        [Column("Action_Addressee")]
		public virtual string ActionAddressee { get; set; }

        public virtual int[] ActionAddresseeArray { get; set; }

        [Column("COI_Status_Id")]
		public virtual int? COIStatusId { get; set; }

		[Column("Remarks")]
		[Required(ErrorMessage = "Remarks Required")]
		public virtual string Remarks { get; set; }
		[Column("AAR_Created")]
		public virtual bool? AARCreated { get; set; } = false;
		[Column("Latitude")]
		[Required(ErrorMessage = "Latitude Required")]
		public virtual decimal? Latitude { get; set; }
		[Column("Longitude")]
		[Required(ErrorMessage = "Longitude Required")]
		public virtual decimal? Longitude { get; set; }
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
