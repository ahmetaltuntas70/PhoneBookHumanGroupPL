using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace PhoneBookHumanGroupAPI.Controllers
{
    [ApiController]
    [Route("d")]
    public class DenemeController : ControllerBase
    {
        private readonly ILogger<DenemeController> _logger;

        public DenemeController(ILogger<DenemeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("betul")]
        public IActionResult GetDateOfToday()
        {
            try
            {
                var now=DateTime.Now;
                StringBuilder result = new StringBuilder();
                result.Append(now.ToString("dd-MM-yyyy"));

                switch (now.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        result.AppendLine("Bugün günlerden Pazar!");
                        break;
                    case DayOfWeek.Monday:
                        result.AppendLine("Bugün günlerden Pazartesi!");
                        break;
                    case DayOfWeek.Tuesday:
                        break;
                    case DayOfWeek.Wednesday:
                        break;
                    case DayOfWeek.Thursday:
                        break;
                    case DayOfWeek.Friday:
                        break;
                    case DayOfWeek.Saturday:
                        result.AppendLine("Bugün günlerden Cumartesi!");
                        break;
                    default:
                        break;
                }

                _logger.LogInformation($"metot: Deneme/GetDateOfToday result: {result.ToString()}");
                return Ok(result.ToString());
            }
            catch (Exception ex)
            {
                //loglama
                _logger.LogError(ex, $"metot: Deneme/GetDateOfToday");
                return BadRequest("Beklenmedik bir hata oluştu!");

            }
        }



        [HttpPost(Name ="btl")]
        public IActionResult FindDayofDate(string date)
        {
            try
            {
                DateTime dt= Convert.ToDateTime(date);
                StringBuilder result = new StringBuilder();
                switch (dt.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        result.AppendLine($"{dt.ToString("dd-mm-yyyy")} tarihindeki gün: Pazar!");
                        break;
                    case DayOfWeek.Monday:
                        result.AppendLine($"{dt.ToString("dd-MM-yyyy")} tarihindeki gün: Pazartesi!");
                        break;
                    case DayOfWeek.Tuesday:
                        result.AppendLine($"{dt.ToString("dd-MM-yyyy")} tarihindeki gün: SAlı!");

                        break;
                    case DayOfWeek.Wednesday:
                        result.AppendLine($"{dt.ToString("dd-MM-yyyy")} tarihindeki gün: ÇArşamba!");

                        break;
                    case DayOfWeek.Thursday:
                        result.AppendLine($"{dt.ToString("dd-MM-yyyy")} tarihindeki gün: Perşembe!");

                        break;
                    case DayOfWeek.Friday:
                        result.AppendLine($"{dt.ToString("dd-MM-yyyy")} tarihindeki gün: Cuma!");

                        break;
                    case DayOfWeek.Saturday:
                        result.AppendLine($"{dt.ToString("dd-MM-yyyy")} tarihindeki gün: Cumartesi!");
                        break;
                    default:
                        break;
                }

                _logger.LogInformation($"metot: Deneme/FindDayofDate  - date:{date}");

                return Ok(result.ToString());
            }
            catch (Exception ex)
            {
                //logladık
                _logger.LogError(ex, $"HATA - metot: Deneme/FindDayofDate");
                return BadRequest("Beklenmedik bir hata oluştu!");

            }
        }
    }
}
