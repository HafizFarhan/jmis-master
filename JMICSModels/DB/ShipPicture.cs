using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
	[Table("tbl_Ship_Pictures")]
	public partial class ShipPicture
	{
		[Key]
		[Column("Ship_Picture_Id")]
		public virtual int ShipPictureId { get; set; }
		[Column("Picture_Name")]
		public virtual int? PictureName { get; set; }
		[Column("IMO")]
		public virtual string IMO { get; set; }
		[Column("Copyright")]
		public virtual string Copyright { get; set; }
		[Column("Date_Of_Photo")]
		public virtual DateTime? DateOfPhoto { get; set; }
		[Column("Created_On")]
		public virtual DateTime? CreatedOn { get; set; }
		[Column("Created_By")]
		public virtual string CreatedBy { get; set; }
	}
}
