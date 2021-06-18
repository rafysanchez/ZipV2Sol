using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ZipV2.Models;
using System.Net.Http;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Http;

namespace ZipV2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _hostEnvironment;
        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }

        public ActionResult Index()
        {
            var contentRoot = _hostEnvironment.ContentRootPath;
            var wwroot = _hostEnvironment.WebRootPath+ "\\files\\";
            var Content = wwroot + "\n" + contentRoot;
            ViewBag.Files = Directory.EnumerateFiles(wwroot);

          //  GenerateAndDownloadZip();
            return View();
        }

        [HttpPost]
        public ActionResult Index(IFormFile zip)
        {
            var contentRoot = _hostEnvironment.ContentRootPath;
            var wwrootFile = _hostEnvironment.WebRootPath + "\\files\\";


            //  var uploads = Server.MapPath("~/uploads");



            //using (ZipArchive archive = new ZipArchive())
            //{
            //    foreach (ZipArchiveEntry entry in archive.Entries)
            //    {
            //        if (!string.IsNullOrEmpty(Path.GetExtension(entry.FullName))) //make sure it's not a folder
            //        {
            //            entry.ExtractToFile(Path.Combine(wwrootFile, entry.FullName));
            //        }
            //        else
            //        {
            //            Directory.CreateDirectory(Path.Combine(wwrootFile, entry.FullName));
            //        }
            //    }
            //}
            GenerateAndDownloadZip();
            ViewBag.Files = Directory.EnumerateFiles(wwrootFile);
            return View();
        }

        public FileResult GenerateAndDownloadZip()
        {
            var wwrootFile = _hostEnvironment.WebRootPath + "\\files\\";
            var exelFiles = Directory.EnumerateFiles(wwrootFile);

            var webRoot = _hostEnvironment.WebRootPath;
            var fileName = "Templates.zip";
            var tempOut = webRoot + "\\filesOut\\" + fileName;

   

            using (ZipOutputStream zipOutputStream = new ZipOutputStream(System.IO.File.Create(tempOut)))
            {
                zipOutputStream.SetLevel(9);
                byte[] buffer = new byte[4096];
                //var ImageList = new List<string>();
                //ImageList.Add(webRoot + "\\images\\img1.png");
                //ImageList.Add(webRoot + "\\images\\img2.png");
                //ImageList.Add(webRoot + "\\images\\img3.png");



                //for (int i = 0; i < ImageList.Count(); i++)
                //{
                //    ZipEntry entry = new ZipEntry(Path.GetFileName(ImageList[i]));

                //    entry.DateTime = DateTime.Now;
                //    entry.IsUnicodeText = true;
                //    zipOutputStream.PutNextEntry(entry);

                //    using (FileStream oFileStream = System.IO.File.OpenRead(ImageList[i]))
                //    {
                //        int sourceBytes;

                //        do
                //        {
                //            sourceBytes = oFileStream.Read(buffer, 0, buffer.Length);
                //            zipOutputStream.Write(buffer, 0, sourceBytes);

                //        }
                //        while (sourceBytes > 0);

                //    }
                //}
                //zipOutputStream.Finish();
                //zipOutputStream.Flush();
                //zipOutputStream.Close();

           
                foreach (var item in exelFiles)
                {
                    ZipEntry entry = new ZipEntry(Path.GetFileName(item));
                    entry.DateTime = DateTime.Now;
                    entry.IsUnicodeText = true;
                    zipOutputStream.PutNextEntry(entry);
                    
                    using (FileStream oFileStream = System.IO.File.OpenRead(item))
                    {
                        int sourceBytes;

                        do
                        {
                            sourceBytes = oFileStream.Read(buffer, 0, buffer.Length);
                            zipOutputStream.Write(buffer, 0, sourceBytes);

                        }
                        while (sourceBytes > 0);

                    }
                }

                zipOutputStream.Finish();
                zipOutputStream.Flush();
                zipOutputStream.Close();
            }
            byte[] finalResult = System.IO.File.ReadAllBytes(tempOut);

            if (System.IO.File.Exists(tempOut))
            {
                System.IO.File.Delete(tempOut);
            }
            
            return File(finalResult, "application/zip",fileName);
        }


        [HttpPost("UploadFiles")]
        public async Task<IActionResult> Post(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            // full path to file in temp location
            var filePath = Path.GetTempFileName();

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            // process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok(new { count = files.Count, size, filePath });
        }

        public ActionResult Upload(IFormFile file)
        {
            byte[] buffer = new byte[file.Length];
            var resultInBytes = ConvertToBytes(file);
            Array.Copy(resultInBytes, buffer, resultInBytes.Length);

            return Ok(buffer);

        }

        private byte[] ConvertToBytes(IFormFile image)
        {
            using (var memoryStream = new MemoryStream())
            {
                image.OpenReadStream().CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        void Test()
        {
            DirectoryInfo from = new DirectoryInfo(@"C:\Test");
            using (FileStream zipToOpen = new FileStream(@"Test.zip", FileMode.Create))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
                {
                    foreach (FileInfo file in from.AllFilesAndFolders().Where(o => o is FileInfo).Cast<FileInfo>())
                    {
                        var relPath = file.FullName.Substring(from.FullName.Length + 1);
                        ZipArchiveEntry readmeEntry = archive.CreateEntryFromFile(file.FullName, relPath);
                    }
                }
            }
        }


        public void PegaXLS()
        {
            string webRootPath = _hostEnvironment.WebRootPath;
            string contentRootPath = _hostEnvironment.ContentRootPath;

            string path = "";
            path = Path.Combine(webRootPath, "xlsx");
            //or path = Path.Combine(contentRootPath , "wwwroot" ,"CSS" );
           // return View();
        }

        string testeFile()
        {
            string fileName = @"C:\mydir\myfile.ext";
            string path = @"C:\mydir\";
            string result;

            result = Path.GetFileNameWithoutExtension(fileName);
            Console.WriteLine("GetFileNameWithoutExtension('{0}') returns '{1}'",
                fileName, result);

            result = Path.GetFileName(path);
            Console.WriteLine("GetFileName('{0}') returns '{1}'",
                path, result);

            // This code produces output similar to the following:
            //
            // GetFileNameWithoutExtension('C:\mydir\myfile.ext') returns 'myfile'
            // GetFileName('C:\mydir\') returns ''
            return result;
        }


    }
}
