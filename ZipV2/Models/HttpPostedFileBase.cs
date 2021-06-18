using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ZipV2.Models
{
    public class HttpPostedFileBase
    {
        public string Name { get; set; }
        public Stream Content { get; set; }

        public IFormFile File { get; set; }
    }
}
