using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace IMS.Infrastructure.Services
{
    public interface IBlobStorageClient
    {
        Uri GeneratePresignedUrl(string blobName, TimeSpan expiryDuration);
    }

    public class BlobStorageClient : IBlobStorageClient
    {
        private readonly string connectionString;
        private readonly string containerName;

        public BlobStorageClient(string connectionString, string containerName)
        {
            this.connectionString = connectionString;
            this.containerName = containerName;
        }

        public Uri GeneratePresignedUrl(string blobName, TimeSpan expiryDuration)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(
                containerName
            );
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            // Specify the permissions for the SAS
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = containerName,
                BlobName = blobName,
                Resource = "b", // Indicates the SAS is for a blob
                ExpiresOn = DateTimeOffset.UtcNow.Add(expiryDuration),
            };

            // Set the permissions for uploading
            sasBuilder.SetPermissions(BlobSasPermissions.Write);

            // Generate the SAS token
            Uri sasUri = blobClient.GenerateSasUri(sasBuilder);

            return sasUri;
        }
    }
}
