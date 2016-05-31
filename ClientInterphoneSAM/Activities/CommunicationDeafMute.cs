using System;

using Android.App;
using Android.OS;
using Android.Widget;
using System.Threading;
using Android.Content;

namespace InterphoneSAM
{
    [Activity(Label = "Communication (Sourd-muet)")]
    public class CommunicationDeafMute : Activity
    {
        private string _choice;
        private Button _sendTextButton; //Bouton pour envoyer le texte
        private Button _hangUp; //Bouton pour raccrocher
        private EditText _textToSend; //Champ pour taper le texte � envoyer
        private TextView _textToReceive; //Champ pour affiche le texte re�u
        private bool boolUpdateTextToReceive; //Boolean pour le thread

        private Thread _updateTextToReceive; //THread de MAJ du texte re�u

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.CommunicationDeafMute); //Utilisation de la vue CommunicationDeafMute
            _choice = Intent.GetStringExtra("choice");
            //Recuperation des elements � partir du layout
            _sendTextButton = FindViewById<Button>(Resource.Id.sendTextButton);
            _textToSend = FindViewById<EditText>(Resource.Id.textToSend);
            _textToReceive = FindViewById<TextView>(Resource.Id.textToReceive);
            _hangUp = FindViewById<Button>(Resource.Id.hangUp);

            _textToSend.SetSingleLine(true);

            //Demarrage du thread
            _updateTextToReceive = new Thread(updateTextToReceiveFunction);
            boolUpdateTextToReceive = true;
            _updateTextToReceive.Start();

            //Gestion de l'evenement click pour les boutons
            _sendTextButton.Click += new EventHandler(sendTextButtonClick);
            _hangUp.Click += new EventHandler(hangUpClick);
        }

        protected override void OnStop()
        {
            base.OnStop();

            //Pour arreter le thread lorsque l'on quitte l'activit�
            boolUpdateTextToReceive = false;
        }

        private void hangUpClick(Object sender, EventArgs e)
        {
            //Si le bouton raccrocher � �t� click�
            MenuActivity.tcpClient.sendText("---STOP---");
            Intent intent = new Intent(this, typeof(WaitActivity));
            intent.PutExtra("choice", _choice);
            StartActivity(intent);
        }

        private void updateTextToReceiveFunction()
        {
            while(boolUpdateTextToReceive == true)
            {
                RunOnUiThread(new Action(updateTextView)); //Pour lancer dans le thread principal (Tres important pour la modification d'�l�ments cr�es dans le thread principal)
                Thread.Sleep(2); //Pause pour ne pas saturer l'application
            }
        }
        private void updateTextView()
        {
                _textToReceive.Text = MenuActivity.tcpClient.phrase; //Mise � jour de la textView avec la nouvelle phrase re�ue
        }

        private void sendTextButtonClick(Object sender, EventArgs e)
        {
            if(_textToSend.Text != "")
            {
                MenuActivity.tcpClient.sendText(_textToSend.Text); //Si l'on click sur le bouton et que le texte envoy� n'est pas vide, on l'envoie au visiteur
                _textToSend.Text = "";
            }
        }
    }
}