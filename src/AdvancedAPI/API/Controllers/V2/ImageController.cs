using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebFramework.API;

namespace API.Controllers.V2
{
    [ApiVersion("2")]
    public class ImageController : BaseApiController
    {
        [HttpPost("[action]")]
        //[Consumes("application/pdf")]
        public async Task<ActionResult> Profile([Required] IFormFile img)
        {
            return File(img.OpenReadStream(), img.ContentType);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> SaveImage()
        {
            var image = Request.Form.Files.GetFile("img");
            return Ok();
        }
    }
}
