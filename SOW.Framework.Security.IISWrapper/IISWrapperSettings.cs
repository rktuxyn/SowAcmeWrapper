//7:45 PM 9/15/2018 Rajib
namespace SOW.Framework.Security {
    public class IISWrapperSettings : IIISWrapperSettings {
        public string ZoneName { get; set; }
        public string SiteName { get; set; }
        public string CertificateStoreName { get; set; }
        public byte[] CertificateHash { get; set; }
        public string[] AppPool { get; set; }
        public string[] Site { get; set; }
    }
}
