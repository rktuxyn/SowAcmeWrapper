//8:32 PM 9/15/2018 Rajib
namespace SOW.Framework.Security.LetsEncrypt {
    using System;
    using System.IO;
    using System.Text;
    using Newtonsoft.Json;
    sealed class Global {
        public static readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented
        };
        static string _APP_DIR { get; set; }
        public static string APP_DIR {
            get {
                if (string.IsNullOrEmpty( _APP_DIR )) {
                    var _assembly = System.Reflection.Assembly.GetEntryAssembly( ).Location;
                    _APP_DIR = Path.GetDirectoryName( _assembly );
                }
                return _APP_DIR;
            }
            set {
                _APP_DIR = value;
            }
        }
        public static IGConfig gConfig { get; set; }
        public static void Load( string physicalPath ) {
            if (!File.Exists( physicalPath ))
                throw new Exception( string.Format( "No Settings file found in {0}", physicalPath ) );
            string data = File.ReadAllText( physicalPath, Encoding.UTF8 );
            if (string.IsNullOrEmpty( data ))
                throw new Exception( string.Format( "No data found in {0}", physicalPath ) );
            try {
                gConfig = JsonConvert.DeserializeObject<GConfig>( data, jsonSettings );
            } catch (Exception e) {
                Console.WriteLine( e.Message );
            }
        }
        public static string RegisterNewDirectory( string threshold, string dir = null ) {
            if (string.IsNullOrEmpty( dir ))
                dir = string.Format( @"{0}\AcmeWrapper\info\", APP_DIR );
            if (!Directory.Exists( dir )) {
                Directory.CreateDirectory( dir );
            }
            dir = string.Format( @"{0}{1}\", dir, threshold.Replace( "@", "_" ).Replace( ".", "_" ).Replace( "*", "_" ) );
            if (!Directory.Exists( dir )) {
                Directory.CreateDirectory( dir );
            }
            return dir;
        }
    }
}