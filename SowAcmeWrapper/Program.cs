using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using SOW.Framework.Security;
using SOW.Framework.Security.LetsEncrypt;
//https://download.microsoft.com/download/D/5/9/D593CD8F-04E7-425D-962C-86FF4C90B1DA/dotnet-sdk-2.2.100-preview2-009404-win-x64.exe
namespace SowAcmeWrapper {
    using SOW.Framework;
    //using System.IO;
    using SOW.Framework.Files;
    class Arguments {
        public string email { get; set; }
        public string web { get; set; }
        public bool forceRenew { get; set; }
        public string acmeApiServer { get; set; }
    }
    class Program {
        static ILogger _logger { get; set; }
        static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource( );
        private static bool ConsoleCtrlCheck( CtrlTypes ctrlType ) {
            _logger.Write( "Program being closed!" );
            _logger.Close( );
            if ( _cancellationTokenSource.IsCancellationRequested ) return true;
            _cancellationTokenSource.Cancel( );
            return true;
        }
        static Arguments GetArguments( string[] args ) {
            if ( args == null || args.Length == 0 ) {
                _logger.Write( "Null Argument..." );
                PrintCommand( );
                return null;
            };
            try {
                Arguments arg = new Arguments { };
                for ( int i = 0, l = args.Length; i < l; i++ ) {
                    string key = args[i];
                    if ( string.IsNullOrEmpty( key ) ) continue;
                    key = key.Replace( "-", "" ).ToUpper( );
                    if ( key == "H" ) {
                        PrintCommand( true );
                        return null;
                    }
                    string value = args[i + 1];
                    switch ( key ) {
                        case "E":
                            arg.email = value == null ? value : value.Trim( ).ToLower( );
                            break;
                        case "W":
                            arg.web = value == null ? value : value.Trim( ).ToLower( );
                            break;
                        case "FR":
                            arg.forceRenew = value == null ? false : value.Trim( ).ToUpper( ) == "Y" ? true : false;
                            break;
                        case "API":
                            arg.acmeApiServer = value;
                            break;
                        default:
                            _logger.Write( "Invalid Argument {0} defined...", key );
                            PrintCommand( );
                            return null;
                    }
                    i = i + 1;
                    if ( i > l ) {
                        _logger.Write( "Invalid Argument defined..." );
                        PrintCommand( );
                        return null;
                    }
                }
                return arg;
            } catch {
                PrintCommand( );
                return null;
            }

        }
        static void PrintCommand( bool isHelp = false ) {
            if ( !Environment.UserInteractive ) return;
            Console.WriteLine( "------------------------------" );
            if ( !isHelp ) {
                Console.WriteLine( "-h help --> Show Command Help" );
            }
            Console.WriteLine( "-api Acme API server. e.g. LetsEncryptV2" );
            Console.WriteLine( "-e myemail@mydomain.com --> Authorization Email" );
            Console.WriteLine( "-w *.mydomain.com --> Need Generate Certificate" );
            Console.WriteLine( "-fr Y/N ---> Force Renew" );
            Console.WriteLine( "------------------------------" );
        }
        static void Exit( ) {
            ConsoleCtrlCheck( CtrlTypes.EVENT_EXIT );
            Environment.Exit( Environment.ExitCode );
        }
        static void Main( string[] args ) {
            var dir = FileWorker.GetDirectoryName( System.Reflection.Assembly.GetEntryAssembly( ).Location );
            _logger = new Logger( );
            _logger.Open( string.Format( @"{0}\log\{1}.log", dir, DateTime.Now.ToString( "yyyy'-'MM'-'dd" ) ) );
            _logger.Write( "------------------------------" );
            _logger.Write( "Opening..." );
            Arguments arguments = GetArguments( args );
            if ( arguments == null ) {
                Exit( );
                return;
            }
            if ( string.IsNullOrEmpty( arguments.email ) || string.IsNullOrEmpty( arguments.web ) ) {
                PrintCommand( );
                Exit( );
                return;
            }
            if ( Environment.UserInteractive ) {
                SetConsoleCtrlHandler( new HandlerRoutine( ConsoleCtrlCheck ), true );
                Console.WriteLine( "CTRL+C,CTRL+BREAK or suppress the application to exit" );
            }

            Thread th = new Thread( ( ) => {
                Task.Run( async ( ) => {
                    try {
                        //6:10 AM 9/14/2018 Rajib
                        //IConfig config = AcmeWrapper.GetConfig( "mssclang@outlook.com" );
                        //IAcmeWrapper acmeWrapper = new AcmeWrapper( config, "*.tripecosys.com", CancellationToken.None );
                        IConfig config = AcmeWrapper.GetConfig( email: arguments.email, dir: dir );
                        if ( config == null ) {
                            _logger.Write( string.Format( "No config found for email==>{0}", arguments.email ) );
                            goto EXIT;
                        }
                        using ( IAcmeWrapper acmeWrapper = new AcmeWrapper( config: config, web: arguments.web, acmeApiServer: arguments.acmeApiServer ?? AcmeWrapper.DefaultAcmeApiServer, logger: _logger, ct: _cancellationTokenSource.Token ) ) {
                            X509Certificate2 cert = null;
                            IIISWrapper iISWrapper = new IISWrapper( );
                            if ( arguments.forceRenew == false ) {
                                _logger.Write( "Check if valid Certificate exists for {0}", acmeWrapper.domain.ZoneName );
                                if ( acmeWrapper.ExistsCertificate( ) ) {
                                    ICertificate cer = acmeWrapper.GetCertificate( );
                                    if ( !cer.isExpired && cer.status ) {
                                        var (status, resp) = acmeWrapper.IsValidConfig( logging: true );
                                        if ( !status ) {
                                            goto EXIT;
                                        }
                                        cert = cer.Cert;
                                        _logger.Write( "A valid Certificate found for {0}; Cert Serial:: {1}", acmeWrapper.domain.ZoneName, cert.SerialNumber );
                                    }
                                }

                            }
                            if ( cert == null ) {
                                IOrderResult orderResult = await acmeWrapper.CreateOrRenewCert( forceRenew: arguments.forceRenew, rec: acmeWrapper.MAX_TRY );
                                if ( orderResult.success != true ) {
                                    goto EXIT;
                                }
                                if ( orderResult.taskType == TaskType.DOWNLOAD_CERT ) {
                                    ICertificate certificate = await acmeWrapper.CreateCertificate( );
                                    if ( certificate.status == false ) {
                                        goto EXIT;
                                    }
                                    cert = certificate.Cert;
                                    if ( !iISWrapper.InstallCertificate( certificate: cert, zoneName: acmeWrapper.domain.ZoneName, oldCertificate: orderResult.oldCertificate, storeName: StoreName.My, logger: _logger ) )
                                        goto EXIT;
                                } else if ( orderResult.taskType == TaskType.INSTALL_CERT ) {
                                    cert = orderResult.oldCertificate;
                                    if ( !iISWrapper.InstallCertificate( certificate: cert, zoneName: acmeWrapper.domain.ZoneName, storeName: StoreName.My, logger: _logger ) )
                                        goto EXIT;
                                } else {
                                    goto EXIT;
                                }
                                orderResult = null;
                            } else {
                                if ( !iISWrapper.InstallCertificate( certificate: cert, zoneName: acmeWrapper.domain.ZoneName, storeName: StoreName.My, logger: _logger ) )
                                    goto EXIT;
                            }

                            _logger.Write( "Bind Certificate for {0}", acmeWrapper.domain.ZoneName );
                            IIISWrapperResponse iSWrapperResponse = iISWrapper.BindCertificate( iISSettings: new IISWrapperSettings {
                                SiteName = acmeWrapper.domain.ZoneName,
                                CertificateStoreName = "My",
                                AppPool = acmeWrapper.domain.IISSettings.AppPool,
                                CertificateHash = cert.GetCertHash( ),
                                Site = acmeWrapper.domain.IISSettings.Site,
                                ZoneName = acmeWrapper.domain.ZoneName
                            }, logger: _logger );
                            if ( iSWrapperResponse.Error ) goto EXIT;
                            _logger.Write( "Certificate process completed for {0}", acmeWrapper.domain.ZoneName );
                            IWinConfig winConfig = acmeWrapper.gConfig.winConfig;
                            string batAbsolute = string.Concat( acmeWrapper.domainDir, "ScheduleRunner.bat" );
                            if ( FileWorker.ExistsFile( batAbsolute ) ) {
                                FileWorker.DeleteFile( batAbsolute );
                            }
                            _logger.Write( "Creating .bat file for Schedule runner for {0}", acmeWrapper.domain.ZoneName );
                            string bat = string.Format( "rem Auto Generated on {0}\r\n", DateTime.Now.ToString( ) );
                            bat += string.Format( "dotnet LetsEncryptWrapper.dll -e {0} -w {1} -fr Y", config.Email, acmeWrapper.domain.DomainName );
                            FileWorker.WriteFile( bat, batAbsolute );
                            _logger.Write( ".bat file creation completed for Schedule runner for {0}", acmeWrapper.domain.ZoneName );

                            _logger.Write( "Register new Task Schedule for Renew the Certificate for {0}", acmeWrapper.domain.ZoneName );

                            TaskSchedule.Create( settings: new ScheduleSettings {
                                TaskName = string.Format( @"\LetsEncryptWrapper\SSL\{0}", acmeWrapper.domain.ZoneName ),
                                TriggerDateTime = cert.NotAfter,/**DateTime.Now.AddDays( 1 ),*/
                                Description = string.Format( "Register new Task Schedule for Renew the Certificate for {0}; Cert Serail::{1}", acmeWrapper.domain.ZoneName, cert.SerialNumber ),
                                ActionPath = batAbsolute,//@"dotnet LetsEncryptWrapper.dll",
                                Arguments = null,//string.Format( "-e {0} -w {1} -fy Y", config.Email, acmeWrapper.domain.DomainName ),
                                UserName = winConfig.WinUser,
                                Password = winConfig.WinPassword,
                                StartIn = dir
                            }, logger: _logger );

                            _logger.Write( "Task Schedule Registration completed for {0}", acmeWrapper.domain.ZoneName );
                        }
                        goto EXIT;
                    } catch ( Exception e ) {
                        _logger.Write( e.Message );
                        _logger.Write( e.StackTrace );
                        goto EXIT;
                    }
                EXIT:
                    //_logger.GetCurLog( );
                    //Send Confirmation
                    Exit( );

                } ).Wait( );
            } );
            th.SetApartmentState( ApartmentState.STA );
            th.Start( );
            do {
                Thread.Sleep( 1000 );
            } while ( !_cancellationTokenSource.IsCancellationRequested );
            th.Join( );
            Environment.Exit( Environment.ExitCode );
            return;
        }
        #region unmanaged
        // Declare the SetConsoleCtrlHandler function
        // as external and receiving a delegate.

        [DllImport( "Kernel32" )]
        public static extern bool SetConsoleCtrlHandler( HandlerRoutine Handler, bool Add );

        // A delegate type to be used as the handler routine
        // for SetConsoleCtrlHandler.
        public delegate bool HandlerRoutine( CtrlTypes CtrlType );

        // An enumerated type for the control messages
        // sent to the handler routine.

        public enum CtrlTypes {
            EVENT_EXIT = -1,
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        #endregion
    }
}
