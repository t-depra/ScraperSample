using System.Web.Mvc;
using System;
using System.Xml;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;
using System.Net;
using System.Text;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Threading.Tasks;
using System.Configuration;
using System.Web.Configuration;

namespace ScraperSample.Controllers
{
    public class ScraperController : Controller
    {
        // GET: Scraper
        public ActionResult Index()
        {
            string url = ConfigurationManager.AppSettings["BingAPIURL"];
            HttpWebRequest HttpWReq = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse HttpWResp = (HttpWebResponse)HttpWReq.GetResponse();

            var encoding = ASCIIEncoding.ASCII;
            string responseText;
            using (var reader = new System.IO.StreamReader(HttpWResp.GetResponseStream(), encoding))
            {
                responseText = reader.ReadToEnd();
            }
            HttpWResp.Close();

            // Storing to Blob Storage 
            string connectionString = WebConfigurationManager.ConnectionStrings["blobConnectionString"].ConnectionString;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("scrapedfiles");
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("scrapertrialstorage");
            blockBlob.UploadTextAsync(responseText);
            return View();
        }
    }
}