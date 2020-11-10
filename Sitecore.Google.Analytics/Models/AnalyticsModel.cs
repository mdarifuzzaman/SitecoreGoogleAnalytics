using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Google.Analytics.Models
{
    public class AnalyticsModel
    {
        public PersonalServiceAccountCred Credentials { get; set; }
        public string CredStr { get; set; }
        public string Url { get; set; }
        public string ViewId { get; set; }

        public string ApplicationName { get; set; }

        public string Title { get; set; }
        public string Dimension { get; set; }
        public string Metrics { get; set; }
        public int Period { get; set; }
        public string Alias { get; set; }
    }
}