using System;
using SWTReeksamen.Interfaces;

namespace SWTReeksamen.Boundaries
{
    public class Log: ILog
    {
        private ILogWriter logWriter;
        private ITimeProvider timeProvider;

        //Dependency injection needed
        public Log(ILogWriter iLogWriter, ITimeProvider iTimeProvider)
        {
            logWriter = iLogWriter;
            timeProvider = iTimeProvider;
        }
        public void LogRelayOn(int temp)
        {
            logWriter.WriteToLog(timeProvider.TimeStamp() + $": Temperatur: {temp}. Varme tændt.");
        }

        public void LogRelayOff(int temp)
        {
            logWriter.WriteToLog(timeProvider.TimeStamp() + $": Temperatur: {temp}. Varme slukket.");
        }
    }
}