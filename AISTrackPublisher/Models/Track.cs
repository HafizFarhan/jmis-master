using System;
using System.Collections.Generic;
using System.Text;

namespace MTC.JMIS.AISTrackPublisher.Models
{
    public class Track
    {
        public long? TRACK_NUMBER { get; set; }
        public long? IMO { get; set; }
        public long? SHIP_ID { get; set; }
        public decimal? LAT { get; set; }
        public decimal? LON { get; set; }
        public decimal? SPEED { get; set; }
        public decimal? HEADING { get; set; }
        public decimal? COURSE { get; set; }
        public long? STATUS { get; set; }
        public DateTime? TIMESTAMP { get; set; }
        public string DSRC { get; set; }
        public long? UTC_SECONDS { get; set; }
        public string SHIPNAME { get; set; }       
        public long? SHIPTYPE { get; set; }
        public string SHIPTYPENAME { get; set; }
        public string CALLSIGN { get; set; }
        public string FLAG { get; set; }
        public decimal? LENGTH { get; set; }
        public decimal? WIDTH { get; set; }
        public long? GRT { get; set; }
        public long? DWT { get; set; }
        public long? DRAUGHT { get; set; }
        public long? YEAR_BUILT { get; set; }
        public long? ROT { get; set; }
        public string TYPE_NAME { get; set; }
        public string AIS_TYPE_SUMMARY { get; set; }
        public string DESTINATION { get; set; }
        public DateTime? ETA { get; set; }
        public string CURRENT_PORT { get; set; }
        public string LAST_PORT { get; set; }
        public DateTime? LAST_PORT_TIME { get; set; }
        public string CURRENT_PORT_ID { get; set; }
        public string CURRENT_PORT_UNLOCODE { get; set; }
        public string CURRENT_PORT_COUNTRY { get; set; }
        public long? LAST_PORT_ID { get; set; }
        public string LAST_PORT_UNLOCODE { get; set; }
        public string LAST_PORT_COUNTRY { get; set; }
        public long? NEXT_PORT_ID { get; set; }
        public string NEXT_PORT_UNLOCODE { get; set; }
        public string NEXT_PORT_NAME { get; set; }
        public string NEXT_PORT_COUNTRY { get; set; }
        public DateTime? ETA_CALC { get; set; }
        public DateTime? ETA_UPDATED { get; set; }
        public long? DISTANCE_TO_GO { get; set; }
        public long? DISTANCE_TRAVELLED { get; set; }
        public decimal? AVG_SPEED { get; set; }
        public decimal? MAX_SPEED { get; set; }
        public string TRACKTYPE { get; set; }
        public string TRACKSOURCE { get; set; }
        public string SOURCE_STATION { get; set; }
        public string TRACK_LABEL { get; set; }
        public decimal? BEARING { get; set; }
        public decimal? DEPTH { get; set; }
        public short? AFFILIATION { get; set; }
        public short? CATEGORY { get; set; }
        public short? SUB_CAT { get; set; }
        public float RANGE { get; set; }
    }
}
