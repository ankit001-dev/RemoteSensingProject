using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace RemoteSensingProject.Models
{
    public class RealtimeDateTime : Hub
    {
        public void SendRealTimeDateTime()
        {
            string currentDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Clients.All.receiveTime(currentDateTime);  // Broadcasting the time to all connected clients
        }
    }
}