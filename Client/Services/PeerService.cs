using System;
using System.IO;
using System.Security.Cryptography;
using System.Linq;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Timers;
using System.Net.Http;
using System.Net;
using System.Text.Json;
using System.Text;
using Netsphere.Client.Models;

namespace Client
{
    class PeerService : PeerModel
    {
        private TcpListener listener;
        private bool isRegistered;
        private List<PackageModel> files;
        private static readonly HttpClient Client = new HttpClient();
        private static readonly string Server = "localhost:5000";

        public PeerService()
        {
            files = new List<PackageModel>();

            var ip = Dns.GetHostAddresses(Dns.GetHostName()).Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).First();
            var port = new Random().Next(5000, 9000);
            listener = new TcpListener(ip, port);
            listener.Start();

            IPEndPoint = listener.LocalEndpoint.ToString();
            
            Timestamp = DateTime.Now;
            AvailableContent = files.SelectMany(p => p.Hash).ToList();
            RegisterRequest();
        }

        private void RegisterRequest()
        {
            var peerAsJson = JsonSerializer.Serialize(this);
            var content = new StringContent(peerAsJson, Encoding.UTF8, "application/json");
            Client.PostAsync(string.Concat(Server, "/Register"), content).Wait();
            isRegistered = true;
        }


        public async void Start()
        {
            while (true)
            {
                await listener.AcceptTcpClientAsync().ContinueWith(r =>
                {
                    var client = r.Result;
                });
            }
        }

        public enum PacketType
        {
            FileRequest = 0x5C,
            FileResponse
        }

        public class Packet
        {
            public PacketType Type { get; set; }
        }

        static void Main(string[] args)
        {
            var path = string.Concat(AppContext.BaseDirectory.Split("bin").First(), "Repository");

            var directory = new DirectoryInfo(path);
            var files = new Dictionary<string, byte[]>();

            using (var hash = SHA256.Create())
            {
                foreach(var file in directory.GetFiles())
                {
                    using (var fStream = file.Open(FileMode.Open))
                    {
                        fStream.Position = 0;
                        var fHash = hash.ComputeHash(fStream);
                        var fHashStr = BitConverter.ToString(fHash).Replace("-", string.Empty);
                        var fBytes = new byte[file.Length];

                        fStream.Read(fBytes, 0, Convert.ToInt32(file.Length));
                        files.Add(fHashStr, fBytes);
                    }
                };
            }
        }
    }
}
