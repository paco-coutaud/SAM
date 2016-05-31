using System;

using Android.App;
using Android.OS;
using Android.Widget;
using System.Threading;
using Refractored.Xam.TTS;
using Speech;

namespace InterphoneSAM
{
    [Activity(Label = "Communication (Mal-voyant)")]
    public class CommunicationBlindNormal : Activity
    {
        private Button _speakButton; //Bouton "Appuyer et parler"
        private string _oldVarSpeechToText;
        private string _oldReceiveData;
        private Thread updateSendText;
        private Thread updateReceiveData;
        private Thread checkCommunication;
        private SpeechToText _speechToText; //Objet SpeechToText
        private bool boolUpdateReceiveData;
        private bool boolUpdateSendText;
        private bool boolCheckCommunication;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.CommunicationBlindNormal);

            _speechToText = new SpeechToText(this);

            _speakButton = FindViewById<Button>(Resource.Id.speakButton);

            _oldVarSpeechToText = "";
            _oldReceiveData = "";

            updateSendText = new Thread(sendText);
            boolUpdateSendText = true;
            updateSendText.Start();

            updateReceiveData = new Thread(updateReceiveDataFunction);
            boolUpdateReceiveData = true;
            updateReceiveData.Start();

            checkCommunication = new Thread(checkCommunicationFunction);
            boolCheckCommunication = true;
            checkCommunication.Start();

            _speakButton.Click += new EventHandler(speakButtonClick);
        }

        protected override void OnStop() //Si l'activité est arretée
        {
            base.OnStop();

            boolUpdateReceiveData = false; //Pour couper le thread de reception de données
            boolUpdateSendText = false; //Pour couper le thread d'envoie de données
            boolCheckCommunication = false;
        }

        private void checkCommunicationFunction()
        {
            while(boolCheckCommunication == true)
            {
                if(MenuActivity.tcpServeur.phrase == "---STOP---")
                {
                    //On quitte l'activité sur le serveur
                    MenuActivity.tcpServeur.cleanBuffer();
                    StartActivity(typeof(MenuActivity));
                }
            }
        }

        private void updateReceiveDataFunction()
        {
            while(boolUpdateReceiveData == true)
            {
                if(_oldReceiveData != MenuActivity.tcpServeur.phrase) //Si une nouvelle phrase est détectée
                {
                    CrossTextToSpeech.Current.Speak(MenuActivity.tcpServeur.phrase); //On lit la phrase à l'oral
                    _oldReceiveData = MenuActivity.tcpServeur.phrase; //On met à jour la variable local avec la phrase courante présente dans le buffer du serveur (phrase du client)
                }

                Thread.Sleep(2); //Pause dans le thread de 2ms pour ne pas saturer l'application
            }
        }
        private void speakButtonClick(Object sender, EventArgs e)
        {
            _speechToText.startListening(); //On lance l'écoute (l'utilisateur parle à ce moment là)
        }

        private void sendText()
        {
            while(boolUpdateSendText == true)
            {
                System.Diagnostics.Debug.WriteLine(_speechToText._voiceListener._varSpeech);
                if (_speechToText._voiceListener._varSpeech != _oldVarSpeechToText) //Si la phrase prononcée est differente de la précédente
                {
                    MenuActivity.tcpServeur.sendData(_speechToText._voiceListener._varSpeech); //On envoie la phrase prononcée au client.
                    _oldVarSpeechToText = _speechToText._voiceListener._varSpeech; //On met à jour la variable local pour ne pas renvoyer plusieurs fois la même phrase
                }

                Thread.Sleep(2); //Pause dans le thread de 2ms pour ne pas saturer l'application
            }
        }
    }
}