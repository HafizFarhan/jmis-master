using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    [Table("tbl_Cities")]
    public partial class City
    {
        [Key]
        [Column("Id")]
        public virtual int Id { get; set; }
        [Column("City_Id")]
        public virtual int CityId { get; set; }
        [Column("City_Name")]
        public virtual string CityName { get; set; }
        [Column("City_Lat")]
        public virtual decimal? CityLat { get; set; }
        [Column("City_Lon")]
        public virtual decimal? CityLon { get; set; }
        [Column("Country")]
        public virtual string Country { get; set; }
    }
}
