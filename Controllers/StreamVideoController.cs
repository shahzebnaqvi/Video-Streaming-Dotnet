using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;


    [ApiController]
    [Route("[controller]")]
    public class VideoController : ControllerBase
    {
        private const string VideoPath = "testvideo.mp4";
        private const int ChunkSize = 10;

        [HttpGet]
        public FileStreamResult Get()
        {
        //    var rangeHeader = Request.Headers["Range"];
        // if (string.IsNullOrEmpty(rangeHeader))
        // {   HttpContext.Response.StatusCode = 206;
        //     return new FileStreamResult(new FileStream(VideoPath, FileMode.Open, FileAccess.Read, FileShare.Read), "video/octet-stream");
        // }
        var rangeHeader = "bytes=0-499";
        var fileInfo = new FileInfo(VideoPath);
        var fileSize = fileInfo.Length;

        var range = rangeHeader.ToString().Replace("bytes=", "");
        var rangeArray = range.Split('-');

        long start = 0, end = 0;
        if (rangeArray.Length > 0 && long.TryParse(rangeArray[0], out start))
        {
            if (rangeArray.Length > 1 && long.TryParse(rangeArray[1], out end))
            {
                end = Math.Min(end, fileSize - 1);
            }
            else
            {
                end = fileSize - 1;
            }
        }

        if (end > fileSize - 1)
        {
            end = fileSize - 1;
        }

        var contentLength = end - start + 1;
        var fileStream = new FileStream(VideoPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var bufferedStream = new BufferedStream(fileStream, ChunkSize);
        var response = new FileStreamResult(bufferedStream, "video/octet-stream")
        {
            EnableRangeProcessing = true,
            LastModified = fileInfo.LastWriteTime,
            EntityTag = new Microsoft.Net.Http.Headers.EntityTagHeaderValue("\"" + fileInfo.LastWriteTime.Ticks.ToString("x") + "\"")
        };
        // var response = new FileStreamResult(new FileStream(VideoPath, FileMode.Open, FileAccess.Read, FileShare.Read), "video/octet-stream")
        // {
        //     EnableRangeProcessing = true,
        //     LastModified = fileInfo.LastWriteTime,
        //     EntityTag = new Microsoft.Net.Http.Headers.EntityTagHeaderValue("\"" + fileInfo.LastWriteTime.Ticks.ToString("x") + "\"")
        // };

        HttpContext.Response.StatusCode = 206;
        Response.Headers.Add("Accept-Ranges", "bytes");
        Response.Headers.Add("Content-Length", contentLength.ToString());
        Response.Headers.Add("Content-Range", $"bytes {start}-{end}/{fileSize}");
        return response;
    }
    }
