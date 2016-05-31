using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace InterphoneSAM
{
    [Activity(Label = "Choix du contact")]
    public class CallActivity : Activity
    {
        private string _choice; //Contient le handicap de la personne
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Call_Activity);

            Intent intent = Intent; //Récupération de l'Intent precedent
            _choice = intent.GetStringExtra("choice"); //Recuperation du handicap de la personne

            Button button1 = FindViewById<Button>(Resource.Id.button1); //Bouton Mr Dupont

            button1.Click += new EventHandler(button1Click); //Creation d'un evenement si le bouton est clické
        }

        private void button1Click(Object sender, EventArgs e)
        {
            if(_choice == "Sourd-Muet")
            {
                MenuActivity.tcpServeur.sendData("---SONNE---");
                Intent intent = new Intent(this, typeof(WaitActivity));
                intent.PutExtra("choice", _choice);
                StartActivity(intent);
            }
            else
            {
                MenuActivity.tcpServeur.sendData("---SONNE---");
                Intent intent = new Intent(this, typeof(WaitActivity));
                intent.PutExtra("choice", _choice);
                StartActivity(intent);
            }
        }
    }
}