using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;

namespace SecureBlobStorageConnector
{
    public class SecureBlobStorageConnector
    {
        private readonly CloudStorageAccount _cloudStorageAccount;

        /// <summary>
        /// Creates a Secure blob storage connector
        /// Secure means it takes advantage of the use of a SAS (Secured Access Signature).
        /// The SAS is valid for only 5 minutes and will be requested for every call to Get() or Upload().
        /// </summary>
        /// <param name="azureStorageAccountConnectionString">Connection string (Can be found in Azure Portal > Storage Accounts > [Your storage account] > Access Keys.</param>
        /// <exception cref="FormatException">Can be thrown when azureStorageAccountConnectionString is invalid.</exception>
        /// <exception cref="ArgumentException">Can be thrown when azureStorageAccountConnectionString is invalid.</exception>
        public SecureBlobStorageConnector(string azureStorageAccountConnectionString)
        {
            _cloudStorageAccount = CloudStorageAccount.Parse(azureStorageAccountConnectionString);
        }

        /// <summary>
        /// Gets one item from the Azure Storage account 
        /// </summary>
        /// <param name="blobName">Name of the blob on Azure</param>
        /// <param name="containerName">Name of the blob container in Azure. If nested in directories use: /container-name/dir1/dir2</param>
        /// <returns></returns>
        public async Task<MemoryStream> GetAsync(string blobName, string containerName)
        {
            // Set policies
            var policy = new SharedAccessAccountPolicy
            {
                Permissions = SharedAccessAccountPermissions.Read | SharedAccessAccountPermissions.List,
                Services = SharedAccessAccountServices.Blob | SharedAccessAccountServices.File,
                ResourceTypes = SharedAccessAccountResourceTypes.Service,
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(5),
                Protocols = SharedAccessProtocol.HttpsOnly
            };

            // Set correct policies
            _cloudStorageAccount.GetSharedAccessSignature(policy);
            
            // Create blobClient
            var blobClient =  _cloudStorageAccount.CreateCloudBlobClient();

            // Get reference from the container
            var container = blobClient.GetContainerReference(containerName);

            // Name of blob
            var blockBlob = container.GetBlockBlobReference(blobName);

            // Download the blob
            var memoryStream = new MemoryStream();
            await blockBlob.DownloadToStreamAsync(memoryStream);

            return memoryStream;
        }
        
        public Task UploadAsync(string blobName, string containerName, Stream stream)
        {
            // Set policies
            var policy = new SharedAccessAccountPolicy
            {
                Permissions = SharedAccessAccountPermissions.Read | SharedAccessAccountPermissions.List |
                              SharedAccessAccountPermissions.Add,
                Services = SharedAccessAccountServices.Blob | SharedAccessAccountServices.File,
                ResourceTypes = SharedAccessAccountResourceTypes.Service,
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(5),
                Protocols = SharedAccessProtocol.HttpsOnly
            };

            // Set correct policies
            _cloudStorageAccount.GetSharedAccessSignature(policy);
            
            // Create blobClient
            var blobClient =  _cloudStorageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference(containerName);
            var cloudBlockBlob = container.GetBlockBlobReference(blobName);
            return cloudBlockBlob.UploadFromStreamAsync(stream);
        }
    }
}