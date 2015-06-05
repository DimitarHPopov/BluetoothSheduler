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

namespace Library
{
    public class LogMessage
    {
        public static string LogTitle = "BluetoothSheduler";

        public static void WriteLogError(Exception ex)
        {
            Log.Error(LogTitle, string.Format("Exception -> Message : {0} -> StackTrace {1}", ex.Message, ex.StackTrace));
        }

        public static void LogVerbose(string message)
        {
            Log.Verbose(LogTitle, message);
        }
    }
}