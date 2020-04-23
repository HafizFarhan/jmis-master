using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    [Table("tbl_Drawing_Coordinates")]
    public partial class DrawingCoordinate
    {
        [Key]
        [Column("Drawing_Coordinate_Id")]
        public virtual int DrawingCoordinateId { get; set; }
        [Column("Drawing_Coordinate_Latitude")]
        public virtual decimal DrawingCoordinateLatitude { get; set; }
        [Column("Drawing_Coordinate_Longitude")]
        public virtual decimal DrawingCoordinateLongitude { get; set; }
        [Column("Drawing_Id")]
        public virtual int DrawingId { get; set; }
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
