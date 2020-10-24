using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Timers;

namespace Netsphere.Client.Services
{
    class ClientService
    {
        private TcpClient client;
        private static Timer Timer = new Timer(2000);


        public ClientService(TcpClient client)
        {
            this.client = client;

            //Só ocorrem quando o registro é feito
            //Timer.Elapsed += OnPing;
            //Timer.Enabled = true;
            //Timer.AutoReset = true;
        }

        private void OnPing(object o, ElapsedEventArgs args)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>(string.Empty, IPEndPoint)
            });

            for (int i = 0; i < 2; i++)
            {
              //  Client.PostAsync(string.Concat(Server, "/Ping"), content).Wait();
            }
        }

        public void Register()
        {
         //   var peer = new Peer();

         //   Client.PostAsync(string.Concat(Server, "/Register"), null).Wait();
          //  isRegistered = true;
        }

        public string IPEndPoint { get; private set; }

    }
}
