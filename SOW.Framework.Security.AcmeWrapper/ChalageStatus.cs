//10:50 PM 9/14/2018 Rajib
namespace SOW.Framework.Security.LetsEncrypt {
    class ChalageStatus : IChalageStatus {
        public bool status { get; set; }
        public string errorDescription { get; set; }
    }
}
