using HA4IoT.Actuators;
using HA4IoT.Automations;
using HA4IoT.Contracts.Areas;
using HA4IoT.Contracts.Core;
using HA4IoT.Contracts.Hardware.Services;
using HA4IoT.Core;
using HA4IoT.Sensors;
using HA4IoT.Services.Areas;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace HA4IoT.Controller.Byrialsen.Taverna
{
    public sealed class StartupTask : IBackgroundTask
    {
        private const int LedGpio = 22;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var options = new ControllerOptions
            {
                StatusLedNumber = LedGpio,
                ConfigurationType = typeof(Configuration)
            };

            var controller = new Core.Controller(options);
            controller.RunAsync(taskInstance);
        }

        class Configuration : IConfiguration
        {
            private readonly IPi2GpioService _pi2GpioService;
            private readonly IAreaService _areaService;
            private readonly ActuatorFactory _actuatorFactory;
            private readonly SensorFactory _sensorFactory;
            private readonly AutomationFactory _automationFactory;

            private enum Room
            {
                InsideBoat,
                OutsideBoat
            }

            private enum InsideBoat
            {
                Button1,
                Button2,
                Button3,
                Button4,
                Button5,
                Button6,
                Button7,
                Button8,
            }

            public Configuration(
                IPi2GpioService pi2GpioService,
                IAreaService areaService,
                ActuatorFactory actuatorFactory,
                SensorFactory sensorFactory,
                AutomationFactory automationFactory
                )
            {
                if (pi2GpioService == null) throw new ArgumentNullException(nameof(pi2GpioService));
                if (actuatorFactory == null) throw new ArgumentNullException(nameof(actuatorFactory));
                if (sensorFactory == null) throw new ArgumentNullException(nameof(sensorFactory));
                if (automationFactory == null) throw new ArgumentNullException(nameof(automationFactory));

                _pi2GpioService = pi2GpioService;
                _areaService = areaService;
                _actuatorFactory = actuatorFactory;
                _sensorFactory = sensorFactory;
                _automationFactory = automationFactory;

            }

            public Task ApplyAsync()
            {
                /*
                Knap/relæ 1 - GPIO 05
                Knap/relæ 2 - GPIO 06
                Knap/relæ 3 - GPIO 13
                Knap/relæ 4 - GPIO 19
                Knap/relæ 5 - GPIO 26
                Knap/relæ 6 - GPIO 12
                Knap/relæ 7 - GPIO 16
                Knap/relæ 8 - GPIO 20
                */

                var insideBoat = _areaService.CreateArea(Room.InsideBoat);

                _actuatorFactory.RegisterSocket(insideBoat, InsideBoat.Button1, _pi2GpioService.GetOutput(5));
                _actuatorFactory.RegisterSocket(insideBoat, InsideBoat.Button2, _pi2GpioService.GetOutput(6));
                _actuatorFactory.RegisterSocket(insideBoat, InsideBoat.Button3, _pi2GpioService.GetOutput(13));
                _actuatorFactory.RegisterSocket(insideBoat, InsideBoat.Button4, _pi2GpioService.GetOutput(19));
                _actuatorFactory.RegisterSocket(insideBoat, InsideBoat.Button5, _pi2GpioService.GetOutput(26));
                _actuatorFactory.RegisterSocket(insideBoat, InsideBoat.Button6, _pi2GpioService.GetOutput(12));
                _actuatorFactory.RegisterSocket(insideBoat, InsideBoat.Button7, _pi2GpioService.GetOutput(16));
                _actuatorFactory.RegisterSocket(insideBoat, InsideBoat.Button8, _pi2GpioService.GetOutput(20));

                return Task.FromResult(0);
            }
        }
    }
}
