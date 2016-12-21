using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using System.IO;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Pixiv.Controllers
{
    public class APIController : Controller
    {
        private static HttpClient client;
        private static Regex singleRegex;
        private static Regex mangoRegex;
        private static Regex mangoImageRegex;
        private static Regex titleRegex;
        private static Regex authorRegex;

        private readonly APISettings _APISettings;

        public APIController(IOptions<APISettings> APISettings)
        {
            _APISettings = APISettings.Value;
            if (client == null)
            {
                var container = new CookieContainer();
                var handler = new HttpClientHandler()
                {
                    CookieContainer = container
                };
                client = new HttpClient(handler)
                {
                    BaseAddress = new Uri(_APISettings.Domain)
                };
                container.Add(new Uri(_APISettings.Domain), new Cookie("PHPSESSID", _APISettings.Cookie));
                singleRegex = new Regex(_APISettings.SingleRegex);
                mangoRegex = new Regex(_APISettings.MangoRegex);
                mangoImageRegex = new Regex(_APISettings.MangoImageRegex);
                titleRegex = new Regex(_APISettings.TitleRegex);
                authorRegex = new Regex(_APISettings.AuthorRegex);
            }
        }

        // GET: /API/
        public string Index()
        {
            //return View();
            return "";
        }

        // GET: /API/Fetch/59685919?Index=0
        public async Task<JsonResult> Fetch(string ID, string Index = "0")
        {
            var image = "";
            var url = "";
            var title = "";
            var author = "";
            var referrer = string.Format(_APISettings.PixivUrl, ID);

            var pageResponse = await client.GetAsync(referrer);
            var HTML = await pageResponse.Content.ReadAsStringAsync();

            // 0 - Whole link. http://i4.pixiv.net/img-original/img/2016/06/02/01/43/49/57182187_p0.png
            // 1 - Server.     i4
            // 2 - Path.       2016/06/02/01/43/49
            // 3 - Extension.  png
            var singleMatch = singleRegex.Match(HTML);
            var titleMatch = titleRegex.Match(HTML);
            var authorMatch = authorRegex.Match(HTML);

            // If the index is over 0 it has to be a mango.
            if (Index == "0" && singleMatch.Groups.Count == 4)
            {
                // 57182187_p0.png
                image = $"{ID}_p0.{singleMatch.Groups[3]}";
                // http://i4.pixiv.net/img-original/img/2016/06/02/01/43/49/57182187_p0.png
                url = $"http://{singleMatch.Groups[1]}.pixiv.net/img-original/img/{singleMatch.Groups[2]}/{image}";
                // /var/www/html/image/57182187_p0.png
                title = titleMatch.Groups[1].Value;
                author = authorMatch.Groups[1].Value;
            }
            else if (mangoRegex.IsMatch(HTML))
            {
                // Mango mode needs the mango version loaded too.
                referrer = string.Format(_APISettings.MangoUrl, ID, Index);
                pageResponse = await client.GetAsync(referrer);
                HTML = await pageResponse.Content.ReadAsStringAsync();

                // 0 - Link and shit. <img src="http://i4.pixiv.net/img-original/img/2016/10/10/20/06/37/59410287_p0.jpg" 
                // 1 - Directories.   http://i4.pixiv.net/img-original/img/2016/10/10/20/06/37/
                // 2 - Filename.      59410287_p0.jpg
                var mangoMatch = mangoImageRegex.Match(HTML);

                if (mangoMatch.Groups.Count == 3)
                {
                    image = mangoMatch.Groups[2].Value;
                    url = $"{mangoMatch.Groups[1]}{mangoMatch.Groups[2]}";
                    title = titleMatch.Groups[1].Value;
                    author = authorMatch.Groups[1].Value;
                }
                else
                {
                    return Json(new Models.ErrorModel { Error = $"Invalid mango! Groups: {mangoMatch.Groups.Count} ID: {ID} Index: {Index}" });
                }
            }
            else
            {
                return Json(new Models.ErrorModel { Error = $"Invalid data! Groups: {singleMatch.Groups.Count} ID: {ID} Index: {Index}" });
            }

            var path = Path.Combine(_APISettings.ImageDirectory, image);
            // Check it's not downloaded already.
            if (!System.IO.File.Exists(path))
            {
                using (var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Get
                })
                {
                    request.Headers.Referrer = new Uri(referrer);

                    var task = client.SendAsync(request).ContinueWith(async (msg) =>
                    {
                        var imageResponse = msg.Result.EnsureSuccessStatusCode();
                        var stream = await imageResponse.Content.ReadAsStreamAsync();

                        using (var fs = System.IO.File.Create(path))
                        {
                            await stream.CopyToAsync(fs);
                            fs.Flush();
                        }
                    });
                    task.Wait();
                }
            }

            return Json(new Models.ImageModel { Image = image, URL = $"{_APISettings.ImageBaseUrl}{image}", PixivURL = string.Format(_APISettings.PixivUrl, ID), Title = title, Author = author });
        }
    }
}
