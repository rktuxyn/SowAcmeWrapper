//7:50 PM 9/15/2018 Rajib
namespace SOW.Framework.Security {
    using System;
    using System.Security.Cryptography.X509Certificates;
    public interface IIISWrapper:IDisposable {
        bool ExistsCertificate( X509Store store, string serialNumber, ILogger logger, bool remove = false, bool validOnly = false );
        bool InstallCertificate( X509Certificate2 certificate, ILogger logger, string zoneName, StoreName storeName, X509Certificate2 oldCertificate = null );
        IIISWrapperResponse BindCertificate( IISWrapperSettings iISSettings, ILogger logger );
    }
}
