//3:16 AM 9/14/2018 Rajib
namespace SOW.Framework.Security.CloudflareWrapper {
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Newtonsoft.Json;
    using SOW.Framework.Security.CloudflareWrapper.Http;
    using System.Threading.Tasks;
   
    public class CFDNS: ICFDNS {
        IWebHttp _webHttp { get; set; }
        ICFConfig _config { get; set; }
        ILogger _logger { get; set; }
        public JsonSerializerSettings jsonSettings { get; set; }
        public CFDNS( ICFConfig config, ILogger logger ) {
            _config = config;
            _logger = logger;
            _webHttp = new WebHttp( _config.CF_API );
            jsonSettings = new JsonSerializerSettings {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };
        }
        private string GetRecordText( CFRecordType cFRecord) {
            switch (cFRecord) {
                case CFRecordType.A: return "A";
                case CFRecordType.AAA: return "AAA";
                case CFRecordType.CNAME: return "CNAME";
                case CFRecordType.MX: return "MX";
                case CFRecordType.LOC: return "LOC";
                case CFRecordType.SRV: return "SRV";
                case CFRecordType.SPF: return "SPF";
                case CFRecordType.TXT: return "TXT";
                case CFRecordType.NS: return "NS";
                case CFRecordType.CAA: return "CAA";
                default:throw new Exception( "Invalid Record type defined!!!" );
            }
        }
        private Dictionary<object, object> GetData( IQueryConfig qConfig ) {
            return new Dictionary<object, object>( ) {
                { "type", this.GetRecordText(qConfig.RECORD_TYPE) },
                { "name", qConfig.RECORD_NAME },
                { "content", qConfig.RECORD_CONTENT },
                { "ttl", 120}
            };
        }
        private Dictionary<string, string> GetHeader(  ) {
            return new Dictionary<string, string>( ) {
                { "X-Auth-Email", _config.CF_AUTH_EMAIL },
                { "X-Auth-Key", _config.CF_AUTH_KEY }
            };
        }
        public async Task<ICFAPIResponse> ExistsRecord( IQueryConfig qConfig ) {
            string requestUriString = string.Format( "{0}zones/{1}/dns_records?type={2}&name={3}&content={4}", _config.CF_URI, _config.CF_DNS_ZONE, qConfig.RECORD_TYPE, qConfig.NAME, qConfig.RECORD_CONTENT );
            IWebHttpResponse resp = await _webHttp.GetAsync( requestUriString, this.GetHeader( ) );
            if (resp.status != WebHttpStatus.SUCCESS) {
                _logger.Write( "Error occured while checking {0} record for {1} . Error=>", qConfig.RECORD_NAME, qConfig.DOMAIN_NAME, resp.errorDescription );
                return new CFAPIResponse {
                    success = false, errors = new object[] { resp.errorDescription }
                };
            }
            if (string.IsNullOrEmpty( resp.responseText )) {
                return new CFAPIResponse {
                    success = false, errors = new object[] { "No Response found from API!!!" }
                };
            }
            ICFAPIResponse cFAPIResponse = JsonConvert.DeserializeObject<CFAPIResponse>( resp.responseText, jsonSettings );
            if (cFAPIResponse.result == null) {
                return new CFAPIResponse {
                    success = false, errors = new object[] { "No Response found from API!!!" }
                };
            }
            if (cFAPIResponse.result is Newtonsoft.Json.Linq.JArray) {
                Newtonsoft.Json.Linq.JArray rs = ( Newtonsoft.Json.Linq.JArray )cFAPIResponse.result;
                if (rs.Count <= 0) {
                    return new CFAPIResponse {
                        success = false, errors = new object[] { "Not Exists!!!" }
                    };
                }
                _logger.Write( "{0} record already exists in {1}", qConfig.RECORD_NAME, qConfig.DOMAIN_NAME );
            }
            return cFAPIResponse;
        }
        public async Task<ICFAPIResponse> RemoveRecord( IQueryConfig qConfig ) {
            try {
                ICFAPIResponse aPI = await this.ExistsRecord( qConfig );
                if (aPI.success != true) {
                    return new CFAPIResponse {
                        success = true, messages = new object[] { "Not Exists this record!!!" }
                    };
                }
                throw new NotImplementedException( "!TODO" );
            } catch (Exception e) {
                _logger.Write( "Error occured Remove DNS TXT Record for {0} . Error=> {1}", qConfig.DOMAIN_NAME, e.Message );
                _logger.Write( e.StackTrace );
                return new CFAPIResponse {
                    success = true, messages = new object[] { e.Message }
                };
            }
        }
        public async Task<ICFAPIResponse> AddRecord( IQueryConfig qConfig, bool checkExistence = true ) {
            if (qConfig.RECORD_TYPE != CFRecordType.TXT)
                throw new NotImplementedException( "Not Implemented Yet!!!" );
            if (checkExistence) {
                ICFAPIResponse aPI = await this.ExistsRecord( qConfig );
                if (aPI.success == true) {
                    return new CFAPIResponse {
                        success = true, messages = new object[] { "Exists this record!!!" }
                    };
                }
            }
            _logger.Write( "Adding DNS Record for {0}, Record type {1}", qConfig.DOMAIN_NAME, this.GetRecordText( qConfig.RECORD_TYPE ) );
            IWebHttpResponse resp = await _webHttp.PostAsync( string.Format( "{0}zones/{1}/dns_records", _config.CF_URI, _config.CF_DNS_ZONE ), JsonConvert.SerializeObject( this.GetData( qConfig ), jsonSettings ), this.GetHeader() );
            if (resp.status != WebHttpStatus.SUCCESS) {
                _logger.Write( "Error occured while add {0} record for {1} . Error=> {2}", qConfig.RECORD_NAME, qConfig.DOMAIN_NAME, resp.errorDescription );
                return new CFAPIResponse {
                    success = false, errors = new object[] { resp.errorDescription }
                };
            }
            if (string.IsNullOrEmpty( resp.responseText )) {
                _logger.Write( "Error occured while add {0} record for {1} . Error=> No Response found from API!!!", qConfig.RECORD_NAME, qConfig.DOMAIN_NAME );
                return new CFAPIResponse {
                    success = false, errors = new object[] { "No Response found from API!!!" }
                };
            }
            return JsonConvert.DeserializeObject<CFAPIResponse>( resp.responseText, jsonSettings );
            
        }
        private static string GetIPAddress() {
            WebClient webClient = new WebClient( );
            return webClient.DownloadString( "https://icanhazip.com/" );
        }
        public void Dispose() {
            _config = null;
            _webHttp.Dispose( );
            GC.SuppressFinalize( this );
            GC.Collect( 0, GCCollectionMode.Optimized );
        }
    }
}
