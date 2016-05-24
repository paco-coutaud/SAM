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
    [Activity(Label = "InterphoneSam",MainLauncher = true)]
    public class MenuActivity : Activity
    {
        public static TCPClient tcpClient = new TCPClient("192.168.43.117", 1234);

        private Intent _intentNextActivity;
        private string _choice; //Chaine de caractere permettant d'enregistrer le handicap/non handicap de la personne c�t� maison
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Menu_Activity);

            _intentNextActivity = new Intent(this, typeof(ComunicationActivity)); //Creation d'un intent portant sur la CallActivity

            Button buttonSrdMt = FindViewById<Button>(Resource.Id.SrdMt);
            Button buttonMalVt = FindViewById<Button>(Resource.Id.MalVt);
            Button buttonPasHcp = FindViewById<Button>(Resource.Id.PasHcp);

            buttonSrdMt.Click += new EventHandler(buttonSrdMtClick);
            buttonMalVt.Click += new EventHandler(buttonMalVtClick);
            buttonPasHcp.Click += new EventHandler(buttonPasHcpClick);
        }

        private void buttonSrdMtClick(object sender, EventArgs e)
        {
            _choice = "Sourd-Muet";
            Intent intent = new Intent(this, typeof(CommunicationDeafMute));
            //intent.PutExtra("choice", _choice);
            StartActivity(intent);
        }

        private void buttonMalVtClick(object sender, EventArgs e)
        {
            _choice = "Mal-Voyant";
            Intent intent = new Intent(this, typeof(CommunicationBlindNormal));
            //intent.PutExtra("choice", _choice);
            StartActivity(intent);
        }

        private void buttonPasHcpClick(object sender, EventArgs e)
        {
            _choice = "Sans handicap";
            Intent intent = new Intent(this, typeof(CommunicationBlindNormal));
            //intent.PutExtra("choice", _choice);
            StartActivity(intent);
        }
    }
}