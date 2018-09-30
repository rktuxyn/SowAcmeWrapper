using System;
using System.Collections.Generic;
using System.Text;

namespace SOW.Framework.Security.CloudflareWrapper {
    interface IPublicIPProvider {
        bool exit { get; set; }
        string GetIp();
    }
}
