using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    [Table("vw_Lost_Contact_Reports")]
    public partial class LostContactReportView : LostContactReport
    {
        [Column("COI_Number")]
        public virtual string COINumber { get; set; }

        [Column("Subscriber_Code")]
        public virtual string SubscriberCode { get; set; }

        [Column("PR_Number")]
        public virtual string PRNumber { get; set; }

        [Column("Action_Addressee_Codes")]
        public virtual string ActionAddresseeCodes { get; set; }
    }
}
