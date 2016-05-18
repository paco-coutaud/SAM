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

namespace InterphoneSAM
{
    [Activity(Label = "CallActivity")]
    public class CallActivity : Activity
    {
        private string _choice;
        private Intent intentNextActivity;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Intent intent = Intent;
            _choice = intent.GetStringExtra("choice");

            SetContentView(Resource.Layout.Call_Activity);///comentaire

            System.Diagnostics.Debug.WriteLine(_choice);

            Button button1 = FindViewById<Button>(Resource.Id.button1);

            intentNextActivity = new Intent(this, typeof(ComunicationActivity));
            intentNextActivity.PutExtra("choice", _choice);

            button1.Click += new EventHandler(button1Click);
        }

        private void button1Click(Object sender, EventArgs e)
        {
            StartActivity(intentNextActivity);
        }
    }
}