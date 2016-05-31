using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.OS;
using Android.Speech;

namespace Speech
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

        public void OnReadyForSpeech(Bundle paramss) {}
        public void OnBeginningOfSpeech(){}
        public void OnRmsChanged(float rmsdB) { }
        public void OnBufferReceived(byte[] buffer) { }
        public void OnEndOfSpeech(){}
        public void OnError(SpeechRecognizerError e)
        {
        }
        public void OnResults(Bundle results)
        {
            data = results.GetStringArrayList(SpeechRecognizer.ResultsRecognition).ToList();
            _varSpeech = data[0];
        }
        public void OnPartialResults(Bundle partialResults) { }
        public void OnEvent(int eventType, Bundle paramss) { }
    }
}