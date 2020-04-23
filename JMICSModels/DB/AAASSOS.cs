using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    [Table("tbl_AAAS_SOS")]
    public partial class AAASSOS
    {
        [Key]
        [Column("Id")]
        public virtual int Id { get; set; }
        [Column("User_Contact_Number")]
        public virtual string UserContactNumber { get; set; }
        [Column("User_IMEI")]
        public virtual decimal? UserIMEI { get; set; }
        [Column("Latitude")]
        public virtual decimal? Latitude { get; set; }
        [Column("Longitude")]
        public virtual decimal? Longitude { get; set; }
        [Column("Address")]
        public virtual string Address { get; set; }
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
