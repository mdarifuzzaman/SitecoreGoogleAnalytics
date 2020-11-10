
using Newtonsoft.Json;
using Sitecore.Google.Analytics.Models;
using Sitecore.Mvc.Controllers;
using Sitecore.Mvc.Presentation;
using System.Web.Mvc;

namespace Sitecore.Google.Analytics.Controllers
{
    public class GoogleAnalyticsReportController : Controller
    {
        // GET: GoogleAnalyticsReport
        public ActionResult Index()
        {
            var analyticsModel = GetModel();
            var response = ReportingManager.GetAnalyticsResponse(analyticsModel, analyticsModel.CredStr);
            var analyticsResponse = new AnalyticsResponse
            {
                Model = analyticsModel,
                Response = response
            };

            return View(analyticsResponse);
        }

        private Data.Items.Item GetSelectedItemFromDroplistField(Data.Items.Item item, string fieldName)
        {
            Data.Fields.Field field = item.Fields[fieldName];
            if (field == null || string.IsNullOrEmpty(field.Value))
            {
                return null;
            }

            var fieldSource = field.Source ?? string.Empty;
            var selectedItemPath = fieldSource.TrimEnd('/') + "/" + field.Value;
            return item.Database.GetItem(selectedItemPath);
        }

        private AnalyticsModel GetModel()
        {

            var item = RenderingContext.Current.ContextItem;
            var credValue = item.Fields["GoogleAnalytics_Credential_File"].GetValue(true);
            var viewId = item.Fields["GoogleAnalytics_Analytics_ViewId"].GetValue(true);
            var applicationName = item.Fields["GoogleAnalytics_App_Name"].GetValue(true);
            var title = item.Fields["GoogleAnalytics_Title"].GetValue(true);

            Data.Items.Item referencedDimensionItem = GetSelectedItemFromDroplistField(item, "GoogleAnalytics_Dimension");
            var dimension = "";
            if (referencedDimensionItem != null)
            {
                dimension = referencedDimensionItem.Fields["Value"].GetValue(true);
            }

            Data.Items.Item referencedMetricsItem = GetSelectedItemFromDroplistField(item, "GoogleAnalytics_Metrics");
            var metrics = "";
            var alias = "";
            if (referencedMetricsItem != null)
            {
                metrics = referencedMetricsItem.Fields["Expression"].GetValue(true);
                alias = referencedMetricsItem.Fields["Alias"].GetValue(true);
            }

            Data.Items.Item referencedPeriodItem = GetSelectedItemFromDroplistField(item, "GoogleAnalytics_ReportPeriod");
            var period = 0;
            if (referencedPeriodItem != null)
            {
                period = int.Parse(referencedPeriodItem.Fields["PeriodValue"].GetValue(true));
            }


            var credString = credValue;
            var credObject = JsonConvert.DeserializeObject<PersonalServiceAccountCred>(credValue);

            return new AnalyticsModel
            {
                ApplicationName = applicationName,
                CredStr = credString,
                Credentials = credObject,
                ViewId = viewId,
                Title = title,
                Dimension = dimension,
                Metrics = metrics,
                Alias = alias,
                Period = period
            };

        }
    }
}