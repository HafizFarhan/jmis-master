using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    [Table("vw_Amplifying_Reports")]
    public partial class AmplifyingReportView : AmplifyingReport
    {
        [Column("Subscriber_Code")]
        public virtual string SubscriberCode { get; set; }

        [Column("COI_Number")]
        public virtual string COINumber { get; set; }

        [Column("PR_Number")]
        public virtual string PRNumber { get; set; }

        [Column("COI_Classification_Name")]
        public virtual string COIClassificationName { get; set; }

        [Column("Information_Addressee_Codes")]
        public virtual string InformationAddresseeCodes { get; set; }

        [Column("Action_Addressee_Codes")]
        public virtual string ActionAddresseeCodes { get; set; }

    }
}
