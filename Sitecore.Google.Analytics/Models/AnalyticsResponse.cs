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
}