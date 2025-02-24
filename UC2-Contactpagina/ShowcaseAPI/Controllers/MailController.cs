using Microsoft.AspNetCore.Mvc;
using ShowcaseAPI.Models;
using System.Net;
using System.Net.Mail;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShowcaseAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly SmtpClient client;
        private readonly IConfiguration smtp;
        public MailController(IConfiguration config)
        {
            smtp = config.GetSection("SMTP");
            Console.WriteLine(smtp.GetValue<string>("Address"));
            client = new(smtp.GetValue<string>("Address"), smtp.GetValue<int>("Port"))
            {
                Credentials = new NetworkCredential(smtp.GetValue<string>("Username"), smtp.GetValue<string>("Password")),
                EnableSsl = true
            };
        }

        // POST api/<MailController>
        [HttpPost]
        public ActionResult Post([Bind("FirstName, LastName, Email, Phone")] Contactform form)
        {
            client.Send(form.Email, smtp.GetValue<string>("DefaultEmail")!, "Bericht", $"Berichtje van {form.FirstName} {form.LastName} met telefoonnummer {form.Phone}");
            return Ok();
        }
    }
}
