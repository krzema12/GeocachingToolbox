using System.Collections.Generic;

namespace GeocachingToolbox.GeocachingCom.Logs
{
    public class LogInfo
    {
        public int LogID { get; set; }
        public int CacheID { get; set; }
        public string LogGuid { get; set; }
        public object Latitude { get; set; }
        public object Longitude { get; set; }
        public string LatLonString { get; set; }
        public string LogType { get; set; }
        public string LogTypeImage { get; set; }
        public string LogText { get; set; }
        public string Created { get; set; }
        public string Visited { get; set; }
        public string UserName { get; set; }
        public int MembershipLevel { get; set; }
        public int AccountID { get; set; }
        public string AccountGuid { get; set; }
        public string Email { get; set; }
        public string AvatarImage { get; set; }
        public int GeocacheFindCount { get; set; }
        public int GeocacheHideCount { get; set; }
        public int ChallengesCompleted { get; set; }
        public bool IsEncoded { get; set; }
        public Creator creator { get; set; }
        public IList<Image> Images { get; set; }
    }
}