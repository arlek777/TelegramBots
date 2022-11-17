using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SharpBrick.PoweredUp;

namespace LegoTest2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly PoweredUpHost _poweredUpHost;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, PoweredUpHost poweredUpHost)
        {
            _logger = logger;
            _poweredUpHost = poweredUpHost;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var cts = new CancellationTokenSource();
            _poweredUpHost.Discover(async hub =>
            {
                await hub.ConnectAsync(); // to get some more properties from it

                using (var technicMediumHub = hub as TwoPortHub)
                {
                    // optionally verify if everything is wired up correctly
                    await technicMediumHub.VerifyDeploymentModelAsync(modelBuilder => modelBuilder
                        .AddHub<TwoPortHub>(hubBuilder => hubBuilder
                            .AddDevice<SystemTrainMotor>(technicMediumHub.A)
                        )
                    );

                    await technicMediumHub.RgbLight.SetRgbColorsAsync(0x00, 0xff, 0x00);

                    var motor = technicMediumHub.A.GetDevice<SystemTrainMotor>();

                    await motor.StartPowerAsync(-15);
                    await motor.StartPowerAsync(-20);
                    await motor.StopByBrakeAsync();

                    await motor.StartPowerAsync(15);
                    await motor.StopByBrakeAsync();

                    await technicMediumHub.SwitchOffAsync();
                }

                // show in UI
            }, cts.Token);

            return Ok();
        }
    }
}
