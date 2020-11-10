using Google.Apis.AnalyticsReporting.v4;
using Google.Apis.AnalyticsReporting.v4.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Sitecore.Google.Analytics.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sitecore.Google.Analytics
{
    public class ReportingManager
    {
        private static AnalyticsReportingService GetAnalyticsReportingServiceInstance(string jsonCredential)
        {
            string[] scopes = { AnalyticsReportingService.Scope.AnalyticsReadonly }; //Read-only access to Google Analytics
            GoogleCredential credential;
            credential = GoogleCredential.FromJson(jsonCredential).CreateScoped(scopes);
            // Create the  Analytics service.
            return new AnalyticsReportingService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "GA Reporting data extraction example",
            });
        }

        /// <summary>
        /// Fetches all required reports from Google Analytics
        /// </summary>
        /// <param name="reportRequests"></param>
        /// <returns></returns>
        private static GetReportsResponse GetReport(GetReportsRequest getReportsRequest, string jsonCredential)
        {
            var analyticsService = GetAnalyticsReportingServiceInstance(jsonCredential);
            return analyticsService.Reports.BatchGet(getReportsRequest).Execute();
        }

        public static List<AnalyticsReportResponse> GetAnalyticsResponse(AnalyticsModel model, string jsonCredential)
        {
            try
            {
                #region Prepare Report Request object 
                // Create the DateRange object. Here we want data from last week.
                var dateRange = new DateRange
                {
                    StartDate = DateTime.UtcNow.AddDays(model.Period).ToString("yyyy-MM-dd"),
                    EndDate = DateTime.UtcNow.ToString("yyyy-MM-dd")
                };
                // Create the Metrics and dimensions object.
                var metrics = new List<Metric> { new Metric { Expression = model.Metrics, Alias = model.Alias } };
                var dimensions = new List<Dimension> { new Dimension { Name = model.Dimension } };

                //Get required View Id from configuration
                var ViewId = model.ViewId;

                // Create the Request object.
                var reportRequest = new ReportRequest
                {
                    DateRanges = new List<DateRange> { dateRange },
                    Metrics = metrics,
                    Dimensions = dimensions,
                    ViewId = ViewId
                };
                var getReportsRequest = new GetReportsRequest();
                getReportsRequest.ReportRequests = new List<ReportRequest> { reportRequest };
                #endregion

                //Invoke Google Analytics API call and get report
                var response = GetReport(getReportsRequest, jsonCredential);

                //Print report data to console
                return MakeReport(response);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }
        }

        private static List<AnalyticsReportResponse> MakeReport(GetReportsResponse response)
        {
            var reportResponse = new List<AnalyticsReportResponse>();
            foreach (var report in response.Reports)
            {
                var rows = report.Data.Rows;
                ColumnHeader header = report.ColumnHeader;
                var dimensionHeaders = header.Dimensions;
                var metricHeaders = header.MetricHeader.MetricHeaderEntries;
                if (!rows.Any())
                {
                    return reportResponse;
                }
                else
                {
                    foreach (var row in rows)
                    {
                        var dimensions = row.Dimensions;
                        var metrics = row.Metrics;

                        reportResponse.Add(new AnalyticsReportResponse(dimensions, metrics));
                        
                    }
                }
            }
            return reportResponse;
        }
    }
}