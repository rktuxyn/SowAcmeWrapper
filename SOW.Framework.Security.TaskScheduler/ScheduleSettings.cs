//12:47 AM 9/20/2018 Rajib
namespace SOW.Framework.Security {
	using System;
    public class ScheduleSettings : IScheduleSettings {
        public string TaskName { get; set; }
        public DateTime TriggerDateTime { get; set; }
        public string Description { get; set; }
        public string ActionPath { get; set; }
        public string Arguments { get; set; }
        public string StartIn { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}