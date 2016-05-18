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

namespace Tcp
{
    public class TCPServeur
    {
        TcpListener tcpl; //TcpListener
        NetworkStream flux; //Flux reseau
        StreamReader sr; //Flux de lecture (recupère données client)
        StreamWriter sw; //Flux d'écriture (envoie donnée client)
        public string _state { get; private set; }

        System.Net.Sockets.Socket sock; //Socket

        string _ipaddress; //Adresse ip sur laquelle écouter (IP SERVEUR)
        int _port; //Port pour la communication
        public bool isReady { get; set; }

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
            isReady = true;

            _state = "listener opened - client connected - ready";
            System.Diagnostics.Debug.WriteLine(_state);
        }

        public string receiveData()
        {
            return sr.ReadLine();
            
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