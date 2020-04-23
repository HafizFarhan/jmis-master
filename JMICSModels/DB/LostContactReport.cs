using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
	[Table("tbl_Lost_Contact_Reports")]
	public partial class LostContactReport
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
        [Column("Last_Observation_Datetime")]
        [Required(ErrorMessage = "Observation Date Required")]
        public virtual DateTime LastObservationDatetime { get; set; }
		[Column("Remarks")]
        [Required(ErrorMessage = "Remarks Required")]
        public virtual string Remarks { get; set; }
		[Column("Latitude")]
        [Required(ErrorMessage = "Latitude Required")]
        public virtual decimal? Latitude { get; set; }
		[Column("Longitude")]
        [Required(ErrorMessage = "Longitude Required")]
        public virtual decimal? Longitude { get; set; }
		[Column("Course")]
		public virtual decimal? Course { get; set; }
		[Column("Heading")]
		public virtual decimal? Heading { get; set; }
		[Column("Speed")]
		public virtual decimal? Speed { get; set; }
		[Column("MMSI")]
		public virtual long? MMSI { get; set; }
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
