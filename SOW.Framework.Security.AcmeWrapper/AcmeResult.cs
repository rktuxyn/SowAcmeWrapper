
namespace SOW.Framework.Security.LetsEncrypt {
    using System.Security.Cryptography.X509Certificates;
    public class AcmeResult : IAcmeResult {
        public string Domain { get; set; }
        public X509Certificate2 Certificate2 { get; set; }
        public string PemKey { get; set; }
        public string Email { get; set; }
        public bool Success { get; set; }
        public string ErrorDescription { get; set; }
    };
}
