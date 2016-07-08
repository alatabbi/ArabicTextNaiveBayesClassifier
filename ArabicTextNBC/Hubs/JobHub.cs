using System;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading.Tasks;
using ATNBC;

namespace SignalRHub
{
    [HubName("jobHub")]
    public class JobHub : Hub
    {
        public void NotifyTaskStart(string clientId, progEventArgs args)
        {
            var Clients = GlobalHost.ConnectionManager.GetHubContext<JobHub>().Clients;
            Clients.Client(clientId).notifyStart(args.Message);
        }
        public void NotifyProgress(string clientId, progEventArgs args)
        {
            var Clients = GlobalHost.ConnectionManager.GetHubContext<JobHub>().Clients;
            Clients.Client(clientId).notifyProgress(args.Message);
        }
        public void NotifyTaskEnd(string clientId, progEventArgs args)
        {
            var Clients = GlobalHost.ConnectionManager.GetHubContext<JobHub>().Clients;
            Clients.Client(clientId).notifyEnd(args.Message);
        }
        public void NotifyTaskCancel(string clientId, progEventArgs args)
        {

            var Clients = GlobalHost.ConnectionManager.GetHubContext<JobHub>().Clients;
            Clients.Client(clientId).notifyCancel(args.Message);
        }


    }
}