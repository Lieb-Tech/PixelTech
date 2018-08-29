using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using HtmlAgilityPack;
using ImageProcessor;
using InstaSharp;
using Newtonsoft.Json.Linq;

namespace PixelTech.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> DotTech()
        {
            return View();
        }
        public async Task<ActionResult> CLetter()
        {
            return View();
        }

        public async Task<ActionResult> Index()
        {
            var url = "https://www.instagram.com/explore/tags/moasdaytona/";

            var webClient = new WebClient();
            var html = webClient.DownloadString(url);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            var htmlElement = doc.DocumentNode.ChildNodes.Where(n => n.Name == "html").FirstOrDefault();
            var body = htmlElement.ChildNodes.Where(n => n.Name == "body").FirstOrDefault();
            var script = body.ChildNodes.ToArray()[3].InnerText;
            var idx = script.IndexOf("display_url");

            var images = new List<string>();
            while (idx > -1 && images.Count() < 25)
            {
                var idx2 = script.IndexOf("jpg", idx);
                var data = script.Substring(idx + 14, idx2 - idx - 11);
                images.Add(data);
                idx = script.IndexOf("display_url", idx2);
                
            }

            string appPath = System.Web.Hosting.HostingEnvironment.MapPath("/");
            if (!Directory.Exists(appPath + "\\SampleImages"))
                Directory.CreateDirectory(appPath + "\\SampleImages");

            var finalList = new Dictionary<int,string>();

            foreach (var im in images)
            {
                var uri = new Uri(im);
                var f = uri.Segments.Last();
                var loc = "SampleImages\\" + f;             
                var path = $@"{appPath}{loc}";

                if (!System.IO.File.Exists(path))
                {
                    var bytes = webClient.DownloadData(im);
                    System.IO.File.WriteAllBytes(path, bytes);
                }
                
                processBySize(10, path);
                processBySize(15, path);
                processBySize(20, path);
                processBySize(25, path);
                processBySize(70, path);
                /*
                // processBySize(35, path);
                processBySize(40, path);
                // processBySize(45, path);
                processBySize(50, path);
                processBySize(60, path);
                processBySize(70, path);
                */
                /*
                finalList.Add(finalList.Count(), loc.Replace("\\", "/"));
                finalList.Add(finalList.Count(), loc.Replace("\\", "/") + ".70.bmp");
                finalList.Add(finalList.Count(), loc.Replace("\\", "/") + ".60.bmp");

                finalList.Add(finalList.Count(), loc.Replace("\\", "/") + ".50.bmp");
                // finalList.Add(finalList.Count(), loc.Replace("\\", "/") + ".45.bmp");
                finalList.Add(finalList.Count(), loc.Replace("\\", "/") + ".40.bmp");
                // finalList.Add(finalList.Count(), loc.Replace("\\", "/") + ".35.bmp");
                */
                finalList.Add(finalList.Count(), loc.Replace("\\", "/") + ".70.bmp");
                finalList.Add(finalList.Count(), loc.Replace("\\", "/") + ".25.bmp");
                finalList.Add(finalList.Count(), loc.Replace("\\", "/") + ".20.bmp");
                finalList.Add(finalList.Count(), loc.Replace("\\", "/") + ".15.bmp");
                finalList.Add(finalList.Count(), loc.Replace("\\", "/") + ".10.bmp");
            }

            ViewBag.stride = 5;
            ViewBag.count = finalList.Count();
            ViewBag.images = finalList.OrderBy(z => z.Key).Select(z => z.Value).ToArray();

            return View();
        }

        void processBySize(int size, string path)
        {
            var dest = $"{path}.{size}.bmp";
            var f = new FileInfo($"{path}.{size}.bmp");
            if (!f.Exists)
            {
                byte[] photoBytes = System.IO.File.ReadAllBytes(path);
                Bitmap bmp = new Bitmap(path);
                using (MemoryStream inStream = new MemoryStream(photoBytes))
                {
                    using (MemoryStream outStream = new MemoryStream())
                    {
                        // Initialize the ImageFactory using the overload to preserve EXIF metadata.
                        using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                        {
                            // Load, resize, set the format and quality and save an image.
                            imageFactory.Load(inStream)
                                .Pixelate(bmp.Width / size)
                                .Save(dest);
                        }
                    }
                }
            }
        }

        /*
        ViewBag.Title = "Home Page";

        // https://api.instagram.com/oauth/authorize/?client_id=8a22857e52fd49989d2bad442b9a45dd&redirect_uri=http://PixTech.azurewebsites.net&response_type=code

        string clientId = "8a22857e52fd49989d2bad442b9a45dd";
        string clientSecret = "7525079f795b4401bae6639a1952075d";
        string redirectUri = "http://PixTech.azurewebsites.net";

        string code = "78ac7a45d14348d8be1d37c46a215c54";

        InstagramConfig config = new InstagramConfig(clientId, clientSecret, redirectUri);

        // add this code to the auth object
        var auth = new OAuth(config);

        // now we have to call back to instagram and include the code they gave us
        // along with our client secret
        var oauthResponse = await auth.RequestToken(code);            

        string token = oauthResponse.AccessToken;
        string tagName = "MOAS";
        string endpoint = $"https://api.instagram.com/v1/tags/{tagName}?access_token={token}";
        endpoint = $"https://api.instagram.com/v1/tags/{tagName}/media/recent?access_token={token}";

        var t = new InstaSharp.Endpoints.Users(config, oauthResponse);
        var tags = await t.
        */


    }
}
