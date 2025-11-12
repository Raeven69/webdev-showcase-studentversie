using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using Showcase_Contactpagina.Models;
using System.Numerics;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Showcase_Contactpagina.Controllers
{
    public class ContactController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string recaptcha;
        public ContactController(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7278");
            recaptcha = config.GetSection("Recaptcha").GetValue<string>("Secret")!;
        }

        // GET: ContactController
        public ActionResult Index()
        {
            return View();
        }

        public async Task<bool> CheckCaptcha(string captcha)
        {
            var response = await _httpClient.PostAsync($"https://www.google.com/recaptcha/api/siteverify?secret={recaptcha}&response={captcha}", null);
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
            var jsonResponse = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(jsonResponse)!;
            return result.success == "true";
        }

        // POST: ContactController
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(Contactform form)
        {
            if(!ModelState.IsValid)
            {
                ViewBag.Message = "De ingevulde velden voldoen niet aan de gestelde voorwaarden";
                return View();
            }

            if (!await CheckCaptcha(Request.Form["g-recaptcha-response"]!))
            {
                ViewBag.Message = "Captcha mislukt.";
                return View();
            }


            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var json = JsonConvert.SerializeObject(form, settings);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            //Gebruik _httpClient om een POST-request te doen naar ShowcaseAPI die de Mail uiteindelijk verstuurt met Mailtrap (of een alternatief).
            //Verstuur de gegevens van het ingevulde formulier mee aan de API, zodat dit per mail verstuurd kan worden naar de ontvanger.
            //Hint: je kunt dit met één regel code doen. Niet te moeilijk denken dus. :-)
            //Hint: vergeet niet om de mailfunctionaliteit werkend te maken in ShowcaseAPI > Controllers > MailController.cs,
            //      nadat je een account hebt aangemaakt op Mailtrap (of een alternatief).

            HttpResponseMessage response = new HttpClient().PostAsync("https://localhost:7278/api/Mail", content).Result; // Vervang deze regel met het POST-request

            if(!response.IsSuccessStatusCode)
            {
                ViewBag.Message = "Er is iets misgegaan";
                return View();
            }

            ViewBag.Message = "Het contactformulier is verstuurd";
            
            return View();
        }
    }
}
