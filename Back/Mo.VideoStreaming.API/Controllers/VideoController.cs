using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;

namespace Mo.VideoStreaming.API.Controllers;

[ApiController]
[Route("[controller]")]
public class VideoController(BlobService blobService) : ControllerBase
{
    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        var containerClient = blobService.GetContainerClient();
        var blobClient = containerClient.GetBlobClient(file.FileName);

        await blobClient.UploadAsync(file.OpenReadStream(), new BlobHttpHeaders { ContentType = file.ContentType });

        return Ok(new { path = blobClient.Uri.ToString() });
    }
    
    [HttpGet("stream/{fileName}")]
    public async Task<IActionResult> StreamVideo(string fileName)
    {
        var containerClient = blobService.GetContainerClient();
        var blobClient = containerClient.GetBlobClient(fileName);

        if (await blobClient.ExistsAsync())
        {
            var download = await blobClient.DownloadStreamingAsync();

            Response.Headers.Append("Content-Disposition", $"inline; filename=\"{fileName}\"");
            return File(download.Value.Content, download.Value.Details.ContentType, enableRangeProcessing: true);
        }
    
        return NotFound("Video not found.");
    }
    
    [HttpGet("list")]
    public async Task<IActionResult> ListVideos()
    {
        var containerClient = blobService.GetContainerClient();
        var videoList = new List<string>();

        await foreach (var blobItem in containerClient.GetBlobsAsync())
        {
            videoList.Add(blobItem.Name);
        }

        return Ok(videoList);
    }
}