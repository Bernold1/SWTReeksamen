using System;
using SWTReeksamen.Interfaces;

namespace SWTReeksamen.Controllers
{
    public class TemperatureController : ITempControl
    {
        private State state;
        private ILog log;
        private IThermalRelay thermalRelay;

        public TemperatureController(ITempGauge itempGauge, IThermalRelay ithermalRelay, ILog ilog)
        {
            thermalRelay = ithermalRelay;
            log = ilog;

            itempGauge.TempChangedEvent += HandleTempChangedEvent;

            state = State.Slukket;
        }

        public bool IsHeatOn()
        {
            if (state == State.Tændt)
                return true;
            return false;
        }

        private void HandleTempChangedEvent(object source, TempChangedEventArgs args)
        {
            var temp = args.Temp;

            switch (state)
            {
                // Principiel implementation ifølge tilstandsmaskinediagrammet
                // For tilstand Slukket, når der kommer en ny temperatur
                case State.Slukket:
                    if (temp < 0)
                    {
                        state = State.Tændt;
                        thermalRelay.TurnOn();
                        log.LogRelayOn(temp);
                    }

                    break;

                case State.Tændt:
                    if (temp > 2)
                    {
                        state = State.Slukket;
                        thermalRelay.TurnOff();
                        log.LogRelayOff(temp);
                    }

                    break;
            }
        }

        // Tilstandsmaskinens tilstande
        private enum State
        {
            Slukket,
            Tændt
        }
    }
}