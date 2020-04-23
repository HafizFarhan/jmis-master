using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    [Table("tbl_Drawing_Properties")]
    public partial class DrawingProperty
    {
        [Key]
        [Column("Drawing_Property_Id")]
        public virtual int DrawingPropertyId { get; set; }
        [Column("Drawing_Property_Name")]
        public virtual string DrawingPropertyName { get; set; }
        [Column("Drawing_Fill_Color")]
        public virtual string DrawingFillColor { get; set; }
        [Column("Drawing_Stroke_Color")]
        public virtual string DrawingStrokeColor { get; set; }
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
