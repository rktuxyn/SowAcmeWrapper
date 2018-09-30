//10:50 PM 9/14/2018 Rajib
namespace SOW.Framework.Security.LetsEncrypt {
    interface IChalageStatus {
        bool status { get; set; }
        string errorDescription { get; set; }
    }
}
