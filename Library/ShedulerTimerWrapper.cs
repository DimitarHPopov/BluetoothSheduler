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
using Android.Bluetooth;
using System.Threading.Tasks;
using Model;

namespace Library
{
    public class ShedulerTimerWrapper : IDisposable
    {
        private Model.WorkingHours _sheduler;
        private SchedulerCountDownTimer _timer;

        public ShedulerTimerWrapper(Model.WorkingHours sheduler)
        {
            this._sheduler = sheduler;

            TimerInit();
        }

        public void TimerInit()
        {
            long timerMilliseconds = -1;
            bool start = true;

            // взема се най близкият ден и се слага най отпред
            ReoderItems();

            // локална променлива за реордарните дни
            var day = _sheduler.Days;

            DateTime nextPool = DateTime.Now;

            while (timerMilliseconds == -1 && day.Count > 0)
            {
                // изчислява се времето в ms до следващото пускане на таймер и неговото действие ( дали да пусне bluetooth или да го спре)
                timerMilliseconds = GetMilliSeconds(ref start, day[0], ref nextPool);

                day.RemoveAt(0);
            }

            TimerStart(new SchedulerCountDownTimer(timerMilliseconds, 30000, _sheduler, start));

            NextPool(nextPool, start);
        }

        private void TimerStart(SchedulerCountDownTimer sct)
        {
            _timer = sct;
            _timer.Finish -= timer_Finish;
            _timer.Finish += timer_Finish;
            _timer.Start();
        }

        private void NextPool(DateTime dt, bool start)
        {
            var pools = Library.FileWorker.LoadFromFile<List<NextPool>>(Enums.PoolsrFile, new List<NextPool>());
           
            var existedPoolForTimer = pools.Where(p => p.TimerGuid == _sheduler.Guid).FirstOrDefault();

            if (existedPoolForTimer != null)
            {
                pools.Remove(existedPoolForTimer);
            }

            existedPoolForTimer = new NextPool();

            existedPoolForTimer.Start = start;
            existedPoolForTimer.Date = dt;
            existedPoolForTimer.TimerGuid = _sheduler.Guid;

            pools.Add(existedPoolForTimer);

            Library.FileWorker.SaveToFile(Enums.PoolsrFile, pools);
        }

        public void ReoderItems()
        {
            //вземат се предстоящите дни от днес нататак
            var firstAvlbItem = _sheduler.Days.Where(d => d >= (int)DateTime.Now.DayOfWeek).ToList();

            if (firstAvlbItem != null && firstAvlbItem.Count > 0)
            {
                _sheduler.Days.RemoveAll(t => firstAvlbItem.Contains(t));
                _sheduler.Days.InsertRange(0, firstAvlbItem);
            }
        }

        void timer_Finish(object sender, EventArgs e)
        {
            //работата с бт
            TimerFinish(_timer.ToStartBluetooth);

            TimerInit();
        }

        private void TimerFinish(bool start)
        {
            BluetoothAdapter mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;

            if (mBluetoothAdapter != null)
            {
                if (start)
                {
                    if (!mBluetoothAdapter.IsEnabled)
                    {
                        mBluetoothAdapter.Enable();
                    }
                }
                else
                {
                    if (mBluetoothAdapter.IsEnabled)
                    {
                        mBluetoothAdapter.Disable();
                    }
                }
            }
        }

        private long GetMilliSeconds(ref bool start, int day, ref DateTime nextPool)
        {
            // изчислява се следващият ден
            DateTime baseDt = DateTime.Now.AddDays( ((day - (int) DateTime.Now.DayOfWeek + 7) % 7));

            DateTime startDay = new DateTime(
                baseDt.Year,
                baseDt.Month,
                baseDt.Day,
                _sheduler.StartDate.Hour,
                _sheduler.StartDate.Minute,
                _sheduler.StartDate.Second);

            DateTime endDay = new DateTime(
                baseDt.Year,
                baseDt.Month,
                baseDt.Day,
                _sheduler.EndDate.Hour,
                _sheduler.EndDate.Minute,
                _sheduler.EndDate.Second);

            if (startDay <= DateTime.Now && DateTime.Now < endDay)
            {
                TimeSpan span = endDay - DateTime.Now;
                start = false;
                nextPool = endDay;
                TimerFinish(true);
                return (long)span.TotalMilliseconds;
            }
            else
            {
                if (startDay >= DateTime.Now)
                {
                    TimeSpan span = startDay - DateTime.Now;
                    start = true;
                    nextPool = startDay;
                    return (long)span.TotalMilliseconds;
                }
                else
                {
                    return -1;
                }
            }
        }

        public void Stop()
        {
            _timer.Cancel();
            _timer.Dispose();
            this.Dispose();
        }

        public void Dispose()
        {
        }
    }
}