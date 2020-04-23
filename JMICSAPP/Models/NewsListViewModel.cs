using Microsoft.AspNetCore.Mvc.Rendering;
using MTC.JMICS.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JMICSAPP.Models
{
    public class NewsListViewModel
    {
        public SelectList NewsStatuses { get; set; }
        public List<NewsView> NewsList { get; set; }
    }
}
