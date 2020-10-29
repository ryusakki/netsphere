using Netsphere.Client.Enums;
using Netsphere.Client.Extensions;
using Netsphere.Client.Logic;
using Netsphere.Client.Models;
using Netsphere.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Timers;

namespace Netsphere.Client.Services
{
    class Connection
    {
        public Connection()
        {
            var ip = Dns.GetHostAddresses(Dns.GetHostName()).Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).First();
            var port = new Random().Next(5000, 9000);
            Listener = new TcpListener(ip, port);
            Listener.Start();

            Task.Run(async () => await Start());
        }

        public TcpListener Listener { get; private set; }

        public async Task<ArchiveModel> Request(PeerModel peer, FileModel file)
        {
            var ip = IPEndPoint.Parse(peer.IPEndPoint);
            var client = new TcpClient();
            client.Connect(ip);

            var request = new RequestPacketModel()
            {
                File = file
            };

            ArchiveModel archive = null;
            using (var netStream = client.GetStream())
            {
                await netStream.WriteAsync(request.Serialize());

                var buffer = new byte[client.ReceiveBufferSize];
                int read = await netStream.ReadAsync(buffer);
                
                if(read > 0)
                {
                    Array.Resize(ref buffer, read);
                    var response = buffer.Deserialize<ResponsePacketModel>();
                    archive = response.Archive;
                }
            }
            return archive;
        }

        private async Task Response(TcpClient client)
        {
            using(var netStream = client.GetStream())
            {
                if (client.ReceiveBufferSize > 0)
                {
                    var buffer = new byte[client.ReceiveBufferSize];
                    int read = await netStream.ReadAsync(buffer);

                    if (read > 0)
                    {
                        Array.Resize(ref buffer, read);

                        var request = buffer.Deserialize<RequestPacketModel>();
                        var response = new ResponsePacketModel()
                        {
                            Archive = Repository.Files.Where(f => f.Hash == request.File.Hash).FirstOrDefault()
                        };

                        await netStream.WriteAsync(response.Serialize());
                        client.Close();
                    }
                }
            }
        }

        public async Task Start()
        {
            while (true)
            {
                await Listener.AcceptTcpClientAsync().ContinueWith(async task => await Response(task.Result)).ConfigureAwait(false);
            }
        }
    }
}
