using System;
using System.Collections.Generic;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    //[Table("tbl_COIs")]
    public class COIMinimal
    {
        //[Column("COI_Number")]
        public virtual string COINumber { get; set; }
       
        //[Column("Reporting_Datetime")]
        public virtual DateTime? ReportingDatetime { get; set; }

        //[Column("Latitude")]
        //[Required(ErrorMessage = "Latitude Required")]
        public virtual decimal? Latitude { get; set; }

        //[Column("Longitude")]
        //[Required(ErrorMessage = "Longitude Required")]
        public virtual decimal? Longitude { get; set; }

        //[Column("COI_Type_Id")]
        //[Required(ErrorMessage = "COI Type Required")]
        public virtual int? COITypeId { get; set; }
    }
}
