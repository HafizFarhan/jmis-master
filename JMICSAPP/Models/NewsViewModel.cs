using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MTC.JMICS.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JMICSAPP.Models
{
    public class NewsViewModel
    {
        public SelectList SubscriberList { get; set; }
        public SelectList NewsTypeList { get; set; }

        public News News { get; set; }

        [BindProperty]
        public int[] SelectedReportedTo { get; set; }
    }
}
