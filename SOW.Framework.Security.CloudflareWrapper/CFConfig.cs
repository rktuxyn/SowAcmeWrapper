//10:13 PM 9/12/2018
namespace SOW.Framework.Security.CloudflareWrapper {
    public class CFConfig: ICFConfig {
        public string CF_API { get; set; }
        public string CF_URI { get; set; }
        public string CF_AUTH_KEY { get; set; }
        public string CF_AUTH_EMAIL { get; set; }
        public string CF_DNS_ZONE { get; set; }
    }
}
