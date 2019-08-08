using System;

namespace SWTReeksamen.Interfaces
{
    public interface ILog
    {
        void LogRelayOn();
        void LogRelayOff();
        void LogCameraOn();
        void LogCameraOff();
    }
}