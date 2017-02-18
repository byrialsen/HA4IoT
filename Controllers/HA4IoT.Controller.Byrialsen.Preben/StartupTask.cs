using HA4IoT.Actuators;
using HA4IoT.Automations;
using HA4IoT.Contracts.Areas;
using HA4IoT.Contracts.Core;
using HA4IoT.Contracts.Hardware.Services;
using HA4IoT.Contracts.Triggers;
using HA4IoT.Core;
using HA4IoT.Sensors;
using HA4IoT.Sensors.Buttons;
using HA4IoT.Services.Areas;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace HA4IoT.Controller.Byrialsen.Preben
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
                LivingRoom
            }

            private enum LivingRoom
            {
                Button,
                StateMachine
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
                var livingRoom = _areaService.CreateArea(Room.LivingRoom);

                _actuatorFactory.RegisterSocket(livingRoom, LivingRoom.Button, _pi2GpioService.GetOutput(5)).;

                _actuatorFactory.RegisterLamp(livingRoom, LivingRoom.Button, _pi2GpioService.GetOutput(5));

                


                /*
                _automationFactory.RegisterConditionalOnAutomation(garden, Garden.LampParkingLotAutomation)
                    .WithActuator(garden.GetLamp(Garden.LampParkingLot))
                    .WithOnAtNightRange()
                    .WithOffBetweenRange(TimeSpan.Parse("22:30:00"), TimeSpan.Parse("05:00:00"));
                    */

                _automationFactory.RegisterConditionalOnAutomation(livingRoom, LivingRoom.Button)
                  .WithOnAtNightRange()
                  .WithOffBetweenRange(TimeSpan.Parse("23:55:00"), TimeSpan.Parse("00:01:00"));


                //_automationFactory.RegisterTurnOnAndOffAutomation(livingRoom, LivingRoom.Button)
                //    .WithTrigger(livingRoom.GetButton(LivingRoom.Button).GetPressedShortlyTrigger())
                //    .WithPauseAfterEveryTurnOn(TimeSpan.FromSeconds(5));
                    
                return Task.FromResult(0);
            }
        }
    }
}
