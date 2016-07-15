/*
The MIT License (MIT)

Copyright(C) 2016 - All Rights Reserved.
Author: Ali Alatabbi.
Email: alialatabbi@gmail.com.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */

 using ATNBC;
using ATNBC.ViewModel;
using Microsoft.AspNet.SignalR;
using SignalRHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SignalRChat.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Session["JobId"] = "";
            JobVM jvm = new JobVM { InputPath = Server.MapPath("~/App_data/Input/"), OutputPath = Server.MapPath("~/App_data/Output") };
            return View(jvm);
        }
        public ActionResult Run(JobVM jvm, string submit)
        {
            if (submit == "Start")
                return Start(jvm);
            if (submit == "Stop")
                return Stop(jvm);
            return View("Error");
        }
        public ActionResult Start(JobVM jvm)
        {
            jvm.InputPath = Server.MapPath("~/App_data/Input/");
            Session["JobId"] = "1234";
            var myWorker = new WorkerClass {ClientId= jvm.ClientId};
            myWorker.Path2InputFolder = Server.MapPath("~/App_data/Input/" + jvm.InputFolder);
            myWorker.Path2ClassFolder = Server.MapPath("~/App_data/Classes");
            myWorker.Path2OutputFolder = Server.MapPath("~/App_data/Output/" + jvm.OutputFolder);
            var TokenSrc = new CancellationTokenSource();
            CancellationToken ct = TokenSrc.Token;
            bool IsWorking = true;
            string m = "";
            Task task = new Task(() =>
            {
                myWorker.DoWork(ct);
                IsWorking = false;
                
            }, ct);
            task.Start();
            while (IsWorking)
            {
                if (!Response.IsClientConnected)
                {
                    TokenSrc.Cancel();
                    break;
                }
            }
            jvm.Message = myWorker.FinalStatus.Message;

            return View("Index",jvm);
        }

        public ActionResult Stop(JobVM jvm)
        {
            jvm.InputPath = Server.MapPath("~/App_data/Input/");
            jvm.Message = Helper.Alerts["Cancel"];
            return View("Index", jvm);
            
        }

        public ActionResult About()
        {
            ViewBag.Message = "";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "";

            return View();
        }

    }
}