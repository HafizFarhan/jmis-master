using Microsoft.AspNetCore.Mvc.Rendering;
using MTC.JMICS.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JMICSAPP.Models
{
    public class COIListViewModel
    {
        public SelectList COIStatuses { get; set; }
        public List<COIView> COIList { get; set; }
    }
}
