//2:55 AM 9/14/2018 Rajib
namespace SOW.Framework.Security.CloudflareWrapper {
    public interface IQueryConfig {
        string DOMAIN_NAME { get; set; }
        CFRecordType RECORD_TYPE { get; set; }
        string RECORD_ID { get; set; }
        string NAME { get; set; }
        string RECORD_NAME { get; set; }
        string RECORD_CONTENT { get; set; }
    }
}
