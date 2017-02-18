using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using HA4IoT.Core;
using HA4IoT.Contracts.Core;
using HA4IoT.Contracts.Hardware.Services;
using HA4IoT.Services.Areas;
using System.Threading.Tasks;
using HA4IoT.Contracts.Areas;
using HA4IoT.Sensors;
using HA4IoT.Actuators.StateMachines;
using HA4IoT.Actuators.Connectors;
using HA4IoT.Sensors.Buttons;
using HA4IoT.Actuators;
using HA4IoT.Hardware.Pi2;
using Windows.Devices.Gpio;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace HA4IoT.Controller.Byrialsen.Paw
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
            //private readonly CCToolsBoardService _ccToolsBoardService;
            private readonly IPi2GpioService _pi2GpioService;
            //private readonly SynonymService _synonymService;
            private readonly IAreaService _areaService;
            private readonly ActuatorFactory _actuatorFactory;
            private readonly SensorFactory _sensorFactory;
            //private readonly AutomationFactory _automationFactory;

            //private enum InstalledDevice
            //{
            //    CellarHSRT16
            //}

            private enum Room
            {
                LivingRoom
            }

            private enum LivingRoom
            {
                Button,
                Button2,
                    /*,
                LampTerrace,
                LampTap,
                LampGarage,
                SpotlightRoof,
                LampRearArea,
                LampParkingLot,
                LampParkingLotAutomation,

                SocketPavillion,

                */
                StateMachine
            }

            public Configuration(
                IPi2GpioService pi2GpioService,
                IAreaService areaService,
                ActuatorFactory actuatorFactory,
                SensorFactory sensorFactory
                //AutomationFactory automationFactory
                )
            {
                if (pi2GpioService == null) throw new ArgumentNullException(nameof(pi2GpioService));
                if (actuatorFactory == null) throw new ArgumentNullException(nameof(actuatorFactory));
                if (sensorFactory == null) throw new ArgumentNullException(nameof(sensorFactory));
                //if (automationFactory == null) throw new ArgumentNullException(nameof(automationFactory));

                _pi2GpioService = pi2GpioService;
                _areaService = areaService;
                _actuatorFactory = actuatorFactory;
                _sensorFactory = sensorFactory;
            //    _automationFactory = automationFactory;
            }

            public Task ApplyAsync()
            {
                var livingRoom = _areaService.CreateArea(Room.LivingRoom);

                //var hsrt16 = _ccToolsBoardService.RegisterHSRT16(InstalledDevice.CellarHSRT16, new I2CSlaveAddress(32));

                //var garden = _areaService.CreateArea(Room.Garden);

                //var parkingLotLamp = new LogicalBinaryOutput(hsrt16[HSRT16Pin.Relay6], hsrt16[HSRT16Pin.Relay7], hsrt16[HSRT16Pin.Relay8]);


                //_actuatorFactory.RegisterSocket(livingRoom, LivingRoom.Button, new Pi2Gpio(new GpioPin()).);



                //// Relay 9 is free.
                //_actuatorFactory.RegisterSocket(garden, Garden.SocketPavillion, hsrt16[HSRT16Pin.Relay10]);
                //_actuatorFactory.RegisterLamp(garden, Garden.LampRearArea, hsrt16[HSRT16Pin.Relay11]);
                //_actuatorFactory.RegisterLamp(garden, Garden.SpotlightRoof, hsrt16[HSRT16Pin.Relay12]);
                //_actuatorFactory.RegisterLamp(garden, Garden.LampTap, hsrt16[HSRT16Pin.Relay13]);
                //_actuatorFactory.RegisterLamp(garden, Garden.LampGarage, hsrt16[HSRT16Pin.Relay14]);
                //_actuatorFactory.RegisterLamp(garden, Garden.LampTerrace, hsrt16[HSRT16Pin.Relay15]);
                //_actuatorFactory.RegisterStateMachine(garden, Garden.StateMachine, InitializeStateMachine);



                // paw
                _actuatorFactory.RegisterSocket(livingRoom, LivingRoom.Button2, _pi2GpioService.GetOutput(23).WithInvertedState());
                //_sensorFactory.RegisterButton(livingRoom, LivingRoom.Button, _pi2GpioService.GetInput(23).WithInvertedState());
//                _sensorFactory.RegisterButton(livingRoom, LivingRoom.Button2, _pi2GpioService.GetInput(16).WithInvertedState());

                //livingRoom.GetStateMachine(LivingRoom.StateMachine).SetState() ConnectMoveNextAndToggleOffWith(livingRoom.GetButton(LivingRoom.Button));
                //livingRoom.GetStateMachine(LivingRoom.StateMachine). ConnectMoveNextAndToggleOffWith(livingRoom.GetButton(LivingRoom.Button2));

                //_automationFactory.RegisterConditionalOnAutomation(garden, Garden.LampParkingLotAutomation)
                //    .WithActuator(garden.GetLamp(Garden.LampParkingLot))
                //    .WithOnAtNightRange()
                //    .WithOffBetweenRange(TimeSpan.Parse("22:30:00"), TimeSpan.Parse("05:00:00"));

                //var ioBoardsInterruptMonitor = new InterruptMonitor(_pi2GpioService.GetInput(4));
                //ioBoardsInterruptMonitor.InterruptDetected += (s, e) => _ccToolsBoardService.PollInputBoardStates();
                //ioBoardsInterruptMonitor.Start();

                return Task.FromResult(0);
            }

            //private void InitializeStateMachine(StateMachine stateMachine, IArea garden)
            //{
            //    stateMachine.AddOffState()
            //        .WithActuator(garden.GetLamp(Garden.LampTerrace), BinaryStateId.Off)
            //        .WithActuator(garden.GetLamp(Garden.LampGarage), BinaryStateId.Off)
            //        .WithActuator(garden.GetLamp(Garden.LampTap), BinaryStateId.Off)
            //        .WithActuator(garden.GetLamp(Garden.SpotlightRoof), BinaryStateId.Off)
            //        .WithActuator(garden.GetLamp(Garden.LampRearArea), BinaryStateId.Off);

            //    stateMachine.AddState(new ComponentState("Te"))
            //        .WithActuator(garden.GetLamp(Garden.LampTerrace), BinaryStateId.On)
            //        .WithActuator(garden.GetLamp(Garden.LampGarage), BinaryStateId.Off)
            //        .WithActuator(garden.GetLamp(Garden.LampTap), BinaryStateId.Off)
            //        .WithActuator(garden.GetLamp(Garden.SpotlightRoof), BinaryStateId.Off)
            //        .WithActuator(garden.GetLamp(Garden.LampRearArea), BinaryStateId.Off);

            //    stateMachine.AddState(new ComponentState("G"))
            //        .WithActuator(garden.GetLamp(Garden.LampTerrace), BinaryStateId.Off)
            //        .WithActuator(garden.GetLamp(Garden.LampGarage), BinaryStateId.On)
            //        .WithActuator(garden.GetLamp(Garden.LampTap), BinaryStateId.Off)
            //        .WithActuator(garden.GetLamp(Garden.SpotlightRoof), BinaryStateId.Off)
            //        .WithActuator(garden.GetLamp(Garden.LampRearArea), BinaryStateId.Off);

            //    stateMachine.AddState(new ComponentState("W"))
            //        .WithActuator(garden.GetLamp(Garden.LampTerrace), BinaryStateId.Off)
            //        .WithActuator(garden.GetLamp(Garden.LampGarage), BinaryStateId.Off)
            //        .WithActuator(garden.GetLamp(Garden.LampTap), BinaryStateId.On)
            //        .WithActuator(garden.GetLamp(Garden.SpotlightRoof), BinaryStateId.Off)
            //        .WithActuator(garden.GetLamp(Garden.LampRearArea), BinaryStateId.Off);

            //    stateMachine.AddState(new ComponentState("D"))
            //        .WithActuator(garden.GetLamp(Garden.LampTerrace), BinaryStateId.Off)
            //        .WithActuator(garden.GetLamp(Garden.LampGarage), BinaryStateId.Off)
            //        .WithActuator(garden.GetLamp(Garden.LampTap), BinaryStateId.Off)
            //        .WithActuator(garden.GetLamp(Garden.SpotlightRoof), BinaryStateId.On)
            //        .WithActuator(garden.GetLamp(Garden.LampRearArea), BinaryStateId.Off);

            //    stateMachine.AddState(new ComponentState("Ti"))
            //        .WithActuator(garden.GetLamp(Garden.LampTerrace), BinaryStateId.Off)
            //        .WithActuator(garden.GetLamp(Garden.LampGarage), BinaryStateId.Off)
            //        .WithActuator(garden.GetLamp(Garden.LampTap), BinaryStateId.Off)
            //        .WithActuator(garden.GetLamp(Garden.SpotlightRoof), BinaryStateId.Off)
            //        .WithActuator(garden.GetLamp(Garden.LampRearArea), BinaryStateId.On);

            //    stateMachine.AddState(new ComponentState("G+W"))
            //        .WithActuator(garden.GetLamp(Garden.LampTerrace), BinaryStateId.Off)
            //        .WithActuator(garden.GetLamp(Garden.LampGarage), BinaryStateId.On)
            //        .WithActuator(garden.GetLamp(Garden.LampTap), BinaryStateId.On)
            //        .WithActuator(garden.GetLamp(Garden.SpotlightRoof), BinaryStateId.Off)
            //        .WithActuator(garden.GetLamp(Garden.LampRearArea), BinaryStateId.Off);

            //    stateMachine.AddState(new ComponentState("Te+G+W"))
            //        .WithActuator(garden.GetLamp(Garden.LampTerrace), BinaryStateId.On)
            //        .WithActuator(garden.GetLamp(Garden.LampGarage), BinaryStateId.On)
            //        .WithActuator(garden.GetLamp(Garden.LampTap), BinaryStateId.On)
            //        .WithActuator(garden.GetLamp(Garden.SpotlightRoof), BinaryStateId.Off)
            //        .WithActuator(garden.GetLamp(Garden.LampRearArea), BinaryStateId.Off);

            //    stateMachine.AddState(new ComponentState("Te+G+W+D+Ti"))
            //        .WithActuator(garden.GetLamp(Garden.LampTerrace), BinaryStateId.On)
            //        .WithActuator(garden.GetLamp(Garden.LampGarage), BinaryStateId.On)
            //        .WithActuator(garden.GetLamp(Garden.LampTap), BinaryStateId.On)
            //        .WithActuator(garden.GetLamp(Garden.SpotlightRoof), BinaryStateId.On)
            //        .WithActuator(garden.GetLamp(Garden.LampRearArea), BinaryStateId.On);
            //}
        }
    }
}
