//6:46 PM 9/14/2018 Rajib
namespace SOW.Framework.Security.LetsEncrypt {
    using System.Security.Cryptography.X509Certificates;
    public interface ICertificate {
        X509Certificate2 Cert { get; set; }
        bool isExpired { get; set; }
        bool status { get; set; }
        string errorDescription { get; set; }
    }
}
