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

namespace Tcp
{
    public class TCPClient
    {
        string _ipaddress;
        int _port;
        TcpClient tcpc;
        NetworkStream flux;
        StreamReader sr;
        StreamWriter sw;

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
                
            }
            catch(System.Net.Sockets.SocketException e)
            {
                System.Diagnostics.Debug.WriteLine("SocketException: {0}", e);
            }

        }

        public void sendText(string text)
        {
            sw.WriteLine(text);
            sw.Flush();
        }


    }
}