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
using Android.Speech;
using System.Collections;

namespace InterphoneSAM
{
    class VoiceListener: Java.Lang.Object, Android.Speech.IRecognitionListener
    {
        private Context _context;
        private List<string> data;
        public string _varSpeech;

        public VoiceListener(Context context)
        {
            data = new List<string>();
            _context = context;
            _varSpeech = "";
        }

        public void OnReadyForSpeech(Bundle paramss) { System.Diagnostics.Debug.WriteLine("HELLO2"); }
            public void OnBeginningOfSpeech()
            {
        }
            public void OnRmsChanged(float rmsdB) { }
            public void OnBufferReceived(byte[] buffer) { }
            public void OnEndOfSpeech()
            {
            }
            public void OnError(SpeechRecognizerError e)
            {
            }
            public void OnResults(Bundle results)
            {
                data = results.GetStringArrayList(SpeechRecognizer.ResultsRecognition).ToList();
                System.Diagnostics.Debug.WriteLine(data[0]);
                _varSpeech = data[0];
                System.Diagnostics.Debug.WriteLine(_varSpeech);
            }
            public void OnPartialResults(Bundle partialResults) { }
            public void OnEvent(int eventType, Bundle paramss) { }
    }
}