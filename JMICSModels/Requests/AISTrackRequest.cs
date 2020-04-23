using MTC.JMICS.Models.DB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTC.JMICS.Models.Requests
{
    public class AISTrackRequest
    {
        [JsonProperty("TRACK_NUMBER", NullValueHandling = NullValueHandling.Ignore)]
        public long? TRACK_NUMBER { get; set; }

        [JsonProperty("IMO", NullValueHandling = NullValueHandling.Ignore)]
        public long? IMO { get; set; }

        [JsonProperty("SHIP_ID", NullValueHandling = NullValueHandling.Ignore)]
        public long? SHIPID { get; set; }

        [JsonProperty("LAT", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? LAT { get; set; }

        [JsonProperty("LON", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? LON { get; set; }

        [JsonProperty("SPEED", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? SPEED { get; set; }

        [JsonProperty("HEADING", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? HEADING { get; set; }

        [JsonProperty("COURSE", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? COURSE { get; set; }

        [JsonProperty("STATUS", NullValueHandling = NullValueHandling.Ignore)]
        public long? STATUS { get; set; }

        [JsonProperty("TIMESTAMP", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? TIMESTAMP { get; set; }

        [JsonProperty("DSRC", NullValueHandling = NullValueHandling.Ignore)]
        public string DSRC { get; set; }

        [JsonProperty("UTC_SECONDS", NullValueHandling = NullValueHandling.Ignore)]
        public long? UTCSECONDS { get; set; }

        [JsonProperty("SHIPNAME", NullValueHandling = NullValueHandling.Ignore)]
        public string SHIPNAME { get; set; }

        [JsonProperty("SHIPTYPE", NullValueHandling = NullValueHandling.Ignore)]
        public long? SHIPTYPE { get; set; }

        [JsonProperty("SHIPTYPENAME", NullValueHandling = NullValueHandling.Ignore)]
        public string SHIPTYPENAME { get; set; }

        [JsonProperty("CALLSIGN", NullValueHandling = NullValueHandling.Ignore)]
        public string CALLSIGN { get; set; }

        [JsonProperty("FLAG", NullValueHandling = NullValueHandling.Ignore)]
        public string FLAG { get; set; }

        [JsonProperty("LENGTH", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? LENGTH { get; set; }

        [JsonProperty("WIDTH", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? WIDTH { get; set; }

        [JsonProperty("GRT", NullValueHandling = NullValueHandling.Ignore)]       
        public long? GRT { get; set; }

        [JsonProperty("DWT", NullValueHandling = NullValueHandling.Ignore)] 
        public long? DWT { get; set; }

        [JsonProperty("DRAUGHT", NullValueHandling = NullValueHandling.Ignore)]       
        public long? DRAUGHT { get; set; }

        [JsonProperty("YEAR_BUILT", NullValueHandling = NullValueHandling.Ignore)]       
        public long? YEARBUILT { get; set; }

        [JsonProperty("ROT", NullValueHandling = NullValueHandling.Ignore)] 
        public long? ROT { get; set; }

        [JsonProperty("TYPE_NAME", NullValueHandling = NullValueHandling.Ignore)]
        public string TYPENAME { get; set; }

        [JsonProperty("AIS_TYPE_SUMMARY", NullValueHandling = NullValueHandling.Ignore)]
        public string AISTYPESUMMARY { get; set; } 

        [JsonProperty("DESTINATION", NullValueHandling = NullValueHandling.Ignore)]
        public string DESTINATION { get; set; }

        [JsonProperty("ETA", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? ETA { get; set; }

        [JsonProperty("CURRENT_PORT", NullValueHandling = NullValueHandling.Ignore)]
        public string CURRENTPORT { get; set; }

        [JsonProperty("LAST_PORT", NullValueHandling = NullValueHandling.Ignore)]
        public string LASTPORT { get; set; }

        [JsonProperty("LAST_PORT_TIME", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? LASTPORTTIME { get; set; }

        [JsonProperty("CURRENT_PORT_ID", NullValueHandling = NullValueHandling.Ignore)]
        public string CURRENTPORTID { get; set; }

        [JsonProperty("CURRENT_PORT_UNLOCODE", NullValueHandling = NullValueHandling.Ignore)]
        public string CURRENTPORTUNLOCODE { get; set; }

        [JsonProperty("CURRENT_PORT_COUNTRY", NullValueHandling = NullValueHandling.Ignore)]
        public string CURRENTPORTCOUNTRY { get; set; }

        [JsonProperty("LAST_PORT_ID", NullValueHandling = NullValueHandling.Ignore)]
        public long? LASTPORTID { get; set; }

        [JsonProperty("LAST_PORT_UNLOCODE", NullValueHandling = NullValueHandling.Ignore)]
        public string LASTPORTUNLOCODE { get; set; }

        [JsonProperty("LAST_PORT_COUNTRY", NullValueHandling = NullValueHandling.Ignore)]
        public string LASTPORTCOUNTRY { get; set; }

        [JsonProperty("NEXT_PORT_ID", NullValueHandling = NullValueHandling.Ignore)]     
        public long? NEXTPORTID { get; set; }

        [JsonProperty("NEXT_PORT_UNLOCODE", NullValueHandling = NullValueHandling.Ignore)]
        public string NEXTPORTUNLOCODE { get; set; }

        [JsonProperty("NEXT_PORT_NAME", NullValueHandling = NullValueHandling.Ignore)]
        public string NEXTPORTNAME { get; set; }

        [JsonProperty("NEXT_PORT_COUNTRY", NullValueHandling = NullValueHandling.Ignore)]
        public string NEXTPORTCOUNTRY { get; set; }

        [JsonProperty("ETA_CALC", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? ETACALC { get; set; }

        [JsonProperty("ETA_UPDATED", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? ETAUPDATED { get; set; }

        [JsonProperty("DISTANCE_TO_GO", NullValueHandling = NullValueHandling.Ignore)]
        public long? DISTANCETOGO { get; set; }

        [JsonProperty("DISTANCE_TRAVELLED", NullValueHandling = NullValueHandling.Ignore)]
        public long? DISTANCETRAVELLED { get; set; }

        [JsonProperty("AVG_SPEED", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? AVGSPEED { get; set; }

        [JsonProperty("MAX_SPEED", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? MAXSPEED { get; set; }

        [JsonProperty("TRACKTYPE", NullValueHandling = NullValueHandling.Ignore)]
        public string TRACKTYPE { get; set; }

        [JsonProperty("TRACKSOURCE", NullValueHandling = NullValueHandling.Ignore)]
        public string TRACKSOURCE { get; set; }
        [JsonProperty("SOURCE_STATION", NullValueHandling = NullValueHandling.Ignore)]
        public string SOURCE_STATION { get; set; }
        [JsonProperty("TRACK_LABEL", NullValueHandling = NullValueHandling.Ignore)]
        public string TRACK_LABEL { get; set; }
        [JsonProperty("BEARING", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? BEARING { get; set; }
        [JsonProperty("DEPTH", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? DEPTH { get; set; }
        [JsonProperty("AFFILIATION", NullValueHandling = NullValueHandling.Ignore)]
        public short? AFFILIATION { get; set; }
        [JsonProperty("CATEGORY", NullValueHandling = NullValueHandling.Ignore)]
        public short? CATEGORY { get; set; }
        [JsonProperty("SUB_CAT", NullValueHandling = NullValueHandling.Ignore)]
        public short? SUB_CAT { get; set; }
        [JsonProperty("RANGE", NullValueHandling = NullValueHandling.Ignore)]
        public float RANGE { get; set; }


        [JsonProperty("IsLloydInfoPresent", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsLloydInfoPresent { get; set; }

        [JsonProperty("LlydDetail", NullValueHandling = NullValueHandling.Ignore)]
        public Ship LloydInfo { get; set; }

    }
}
