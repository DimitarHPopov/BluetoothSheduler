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
using Model;

namespace Library
{
    public class SchedulerCountDownTimer : CountDownTimer
    {
        public WorkingHours Sheduler { get; set; }

        public bool ToStartBluetooth;

        public event EventHandler Finish;

        public SchedulerCountDownTimer(long millisecond, long countDownInterval, Model.WorkingHours sheduler, bool toStartBluetooth)
            : base(millisecond, countDownInterval)
        {
            this.Sheduler = sheduler;
            this.ToStartBluetooth = toStartBluetooth;
        }

        public override void OnFinish()
        {
            if(Finish != null)
            {
                Finish(this.Sheduler, null);
            }
        }

        public override void OnTick(long millisUntilFinished)
        {
        }
    }
}