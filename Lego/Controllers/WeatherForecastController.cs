using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SharpBrick.PoweredUp;
using Microsoft.Extensions.DependencyInjection;

namespace Lego.Controllers
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
        private readonly IServiceProvider _serviceProvider;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        [HttpGet]
        public async Task Get()
        {
            var host = _serviceProvider.GetService<PoweredUpHost>();

            var cts = new CancellationTokenSource();
            host.Discover(async hub =>
            {
                await hub.ConnectAsync(); // to get some more properties from it

                var hub2 = hub as TwoPortHub;
                using (var technicMediumHub = hub2)
                {
                    // optionally verify if everything is wired up correctly
                    await technicMediumHub.VerifyDeploymentModelAsync(modelBuilder => modelBuilder
                        .AddHub<TwoPortHub>(hubBuilder => hubBuilder
                            .AddDevice<SystemTrainMotor>(technicMediumHub.A)
                        )
                    );

                    await technicMediumHub.RgbLight.SetRgbColorsAsync(0x00, 0xff, 0x00);

                    var motor = technicMediumHub.A.GetDevice<SystemTrainMotor>();

                    while (true)
                    {
                        await motor.StartPowerAsync(25);
                        await Task.Delay(2900);
                        await motor.StopByBrakeAsync();
                        await Task.Delay(4000);
                        await motor.StartPowerAsync(25);

                        await Task.Delay(2600);

                        await motor.StartPowerAsync(-25);

                        await Task.Delay(6150);
                    }

                    await hub.SwitchOffAsync();
                }
            }, cts.Token);
        }
    }
}
