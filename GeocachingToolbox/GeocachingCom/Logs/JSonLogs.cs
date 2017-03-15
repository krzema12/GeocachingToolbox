using System.Collections.Generic;

namespace GeocachingToolbox.GeocachingCom.Logs
{
    public class JSonLogs
    {
        public string status { get; set; }
        public IList<LogInfo> data { get; set; }
        public PageInfo pageInfo { get; set; }
    }
}
