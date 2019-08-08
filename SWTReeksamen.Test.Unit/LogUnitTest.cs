using NSubstitute;
using NUnit.Framework;
using SWTReeksamen.Boundaries;
using SWTReeksamen.Interfaces;

namespace SWTReeksamen.Test.Unit
{
    [TestFixture]
    public class LogUnitTest
    {
        private ILog _uut;
        private ILogWriter _logWriter;
        private ITimeProvider _timeProvider;


        [SetUp]
        public void Setup()
        {
            _logWriter = Substitute.For<ILogWriter>();
            _timeProvider = Substitute.For<ITimeProvider>();

            _uut = new Log(_logWriter, _timeProvider);
        }

        [TestCase(-2)]
        [TestCase(-3)]
        [TestCase(-10)]
        public void LogRelayOn_OnNegativeBoundaryValues_HasCorrectOutput(int temp)
        {
            _uut.LogRelayOn(temp);
            //Not possible to test if text is actually written to a log file
            //as it is an uncontrollable external dependency
            //Therefore it is tested that the correct text is given
            _logWriter.Received(1).WriteToLog(_timeProvider.TimeStamp() + $": Temperatur: {temp}. Varme tændt.");
        }

        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void LogRelayOff_OnPositiveBoundaryValuesOver2_HasCorrectOutput(int temp)
        {
            _uut.LogRelayOff(temp);
            //Testing the correct text
            _logWriter.Received(1).WriteToLog(_timeProvider.TimeStamp() + $": Temperatur: {temp}. Varme slukket.");
        }

        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void LogRelayOff_OnPositiveBoundaryValuesOver2_HasWrongOutput(int temp)
        {
            _uut.LogRelayOff(temp);
            //Testing the wrong text
            _logWriter.DidNotReceive().WriteToLog(_timeProvider.TimeStamp() + $": Temperatur: {temp}. Varme tændt.");
        }
    }
}