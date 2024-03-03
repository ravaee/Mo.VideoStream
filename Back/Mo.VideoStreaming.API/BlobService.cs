using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;

namespace Mo.VideoStreaming.API;

public class BlobService(BlobServiceClient blobServiceClient)
{
    public BlobContainerClient GetContainerClient()
    {
        return blobServiceClient.GetBlobContainerClient("videos");
    }
}
