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

namespace InterphoneSAM
{
    [Activity(Label = "CommunicationDeafMute")]
    public class CommunicationDeafMute : Activity
    {
        private Button _sendTextButton;
        private EditText _textToSend;
        private TextView _textToReceive;

        private Thread _updateTextToReceive;
        private bool boolUpdateTextToReceive;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.CommunicationDeafMute);

            _sendTextButton = FindViewById<Button>(Resource.Id.sendTextButton);
            _textToSend = FindViewById<EditText>(Resource.Id.textToSend);
            _textToReceive = FindViewById<TextView>(Resource.Id.textToReceive);

            _updateTextToReceive = new Thread(updateTextToReceiveFunction);
            boolUpdateTextToReceive = true;
            _updateTextToReceive.Start();

            _sendTextButton.Click += new EventHandler(sendTextButtonClick);
        }

        protected override void OnStop()
        {
            base.OnStop();

            boolUpdateTextToReceive = false;
        }

        private void updateTextToReceiveFunction()
        {
            while(boolUpdateTextToReceive == true)
            {
                RunOnUiThread(new Action(updateTextView));
                Thread.Sleep(2);
            }
        }
        private void updateTextView()
        {
             _textToReceive.Text = MenuActivity.tcpServeur.phrase;
        }

        private void sendTextButtonClick(Object sender, EventArgs e)
        {
            if(_textToSend.Text != "")
            {
                MenuActivity.tcpServeur.sendData(_textToSend.Text);
            }
        }
    }
}