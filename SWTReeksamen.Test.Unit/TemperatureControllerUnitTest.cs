using System;
using NSubstitute;
using NUnit.Framework;
using SWTReeksamen.Controllers;
using SWTReeksamen.Interfaces;

namespace SWTReeksamen.Test.Unit
{
    [TestFixture]
    class TemperatureControllerUnitTest
    {
        private TemperatureController _uut;
        private ITempGauge _tempGauge;
        private IThermalRelay _thermalRelay;
        private ILog _log;

        [SetUp]
        public void Setup()
        {
            _tempGauge = Substitute.For<ITempGauge>();
            _thermalRelay = Substitute.For<IThermalRelay>();
            _log = Substitute.For<ILog>();
            _uut = new TemperatureController(_tempGauge, _thermalRelay,_log);
        }

        [Test]
        public void NoCallsWereRecievedAtStart()
        {
            //Verificerer at der ikke sker noget ved opstart
            _log.DidNotReceive().LogRelayOff();
        }

        [Test]
        public void HandleTempChangedEvent_InStateOff_TempIsBelowBoundaryValue()
        {
            //Tjekker at relæet kun tænder en gang når at temperaturen er under 0.
            _tempGauge.TempChangedEvent += Raise.EventWith(new TempChangedEventArgs {Temp = -1});
            _thermalRelay.Received(1).TurnOn();
        }
        
        [Test]
        public void HandleTempChangedEvent_InStateOff_LogsOnceWhenTurnedOn()
        {
            //Tester at der kun logges en gang når at temperaturen er under 0 og relæeet tænder.
            _tempGauge.TempChangedEvent += Raise.EventWith(new TempChangedEventArgs { Temp = -2 });
            _log.Received(1).LogRelayOn();
        }

        [TestCase(5)]
        [TestCase(4)]
        [TestCase(3)]
        public void HandleTempChangedEvent_InStateOff_TempIsOverBoundaryValueDoesNotRecieveThermalRelayTurnOn(int temp)
        {
            //Tester at relæet ikke tænder hvis temperaturen er over 2 og state allerede er i slukket
            _tempGauge.TempChangedEvent += Raise.EventWith(new TempChangedEventArgs { Temp = temp});
            _thermalRelay.DidNotReceive().TurnOn();
        }

        [TestCase(5)]
        [TestCase(4)]
        [TestCase(3)]
        public void HandleTempChangedEvent_InStateOff_DoesNotLogAnythingIfTemperatureIsNotWithinBoundary(int temp)
        {
            //Tester at der ikkelogges noget hvis temperaturen er over 2 og state allerede er i slukket
            _tempGauge.TempChangedEvent += Raise.EventWith(new TempChangedEventArgs { Temp = temp });
            _log.DidNotReceive().LogRelayOn();
        }


        [TestCase(-3)]
        [TestCase(-2)]
        [TestCase(-1)]
        public void RelayIsHeatingReturnsTrue(int temp)
        {
            //Tester at når relæet tænder og staten er sat til tændt
            //at når man kalder IsHeatOn() at den returner true
            _tempGauge.TempChangedEvent += Raise.EventWith(new TempChangedEventArgs { Temp = temp });
            Assert.That(_uut.IsHeatOn, Is.True);
        }


        [TestCase( 3)]
        [TestCase( 4)]
        [TestCase( 5)]
        public void RelayIsNotHeatingReturnsFalse(int temp)
        {
            //Tester at IsHeatOn returnerer False hvis at staten er i slukket
            _tempGauge.TempChangedEvent += Raise.EventWith(new TempChangedEventArgs { Temp = temp });
            Assert.That(_uut.IsHeatOn, Is.False);
        }



        [Test]
        public void HandleTempChangedEvent_InStateOn_ThermalRelayTurnOff_IsCalledOnce()
        {
            //Temperaturen ændres og eventet kaldes, som tænder for relæet og sætter den i tændt tilstand,
            //tester at når temperaturen derefter er over 2 at relæet slukkes.
            _tempGauge.TempChangedEvent += Raise.EventWith(new TempChangedEventArgs { Temp = -2 });
            _tempGauge.TempChangedEvent += Raise.EventWith(new TempChangedEventArgs { Temp = 3 });
            _thermalRelay.Received(1).TurnOff();
        }


        [Test]
        public void HandleTempChangedEvent_InStateOn_LogRelayOff_IsCalledOnce()
        {
            //Tester at der logges en gang når temperaturen er over 2 og relæet slukkes. 
            _tempGauge.TempChangedEvent += Raise.EventWith(new TempChangedEventArgs { Temp = -2 });
            _tempGauge.TempChangedEvent += Raise.EventWith(new TempChangedEventArgs { Temp = 3 });
            _log.Received(1).LogRelayOff();
        }

        [Test]
        public void HandleTempChangedEvent_InStateOn_TemperatureIsNotWarmEnough_ThermalRelayTurnOff_IsNotCalled()
        {
            _tempGauge.TempChangedEvent += Raise.EventWith(new TempChangedEventArgs { Temp = -2 });
            _tempGauge.TempChangedEvent += Raise.EventWith(new TempChangedEventArgs { Temp = 1 });
            _thermalRelay.DidNotReceive().TurnOff();
        }

        [TestCase(1, -4)]
        [TestCase(2, -2)]
        [TestCase(3, -1)]
        public void HandleTempChangedEvent_InStateOff_StillHeating_ThermalRelayTurnOn_OnlyCalledOnce_WithMultipleRaises(int calls, int testTemp)
        {
            for (int i = 0; i < calls; i++)
            {
                _tempGauge.TempChangedEvent += Raise.EventWith(new TempChangedEventArgs { Temp = testTemp });
            }
            _thermalRelay.Received(1).TurnOn();
        }

        [TestCase(1, -4)]
        [TestCase(2, -2)]
        [TestCase(3, -1)]
        public void HandleTempChangedEvent_InStateOff_StillHeating_LogRelayOn_OnlyCalledOnce_WithMultipleRaises(int calls, int testTemp)
        {
            for (int i = 0; i < calls; i++)
            {
                _tempGauge.TempChangedEvent += Raise.EventWith(new TempChangedEventArgs { Temp = testTemp });
            }
            _log.Received(1).LogRelayOn();
        }

        [TestCase(1, 3)]
        [TestCase(2, 4)]
        [TestCase(3, 3)]
        public void HandleTempChangedEvent_InStateOff_RaisedMultipleTimesButTemperaturWithInBoundaries(int calls, int testTemp)
        {
            //Tester at relæet ikke prøver at slukke når temperaturen er udenfor boundary værdierne men temperaturen skifter flere gange
            for (int i = 0; i < calls; i++)
            {
                _tempGauge.TempChangedEvent += Raise.EventWith(new TempChangedEventArgs { Temp = testTemp });
            }
            _thermalRelay.DidNotReceive().TurnOff();
        }










    }
}
