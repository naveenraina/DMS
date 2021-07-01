using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DMS.Data;
using DMS.Service;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.IO;

namespace DMS.Controllers
{

    [Authorize(Roles="Admin,User")]
    public class VideoController : Controller
    {
        [HttpGet("video/documents/{name}")]
        public async Task<FileStreamResult> IndexAsync(string name)
        {
            var stream = await GetVideoByName(name);
            return new FileStreamResult(stream, "video/mp4");
        }
        
        private async Task<Stream> GetVideoByName(string name)
        {
            HttpClient _client;
            _client = new HttpClient();
            var urlBlob = string.Empty;            
            urlBlob = "http://localhost:2711/Documents/" + name;
            return await _client.GetStreamAsync(urlBlob);
        }

    }
}