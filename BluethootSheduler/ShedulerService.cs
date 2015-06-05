using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using Library;
using System.Timers;
using Android.Bluetooth;

namespace BluetoothSheduler
{
    [Service]
    public class ShedulerService : Service
    {
        private List<ShedulerTimerWrapper> _runingWrappers;

        public ShedulerService()
        {
            LogMessage.LogVerbose("Service Start (Constructor)");
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            LogMessage.LogVerbose("Service OnStart (OnStartCommand)");

            ReloadServiceInfo();

            return base.OnStartCommand(intent, flags, startId);
        }

        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }

        private void ReloadServiceInfo()
        {
            if(_runingWrappers != null && _runingWrappers.Count > 0)
            {
                foreach(var wrapper in _runingWrappers)
                {
                    wrapper.Stop();
                }
            }

            _runingWrappers = new List<ShedulerTimerWrapper>();

            var scedulers = Library.FileWorker.LoadFromFile<List<Model.WorkingHours>>(Model.Enums.SchedulerFile, new List<Model.WorkingHours>());

            if(scedulers.Count > 0)
            {
                foreach(var s in scedulers)
                {
                    SetCountDownTimer(s);
                }
            }
        }

        private void SetCountDownTimer(Model.WorkingHours sheduler)
        {
           _runingWrappers.Add(new ShedulerTimerWrapper(sheduler));
        }
    }
}