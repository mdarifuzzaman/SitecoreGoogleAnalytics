using System.Collections.Generic;

namespace Sitecore.Google.Analytics.Models
{
    public class AnalyticsResponse
    {
        public AnalyticsResponse()
        {
            Response = new List<AnalyticsReportResponse>();
        }

        public AnalyticsModel Model { get; set; }
        public List<AnalyticsReportResponse> Response { get; set; }
    }

    public class AnalyticsChartResponse: AnalyticsResponse
    {
        public AnalyticsChartResponse()
        {
        }
        public string Chart { get; set; }
        public Dictionary<string, List<DataPoint>> DataPoints { get; set; }

        public string DataObject { get; set; }
    }
}