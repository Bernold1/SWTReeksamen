using System;

namespace SWTReeksamen.Interfaces
{
    public class TempChangedEventArgs : EventArgs
    {
        public int Temp { get; set; }
    }
    public interface ITempGauge
    {
        event EventHandler<TempChangedEventArgs> TempChangedEvent;
        int GetTemperatur();
    }
}