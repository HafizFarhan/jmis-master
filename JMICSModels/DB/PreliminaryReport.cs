using MTC.JMICS.Utility.Validators;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace MTC.JMICS.Models.DB
{
	[Table("tbl_Preliminary_Reports")]
	public partial class PreliminaryReport
	{
		[Key]
		[Column("Id")]
		public virtual int Id { get; set; }
		[Column("COI_Id")]
		public virtual int? COIId { get; set; }
		[Column("PR_Number")]
		public virtual string PRNumber { get; set; }
		[Column("Title")]
		public virtual string Title { get; set; }
		[Column("Reporting_Datetime")]
		public virtual DateTime? ReportingDatetime { get; set; }
		[Column("Subscriber_Id")]
		public virtual int? SubscriberId { get; set; }
		public virtual int[] ActionAddresseeArray { get; set; }
		[Required(ErrorMessage = "Info Addressee Required")]
		public virtual int[] InformationAddresseeArray { get ; set; }
		[Column("Action_Addressee")]
		public virtual string ActionAddressee { get; set; }
		[Column("Information_Addressee")]
		public virtual string InformationAddressee { get; set; }
		[Column("COI_Type_Id")]
		[Required(ErrorMessage = "COI Type Required")]
		public virtual int? COITypeId { get; set; }
		[Column("Nature_Of_Threat_Id")]
		[Required(ErrorMessage = "Nature Of Threat Required")]
		public virtual int? NatureOfThreatId { get; set; }
		[Column("Last_Observation_Datetime")]
		[Required(ErrorMessage = "Observation Date Required")]
		public virtual DateTime? LastObservationDatetime { get; set; }
		[Column("Remarks")]
		[Required(ErrorMessage = "Remarks Required")]
		public virtual string Remarks { get; set; }
		
		
		[Column("Latitude")]
		[Required(ErrorMessage = "Latitude Required")]
		//[RegularExpression(@"([0-9]{1,2})[:|°]([0-9]{1,2})[:|'|′]?([0-9]{1,2}(?:\.[0-9]+){0,1})?["" |""″] ([N | S])", ErrorMessage = "Invalid Latitude (24° 50′ 42″ N/S)")]
		public virtual decimal? Latitude { get; set; }


		[Column("Longitude")]
		[Required(ErrorMessage = "Longitude Required")]
		public virtual decimal? Longitude { get; set; }
		[Column("Course")]
		//[Required(ErrorMessage = "Course Required")]
		public virtual decimal? Course { get; set; }
		[Column("Heading")]
		//[Required(ErrorMessage = "Heading Required")]
		public virtual decimal? Heading { get; set; }
		[Column("speed")]
		//[Required(ErrorMessage = "Speed Required")]
		public virtual decimal? Speed { get; set; }
		[Column("MMSI")]
		//[Required(ErrorMessage = "MMSI Required")]
		public virtual string MMSI { get; set; }
		[Column("Is_Dropped")]
		public virtual bool? IsDropped { get; set; } = false;
		[Column("Is_Lost")]
		public virtual bool? IsLost { get; set; } = false;
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
