using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    [Table("tbl_Rad_Units")]
    public partial class RadUnit
    {
        [Key]
        [Column("Rad_Unit_Id")]
        public virtual int RadUnitId { get; set; }
        [Column("Rad_Unit_Name")]
        public virtual string RadUnitName { get; set; }
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
