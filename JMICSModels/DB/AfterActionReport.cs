using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    [Table("tbl_After_Action_Reports")]
    public partial class AfterActionReport
    {
        [Key]
        [Column("AAR_Id")]
        public virtual int AARId { get; set; }
        [Column("COI_Id")]
        public virtual int COIId { get; set; }
        [Column("PR_Id")]
        public virtual int PRId { get; set; }
        [Column("DR_Id")]
        public virtual int DRId { get; set; }
        [Column("Reporting_Datetime")]
        public virtual DateTime? ReportingDatetime { get; set; }
        [Column("Subscriber_Id")]
        public virtual int? SubscriberId { get; set; }
        [Column("Addressed_To")]
        public virtual string AddressedTo { get; set; }
        public virtual int[] AddressedToArray { get; set; }
        [Column("Initiation_Datetime")]
        [Required(ErrorMessage = "Initiation Date Required")]
        public virtual DateTime? InitiationDatetime { get; set; }
        [Column("COI_Type_Id")]
        [Required(ErrorMessage = "COI Type Required")]
        public virtual int? COITypeId { get; set; }
        [Column("Nature_Of_Threat_Id")]
        [Required(ErrorMessage = "Nature Of Threat Required")]
        public virtual int? NatureOfThreatId { get; set; }
        [Column("Initial_Reported_Latitude")]
        public virtual decimal? InitialReportedLatitude { get; set; }
        [Column("Initial_Reported_Longitude")]
        public virtual decimal? InitialReportedLongitude { get; set; }
        [Column("Initial_Reported_Course")]
        public virtual decimal? InitialReportedCourse { get; set; }
        [Column("Initial_Reported_Heading")]
        public virtual decimal? InitialReportedHeading { get; set; }
        [Column("Initial_Reported_Speed")]
        public virtual decimal? InitialReportedSpeed { get; set; }
        [Column("Initial_Reported_MMSI")]
        public virtual string InitialReportedMMSI { get; set; }
        [Column("Last_Reported_Latitude")]
        public virtual decimal? LastReportedLatitude { get; set; }
        [Column("Last_Reported_Longitude")]
        public virtual decimal? LastReportedLongitude { get; set; }
        [Column("Last_Reported_Course")]
        public virtual decimal? LastReportedCourse { get; set; }
        [Column("Last_Reported_Heading")]
        public virtual decimal? LastReportedHeading { get; set; }
        [Column("Last_Reported_Speed")]
        public virtual decimal? LastReportedSpeed { get; set; }
        [Column("Last_Reported_MMSI")]
        public virtual string LastReportedMMSI { get; set; }
        [Column("Remarks")]
        [Required(ErrorMessage = "Remarks Required")]
        public virtual string Remarks { get; set; }
        [Column("Info_Confidence_Level_Id")]
        public virtual int? InfoConfidenceLevelId { get; set; }
        [Column("Sources_Of_Info")]
        public virtual string SourcesOfInfo { get; set; }
        [Column("COI_Activation_DateTime")]
        public virtual DateTime? COIActivationDateTime { get; set; }
        [Column("Pre_Action_Preparations")]
        public virtual string PreActionPreparations { get; set; }
        [Column("Chronological_Summary")]
        public virtual string ChronologicalSummary { get; set; }
        [Column("Achieved_End_State")]
        public virtual string AchievedEndState { get; set; }
        [Column("Type_Of_Stakeholders_Support")]
        public virtual string TypeOfStakeholdersSupport { get; set; }
        [Column("Overall_Assessment")]
        public virtual string OverallAssessment { get; set; }
        [Column("Recommendations")]
        public virtual string Recommendations { get; set; }
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
