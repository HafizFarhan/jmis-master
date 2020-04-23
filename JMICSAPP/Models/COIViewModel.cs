using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MTC.JMICS.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JMICSAPP.Models
{
    public class COIViewModel
    {
        public SelectList SubscriberList { get; set; }
        public SelectList COITypeList { get; set; }
        public SelectList NatureOfThreatList { get; set; }
        public COI COI { get; set; }

        [BindProperty]
        public int[] SelectedAddressTo { get; set; }
    }
}
