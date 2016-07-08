/*
 * 
 * Copyright(C) 2016 - All Rights Reserved 
 * Author: Ali Alatabbi.
 * Email: alialatabbi@gmail.com.
 * You may use, distribute and modify this code,  
 * provided that this copyright notice(s) and this permission notice appear 
 * in all copies of the Software and supporting documentation.
 * 
 */

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using SignalRHub;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace ATNBC
{
    public class progEventArgs : EventArgs
    {
        public int Percentage;
        public string Message { get; set; }
    }

    public class WorkerClass
    {
        
        public string Path2InputFolder { get; set; }
        public string Path2ClassFolder { get; set; }
        public string Path2OutputFolder { get; set; }
        private StringBuilder _result;
        NaiveBayesianClassifier classifier;
        public progEventArgs FinalStatus;
        private int _percentage;
        public string ClientId { get; set; }

        public WorkerClass()
        {
            _result = new StringBuilder();
            FinalStatus = new progEventArgs();
        }

        public void DoWork(CancellationToken cancellationToken)
        {
            try
            {
                if (!string.IsNullOrEmpty(Path2ClassFolder))
                    classifier = new NaiveBayesianClassifier(Path2ClassFolder);
                //var Clients = GlobalHost.ConnectionManager.GetHubContext<JobHub>().Clients;
                DefaultHubManager hd = new DefaultHubManager(GlobalHost.DependencyResolver);
                var hub = hd.ResolveHub("JobHub") as JobHub;
                hub.NotifyTaskStart(ClientId, new progEventArgs { Message = Helper.Alerts["Start"] });
                _result.AppendLine(Helper.Alerts["Start"]);
                int len = 100;
                bool IsCanceled = false;

                ((new DirectoryInfo(Path2InputFolder)).GetFiles("*.txt", SearchOption.AllDirectories)).ToList().ForEach(fi =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        IsCanceled = true;
                        return;
                    }
                    string defaultCategory = classifier.GuessMostProbableClass(fi);
                    Thread.Sleep(100);
                    //_result.AppendLine("item" + (i + 1).ToString() + " processing completed...");
                    _percentage += (int)((1.0 / len) * 100);
                    hub.NotifyProgress(ClientId, new progEventArgs { Message = fi.Name + " ->" + defaultCategory });
                    _result.AppendLine("<li><strong>" + fi.Name + ": " + defaultCategory + "</strong>   </li>");

                });
                if (IsCanceled)
                {
                    hub.NotifyTaskCancel(ClientId, new progEventArgs { Message = Helper.Alerts["Cancel"] });
                    _result.AppendLine(Helper.Alerts["Cancel"]);
                }
                else
                {
                    hub.NotifyTaskEnd(ClientId, new progEventArgs { Message = Helper.Alerts["End"] });
                    _result.AppendLine(Helper.Alerts["End"]);
                }
                FinalStatus.Percentage = 100;
                FinalStatus.Message = _result.ToString();
                _result.Clear();
            }
            catch (System.Exception ex)
            {
                FinalStatus.Message = Helper.Alerts["Error"];
            }
        }
    }
}