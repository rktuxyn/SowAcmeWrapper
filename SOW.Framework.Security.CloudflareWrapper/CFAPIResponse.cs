//2:40 AM 9/14/2018 Rajib
namespace SOW.Framework.Security.CloudflareWrapper {
    public class CFAPIResponse : ICFAPIResponse {
        public object result { get; set; }
        public bool success { get; set; }
        public object errors { get; set; }
        public object messages { get; set; }
    }
}
