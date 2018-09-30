//2:55 AM 9/14/2018 Rajib
namespace SOW.Framework.Security.CloudflareWrapper {
    public class QueryConfig: IQueryConfig {
        public string DOMAIN_NAME { get; set; }
        public CFRecordType RECORD_TYPE { get; set; }
        public string RECORD_ID { get; set; }
        public string NAME { get; set; }
        public string RECORD_NAME { get; set; }
        public string RECORD_CONTENT { get; set; }
    }
}
