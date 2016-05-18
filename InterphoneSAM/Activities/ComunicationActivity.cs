using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Tcp;
using System.Threading;
using Refractored.Xam.TTS;

namespace InterphoneSAM
{
    [Activity(Label = "ComunicationActivity")]
    public class ComunicationActivity : Activity 
    {
        private TCPServeur _tcpServeur;
        private Thread tcpStart;
        private Thread tcpState;
        private Thread checkHandicap;
        private TextView _stateTCP;
        private string _handicapClient;
        private bool _getHandicapClient;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            ActionBar.Hide();
            SetContentView(Resource.Layout.Communication_Activity);
            if (bundle == null)
            { 
                FragmentManager.BeginTransaction().Add(Resource.Id.container, CameraFragment.NewInstance()).Commit();
            }
            _stateTCP = FindViewById<TextView>(Resource.Id.stateTCP);

            _tcpServeur = new TCPServeur("192.168.43.117", 1234);

            tcpStart = new Thread(tcpStartFunction);
            tcpStart.Start();

            tcpState = new Thread(tcpStateFunction);
            tcpState.Start();

            _getHandicapClient = false;

            checkHandicap = new Thread(checkHandicapFunction);
            checkHandicap.Start();

            CrossTextToSpeech.Current.Speak("Initialisation complétée");
        }

        private void tcpStateFunction()
        {
            while (tcpState.IsAlive)
            {
                RunOnUiThread(new Action(update));
                Thread.Sleep(2);
            }
        }

        private void update()
        {
            _stateTCP.Text = "State : " + _tcpServeur._state;
        }
        private void tcpStartFunction()
        {
            //Demarrage du serveur TCP
            _tcpServeur.start();
        }

        private void checkHandicapFunction()
        {
            while(_getHandicapClient == false)
            {
                System.Diagnostics.Debug.WriteLine("checkHandicapFunction");
                System.Diagnostics.Debug.WriteLine(_tcpServeur._state);
                if(_tcpServeur._state == "listener opened - client connected - ready")
                {
                    _handicapClient = _tcpServeur.receiveData();
                    _getHandicapClient = true;
                    System.Diagnostics.Debug.WriteLine(_handicapClient);
                    CrossTextToSpeech.Current.Speak("Le Client est " + _handicapClient);
                }

                Thread.Sleep(2);
            }
        }

    }
}