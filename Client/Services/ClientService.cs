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
        private TcpListener listener;
        private bool isRegistered;

        private static Timer Timer = new Timer(2000);
        private static readonly HttpClient Client = new HttpClient();
        private static readonly string Server = "localhost:5000";

        public ClientService()
        {
            var ip = Dns.GetHostAddresses(Dns.GetHostName()).Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).First();
            var port = new Random().Next(5000, 9000);
            listener = new TcpListener(ip, port);
            listener.Start();

            IPEndPoint = listener.LocalEndpoint.ToString();
            isRegistered = true;

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
                Client.PostAsync(string.Concat(Server, "/Ping"), content).Wait();
            }
        }

        private void Register()
        {
            Client.PostAsync(string.Concat(Server, "/Ping"), null).Wait();
            isRegistered = true;
        }

        public string IPEndPoint { get; private set; }

        public async void Start()
        {
            while(true)
            {
                await listener.AcceptTcpClientAsync().ContinueWith(r =>
                {
                    var client = r.Result;
                });
            }
        }
    }
}
