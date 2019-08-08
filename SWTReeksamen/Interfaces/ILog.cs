using System;

namespace SWTReeksamen.Interfaces
{
    public interface ILog
    {
        void LogRelayOn(int temp);
        void LogRelayOff(int temp);
    }
}