/****************************** Module Header ******************************\
* Module Name:  SampleService.cs
* Project:      GreyhoundsUpdateService
* Copyright (c) Microsoft Corporation.
* 
* Provides a sample service class that derives from the service base class - 
* System.ServiceProcess.ServiceBase. The sample service logs the service 
* start and stop information to the Application event log, and shows how to 
* run the main function of the service in a thread pool worker thread. 
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#region Using directives
using System.ServiceProcess;
using System.Threading;
using System;
using System.IO;
#endregion


namespace GreyhoundsUpdateService
{
    public partial class GreyhoundsService : ServiceBase
    {
        public GreyhoundsService()
        {
            InitializeComponent();

            this.stopping = false;
            this.stoppedEvent = new ManualResetEvent(false);
        }


        /// <summary>
        /// The function is executed when a Start command is sent to the 
        /// service by the SCM or when the operating system starts (for a 
        /// service that starts automatically). It specifies actions to take 
        /// when the service starts. In this code sample, OnStart logs a 
        /// service-start message to the Application log, and queues the main 
        /// service function for execution in a thread pool worker thread.
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <remarks>
        /// A service application is designed to be long running. Therefore, 
        /// it usually polls or monitors something in the system. The 
        /// monitoring is set up in the OnStart method. However, OnStart does 
        /// not actually do the monitoring. The OnStart method must return to 
        /// the operating system after the service's operation has begun. It 
        /// must not loop forever or block. To set up a simple monitoring 
        /// mechanism, one general solution is to create a timer in OnStart. 
        /// The timer would then raise events in your code periodically, at 
        /// which time your service could do its monitoring. The other 
        /// solution is to spawn a new thread to perform the main service 
        /// functions, which is demonstrated in this code sample.
        /// </remarks>
        protected override void OnStart(string[] args)
        {
            // Log a service start message to the Application log.
            this.eventLog.WriteEntry("GreyhoundsUpdateService in OnStart.");

            // Queue the main service function for execution in a worker thread.
            ThreadPool.QueueUserWorkItem(new WaitCallback(ServiceWorkerThread));
        }


        /// <summary>
        /// The method performs the main function of the service. It runs on 
        /// a thread pool worker thread.
        /// </summary>
        /// <param name="state"></param>
        private void ServiceWorkerThread(object state)
        {
            string currDir = Directory.GetCurrentDirectory();

            // Periodically check if the service is stopping.
            while (!this.stopping)
            {                
                string mDir = "C:\\inetpub\\wwwroot\\Greyhound\\Canideos";
                Directory.SetCurrentDirectory(mDir);
                try
                {
                    DateTime curr_date = DateTime.Today.Date;
                    string curr_month = (curr_date.Month < 10 ? "0" + curr_date.Month : "" + curr_date.Month);
                    string curr_day = (curr_date.Month < 10 ? "0" + curr_date.Day : "" + curr_date.Day);
                    string currDate = curr_date.Year + "-" + curr_month + "-" + curr_day;
                    this.eventLog.WriteEntry("GreyhoundsUpdateService: run for present_date " + currDate);
                    System.Diagnostics.Process.Start("C:\\Perl64\\bin\\perl.exe", "C:\\inetpub\\wwwroot\\Greyhound\\Canideos\\race_master.pl " + currDate);
                }
                catch (Exception e)
                {
                    this.eventLog.WriteEntry("GreyhoundsUpdateService Exception: " + e.Message);
                }

                try
                {
                    DateTime future_date = DateTime.Today.AddDays(1).Date;
                    string future_month = (future_date.Month < 10 ? "0" + future_date.Month : "" + future_date.Month);
                    string future_day = (future_date.Month < 10 ? "0" + future_date.Day : "" + future_date.Day);
                    string futDate = future_date.Year + "-" + future_month + "-" + future_day;
                    this.eventLog.WriteEntry("GreyhoundsUpdateService: run for future_date " + futDate);
                    System.Diagnostics.Process.Start("C:\\Perl64\\bin\\perl.exe", "C:\\inetpub\\wwwroot\\Greyhound\\Canideos\\future_race_master.pl " + futDate);
                }
                catch (Exception e)
                {
                    this.eventLog.WriteEntry("GreyhoundsUpdateService Exception: " + e.Message);
                }

                Thread.Sleep(2 * 3600 * 1000);  // Run every 4 hours
            }

            Directory.SetCurrentDirectory(currDir);

            // Signal the stopped event.
            this.stoppedEvent.Set();
        }


        /// <summary>
        /// The function is executed when a Stop command is sent to the 
        /// service by SCM. It specifies actions to take when a service stops 
        /// running. In this code sample, OnStop logs a service-stop message 
        /// to the Application log, and waits for the finish of the main 
        /// service function.
        /// </summary>
        protected override void OnStop()
        {
            // Log a service stop message to the Application log.
            this.eventLog.WriteEntry("GreyhoundsUpdateService in OnStop.");

            // Indicate that the service is stopping and wait for the finish 
            // of the main service function (ServiceWorkerThread).
            this.stopping = true;
            this.stoppedEvent.WaitOne();
        }


        private bool stopping = false;
        private ManualResetEvent stoppedEvent;

        private void eventLog_EntryWritten(object sender, System.Diagnostics.EntryWrittenEventArgs e)
        {

        }
    }
}