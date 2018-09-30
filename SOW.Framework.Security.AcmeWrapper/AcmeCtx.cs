namespace SOW.Framework.Security.LetsEncrypt {
    using Certes;
    using Certes.Acme;
    class AcmeCtx : IAcmeCtx {
        public IAcmeContext Ctx { get; set; }
        public IAccountContext ACtx { get; set; }
    }
}
