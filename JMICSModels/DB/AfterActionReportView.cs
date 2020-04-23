using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    [Table("vw_After_Action_Reports")]
    public partial class AfterActionReportView : AfterActionReport
    {
        [Column("Subscriber_Code")]
        public virtual string SubscriberCode { get; set; }

        [Column("COI_Number")]
        public virtual string COINumber { get; set; }

        [Column("PR_Number")]
        public virtual string PRNumber { get; set; }

        [Column("COI_Type_Name")]
        public virtual string COITypeName { get; set; }

        [Column("Threat_Name")]
        public virtual string ThreatName { get; set; }

        [Column("Info_Confidence_Level_Name")]
        public virtual string InfoConfidenceLevelName { get; set; }

        [Column("Addressed_To_Codes")]
        public virtual string AddressedToCodes { get; set; }
    }
}
