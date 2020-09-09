// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using FunctionAppBlobStorage.Models;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FunctionAppBlobStorage
{
    public static class FunctionCreateBlob
    {
        [FunctionName("FunctionCreateBlob")]
        public static void Run([EventGridTrigger] EventGridEvent eventGridEvent, ILogger log)
        {
            try
            {
                log.LogInformation(eventGridEvent.Data.ToString());
                Blob blobData = GetInputData(eventGridEvent);
                if (string.IsNullOrEmpty(blobData?.RequestId))
                    log.LogInformation(blobData.RequestId ?? "No data");
                else
                {
                    log.LogInformation("CallFileEndPoint");
                    var called = CallFileEndPoint(blobData).Result;
                    log.LogInformation(called ? "No data" : $"Saved {blobData.RequestId}");
                }
            }
            catch (Exception e)
            {
                log.LogInformation($"Exception {e.Message}");
            }
        }

        #region Auxiliar Methods
        private static Blob GetInputData(EventGridEvent eventGridEvent)
        {
            string eventGridData = eventGridEvent.Data.ToString();
            return JsonConvert.DeserializeObject<Blob>(eventGridData);
        }
        private static async Task<bool> CallFileEndPoint(Blob blob)
        {
            bool result = false;
            using (var httpClient = new HttpClient())
            {
                var notificationData = JsonConvert.SerializeObject(blob);
                var notificationContent = new StringContent(notificationData, UnicodeEncoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("https://demosapicmejia.azurewebsites.net/api/Files", notificationContent);

                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();

                result = response.IsSuccessStatusCode;
            }
            return result;
        }
        #endregion
    }
}
