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

namespace Model
{
    public class NextPool
    {
        public DateTime Date;
        public bool Start;
        public string StartString
        {
            get
            {
                return Start ? "пускане" : "спиране";
            }
        }
        public Guid TimerGuid;
    }
}