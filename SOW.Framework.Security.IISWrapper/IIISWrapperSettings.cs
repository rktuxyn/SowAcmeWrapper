//7:45 PM 9/15/2018 Rajib
namespace SOW.Framework.Security {
    public interface IIISWrapperSettings {
        string SiteName { get; set; }
        string ZoneName { get; set; }
        byte[] CertificateHash { get; set; }
        string CertificateStoreName { get; set; }
        string[] AppPool { get; set; }
        string[] Site { get; set; }
    }
}
