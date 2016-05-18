using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Speech;

namespace Speech
{
    public class SpeechToText
    {
        private SpeechRecognizer speechRecognizer;
        private Intent intent;
        private Context _context;
        public string _varSpeechToText;
        public SpeechToText(Context context, string varSpeechToText)
        {
            _context = context;
            _varSpeechToText = varSpeechToText;
            initVoiceRecognizer();
        }

        private void startListeningClick(Object sender, EventArgs e)
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
                speechRecognizer.SetRecognitionListener(new VoiceListener(_context,_varSpeechToText));
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

