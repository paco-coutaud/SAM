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
using Refractored.Xam.TTS;

namespace InterphoneSAM
{
    [Activity(Label = "ComunicationActivity")]
    public class ComunicationActivity : Activity 
    {
        
        private TCPClient _tcpClient;
        private string _choice;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Intent intent = Intent;
            _choice = intent.GetStringExtra("choice");

            SetContentView(Resource.Layout.Communication_Activity);

            if (bundle == null)
            {
                
                FragmentManager.BeginTransaction().Add(Resource.Id.container, CameraFragment.NewInstance()).Commit();
            }

            _tcpClient = new TCPClient("192.168.43.117", 1234); //Instanciation de l'objet TCPClient(adresse serveur, port d'écoute).
            System.Threading.Thread.Sleep(10);
            _tcpClient.sendText(_choice);
            //CrossTextToSpeech.Current.Speak("Client " + _choice);
        }

    }
}