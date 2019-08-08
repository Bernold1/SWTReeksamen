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


        [TestCase(-1)]
        [TestCase(-2)]
        [TestCase(-3)]
        public void HandleTempChangedEvent_InStateOff_TempIsBelowBoundaryValue(int temp)
        {
            //Tjekker at relæet kun tænder en gang når at temperaturen er under 0.
            _tempGauge.TempChangedEvent += Raise.EventWith(new TempChangedEventArgs {Temp = temp});
            _thermalRelay.Received(1).TurnOn();
        }

        [TestCase(-1)]
        [TestCase(-2)]
        [TestCase(-3)]
        public void HandleTempChangedEvent_InStateOff_LogsOnceWhenTurnedOn(int temp)
        {
            //Tjekker at relæet kun tænder en gang når at temperaturen er under 0.
            _tempGauge.TempChangedEvent += Raise.EventWith(new TempChangedEventArgs { Temp = temp });
            _log.Received(1).LogRelayOn();
        }


        [TestCase(1, 3)]
        [TestCase(2, 4)]
        [TestCase(3, 3)]
        public void HandleTempChangedEvent_InStateOff_RaisedMultipleTimesButTemperaturIsNotRight(int calls, int testTemp)
        {
            //Tester at relæet ikke tændes når temperaturen er udenfor boundary værdierne men temperaturen skifter flere gange
            for (int i = 0; i < calls; i++)
            {
                _tempGauge.TempChangedEvent += Raise.EventWith(new TempChangedEventArgs {Temp = testTemp});
            }
            _thermalRelay.DidNotReceive().TurnOn();
        }



    }
}
