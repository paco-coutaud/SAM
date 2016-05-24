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
using Java.Net;
using Java.IO;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Tcp
{
    public class TCPClient
    {
        private string _ipaddress;
        private int _port;
        private TcpClient tcpc;
        private NetworkStream flux;
        private StreamReader sr;
        private StreamWriter sw;
        public string phrase;
        private Thread updatePhrase;

        public TCPClient(string ipaddress, int port)
        {
            _ipaddress = ipaddress;
            _port = port;

            try
            {
                tcpc = new TcpClient(_ipaddress, _port);
                flux = tcpc.GetStream();
                sr = new StreamReader(flux);
                sw = new StreamWriter(flux);
                System.Diagnostics.Debug.WriteLine("initialize");

                updatePhrase = new Thread(updatePhraseFunction);
                updatePhrase.Start();
                
            }
            catch(System.Net.Sockets.SocketException e)
            {
                System.Diagnostics.Debug.WriteLine("SocketException: {0}", e);
            }

        }

        private void updatePhraseFunction()
        {
            //if(sr.ReadLine() != null)
            //{
            while(updatePhrase.IsAlive)
            {
                phrase = sr.ReadLine();
            }
            //}

            Thread.Sleep(2);
        }
        public string readText()
        {
            return phrase;
        }

        public void sendText(string text)
        {
            sw.WriteLine(text);
            sw.Flush();
        }


    }
}