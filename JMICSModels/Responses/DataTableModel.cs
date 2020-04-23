using System;
using System.Collections.Generic;
using System.Text;

namespace MTC.JMICS.Models.Responses
{
    public class DataTableModel
    {
        public Meta Meta { get; set; }
        public object Data { get; set; }
    }
    public class Meta
    {
        public long page { get; set; }
        public long pages { get; set; }
        public long perpage { get; set; }
        public long total { get; set; }
        public string sort { get; set; }
        public string field { get; set; }

        public string query { get; set; }
    }
}
