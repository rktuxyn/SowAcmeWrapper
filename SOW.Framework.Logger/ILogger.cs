
namespace SOW.Framework {
    public interface ILogger {
        ILogger Open( string physicalPath = null );
        byte[] GetCurLog();
        void Write( byte[] buffer );
        void Write( string data );
        void Write( params string[] data );
        void Close();
    }
}
