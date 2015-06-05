using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Bluetooth;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BluetoothSheduler
{
    [Activity(Label = "BluetoothSheduler", Theme = "@style/Theme.Base", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : Activity
    {

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            var scedulers = Library.FileWorker.LoadFromFile<List<Model.WorkingHours>>(Model.Enums.SchedulerFile, new List<Model.WorkingHours>());

            var transaction = this.FragmentManager.BeginTransaction();

            for (int i = 0; i < 2; i++)
            {
                if (i < scedulers.Count)
                {
                    transaction.Add(Resource.Id.content_timer, new TimerFragment(scedulers[i]), "timer" + i.ToString());
                }
                else
                {
                    transaction.Add(Resource.Id.content_timer, new TimerFragment(null), "timer" + i.ToString());
                }
            }

            transaction.Commit();
        }

        public async Task<bool> ChangedSceduler(Model.WorkingHours sc)
        {
            var scedulers = Library.FileWorker.LoadFromFile<List<Model.WorkingHours>>(Model.Enums.SchedulerFile, new List<Model.WorkingHours>());

            var item = scedulers.Where(s => s.Guid == sc.Guid).FirstOrDefault();

            if (item != null)
            {
                item.Active = sc.Active;
                item.Days = sc.Days;
                item.StartDate = sc.StartDate;
                item.EndDate = sc.EndDate;
            }
            else
            {
                scedulers.Add(sc);
            }

            bool res = await Library.FileWorker.SaveToFile(Model.Enums.SchedulerFile, scedulers);

            StartService(new Intent(this, typeof(ShedulerService)));

            return res;
        }
    }
}

