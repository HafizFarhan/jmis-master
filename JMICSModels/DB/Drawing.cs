using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    [Table("tbl_Drawings")]
    public partial class Drawing
    {
        [Key]
        [Column("Drawing_Id")]
        public virtual int DrawingId { get; set; }
        [Column("Shape_Id")]
        [Required(ErrorMessage = "Shape Type Required")]
        public virtual int ShapeId { get; set; }
        [Column("Subscriber_Id")]
        public virtual int SubscriberId { get; set; }

        [Column("Drawing_Type")]
        public virtual string DrawingType { get; set; }

        [Column("Drawing_Name")]
        [Required(ErrorMessage = "Name Required")]
        public virtual string DrawingName { get; set; }

        [Column("Drawing_Fill_Color")]
        public virtual string DrawingFillColor { get; set; }

        [Column("Drawing_Outline_Color")]
        public virtual string DrawingOutlineColor { get; set; }

        [Column("Circle_Radius_Unit_Id")]
        public virtual int? CircleRadiusUnitId { get; set; }

        [Column("Circle_Radius")]
        //[Required(ErrorMessage = "Radius Required")]
        public virtual decimal? CircleRadius { get; set; }
        
        [Column("Circle_Latitude")]
        //[Required(ErrorMessage = "Latitude Required")]
        public virtual decimal? CircleLatitude { get; set; }

        [Column("Circle_Longitude")]
        //[Required(ErrorMessage = "Longitude Required")]
        public virtual decimal? CircleLongitude { get; set; }

        [Column("Drawing_Source")]
        public virtual string DrawingSource { get; set; }
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
