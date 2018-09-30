//8:18 PM 9/13/2018 Rajib
namespace SOW.Framework.Security.CloudflareWrapper.Http {
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    interface IWebHttp : IDisposable {
        Task<IWebHttpResponse> GetAsync( string requestUri, Dictionary<string, string> header = null );
        Task<IWebHttpResponse> PostAsync( string requestUri, string postJson, Dictionary<string, string> header = null );
    }
}
