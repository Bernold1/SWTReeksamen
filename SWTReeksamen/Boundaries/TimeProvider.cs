using System;
using SWTReeksamen.Interfaces;

namespace SWTReeksamen.Boundaries
{
    public class TimeProvider:ITimeProvider
    {
        public string TimeStamp()
        {
            return DateTime.Now.ToLongTimeString();
        }
    }
}