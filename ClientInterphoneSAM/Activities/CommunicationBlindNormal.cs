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
using System.Threading;
using Android.Speech;
using Refractored.Xam.TTS;

namespace InterphoneSAM
{
    [Activity(Label = "CommunicationBlind")]
    public class CommunicationBlindNormal : Activity
    {
        private Button _speakButton;
        public string _varSpeechToText;
        private string _oldVarSpeechToText;
        private string _oldReceiveData;
        private Thread updateSendText;
        private Thread updateReceiveData;
        private SpeechToText _speechToText;
        private bool boolUpdateReceiveDataFunction;
        private bool boolUpdateSendText;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.CommunicationBlindNormal);

            _speechToText = new SpeechToText(this);

            _speakButton = FindViewById<Button>(Resource.Id.speakButton);

            _oldVarSpeechToText = "";
            _varSpeechToText = "";
            _oldReceiveData = "";

            updateSendText = new Thread(sendText);
            boolUpdateSendText = true;
            updateSendText.Start();

            updateReceiveData = new Thread(updateReceiveDataFunction);
            boolUpdateReceiveDataFunction = true;
            updateReceiveData.Start();

            _speakButton.Click += new EventHandler(speakButtonClick);
        }

        protected override void OnStop()
        {
            base.OnStop();

            boolUpdateReceiveDataFunction = false;
            boolUpdateSendText = false;
        }

        private void updateReceiveDataFunction()
        {
            while(boolUpdateReceiveDataFunction == true)
            {
                if(_oldReceiveData != MenuActivity.tcpClient.phrase)
                {
                    CrossTextToSpeech.Current.Speak(MenuActivity.tcpClient.phrase);
                    _oldReceiveData = MenuActivity.tcpClient.phrase;
                }

                Thread.Sleep(2);
            }
        }
        private void speakButtonClick(Object sender, EventArgs e)
        {
            _speechToText.startListening();
        }

        private void sendText()
        {
            while(boolUpdateSendText == true)
            {
                System.Diagnostics.Debug.WriteLine(_speechToText._voiceListener._varSpeech);
                if (_speechToText._voiceListener._varSpeech != _oldVarSpeechToText)
                {
                    MenuActivity.tcpClient.sendText(_speechToText._voiceListener._varSpeech);
                    _oldVarSpeechToText = _speechToText._voiceListener._varSpeech;
                }

                Thread.Sleep(2);
            }
        }
    }
}