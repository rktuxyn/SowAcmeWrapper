//8:18 PM 9/13/2018 Rajib
namespace SOW.Framework.Security.CloudflareWrapper.Http {
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Linq;
    using System.Text;
    using System.IO.Compression;
    using System.IO;

    class WebHttp :IWebHttp {
        string _baseAddress { get; set; }
        HttpClient _client { get; set; }
        public WebHttp(string baseAddress ) {
            _baseAddress = baseAddress;
            _client = new HttpClient( );
            _client.Timeout = TimeSpan.FromMinutes( 10 );
            ServicePointManager.ServerCertificateValidationCallback += ( ( sender, certificate, chain, sslPolicyErrors ) => true );
        }
        public async Task<IWebHttpResponse> GetAsync( string requestUri, Dictionary<string, string> header = null ) {
            return await this.AssignHttpClientAsync( requestUri: requestUri, header: header, method: "GET" );
        }
        public async Task<IWebHttpResponse> PostAsync( string requestUri, string postJson, Dictionary<string, string> header = null ) {
            return await this.AssignHttpClientAsync( requestUri: requestUri, postJson: postJson, method: "POST", header: header );
        }
        private async Task<IWebHttpResponse> AssignHttpClientAsync( string requestUri, Dictionary<string, string> header = null, string postJson = null, string method = "POST" ) {
            try {
                if (method == "POST") {
                    if (null == postJson) {
                        return new WebHttpResponse {
                            status = WebHttpStatus.NOT_ASSIGN,
                            errorDescription = "Post data required !!!"
                        };
                    }
                }
                string responseText = string.Empty;
                using (HttpRequestMessage hreq = new HttpRequestMessage( )) {
                    hreq.RequestUri = new Uri( string.Concat( _baseAddress, requestUri ) );
                    hreq.Headers.Add( "Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8" );
                    hreq.Headers.Add( "accept-encoding", "gzip, deflate" );
                    if (header != null) {
                        header.Select( a => {
                            hreq.Headers.Add( a.Key, a.Value );
                            return a;
                        } ).ToList( );
                        header.Clear( );
                    }
                    if (method == "POST") {
                        hreq.Method = HttpMethod.Post;
                        hreq.Content = new StringContent( postJson, Encoding.UTF8, "application/json" );
                    } else {
                        hreq.Method = HttpMethod.Get;
                        //hreq.Headers.Add( "Content-type", "application/json" );
                        //hreq.Content = new StringContent( string.Empty, Encoding.UTF8, "application/json" );
                    }
                    using (HttpResponseMessage hrm = await _client.SendAsync( hreq )) {
                        responseText = await this.ReadContentAsStringAsync( hrm );
                    }
                }
                return new WebHttpResponse {
                    status = WebHttpStatus.SUCCESS,
                    responseText = responseText
                };
            } catch (HttpRequestException e) {
                return new WebHttpResponse {
                    status = WebHttpStatus.ERROR,
                    responseText = null,
                    errorDescription = e.InnerException.Message
                };
            }
        }
        private async Task<IWebHttpResponse> AssignWebRequestAsync( string requestUri,Dictionary<string, string> header = null, string postJson= null, string method = "POST" ) {
            try {
                if (method == "POST") {
                    if (null == postJson) {
                        return new WebHttpResponse {
                            status = WebHttpStatus.NOT_ASSIGN,
                            errorDescription = "Post data required !!!"
                        };
                    }
                }
                HttpWebRequest req = ( HttpWebRequest )WebRequest.Create( string.Concat( _baseAddress, requestUri ) );
                req.ServerCertificateValidationCallback += ( ( sender, certificate, chain, sslPolicyErrors ) => true );
                req.Timeout = 5 * 1000;//5s
                req.Method = method;
                {
                    WebHeaderCollection hc = new WebHeaderCollection( );
                    hc.Add( "accept-encoding", "gzip, deflate" );
                    if (header != null) {
                        header.Select( a => {
                            hc.Add( a.Key, a.Value );
                            return a;
                        } ).ToList();
                        //header.Clear( );
                    }
                    req.Headers = hc;
                }
                if (method == "POST") {
                    req.ContentType = "application/json, multipart/form-data";
                    //write to a byte array
                    byte[] buffer = Encoding.UTF8.GetBytes( postJson );
                    req.ContentLength = buffer.Length;
                    using (Stream reqStream = req.GetRequestStream( )) {
                        await reqStream.WriteAsync( buffer, 0, buffer.Length );
                    }
                }
                string responseText = string.Empty;
                using (WebResponse resp = await req.GetResponseAsync( )) {
                    responseText = await this.ReadContentAsStringAsync( resp );
                }
                req.Abort( );
                return new WebHttpResponse {
                    status = WebHttpStatus.SUCCESS,
                    responseText = responseText
                };
            } catch (Exception e) {
                return new WebHttpResponse {
                    status = WebHttpStatus.ERROR,
                    responseText = null,
                    errorDescription = e.Message
                };
            }
        }
        private async Task<string> ReadContentAsStringAsync( WebResponse response ) {
            // Check whether response is compressed.
            if (response.Headers.AllKeys.Any( x => x == "Content-Encoding" )) {
                if (response.Headers["Content-Encoding"] == "gzip") {
                    // Decompress manually
                    using (var s = response.GetResponseStream( )) {
                        using (var decompressed = new GZipStream( s, CompressionMode.Decompress )) {
                            using (var rdr = new StreamReader( decompressed )) {
                                return await rdr.ReadToEndAsync( );
                            }
                        }
                    }
                }
            }
            // Use standard implementation if not compressed
            using (StreamReader sr = new StreamReader( response.GetResponseStream( ) )) {
                return await sr.ReadToEndAsync( );
            }
        }
        private async Task<string> ReadContentAsStringAsync( HttpResponseMessage response ) {
            // Check whether response is compressed
            if (response.Content.Headers.ContentEncoding.Any( x => x == "gzip" )) {
                // Decompress manually
                using (var s = await response.Content.ReadAsStreamAsync( )) {
                    using (var decompressed = new GZipStream( s, CompressionMode.Decompress )) {
                        using (StreamReader sr = new StreamReader( decompressed )) {
                            return await sr.ReadToEndAsync( );
                        }
                    }
                }
            } else
                // Use standard implementation if not compressed
                return await response.Content.ReadAsStringAsync( );
        }

        public void Dispose() {
            _client.Dispose( );
            _client = null;
            _baseAddress = null;
            GC.Collect( 0, GCCollectionMode.Optimized );
        }
    }
}
