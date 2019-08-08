using System;
using System.IO;
using SWTReeksamen.Interfaces;

namespace SWTReeksamen.Boundaries
{
    public class LogWriter : ILogWriter
    {
        private string logFile = "logfile.txt"; 
        public void WriteToLog(string text)
        {
            using (var writer = File.AppendText(logFile))
            {
                writer.WriteLine(text);
            }
        }


    }
}