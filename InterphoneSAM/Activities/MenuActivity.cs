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
using Tcp;
using System.Threading;

namespace InterphoneSAM
{
    [Activity(Label = "InterphoneSam",MainLauncher =true)]
    public class MenuActivity : Activity
    {
        private string _choice;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Menu_Activity);

            Intent intent = new Intent(this, typeof(CallActivity));

            Button buttonSrdMt = FindViewById<Button>(Resource.Id.SrdMt);
            Button buttonMalVt = FindViewById<Button>(Resource.Id.MalVt);
            Button buttonPasHcp = FindViewById<Button>(Resource.Id.PasHcp);

            buttonSrdMt.Click += delegate
            {
                _choice = "Sourd-Muet";
                intent.PutExtra("choice", _choice);
                StartActivity(intent);
            };
            //StartActivity(typeof(CallActivity)); _choice = "Death-Mute"; };
            buttonMalVt.Click += delegate
            {
                _choice = "Mal-Voyant";
                intent.PutExtra("choice", _choice);
                StartActivity(intent);
            };// StartActivity(typeof(CallActivity)); _choice = "Blind"; };
            buttonPasHcp.Click += delegate
            {
                _choice = "Sans-handicap";
                intent.PutExtra("choice", _choice);
                StartActivity(intent);
            };// StartActivity(typeof(CallActivity)); _choice = "Normal"};

        }
    }
}