
namespace SOW.Framework {
    using System;
    using System.IO;
    using System.Text;
    public class Logger : ILogger {
        MemoryStream _ms { get; set; }
        string _physicalPath { get; set; }
        bool _closed = false;
        string _dir { get; set; }
        bool _iSUserInteractive { get; set; }
        public Logger() {
            _iSUserInteractive = Environment.UserInteractive;
        }
        public byte[] GetCurLog() {
            if (_ms == null || _ms.CanRead == false) return null;
            return _ms.ToArray( );
        }
        private void WriteToFile() {
            Stream fs;
            try {
                if (File.Exists( _physicalPath )) {
                    fs = new FileStream( _physicalPath, FileMode.Append, FileAccess.Write, FileShare.Read );
                } else {
                    fs = new FileStream( _physicalPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite );
                    byte[] grettings = Encoding.UTF8.GetBytes( string.Concat( "Log Generated on ", DateTime.Now.ToString( ), "\r\n------------------------------------------------------\r\n" ) );
                    fs.Write( grettings, 0, grettings.Length );
                }
            } catch {
                _physicalPath = string.Concat( _physicalPath, "__", Guid.NewGuid( ).ToString( "N" ) );
                System.Threading.Thread.Sleep( 100 );
                WriteToFile( );
                return;
            }
            byte[] bytes = _ms.ToArray();
            fs.Write( bytes, 0, bytes.Length );
            _ms.Close( );
            fs.Flush( );
            _ms.Flush( );
            _ms = null;
        }
        public ILogger Open( string physicalPath = null ) {
            if (!string.IsNullOrEmpty( physicalPath )) {
                _physicalPath = physicalPath;
                _dir = Path.GetDirectoryName( physicalPath );
            }
            if (string.IsNullOrEmpty( _physicalPath ) || string.IsNullOrEmpty( _dir ))
                throw new ArgumentNullException( "Physical path required" );
            
            if (!Directory.Exists( _dir )) {
                Directory.CreateDirectory( _dir );
            }
            try {
                _ms = new MemoryStream( );
                /*if (File.Exists( physicalPath )) {
                    _fs = new FileStream( physicalPath, FileMode.Append, FileAccess.Write, FileShare.Read );
                } else {
                    _fs = new FileStream( physicalPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite );
                }*/
            } catch {
                System.Threading.Thread.Sleep( 100 );
                if (!string.IsNullOrEmpty( physicalPath ))
                    return Open( string.Concat( physicalPath, "__", Guid.NewGuid( ).ToString( "N" ) ) );
            }
            return this;
        }
        public void Write( params string[] data ) {
            if (data == null) return;
            if (data.Length <= 0) return;
            Write( string.Format( data[0], data.Slice( 1, data.Length ) ) );
        }
        public void Write( byte[] buffer ) {
            if (_ms == null || !_ms.CanWrite || _closed) {
                Open( );
                _closed = false;
            }
            lock (_ms) {
                _ms.Write( buffer, 0, buffer.Length );
            }
        }
        public void Write( string data ) {
            //data = data.Replace( "tripecosys.com", "mydomain.com" ).Replace( "vssl.com.bd", "mydomain.com" );
            //data = string.Concat( DateTime.Now.ToString( ), "\t\t", data, "\r\n" );
            if (_iSUserInteractive)
                Console.WriteLine( data );
            //Write( Encoding.UTF8.GetBytes( data ) );
            Write( Encoding.UTF8.GetBytes( string.Concat( DateTime.Now.ToString( ), "\t\t", data, "\r\n" ) ) );
        }
        public void Close() {
            if (_closed) return;
            WriteToFile( );
            //_ms.Flush( ); _ms = null;
            _closed = true;
            GC.Collect( );
        }
    }
    public static class Extensions {
        /// <summary>
        /// Get the array slice between the two indexes.
        /// ... Inclusive for start index, exclusive for end index.
        /// </summary>
        public static T[] Slice<T>( this T[] source, int start, int end ) {
            // Handles negative ends.
            if (end < 0) {
                end = source.Length + end;
            }
            int len = end - start;

            // Return new array.
            T[] res = new T[len];
            for (int i = 0; i < len; i++) {
                res[i] = source[i + start];
            }
            return res;
        }
    }
}
