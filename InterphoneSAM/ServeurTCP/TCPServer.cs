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
using System.IO;
using System.Threading;

namespace Tcp
{
    public class TCPServeur
    {
        private TcpListener tcpl; //TcpListener
        private NetworkStream flux; //Flux reseau
        private StreamReader sr; //Flux de lecture (recupère données client)
        private StreamWriter sw; //Flux d'écriture (envoie donnée client)
        private Thread updatePhrase;
        public string _state { get; private set; }

        private System.Net.Sockets.Socket sock; //Socket

        private string _ipaddress; //Adresse ip sur laquelle écouter (IP SERVEUR)
        private int _port; //Port pour la communication
        public bool isReady { get; set; }
        public string phrase { get; private set; }

        public TCPServeur(string ipaddress, int port)
        {
            _ipaddress = ipaddress;
            _port = port;

            tcpl = new TcpListener(IPAddress.Parse(_ipaddress), _port); //Nouveau tcp listener
            isReady = false;
            _state = "listener created";
        }

        public void start()
        {
            tcpl.Start(); //Demarre le listener (on commence à écouter)
            _state = "listener opened - waiting for client";
            System.Diagnostics.Debug.WriteLine(_state);
            sock = tcpl.AcceptSocket(); //On accepte le socket
            _state = "listener opened - client connected";
            System.Diagnostics.Debug.WriteLine(_state);

            flux = new NetworkStream(sock); //Recuperation du flux à partir du socket
            sr = new StreamReader(flux); //Instanciation du flux de lecture
            sw = new StreamWriter(flux); //Instanciation du flux d'écriture

            updatePhrase = new Thread(updatePhraseFunction);
            updatePhrase.Start();

            isReady = true;

            _state = "listener opened - client connected - ready";
            System.Diagnostics.Debug.WriteLine(_state);
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
        public string receiveData()
        {
            return phrase;
            
        }

        public void sendData(string text)
        {
            sw.WriteLine(text);
            sw.Flush();
        }

        public bool isAMessageWaiting()
        {
            if(sr.ReadLine() != "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }


}