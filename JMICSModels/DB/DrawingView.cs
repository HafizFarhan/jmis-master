using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    [Table("vw_Drawings")]
    public partial class DrawingView : Drawing
    {
        [Column("Shape_Name")]
        public virtual string ShapeName { get; set; }

        [Column("Rad_Unit_Name")]
        public virtual string RadiusUnitName { get; set; }
    }
}
