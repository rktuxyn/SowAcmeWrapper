
namespace SOW.Framework.Security.LetsEncrypt {
    using System.Security.Cryptography.X509Certificates;
    public enum TaskType {
        NOTHING = 0,
        ADD_DNS_TEXT = 1,
        DOWNLOAD_CERT = 2,
        INSTALL_CERT = 3
    }
    public interface IOrderResult {
        X509Certificate2 oldCertificate { get; set; }
        TaskType taskType { get; set; }
        string dnsText { get; set; }
        bool success { get; set; }
        string errorDescription { get; set; }
    }
    public class OrderResult : IOrderResult {
        public X509Certificate2 oldCertificate { get; set; }
        public TaskType taskType { get; set; }
        public string dnsText { get; set; }
        public bool success { get; set; }
        public string errorDescription { get; set; }
    }
    public interface IAcmeResult {
        string Domain { get; set; }
        X509Certificate2 Certificate2 { get; set; }
        string PemKey { get; set; }
        string Email { get; set; }
        bool Success { get; set; }
        string ErrorDescription { get; set; }
    }
}
