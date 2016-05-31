using System;

using Android.App;
using Android.OS;
using Android.Widget;
using System.Threading;

namespace InterphoneSAM
{
    [Activity(Label = "CommunicationDeafMute")]
    public class CommunicationDeafMute : Activity
    {
        private Button _sendTextButton; //Bouton pour envoyer les données
        private EditText _textToSend; //Champ pour taper le texte à envoyer
        private TextView _textToReceive; //Champ pour recevoir le texte

        private Thread _updateTextToReceive;
        private Thread checkCommunication;

        private bool boolUpdateTextToReceive;
        private bool boolCheckCommunication;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.CommunicationDeafMute);

            //Recupération des élements depuis le layout
            _sendTextButton = FindViewById<Button>(Resource.Id.sendTextButton);
            _textToSend = FindViewById<EditText>(Resource.Id.textToSend);
            _textToReceive = FindViewById<TextView>(Resource.Id.textToReceive);

            _updateTextToReceive = new Thread(updateTextToReceiveFunction);
            boolUpdateTextToReceive = true;
            _updateTextToReceive.Start();

            checkCommunication = new Thread(checkCommunicationFunction);
            boolCheckCommunication = true;
            checkCommunication.Start();

            _sendTextButton.Click += new EventHandler(sendTextButtonClick); //Evenement lorsque le bouton "Envoyer" est préssé
        }

        protected override void OnStop()
        {
            base.OnStop();

            boolUpdateTextToReceive = false; //Pour stopper le thread de reception de données
            boolCheckCommunication = false;
        }

        private void checkCommunicationFunction()
        {
            while (boolCheckCommunication == true)
            {
                if (MenuActivity.tcpServeur.phrase == "---STOP---")
                {
                    //On quitte l'activité sur le serveur
                    MenuActivity.tcpServeur.cleanBuffer();
                    StartActivity(typeof(MenuActivity));
                }
            }
        }

        private void updateTextToReceiveFunction()
        {
            while(boolUpdateTextToReceive == true)
            {
                RunOnUiThread(new Action(updateTextView)); //Mise à jour de la textView par le thread principal
                Thread.Sleep(2); //Pause de 2ms pour ne pas saturer l'application
            }
        }
        private void updateTextView()
        {
             _textToReceive.Text = MenuActivity.tcpServeur.phrase; //Mise à jour avec la phrase du buffer (phrase du client)
        }

        private void sendTextButtonClick(Object sender, EventArgs e)
        {
            if(_textToSend.Text != "")
            {
                MenuActivity.tcpServeur.sendData(_textToSend.Text); //Si on envoie pas du texte vide, on envoie le texte au client.
            }
        }
    }
}