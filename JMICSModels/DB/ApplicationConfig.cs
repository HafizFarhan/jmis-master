using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    [Table("tbl_Application_Config")]
    public class ApplicationConfig
    {
        [Key]
        [Column("Application_Config_Id")]
        public int ApplicationConfigId { set; get; }
        [Column("Environment")]
        public string Environment { set; get; }
        [Column("Key")]
        public string Key { set; get; }
        [Column("Value")]
        public string Value { set; get; }
        [Column("Comment")]
        public string Comment { set; get; }
        [Column("Value_Data_Type")]
        public string ValueDataType { set; get; }
        [Column("Active")]
        public virtual bool Active { get; set; }
        [Column("Created_On")]
        public virtual DateTime? CreatedOn { get; set; }
        [Column("Created_By")]
        public virtual string CreatedBy { get; set; }
        [Column("Last_Modified_On")]
        public virtual DateTime? LastModifiedOn { get; set; }
        [Column("Last_Modified_By")]
        public virtual string LastModifiedBy { get; set; }
    }
}
