//10:13 PM 9/12/2018
namespace SOW.Framework.Security.CloudflareWrapper {
    public interface ICFConfig {
        string CF_API { get; set; }
        string CF_URI { get; set; }
        string CF_AUTH_KEY { get; set; }
        string CF_AUTH_EMAIL { get; set; }
        string CF_DNS_ZONE { get; set; }
    }
}
