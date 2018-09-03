using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ShortUrl.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ShortUrl.Controllers
{
    [Route("")]
    public class ShortUrlController : Controller
    {

        private readonly UrlContext _context;

        public ShortUrlController(UrlContext context)
        {
            _context = context;
        }
        //private static string[] Summaries = new[]
        //{
        //    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        //};

        //[HttpGet("[action]")]
        //public IEnumerable<WeatherForecast> WeatherForecasts()
        //{
        //    var rng = new Random();
        //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    {
        //        DateFormatted = DateTime.Now.AddDays(index).ToString("d"),
        //        TemperatureC = rng.Next(-20, 55),
        //        Summary = Summaries[rng.Next(Summaries.Length)]
        //    });
        //}

        //public class WeatherForecast
        //{
        //    public string DateFormatted { get; set; }
        //    public int TemperatureC { get; set; }
        //    public string Summary { get; set; }

        //    public int TemperatureF
        //    {
        //        get
        //        {
        //            return 32 + (int)(TemperatureC / 0.5556);
        //        }
        //    }
        //}

        [HttpPost("api/[controller]/[action]")]
        public async Task<IActionResult> Shorten(String uriList)
        {
            if (string.IsNullOrEmpty(uriList))
                return BadRequest(new { Error = "Invalid URI" });
            
            var uriArray = uriList.Trim().Split(" ");
            foreach (var uriName in uriArray)
            {                
                if (!isValidUri(uriName))
                {
                    return BadRequest(new { Error = String.Format("Invalid URI {0}", uriName) });                    
                }
            }

            var flagNewToken = false;
            var listUrls = new List<Url>();
            foreach (var uriName in uriArray)
            {
                if (!_context.Urls.Any(u => u.LongUrl == uriName))
                {
                    var newToken = RandomString();
                    while(_context.Urls.Any(t => t.Token == newToken))
                    {
                        newToken = RandomString();
                    }

                    var urlItem = new Url()
                    {
                        Token = newToken,
                        LongUrl = uriName
                    };
                    _context.Urls.Add(urlItem);
                    listUrls.Add(urlItem);
                    await _context.SaveChangesAsync();
                    flagNewToken = true;
                }
                else
                {
                    var urlItem = new Url()
                    {
                        Token = _context.Urls.Where(u => u.LongUrl == uriName).First().Token,
                        LongUrl = uriName
                    };
                    listUrls.Add(urlItem);
                }
            }
            return flagNewToken ? StatusCode((int)HttpStatusCode.Created, listUrls) : StatusCode((int)HttpStatusCode.OK, listUrls);
        }        

        [HttpGet("{token}")]
        public IActionResult RedirectUrl(string token)
        {            
            if (!_context.Urls.Any(u => u.Token == token))
            {
                return StatusCode((int)HttpStatusCode.NotFound, token);
            }
            return Redirect(_context.Urls.Where(u => u.Token == token).First().LongUrl);
        }

        //[HttpGet("[action]")]
        //public IEnumerable<Url> GetUrls()
        //{
        //    return _context.Urls;
        //}

        private static Random random = new Random();
        public static string RandomString()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 7)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private static bool isValidUri(string uriName)
        {
            Uri uriResult;                       
            return Uri.TryCreate(uriName, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);            
        }
    }
}
