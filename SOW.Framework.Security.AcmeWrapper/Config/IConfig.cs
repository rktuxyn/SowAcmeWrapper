
namespace SOW.Framework.Security.LetsEncrypt {
    using System.Collections.Generic;
    using Certes;
    public class IISSettings {
        public string[] AppPool { get; set; }
        public string[] Site { get; set; }
    }
    public interface IDomain {
        string CF_DNS_ZONE { get; set; }
        string ZoneName { get; set; }
        string DomainName { get; set; }
        bool IsWildcard { get; set; }
        bool IsSelfHost { get; set; }
        bool StoreCertificate { get; set; }
        CsrInfo Csr { get; set; }
        IISSettings IISSettings { get; set; }
    }
    public class Domain : IDomain {
        public string CF_DNS_ZONE { get; set; }
        public bool StoreCertificate { get; set; }
        public string ZoneName { get; set; }
        public string DomainName { get; set; }
        public bool IsWildcard { get; set; }
        public bool IsSelfHost { get; set; }
        public CsrInfo Csr { get; set; }
        public IISSettings IISSettings { get; set; }
    };
    public interface IConfig {
        string CF_AUTH_KEY { get; set; }
        string Dir { get; set; }
        string Email { get; set; }
        List<Domain> Domain { get; set; }
    }
    public class Config : IConfig {
        public string CF_AUTH_KEY { get; set; }
        public string Dir { get; set; }
        public string Email { get; set; }
        public List<Domain> Domain { get; set; }
    }
    public interface IWinConfig {
        string WinUser { get; set; }
        string WinPassword { get; set; }
    }
    public class WinConfig: IWinConfig {
        public string WinUser { get; set; }
        public string WinPassword { get; set; }
    }
    public interface IGConfig {
        string CF_API { get; set; }
        string CF_URI { get; set; }
        WinConfig winConfig { get; set; }
        List<Config> config { get; set; }
        object SmtpSettings { get; set; }
    }
    public class GConfig: IGConfig {
        public string CF_API { get; set; }
        public string CF_URI { get; set; }
        public WinConfig winConfig { get; set; }
        public List<Config> config { get; set; }
        public object SmtpSettings { get; set; }
    }
}
