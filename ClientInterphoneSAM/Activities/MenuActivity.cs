using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Tcp;

namespace InterphoneSAM
{
    [Activity(MainLauncher = true)]
    public class MenuActivity : Activity
    {
        public static TCPClient tcpClient = new TCPClient("192.168.43.117", 1234); //Variable static, doit etre accessible de partout car il s'agit du TCPClient

        private string _choice; //Pour se souvenir du handicap de l'utilisateur

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Menu_Activity); //Utilisation de la vue Menu_Activity

            //Recuperation des elements à partir du layout
            Button buttonSrdMt = FindViewById<Button>(Resource.Id.SrdMt);
            Button buttonMalVt = FindViewById<Button>(Resource.Id.MalVt);
            Button buttonPasHcp = FindViewById<Button>(Resource.Id.PasHcp);

            //Gestion du click sur les differents boutons
            buttonSrdMt.Click += new EventHandler(buttonSrdMtClick);
            buttonMalVt.Click += new EventHandler(buttonMalVtClick);
            buttonPasHcp.Click += new EventHandler(buttonPasHcpClick);
        }

        private void buttonSrdMtClick(object sender, EventArgs e)
        {
            _choice = "Sourd-Muet";
            Intent intent = new Intent(this, typeof(WaitActivity));
            intent.PutExtra("choice", _choice); //Pour passer d'une activité à une autre une variable de type string
            StartActivity(intent); //Demarrer une activité
        }

        private void buttonMalVtClick(object sender, EventArgs e)
        {
            _choice = "Mal-Voyant";
            Intent intent = new Intent(this, typeof(WaitActivity));
            intent.PutExtra("choice", _choice);
            StartActivity(intent);
        }

        private void buttonPasHcpClick(object sender, EventArgs e)
        {
            _choice = "Sans handicap";
            Intent intent = new Intent(this, typeof(WaitActivity));
            intent.PutExtra("choice", _choice);
            StartActivity(intent);
        }
    }
}