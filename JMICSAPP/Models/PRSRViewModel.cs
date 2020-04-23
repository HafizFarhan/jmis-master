using MTC.JMICS.Models.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JMICSAPP.Models
{
    public class PRSRViewModel
    {

        public int Id { get; set; }
        public string COIId { get; set; }
        public string PRNumber { get; set; }
        public string Title { get; set; }
        public DateTime? ReportingDatetime { get; set; }
        public int? SubscriberId { get; set; }
        public string ActionAddressee { get; set; }
        public string InformationAddressee { get; set; }
        public int? COITypeId { get; set; }
        public int? NatureOfThreatId { get; set; }
        public DateTime? LastObservationDatetime { get; set; }
        public string Remarks { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Course { get; set; }
        public decimal? Heading { get; set; }
        public decimal? Speed { get; set; }
        public string MMSI { get; set; }
        public bool? IsDropped { get; set; }
        public bool? IsLost { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public virtual string SubscriberName { get; set; }
        public virtual string COITypeName { get; set; }
        public virtual string ThreatName { get; set; }
        public List<SubsequentReportView> SRViewList { get; set; }
    }
}
