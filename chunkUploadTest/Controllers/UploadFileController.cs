using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using chunkUploadTest.Models;
using Microsoft.AspNetCore.Hosting;
using File = System.IO.File;

namespace chunkUploadTest.Controllers
{
    public class ResponseContext
    {
        public dynamic Data { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string ErrorMessage { get; set; }
    }
    public class UploadFileController : Controller
    {
        private static int index = 0;
        private IWebHostEnvironment _hostEnvironment;
        private readonly object _responseData;

        public UploadFileController(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            _responseData = new ResponseContext();
            
        }


       




        // GET: UploadFileController
        public ActionResult Index()
        {
            return View();
        }

        // GET: UploadFileController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UploadFileController/Create
        public ActionResult Create()
        {
            return View();
        }







        [HttpPost]
        public string UploadChunks( int CHUNK_INDEX, string filename,string ISBN,long USER_ID, IFormFile CHUNK)
        {
            string path = Path.Combine(_hostEnvironment.WebRootPath, @"file\temp");
            string chunkName = USER_ID.ToString()+"-"+ISBN + ".pdf" + CHUNK_INDEX.ToString();
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string newpath = Path.Combine(path, chunkName );
            using (Stream fileStream = new FileStream(newpath, FileMode.Create))
            {

                CHUNK.CopyTo(fileStream);

            }
            return "succes" ;
        }

        

        [HttpPost]
        public string UploadComplete( string ISBN, long USER_ID)
        {
          
                string FinalFileName = USER_ID.ToString() + "-" + ISBN +".pdf" ;
                string path = Path.Combine(_hostEnvironment.WebRootPath, @"file\temp");
                string newpath = Path.Combine(_hostEnvironment.WebRootPath, @"file", FinalFileName );
                string[] filePaths = Directory.GetFiles(path).Where(p => p.Contains(FinalFileName)).OrderBy(p => Int32.Parse(p.Replace(FinalFileName, "$").Split('$')[1])).ToArray();
                foreach (string item in filePaths)
                {
                    MergeFiles(newpath, item);

                }
            

            return "success";
        }

    

        private static void MergeFiles(string file1, string file2)
        {
            FileStream fs1 = null;
            FileStream fs2 = null;
            try
            {
                fs1 = System.IO.File.Open(file1, FileMode.Append);
                fs2 = System.IO.File.Open(file2, FileMode.Open);
                byte[] fs2Content = new byte[fs2.Length];
                fs2.Read(fs2Content, 0, (int)fs2.Length);
                fs1.Write(fs2Content, 0, (int)fs2.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " : " + ex.StackTrace);
            }
            finally
            {
                fs1.Close();
                fs2.Close();
                System.IO.File.Delete(file2);
            }
        }




    }


}

