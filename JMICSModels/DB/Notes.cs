using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
	[Table("tbl_Notes")]
	public partial class Notes
	{
		[Key]
		[Column("Note_Id")]
		public virtual int NoteId { get; set; }
		[Column("Description")]
		[Required(ErrorMessage = "Notes description required")]
		public virtual string Description { get; set; }
		[Column("Active")]
		public virtual bool? Active { get; set; } = true;
		[Column("Created_On")]
		public virtual DateTime CreatedOn { get; set; }
		[Column("Created_By")]
		public virtual string CreatedBy { get; set; }
	}
}
