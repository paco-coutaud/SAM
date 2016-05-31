using System;

using Android.App;
using Android.OS;
using Android.Widget;
using System.Threading;
using Refractored.Xam.TTS;
using Speech;
using Android.Content;

namespace InterphoneSAM
{
    [Activity(Label = "Communication (Mal-voyant)")]
    public class CommunicationBlindNormal : Activity
    {
        private string _choice; //Handicap de l'utilisateur
        private Button _speakButton; //Bouton "Appuyer et parler"
        private Button _hangUp; //Bouton "Raccrocher"
        private string _oldVarSpeechToText; //Variable de comparaison
        private string _oldReceiveData; //Variable de comparaison
        private Thread updateSendText; //THread de MAJ pour l'envoie du texte
        private Thread updateReceiveData; //Thread de MAJ pour la reception de données
        private SpeechToText _speechToText; //Objet SpeechToText pour la reconnaissance vocale
        private bool boolUpdateReceiveDataFunction; //Variable booleene pour les threads
        private bool boolUpdateSendText; //Variable booleene pour les threads

        //Lorsque l'on rentre dans l'activité, cette méthode est appelée directement.
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.CommunicationBlindNormal); //Utilisation de la vue "CommunicationBlindNormal"

            _speechToText = new SpeechToText(this); //Instanciation d'un objet SpeechToText(context)

            _choice = Intent.GetStringExtra("choice");

            //Recuperation des elements de la vue
            _speakButton = FindViewById<Button>(Resource.Id.speakButton);
            _hangUp = FindViewById<Button>(Resource.Id.hangUp);

            //Initialisation des variables de comparaison
            _oldVarSpeechToText = "";
            _oldReceiveData = "";

            //Demarrage des threads
            updateSendText = new Thread(sendText);
            boolUpdateSendText = true;
            updateSendText.Start();

            updateReceiveData = new Thread(updateReceiveDataFunction);
            boolUpdateReceiveDataFunction = true;
            updateReceiveData.Start();

            //Gestion des de l'evenement click pour les boutons
            _speakButton.Click += new EventHandler(speakButtonClick);
            _hangUp.Click += new EventHandler(hangUpClick);
        }

        //Lorsque l'on quite l'activité, cette méthode est appelée.
        protected override void OnStop()
        {
            base.OnStop();

            //Pour arreter les threads lorsque l'on quitte l'activité
            boolUpdateReceiveDataFunction = false;
            boolUpdateSendText = false;
        }

        //Si l'utilisateur clique sur le bouton "Raccrocher"
        private void hangUpClick(Object sender, EventArgs e)
        {
            //Lorsque l'on clique sur le bouton raccrocher
            MenuActivity.tcpClient.sendText("---STOP---");
            Intent intent = new Intent(this, typeof(WaitActivity));
            intent.PutExtra("choice", _choice);
            StartActivity(intent);
        }

        //Thread de MAJ des données reçues
        private void updateReceiveDataFunction()
        {
            while(boolUpdateReceiveDataFunction == true)
            {
                if(_oldReceiveData != MenuActivity.tcpClient.phrase)
                {
                    CrossTextToSpeech.Current.Speak(MenuActivity.tcpClient.phrase); //Pour utiliser la synthese vocale
                    _oldReceiveData = MenuActivity.tcpClient.phrase; //Mise à jour de la variable de comparaison
                }

                Thread.Sleep(2); //Pause du thread pour ne pas saturer l'application
            }
        }

        //Si l'utilisateur clique sur le bouton "Appuyer et parler".
        private void speakButtonClick(Object sender, EventArgs e)
        {
            _speechToText.startListening(); //Pour commencer l'écoute
        }

        //THread de MAJ de l'envoie de texte.
        private void sendText()
        {
            while(boolUpdateSendText == true)
            {
                if (_speechToText._voiceListener._varSpeech != _oldVarSpeechToText)
                {
                    MenuActivity.tcpClient.sendText(_speechToText._voiceListener._varSpeech);
                    _oldVarSpeechToText = _speechToText._voiceListener._varSpeech; //Mise à jour de la variable de comparaison
                }

                Thread.Sleep(2); //Pause de 2ms dans le thread pour eviter la saturation
            }
        }
    }
}