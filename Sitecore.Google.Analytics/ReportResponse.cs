using Google.Apis.AnalyticsReporting.v4.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Google.Analytics
{
    public class AnalyticsReportResponse
    {
        public AnalyticsReportResponse(IList<string> dimensions, IList<DateRangeValues> metrics)
        {
            Dimension = dimensions;
            Metrics = metrics;
        }
        public IList<string> Dimension { get; }

        public IList<DateRangeValues> Metrics { get; }
    }
}