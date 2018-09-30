//2:55 AM 9/14/2018 Rajib
namespace SOW.Framework.Security.CloudflareWrapper {
    using Newtonsoft.Json;
    using System.Threading.Tasks;
    using System;
    public interface ICFDNS : IDisposable {
        JsonSerializerSettings jsonSettings { get; set; }
        Task<ICFAPIResponse> AddRecord( IQueryConfig qConfig, bool checkExistence = true );
        Task<ICFAPIResponse> ExistsRecord( IQueryConfig qConfig );
        Task<ICFAPIResponse> RemoveRecord( IQueryConfig qConfig );
    }
}
