//10:08 PM 9/14/2018 Rajib
namespace SOW.Framework.Security.LetsEncrypt {
    using Certes.Acme;
    interface IChallengeCtx {
        IChallengeContext ctx { get; set; }
        string txtName { get; set; }
    }
    interface IDnsTxtStore {
        string id { get; set; }
        string name { get; set; }
        string content { get; set; }
    }
    [System.Serializable]
    class DnsTxtStore: IDnsTxtStore {
        public string id { get; set; }
        public string name { get; set; }
        public string content { get; set; }
    }
}
