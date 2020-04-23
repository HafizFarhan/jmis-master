using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
    [Table("tbl_AIS_Tracks")]
    public partial class AISTrack
    {
        [Key]
        [Column("AIS_Track_Id")]
        public virtual int AISTrackId { get; set; }
        [Column("MMSI")]
        public virtual long? MMSI { get; set; }
        [Column("IMO")]
        public virtual long? IMO { get; set; }
        [Column("SHIP_ID")]
        public virtual long? SHIPID { get; set; }
        [Column("LAT")]
        public virtual decimal? LAT { get; set; }
        [Column("LON")]
        public virtual decimal? LON { get; set; }
        [Column("SPEED")]
        public virtual decimal? SPEED { get; set; }
        [Column("HEADING")]
        public virtual decimal? HEADING { get; set; }
        [Column("COURSE")]
        public virtual decimal? COURSE { get; set; }
        [Column("STATUS")]
        public virtual int? STATUS { get; set; }
        [Column("TIMESTAMP")]
        public virtual DateTime? TIMESTAMP { get; set; }
        [Column("DSRC")]
        public virtual string DSRC { get; set; }
        [Column("UTC_SECONDS")]
        public virtual int? UTCSECONDS { get; set; }
        [Column("SHIPNAME")]
        public virtual string SHIPNAME { get; set; }
        [Column("SHIPTYPE")]
        public virtual int? SHIPTYPE { get; set; }
        [Column("CALLSIGN")]
        public virtual string CALLSIGN { get; set; }
        [Column("FLAG")]
        public virtual string FLAG { get; set; }
        [Column("LENGTH")]
        public virtual decimal? LENGTH { get; set; }
        [Column("WIDTH")]
        public virtual decimal? WIDTH { get; set; }
        [Column("GRT")]
        public virtual long? GRT { get; set; }
        [Column("DWT")]
        public virtual long? DWT { get; set; }
        [Column("DRAUGHT")]
        public virtual int? DRAUGHT { get; set; }
        [Column("YEAR_BUILT")]
        public virtual int? YEARBUILT { get; set; }
        [Column("ROT")]
        public virtual int? ROT { get; set; }
        [Column("TYPE_NAME")]
        public virtual string TYPENAME { get; set; }
        [Column("AIS_TYPE_SUMMARY")]
        public virtual string AISTYPESUMMARY { get; set; }
        [Column("DESTINATION")]
        public virtual string DESTINATION { get; set; }
        [Column("ETA")]
        public virtual DateTime? ETA { get; set; }
        [Column("CURRENT_PORT")]
        public virtual string CURRENTPORT { get; set; }
        [Column("LAST_PORT")]
        public virtual string LASTPORT { get; set; }
        [Column("LAST_PORT_TIME")]
        public virtual DateTime? LASTPORTTIME { get; set; }
        [Column("CURRENT_PORT_ID")]
        public virtual string CURRENTPORTID { get; set; }
        [Column("CURRENT_PORT_UNLOCODE")]
        public virtual string CURRENTPORTUNLOCODE { get; set; }
        [Column("CURRENT_PORT_COUNTRY")]
        public virtual string CURRENTPORTCOUNTRY { get; set; }
        [Column("LAST_PORT_ID")]
        public virtual string LASTPORTID { get; set; }
        [Column("LAST_PORT_UNLOCODE")]
        public virtual string LASTPORTUNLOCODE { get; set; }
        [Column("LAST_PORT_COUNTRY")]
        public virtual string LASTPORTCOUNTRY { get; set; }
        [Column("NEXT_PORT_ID")]
        public virtual long? NEXTPORTID { get; set; }
        [Column("NEXT_PORT_UNLOCODE")]
        public virtual string NEXTPORTUNLOCODE { get; set; }
        [Column("NEXT_PORT_NAME")]
        public virtual string NEXTPORTNAME { get; set; }
        [Column("NEXT_PORT_COUNTRY")]
        public virtual string NEXTPORTCOUNTRY { get; set; }
        [Column("ETA_CALC")]
        public virtual DateTime? ETACALC { get; set; }
        [Column("ETA_UPDATED")]
        public virtual DateTime? ETAUPDATED { get; set; }
        [Column("DISTANCE_TO_GO")]
        public virtual long? DISTANCETOGO { get; set; }
        [Column("DISTANCE_TRAVELLED")]
        public virtual long? DISTANCETRAVELLED { get; set; }
        [Column("AVG_SPEED")]
        public virtual decimal? AVGSPEED { get; set; }
        [Column("MAX_SPEED")]
        public virtual decimal? MAXSPEED { get; set; }
        [Column("TRACK_TYPE")]
        public virtual string TRACKTYPE { get; set; }
        [Column("TRACK_SOURCE")]
        public virtual string TRACKSOURCE { get; set; }
    }
}
