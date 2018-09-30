namespace SOW.Framework.Security.LetsEncrypt {
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    public interface IAcmeWrapper: IDisposable {
        int MAX_TRY { get; set; }
        string domainDir { get; set; }
        IGConfig gConfig { get; set; }
        IDomain domain { get; set; }
        (bool, string) IsValidConfig( bool logging = false );
        Task<IOrderResult> CreateOrRenewCert( int rec = 0, bool forceRenew = false );
        Task<ICertificate> CreateCertificate();
        ICertificate GetCertificate();
        bool ExistsCertificate();
        void ReInit( string web );
        void Init( string web, IConfig config = null, ILogger logger = null, int mxtry = 10, CancellationToken ct = default( CancellationToken ) );
    }
}
