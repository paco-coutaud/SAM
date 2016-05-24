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

namespace InterphoneSAM
{
    class SpeechToText
    {
        private SpeechRecognizer speechRecognizer;
        private SpeechToText speechToText;
        public VoiceListener _voiceListener { get; private set; }
        private Context _context;
        private Intent intent;

        public SpeechToText(Context context)
        {
            _context = context;
            _voiceListener = new VoiceListener(_context);
            initVoiceRecognizer();
        }

        public void startListening()
        {
            if (speechRecognizer != null)
            {
                speechRecognizer.Cancel();
            }

            speechRecognizer.StartListening(intent);
        }
        private SpeechRecognizer getSpeechRecognizer()
        {
            if (speechRecognizer == null)
            {
                speechRecognizer = SpeechRecognizer.CreateSpeechRecognizer(_context);
                speechRecognizer.SetRecognitionListener(_voiceListener);
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