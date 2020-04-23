using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    [Table("tbl_Weather_Details")]
    public partial class WeatherDetail
    {
        [Key]
        [Column("Weather_Id")]
        public virtual int WeatherId { get; set; }
        [Column("City_Id")]
        public virtual int? CityId { get; set; }
        [Column("City_Name")]
        public virtual string Name { get; set; }
        [Column("Weather_Main")]
        public virtual string Main { get; set; }
        [Column("Weather_Description")]
        public virtual string Description { get; set; }
        [Column("Temperature")]
        public virtual decimal? Temp { get; set; }
        [Column("Pressure")]
        public virtual decimal? Pressure { get; set; }
        [Column("Humidity")]
        public virtual decimal? Humidity { get; set; }
        [Column("Temp_Min")]
        public virtual decimal? TempMin { get; set; }
        [Column("Temp_Max")]
        public virtual decimal? TempMax { get; set; }
        [Column("Wind_Speed")]
        public virtual decimal? Speed { get; set; }
        [Column("Wind_Degree")]
        public virtual decimal? Deg { get; set; }
        [Column("Country")]
        public virtual string Country { get; set; }
        [Column("Sunrise")]
        public virtual DateTime? Sunrise { get; set; }
        [Column("Sunset")]
        public virtual DateTime? Sunset { get; set; }
        [Column("TimeZone")]
        public virtual string Timezone { get; set; }
        [Column("Recorded_On")]
        public virtual DateTime? Dt { get; set; }
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
