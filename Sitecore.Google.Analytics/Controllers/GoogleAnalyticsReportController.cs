
using Google.Apis.AnalyticsReporting.v4.Data;
using Newtonsoft.Json;
using Sitecore.Google.Analytics.Models;
using Sitecore.Mvc.Controllers;
using Sitecore.Mvc.Presentation;
using System.Collections.Generic;
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

        // GET: GoogleAnalyticsReport
        public ActionResult Chart()
        {
            var analyticsModel = GetModel();
            var chart = GetChartName();

            if (string.IsNullOrEmpty(chart))
            {
                return View();
            }

            var response = ReportingManager.GetAnalyticsResponse(analyticsModel, analyticsModel.CredStr);
            var analyticsResponse = new AnalyticsChartResponse
            {
                Model = analyticsModel,
                Response = response,
                Chart = chart
            };

            Dictionary<string, List<DataPoint>> dataDict = new Dictionary<string, List<DataPoint>>();
            foreach (var responseData in analyticsResponse.Response)
            {
                List<DataPoint> dataPoints1 = new List<DataPoint>();
                foreach (var metric in responseData.Metrics)
                {
                    DateRangeValues values = metric;
                    for (int k = 0; k < values.Values.Count; k++)
                    {
                        dataPoints1.Add(new DataPoint(chart, double.Parse(values.Values[k].ToString())));
                    }
                }
                dataDict.Add(responseData.Dimension[0], dataPoints1);
            }

            analyticsResponse.DataPoints = dataDict;
            List<ChartModel> model = new List<ChartModel>();
            foreach (var key in analyticsResponse.DataPoints.Keys) {
                model.Add(new ChartModel() { dataPoints = analyticsResponse.DataPoints[key], showInLegend = true, name = key, type = chart, axisYType="secondary" });
            }

            analyticsResponse.DataObject = JsonConvert.SerializeObject(model);

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

        private string GetChartName()
        {
            var item = RenderingContext.Current.ContextItem;

            Data.Items.Item chartType = GetSelectedItemFromDroplistField(item, "GoogleAnalytics_Charts");
            var chartValue = "";
            if (chartType != null)
            {
                chartValue = chartType.Fields["Value"].GetValue(true);
            }
            return chartValue;
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