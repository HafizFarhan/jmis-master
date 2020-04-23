using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    [Table("vw_Preliminary_Reports")]
    public partial class PreliminaryReportView: PreliminaryReport
    {
        [Column("COI_Number")]
        public virtual string COINumber { get; set; }

        [Column("Subscriber_Code")]
        public virtual string SubscriberCode { get; set; }

        [Column("COI_Type_Name")]
        public virtual string COITypeName { get; set; }

        [Column("Threat_Name")]
        public virtual string ThreatName { get; set; }

        [Column("Information_Addressee_Codes")]
        public virtual string InformationAddresseeCodes { get; set; }

        [Column("Action_Addressee_Codes")]
        public virtual string ActionAddresseeCodes { get; set; }
    }
}
