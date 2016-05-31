using System;
using Android.App;
using Android.OS;
using System.Threading;
using Android.Media;
using Android.Widget;
using Android.Content;
using Tcp;

//ATTENTION REVOIR PASSAGE PUT EXTRA POUR DECROCHER
namespace InterphoneSAM
{
    [Activity(Label = "WaitActivity")]
    public class WaitActivity : Activity
    {
        private bool stopThread;
        private bool keepThread;
        private Thread _updateTextToReceive;
        private Button buttonPickUp;
        private Button buttonHangUp;
        MediaPlayer mPlayer = null;
        private string _choice;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            
            base.OnCreate(savedInstanceState);
            stopThread = false;
            keepThread = true;
            SetContentView(Resource.Layout.Wait_Activity);

            _choice = Intent.GetStringExtra("choice");
            buttonPickUp = FindViewById<Button>(Resource.Id.buttonPickUp);
            buttonPickUp.Visibility = Android.Views.ViewStates.Invisible;
            buttonPickUp.Click += new EventHandler(pickUpAction);

            buttonHangUp = FindViewById<Button>(Resource.Id.buttonHangUp);
            buttonHangUp.Visibility = Android.Views.ViewStates.Invisible;
            buttonHangUp.Click += new EventHandler(HangUpAction);

            _updateTextToReceive = new Thread(updateTextToReceiveFunction);
            _updateTextToReceive.Start();

        }

        private void HangUpAction(object sender, EventArgs e)
        {
            mPlayer.Stop();
            MenuActivity.tcpClient.sendText("---RACROCHE---");
            MenuActivity.tcpClient.cleanBuffer();
            keepThread = true;
            buttonHangUp.Visibility = Android.Views.ViewStates.Invisible;
            buttonPickUp.Visibility = Android.Views.ViewStates.Invisible;
        }

        protected override void OnStop()
        {
            base.OnStop();
            stopThread = true;
            if (mPlayer != null)
            {
                mPlayer.Stop();
                mPlayer.Release();
            }
        }

        private void updateTextToReceiveFunction()
        {
            while (stopThread==false)
            {
                if (MenuActivity.tcpClient.phrase == "---SONNE---" && keepThread == true) 
                {
                    keepThread = false;
                    RunOnUiThread(new Action(ringTone));
                }
                Thread.Sleep(2);
            }
        }

        private void pickUpAction(object sender, EventArgs e)
        {
            
            MenuActivity.tcpClient.sendText("---DECROCHE---");
            stopThread = true;

            if (_choice == "Sourd-Muet")
            {
                
                MenuActivity.tcpClient.cleanBuffer();
                Intent intent = new Intent(this, typeof(CommunicationDeafMute));
                StartActivity(intent);
            }
            else if (_choice == "Mal-Voyant")
            {
               
                MenuActivity.tcpClient.cleanBuffer();
                Intent intent = new Intent(this, typeof(CommunicationBlindNormal));
                StartActivity(intent);
            }
            else 
            {
                
                MenuActivity.tcpClient.cleanBuffer();
                Intent intent = new Intent(this, typeof(CommunicationBlindNormal));
                StartActivity(intent);
            }
        }

        private void ringTone()
        {
            buttonPickUp.Visibility = Android.Views.ViewStates.Visible;
            buttonHangUp.Visibility = Android.Views.ViewStates.Visible;
            if (mPlayer != null)
            {
                mPlayer.Stop();
                mPlayer.Release();
            }

            mPlayer = MediaPlayer.Create(this, Resource.Raw.ring_0);
            mPlayer.Looping = true;
            mPlayer.Start();
        }
    }
}