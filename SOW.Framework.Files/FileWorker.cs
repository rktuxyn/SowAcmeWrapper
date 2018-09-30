using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace SOW.Framework.Files {
    public class FileWorker {
        public FileWorker() { }
        public static void CreateDirectory( string dir ) {
            if (!Directory.Exists( dir )) {
                Directory.CreateDirectory( dir );
            }
        }
        public static string GetDirectoryName( string absolute ) {
            if (string.IsNullOrEmpty( absolute )) return null;
            return Path.GetDirectoryName( absolute );
        }
        public static void DeleteFile( string absolute ) {
            if (!string.IsNullOrEmpty( absolute )) {
                if (File.Exists( absolute )) {
                    File.Delete( absolute );
                }
            }
        }

        public static bool ExistsFile( string absolute ) {
            if (string.IsNullOrEmpty( absolute ))
                return false;
            return ( File.Exists( absolute ) ? true : false );
        }

        public static string Read( string absolute ) {
            string str;
            if (File.Exists( absolute )) {
                str = File.ReadAllText( absolute, Encoding.UTF8 );
            } else {
                str = null;
            }
            return str;
        }

        public static byte[] ReadAllByte( string absolute ) {
            byte[] numArray;
            if (File.Exists( absolute )) {
                numArray = File.ReadAllBytes( absolute );
            } else {
                numArray = null;
            }
            return numArray;
        }

        public static object ReadBinary( string absolute ) {

            if (string.IsNullOrEmpty( absolute )) return null;
            if (!File.Exists( absolute )) {
                return null;
            }
            object obj;
            using (FileStream fileStream = new FileStream( absolute, FileMode.Open, FileAccess.Read )) {
                try {
                    obj = ( new BinaryFormatter( ) ).Deserialize( fileStream );
                } finally {
                    fileStream.Close( );
                    GC.Collect( );
                }
            }
            return obj;
        }

        public static void WriteBinary<T>( T data, string absolute ) {
            if (string.IsNullOrEmpty( absolute )) return;
            if (!File.Exists( absolute )) {
                return;
            }
            using (FileStream fileStream = new FileStream( absolute, FileMode.CreateNew, FileAccess.ReadWrite )) {
                try {
                    ( new BinaryFormatter( ) ).Serialize( fileStream, data );
                    fileStream.Flush( true );
                } finally {
                    GC.Collect( );
                }
            }
        }

        public static void WriteFile( string data, string absolute ) {
            FileWorker.WriteFile( Encoding.UTF8.GetBytes( data ), absolute );
        }

        public static void WriteFile( byte[] buffer, string absolute, bool delete = true ) {
            string directoryName = Path.GetDirectoryName( absolute );
            if (!Directory.Exists( directoryName )) {
                Directory.CreateDirectory( directoryName );
            }
            if (File.Exists( absolute )) {
                File.Move( absolute, string.Concat( absolute, "__old.", Guid.NewGuid( ).ToString( "N" ) ) );
            }
            using (FileStream fileStream = new FileStream( absolute, FileMode.CreateNew, FileAccess.ReadWrite )) {
                try {
                    fileStream.Write( buffer, 0, ( int )buffer.Length );
                    fileStream.Flush( true );
                } finally {
                    GC.Collect( );
                }
            }
        }
    }
}