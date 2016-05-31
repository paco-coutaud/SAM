using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Tcp;
using System.Threading;

namespace InterphoneSAM
{
    [Activity(Label = "Interphone SAM",MainLauncher = true)]
    public class MenuActivity : Activity
    {
        public static TCPServeur tcpServeur = new TCPServeur("192.168.43.117",1234); //Variable static pour le serveur TCP (Doit être accessible dans toutes les activités)

        private string _choice; //Pour se souvenir du handicap de l'utilisateur
        private Thread tcpStart;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Menu_Activity);
            Intent intent = new Intent(this, typeof(CallActivity));

            Button buttonSrdMt = FindViewById<Button>(Resource.Id.SrdMt);
            Button buttonMalVt = FindViewById<Button>(Resource.Id.MalVt);
            Button buttonPasHcp = FindViewById<Button>(Resource.Id.PasHcp);

            tcpStart = new Thread(tcpStartFunction);
            tcpStart.Start();

            buttonSrdMt.Click += delegate
            {
                _choice = "Sourd-Muet";
                intent.PutExtra("choice", _choice); //Passage du choix dans la prochaine activité
                StartActivity(intent);
            };
            buttonMalVt.Click += delegate
            {
                _choice = "Mal-Voyant";
                intent.PutExtra("choice", _choice);
                StartActivity(intent);
            };
            buttonPasHcp.Click += delegate
            {
                _choice = "Sans-handicap";
                intent.PutExtra("choice", _choice);
                StartActivity(intent);
            };

        }

        private void tcpStartFunction()
        {
            //Demarrage du serveur TCP
            tcpServeur.start();
        }
    }
}