using System;
using Android.App;
using Android.OS;
using System.Threading;
using Android.Media;
using Android.Widget;
using Android.Content;
using Tcp;

namespace InterphoneSAM
{
    [Activity(Label = "En attente d'une réponse")]
    public class WaitActivity : Activity
    {
        private bool stopThread;
        private Thread _updateTextToReceive;
        private string _choice;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            stopThread = false;
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Wait_Activity);
            _choice = Intent.GetStringExtra("choice");

            _updateTextToReceive = new Thread(updateTextToReceiveFunction);
            _updateTextToReceive.Start();

        }


        private void updateTextToReceiveFunction()
        {
            while (stopThread==false)
            {
                if (MenuActivity.tcpServeur.phrase == "---DECROCHE---") 
                {
                    stopThread = true;
                    RunOnUiThread(new Action(PickUpAction));
                }
                else if (MenuActivity.tcpServeur.phrase == "---RACROCHE---")
                {
                    stopThread = true;
                    MenuActivity.tcpServeur.cleanBuffer();
                    RunOnUiThread(new Action(HangUpAction));
                }
                Thread.Sleep(2);
            }
        }

        private void HangUpAction()
        {
            StartActivity(typeof(MenuActivity));
        }

        private void PickUpAction()
        {
            _choice = Intent.GetStringExtra("choice");

            if(_choice == "Sourd-Muet")
            {
                
                MenuActivity.tcpServeur.cleanBuffer();
                Intent intent = new Intent(this, typeof(CommunicationDeafMute));
                StartActivity(intent);
            }
            else if (_choice == "Mal-Voyant")
            {
               
                MenuActivity.tcpServeur.cleanBuffer();
                Intent intent = new Intent(this, typeof(CommunicationBlindNormal));
                StartActivity(intent);
            }
            else 
            {
                
                MenuActivity.tcpServeur.cleanBuffer();
                Intent intent = new Intent(this, typeof(CommunicationBlindNormal));
                StartActivity(intent);
            }
        }

    }
}