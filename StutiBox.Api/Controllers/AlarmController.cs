using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StutiBox.Api.Config;
using StutiBox.Api.Workers;

namespace StutiBox.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlarmController : ControllerBase
    {
        MorningAlarmWorker _alarmWorker;
        public AlarmController(MorningAlarmWorker alarmWorker)
        {
            _alarmWorker = alarmWorker;
        }
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_alarmWorker.AlarmConfiguration);
        }

        [HttpPost]
        public async Task<IActionResult> Post(AlarmConfiguration configuration)
        {
            var ct = new CancellationToken();
            await _alarmWorker.StopAsync(ct);
            _alarmWorker.AlarmConfiguration = configuration;
            await _alarmWorker.StartAsync(ct);
            return Ok(_alarmWorker.AlarmConfiguration);
        }
    }
}
