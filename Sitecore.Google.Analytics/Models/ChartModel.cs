using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Google.Analytics.Models
{
    public class ChartModel
    {
        public ChartModel()
        {
            dataPoints = new List<DataPoint>();
        }
        public string type { get; set; }
        public string name { get; set; }

        public string axisYType { get; set; }

        public bool showInLegend { get; set; }
        public List<DataPoint> dataPoints { get; set; }
    }
}