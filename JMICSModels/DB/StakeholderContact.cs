using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
	[Table("tbl_Stakeholder_Contacts")]
	public partial class StakeholderContact
	{
		[Key]
		[Column("Contact_Id")]
		public virtual int ContactId { get; set; }
		[Column("Stakeholder_Id")]
		public virtual int StakeholderId { get; set; }
		[Column("Contact_Name")]
		public virtual string ContactName { get; set; }
		[Column("Contact_Designation")]
		public virtual string ContactDesignation { get; set; }
		[Column("Contact_Email")]
		public virtual string ContactEmail { get; set; }
		[Column("Contact_Work_Phone")]
		public virtual string ContactWorkPhone { get; set; }
		[Column("Contact_Cell_Phone")]
		public virtual string ContactCellPhone { get; set; }
		[Column("Contact_Fax_Number")]
		public virtual string ContactFaxNumber { get; set; }
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
