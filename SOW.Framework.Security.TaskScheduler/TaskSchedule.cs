//12:46 AM 9/20/2018 Rajib
namespace SOW.Framework.Security {
    using System;
    using System.Security.Principal;
    using Microsoft.Win32.TaskScheduler;
    public class TaskSchedule {
        public static void Create( IScheduleSettings settings, ILogger logger ) {
            try {
                TaskService ts = TaskService.Instance;
                TaskDefinition td = ts.NewTask( );
                td.Settings.Enabled = true;
                {
                    Task task = ts.GetTask( settings.TaskName );
                    if (task != null) {

                        var identity = WindowsIdentity.GetCurrent( );
                        var principal = new WindowsPrincipal( identity );
                        // Check to make sure account privileges allow task deletion
                        if (!principal.IsInRole( WindowsBuiltInRole.Administrator ))
                            throw new Exception( $"Cannot delete task with your current identity '{identity.Name}' permissions level." +
                            "You likely need to run this application 'as administrator' even if you are using an administrator account." );
                        // Remove the task we just created
                        ts.RootFolder.DeleteTask( settings.TaskName/*@"\SSL\sitename"*/ );
                    }
                }
                td.RegistrationInfo.Description = settings.Description;
                // Create a trigger that will fire the task at this time every other day
                td.Triggers.Add( new TimeTrigger( ) { StartBoundary = settings.TriggerDateTime } );
                // Create an action that will launch Notepad whenever the trigger fires
                td.Actions.Add( new ExecAction( settings.ActionPath, settings.Arguments, settings.StartIn ) );
                // Register the task in the root folder
                //ts.RootFolder.RegisterTaskDefinition( settings.TaskName, td );
                ts.RootFolder.RegisterTaskDefinition( settings.TaskName, td, TaskCreation.CreateOrUpdate, settings.UserName, settings.Password, TaskLogonType.Password );
            } catch (Exception e) {
                logger.Write( "TaskSchedule.Create Error {0}", e.Message );
                logger.Write( e.StackTrace );
            }

        }
    }
}