using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using paystation_two_party.Models;

namespace paystation_two_party.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;

        public HomeController(IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            _configuration = configuration;
            _contextAccessor = contextAccessor;
        }

        public IActionResult Index()
        {
            Payment payment = new Payment();
            return View(payment);
        }

        [HttpPost]
        public IActionResult CreatePayment([Bind("Amount, CardNumber, CardExpiry")] Payment payment)
        {
            string paystationID = _configuration["AppConfiguration:PaystationID"];
            string gatewayID = _configuration["AppConfiguration:GatewayID"];

            payment.PaystationId = paystationID;
            payment.GatewayId = gatewayID;
            payment.SessionId = _contextAccessor.HttpContext.Session.Id;
            var response = payment.GetResponse();
            return View(response);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
