//8:18 PM 9/13/2018 Rajib
namespace SOW.Framework.Security.CloudflareWrapper.Http {
    interface IWebHttpResponse {
        WebHttpStatus status { get; set; }
        string errorDescription { get; set; }
        string responseText { get; set; }
    }
}
