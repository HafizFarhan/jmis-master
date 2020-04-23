using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    [Table("tbl_Drawing_Shapes")]
    public partial class DrawingShape
    {
        [Key]
        [Column("Shape_Id")]
        public virtual int ShapeId { get; set; }
        [Column("Shape_Name")]
        public virtual string ShapeName { get; set; }
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
