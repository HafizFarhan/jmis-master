using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    [Table("vw_Drop_Reports")]
    public partial class DropReportView : DropInfoSharingReport
    {
        [Column("COI_Number")]
        public virtual string COINumber { get; set; }

        [Column("Subscriber_Code")]
        public virtual string SubscriberCode { get; set; }

        [Column("PR_Number")]
        public virtual string PRNumber { get; set; }

        [Column("COI_Status")]
        public virtual string COIstatus { get; set; }

        [Column("Action_Addressee_Codes")]
        public virtual string ActionAddresseeCodes { get; set; }
    }
}
