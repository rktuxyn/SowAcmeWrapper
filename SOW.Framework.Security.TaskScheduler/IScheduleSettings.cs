//12:47 AM 9/20/2018 Rajib
namespace SOW.Framework.Security {
	using System;
	public interface IScheduleSettings {
		string TaskName { get; set; }
		DateTime TriggerDateTime { get; set; }
		string Description { get; set; }
		string ActionPath { get; set; }
		string Arguments { get; set; }
		string StartIn { get; set; }
		string UserName { get; set; }
		string Password { get; set; }
	}
}