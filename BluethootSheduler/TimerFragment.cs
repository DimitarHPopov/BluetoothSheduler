using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Library;
using System.Threading.Tasks;
using Model;

namespace BluetoothSheduler
{
    public class TimerFragment : Fragment
    {
        private TextView tv_start;
        private TextView tv_end;
        private TextView tv_fake;
        private TextView tv_next_pool;
        private Button start_time;
        private Button end_time;
        private List<CheckLinear> _checkboxes = new List<CheckLinear>();

        private int? hour;
        private int? minute;
        private int? hourEnd;
        private int? minuteEnd;
        private Model.WorkingHours _sheduler;

        const int TIME_DIALOG_START = 0;
        const int TIME_DIALOG_END = 1;

        public TimerFragment(Model.WorkingHours sheduler)
        {
            this._sheduler = sheduler;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var v = inflater.Inflate(Resource.Layout.timer_layout, container, false);

            tv_start = v.FindViewById<TextView>(Resource.Id.tv_start);
            tv_end = v.FindViewById<TextView>(Resource.Id.tv_end);
            tv_fake = v.FindViewById<TextView>(Resource.Id.tv_fake);
            tv_next_pool = v.FindViewById<TextView>(Resource.Id.tv_next_pool);
            start_time = v.FindViewById<Button>(Resource.Id.btn_start_time);
            end_time = v.FindViewById<Button>(Resource.Id.btn_end_time);

            _checkboxes.Add(v.FindViewById<CheckLinear>(Resource.Id.cl_sunday));
            _checkboxes.Add(v.FindViewById<CheckLinear>(Resource.Id.cl_monday));
            _checkboxes.Add(v.FindViewById<CheckLinear>(Resource.Id.cl_tuesday));
            _checkboxes.Add(v.FindViewById<CheckLinear>(Resource.Id.cl_wednesday));
            _checkboxes.Add(v.FindViewById<CheckLinear>(Resource.Id.cl_thursday));
            _checkboxes.Add(v.FindViewById<CheckLinear>(Resource.Id.cl_friday));
            _checkboxes.Add(v.FindViewById<CheckLinear>(Resource.Id.cl_saturday));

            v.FindViewById<Button>(Resource.Id.btn_save).Click += TimerFragment_Click;
            // Add a click listener to the button
            start_time.Click += (o, e) => CreateDialog(TIME_DIALOG_START).Show();
            end_time.Click += (o, e) => CreateDialog(TIME_DIALOG_END).Show();

            SetSheduler();

            return v;
        }

        private void SetSheduler()
        {
            if(_sheduler == null)
            {
                tv_start.Text = "N/A";
                tv_end.Text = "N/A";
                tv_fake.Text = "Спрян";
                
            }
            else
            {
                for(int i = 0 ; i < _sheduler.Days.Count; i++)
                {
                    _checkboxes[_sheduler.Days[i]].Checked = true;
                }

                hour = _sheduler.StartDate.Hour;
                minute = _sheduler.StartDate.Minute;

                hourEnd = _sheduler.EndDate.Hour;
                minuteEnd = _sheduler.EndDate.Minute;

                tv_fake.Text = "Пуснат";


                var pools = Library.FileWorker.LoadFromFile<List<NextPool>>(Enums.PoolsrFile, new List<NextPool>()).Where(p => p.TimerGuid == _sheduler.Guid).FirstOrDefault();

                tv_next_pool.Text = pools != null ? string.Format("{0} във {1}", pools.StartString, pools.Date) : string.Empty;

                UpdateDisplay(true);
                UpdateDisplay(false);
            }
        }

        void TimerFragment_Click(object sender, EventArgs e)
        {
            SaveTimer();
        }

        private async Task SaveTimer()
        {
            if (_sheduler == null)
            {
                _sheduler = new Model.WorkingHours();
                _sheduler.Guid = Guid.NewGuid();
            }

            _sheduler.Days = new List<int>();

            for (int i = 0; i < _checkboxes.Count; i++)
            {
                if (_checkboxes[i].Checked)
                {
                    _sheduler.Days.Add(i);
                }
            }

            string error = string.Empty;

            if (_sheduler.Days.Count > 0)
            {
                _sheduler.StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour.Value, minute.Value, 0);
                _sheduler.EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hourEnd.Value, minuteEnd.Value, 0);

                if (_sheduler.StartDate < _sheduler.EndDate)
                {
                    tv_fake.Text = await (this.Activity as MainActivity).ChangedSceduler(this._sheduler) ? "Пуснат" : "Спрян";
                }
                else
                {
                    error = "Стартовите часове трябва да са по големи от тези за спиране";
                }
            }
            else
            {
                error = "Изберете поне един ден";
            }

            if (!string.IsNullOrEmpty(error))
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(Activity);

                builder.SetMessage(error)
                       .SetTitle("Грешка");

                builder.SetPositiveButton("Ok", (s, e) => { });

                AlertDialog dialog = builder.Create();

               

                dialog.Show();
            }
        }

        private void UpdateDisplay(bool start)
        {
            if (start)
            {
                tv_start.Text = string.Format("{0}:{1}", hour, minute.ToString().PadLeft(2, '0'));
            }
            else
            {
                tv_end.Text = string.Format("{0}:{1}", hourEnd, minuteEnd.ToString().PadLeft(2, '0'));
            }
        }

        private void TimePickerCallback(object sender, TimePickerDialog.TimeSetEventArgs e)
        {
            hour = e.HourOfDay;
            minute = e.Minute;

            UpdateDisplay(true);
        }

        private void TimePickerCallbackEnd(object sender, TimePickerDialog.TimeSetEventArgs e)
        {
            hourEnd = e.HourOfDay;
            minuteEnd = e.Minute;

            UpdateDisplay(false);
        }

        private Dialog CreateDialog(int id)
        {
            switch (id)
            {
                case TIME_DIALOG_START:
                    return new TimePickerDialog(this.Activity, TimePickerCallback, hour.HasValue ? hour.Value : DateTime.Now.Hour, minute.HasValue ? minute.Value : DateTime.Now.Minute, true);

                case TIME_DIALOG_END:
                    return new TimePickerDialog(this.Activity, TimePickerCallbackEnd, hourEnd.HasValue ? hourEnd.Value : DateTime.Now.Hour, minuteEnd.HasValue ? minuteEnd.Value : DateTime.Now.Minute, true);

            }
            return null;
        }
    }
}