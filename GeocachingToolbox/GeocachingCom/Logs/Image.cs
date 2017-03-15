namespace GeocachingToolbox.GeocachingCom.Logs
{
    public class Image
    {
        public int ImageID { get; set; }
        public string ImageGuid { get; set; }
        public string Name { get; set; }
        public string Descr { get; set; }
        public string FileName { get; set; }
        public string Created { get; set; }
        public int LogID { get; set; }
        public int CacheID { get; set; }
        public object ImageUrl { get; set; }
    }
}