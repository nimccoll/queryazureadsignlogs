//===============================================================================
// Microsoft FastTrack for Azure
// Query Azure AD Sign In Logs Samples
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================
using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.Monitor.Query;
using Azure.Monitor.Query.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using QueryAzureADSignInLogs.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace QueryAzureADSignInLogs.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly string _graphQuery = "v1.0/auditLogs/signIns";

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = new HttpClient();
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult QueryViaLogAnalytics()
        {
            return View();
        }

        [HttpPost]
        [ActionName("QueryViaLogAnalytics")]
        public async Task<IActionResult> QueryViaLogAnalyticsPost()
        {
            List<SignInLog> model = new List<SignInLog>();
            int fromDays = Convert.ToInt32(Request.Form["numberOfDays"]);
            string userPrincipalName = Request.Form["userPrincipalName"];

            // Build the Kusto query
            StringBuilder sbQuery = new StringBuilder("SigninLogs");
            if (!string.IsNullOrEmpty(userPrincipalName)) sbQuery.Append($" | where UserPrincipalName == \"{userPrincipalName}\"");
            sbQuery.Append(" | order by TimeGenerated desc");

            // Authenticate
            DefaultAzureCredential defaultAzureCredential = new DefaultAzureCredential();
            LogsQueryClient logsQueryClient = new LogsQueryClient(defaultAzureCredential);

            // Execute query
            string workspaceId = _configuration.GetValue<string>("LogAnalyticsWorkspaceId");
            Response<LogsQueryResult> response = await logsQueryClient.QueryWorkspaceAsync(
                workspaceId,
                sbQuery.ToString(),
                new QueryTimeRange(TimeSpan.FromDays(fromDays)));

            // Process results
            LogsTable table = response.Value.Table;

            foreach (var row in table.Rows)
            {
                SignInLog signInLog = new SignInLog();
                signInLog.userDisplayName = row["UserDisplayName"].ToString();
                signInLog.userPrincipalName = row["UserPrincipalName"].ToString();
                signInLog.createdDateTime = Convert.ToDateTime(row["CreatedDateTime"].ToString());
                signInLog.status = JsonConvert.DeserializeObject<Status>(row["Status"].ToString());
                model.Add(signInLog);
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult QueryViaStorage()
        {
            return View();
        }

        [HttpPost]
        [ActionName("QueryViaStorage")]
        public IActionResult QueryViaStoragePost()
        {
            List<SignInLog> model = new List<SignInLog>();
            int fromDays = Convert.ToInt32(Request.Form["numberOfDays"]);
            string userPrincipalName = Request.Form["userPrincipalName"];

            // Determine which blobs to scan
            BlobServiceClient blobServiceClient = new BlobServiceClient(_configuration.GetValue<string>("Storage:ConnectionString"));
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_configuration.GetValue<string>("Storage:ContainerName"));
            if (containerClient.Exists())
            {
                DateTime startDate = DateTime.UtcNow.AddDays(fromDays * -1);
                for (int i = 0; i <= fromDays; i++)
                {
                    string startYear = startDate.Year.ToString();
                    string startMonth = startDate.Month.ToString("00");
                    string startDay = startDate.Day.ToString("00");
                    // Compute the blob prefix based on the dates we wish to retrieve data for
                    // Format of blob prefix: tenantId=57d7cf4a-c3b2-4f35-9547-2f82d9010a20/y=2023/m=01/d=10
                    string blobPrefix = $"tenantId={Environment.GetEnvironmentVariable("AZURE_TENANT_ID")}/y={startYear}/m={startMonth}/d={startDay}";
                    List<BlobItem> blobs = containerClient.GetBlobs(prefix: blobPrefix).ToList();
                    foreach (BlobItem blob in blobs)
                    {
                        BlobClient blobClient = containerClient.GetBlobClient(blob.Name);
                        if (blobClient.Exists())
                        {
                            BlobDownloadInfo download = blobClient.Download();
                            string signInData = string.Empty;
                            using (StreamReader reader = new StreamReader(download.Content))
                            {
                                signInData = reader.ReadToEnd();
                            }
                            // Parse the JSON in the blob
                            string[] signInLogs = signInData.Split("\n", StringSplitOptions.RemoveEmptyEntries);
                            foreach (string log in signInLogs)
                            {
                                dynamic jToken = JsonConvert.DeserializeObject<dynamic>(log);
                                SignInLog signInLog = JsonConvert.DeserializeObject<SignInLog>(jToken.properties.ToString());
                                // If a user principal name was specified, only include sign in entries for that account
                                if (string.IsNullOrEmpty(userPrincipalName)
                                    || userPrincipalName == signInLog.userPrincipalName)
                                {
                                    model.Add(signInLog);
                                }
                            }
                        }
                    }
                    startDate = startDate.AddDays(1); // Find blobs for the next day to be included
                }
            }

            return View(model.OrderByDescending(m => m.createdDateTime).ToList());
        }

        [HttpGet]
        public IActionResult QueryViaRestAPI()
        {
            return View();
        }

        [HttpPost]
        [ActionName("QueryViaRestAPI")]
        public async Task<IActionResult> QueryViaRestAPIPost()
        {
            int fromDays = Convert.ToInt32(Request.Form["numberOfDays"]);
            string userPrincipalName = Request.Form["userPrincipalName"];

            // Build the OData query
            DateTime fromDate = DateTime.UtcNow.AddDays(fromDays * -1);
            string dateTimeFilter = fromDate.ToString("yyyy-MM-ddTHH:mm:ssZ");
            StringBuilder sbQuery = new StringBuilder(_graphQuery);
            sbQuery.Append($"?$filter=createdDateTime gt {dateTimeFilter}");
            if (!string.IsNullOrEmpty(userPrincipalName))
            {
                sbQuery.Append($" and userPrincipalName eq '{userPrincipalName}'");
            }
            sbQuery.Append("&$orderby=createdDateTime desc");

            // Authenticate
            DefaultAzureCredential defaultAzureCredential = new DefaultAzureCredential();
            AccessToken accessToken = await defaultAzureCredential.GetTokenAsync(new TokenRequestContext(new string[] { "https://graph.microsoft.com/.default" }));
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken.Token);

            // Call the graph
            HttpResponseMessage response = await _httpClient.GetAsync($"{_configuration.GetValue<string>("API:APIBaseAddress")}{sbQuery.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                string results = await response.Content.ReadAsStringAsync();
                SignInLogs signInLogs = JsonConvert.DeserializeObject<SignInLogs>(results);
                return View(signInLogs.value);
            }
            else
            {
                string error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Call to the Authorization API failed with the following error {0}", error);
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
