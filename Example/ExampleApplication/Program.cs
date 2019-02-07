using System;
using System.Diagnostics;
using System.IO;

namespace ExampleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            const string connectionStringAzureStorageAccount = "DefaultEndpointsProtocol=https;AccountName=masterdatapoc;AccountKey=TM4WDE4Bfd3Dmo6JWvKXpNBAr2YEtg2iHZ5EKYBvAedTp9Z3GgTxwGNbVybK7+q4BNMGSkrCX+Vm48pUhUlEwg==;EndpointSuffix=core.windows.net";
            var secureBlobStorageConnector = new SecureBlobStorageConnector.SecureBlobStorageConnector(connectionStringAzureStorageAccount);
            
            // Upload blob:
            var localPath = Environment.CurrentDirectory + "/new.jpg";
            using (var fileStream = File.Open(localPath, FileMode.Open, FileAccess.Read))
            {
                secureBlobStorageConnector.UploadAsync("car.png", "mediafiles", fileStream).GetAwaiter().GetResult();
            }
            Console.WriteLine($"File {localPath} uploaded!");
            
            // Getting blob:
            var stream = secureBlobStorageConnector.GetAsync("car.png", "mediafiles").GetAwaiter().GetResult();
            Console.WriteLine("File car.png downloaded, saving it to local disk...");

            // Write the file to disk
            var destinationPath = Environment.CurrentDirectory + "/car.png";
            using (var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.ReadWrite))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fileStream);
                stream.Close();
            }

            Console.WriteLine($"The downloaded image is now here: {destinationPath}");
            Console.ReadLine();
        }
    }
}