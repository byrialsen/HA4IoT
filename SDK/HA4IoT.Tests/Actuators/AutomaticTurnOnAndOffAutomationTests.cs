﻿using System;
using HA4IoT.Actuators.Lamps;
using HA4IoT.Automations;
using HA4IoT.Components;
using HA4IoT.Contracts.Actuators;
using HA4IoT.Contracts.Automations;
using HA4IoT.Contracts.Components;
using HA4IoT.Contracts.Components.States;
using HA4IoT.Contracts.Services.Daylight;
using HA4IoT.Contracts.Services.Settings;
using HA4IoT.Contracts.Services.System;
using HA4IoT.Tests.Mockups;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace HA4IoT.Tests.Actuators
{
    [TestClass]
    public class AutomaticTurnOnAndOffAutomationTests
    {
        [TestMethod]
        public void Should_TurnOn_IfMotionDetected()
        {
            var testController = new TestController();
            var motionDetectorFactory = testController.GetInstance<TestMotionDetectorFactory>();

            var automation = new FlipFlopAutomation(
                AutomationIdGenerator.EmptyId,
                testController.GetInstance<IDateTimeService>(), 
                testController.GetInstance<ISchedulerService>(),
                testController.GetInstance<ISettingsService>(),
                testController.GetInstance<IDaylightService>());

            var motionDetector = motionDetectorFactory.CreateTestMotionDetector();
            var output = new Lamp(new ComponentId("?"), new TestBinaryStateAdapter());
            Assert.AreEqual(true, output.GetState().Has(PowerState.Off));

            automation.WithTrigger(motionDetector);
            automation.WithTarget(output);

            motionDetector.TriggerMotionDetection();

            Assert.AreEqual(true, output.GetState().Has(PowerState.On));
        }

        [TestMethod]
        public void Should_TurnOn_IfButtonPressedShort()
        {
            var testController = new TestController();
            var buttonFactory = testController.GetInstance<TestButtonFactory>();

            var automation = new FlipFlopAutomation(
                AutomationIdGenerator.EmptyId,
                testController.GetInstance<IDateTimeService>(),
                testController.GetInstance<ISchedulerService>(),
                testController.GetInstance<ISettingsService>(),
                testController.GetInstance<IDaylightService>());

            var button = buttonFactory.CreateTestButton();
            var output = new Lamp(new ComponentId("?"), new TestBinaryStateAdapter());
            Assert.AreEqual(true, output.GetState().Has(PowerState.Off));

            automation.WithFlipTrigger(button.PressedShortlyTrigger);
            automation.WithTarget(output);

            button.PressShortly();

            Assert.AreEqual(true, output.GetState().Has(PowerState.On));
        }

        [TestMethod]
        public void Should_NotTurnOn_IfMotionDetected_AndTimeRangeConditionIs_NotFulfilled()
        {
            var testController = new TestController();
            testController.SetTime(TimeSpan.Parse("18:00:00"));
            var motionDetectorFactory = testController.GetInstance<TestMotionDetectorFactory>();

            var automation = new FlipFlopAutomation(
                AutomationIdGenerator.EmptyId,
                testController.GetInstance<IDateTimeService>(),
                testController.GetInstance<ISchedulerService>(),
                testController.GetInstance<ISettingsService>(),
                testController.GetInstance<IDaylightService>());

            var motionDetector = motionDetectorFactory.CreateTestMotionDetector();
            var output = new Lamp(new ComponentId("?"), new TestBinaryStateAdapter());
            Assert.AreEqual(true, output.GetState().Has(PowerState.Off));

            automation.WithTurnOnWithinTimeRange(() => TimeSpan.Parse("10:00:00"), () => TimeSpan.Parse("15:00:00"));
            automation.WithTrigger(motionDetector);
            automation.WithTarget(output);

            motionDetector.TriggerMotionDetection();

            Assert.AreEqual(true, output.GetState().Has(PowerState.Off));
        }

        [TestMethod]
        public void Should_TurnOn_IfButtonPressed_EvenIfTimeRangeConditionIs_NotFulfilled()
        {
            var testController = new TestController();
            testController.SetTime(TimeSpan.Parse("18:00:00"));
            var buttonFactory = testController.GetInstance<TestButtonFactory>();

            var automation = new FlipFlopAutomation(
                AutomationIdGenerator.EmptyId,
                testController.GetInstance<IDateTimeService>(),
                testController.GetInstance<ISchedulerService>(),
                testController.GetInstance<ISettingsService>(),
                testController.GetInstance<IDaylightService>());

            var button = buttonFactory.CreateTestButton();
            var output = new Lamp(new ComponentId("?"), new TestBinaryStateAdapter());
            Assert.AreEqual(true, output.GetState().Has(PowerState.Off));

            automation.WithTurnOnWithinTimeRange(() => TimeSpan.Parse("10:00:00"), () => TimeSpan.Parse("15:00:00"));
            automation.WithFlipTrigger(button.PressedShortlyTrigger);
            automation.WithTarget(output);

            button.PressShortly();

            Assert.AreEqual(true, output.GetState().Has(PowerState.On));
        }

        [TestMethod]
        public void Should_NotTurnOn_IfMotionDetected_AndSkipConditionIs_Fulfilled()
        {
            var testController = new TestController();
            testController.SetTime(TimeSpan.Parse("14:00:00"));
            var motionDetectorFactory = testController.GetInstance<TestMotionDetectorFactory>();

            var automation = new FlipFlopAutomation(
                AutomationIdGenerator.EmptyId,
                testController.GetInstance<IDateTimeService>(),
                testController.GetInstance<ISchedulerService>(),
                testController.GetInstance<ISettingsService>(),
                testController.GetInstance<IDaylightService>());

            var motionDetector = motionDetectorFactory.CreateTestMotionDetector();

            var output = new Lamp(new ComponentId("?"), new TestBinaryStateAdapter());
            Assert.AreEqual(true, output.GetState().Has(PowerState.Off));

            automation.WithTrigger(motionDetector);
            automation.WithTarget(output);

            var other2 = new Lamp(new ComponentId("?"), new TestBinaryStateAdapter());
            other2.TryTurnOn();

            IActuator[] otherActuators =
            {
                new Lamp(new ComponentId("?"), new TestBinaryStateAdapter()),
                other2
            };

            automation.WithSkipIfAnyActuatorIsAlreadyOn(otherActuators);

            motionDetector.TriggerMotionDetection();

            Assert.AreEqual(true, output.GetState().Has(PowerState.Off));
        }

        [TestMethod]
        public void Should_TurnOn_IfMotionDetected_AndSkipConditionIs_NotFulfilled()
        {
            var testController = new TestController();
            testController.SetTime(TimeSpan.Parse("14:00:00"));
            var motionDetectorFactory = testController.GetInstance<TestMotionDetectorFactory>();

            var automation = new FlipFlopAutomation(
                AutomationIdGenerator.EmptyId,
                testController.GetInstance<IDateTimeService>(),
                testController.GetInstance<ISchedulerService>(),
                testController.GetInstance<ISettingsService>(),
                testController.GetInstance<IDaylightService>());

            var motionDetector = motionDetectorFactory.CreateTestMotionDetector();

            var output = new Lamp(new ComponentId("?"), new TestBinaryStateAdapter());
            Assert.AreEqual(true, output.GetState().Has(PowerState.Off));

            automation.WithTrigger(motionDetector);
            automation.WithTarget(output);

            IActuator[] otherActuators =
            {
                new Lamp(new ComponentId("?"), new TestBinaryStateAdapter()),
                new Lamp(new ComponentId("?"), new TestBinaryStateAdapter())
            };

            automation.WithSkipIfAnyActuatorIsAlreadyOn(otherActuators);

            motionDetector.TriggerMotionDetection();

            Assert.AreEqual(true, output.GetState().Has(PowerState.On));
        }

        [TestMethod]
        public void Should_TurnOff_IfButtonPressed_WhileTargetIsAlreadyOn()
        {
            var testController = new TestController();
            testController.SetTime(TimeSpan.Parse("14:00:00"));
            var buttonFactory = testController.GetInstance<TestButtonFactory>();

            var automation = new FlipFlopAutomation(
                AutomationIdGenerator.EmptyId,
                testController.GetInstance<IDateTimeService>(),
                testController.GetInstance<ISchedulerService>(),
                testController.GetInstance<ISettingsService>(),
                testController.GetInstance<IDaylightService>());

            var button = buttonFactory.CreateTestButton();

            var output = new Lamp(new ComponentId("?"), new TestBinaryStateAdapter());
            Assert.AreEqual(true, output.GetState().Has(PowerState.Off));

            automation.WithFlipTrigger(button.PressedShortlyTrigger);
            automation.WithTarget(output);

            IActuator[] otherActuators =
            {
                new Lamp(new ComponentId("?"), new TestBinaryStateAdapter()),
                new Lamp(new ComponentId("?"), new TestBinaryStateAdapter())
            };

            automation.WithSkipIfAnyActuatorIsAlreadyOn(otherActuators);

            button.PressShortly();
            Assert.AreEqual(true, output.GetState().Has(PowerState.On));

            button.PressShortly();
            Assert.AreEqual(true, output.GetState().Has(PowerState.On));

            automation.WithTurnOffIfButtonPressedWhileAlreadyOn();
            button.PressShortly();
            Assert.AreEqual(true, output.GetState().Has(PowerState.Off));
        }
    }
}