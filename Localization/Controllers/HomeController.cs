using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Diagnostics;
using Localization.Models;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using static Localization.Controllers.HomeController;

namespace Localization.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStringLocalizer<HomeController> _stringLocalizer;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public HomeController(IStringLocalizer<HomeController> stringLocalizer, IHttpContextAccessor httpContextAccessor)
        {
            _stringLocalizer = stringLocalizer;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = _stringLocalizer["title"].Value;
            var today = DateTime.Now;
            var number = 1234567.89;
            var weight = 100.00;
            var volumeLiquid = 100.00;
            var currentCulture = _httpContextAccessor.HttpContext?.Features.Get<IRequestCultureFeature>()?.RequestCulture.Culture.Name;
            
            if (currentCulture != null)
            {
                var cultureInfo = new CultureInfo(currentCulture);
                var measurementSystem = GetMeasurementSystem(cultureInfo);
                ViewBag.Date = today.ToString("d", cultureInfo);
                ViewBag.NumberDouble = number.ToString("N", cultureInfo.NumberFormat);
                ViewBag.Weight = ConvertWeight(weight, measurementSystem);
                ViewBag.Volume = ConvertVolume(volumeLiquid, measurementSystem);
            }
            else
            {
                ViewBag.Date = today.ToString("d");
                ViewBag.NumberDouble = number.ToString("N");
                ViewBag.Weight = weight;
                ViewBag.Volume = volumeLiquid;
            }
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private MeasurementSystem GetMeasurementSystem(CultureInfo cultureInfo)
        {
            var culture = cultureInfo.Name;

            var metricCultures = new List<string> { "fr", "uk" };

            if (metricCultures.Contains(culture))
            {
                return MeasurementSystem.Metric;
            }
            else
            {
                // Default to Imperial for other cultures
                return MeasurementSystem.Imperial;
            }
        }

        private double ConvertWeight(double weight, MeasurementSystem measurementSystem)
        {
            return measurementSystem switch
            {
                MeasurementSystem.Metric => weight, // weight is already in kilograms
                MeasurementSystem.Imperial => KilogramsToPounds(weight), // convert weight from kilograms to pounds 
                _ => weight,
            };
        }

        private double KilogramsToPounds(double kilos)
        {
            var convertedValue = kilos / 0.45359237;
            return Math.Round(convertedValue, 3);
        }

        private double ConvertVolume(double volume, MeasurementSystem measurementSystem)
        {
            return measurementSystem switch
            {
                MeasurementSystem.Metric => volume, // volume is already in litres
                MeasurementSystem.Imperial => LitresToFluidOunces(volume), // convert volume from litres to US gallons
                _ => volume,
            };
        }
        private double LitresToFluidOunces(double litres)
        {
            var convertedValue = litres * 33.8140226;
            return Math.Round(convertedValue, 3);
        }
    }
}
