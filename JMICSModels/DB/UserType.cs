using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    [Table("tbl_User_Types")]
    public partial class UserType
    {
        [Key]
        [Column("User_Type_Id")]
        public virtual int UserTypeId { get; set; }
        [Column("User_Type_Name")]
        public virtual string UserTypeName { get; set; }
        [Column("Active")]
        public virtual bool Active { get; set; }
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
