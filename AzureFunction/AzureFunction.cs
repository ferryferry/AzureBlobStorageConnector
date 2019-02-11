using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Runtime.CompilerServices;
using MimeSharp;


namespace SecureBlobStorage.Function
{
    public static class AzureFunction
    {
        private const string _connectionString = "CONNECTIONSTRINGHERE";

        [FunctionName("AzureFunction")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Request is a POST request
            if (req.Method.Equals(HttpMethods.Post))
            {
                log.LogInformation("POST function triggered.");
                return await HandlePOST(req, log);
            }

            // Request is a GET request
            log.LogInformation("GET function triggered.");
            return await HandleGET(req);
        }

        private static async Task<IActionResult> HandlePOST(HttpRequest req, ILogger log)
        {
            var secureBlobStorageConnector = new SecureBlobStorageConnector.SecureBlobStorageConnector(_connectionString);

            try
            {
                if (req.Body.Length <= 0)
                {
                    return new BadRequestObjectResult("Provided file is empty!");
                }
            }
            catch (NotSupportedException)
            {
                return new BadRequestObjectResult("No file provided, or provided file is empty!");
            }

            var contentType = req.Headers["content-type"];
            if (string.IsNullOrEmpty(contentType))
            {
                return new BadRequestObjectResult("No content type provided in headers.");
            }

            var mime = new Mime();
            if (mime.Extension(contentType).Count == 0)
            {
                return new BadRequestObjectResult($"Specified content type {contentType} invalid!");
            }

            var extension = mime.Extension(contentType)[0];
            var fileName = $"{Guid.NewGuid().ToString()}.{extension}";
            var containerName = req.Query["containerName"];

            await secureBlobStorageConnector.UploadAsync(fileName, req.Query["containerName"], req.Body);


            return new CreatedResult(fileName, null);
        }

        private static async Task<IActionResult> HandleGET(HttpRequest req)
        {
            var blobName = req.Query["blobName"];
            var containerName = req.Query["containerName"];

            if (string.IsNullOrEmpty(blobName) || string.IsNullOrEmpty(containerName))
            {
                return new BadRequestObjectResult("Blob name or containerName not specified!");
            }

            var secureBlobStorageConnector = new SecureBlobStorageConnector.SecureBlobStorageConnector(_connectionString);

            var blob = await secureBlobStorageConnector.GetAsync(blobName, containerName);
            if (blob == null || blob.Length <= 0)
            {
                return new BadRequestObjectResult("Blob is empty");
            }

            return new FileContentResult(blob.ToArray(), "application/octet-stream")
            {
                FileDownloadName = blobName
            };
        }
    }
}
