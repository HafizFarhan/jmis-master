using Microsoft.AspNetCore.Mvc.Rendering;
using MTC.JMICS.Models.DB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JMICSAPP.Models
{
    public class DrawingViewModel
    {
        public SelectList ShapeTypeList { get; set; }
        public SelectList RadiusUnitList { get; set; }
        public Drawing Drawing { get; set; }
    }


    public partial class DrawingBaseModel
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public Properties Properties { get; set; }

    }

    public partial class Properties
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("fillColor")]
        public string FillColor { get; set; }
        [JsonProperty("strokeColor")]
        public string StrokeColor { get; set; }
    }


    public partial class DrawingModel : DrawingBaseModel
    {
        [JsonProperty("geometry")]
        public Geometry Geometry { get; set; }
    }

    public partial class PolygonDrawingModel : DrawingBaseModel
    {
        [JsonProperty("geometry")]
        public PolygonGeometry Geometry { get; set; }
    }

    public partial class Geometry
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("coordinates")]
        public List<List<decimal>> Coordinates { get; set; }
    }
    public partial class PolygonGeometry
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("coordinates")]
        public List<double[][]> Coordinates { get; set; }
    }


}