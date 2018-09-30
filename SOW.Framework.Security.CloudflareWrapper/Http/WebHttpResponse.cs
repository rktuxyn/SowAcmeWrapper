//8:18 PM 9/13/2018 Rajib
namespace SOW.Framework.Security.CloudflareWrapper.Http {
    class WebHttpResponse : IWebHttpResponse {
        public WebHttpStatus status { get; set; }
        public string errorDescription { get; set; }
        public string responseText { get; set; }
    }
}
