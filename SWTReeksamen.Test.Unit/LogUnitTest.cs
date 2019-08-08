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
        void Setup()
        {
            _logWriter = Substitute.For<ILogWriter>();
            _timeProvider = Substitute.For<ITimeProvider>();

            _uut = new Log(_logWriter, _timeProvider);
        }


    }
}