using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace SecureBlobStorage.UploadFunction
{
    public static class UploadFunction
    {
        [FunctionName("UploadFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var blobName = req.Query["blobName"];
            var containerName = req.Query["containerName"];

            if (string.IsNullOrEmpty(blobName) || string.IsNullOrEmpty(containerName))
            {
                return new BadRequestObjectResult("Blob name or containerName not specified!");
            }

            const string connectionStringAzureStorageAccount = "CONNECTIONSTRINGHERE";
            var secureBlobStorageConnector = new SecureBlobStorageConnector.SecureBlobStorageConnector(connectionStringAzureStorageAccount);

            var blob = await secureBlobStorageConnector.GetAsync(blobName, containerName);
            if (blob == null || blob.Length <= 0)
            {
                return new BadRequestObjectResult("Blob is empty");
            }
            
            return new FileContentResult(blob.ToArray(), "application/octet-stream"){
                FileDownloadName = blobName
            };
        }
    }
}
