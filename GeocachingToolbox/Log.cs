using System;

namespace GeocachingToolbox
{
    public class Log
    {
        public DateTime Date { get; set; }
        public string Comment { get; set; }
        public ILoggable Thing { get; set; }
        public GeocacheLogType LogType { get; set; }
        public string Username { get; set; }

        public override string ToString()
        {
            return "Log from " + Date.ToString() + ": " + Comment;
        }
    }
}
