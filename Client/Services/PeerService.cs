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
using Netsphere.Shared.Models;
using Netsphere.Client.Extensions;
using System.Threading.Tasks;

namespace Client
{
    class PeerService : PeerModel
    {
        private TcpListener listener;
        private bool isRegistered;
        private List<PackageModel> localFiles;
        private static readonly HttpClient Client = new HttpClient();
        private static readonly Timer Timer = new Timer(2000);
        private readonly object Locker = new object();
        private static readonly string Server = "http://localhost:5001/Netsphere";

        public PeerService()
        {
            localFiles = LoadFiles();
            isRegistered = false;

            var ip = Dns.GetHostAddresses(Dns.GetHostName()).Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).First();
            var port = new Random().Next(5000, 9000);
            listener = new TcpListener(ip, port);
            listener.Start();

            IPEndPoint = listener.LocalEndpoint.ToString();
            Timestamp = DateTime.Now;
            Files = localFiles.Cast<FileModel>().ToList();

            Timer.Elapsed += (source, args) =>
            {
                lock(Locker)
                {
                    if(isRegistered)
                    {
                        var apiEndPoint = string.Concat(Server, "/Ping");
                        Client.PostAsJsonAsync(apiEndPoint, IPEndPoint).Wait();
                    }
                }
            };
            
            Timer.Enabled = true;
            Timer.AutoReset = true;
        }

        private List<PackageModel> LoadFiles()
        {
            var path = string.Concat(AppContext.BaseDirectory.Split("bin").First(), "Repository");
            var directory = new DirectoryInfo(path);
            return directory.GetFiles().ToList().ConvertAll(f =>
            {
                var fBytes = new byte[f.Length];
                using (var fStream = f.Open(FileMode.Open))
                {
                    fStream.Position = 0;
                    fStream.Read(fBytes, 0, Convert.ToInt32(f.Length));
                }
                return new PackageModel(f.Name, fBytes);
            });
        }

        public async Task RegisterRequest()
        {
            var apiEndPoint = string.Concat(Server, "/Register");
            isRegistered = await Client.PostAsJsonAsync(apiEndPoint, this);
        }

        public async Task<List<PeerModel>> CatalogRequest()
        {
            var apiEndPoint = string.Concat(Server, "/Catalog");
            var request = await Client.GetAsync(apiEndPoint);
            var json = await request.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<PeerModel>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task Start()
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
    }
}
