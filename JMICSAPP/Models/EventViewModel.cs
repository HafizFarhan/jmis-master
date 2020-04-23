using MTC.JMICS.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JMICSAPP.Models
{
    public class EventViewModel
    {
        public EventType EventType { get; set; }
        //public List<Event> EventList { get; set; }
        public Event Event { get; set; }
    }
}
