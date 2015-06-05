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

namespace BluetoothSheduler
{
    [Activity(MainLauncher = true, NoHistory = true, Icon = "@drawable/bluetooth", Theme = "@style/Theme.Splash", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class SplashScreen : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here

            StartActivity(new Intent(this, typeof(MainActivity)));
        }
    }
}