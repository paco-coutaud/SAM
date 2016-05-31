using System;
using Android.App;
using Android.OS;
using System.Threading;
using Android.Media;
using Android.Widget;
using Android.Content;
using Android.Net;
using Java.IO;
using Tcp;

//ATTENTION REVOIR PASSAGE PUT EXTRA POUR DECROCHER
namespace InterphoneSAM
{
    [Activity(Label = "En attente d'un visiteur")]
    public class WaitActivity : Activity
    {
        private bool stopThread;
        private bool keepThread;
        private Thread _updateTextToReceive;
        private Button buttonPickUp;
        private Button buttonHangUp;
        private ImageView _imageVisitor;
        private ProgressBar _pgBar;
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
            _imageVisitor = FindViewById<ImageView>(Resource.Id.imageVisitor);
            _pgBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);

            _imageVisitor.Visibility = Android.Views.ViewStates.Invisible;
            buttonPickUp.Visibility = Android.Views.ViewStates.Invisible;
            buttonPickUp.Click += new EventHandler(pickUpAction);

            buttonHangUp = FindViewById<Button>(Resource.Id.buttonHangUp);
            buttonHangUp.Visibility = Android.Views.ViewStates.Invisible;
            buttonHangUp.Click += new EventHandler(HangUpAction);

            _pgBar.Visibility = Android.Views.ViewStates.Visible;

            _updateTextToReceive = new Thread(updateTextToReceiveFunction);
            _updateTextToReceive.Start();

        }

        private void HangUpAction(object sender, EventArgs e)
        {
            mPlayer.Stop();
            MenuActivity.tcpClient.sendText("---RACCROCHE---");
            MenuActivity.tcpClient.cleanBuffer();
            keepThread = true;
            buttonHangUp.Visibility = Android.Views.ViewStates.Invisible;
            buttonPickUp.Visibility = Android.Views.ViewStates.Invisible;
            _imageVisitor.Visibility = Android.Views.ViewStates.Invisible;
            _pgBar.Visibility = Android.Views.ViewStates.Visible;
            
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
                intent.PutExtra("choice", _choice);
                StartActivity(intent);
            }
            else if (_choice == "Mal-Voyant")
            {
               
                MenuActivity.tcpClient.cleanBuffer();
                Intent intent = new Intent(this, typeof(CommunicationBlindNormal));
                intent.PutExtra("choice", _choice);
                StartActivity(intent);
            }
            else 
            {
                
                MenuActivity.tcpClient.cleanBuffer();
                Intent intent = new Intent(this, typeof(CommunicationBlindNormal));
                intent.PutExtra("choice", _choice);
                StartActivity(intent);
            }
        }

        private void ringTone()
        {
            buttonPickUp.Visibility = Android.Views.ViewStates.Visible;
            buttonHangUp.Visibility = Android.Views.ViewStates.Visible;
            _pgBar.Visibility = Android.Views.ViewStates.Invisible;
            recupererImage();
            _imageVisitor.Visibility = Android.Views.ViewStates.Visible;

            if (mPlayer != null)
            {
                mPlayer.Stop();
                mPlayer.Release();
            }

            mPlayer = MediaPlayer.Create(this, Resource.Raw.ring_0);
            mPlayer.Looping = true;
            mPlayer.Start();
        }

        private void recupererImage()
        {
            File image = new File("/storage/emulated/0/Android/data/InterphoneSAM.InterphoneSAM/files/DSC_0001.JPG");
            Thread.Sleep(2);   //Pour être certains d'avoir chargé l'image...     
            _imageVisitor.SetImageURI(Android.Net.Uri.FromFile(image));
        }
    }
}