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
        private SpeechRecognizer speechRecognizer;
        private Button _speakButton;
        public string _varSpeechToText;
        private string _oldVarSpeechToText;
        private string _oldReceiveData;
        private Thread updateSendText;
        private Thread updateReceiveData;
        private Intent intent;
        private Context _context;
        private VoiceListener voiceListener;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.CommunicationBlindNormal);

            voiceListener = new VoiceListener(this, _varSpeechToText);

            _context = this;

            _speakButton = FindViewById<Button>(Resource.Id.speakButton);

            _oldVarSpeechToText = "";
            _varSpeechToText = "";
            _oldReceiveData = "";

            updateSendText = new Thread(sendText);
            updateSendText.Start();

            updateReceiveData = new Thread(updateReceiveDataFunction);
            updateReceiveData.Start();

            _speakButton.Click += new EventHandler(speakButtonClick);
            initVoiceRecognizer();
        }

        public void maj()
        {

        }

        private void updateReceiveDataFunction()
        {
            while(updateReceiveData.IsAlive)
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
            if (speechRecognizer != null)
            {
                speechRecognizer.Cancel();
            }

            speechRecognizer.StartListening(intent);
        }

        private void sendText()
        {
            while(updateSendText.IsAlive)
            {
                System.Diagnostics.Debug.WriteLine(voiceListener._varSpeech);
                if (voiceListener._varSpeech != _oldVarSpeechToText)
                {
                    MenuActivity.tcpClient.sendText(voiceListener._varSpeech);
                    _oldVarSpeechToText = voiceListener._varSpeech;
                }

                Thread.Sleep(2);
            }
        }

        private SpeechRecognizer getSpeechRecognizer()
        {
            if (speechRecognizer == null)
            {
                speechRecognizer = SpeechRecognizer.CreateSpeechRecognizer(_context);
                speechRecognizer.SetRecognitionListener(voiceListener);
            }
            return speechRecognizer;
        }

        private void initVoiceRecognizer()
        {
            speechRecognizer = getSpeechRecognizer();
            intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            intent.PutExtra(RecognizerIntent.ExtraLanguage, "fr-FR");
            intent.PutExtra(RecognizerIntent.ExtraCallingPackage, _context.PackageName);
            intent.PutExtra(RecognizerIntent.ExtraMaxResults, 60);
        }
    }
}